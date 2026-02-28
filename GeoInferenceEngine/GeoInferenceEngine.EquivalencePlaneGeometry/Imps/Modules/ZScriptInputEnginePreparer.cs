using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs.MutNodes;
using GeoInferenceEngine.Knowledges.Imps.Componments;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;
using GeoInferenceEngine.PredicateShared.Models;

using System.Reflection;

using System.Text.RegularExpressions;

using Throw;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Modules
{
    public class ZScriptInputEnginePreparerConfig : AInferenceSetting
    {
        public bool IsAcceptAuxiliary { get; set; } = true;
        public bool IsThrowZScriptParseExextion { get; set; } = true;
    }
    [Description("准备模块")]
    public class ZScriptInputEnginePreparer : IEnginePreparer
    {
        public Action Preparering { get; set; }
        public Action Preparered { get; set; }
        [ZDI]
        ZScriptInputEnginePreparerConfig config;
        [ZDI]
        ExprService ExprService;
        [ZDI]
        Logger Logger { get; set; }
        [ZDI]
        AppInfo appInfo { get; set; }
        [ZDI]
        PRAssembliesBase PRAssembliesBase { get; set; }
        [ZDI]
        ZScriptInput ZScriptInput { get; set; }
        [ZDI]
        IInferenceEngine Engine { get; set; }
        public void Prepare()
        {
            Preparering?.Invoke();
            ExprService.Init();
            Knowledge.InitClassIndex(PRAssembliesBase.KnowledgeAssemblies.ToArray());
            appInfo.CurAction = "解析ZScript";
            if (config.IsThrowZScriptParseExextion)
            {
                Run(ZScriptInput);
            }
            else
            {
                try
                {
                    Run(ZScriptInput);
                }
                catch (Exception ex)
                {
                    appInfo.IsActivedStop = true;
                    appInfo.ActivedStopReasons.Add(ex.Message);
                }
            }
            Preparered?.Invoke();
            appInfo.CurAction = "初始化推理引擎";
            Engine.Init();
        }
        //----------------------------------以下为zscript解析-----------------------------------------
        #region 引用
        [ZDI]
        FormularBase formularBase;
        [ZDI]
        AppInfo AppInfo;
        [ZDI]
        EngineInfo EngineInfo;
        [ZDI]
        KnowledgeBase knowledgeBase;
        [ZDI]
        TargetBase targetBase;
        [ZDI]
        PRAssembliesBase assemblyBase;
        [ZDI]
        KnowledgeAddProcessor AddProcessor { get; set; }
        #endregion

        #region 准备工作
        void loadDefaultPointNames()
        {
            var AToZ = StringTool.GetAToZList();
            foreach (var item in AToZ)
            {
                PointNames.Add(item);
                for (int i = 0; i < 10; i++)
                {
                    PointNames.Add($"{item}{i}");
                    PointNames.Add($"{item}_{i}");
                }
            }
        }
        void loadDefaultVarNames()
        {
            var aToz = StringTool.GetaTozList();
            foreach (var item in aToz)
            {
                VarNames.Add(item);
            }
        }
        void loadStringTypeMap(IEnumerable<Assembly> forCheckAssemblis)
        {
            Dictionary<string, (string, string)> duplicateDict = new();
            foreach (var assemble in forCheckAssemblis)
            {
                foreach (var type in assemble.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Knowledge)))
                    {
                        stringTypeMap.Add(type.Name, type);

                        var descriptions = DescriptionAttribute.GetDescriptions(type);
                        foreach (var description in descriptions)
                        {
                            if (stringTypeMap.Keys.Contains(description))
                            {
                                duplicateDict.Add(description, (stringTypeMap[description].Name, type.Name));
                            }
                            else
                            {
                                if (description.Contains("<T>"))
                                {
                                    genStringTypeMap.Add(description.Replace("<T>", "(.*)"), type);

                                }
                                stringTypeMap.Add(description, type);
                            }
                        }
                    }
                }
            }
            if (duplicateDict.Count > 0)
            {
                throw new ArgumentException("谓词定义中出现了重复的中文");
            }
            stringTypeMap = stringTypeMap.OrderByDescending((kv) => { return kv.Key.Length; }).ToDictionary(kv => kv.Key, kv => kv.Value);

        }
        #endregion

        Dictionary<string, Type> genStringTypeMap = new Dictionary<string, Type>();
        Dictionary<string, Type> stringTypeMap = new Dictionary<string, Type>();
        public Dictionary<string, Point> PointRefs = new();
        public List<string> PointNames { get; set; } = new();
        public List<string> VarNames { get; set; } = new List<string>();
        public int curLineIndex { get; set; }
        public string curLine { get; set; }
        public int questionIndex { get; set; }
        #region Parse
        void Run(ZScriptInput zScriptInput)
        {
            //加载
            loadStringTypeMap(assemblyBase.KnowledgeAssemblies);
            loadDefaultPointNames();
            loadDefaultVarNames();
            //标准化处理
            var zscript = zScriptInput.Content;
            zscript = ZScriptDefinition.FormatStandard(zscript);
            var lines = zscript.Split('\n').Select(s => s.Trim()).ToList();

            curLineIndex = 1;
            //逐行处理
            foreach (var line in lines)
            {
                curLine = line;
                curLineIndex++;
                //空白
                if (string.IsNullOrEmpty(line)) continue;
                //注释
                else if (line.StartsWith("//")) continue;
                //点列表
                else if (line.StartsWith(ZScriptDefinition.PointsDef))
                {
                    //清空默认点列表
                    PointNames.Clear();
                    var content = line.Replace(ZScriptDefinition.PointsDef, string.Empty);
                    //以空格分割进行解析
                    foreach (var pointInfo in content.Split(' '))
                    {
                        var matchResult = Regex.Match(pointInfo, "([\\S\\s]*)\\(([\\S\\s]*)\\)");
                        //存在伪坐标
                        if (matchResult.Success)
                        {
                            var pointName = matchResult.Groups[1].Value;
                            var pointCoordInfo = matchResult.Groups[2].Value;
                            var pointCoords = pointCoordInfo.Split(',');

                            Point point = new Point(pointName);
                            point = (Point)AddProcessor.Add(point);
                            PointRefs.Add(pointName, point);

                            if (pointCoords.Length == 2)
                            {
                                point.X = double.Parse(pointCoords[0]);
                                point.Y = double.Parse(pointCoords[1]);
                            }
                            else
                                throw new ZscriptParseException($"伪坐标{pointInfo}只支持2维",curLineIndex,curLine);

                            PointNames.Add(pointName);
                            continue;
                        }
                        //不存在伪坐标
                        else
                        {
                            matchResult = Regex.Match(pointInfo, "([\\S\\s]*)");
                            if (matchResult.Success)
                            {
                                Point point = new Point(pointInfo);
                                point = (Point)AddProcessor.Add(point);
                                PointRefs.Add(pointInfo, point);
                                PointNames.Add(pointInfo);
                            }
                        }
                    }

                    //为避免 A 先于 A_1 匹配，对长度进行排序
                    PointNames.Sort((a, b) => a.Length == b.Length ? 0 : a.Length > b.Length ? -1 : 1);
                }
                //待证明信息
                else if (line.StartsWith(ZScriptDefinition.ProveDef))
                {
                    var content = line.Replace(ZScriptDefinition.ProveDef, string.Empty);

                    //识别等式特有标识
                    if (content.Contains("="))
                    {
                        GeoEquation equation = parseEquation(content);
                        GeoEquationInfo equationInfo = ExprService.GetEquationInfo(equation);
                        //将等式转变为几何关系
                        if (equationInfo.Type == GeoEquationTypes.Value || equationInfo.Type == GeoEquationTypes.Ratio)
                        {
                            var target = new KnowledgeTarget()
                            {
                                Index = questionIndex++,
                                Target = equationInfo.EquivalenceKnowledge
                            };
                            targetBase.ToProves.Add(target);
                            targetBase.KnowledgeTargetInfos.Add(target);
                        }
                        else
                        {
                            var target = new EquationTarget()
                            {
                                Index = questionIndex++,
                                Target = equation
                            };
                            targetBase.ToProves.Add(target);
                            targetBase.EquationTargetInfos.Add(target);
                        }
                    }
                    //else if (content.Contains("+") || content.Contains("-") || content.Contains("*") || content.Contains("/")) { throw new ZscriptParseException($"证明的目标不能是纯表达式", curLineIndex, curLine); }
                    else
                    {
                        Knowledge knowledge = parseKnowledge(content);
                        var target = new KnowledgeTarget()
                        {
                            Index = questionIndex++,
                            Target = knowledge
                        };
                        targetBase.ToProves.Add(target);
                        targetBase.KnowledgeTargetInfos.Add(target);
                        continue;
                    }
                }
                //待求解信息
                else if (line.StartsWith(ZScriptDefinition.SolveDef))
                {
                    var content = line.Replace(ZScriptDefinition.SolveDef, string.Empty);
                    if (VarNames.Contains(content))
                    {
                        var target = new VarTarget()
                        {
                            Index = questionIndex++,
                            Target = new Var(content)
                        };
                        targetBase.ToSolves.Add(target);
                        targetBase.VarTargets.Add(target);
                        continue;
                    }
                    if (content.Contains("=")) { throw new ZscriptParseException($"求解的目标不能是等式", curLineIndex, curLine); }
                    //识别表达式特有表示 尽管不全但是够用
                    else if (content.Contains("+") || content.Contains("-") || content.Contains("*") || content.Contains("/"))
                    {
                        var e = parseExpr(content);
                        var target = new ExprTarget()
                        {
                            Index = questionIndex++,
                            Target = e
                        };
                        targetBase.ToSolves.Add(target);
                        targetBase.ExprTargets.Add(target);
                        continue;
                    }
                    else
                    {
                        //尝试针对单变量
                        try
                        {
                            var expr = parseMut(content);
                            if (expr is GeoProp g)
                            {
                                var knowledge = g.ToSolveKnowledge();
                                var target = new RelationTarget()
                                {
                                    Index = questionIndex++,
                                    Target = knowledge
                                };
                                targetBase.ToSolves.Add(target);
                                targetBase.RelationTargets.Add(target);
                                continue;
                            }
                            else if (expr is Var var)
                            {
                                var e = parseExpr(content);
                                var target = new ExprTarget()
                                {
                                    Index = questionIndex++,
                                    Target = e
                                };
                                targetBase.ToSolves.Add(target);
                                targetBase.ExprTargets.Add(target);
                                continue;
                            }
                        }
                        catch
                        {
                            //进一步尝试作为知识进行解析
                            try
                            {
                                Knowledge knowledge = parseKnowledge(content);
                                var target = new RelationTarget()
                                {
                                    Index = questionIndex++,
                                    Target = knowledge
                                };
                                targetBase.ToSolves.Add(target);
                                targetBase.RelationTargets.Add(target);
                                continue;
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    throw new ZscriptParseException($"无法解析求解目标{content}", curLineIndex, curLine);
                }
                //待证伪的信息
                else if (line.StartsWith(ZScriptDefinition.DisproveDef))
                {
                    throw new ZscriptParseException($"未处理DetermineDef关键词", curLineIndex, curLine);
                    //var content = line.Replace(ZScriptDefinition.DisproveDef, string.Empty);
                    //Knowledge knowledge = parseKnowledge(content);
                    //targetBase.ToDisproves.Add(new KnowledgeTarget() { Index = questionIndex++, Target = knowledge });
                    continue;
                }
                //待确定信息
                else if (line.StartsWith(ZScriptDefinition.DetermineDef))
                {
                    throw new ZscriptParseException(curLine + $"未处理DetermineDef关键词", curLineIndex, curLine);

                }
                //待满足信息
                else if (line.StartsWith(ZScriptDefinition.SatisfyDef))
                {
                    throw new ZscriptParseException(curLine + $"未处理SatisfyDef关键词", curLineIndex, curLine);
                }
                else if (line.StartsWith(ZScriptDefinition.AuxDef))
                {
                    if (config.IsAcceptAuxiliary)
                    {
                        if (line.Contains("="))
                        {
                            var parts = line.Split("=");
                            var left = parseExpr(parts[0]);
                            var right = parseExpr(parts[1]);
                            GeoEquation geoEquation = new GeoEquation(left, right);
                            geoEquation.AddReason("known");
                            AddProcessor.Add(geoEquation);
                        }
                        else
                        {
                            Knowledge knowledge = parseKnowledge(line);
                            knowledge.AddReason("known");
                            AddProcessor.Add(knowledge);
                        }
                    }

                }
                //一般的知识
                else
                {

                    if (line.Contains("="))
                    {
                        var parts = line.Split("=");
                        var left = parseExpr(parts[0]);
                        var right = parseExpr(parts[1]);
                        GeoEquation geoEquation = new GeoEquation(left, right);
                        geoEquation.AddReason("known");
                        AddProcessor.Add(geoEquation);
                    }
                    else
                    { 
                           Knowledge knowledge = parseKnowledge(line);
                           knowledge.AddReason("known");
                           AddProcessor.Add(knowledge);
                    }
                }
            }
        }
        public Knowledge parseKnowledge(string line)
        {
            if (line.Contains("="))
            {
                GeoEquation geoEquation = parseEquation(line);
                return geoEquation;
            }
            else
            {
                Knowledge knowledge = parseRelation(line);
                return knowledge;
            }
        }
        Knowledge parseRelation(string line)
        {
            //为构造函数的特殊处理
            if (line.Contains(":"))
            {
                var parts = line.Split(':');
                parts.Throw(() => new ZscriptParseException($"不符合构造性语句输入规范",curLineIndex,curLine))
                    .IfCountNotEquals(2);
                var reform = $"{parts[1]},{parts[0]}";
                line = reform;
            }
            var content = line.Split(' ');
            content.Throw(() => new ZscriptParseException($"谓词与参数没有空格",curLineIndex,curLine))
                .IfCountNotEquals(2);
            Type predicate = parsePredicate(content[0]);
            var args = parseArgs(content[1]);
            Knowledge result = combinePredicate_AND_Args_Then_Add(predicate, args);
            return result;
        }
        GeoEquation parseEquation(string line)
        {
            var parts = line.Split("=");
            var left = parseExpr(parts[0]);
            var right = parseExpr(parts[1]);
            GeoEquation geoEquation = new GeoEquation(left, right);
            return geoEquation;
        }
        public Expr parseExpr(string str)
        {
            return  new ZExprParser(str) { ParserMutFunc=parseMut}.Parse();
        }
        Mut parseMut(string str)
        {
            if (ExprService.MutRefs.ContainsKey(str)) { return ExprService.MutRefs[str]; }
            else if (VarNames.Contains(str))
            {
                return new Var(str);
            }
            else
            {
                return parseGeoProp(str);
            }
        }
        Mut parseGeoProp(string idNode)
        {
            var match = Regex.Match(idNode, "^三角形([A-Z0-9]+|[a-z0-9]+$)的面积|^Δ([A-Z0-9]+|[a-z0-9]+)$的面积");
            if (match.Success)
            {
                var points = getPointsFromStr(match.Groups[1].Value);
                if (points.Count == 3)
                {
                    Triangle pred = new Triangle(points[0], points[1], points[2]);
                    pred = (Triangle)AddProcessor.Add(pred);
                    return pred.Area;
                }
                else
                    throw new ZscriptParseException("三角形点数量超过三",curLineIndex,curLine);
            }
            match = Regex.Match(idNode, "^弧([A-Z0-9]+|[a-z0-9]+)$");
            if (match.Success)
            {
                var points = getPointsFromStr(match.Groups[1].Value);
                if (points.Count == 3)
                {
                    Arc pred = new Arc(points[0], points[1], points[2]);
                    pred = (Arc)AddProcessor.Add(pred);
                    return pred.Length;
                }
                else
                    throw new ZscriptParseException("弧点数量超过三", curLineIndex, curLine);
            }
            match = Regex.Match(idNode, "^角([A-Z]+)");
            if (match.Success)
            {
                var points = getPointsFromStr(match.Groups[1].Value);
                if (points.Count == 3)
                {
                    Angle pred = new Angle(new() { points[0] }, points[1], new() { points[2] });
                    pred = (Angle)AddProcessor.Add(pred);
                    return pred.Size;
                }
                else
                    throw new ZscriptParseException("角点数量不为三", curLineIndex, curLine);
            }

            match = Regex.Match(idNode, "^[A-Z0-9]+|[a-z0-9]+$");
            if (match.Success)
            {
                var points = getPointsFromStr(match.Groups[0].Value);
                if (points.Count == 2)
                {
                    Segment pred = new Segment(points[0], points[1]);
                    pred = (Segment)AddProcessor.Add(pred);

                    return pred.Length;
                }
                else
                    throw new ZscriptParseException("只有线段可以省略写", curLineIndex, curLine);

            }
            return null;
            throw new ZscriptParseException("未知的几何属性", curLineIndex, curLine);
        }
        #endregion

        #region 解析知识
        /// <summary>
        /// 取出字符串中全部点
        /// 点列表应该是个完整的点列表，如果还有字符串没被匹配上则抛出异常
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        List<Point> getPointsFromStr(string arg)
        {
            /// <summary>
            /// 递归方法一个一个取出点
            /// </summary>
            /// <param name="arg"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            string getPointFromStr(string arg, out string point)
            {
                foreach (var pointName in PointNames)
                {
                    if (arg.StartsWith(pointName))
                    {
                        point = pointName;
                        Regex r = new Regex(pointName);
                        arg = r.Replace(arg, string.Empty, 1);//只替换第一次（个）
                        return arg;
                    }
                }
                point = null;
                return arg;
            }
            List<Point> points = new List<Point>();

            string pointName = null;
            var left = getPointFromStr(arg, out pointName);
            bool first = true;
            while (pointName is not null)
            {
                if (first)
                    first = false;
                points.Add(PointRefs[pointName]);
                left = getPointFromStr(left, out pointName);
                if (!first && left != string.Empty && pointName is null)
                {
                    throw new ZscriptParseException("点列表中有不是点", curLineIndex, curLine);
                }
            }
            return points;
        }
        Type parsePredicate(string predicateStr)
        {
            if (stringTypeMap.ContainsKey(predicateStr))
            {
                Type type = stringTypeMap[predicateStr];
                return type;
            }
            else
            {
                foreach (var key in genStringTypeMap.Keys)
                {
                    Match match = Regex.Match(predicateStr, key);
                    if (match.Success)
                    {
                        Type genType = genStringTypeMap[key];
                        string innerStr = match.Groups[1].Value;
                        Type innerType = parsePredicate(innerStr);
                        var completeType = genType.MakeGenericType(innerType);
                        return completeType;
                    }
                }
            }
            throw new ZscriptParseException($"找不到{predicateStr}的对应谓词类", curLineIndex, curLine);
        }
        List<string> parseArgs(string argsStr)
        {
            
           return argsStr.Split(",").ToList();
   
        }
       
        Knowledge processInnerKnowledge(Type predicateType, List<string> args)
        {
            Knowledge newKnowledge;
            var ctor = predicateType.GetConstructors()[0];

            List<object> newKnowledgeArgs = new();
            var parameters = predicateType.GetConstructors()[0].GetParameters();
            if (predicateType == typeof(Angle))
            {
                var pointList = getPointsFromStr(args[0]);
                args.RemoveAt(0);
                if (pointList.Count == 3)
                {
                    newKnowledgeArgs.Add(new List<Point>() { pointList[0] });
                    newKnowledgeArgs.Add(pointList[1]);
                    newKnowledgeArgs.Add(new List<Point>() { pointList[2] });
                }
                else
                    throw new ZscriptParseException("角参数错误", curLineIndex, curLine);
            }
            //特殊情况------------------------点列表
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(Point[]))
            {
                //判断是点列表
                var pointList = getPointsFromStr(args[0]);
                args.RemoveAt(0);
                newKnowledgeArgs.Add(pointList.ToArray());
            }
            //特殊情况------------------------全是点
            else if (parameters.ToList().TrueForAll(p => p.ParameterType == typeof(Point)))
            {
                //判断是点列表
                var pointList = getPointsFromStr(args[0]);
                args.RemoveAt(0);
                newKnowledgeArgs.AddRange(pointList);
            }
            else if (predicateType.IsGenericType)
            {
                //规定泛型只有一个泛型参数 且 必须在第一位
                var innerType = predicateType.GetGenericArguments()[0];
                var inner_knowledge = processInnerKnowledge(innerType, args);
                newKnowledgeArgs.Add(inner_knowledge);
                foreach (var parameter in parameters.Skip(1))
                {
                    if (parameter.ParameterType == typeof(Expr))
                    {
                        var arg = args[0];
                        args.RemoveAt(0);
                        var expr = parseExpr(args[0]);
                        newKnowledgeArgs.Add(expr);
                    }
                    else
                    {
                        Knowledge knowledgeRef = processInnerKnowledge(parameter.ParameterType, args);
                        newKnowledgeArgs.Add(knowledgeRef);
                    }
                }
            }
            else
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterType == typeof(Point))
                    {
                        var arg = args[0];
                        args.RemoveAt(0);
                        newKnowledgeArgs.Add(PointRefs[arg]);
                    }
                    else if (parameter.ParameterType == typeof(Expr))
                    {
                        var expr = parseExpr(args[0]);
                        var arg = args[0];
                        args.RemoveAt(0);
                        newKnowledgeArgs.Add(expr);
                    }
                    else if (parameter.ParameterType == typeof(string))
                    {
                        var arg = args[0];
                        args.RemoveAt(0);
                        newKnowledgeArgs.Add(arg);
                    }
                    else
                    {
                        var innerKnowledge = processInnerKnowledge(parameter.ParameterType, args);
                        newKnowledgeArgs.Add(innerKnowledge);
                    }
                }

            }
            newKnowledge = (Knowledge)ctor.Invoke(newKnowledgeArgs.ToArray());
            return AddProcessor.Add(newKnowledge);
        }
        private Knowledge combinePredicate_AND_Args_Then_Add(Type predicateType, List<string> args)
        {
            Knowledge newKnowledge;
            var ctor = predicateType.GetConstructors()[0];

            List<object> newKnowledgeArgs = new();
            var parameters = predicateType.GetConstructors()[0].GetParameters();
            //特殊情况------------------------等价类的角
            if (predicateType == typeof(Angle))
            {
                if (args.Count == 3)
                {
                    //判断是点列表
                    var edge1 = getPointsFromStr(args[0]);
                    var v = getPointsFromStr(args[1]);
                    var edge2 = getPointsFromStr(args[2]);
                    newKnowledgeArgs.AddRange([edge1, v[0], edge2]);
                }
                else if (args.Count == 1)
                {
                    var points = getPointsFromStr(args[0]);
                    newKnowledgeArgs.AddRange([new List<Point>() { points[0] }, points[1], new List<Point>() { points[2] }]);
                }
                else
                    throw new ZscriptParseException("角参数错误",curLineIndex,curLine);
            }
            //特殊情况------------------------等价类的角
            else if (predicateType == typeof(PointOnVerticalBisector))
            {
                var angle = processInnerKnowledge(typeof(Segment), new() { args[0] });
                newKnowledgeArgs.Add(angle);
                var edge1 = getPointsFromStr(args[1]);
                newKnowledgeArgs.Add(edge1.ToArray());
            }
            //特殊情况------------------------等价类的角
            else if (predicateType == typeof(PointOnAngularBisector))
            {
                var angle = processInnerKnowledge(typeof(Angle), new() { args[0] });
                newKnowledgeArgs.Add(angle);
                var edge1 = getPointsFromStr(args[1]);
                newKnowledgeArgs.Add(edge1.ToArray());
            }
            //特殊情况------------------------点列表
            else if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(Point[]))
            {
                //判断是点列表
                var pointList = getPointsFromStr(args[0]);
                newKnowledgeArgs.Add(pointList.ToArray());
            }
            //特殊情况------------------------全是点
            else if (parameters.ToList().TrueForAll(p => p.ParameterType == typeof(Point)))
            {
                //判断是点列表
                var pointList = getPointsFromStr(args[0]);
                newKnowledgeArgs.AddRange(pointList);
            }
            else if (predicateType.IsGenericType)
            {
                //规定泛型只有一个泛型参数 且 必须在第一位
                var innerType = predicateType.GetGenericArguments()[0];
                var inner_knowledge = processInnerKnowledge(innerType, args);
                newKnowledgeArgs.Add(inner_knowledge);
                foreach (var parameter in parameters.Skip(1))
                {
                    if (parameter.ParameterType == typeof(Expr))
                    {
                        var arg = args[0];
                        args.RemoveAt(0);
                        var expr = parseExpr(args[0]);
                        newKnowledgeArgs.Add(expr);
                    }
                    else
                    {
                        Knowledge knowledgeRef = processInnerKnowledge(parameter.ParameterType, args);
                        newKnowledgeArgs.Add(knowledgeRef);
                    }
                }
            }
            else
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterType == typeof(Point))
                    {
                        var arg = args[0];
                        args.RemoveAt(0);
                        newKnowledgeArgs.Add(PointRefs[arg]);
                    }
                    else if (parameter.ParameterType == typeof(Expr))
                    {
                        if (args[0] == "value")
                        {
                            var expr = Expr.FromString(args[0]);
                            args.RemoveAt(0);
                            newKnowledgeArgs.Add(expr);
                        }
                        else
                        {
                            var expr = parseExpr(args[0]);
                            expr = expr.Simplify();
                            var arg = args[0];
                            args.RemoveAt(0);
                            newKnowledgeArgs.Add(expr);
                        }

                    }
                    else if (parameter.ParameterType == typeof(string))
                    {
                        var arg = args[0];
                        args.RemoveAt(0);
                        newKnowledgeArgs.Add(arg);
                    }
                    else
                    {
                        var innerKnowledge = processInnerKnowledge(parameter.ParameterType, args);
                        newKnowledgeArgs.Add(innerKnowledge);
                    }
                }

            }
            newKnowledge = (Knowledge)ctor.Invoke(newKnowledgeArgs.ToArray());
            //return AddProcessor.Add(newKnowledge);
            return newKnowledge;
        }
        #endregion

        #region 解析表达式

        public class ZExprTokenizer
        {
            private readonly string _expression;
            private int _position;
            public ZExprTokenizer(string expression)
            {
                _expression = expression;
                _position = 0;
            }
            public string NextToken()
            {
                while (_position < _expression.Length && char.IsWhiteSpace(_expression[_position]))
                {
                    _position++;
                }

                if (_position == _expression.Length)
                {
                    return null;
                }

                if (char.IsDigit(_expression[_position]) || _expression[_position] == '.')
                {
                    return ReadNumber();
                }

                if (char.IsLetter(_expression[_position]))
                {
                    return ReadIdentifier();
                }

                return _expression[_position++].ToString();
            }

            private string ReadNumber()
            {
                int start = _position;
                while (_position < _expression.Length &&
                       (char.IsDigit(_expression[_position]) || _expression[_position] == '.'))
                {
                    _position++;
                }
                return _expression.Substring(start, _position - start);
            }

            private string ReadIdentifier()
            {
                int start = _position;
                while (_position < _expression.Length && char.IsLetterOrDigit(_expression[_position]))
                {
                    _position++;
                }
                return _expression.Substring(start, _position - start);
            }
        }

        public class ZExprParser
        {
            public Func<string, Mut> ParserMutFunc { get; set; }
            private readonly ZExprTokenizer _tokenizer;
            private string _currentToken;
            public ZExprParser(string expression)
            {
                _tokenizer = new ZExprTokenizer(expression);
                NextToken();
            }

            private void NextToken()
            {
                _currentToken = _tokenizer.NextToken();
            }

            public Expr Parse()
            {
                return ParseExpression();
            }

            private Expr ParseExpression()
            {
                SumNode sum = new SumNode();
                sum.Addends.Add(ParseTerm());

                while (_currentToken == "+" || _currentToken == "-")
                {
                    bool isPositive = _currentToken == "+";
                    NextToken();
                    if (isPositive)
                    {
                        sum.Addends.Add(ParseTerm());
                    }
                    else
                    {
                        sum.Subtrahends.Add(ParseTerm());
                    }

                }
                return sum.Addends.Count + sum.Subtrahends.Count == 1 ?
                    sum.Addends.Count == 1 ? sum.Addends[0] : sum.Subtrahends[0] : sum;
            }

            private Expr ParseTerm()
            {
                ProductNode product = new ProductNode();
                Expr factor = ParseFactor();
                product.Multipliers.Add(factor);

                while (_currentToken == "*" || _currentToken == "/")
                {
                    bool isDivision = _currentToken == "/";
                    NextToken();
                    if (isDivision)
                    {
                        product.Divisors.Add(ParseTerm());
                    }
                    else
                    {
                        product.Multipliers.Add(ParseTerm());
                    }
                }

                return product.Multipliers.Count + product.Divisors.Count == 1 && product.IsPositive ?
                    product.Multipliers.Count == 1 ? product.Multipliers[0] : product.Divisors[0] : product;
            }

            private Expr ParseFactor()
            {
                Expr baseExpr = ParsePrimary(); // 解析基础表达式（数字、变量或括号）  

                while (_currentToken == "^") // 检查是否是指数运算符  
                {
                    NextToken();
                    Expr exponent = ParseFactor(); // 右结合：递归解析右侧的指数  
                    var powerNode = new PowerNode
                    {
                        Base = baseExpr,
                        Exponent = exponent
                    };
                    baseExpr = powerNode; // 更新基础表达式为指数节点  
                }

                return baseExpr;
            }

            private Expr ParsePrimary()
            {
                if (_currentToken == "(")
                {
                    NextToken();
                    Expr node = ParseExpression();
                    if (_currentToken != ")")
                        throw new ArgumentException("Missing closing parenthesis");
                    NextToken();
                    return node;
                }

                if (_currentToken == "-")
                {
                    NextToken();
                    ProductNode node = new ProductNode();
                    node.IsPositive = false;
                    node.Multipliers.Add(ParseTerm());
                    return node;
                }

                if (int.TryParse(_currentToken, out int number))
                {
                    NextToken();
                    return new IntNode(number);
                }

                if (_currentToken == "sin" || _currentToken == "cos" || _currentToken == "tan")
                {

                    string function = _currentToken;
                    NextToken();
                    if (_currentToken != "(")
                        throw new ArgumentException($"Missing opening parenthesis after {function}");
                    NextToken();
                    Expr argument = ParseExpression();
                    if (_currentToken != ")")
                        throw new ArgumentException($"Missing closing parenthesis after {function} argument");
                    NextToken();

                    if (function == "sin")
                    {
                        var node = new SinNode();
                        node.Expr = argument;
                        return node;
                    }
                    else if (function == "cos")
                    {
                        var node = new CosNode();
                        node.Expr = argument;
                        return node;
                    }
                    else if (function == "tan")
                    {
                        var node = new TanNode();
                        node.Expr = argument;
                        return node;
                    }
                    else if (function == "abs")
                    {
                        //var node = new AbsNode();
                        //node.Expr = argument;
                        //return node;
                    }
                    else if (function == "sqrt")
                    {
                        var node = new PowerNode() { Exponent = Expr.AHalf };
                        node.Base = argument;
                        return node;
                    }
                    throw new ArgumentException();
                }

                if (char.IsLetter(_currentToken[0]))
                {
                    string variable = _currentToken;
                    NextToken();
                    return ParserMutFunc(variable);
                    var v = new NameNode(variable);
                    NameNode varNode = new NameNode(variable);
                    return varNode;
                }

                throw new ArgumentException($"Unexpected token: {_currentToken}");
            }


        }
        #endregion
    }
}
