namespace ZTool.Infrastructures.ZDI.Creators
{
    public class InstanceSignletonGetter : AGetter
    {
        object instance;
        public InstanceSignletonGetter(object instance)
        {
            this.instance = instance;
        }
        public override string ToString()
        {
            return $"{this.ActualType}->{this.TargetType}";
        }
        public override object Get()
        {
            return instance;
        }

    }
}
