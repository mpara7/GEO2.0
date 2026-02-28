using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoInferenceEngine.PredicateShared.Models.Exprs.MapleExprs
{
    static class MapleEngine
    {
        // interface callback definitions
        public delegate void TextCallBack(nint data, int tag, nint output);
        public delegate void ErrorCallBack(nint data, nint offset, nint msg);
        public delegate void StatusCallBack(nint data, nint used, nint alloc, double time);
        public delegate nint ReadLineCallBack(nint data, nint debug);
        public delegate long RedirectCallBack(nint data, nint name, nint mode);
        public delegate nint StreamCallBack(nint data, nint stream, int nargs, nint args);
        public delegate long QueryInterrupt(nint data);
        public delegate nint CallBackCallBack(nint data, nint output);

        public struct MapleCallbacks
        {
            public TextCallBack textCallBack;
            public ErrorCallBack errorCallBack;
            public StatusCallBack statusCallBack;
            public ReadLineCallBack readlineCallBack;
            public RedirectCallBack redirectCallBack;
            public StreamCallBack streamCallBack;
            public QueryInterrupt queryInterrupt;
            public CallBackCallBack callbackCallBack;
        }
        // OpenMaple API methods (there are many more commands in the API
        [DllImport(@"maplec.dll")]
        public static extern nint StartMaple(int argc, string[] argv, ref MapleCallbacks cb, nint data, nint info, byte[] err);
        [DllImport(@"maplec.dll")]
        public static extern nint EvalMapleStatement(nint kv, byte[] statement);
        [DllImport(@"maplec.dll")]
        public static extern nint IsMapleStop(nint kv, nint obj);
        [DllImport(@"maplec.dll")]
        public static extern void StopMaple(nint kv);
        [DllImport(@"maplec.dll")]
        public static extern void RestartMaple(nint kv, byte[] err);
    }
    public class MapleApp
    {
        static MapleApp App;
        public static MapleApp GetMapleApp()
        {
            if (App is null)
            {
                App = new MapleApp();
            }
            else
            {
                App.RestartMaple();
            }
            return App;
        }
        public MapleApp()
        {
            InitMaple();
            Run("with(RealDomain);");
        }

        #region 初始化属性
        MapleEngine.MapleCallbacks cb;
        byte[] err = new byte[2048];
        nint kv;

        string[] argv = new string[2] { "maple", "-A2" };
        #endregion

        #region 运行时属性
        static bool HasError;
        static string Error;
        static string Result;
        #endregion

        public void InitMaple()
        {
            cb.textCallBack = cbText;
            cb.errorCallBack = cbError;
            cb.statusCallBack = null;
            cb.readlineCallBack = null;
            cb.redirectCallBack = null;
            cb.streamCallBack = null;
            cb.queryInterrupt = null;
            cb.callbackCallBack = null;

            try
            {
                kv = MapleEngine.StartMaple(2, argv, ref cb, nint.Zero, nint.Zero, err);
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
            catch (EntryPointNotFoundException e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

            // make sure we got a good kernel vector handle back
            if (kv.ToInt64() == 0)
            {
                // Maple didn't start properly.  The "err" parameter will be filled
                // in with the reason why (usually a license error)
                // Note that since we passed in a byte[] array we need to trim
                // the characters past \0 during conversion to string
                Console.WriteLine("Fatal Error, could not start Maple: "
                        + Encoding.ASCII.GetString(err, 0, Array.IndexOf(err, (byte)0))
                    );
                return;
            }

            // set the default plot driver to something nicer than "char"
            MapleEngine.EvalMapleStatement(kv, Encoding.ASCII.GetBytes("plotsetup(maplet):"));
        }
        public void EndMaple()
        {
            MapleEngine.StopMaple(kv);
        }
        public void RestartMaple()
        {
            MapleEngine.EvalMapleStatement(kv, Encoding.ASCII.GetBytes("restart;"));
            // MapleEngine.RestartMaple(kv, err);
        }

        #region 回调函数
        // When evaluating something Maple will send all displayed
        // output through this function.
        public static void cbText(nint data, int tag, nint output)
        {
            //Console.WriteLine("tag is " + tag);
            string s = Marshal.PtrToStringUTF8(output);
            Result = s;
        }

        // When evaluating something Maple will send errors through this function.
        // If not defined, errors will go through the text callback.
        public static void cbError(nint data, nint offset, nint msg)
        {
            //Console.WriteLine("offset is " + offset.ToInt32() );
            HasError = true;
            string s = Marshal.PtrToStringAnsi(msg);
            Error = s;
        }

        // When evaluating something Maple will send a message about resources
        // used once per garbage collection.  If you don't want to see these
        // messages, just comment out the WriteLine command inside.
        public static void cbStatus(nint data, nint used, nint alloc, double time)
        {
            Console.WriteLine("[cbStatus] -> cputime=" + time
              + "; memory used=" + used.ToInt64() + "kB"
              + " alloc=" + alloc.ToInt64() + "kB"
            );
        }
        #endregion


        /// <summary>
        /// 例：|a1|=>abs(a1)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string ConvertAbsExpr(string command)
        {
            var abses = Regex.Matches(command, "\\|[\\s\\S]*\\|");
            foreach (var abs in abses)
            {
                string content = abs.ToString().Trim('|');
                content = $"abs({content})";
                command = command.Replace(abs.ToString(), content);
            }
            return command;
        }

        /// <summary>
        /// 除以0返回无限
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string Run(string command)
        {
            HasError = false;
            command = ConvertAbsExpr(command);

            nint val = MapleEngine.EvalMapleStatement(kv, Encoding.UTF8.GetBytes(command + ";"));
            if (HasError)
            {
                if (Error.Contains("division by zero"))
                    return "infinity";
                else
                    return Error;
            }
            else
            {
                return Result;
            }

        }

    }
}
