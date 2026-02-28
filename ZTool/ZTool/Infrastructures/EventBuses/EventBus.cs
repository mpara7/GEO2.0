namespace ZTool.Infrastructures.EventBuses
{
    public class QEventBus
    {
        public static ZEventBus EventBus { get; set; } = new ZEventBus();
        /// <summary>
        /// 默认会有一个事件管理器
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            if (EventBus == null)
                throw new InvalidOperationException("在未指定全局事件管理器（EventBus.QEventBus is null）时使用了事件管理器");
            lock (EventBus?.Events)
            {
                EventBase value;
                //如果没有则创建
                if (!EventBus.Events.TryGetValue(typeof(TEventType), out value))
                {
                    TEventType val = new TEventType();
                    EventBus.Events[typeof(TEventType)] = val;
                    return val;
                }
                return (TEventType)value;
            }
        }
        public static void Publish<TEventType>() where TEventType : Event, new()
        {
            GetEvent<TEventType>().Publish();
        }
        public static void Subscribe<TEventType>(Action action) where TEventType : Event, new()
        {
            GetEvent<TEventType>().Subscribe(action);
        }
    }
    /// <summary>
    /// 模块间通讯，模块只关心是否出现事件，不关心谁发布
    /// </summary>
    public class ZEventBus
    {
        public readonly Dictionary<Type, EventBase> Events = new();
        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            lock (Events)
            {
                EventBase value;
                //如果没有则创建
                if (!Events.TryGetValue(typeof(TEventType), out value))
                {
                    TEventType val = new TEventType();
                    Events[typeof(TEventType)] = val;
                    return val;
                }
                return (TEventType)value;
            }
        }
    }
}
