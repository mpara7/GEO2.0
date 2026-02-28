namespace ZTool.Infrastructures.EventBuses;
public abstract class EventBase
{

    private readonly List<AEventSubscription> _subscriptions = new List<AEventSubscription>();
    /// <summary>
    /// 获取所有订阅
    /// </summary>
    /// <value>The current subscribers.</value>
    protected ICollection<AEventSubscription> Subscriptions => _subscriptions;

    private List<Action<object[]>> GetSubscriptions()
    {
        return Subscriptions.Select(subscriptions => subscriptions.GetAction()).ToList();
    }
    /// <summary>
    /// 通用发布
    /// </summary>
    /// <param name="arguments">The arguments that will be passed to the listeners.</param>
    protected virtual void InternalPublish(params object[] arguments)
    {
        foreach (Action<object[]> item in GetSubscriptions())
        {
            item(arguments);
        }
    }
    /// <summary>
    /// 通用订阅
    /// </summary>
    /// <param name="eventSubscription"></param>
    protected virtual void InternalSubscribe(AEventSubscription eventSubscription)
    {
        lock (Subscriptions)
        {
            Subscriptions.Add(eventSubscription);
        }
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
