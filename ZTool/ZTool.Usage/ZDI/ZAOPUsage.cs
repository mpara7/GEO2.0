using System.Diagnostics;
using ZTool.Infrastructures.AOP;

namespace ZTool.Usage.ZDI
{
    public class MethodTimmer : InvokerAttribute
    {
        public override void Invoke(InvocationContext invokeContext)
        {
            Console.WriteLine("开始计时");
            Stopwatch timmer = new();
            timmer.Start();
            Next();
            timmer.Stop();
            Console.WriteLine(timmer.ElapsedMilliseconds);
            Console.WriteLine("结束计时");

        }
    }
    public class MethodLogger : InvokerAttribute
    {
        public override void Invoke(InvocationContext invokeContext)
        {
            Console.WriteLine("开始执行");
            Next();
            Console.WriteLine("结束执行");
        }
    }
    public class TimesTwo : InvokerAttribute
    {
        public override void Invoke(InvocationContext invokeContext)
        {
            Next();
            //修改返回结果
            invokeContext.ReturnValue = ((int)invokeContext.ReturnValue) * 2;
        }
    }
    public class Base
    {
        /// <summary>
        /// 支持修改值类型与引用类型
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodLogger]
        [TimesTwo]
        public virtual int ADD(int a, int b)
        {
            return a + b;
        }

        [MethodTimmer]
        [MethodLogger]
        public virtual void Say(int a, object obj)
        {
            Thread.Sleep(10);
            Console.WriteLine($"Say {a} {obj}");
        }
    }


    internal class ZAOPUsage
    {
        public static void Usage()
        {
            var a = ZAOP.Build<Base>();
            a.Say(9, "dd");
        }
    }
}
