using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.Predicates.Relations.PlainFigureRelations
{
    [OrdinaryKnowledge]
    [Description("点在圆内")]
    public class PointInCircle : PlainRelation
    {
        public PointInCircle(Point point, Circle circle)
        {
            Add(point, circle);
            Normalize();
            SetHashCode();
        }

        public Point Point => (Point)Properties[0];
        public Circle Circle => (Circle)Properties[1];

        public override string ToString() => $"{Point}在{Circle}内";

        public override void Normalize()
        {
        }
    }
}
