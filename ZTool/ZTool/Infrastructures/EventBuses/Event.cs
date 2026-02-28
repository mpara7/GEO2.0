namespace ZTool.Infrastructures.EventBuses;
public abstract class Event : EventBase
{
    public void Publish()
    {
        base.InternalPublish();
    }
    public void Subscribe(Action action)
    {
        base.InternalSubscribe(new EventSubscription(() => action()));
    }
}
public abstract class Event<T1> : EventBase
{
    public void Publish(T1 arg1)
    {
        base.InternalPublish(arg1);
    }
    public void Subscribe(Action<T1> action)
    {
        base.InternalSubscribe(new EventSubscription<T1>((args) => action(args)));
    }
}
public abstract class Event<T1, T2> : EventBase
{
    public void Publish(T1 arg1, T2 arg2)
    {
        base.InternalPublish(arg1, arg2);
    }
    public void Subscribe(Action<T1, T2> action)
    {
        base.InternalSubscribe(new EventSubscription<T1, T2>(
            (arg1, arg2) => action(arg1, arg2)));
    }
}
public abstract class Event<T1, T2, T3> : EventBase
{
    public void Publish(T1 arg1, T2 arg2, T3 arg3)
    {
        base.InternalPublish(arg1, arg2, arg3);
    }
    public void Subscribe(Action<T1, T2, T3> action)
    {
        base.InternalSubscribe(new EventSubscription<T1, T2, T3>(
            (arg1, arg2, arg3) => action(arg1, arg2, arg3)));
    }
}
public abstract class Event<T1, T2, T3, T4> : EventBase
{
    public void Publish(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        base.InternalPublish(arg1, arg2, arg3, arg4);
    }
    public void Subscribe(Action<T1, T2, T3, T4> action)
    {
        base.InternalSubscribe(new EventSubscription<T1, T2, T3, T4>(
            (arg1, arg2, arg3, arg4) => action(arg1, arg2, arg3, arg4)));
    }
}
public abstract class Event<T1, T2, T3, T4, T5> : EventBase
{
    public void Publish(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        base.InternalPublish(arg1, arg2, arg3, arg4, arg5);
    }
    public void Subscribe(Action<T1, T2, T3, T4, T5> action)
    {
        base.InternalSubscribe(new EventSubscription<T1, T2, T3, T4, T5>(
            (arg1, arg2, arg3, arg4, arg5) => action(arg1, arg2, arg3, arg4, arg5)));
    }
}
public abstract class Event<T1, T2, T3, T4, T5, T6> : EventBase
{
    public void Publish(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        base.InternalPublish(arg1, arg2, arg3, arg4, arg5, arg6);
    }
    public void Subscribe(Action<T1, T2, T3, T4, T5, T6> action)
    {
        base.InternalSubscribe(new EventSubscription<T1, T2, T3, T4, T5, T6>(
            (arg1, arg2, arg3, arg4, arg5, arg6) => action(arg1, arg2, arg3, arg4, arg5, arg6)));
    }
}


