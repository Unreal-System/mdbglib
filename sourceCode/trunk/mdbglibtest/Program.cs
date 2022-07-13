using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using msdbg = MS.Debuggers.DbgEng;
using MS.Debuggers.DbgEng;
using System.IO;
using System.Threading;

namespace mdbglibtest
{
    class Program
    {
        static void Main(string[] args)
        {
            Debuggee dbg = new Debuggee();
            dbg.DebugOutput += new EventHandler<DebugOutputEventArgs>(dbg_DebugOutput);

            for (int i = 0; i < args.Length; i++)
            {
                string cmd = args[i];
                Console.WriteLine("> "+ cmd);
                HandleCommand(dbg, cmd);
            }

            WriteRuntimeHelp();
            bool keeprunning = true;
            while (keeprunning)
            {
                Console.Write("> ");
                string cmd = Console.ReadLine();
                if (!HandleCommand(dbg, cmd)) break;
            }
        }
        static bool HandleCommand(Debuggee dbg, string cmd)
        {
            try
            {
                if (cmd.StartsWith("$") || "exit" == cmd || "quit" == cmd || "?" == cmd || "/?" == cmd || "cls" == cmd)
                {
                    switch (cmd)
                    {
                        case "$quit":
                        case "$exit":
                        case "quit":
                        case "exit":
                            return false;
                        case "$what":
                            Console.WriteLine(dbg.GetExecutionStatus().ToString());
                            break;
                        case "$info":
                            DisplayDebuggeeInfo(dbg);
                            break;
                        case "$wait":
                            dbg.WaitForEvent(0);
                            break;
                        case "$cls":
                        case "cls":
                            Console.Clear();
                            break;
                        case "$help":
                        case "?":
                        case "/?":
                            WriteRuntimeHelp();
                            break;
                        default:
                            if (cmd.StartsWith("$opendump"))
                            {
                                string dumpFilePath = cmd.Substring("$opendump".Length + 1).Trim();
                                dbg.OpenDumpFile(dumpFilePath);
                                dbg.WaitForEvent(0);
                            }
                            else if (cmd.StartsWith("$sympath"))
                            {
                                string symPath = cmd.Substring("$sympath".Length + 1).Trim();
                                dbg.SymbolPath = symPath;
                            }
                            else
                            {
                                Console.WriteLine("Unknown command: " + cmd);
                                WriteRuntimeHelp();
                            }
                            break;
                    }
                }
                else
                {
                    dbg.Execute(cmd);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("!Exception " + e.Message);
            }

            return true;
        }
        static void DisplayDebuggeeInfo(Debuggee dbg) 
        {
            Console.WriteLine("#Processes = " + dbg.ProcessCount);
            Console.WriteLine("#Cur proc id = " + dbg.CurrentProcessId);
            Console.WriteLine("#Proc name = " + dbg.ProcessExecutableName);
        }
        static void dbg_DebugOutput(object sender, DebugOutputEventArgs e)
        {
            Console.WriteLine(e.Output);
        }
        static void WriteRuntimeHelp() 
        {
            Console.WriteLine("$quit     - quit");
            Console.WriteLine("$exit     - quit");
            Console.WriteLine("$help     - this message");
            Console.WriteLine("$what     - get execution status");
            Console.WriteLine("$info     - current state");
            Console.WriteLine("$opendump - open a dump file");
            Console.WriteLine("$sympath  - set symbol path");
            Console.WriteLine("$wait     - issues a wait command");
            Console.WriteLine("$cls      - clear screen");
            Console.WriteLine("");
            Console.WriteLine("Anything else will be sent to debugger engine");
        }
    }
}
