using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.CalExecutors.GeoCalHandler;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal
{
    class RatioInfoKnowledgeMaker
    {
        RatioInfo RatioInfo { get; set; }
        int LastIndex=0;
        public RatioInfoKnowledgeMaker(RatioInfo ratioInfo) 
        {
            RatioInfo=ratioInfo;
        }
        public IEnumerable<Knowledge> MakeNew()
        {
            List<Knowledge> newKnowledges = new List<Knowledge>();
            var mutList = RatioInfo.CoffDict.Keys.ToList();

            for (int i = 0; i < mutList.Count; i++)
            {
                for (int j = i+1; j < mutList.Count; j++)
                {
                    var mut1 = mutList[i] as GeoProp;
                    var mut2 = mutList[j] as GeoProp;

                    if (mut1!= null&& mut2 != null)
                    {
                        if (mut1.PropName == mut2.PropName)
                        {
                            var type1 = mut1.Knowledge.GetType();
                            var type2 = mut1.Knowledge.GetType();
                            if (type1 == type2)
                            {
                                var reasons= RatioInfo.SimpleFindReason(mut1, mut2);
                                if (type1 == typeof(Segment)&&mut1.PropName==GeoProp.Length)
                                {
                                    Expr ratio = RatioInfo.CoffDict[mut2].Clone() / RatioInfo.CoffDict[mut1];
                                    if (ratio == 1)
                                    {
                                        SegmentLengthEqual pred = new SegmentLengthEqual((Segment)mut1.Knowledge, (Segment)mut2.Knowledge);
                                        pred.AddCondition(reasons);
                                        pred.AddReason();
                                        newKnowledges.Add(pred);
                                    }
                                    else
                                    {
                                        SegmentLengthRatio pred = new SegmentLengthRatio((Segment)mut1.Knowledge, (Segment)mut2.Knowledge,ratio);
                                        pred.AddCondition(reasons);
                                        pred.AddReason();
                                        newKnowledges.Add(pred);
                                    }
                                }
                                if (type1 == typeof(Angle) && mut1.PropName == GeoProp.Size)
                                {
                                    Expr ratio = RatioInfo.CoffDict[mut2].Clone() / RatioInfo.CoffDict[mut1];
                                    if (ratio == 1)
                                    {
                                        AngleSizeEqual pred = new AngleSizeEqual((Angle)mut1.Knowledge, (Angle)mut2.Knowledge);
                                        pred.AddCondition(reasons);
                                        pred.AddReason();
                                        
                                        newKnowledges.Add(pred);
                                    }
                                    else
                                    {
                                        AngleSizeRatio pred = new AngleSizeRatio((Angle)mut1.Knowledge, (Angle)mut2.Knowledge, ratio);
                                        pred.AddCondition(reasons);
                                        pred.AddReason();
                                        newKnowledges.Add(pred);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            LastIndex=mutList.Count;
            return newKnowledges;
        }

    }
    [Description("只涉及传递性的简单计算")]
    internal class SimpleCalExecutor : ACalExecutor
    {
        [ZDI]
        KnowledgeAddProcessor AddProcessor { get; set; }
        [ZDI]
        FormularBase FormularBase { get; set; }
        // ================== 1. 注入我们的贪心消元器 ==================
        // 注意：如果你放在了不同的命名空间，记得在文件最上方加上 using
        [ZDI]
        public SREEGreedyElimination _sreeGreedyElimination { get; set; }


        public override void Init()
        {
        }
        int LastIndex {  get; set; }
        Dictionary<RatioInfo, RatioInfoKnowledgeMaker> makerDict = new Dictionary<RatioInfo, RatioInfoKnowledgeMaker>();
        public override void Do()
        {
            CheckRatioInfos();
            MakeNewKnowledges();
            // ================== 2. 执行贪心消元 ==================
            // 在系统完成简单的比例传递、并可能生成了新的 SREE 之后，
            // 立刻执行贪心扫描，把长等式吃掉化简，并转换为 GeoEquation！
            if (_sreeGreedyElimination != null)
            {
                _sreeGreedyElimination.ExecuteElimination();
            }
            // =====================================================

        }
        public void CheckRatioInfos()
        {
            foreach (var ratio in FormularBase.DistanceRatioInfos)
            {
                if (!makerDict.ContainsKey(ratio))
                {
                    RatioInfoKnowledgeMaker maker = new RatioInfoKnowledgeMaker(ratio);
                    makerDict.Add(ratio, maker);
                }
            }
            foreach (var ratio in FormularBase.AngleRatioInfos)
            {
                if (!makerDict.ContainsKey(ratio))
                {
                    RatioInfoKnowledgeMaker maker = new RatioInfoKnowledgeMaker(ratio);
                    makerDict.Add(ratio, maker);
                }
            }
        }
        public void MakeNewKnowledges()
        {
            List<Knowledge> knowledges = new List<Knowledge>();
            foreach (var item in makerDict)
            {
                knowledges.AddRange(item.Value.MakeNew());
            }
            foreach (var item in knowledges)
            {
                AddProcessor.Add(item);
            }
        }
    }
}
