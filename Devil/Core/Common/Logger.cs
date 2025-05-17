using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilInfinite.Devil.Core.Common
{
    public class Logger : ILogger
    {

        static readonly Logger logerr = new Logger();
        public void Info(string message)
        {
            var methodInfo = new StackTrace().GetFrame(2).GetMethod();
            var className = methodInfo.ReflectedType.Name;
            GD.Print($"[{className}] {message}");
        }

        public static void Log(string message)
        {
            logerr.Info(message);
        }
    }
}
