
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.Knowledges;
using System.Reflection;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.IO.Outputs
{
    public class HumanoidAnswerMakerConfig : AInferenceSetting
    {
        public bool IsSlim { get; set; } = true;
    }
    [Description("类人答题生成器")]
    public class HumanLikeAnswerOutputMaker : IInferenceOutputMaker<HumanLikeAnswerOutput>
    {
        [ZDI]
        TargetBase tBase;
        [ZDI]
        HumanoidAnswerMakerConfig config;
        string name;
        public string Name { get => name; set => name = value; }
        string MakeConditionStr(Knowledge knowledge)
        {
            var answer = "";
            var conditions = new List<Knowledge>();
            GetList(conditions, knowledge);
            conditions.Reverse();

            int index = 1;
            foreach (var condition in conditions)
            {
                if (condition.Conditions.Count == 0)
                {
                    //answer += $"({index++})∵\t{condition}\t(known)\n";
                    answer += $"({index++})∵\t{condition,-35} (已知)\n";
                }
                else
                {
                    List<int> condictionIndexs = new List<int>();
                    foreach (var item in condition.Conditions.Distinct())
                    {
                        condictionIndexs.Add(conditions.IndexOf(item) + 1);
                    }
                    //answer += $"({index++})∴\t{condition}\t({condition.Reason}\t{StringTool.ComposeList(condictionIndexs)})\n";
                    answer += $"({index++})∴\t{condition,-35}({condition.Reason,-10}\t{StringTool.ComposeList(condictionIndexs)})\n";
                }
            }
            answer += "\n";
            return answer;
        }
        void GetList(in List<Knowledge> historyKnowledges, Knowledge knowledge)
        {
            if (historyKnowledges.Contains(knowledge))
            {
                historyKnowledges.Remove(knowledge);
                historyKnowledges.Add(knowledge);
            }
            else
            {
                historyKnowledges.Add(knowledge);
            }
            //倒过来0 0
            for (int i = knowledge.Conditions.Count - 1; i >= 0; i--)
            {
                if (config.IsSlim)
                {
                    if (knowledge.Conditions[i].GetType().GetCustomAttribute<PrimitiveKnowledgeAttribute>() is not null)
                        continue;
                }
                GetList(historyKnowledges, knowledge.Conditions[i]);
            }

        }
        public HumanLikeAnswerOutput Make()
        {
            var output = new HumanLikeAnswerOutput();
            foreach (var item in tBase.ToProves)
            {
                if (item.IsSuccess)
                {
                    output.Answers.Add(new()
                    {
                        Index = item.Index + 1,
                        IsSuccess = item.IsSuccess,
                        Question = item.ToString(),
                        Answer = MakeConditionStr(item.Conclusion)
                    });
                }
                else
                {
                    output.Answers.Add(new()
                    {
                        Index = item.Index + 1,
                        IsSuccess = item.IsSuccess,
                        Question = item.ToString(),
                        Answer = "无"
                    });
                }

            }
            foreach (var item in tBase.ToSolves)
            {
                if (item.IsSuccess)
                {
                    output.Answers.Add(new()
                    {
                        Index = item.Index + 1,
                        IsSuccess = item.IsSuccess,
                        Question = item.ToString(),
                        Answer = MakeConditionStr(item.Conclusion)
                    });
                }
                else
                {
                    output.Answers.Add(new()
                    {
                        Index = item.Index + 1,
                        IsSuccess = item.IsSuccess,
                        Question = item.ToString(),
                        Answer = "无"
                    });
                }
            }
            output.Answers.Sort((a, b) => a.Index.CompareTo(b.Index));
            return output;
        }
    }
}
