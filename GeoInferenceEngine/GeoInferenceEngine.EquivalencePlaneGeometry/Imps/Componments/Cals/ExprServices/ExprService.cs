using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Modules;
using GeoInferenceEngine.PredicateShared.Models;
using GeoInferenceEngine.PredicateShared.Models.Exprs.MapleExprs;

namespace GeoInferenceEngine.Knowledges.Imps.Componments
{
    public class ExprService : IInferenceComponent
    {
        ZScriptInputEnginePreparer zpreparer;
        [ZDI]
        IEnginePreparer preparer;
        [ZDI]
        public FormularBase FormularBase { get; set; }
        MapleApp mapleApp { get; set; }
        public List<string> DefaultVarNames { get; set; } = new List<string>();
        public Dictionary<string, Mut> MutRefs { get; set; } = new Dictionary<string, Mut>();

        int newVarNameIndex = 0;
        public virtual void Init()
        {
            mapleApp = new MapleApp();
            Mut.Record = (mut) => MutRefs[mut.ToString()] = mut;
            zpreparer = preparer as ZScriptInputEnginePreparer;
        }
        #region 工具
        public GeoEquationInfo GetEquationInfo(GeoEquation equation)
        {

            return new GeoEquationInfo(equation, this);
        }
        public ExprTypes JudgeExprType(Expr expr)
        {
            if (expr is RealNode) { return ExprTypes.Value; }
            if (expr is MutNode) { return ExprTypes.Mut; }
            else if (expr is ProductNode product)
            {
                if (product.IsSingle && product.Single is MutNode)
                {
                    return ExprTypes.Mut;
                }
                else if (product.Multipliers.Union(product.Divisors).ToList().TrueForAll(e => e is not SumNode))
                {
                    return ExprTypes.Multiplication;
                }
                else
                {
                    return ExprTypes.Complex;
                }
            }
            else if (expr is SumNode sumNode)
            {
                if (sumNode.Addends.Union(sumNode.Subtrahends).ToList().TrueForAll(e =>
                {
                    if(e is ProductNode p)
                    {
                        return p.IsSingle;
                    }
                    else
                    {
                        return true;
                    }
                }))
                {
                    return ExprTypes.Addition;
                }
                else
                {
                    return ExprTypes.Complex;

                }
            }
            else if (expr is PowerNode powerNode)
            {
                if (powerNode.Base is MutNode)
                {
                    return ExprTypes.Multiplication;
                }
                else
                {
                    return ExprTypes.Complex;
                }
            }
            throw new NotImplementedException();
        }
        public List<Mut> FindMuts(Expr expr)
        {
            void findMuts(List<Mut> results, Expr expr)
            {
                if (expr is SumNode sum)
                {
                    foreach (Expr e in sum.Addends.Union(sum.Subtrahends))
                    {
                        if (e is MutNode mut)
                            if (!results.Contains(mut.Mut))
                                results.Add(mut.Mut);
                            else;
                        else
                            findMuts(results, e);
                    }
                }
                else if (expr is ProductNode product)
                {
                    foreach (Expr e in product.Multipliers.Union(product.Divisors))
                    {
                        if (e is MutNode mut)
                            if (!results.Contains(mut.Mut))
                                results.Add(mut.Mut);
                            else;
                        else
                            findMuts(results, e);
                    }
                }
                else if (expr is PowerNode power)
                {
                    if (power.Base is MutNode mut)
                        if (!results.Contains(mut.Mut))
                            results.Add(mut.Mut);
                        else;
                    else
                        findMuts(results, power.Base);
                }
            }
            List<Mut> result = new List<Mut>();
            findMuts(result, expr);
            return result;
        }
        public Dictionary<Mut, Expr> FindCoffs(Expr expr)
        {
            if (expr is ProductNode product)
            {
                Dictionary<Mut, Expr> result = new Dictionary<Mut, Expr>();
                var p = FindCoffs(product);
                result.Add(p.Item1, p.Item2);
                return result;
            }
            else if (expr is SumNode sum)
            {
                return FindCoffs(sum);
            }
            else if (expr is MutNode mut)
            {
                Dictionary<Mut, Expr> result = new Dictionary<Mut, Expr>();
                result.Add(mut.Mut, 1);
                return result;
            }
            throw new ArgumentException();
        }
        public (Mut, Expr) FindCoffs(ProductNode expr)
        {
            if (expr.IsSingle)
            {
                return expr.IsPositive ? (((MutNode)expr.Single).Mut, expr.Constant) : (((MutNode)expr.Single).Mut, expr.Constant.Opposite());
            }
            else
            {
                throw new ArgumentException();
            }
        }
        public Dictionary<Mut, Expr> FindCoffs(SumNode expr)
        {
            Dictionary<Mut, Expr> result = new Dictionary<Mut, Expr>();
            foreach (var item in expr.Addends)
            {
                if (item is MutNode m)
                {
                    result.Add(m.Mut, 1);
                }
                else if (item is ProductNode p)
                {
                    if (p.IsSingle && p.Single is MutNode pm)
                    {
                        result.Add(pm.Mut, p.Constant);
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
            foreach (var item in expr.Subtrahends)
            {
                if (item is MutNode m)
                {
                    result.Add(m.Mut, -1);
                }
                else if (item is ProductNode p)
                {
                    if (p.IsSingle && p.Single is MutNode pm)
                    {
                        result.Add(pm.Mut, p.Constant.Opposite());
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
            return result;
        }
        public Dictionary<Mut, Expr> FindExponents(ProductNode expr)
        {
            Dictionary<Mut, Expr> result = new Dictionary<Mut, Expr>();
            foreach (var item in expr.Multipliers)
            {
                if (item is MutNode m)
                {
                    result.Add(m.Mut, 1);
                }
                else if (item is PowerNode p)
                {
                    if (p.Base is MutNode pm)
                    {
                        result.Add(pm.Mut, p.Exponent);
                    }
                }
            }
            foreach (var item in expr.Divisors)
            {
                if (item is MutNode m)
                {
                    result.Add(m.Mut, -1);
                }
                else if (item is PowerNode p)
                {
                    if (p.Base is MutNode pm)
                    {
                        result.Add(pm.Mut, p.Exponent.Opposite());
                    }
                }
            }
            return result;
        }
        public Expr BetterSimplify(Expr expr)
        {
            var result = mapleApp.Run($"simplify({ToString()})");
            return zpreparer.parseExpr(result);
        }
        #endregion
    }
}
