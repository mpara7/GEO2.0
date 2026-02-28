using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.CalByCombination.GeoCalHandler;
using GeoInferenceEngine.PlaneKnowledges.Engine.Comps.GeoCal;
using GeoInferenceEngine.PredicateShared.Imps.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.OutputMakers
{
    [Description("高斯矩阵结果生成器")]
    public class FormularMatrixOutputMaker : IInferenceOutputMaker<FormularMatrixOutput>
    {
        [ZDI]
        AppInfo AppInfo { get; set; }
        [ZDI]
        ZDIContainer ZDIContainer;
        string name;
        public string Name { get => name; set => name = value; }

        public FormularMatrixOutput Make()
        {
            FormularMatrixOutput output = new FormularMatrixOutput();
            if (ZDIContainer.HasSet<CalHanderStrategy>())
            {
                CalHanderStrategy gaussianElimination = ZDIContainer.Get<CalHanderStrategy>();
                //确定变量数量
                foreach (var dict in gaussianElimination.disMatrixList)
                {
                    foreach (var item in dict)
                    {
                        if (!output.MutNames.Contains(item.Key.ToString()))
                        {
                            output.MutNames.Add(item.Key.ToString());
                        }
                    }
                }
                output.MutNames.Add("常数");

                //生成
                for (int i = 0; i < gaussianElimination.disMatrixList.Count; i++)
                {
                    List<string> exprs = new List<string>();
                    foreach (var item in output.MutNames)
                    {
                        exprs.Add("");
                    }
                    foreach (var item in gaussianElimination.disMatrixList[i])
                    {
                        var index = output.MutNames.IndexOf(item.Key.ToString());
                        exprs[index] = item.Value.ToString();
                    }
                    exprs[exprs.Count-1]=gaussianElimination.disConstList[i].ToString();
                    output.Values.Add(exprs);
                }
            }
            else
            {
                AppInfo.WarningInfo.Warnings.Add("高斯矩阵结果生成器：推理组件不包含高斯矩阵");
            }
            return output;
        }
    }
}
