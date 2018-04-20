using System;
using System.IO;
using System.Diagnostics;

namespace Ev3DevLib
{
    public static class TermController
    {
        public static StreamReader RunCommand(string command, string args)
        {
            Process p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.Arguments = args;
            p.StartInfo.FileName = command;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            p.Start();
            return p.StandardOutput;
        }
        public static StreamReader RunCommandWithPipe(StreamReader Pipe, string command,string args)
        {
            Process p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.Arguments = args;
            p.StartInfo.FileName = command;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            p.Start();
            Pipe.BaseStream.CopyToAsync(p.StandardInput.BaseStream);
            return p.StandardOutput;
        }
    }
}
