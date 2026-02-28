namespace ZTool.Infrastructures.ZDI.Creators
{
    public delegate object DelegateFunc();
    public class DelegateGetter : AGetter
    {
        DelegateFunc func;
        public DelegateGetter(DelegateFunc func)
        {
            this.func = func;
            ActualType = func.Method.ReturnType;
        }

        public override object Get()
        {
            return func.Invoke();
        }

    }
}
