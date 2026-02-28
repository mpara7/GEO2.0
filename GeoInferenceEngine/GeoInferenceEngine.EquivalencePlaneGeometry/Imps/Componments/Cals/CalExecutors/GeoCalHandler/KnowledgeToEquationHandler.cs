//using GeoInferenceEngine.Abstractions.EngineInterfaces;
//using GeoInferenceEngine.PredCommonImp.Engine.DataBases;

//namespace GeoInferenceEngine.PlaneKnowledges.Engine.Comps.GeoCal;

//public class KnowledgeToEquationHandler : IInferenceComponent
//{
//    [ZDI]
//    private KnowledgeBase knowledgeBase;
//    public KnowledgeToEquationHandler() { }

//    //由知识生成等式
//    public void KnowledgeBaseToEquation()
//    {
//        //获取线段长度
//        if (knowledgeBase.Categories.ContainsKey(typeof(SegmentLength)))
//        {
//            foreach (SegmentLength segmentLength in knowledgeBase.Categories[typeof(SegmentLength)])
//            {
//                var segment = (Segment)segmentLength.Properties[0];
//                var value = segmentLength.Exprs[0];
//                segment.Length.Value = value;
//                segment.Length.HasValue = true;
//            }
//        }
//        //获取角的大小
//        if (knowledgeBase.Categories.ContainsKey(typeof(AngleSize)))
//        {
//            foreach (AngleSize angleSize in knowledgeBase.Categories[typeof(AngleSize)])
//            {
//                var angle = (Angle)angleSize.Properties[0];
//                var value = angleSize.Exprs[0];
//                angle.Size.Value = value;
//                angle.Size.HasValue = true;
//            }
//        }

//        //获取面积大小
//        //...各种大小

//        //获取比值

//        //获取线段长度相等
//        //...各种相等
//    }
//}
