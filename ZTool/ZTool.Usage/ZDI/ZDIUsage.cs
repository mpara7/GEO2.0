using ZTool.Infrastructures.ZDI;

namespace ZTool.Usage.ZDI
{
    class Engine
    {
        [ZDI]
        KnowledgeBase kb;
        [ZDI]
        Excutor excutor { get; set; }
        public void Do()
        {
            excutor.Do();
        }
    }
    class KnowledgeBase
    {
        public List<string> strings = new List<string>();
    }
    class Excutor
    {
        [ZDI]
        KnowledgeBase kb;
        [ZDI]
        PairMaker pairMaker { get; set; }
        public void Do()
        {
            kb.strings.Add("a");
            pairMaker.MakePair();
        }
    }
    class PairMaker
    {
        [ZDI]
        KnowledgeBase kb;
        public void MakePair()
        {
            Console.WriteLine(kb.strings[0]);
        }
    }
    internal class ZDIUsage
    {
        public static void Usage()
        {
            ZDIContainer container = new ZDIContainer();
            container.SetSingleton<KnowledgeBase>();
            container.SetSingleton<PairMaker>();
            container.SetSingleton<Excutor>();
            container.SetSingleton<Engine>();
            var engine = container.Get<Engine>();
            engine.Do();
        }
    }
}
