using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.PredicateShared.Models;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases
{
    public abstract class Target 
    {
        public int Index { get; set; }
        public bool IsSuccess { get; set; }
        public Knowledge Conclusion { get; set; }
    }
    public abstract class ProveTarget : Target { }
    public abstract class SolveTarget:Target { }
    
    #region 证明目标
    public class KnowledgeTarget: ProveTarget
    {
        public Knowledge Target { get; set; }
        public override string ToString()=>$"Prove：{Target}";
    }
    public class EquationTarget : ProveTarget
    {
        public GeoEquation Target { get; set; }
        public GeoEquationInfo TargetInfo { get; set; }
        public override string ToString() => $"Prove：{Target}";
    }
    #endregion
    #region 求解目标
    public class VarTarget : SolveTarget
    {
        public Var Target { get; set; }
        public override string ToString() => $"Solve：{Target}";
    }
    public class GeoPropTarget :SolveTarget
    {
        public GeoProp Target { get; set; }
        public override string ToString() => $"Solve：{Target}";
    }
    public class ExprTarget : SolveTarget
    {
        /// <summary>
        /// 变量或者几何属性表达式
        /// </summary>
        public Expr Target { get; set; }
        public override string ToString() => $"Solve：{Target}";
    }
    public class RelationTarget : SolveTarget
    {
        /// <summary>
        /// 变量或者几何属性表达式
        /// </summary>
        public Knowledge Target { get; set; }
        public override string ToString() => $"Solve：{Target}";
    }
    #endregion


    [Description("求解目标库")]
    public class TargetBase : IInferenceDatabase
    {
        /// <summary>
        /// 求证
        /// </summary>
        public List<ProveTarget> ToProves { get; set; } = new();

        public List<KnowledgeTarget> KnowledgeTargetInfos { get; set; } = new(); 
        public List<EquationTarget> EquationTargetInfos { get; set; } = new();

        /// <summary>
        /// 求解
        /// </summary>
        public List<SolveTarget> ToSolves { get; set; } = new();
        public List<VarTarget> VarTargets { get; set; } = new();
        public List<GeoPropTarget> GeoPropTargets { get; set; } = new();
        public List<ExprTarget> ExprTargets { get; set; } = new();
        public List<RelationTarget> RelationTargets { get; set; } = new();

        ///// <summary>
        ///// 求证伪
        ///// </summary>
        //public List<KnowledgeTarget> ToDisproves { get; set; } = new();


        ///// <summary>
        ///// 满足条件 类似当线段xy取最小值 ulong去掉前十位来比较
        ///// </summary>
        //public List<KnowledgeTarget> ToSatisfy { get; set; } = new();

        ///// <summary>
        ///// 求确定
        ///// </summary>
        //public Dictionary<int, (Knowledge toDetermine, bool determined)> ToDetermine { get; set; } = new();
    }
}
