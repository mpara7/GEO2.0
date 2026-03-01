using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases
{
    [Description("知识库")]
    public class KnowledgeBase : IInferenceDatabase
    {
        public List<(Point, Point)> Connections { get; set; } = new();
        public Dictionary<Point, (double X, double Y, double Z)> PseudoCoords { get; set; } = new();


        /// <summary>
        /// 分类知识库，string:类型名字
        /// </summary>
        public Dictionary<Type, List<Knowledge>> Categories { get; set; } = new()
        {
            {typeof(Point),new () },
            {typeof(Segment),new () }, 
            {typeof(Line),new () },
            {typeof(Angle),new () },

            {typeof(Triangle),new () },
            {typeof(Quadriliateral),new () },

            {typeof(Arc),new () },
            {typeof(Sector),new () },
            {typeof(Circle),new () },
            {typeof(SegmentLengthRatio),new () },
            {typeof(SLR),new () },
            {typeof(SREE),new () },
        };

        /// <summary>
        /// 不成立的知识
        /// </summary>
        public Dictionary<ulong, NegatingKnowledge> ConstructiveKnowledges { get; set; } = new();
        /// <summary>
        /// 不成立的知识
        /// </summary>
        public Dictionary<ulong, NegatingKnowledge> NegaKnowledges { get; set; } = new();
        /// <summary>
        /// 可用知识库
        /// </summary>
        public Dictionary<ulong, Knowledge> Knowledges { get; set; } = new();
        /// <summary>
        /// 不可用（已过时）知识库
        /// </summary>
        public Dictionary<ulong, Knowledge> UnAvalibleKnowledges { get; set; } = new();


        #region 获取几何对象

        /// <summary>
        /// 泛型获取某类型的所有知识对象 (二次开发补充)
        /// </summary>
        public List<T> GetKnowledgesByType<T>() where T : Knowledge
        {
            if (Categories.ContainsKey(typeof(T)))
            {
                // 将 List<Knowledge> 强转为指定的子类 List<T>
                return Categories[typeof(T)].Cast<T>().ToList();
            }
            return new List<T>();
        }
        public virtual bool HasSegment(Knowledge p1, Knowledge p2)
        {
            Segment segment = new Segment((Point)p1, (Point)p2);
            if (Knowledges.ContainsKey(segment.HashCode))
                return true;
            return false;
        }
        public virtual bool HasAllSegment(params Point[] points)
        {
            var pairs = ListTool.GetCombination(points.ToList(), 2);

            return pairs.TrueForAll(p => HasSegment(p[0], p[1]));
        }
        public virtual Point GetPoint(string name)
        {
            Point pred = new Point(name);
            if (Knowledges.ContainsKey(pred.HashCode))
                return (Point)Knowledges[pred.HashCode];
            return null;
        }
        public virtual Segment GetSegment(Knowledge p1, Knowledge p2)
        {
            Segment segment = new Segment((Point)p1, (Point)p2);
            if (Knowledges.ContainsKey(segment.HashCode))
                return (Segment)Knowledges[segment.HashCode];
            return null;

            foreach (var pred in Categories[typeof(Segment)])
            {
                if (pred.Properties.Contains(p1) && pred.Properties.Contains(p2))
                    return (Segment)pred;
            }
            return null;
        }
        public virtual Line GetLine(Knowledge p1, Knowledge p2)
        {
            var lines = Categories[typeof(Line)].Where(l => l.IsAvailable);
            foreach (var pred in lines)
            {
                if (pred.Properties.Contains(p1) && pred.Properties.Contains(p2))
                    return (Line)pred;
            }
            return null;
        }
        public virtual Line GetLine(params Knowledge[] ps)
        {
            var lines = Categories[typeof(Line)].Where(l => l.IsAvailable);
            foreach (var pred in lines)
            {
                if (ps.ToList().TrueForAll(pred.Properties.Contains))
                    return (Line)pred;
            }
            return null;
        }
        public virtual bool HasColine(params Knowledge[] points)
        {
            Line lineTemplete = new Line(points.Select(p => (Point)p).ToArray());
            List<Knowledge> lines = new();
            foreach (var line in Categories[typeof(Line)])
            {
                if (points.ToList().TrueForAll(p => line.Properties.Contains(p)))
                    return true;
            }
            return false;
        }
        public virtual Knowledge GetColine(params Knowledge[] points)
        {
            Line lineTemplete = new Line(points.Select(p => (Point)p).ToArray());
            List<Knowledge> lines = new();
            foreach (var line in Categories[typeof(Line)])
            {
                if (points.ToList().TrueForAll(p => line.Properties.Contains(p)))
                    return (Line)line;
            }
            //取不成立的知识
            if (points.Count() == 3)
            {
                return Knowledges[~lineTemplete.HashCode];
            }
            else
            {
                throw new Exception();
            }
        }
        public virtual Angle GetAngle(Knowledge p1, Knowledge v, Knowledge p2)
        {
            var list = Categories[typeof(Angle)].Where(a => a.IsAvailable).Where(a=>((Angle)a).Vertex==v).ToList();
            foreach (Angle angle in list)
            {
                if ((angle.Edge1.Contains(p1) && angle.Edge2.Contains(p2) || angle.Edge2.Contains(p1) && angle.Edge1.Contains(p2)))
                    return angle;
            }
            return null;
        }
        //找到到当前线段是否已经存在中点
        public virtual Midpoint GetMidPoint(Point p1, Point p2)
        {
            foreach (var pred in Categories[typeof(Midpoint)])
            {
                if ((pred.Properties[1] == p1 && pred.Properties[2] == p2 ) || (pred.Properties[1] == p2 && pred.Properties[2] == p1))
                    return (Midpoint)pred;
            }
            return null;
        }
        public virtual Triangle GetTriangle(Point p1, Point p2, Point p3)
        {
            foreach (var pred in Categories[typeof(Triangle)])
            {
                if (pred.Properties.Contains(p1) && pred.Properties.Contains(p2) && pred.Properties.Contains(p3))
                    return (Triangle)pred;
            }
            return null;
        }
        public virtual Quadriliateral GetQuadriliateral(Point p1, Point p2, Point p3, Point p4)
        {
            foreach (var pred in Categories[typeof(Quadriliateral)])
            {
                if (pred.Properties.Contains(p1) && pred.Properties.Contains(p2) && pred.Properties.Contains(p3) && pred.Properties.Contains(p4))
                    return (Quadriliateral)pred;
            }
            return null;
        }

        public virtual AngleSizeEqual GetAngleSizeEqual(Angle angle1, Angle angle2)
        {
            foreach (var pred in Categories[typeof(AngleSizeEqual)])
            {
                if ((pred.Properties[0] == angle1 && pred.Properties[1] == angle2) || (pred.Properties[0] == angle2&& pred.Properties[1] == angle1))
                    return (AngleSizeEqual)pred;
            }
            return null;
        }

        public virtual SegmentLengthEqual GetSegmentLengthEqual(Segment seg1, Segment seg2)
        {
            foreach (var pred in Categories[typeof(SegmentLengthEqual)])
            {
                if ((pred.Properties[0] == seg1 && pred.Properties[1] == seg2) || (pred.Properties[0] == seg2 && pred.Properties[1] == seg1))
                    return (SegmentLengthEqual)pred;
            }
            return null;
        }

        public virtual SegmentLengthRatio GetSegmentLengthRatio(Segment seg1, Segment seg2)
        {


            foreach (var pred in Categories[typeof(SegmentLengthRatio)])
            {
                if ((pred.Properties[0] == seg1 && pred.Properties[1] == seg2) ||
                    (pred.Properties[0] == seg2 && pred.Properties[1] == seg1))
                {
                    return (SegmentLengthRatio)pred;
                }
            }
            return new SegmentLengthRatio(seg1, seg2, 99);
        }

        public virtual SLR GetSegmentLengthRatio1(Point p1, Point p2, Point p3)
        {


            foreach (var pred in Categories[typeof(SLR)])
            {
                if ((pred.Properties[0] == p1 && pred.Properties[1] == p2) && pred.Properties[2] == p3)
                {
                    return (SLR)pred;
                }
            }
            return new SLR(p1, p2,p3, 99);
        }

        

        #endregion

        #region 获取平凡知识
        public virtual Knowledge GetJudgeIpsilateral(Point point1, Point point2, Point point3, Point point4)
        {
            var a = new PointsOnLineSameSide(point1, point2, point3, point4);
            if (Knowledges.ContainsKey(a.HashCode))
                return Knowledges[a.HashCode];
            var b = new PointsOnLineDifferentSide(point1, point2, point3, point4);
            if (Knowledges.ContainsKey(b.HashCode))
                return Knowledges[b.HashCode];
            return null;
        }
        public virtual Knowledge GetPointWithin(Point point1, Point point2, Point point3)
        {
            var a = new PointWithInPoints(point1, point2, point3);
            if (Knowledges.ContainsKey(a.HashCode))
                return Knowledges[a.HashCode];
            return null;
        }
        #endregion
    }
}
