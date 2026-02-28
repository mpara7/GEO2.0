using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models;
using GeoInferenceEngine.Knowledges;
using System.Reflection;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs.PairMakers
{
    [Description("全排列与全组合组队器设置")]
    public class RoundPairMakerConfig : AInferenceSetting
    {
        /// <summary>
        /// 单轮中最大组队次数限制
        /// </summary>
        public int MaxRoundPairNum { get; set; } = 100000;
    }
    /// <summary>
    /// 表示使用全组合（默认全排列）
    /// </summary>
    public class CombinationAttribute : Attribute { }

    [Description("全排列与全组合组队器")]
    public class PairMaker : IInferenceComponent
    {
        public int Round { get; set; }
        public virtual void Init() { }
        public virtual void Release() { }
        [ZDI]
        EngineInfo ERBase { get; set; }
        [ZDI]
        RoundPairMakerConfig config;
        [ZDI]
        Logger logger;
        [ZDI]
        KnowledgeBase knowledgeBase;
        [ZDI]
        RuleBase ruleBase;
        Dictionary<string, PairMakerBase> historyPairMakers { get; set; } = new();
        List<PairMakerBase> usingPairMakers { get; set; } = new List<PairMakerBase>();
        List<PairMakerBase> runnablePairMakers { get; set; } = new List<PairMakerBase>();
        Queue<(RuleInfo rule, Knowledge[] args)> remainPairs = new();
        public void Reload()
        {
            Round = 0;
            //保存历史
            foreach (var info in usingPairMakers)
            {
                if (!historyPairMakers.ContainsKey(info.RuleInfo.Name))
                {
                    historyPairMakers.Add(info.RuleInfo.Name, info);
                }
            }
            usingPairMakers.Clear();
            runnablePairMakers.Clear();
            foreach (var ruleInfo in ruleBase.RuleInfos)
            {
                if (historyPairMakers.ContainsKey(ruleInfo.Name))
                {
                    usingPairMakers.Add(historyPairMakers[ruleInfo.Name]);
                }
                else
                {
                    var attri = ruleInfo.RuleMethod.GetCustomAttribute<CombinationAttribute>();
                    if (attri is null)
                    {
                        PermutationPairMaker pairMaker = new PermutationPairMaker() { RuleInfo = ruleInfo };
                        usingPairMakers.Add(pairMaker);
                    }
                    else
                    {
                        CombinationPairMaker pairMaker = new CombinationPairMaker() { RuleInfo = ruleInfo };
                        usingPairMakers.Add(pairMaker);
                    }
                }
            }
        }
        public (MethodInfo rule, PlaneRuleClass @class, Knowledge[] args) GetNextPair()
        {
            var a = remainPairs.Dequeue();

            return (a.rule.RuleMethod, a.rule.RuleClass, a.args.ToArray());
        }

        public bool isRoundEnd = true;
        int needRoundPairNum = 0;
        private int curRuleRuingInfoIndex;

        public bool HasNextPair()
        {
            if (remainPairs.Count > 0)
            {
                return true;
            }
            else
            {

                if (isRoundEnd)
                    checkAndUpdatePairMaker();
                needRoundPairNum = config.MaxRoundPairNum;
                makePairs();
                Round++;
                logger.Info($"第{Round}轮组队完成，共{remainPairs.Count}队");

                return remainPairs.Count > 0;
            }
        }
        private void checkAndUpdatePairMaker()
        {
            foreach (var item in usingPairMakers.Where(i => i.IsRunnable))
            {
                if (!runnablePairMakers.Contains(item))
                {
                    runnablePairMakers.Add(item);
                }
            }

            foreach (var ruleRunningInfo in usingPairMakers.Where(i => !i.IsRunnable))
            {
                ruleRunningInfo.CheckIfIsRunnable(knowledgeBase);
                if (ruleRunningInfo.IsRunnable)
                {
                    runnablePairMakers.Add(ruleRunningInfo);
                }
            }
            foreach (var ruleRunningInfo in runnablePairMakers)
            {
                ruleRunningInfo.UpdatePositions();
            }
            curRuleRuingInfoIndex = 0;
            needRoundPairNum = config.MaxRoundPairNum;
            isRoundEnd = false;
        }
        private void makePairs()
        {
            for (; curRuleRuingInfoIndex < runnablePairMakers.Count;)
            {

                var ruleInfo = runnablePairMakers[curRuleRuingInfoIndex];
                if (ruleInfo.IsMadeAllPairs)
                {
                    curRuleRuingInfoIndex++;
                }
                else
                {
                    ruleInfo.NeededRoundPairNum = needRoundPairNum;
                    var pairs = ruleInfo.MakePairs();
                    if (pairs.Count > 0)
                    {
                        logger.Debug($"{ruleInfo.RuleInfo.Name},共{pairs.Count}对");
                    }

                    foreach (var pair in pairs)
                    {
                        remainPairs.Enqueue((ruleInfo.RuleInfo, pair.ToArray()));
                    }
                }
                needRoundPairNum = config.MaxRoundPairNum - remainPairs.Count;
                if (needRoundPairNum == 0)
                    return;
            }
            isRoundEnd = true;
        }

        public virtual void CheckKnowledgeAvailability()
        {
        }
    }
    public abstract class PairMakerBase
    {
        public RuleInfo RuleInfo { get; set; }
        public int NeededRoundPairNum { get; set; }
        public bool IsRunnable { get; set; }
        public bool IsMadeAllPairs { get; set; }
        public abstract List<Knowledge[]> MakePairs();
        public abstract void UpdatePositions();
        public abstract void CheckIfIsRunnable(KnowledgeBase knowledgeBase);
        public override string ToString()
        {
            return RuleInfo.ToString();
        }
    }
    /// <summary>
    /// 全排列
    /// </summary>
    public class PermutationPairMaker : PairMakerBase
    {
        public List<int> OldPositions { get; set; } = new List<int>();
        public List<int> NewPositions { get; set; } = new List<int>();
        public List<List<Knowledge>> KnowledgeList { get; set; } = new();
        public List<int> CurPositions { get; set; } = new List<int>();
        List<int> targetStartPositions { get; set; } = new List<int>();
        List<int> targetEndPositions { get; set; } = new List<int>();
        int newTypeIndex { get; set; } = 0;
        public override void CheckIfIsRunnable(KnowledgeBase knowledgeBase)
        {
            //没写完的规则
            if (RuleInfo.LeftTypes.Count == 0)
                return;
            if (RuleInfo.LeftTypes.TrueForAll(type => knowledgeBase.Categories.ContainsKey(type)))
            {
                foreach (var type in RuleInfo.LeftTypes)
                {
                    OldPositions.Add(0);
                    NewPositions.Add(0);
                    CurPositions.Add(0);
                    targetEndPositions.Add(0);
                    targetStartPositions.Add(0);
                    KnowledgeList.Add(knowledgeBase.Categories[type]);
                }
                IsRunnable = true;
            }
        }
        public override void UpdatePositions()
        {
            for (var i = 0; i < NewPositions.Count; i++)
            {
                OldPositions[i] = NewPositions[i];
                var c = KnowledgeList[i];
                NewPositions[i] = c.Count;
            }

            //重置
            newTypeIndex = 0;
            setTargetPositions(newTypeIndex);
            //检查有无新知识
            IsMadeAllPairs = true;
            for (int i = 0; i < RuleInfo.LeftTypes.Count; i++)
            {
                if (targetStartPositions[i] != targetEndPositions[i])
                {
                    IsMadeAllPairs = false;
                    break;
                }
            }

            for (int i = 0; i < RuleInfo.LeftTypes.Count; i++)
            {
                CurPositions[i] = targetStartPositions[i];
            }
        }
        void setTargetPositions(int num)
        {
            for (int i = 0; i < RuleInfo.LeftTypes.Count; i++)
            {
                if (i < num)
                {
                    targetStartPositions[i] = 0;
                    targetEndPositions[i] = OldPositions[i];
                }
                else if (i == num)
                {
                    targetStartPositions[i] = OldPositions[i];
                    targetEndPositions[i] = NewPositions[i];
                }
                else if (i > num)
                {
                    targetStartPositions[i] = 0;
                    targetEndPositions[i] = NewPositions[i];
                }
            }
        }
        int maxPairCount;
        List<Knowledge[]> tempList = new();
        public int MadePairCount { get; set; }
        public override List<Knowledge[]> MakePairs()
        {
            maxPairCount = maxPairCount;
            tempList = new List<Knowledge[]>();
            switch (RuleInfo.LeftTypes.Count)
            {
                case 1:
                    makePair1(); break;
                case 2:
                    makePair2(); break;
                case 3:
                    makePair3(); break;
                case 4:
                    makePair4(); break;
                case 5:
                    makePair5(); break;
                case 6:
                    makePair6(); break;
                default:
                    throw new NotImplementedException("规则参数过多");
            }
            MadePairCount += tempList.Count;
            return tempList;
        }
        #region subMakePair
        void makePair1()
        {
            for (; CurPositions[0] < NewPositions[0]; CurPositions[0]++)
            {
                Knowledge[] knowledges = [KnowledgeList[0][CurPositions[0]]];
                tempList.Add(knowledges);

                if (maxPairCount == tempList.Count)
                    return;
            }
            IsMadeAllPairs = true;
        }
        void makePair2()
        {
            while (newTypeIndex < RuleInfo.LeftTypes.Count)
            {
                for (; CurPositions[0] < targetEndPositions[0]; CurPositions[0]++)
                {
                    for (; CurPositions[1] < targetEndPositions[1]; CurPositions[1]++)
                    {
                        Knowledge[] knowledges =
                            new Knowledge[] { KnowledgeList[0][CurPositions[0]],
                                    KnowledgeList[1][CurPositions[1]] };
                        if (knowledges.Distinct().Count() != knowledges.Count())
                            continue;
                        tempList.Add(knowledges);
                        if (maxPairCount == tempList.Count)
                            return;
                    }
                    CurPositions[1] = targetStartPositions[1];
                }
                newTypeIndex++;
                setTargetPositions(newTypeIndex);
                for (int i = 0; i < RuleInfo.LeftTypes.Count; i++)
                {
                    CurPositions[i] = targetStartPositions[i];
                }
            }
            IsMadeAllPairs = true;
        }
        void makePair3()
        {
            while (newTypeIndex < RuleInfo.LeftTypes.Count)
            {
                for (; CurPositions[0] < targetEndPositions[0]; CurPositions[0]++)
                {
                    for (; CurPositions[1] < targetEndPositions[1]; CurPositions[1]++)
                    {
                        for (; CurPositions[2] < targetEndPositions[2]; CurPositions[2]++)
                        {
                            Knowledge[] knowledges =
                            new Knowledge[] { KnowledgeList[0][CurPositions[0]],
                                    KnowledgeList[1][CurPositions[1]],
                                    KnowledgeList[2][CurPositions[2]]};
                            if (knowledges.Distinct().Count() != knowledges.Count())
                                continue;
                            tempList.Add(knowledges);

                            if (maxPairCount == tempList.Count)
                                return;
                        }
                        CurPositions[2] = targetStartPositions[2];
                    }
                    CurPositions[1] = targetStartPositions[1];
                }
                newTypeIndex++;
                setTargetPositions(newTypeIndex);
                for (int i = 0; i < RuleInfo.LeftTypes.Count; i++)
                {
                    CurPositions[i] = targetStartPositions[i];
                }
            }
            IsMadeAllPairs = true;
        }
        void makePair4()
        {
            while (newTypeIndex < RuleInfo.LeftTypes.Count)
            {
                for (; CurPositions[0] < targetEndPositions[0]; CurPositions[0]++)
                {
                    for (; CurPositions[1] < targetEndPositions[1]; CurPositions[1]++)
                    {
                        for (; CurPositions[2] < targetEndPositions[2]; CurPositions[2]++)
                        {
                            for (; CurPositions[3] < targetEndPositions[3]; CurPositions[3]++)
                            {
                                Knowledge[] knowledges =
                            new Knowledge[]{ KnowledgeList[0][CurPositions[0]],
                                    KnowledgeList[1][CurPositions[1]],
                                    KnowledgeList[2][CurPositions[2]],
                                    KnowledgeList[3][CurPositions[3]]};
                                if (knowledges.Distinct().Count() != knowledges.Count())
                                    continue;
                                tempList.Add(knowledges);

                                if (maxPairCount == tempList.Count)
                                    return;
                            }
                            CurPositions[3] = targetStartPositions[3];
                        }
                        CurPositions[2] = targetStartPositions[2];
                    }
                    CurPositions[1] = targetStartPositions[1];
                }
                newTypeIndex++;
                setTargetPositions(newTypeIndex);
                for (int i = 0; i < RuleInfo.LeftTypes.Count; i++)
                {
                    CurPositions[i] = targetStartPositions[i];
                }
            }
            IsMadeAllPairs = true;
        }
        void makePair5()
        {
            while (newTypeIndex < RuleInfo.LeftTypes.Count)
            {
                for (; CurPositions[0] < targetEndPositions[0]; CurPositions[0]++)
                {
                    for (; CurPositions[1] < targetEndPositions[1]; CurPositions[1]++)
                    {
                        for (; CurPositions[2] < targetEndPositions[2]; CurPositions[2]++)
                        {
                            for (; CurPositions[3] < targetEndPositions[3]; CurPositions[3]++)
                            {
                                for (; CurPositions[4] < targetEndPositions[4]; CurPositions[4]++)
                                {
                                    Knowledge[] knowledges =
                            new Knowledge[]{ KnowledgeList[0][CurPositions[0]],
                                    KnowledgeList[1][CurPositions[1]],
                                    KnowledgeList[2][CurPositions[2]],
                                    KnowledgeList[3][CurPositions[3]],
                                    KnowledgeList[4][CurPositions[4]]};
                                    if (knowledges.Distinct().Count() != knowledges.Count())
                                        continue;
                                    tempList.Add(knowledges);
                                    if (maxPairCount == tempList.Count)
                                        return;
                                }
                                CurPositions[4] = targetStartPositions[4];
                            }
                            CurPositions[3] = targetStartPositions[3];
                        }
                        CurPositions[2] = targetStartPositions[2];
                    }
                    CurPositions[1] = targetStartPositions[1];
                }
                newTypeIndex++;
                setTargetPositions(newTypeIndex);
                for (int i = 0; i < RuleInfo.LeftTypes.Count; i++)
                {
                    CurPositions[i] = targetStartPositions[i];
                }
            }
            IsMadeAllPairs = true;
        }
        void makePair6()
        {
            while (newTypeIndex < RuleInfo.LeftTypes.Count)
            {
                for (; CurPositions[0] < targetEndPositions[0]; CurPositions[0]++)
                {
                    for (; CurPositions[1] < targetEndPositions[1]; CurPositions[1]++)
                    {
                        for (; CurPositions[2] < targetEndPositions[2]; CurPositions[2]++)
                        {
                            for (; CurPositions[3] < targetEndPositions[3]; CurPositions[3]++)
                            {
                                for (; CurPositions[4] < targetEndPositions[4]; CurPositions[4]++)
                                {
                                    for (; CurPositions[5] < targetEndPositions[5]; CurPositions[5]++)
                                    {
                                        Knowledge[] knowledges =
                            new Knowledge[] { KnowledgeList[0][CurPositions[0]],
                                    KnowledgeList[1][CurPositions[1]],
                                    KnowledgeList[2][CurPositions[2]],
                                    KnowledgeList[3][CurPositions[3]],
                                    KnowledgeList[4][CurPositions[4]],
                                    KnowledgeList[5][CurPositions[5]]};
                                        if (knowledges.Distinct().Count() != knowledges.Count())
                                            continue;
                                        tempList.Add(knowledges);
                                        if (maxPairCount == tempList.Count)
                                            return;
                                    }
                                    CurPositions[5] = targetStartPositions[5];
                                }
                                CurPositions[4] = targetStartPositions[4];
                            }
                            CurPositions[3] = targetStartPositions[3];
                        }
                        CurPositions[2] = targetStartPositions[2];
                    }
                    CurPositions[1] = targetStartPositions[1];
                }
                newTypeIndex++;
                setTargetPositions(newTypeIndex);
                for (int i = 0; i < RuleInfo.LeftTypes.Count; i++)
                {
                    CurPositions[i] = targetStartPositions[i];
                }
            }
            IsMadeAllPairs = true;
        }
    }
    #endregion
    /// <summary>
    /// 全组合
    /// </summary>
    public class CombinationPairMaker : PairMakerBase
    {
        public int MadePairCount { get; set; }

        public Dictionary<string, int> KnowledgeTypeCount = new();
        public Dictionary<string, List<Knowledge>> KnowledgeList = new();
        public Dictionary<string, int> OldPositions { get; set; } = new();
        public Dictionary<string, int> NewPositions { get; set; } = new();

        public Dictionary<string, bool> IsFirstMakeGroup = new();
        public List<int> CurPositions { get; set; } = new List<int>();
        public Dictionary<string, List<Knowledge[]>> GroupKnowledge { get; set; } = new();
        public override void CheckIfIsRunnable(KnowledgeBase knowledgeBase)
        {
            //没写完的规则
            if (RuleInfo.LeftTypes.Count == 0)
                return;
            KnowledgeTypeCount = DictionaryTool.CountItemNum(RuleInfo.LeftTypes.Select(t => t.Name));
            if (RuleInfo.LeftTypes.TrueForAll(type => knowledgeBase.Categories.ContainsKey(type)))
            {
                var t = RuleInfo.LeftTypes.Distinct();
                foreach (var type in t)
                {
                    OldPositions.Add(type.Name, 0);
                    NewPositions.Add(type.Name, 0);
                    CurPositions.Add(0);
                    KnowledgeList.Add(type.Name, knowledgeBase.Categories[type]);
                    IsFirstMakeGroup.Add(type.Name, true);
                    GroupKnowledge.Add(type.Name, new());
                }
                IsRunnable = true;
            }
        }
        public override void UpdatePositions()
        {
            foreach (var key in KnowledgeList.Keys)
            {
                OldPositions[key] = NewPositions[key];
                var c = KnowledgeList[key];
                NewPositions[key] = c.Count;
            }
            //更新初始位置
            for (var i = 0; i < CurPositions.Count; i++)
            {
                CurPositions[0] = 0;
            }
            //检查有无新知识
            IsMadeAllPairs = true;
            foreach (var key in KnowledgeList.Keys)
            {
                if (OldPositions[key] != NewPositions[key])
                {
                    IsMadeAllPairs = false;
                    break;
                }
            }
            makeAllGroupKnowledge();
        }
        public void makeAllGroupKnowledge()
        {
            foreach (var k in KnowledgeList.Keys)
            {
                var olePos = OldPositions[k];
                var newPos = NewPositions[k];
                makeGroupKnowledge(olePos, newPos, k);
            }
        }
        public void makeGroupKnowledge(int oldPos, int newPos, string type)
        {
            List<Knowledge[]> knowledges = null;
            switch (KnowledgeTypeCount[type])
            {
                case 1:
                    knowledges = makePair1(oldPos, newPos, type); break;
                case 2:
                    knowledges = makePair2(oldPos, newPos, type); break;
                case 3:
                    knowledges = makePair3(oldPos, newPos, type); break;
                case 4:
                    knowledges = makePair4(oldPos, newPos, type); break;
                case 5:
                    knowledges = makePair5(oldPos, newPos, type); break;
                case 6:
                    knowledges = makePair6(oldPos, newPos, type); break;
                default:
                    throw new NotImplementedException("规则参数过多");
            }
            GroupKnowledge[type] = knowledges;
        }
        #region sub
        List<Knowledge[]> makePair1(int oldPos, int newPos, string type)
        {
            bool isFirstMakeGroup = IsFirstMakeGroup[type];
            List<Knowledge> knowledges = KnowledgeList[type];
            List<Knowledge[]> list = new List<Knowledge[]>();
            if (isFirstMakeGroup)
            {
                IsFirstMakeGroup[type] = false;
                for (var l1 = 0; l1 < newPos; l1++)
                {
                    Knowledge[] pair = [knowledges[l1],];
                    list.Add(pair);
                }
            }
            else
            {
                for (var l1 = oldPos; l1 < newPos; l1++)
                {
                    Knowledge[] pair = [knowledges[l1],];
                    list.Add(pair);
                }
            }
            return list;
        }
        List<Knowledge[]> makePair2(int oldPos, int newPos, string type)
        {
            bool isFirstMakeGroup = IsFirstMakeGroup[type];
            List<Knowledge> knowledges = KnowledgeList[type];
            List<Knowledge[]> list = new List<Knowledge[]>();
            if (isFirstMakeGroup)
            {
                IsFirstMakeGroup[type] = false;
                for (var l1 = 0; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < newPos; l2++)
                    {
                        Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],];
                        list.Add(pair);
                    }
                }
            }
            else
            {
                for (var l1 = oldPos; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < oldPos; l2++)
                    {
                        Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],];
                        list.Add(pair);
                    }
                }
            }
            return list;
        }
        List<Knowledge[]> makePair3(int oldPos, int newPos, string type)
        {
            bool isFirstMakeGroup = IsFirstMakeGroup[type];
            List<Knowledge> knowledges = KnowledgeList[type];
            List<Knowledge[]> list = new List<Knowledge[]>();
            if (isFirstMakeGroup)
            {
                IsFirstMakeGroup[type] = false;
                for (var l1 = 0; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < newPos; l2++)
                    {
                        for (var l3 = l2 + 1; l3 < newPos; l3++)
                        {
                            Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],knowledges[l3],];
                            list.Add(pair);
                        }
                    }
                }
            }
            else
            {
                for (var l1 = oldPos; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < oldPos; l2++)
                    {
                        for (var l3 = l2 + 1; l3 < oldPos; l3++)
                        {
                            Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],knowledges[l3],];
                            list.Add(pair);
                        }
                    }
                }
            }
            return list;
        }
        List<Knowledge[]> makePair4(int oldPos, int newPos, string type)
        {
            bool isFirstMakeGroup = IsFirstMakeGroup[type];
            List<Knowledge> knowledges = KnowledgeList[type];
            List<Knowledge[]> list = new List<Knowledge[]>();
            if (isFirstMakeGroup)
            {
                IsFirstMakeGroup[type] = false;
                for (var l1 = 0; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < newPos; l2++)
                    {
                        for (var l3 = l2 + 1; l3 < newPos; l3++)
                        {
                            for (var l4 = l3 + 1; l4 < newPos; l4++)
                            {
                                Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],knowledges[l3],knowledges[l4],
                                        ];
                                list.Add(pair);
                            }
                        }
                    }
                }
            }
            else
            {
                for (var l1 = oldPos; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < oldPos; l2++)
                    {
                        for (var l3 = l2 + 1; l3 < oldPos; l3++)
                        {
                            for (var l4 = l3 + 1; l4 < oldPos; l4++)
                            {
                                Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],knowledges[l3],knowledges[l4],];
                                list.Add(pair);
                            }
                        }
                    }
                }
            }
            return list;
        }
        List<Knowledge[]> makePair5(int oldPos, int newPos, string type)
        {
            bool isFirstMakeGroup = IsFirstMakeGroup[type];
            List<Knowledge> knowledges = KnowledgeList[type];
            List<Knowledge[]> list = new List<Knowledge[]>();
            if (isFirstMakeGroup)
            {
                IsFirstMakeGroup[type] = false;
                for (var l1 = 0; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < newPos; l2++)
                    {
                        for (var l3 = l2 + 1; l3 < newPos; l3++)
                        {
                            for (var l4 = l3 + 1; l4 < newPos; l4++)
                            {
                                for (var l5 = l4 + 1; l5 < newPos; l5++)
                                {
                                    Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],knowledges[l3],knowledges[l4],
                                        knowledges[l5],];
                                    list.Add(pair);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (var l1 = oldPos; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < oldPos; l2++)
                    {
                        for (var l3 = l2 + 1; l3 < oldPos; l3++)
                        {
                            for (var l4 = l3 + 1; l4 < oldPos; l4++)
                            {
                                for (var l5 = l4 + 1; l5 < oldPos; l5++)
                                {
                                    Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],knowledges[l3],knowledges[l4],
                                        knowledges[l5],];
                                    list.Add(pair);
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }
        List<Knowledge[]> makePair6(int oldPos, int newPos, string type)
        {
            bool isFirstMakeGroup = IsFirstMakeGroup[type];
            List<Knowledge> knowledges = KnowledgeList[type];
            List<Knowledge[]> list = new List<Knowledge[]>();
            if (isFirstMakeGroup)
            {
                IsFirstMakeGroup[type] = false;
                for (var l1 = 0; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < newPos; l2++)
                    {
                        for (var l3 = l2 + 1; l3 < newPos; l3++)
                        {
                            for (var l4 = l3 + 1; l4 < newPos; l4++)
                            {
                                for (var l5 = l4 + 1; l5 < newPos; l5++)
                                {
                                    for (var l6 = l5 + 1; l6 < newPos; l6++)
                                    {
                                        Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],knowledges[l3],knowledges[l4],
                                        knowledges[l5],knowledges[l6],];
                                        list.Add(pair);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (var l1 = oldPos; l1 < newPos; l1++)
                {
                    for (var l2 = l1 + 1; l2 < oldPos; l2++)
                    {
                        for (var l3 = l2 + 1; l3 < oldPos; l3++)
                        {
                            for (var l4 = l3 + 1; l4 < oldPos; l4++)
                            {
                                for (var l5 = l4 + 1; l5 < oldPos; l5++)
                                {
                                    for (var l6 = l5 + 1; l6 < oldPos; l6++)
                                    {
                                        Knowledge[] pair = [ knowledges[l1],
                                   knowledges[l2],knowledges[l3],knowledges[l4],
                                        knowledges[l5],knowledges[l6],];
                                        list.Add(pair);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }
        #endregion
        List<Knowledge[]> tempList = new();

        void RGetPairs(int layer, List<Knowledge> knowledges)
        {
            if (layer == GroupKnowledge.Keys.Count)
            {
                tempList.Add(knowledges.ToArray());
                return;
            }
            var key = GroupKnowledge.Keys.ToArray()[layer];
            foreach (var group in GroupKnowledge[key])
            {
                List<Knowledge> subKnowledges = new List<Knowledge>(knowledges);
                subKnowledges.AddRange(group);
                RGetPairs(layer + 1, subKnowledges);
            }
        }
        public override List<Knowledge[]> MakePairs()
        {
            tempList = new List<Knowledge[]>();
            RGetPairs(0, new List<Knowledge>());
            IsMadeAllPairs = true;
            MadePairCount += tempList.Count;
            return tempList;
        }


    }
}
