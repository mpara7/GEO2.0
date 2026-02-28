using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTool.Infrastructures.AutoMapper;

namespace ZTool.Usage.ZDI
{
    internal class ZMapperUsage
    {
        public class SubFromClass
        {
            public string Prop { get; set; } = "sub_from";
        }
        public class FromClass
        {
            public int Id { get; set; } = 60;
            public string Prop { get; set; } = "from";
            public SubFromClass Sub { get; set; } = new SubFromClass();
        }
        public class SubToClass
        {
            public string Prop { get; set; } = "from";
        }
        public class ToClass 
        {
            public string Id { get; set; }
            public string Prop { get; set; }
            public string SubStr { get; set; }
            public SubToClass Sub { get; set; }
        }    

        public static void Usage()
        {
            ZMapper mapper = new ZMapper();
            mapper.CreateMap<FromClass,ToClass>().MapProp(f=>f.Sub,t=>t.SubStr);
            mapper.CreateMap<SubFromClass, SubToClass>();

            mapper.AddMapFunc<int, string>(i=>i.ToString()+"+10086");
            mapper.AddMapFunc<SubFromClass, string>(i => i.Prop+ "+10086");
            
            ToClass toClass=  mapper.Map<FromClass,ToClass>(new FromClass());

        }
    }
}
