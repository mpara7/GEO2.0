using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using ZTool.Infrastructures.EventBuses;

namespace ZTool.Usage.ZDI
{
    internal class ZEventUsage
    {
        class CatShowTimes : Event<int>
        {
        }
        public static void Usage()
        {
            //使用全局
            QEventBus.GetEvent<CatShowTimes>().Subscribe(a=>Console.WriteLine(a));
            QEventBus.GetEvent<CatShowTimes>().Publish(1);
        }
    }
}
