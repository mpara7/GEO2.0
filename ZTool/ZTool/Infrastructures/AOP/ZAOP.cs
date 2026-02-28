using System.Reflection;
using System.Reflection.Emit;

using Throw;

namespace ZTool.Infrastructures.AOP;
/// <summary>
/// 动态构建AOP
/// 将继承InvokeAttribute的特性打在虚方法上
/// 基于类型的就不做对象了
/// </summary>
public class ZAOP
{
    public static Func<Type, object> CreateAttriFunc;
    /// <summary>
    /// 获取TypeBuilder
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static TypeBuilder CreateTypeBuilder(Type type)
    {
        //定义程序集的名称
        AssemblyName aName = new AssemblyName("DynamicAssembly");

        // 创建一个程序集构建器
        // Framework应该这样：AppDomain.CurrentDomain.DefineDynamicAssembly
        AssemblyBuilder ab =
            AssemblyBuilder.DefineDynamicAssembly(
                aName,
                AssemblyBuilderAccess.Run);
        // 使用程序集构建器创建一个模块构建器
        ModuleBuilder mb = ab.DefineDynamicModule(aName.Name + ".dll");
        // 使用模块构建器创建一个类型构建器
        TypeBuilder tb = mb.DefineType(type.Name + "_ztool", type.Attributes, type);
        return tb;
    }
    static void DefineCtor(Type type, TypeBuilder tb)
    {
        foreach (var ctor in type.GetConstructors())
        {
            var method = tb.DefineConstructor(ctor.Attributes, ctor.CallingConvention, ctor.GetParameters().Select(p => p.ParameterType).ToArray());
            var IL = method.GetILGenerator();
            for (int i = 0; i < ctor.GetParameters().Length + 1; i++)
            {
                IL.Emit(OpCodes.Ldarg_S, i);
            }
            IL.Emit(OpCodes.Call, ctor);
            IL.Emit(OpCodes.Ret);
        }
    }

    /// <summary>
    /// 检索方法以及需要附加的特性
    /// </summary>
    /// <param name="enrichTypes"></param>
    /// <returns></returns>
    static Dictionary<MethodInfo, List<Attribute>> GetForOverrideMethods(Type enrichType)
    {
        Dictionary<MethodInfo, List<Attribute>> anyDict = new();

        foreach (var methodInfo in enrichType.GetMethods())
        {
            if (methodInfo.IsVirtual == false)
                continue;
            anyDict.Add(methodInfo, new List<Attribute>());
            foreach (var attribute in methodInfo.GetCustomAttributes())
            {
                if (attribute is InvokerAttribute m)
                {
                    anyDict[methodInfo].Add(m);
                }
            }
        }
        return anyDict.Where(kv => kv.Value.Count > 0).ToDictionary(kv => kv.Key, kv => kv.Value);
    }
    static void OverrideMethods(Dictionary<MethodInfo, List<Attribute>> keyValuePairs, TypeBuilder tb)
    {
        foreach (var kv in keyValuePairs)
        {
            var orgMethod = kv.Key;
            var mb = tb.DefineMethod(orgMethod.Name, orgMethod.Attributes, orgMethod.ReturnType, orgMethod.GetParameters().Select(p => p.ParameterType).ToArray());
            tb.DefineMethodOverride(mb, kv.Key);
            var IL = mb.GetILGenerator();

            if (orgMethod.ReturnType == typeof(void))
            {
                MethodBuild.ConstructAction(tb, IL, kv.Value, orgMethod);
            }
            else
            {
                MethodBuild.ConstructFunc(tb, IL, kv.Value, orgMethod);
            }
        }
    }

    /// <summary>
    /// 构建类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type BuildType(Type type)
    {
        type.Throw(() => new ArgumentException($"{type} 不是公共类型,无法继承")).IfFalse(t => t.IsPublic);
        var tb = CreateTypeBuilder(type);
        DefineCtor(type, tb);
        var methodInfos = GetForOverrideMethods(type);
        OverrideMethods(methodInfos, tb);
        return tb.CreateType();
    }
    /// <summary>
    /// 动态构建对象
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object Build(Type type, params object[] args)
    {
        type.Throw(() => new ArgumentException($"{type} 无法继承")).IfTrue(t => t.IsSealed);
        type.Throw(() => new ArgumentException($"{type} 不是公共类型,无法继承")).IfFalse(t => t.IsPublic);
        var t = BuildType(type);
        return Activator.CreateInstance(t, args);
    }
    /// <summary>
    /// 动态构建对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public static T Build<T>(params object[] args)
    {
        return (T)Build(typeof(T), args);
    }
}
