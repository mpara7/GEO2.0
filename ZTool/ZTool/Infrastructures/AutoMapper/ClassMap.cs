using System.Collections;
using System.Reflection;

namespace ZTool.Infrastructures.AutoMapper
{
    public class ClassMap : AMap
    {
        #region 配置
        public List<PropertyInfo> NotConfigPropMaps = new();
        public List<(PropertyInfo fromProp, PropertyInfo toProp)> ConfigPropMaps = new();
        public Action<object, object> Final { get; set; }
        #endregion
        #region 映射
        public override void Pump(object fromObj, object toObj)
        {
            var usingPropMaps = ComfirmMap(fromObj, toObj);
            usingPropMaps.AddRange(ConfigPropMaps);
            usingPropMaps = usingPropMaps.Where(m => !NotConfigPropMaps.Exists(prop => m.toProp.Name == prop.Name)).ToList();
            DoMap(usingPropMaps, fromObj, toObj);
        }
        public override object Map(object fromObj)
        {
            var toObj = ZMapper.CreatNewFunc(ToType);
            Pump(fromObj, toObj);
            return toObj;
        }

        List<(PropertyInfo fromProp, PropertyInfo toProp)> ComfirmMap(object fromObj, object toObj)
        {
            List<(PropertyInfo, PropertyInfo)> propMaps = new List<(PropertyInfo, PropertyInfo)>();
            var fromMembers = fromObj.GetType().GetProperties().ToDictionary(m => m.Name.Replace("_", "").ToLower(), m => m);
            var toMembers = toObj.GetType().GetProperties().ToDictionary(m => m.Name.Replace("_", "").ToLower(), m => m);
            //根据相似命名自动收集映射关系
            foreach (var kv in toMembers)
            {
                PropertyInfo toMemberInfo = kv.Value;
                if (fromMembers.ContainsKey(kv.Key))
                {
                    PropertyInfo fromMemberInfo = fromMembers[kv.Key];
                    propMaps.Add((fromMemberInfo, toMemberInfo));
                }
            }
            return propMaps;
        }
        void DoMap(List<(PropertyInfo fromProp, PropertyInfo toProp)> propMaps, object fromObj, object toObj)
        {
            foreach (var propPair in propMaps)
            {
                var fromMemberInfo = propPair.fromProp;
                var toMemberInfo = propPair.toProp;
                //列表特殊处理
                if (fromMemberInfo.PropertyType.IsGenericType && toMemberInfo.PropertyType.IsGenericType)
                {
                    if (fromMemberInfo.PropertyType.IsAssignableTo(typeof(IList)) &&
                        toMemberInfo.PropertyType.IsAssignableTo(typeof(IList)))
                    {
                        var innerFromType = fromMemberInfo.PropertyType.GetGenericArguments()[0];
                        var innerToType = toMemberInfo.PropertyType.GetGenericArguments()[0];

                        dynamic fromList = fromMemberInfo.GetValue(fromObj);
                        dynamic toList = ZMapper.CreatNewFunc(toMemberInfo.PropertyType);
                        if (fromList != null)
                        {
                            foreach (var fromValue in fromList)
                            {
                                if (innerFromType == innerToType)
                                {
                                    toList.Add(fromValue);
                                }
                                else
                                {
                                    var toValue = ZMapper.Map(fromValue, innerFromType, innerToType);
                                    toList.Add(toValue);
                                }
                            }
                        }
                        toMemberInfo.SetValue(toObj, toList);
                        continue;
                    }
                }



                if (toMemberInfo.PropertyType == fromMemberInfo.PropertyType)
                {
                    toMemberInfo.SetValue(toObj, fromMemberInfo.GetValue(fromObj));
                }
                else
                {
                    var fromValue = fromMemberInfo.GetValue(fromObj);
                    var toValue = ZMapper.Map(fromValue, fromMemberInfo.PropertyType, toMemberInfo.PropertyType);
                    toMemberInfo.SetValue(toObj, toValue);
                }
            }
        }
        #endregion

    }
}
