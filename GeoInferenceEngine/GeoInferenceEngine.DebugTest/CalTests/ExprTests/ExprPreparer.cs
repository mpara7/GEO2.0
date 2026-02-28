using GeoInferenceEngine.Knowledges.Imps.Componments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs.Expr;
namespace GeoInferenceEngine.DebugTest.CalTests.ExprTests
{
    internal class ExprPreparer
    {
        public static bool IsUseMaple { get; set; } = true;
        public static ExprService ExprService { get; set; }
        public static void Init()
        {
            if (IsUseMaple)
            {
                ExprService  = new ExprService();
                ExprService.Init();
            }
            else
            {
                ExprService = new ExprService();
                ExprService.Init();
            }
        }
    }
}
