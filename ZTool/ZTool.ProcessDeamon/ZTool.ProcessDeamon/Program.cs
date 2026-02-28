// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
//使用：
//启动守护程序 传入一个参数作为进程的id
//根据id获得程序地址
//在id进程关闭后 根据程序地址重新启动程序
//最后关闭守护程序
int id = 0;
//输入id处理
if (args.Length > 0)
{
    id = int.Parse(args[0]);
    Console.WriteLine($"需要守护的id是{id}");
}
else
{
    Console.WriteLine("没有id");
    return;
}

ManualResetEventSlim manuallock = new ManualResetEventSlim(false);
//分析程序位置
Process process = null;
string path = null;
string disk = null;
string dirPath = null;
string exe = null;
try
{
    process = Process.GetProcessById(id);
    path = process.MainModule.FileName;
    disk = Path.GetPathRoot(path).Replace("\\", "");
    dirPath = Path.GetDirectoryName(path);
    exe = Path.GetFileName(path);
}
catch (Exception ex)
{
    Console.WriteLine(ex.InnerException);
    Console.WriteLine(ex.StackTrace);
    Console.WriteLine(ex.Message);
    Console.WriteLine("分析 进程 出错->退出");
    return;
}
//当程序退出时重新启动
void P_Exited(object? sender, EventArgs e)
{
    Process process_cmd = new Process();
    process_cmd.StartInfo.FileName = "cmd.exe";
    process_cmd.StartInfo.RedirectStandardInput = true;//是否可以输入
    process_cmd.StartInfo.RedirectStandardOutput = true;//是否可以输出
    process_cmd.StartInfo.CreateNoWindow = true;//不创建窗体 也就是隐藏窗体
    process_cmd.StartInfo.UseShellExecute = false;//是否使用系统shell执行，否
    process_cmd.Start();

    process_cmd.StandardInput.WriteLine(@"cd\");
    process_cmd.StandardInput.WriteLine(disk);
    process_cmd.StandardInput.WriteLine($"cd {dirPath}");
    process_cmd.StandardInput.WriteLine($"start {exe}");
    manuallock.Set();
}
process.EnableRaisingEvents = true;
process.Exited += P_Exited;

manuallock.Wait();

