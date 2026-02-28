using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.Knowledges
{
    public class Var:Mut
    {
        public string Name { get; }
        public Var(string name,bool record=true) : base()
        {
            Name = name;
            if (record)
            {
                Mut.Record.Invoke(this);
            }
            
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
