namespace GeoInferenceEngine.Backbone.AppBuilder
{
    internal class EngineBuilder
    {
        public ZDIContainer container { get; set; } = new();
        EngineConfig config;
        IEnumerable<AInferenceInput> inputs;
        public IEnginePreparer EnginePreparer { get; set; }
        public IInferenceEngine Engine { get; set; }
        public IEngineOutputGetter EngineOutputGetter { get; set; }

        #region 配置
        public EngineBuilder SetInput(IEnumerable<AInferenceInput> inputs)
        {
            this.inputs = inputs;
            return this;
        }
        public EngineBuilder SetEngineConfig(EngineConfig config)
        {
            this.config = config;
            return this;
        }
        #endregion

        #region 构建推理引擎组件
        void buildDataBases()
        {
            foreach (var database in config.DataBases)
            {
                var componentType1 = Type.GetType(database.actualType);
                var componentType2 = Type.GetType(database.targetType);
                container.SetSingleton(componentType1, componentType2);
            }
        }
        void buildRunningConfigs()
        {
            foreach (var config in config.Configs)
            {
                var type = Type.GetType(config.type);

                var obj = YAML.Deserialize(type, config.yaml);
                container.SetSingleton(obj, type);
            }
        }
        void buildComponents()
        {
            foreach (var component in config.Components)
            {
                var componentType1 = Type.GetType(component.actualType);
                var componentType2 = Type.GetType(component.targetType);
                container.SetSingleton(componentType1, componentType2);
            }
        }
        /// <summary>
        /// plugIn就不允许继承了
        /// </summary>
        void buildStandPlugIns()
        {
            foreach (var plugIn in config.PlugIns)
            {
                var plugInType = Type.GetType(plugIn);
                container.SetSingleton(plugInType, plugInType);
            }
        }
        #endregion

        #region 建造输出
        public List<IInferenceOutputMaker<AInferenceOutput>> ProcessInfoMakers = new();
        public List<IInferenceOutputMaker<AInferenceOutput>> ResultMakers = new();
        void buildProcessInfoGetter()
        {
            foreach (var getterConfig in config.ProcessInfoGetters)
            {
                foreach (var plugInTypeStr in getterConfig.PlugIns)
                {
                    var plugInType = Type.GetType(plugInTypeStr);
                    container.Set(plugInType);
                }
                ZDIContainer subContainer = new ZDIContainer(container);
                var getterType = Type.GetType(getterConfig.TypeName);
                subContainer.Set(getterType);
                foreach (var kv in getterConfig.Configs)
                {
                    var configType = Type.GetType(kv.Key);
                    var config = YAML.Deserialize(configType, kv.Value);
                    subContainer.SetSingleton(config, configType);
                }


                var getter = (IInferenceOutputMaker<AInferenceOutput>)subContainer.Get(getterType);
                getter.Name = getterConfig.TypeName;


                container.SetSingleton(getter, typeof(IInferenceOutputMaker<AInferenceOutput>));
                ProcessInfoMakers.Add(getter);
            }
        }
        void buildResultGetter()
        {
            foreach (var getterConfig in config.ResultGetters)
            {
                foreach (var plugInTypeStr in getterConfig.PlugIns)
                {
                    var plugInType = Type.GetType(plugInTypeStr);
                    container.Set(plugInType);
                }
                ZDIContainer subContainer = new ZDIContainer(container);
                var getterType = Type.GetType(getterConfig.TypeName);
                subContainer.Set(getterType);
                foreach (var kv in getterConfig.Configs)
                {
                    var configType = Type.GetType(kv.Key);
                    var config = YAML.Deserialize(configType, kv.Value);
                    subContainer.SetSingleton(config, configType);
                }
                var getter = (IInferenceOutputMaker<AInferenceOutput>)subContainer.Get(getterType);
                getter.Name = getterConfig.TypeName;

                container.SetSingleton(getter, typeof(IInferenceOutputMaker<AInferenceOutput>));
                ResultMakers.Add(getter);
            }
        }
        void injectGettersToModule()
        {
            foreach (var getter in ProcessInfoMakers)
            {
                EngineOutputGetter.AddProcessingInfoMaker(getter);
            }
            foreach (var getter in ResultMakers)
            {
                EngineOutputGetter.AddResultMaker(getter);
            }
        }
        List<IInferencePlugIn> inferencePlugIns { get; set; } = new List<IInferencePlugIn>();
        /// 插件的创建之间会干扰
        /// <summary>
        /// 将实现插件接口的组件 当作插件 //不再支持
        /// 加入到使用那些插件的组件中
        /// </summary>
        void setPlugIn()
        {
            //组件类型 可插入的类型
            List<(Type, object)> dict = new();
            //收集
            foreach (var getter in container.Getters)
            {
                if (getter is SignletonGetter sc)
                {
                    var interfaces = sc.ActualType.GetInterfaces();
                    foreach (var iface in interfaces)
                    {
                        if (iface.IsAssignableTo(typeof(IInferencePlugIn)))
                        {
                            inferencePlugIns.Add((IInferencePlugIn)container.GetWithoutDI(getter.TargetType));
                        }

                    }
                }
            }
            foreach (var kv in dict)
            {
                container.SetSingleton(kv.Item2, kv.Item1);

            }
        }
        private void PsetGetterAsPlugIn()
        {
            //收集Getter实现PlugIn接口的信息
            foreach (var getter in EngineOutputGetter.ProcessingInfoMakers.Union(EngineOutputGetter.ResultMakers))
            {
                var interfaces = getter.GetType().GetInterfaces();
                foreach (var iface in interfaces)
                {
                    if (iface.IsAssignableTo(typeof(IInferencePlugIn)))
                    {
                        if (iface != typeof(IInferencePlugIn))
                        {
                            container.SetSingleton(getter, iface);
                        }
                    }
                }
            }
            //检查每个组件重新注入
            foreach (var getter in container.Getters)
            {
                if (getter is SignletonGetter sc)
                {
                    container.InjectDI(sc.Get());
                }
            }
        }
        #endregion
        void buildInputs()
        {
            foreach (var input in inputs)
            {
                container.SetSingleton(input, input.GetType());
            }
        }
        void buildEnginePreparer()
        {
            var engineType = Type.GetType(config.EnginePreparer);
            container.SetSingleton(engineType, typeof(IEnginePreparer));
            EnginePreparer = container.Get<IEnginePreparer>();
        }
        void buildEngine()
        {
            var enginePreparerType = Type.GetType(config.EnginePreparer);
            container.SetSingleton(enginePreparerType, typeof(IEnginePreparer));

            var engineType = Type.GetType(config.Engine);
            container.SetSingleton(engineType, typeof(IInferenceEngine));

            var engineOutputGetterType = Type.GetType(config.EngineOutputGetter);
            container.SetSingleton(engineOutputGetterType, typeof(IEngineOutputGetter));

            EnginePreparer = container.Get<IEnginePreparer>();
            Engine = container.Get<IInferenceEngine>();
            EngineOutputGetter = container.Get<IEngineOutputGetter>();
        }
        void buildEngineResultGetter()
        {
            var engineType = Type.GetType(config.EngineOutputGetter);
            container.SetSingleton(engineType, typeof(IEngineOutputGetter));
            EngineOutputGetter = container.Get<IEngineOutputGetter>();
        }
        public (IEnginePreparer preparer, IInferenceEngine engine, IEngineOutputGetter resultGetter) Build()
        {
            buildInputs();
            //设置引擎组件
            buildStandPlugIns();
            buildDataBases();
            buildRunningConfigs();
            buildComponents();
            buildProcessInfoGetter();
            buildResultGetter();

            buildEngine();
            

            setPlugIn();
            //这个是给插件添加依赖关系的
            foreach (var item in inferencePlugIns)
            {
                container.InjectDI(item);
                item.Init();
            }

            //设置输出
            injectGettersToModule();
            PsetGetterAsPlugIn();

            return (EnginePreparer, Engine, EngineOutputGetter);
        }

    }
}
