namespace ZTool.Infrastructures.EventBuses
{
    public abstract class AEventSubscription
    {
        public abstract Action<object[]> GetAction();
    }
    internal class EventSubscription : AEventSubscription
    {
        private Action Action { get; set; }
        public EventSubscription(Action action)
        {
            Action = action;
        }
        public override Action<object[]> GetAction()
        {
            return (objs) => Action();
        }
    }
    internal class EventSubscription<T> : AEventSubscription
    {
        private Action<T> Action { get; set; }
        public EventSubscription(Action<T> action)
        {
            Action = action;
        }
        public override Action<object[]> GetAction()
        {
            return (objs) => Action((T)objs[0]);
        }
    }
    internal class EventSubscription<T1, T2> : AEventSubscription
    {
        private Action<T1, T2> Action { get; set; }
        public EventSubscription(Action<T1, T2> action)
        {
            Action = action;
        }
        public override Action<object[]> GetAction()
        {
            return (objs) => Action((T1)objs[0], (T2)objs[1]);
        }
    }
    internal class EventSubscription<T1, T2, T3> : AEventSubscription
    {
        private Action<T1, T2, T3> Action { get; set; }
        public EventSubscription(Action<T1, T2, T3> action)
        {
            Action = action;
        }
        public override Action<object[]> GetAction()
        {
            return (objs) => Action((T1)objs[0], (T2)objs[1], (T3)objs[2]);
        }
    }
    internal class EventSubscription<T1, T2, T3, T4> : AEventSubscription
    {
        private Action<T1, T2, T3, T4> Action { get; set; }
        public EventSubscription(Action<T1, T2, T3, T4> action)
        {
            Action = action;
        }
        public override Action<object[]> GetAction()
        {
            return (objs) => Action((T1)objs[0], (T2)objs[1], (T3)objs[2], (T4)objs[3]);
        }
    }
    internal class EventSubscription<T1, T2, T3, T4, T5> : AEventSubscription
    {
        private Action<T1, T2, T3, T4, T5> Action { get; set; }
        public EventSubscription(Action<T1, T2, T3, T4, T5> action)
        {
            Action = action;
        }
        public override Action<object[]> GetAction()
        {
            return (objs) => Action((T1)objs[0], (T2)objs[1], (T3)objs[2], (T4)objs[3], (T5)objs[4]);
        }
    }
    internal class EventSubscription<T1, T2, T3, T4, T5, T6> : AEventSubscription
    {
        private Action<T1, T2, T3, T4, T5, T6> Action { get; set; }
        public EventSubscription(Action<T1, T2, T3, T4, T5, T6> action)
        {
            Action = action;
        }
        public override Action<object[]> GetAction()
        {
            return (objs) => Action((T1)objs[0], (T2)objs[1], (T3)objs[2], (T4)objs[3], (T5)objs[4], (T6)objs[5]);
        }
    }

}
