using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ZTool.UsefulTypes;
/// <summary>
/// using var _ = new Defer(()=>Console.WriteLine("a"));
/// </summary>
public class Defer : IDisposable
{
    Action Action;
    public Defer(Action action)
    {
        Action = action;
    }
    public void Dispose()
    {
        Action();
    }
}
