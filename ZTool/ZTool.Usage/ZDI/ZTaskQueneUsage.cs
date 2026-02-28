using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZTool.Infrastructures.TaskQuene;

namespace ZTool.Usage.ZDI
{
    internal class ZTaskQueneUsage
    {
        class AnySecWork : ZTask
        {
            int waitSec = 3;
            public AnySecWork(int waitSec)
            {

                this.waitSec = waitSec;

            }
            public override void Run()
            {
                base.Run();
                Thread.Sleep(waitSec*1000);
                Console.WriteLine(Id+" has done.");
            }
        }
        public static void Usage()
        {
            ZTaskQuene<AnySecWork> taskQuene = new ();
            taskQuene.Start();
            taskQuene.AddTask(new AnySecWork(1));
            taskQuene.AddTask(new AnySecWork(2));
            taskQuene.AddTask(new AnySecWork(3));
            taskQuene.AddTask(new AnySecWork(4));
        }
    }
}
