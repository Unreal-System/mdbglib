using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.IO;
using MS.Debuggers.DbgEng;
//using Darwen.Windows.Forms.Controls.Docking.Serialization;
using System.Threading;
using System.Xml.Serialization;

namespace ASDumpAnalyzer
{
    public partial class MainForm : Form
    {
        private Debuggee m_debuggee;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //DockingControlsPersister.Deserialize(this.myDockingManager1);
            
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            Settings s = null;
            try
            {
                if (File.Exists("ASDumpAnalyzer.xml"))
                {
                    XmlSerializer xmlSer = new XmlSerializer(typeof(Settings));
                    using (FileStream fs = File.OpenRead("ASDumpAnalyzer.xml"))
                    {
                        s = (Settings)xmlSer.Deserialize(fs);
                    }
                }
            }
            catch (Exception x)
            {
                s = null;
                MessageBox.Show("Unable to load settings\n" + x.ToString());
            }

            if (null != s)
            {
                this.textBoxDumpPath.Text = s.DumpFilePath;
                this.textBoxSymbols.Text = s.SymbolsPath;
            }
            else 
            {
                this.textBoxDumpPath.Text = @"C:\svn\mdbglib.svn.codeplex.com\trunk\dumps\267166\d32b6e07-d4a8-4125-933e-9956a4175aaf.mdmpFull.mdmp";
                this.textBoxSymbols.Text = @"C:\svn\mdbglib.svn.codeplex.com\trunk\dumps\267166\Symbols";
            }

            //this.textBoxDumpPath.Text = @"E:\share\dumps\255562\59e8b79e-8f90-4a35-8db9-3e4ada034f85.mdmpfull.mdmp";
            //this.textBoxSymbols.Text = @"E:\share\dumps\255562\10.00.9011.03";
            //Environment.GetEnvironmentVariable("_NT_SYMBOL_PATH"); //
            this.OpenDump();

        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DetachDump();
            //DockingControlsPersister.Serialize(this.myDockingManager1);
        }


        private void Test()
        {
            WriteLine("Event thread id: " + this.m_debuggee.EventThreadId);
            foreach (DbgFrame f in this.m_debuggee.GetEventStackTrace()) 
            {
                WriteLine(f.FunctionName);
            }
        }

        void debugger_DebugOutput(object sender, DebugOutputEventArgs e)
        {
            this.Write(e.Output);
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            Test();
        }

        private void buttonPrintStack_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.PrintThreadsInfo));
        }

        private static bool symbolsWarningDisplayed = false;
        private void AnalyzeStack(DbgFrame[] stack, string threadId) 
        {
            //WriteLine("" + t.ThreadId + " Id: " + t.ThreadSystemId);
            bool isQueryThread = false;
            foreach (DbgFrame f in stack)
            {
                //WriteLine(f.FunctionName);
                if (f.FunctionName == "msmdsrv!PXSession::InternalExecuteCommand")
                    isQueryThread = true;
                else if ((f.FunctionName == "msmdsrv") && (!symbolsWarningDisplayed))
                {
                    MessageBox.Show("Possibly the symbols for msmdsrv are not loaded. \nPlease make sure the symbols path contains the right information.");
                    symbolsWarningDisplayed = true;
                }
                this.stackProgressBar1.ActiveFrame++;
            }
            //WriteLine("==========================================");
            if (isQueryThread)
            {
                WriteLine("ThreadId=" + threadId);
                WriteLine(ASDump.GetQuery(stack, new UpdateProgressDelegate(this.UpdateProgress)));
            }
        }
        private void PrintThreadsInfo(object state) 
        {
            symbolsWarningDisplayed = false;

            this.stackProgressBar2.FrameCount = this.m_debuggee.Threads.Length;
            this.stackProgressBar2.ActiveFrame = 0;

            WriteLine("Event query:");
            AnalyzeStack(this.m_debuggee.GetEventStackTrace(), this.m_debuggee.EventThreadId.ToString());
            WriteLine("=============");

            foreach (DbgThread t in this.m_debuggee.Threads)
            //DbgThread t = this.m_debuggee.GetEventThread();
            {
                DbgFrame[] stack = t.StackTrace;
                this.stackProgressBar1.FrameCount = stack.Length;
                this.stackProgressBar1.ActiveFrame = 0;

                AnalyzeStack(stack, t.ThreadId.ToString());
                this.stackProgressBar2.ActiveFrame++;
            }
            WriteLine("");
            WriteLine("Done.");
        }

        private void DetachDump() 
        {
            if (null != this.m_debuggee)
            {
                this.m_debuggee.DebugOutput -= new EventHandler<DebugOutputEventArgs>(debugger_DebugOutput);
                this.m_debuggee.Dispose();
            }
        }
        private void OpenDump() 
        {
            if (null != this.m_debuggee)
                this.DetachDump();

            string dumpFilePath = this.textBoxDumpPath.Text;
            string symbolPath = this.textBoxSymbols.Text;

            if (!File.Exists(dumpFilePath)) { MessageBox.Show("Selected dump file does not exist"); return; }

            this.m_debuggee = Debuggee.OpenDumpFile(dumpFilePath, symbolPath);
            this.m_debuggee.DebugOutput += new EventHandler<DebugOutputEventArgs>(debugger_DebugOutput);
            //this.m_debuggee.Output((OutputModes)1, "Testing !!");
        }

        private void openCrashDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Dump files (*.mdmp;*.dmp)|*.mdmp;*.dmp|All files (*.*)|*.*";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK) 
            {
                this.textBoxDumpPath.Text = dlg.FileName;
                this.OpenDump();
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK) 
            {
                this.textBoxSymbols.Text = dlg.SelectedPath;
                this.OpenDump();
            }
        }

        private void buttonBrowseDump_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Dump files (*.mdmp;*.dmp)|*.mdmp;*.dmp|All files (*.*)|*.*";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.textBoxDumpPath.Text = dlg.FileName;
                this.OpenDump();
            }
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            this.OpenDump();
        }


        private void UpdateProgress(double p) 
        {
            this.stackProgressBar1.Progress = p;
        }

        private delegate void WriteTextDelegate(string text);

        private void Write(string text)
        {
            if (this.InvokeRequired)
                this.Invoke(new WriteTextDelegate(this.Write), text);
            else
                this.myConsole.AppendText(text);
        }
        private void WriteLine(string text)
        {
            this.Write(text + Environment.NewLine);
        }

        private void MainForm_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            try 
            {
                Settings s = new Settings();
                s.DumpFilePath = this.textBoxDumpPath.Text;
                s.SymbolsPath = this.textBoxSymbols.Text;

                XmlSerializer xmlSer = new XmlSerializer(typeof(Settings));
                using (FileStream fs = File.Create("ASDumpAnalyzer.xml")) 
                {
                    xmlSer.Serialize(fs, s);
                }
            }
            catch(Exception x)
            {
                MessageBox.Show("Unable to save settings\n"+x.ToString());
            }

        }



        #region Old Walk Stack
        //private void MyTestWalkStack(DebugClient client, DebugControl control, DebugSymbols symbols)
        //{
        //    DebugStackTrace stackTrace = control.GetStackTrace(50);
        //    foreach (DebugStackFrame frame in stackTrace)
        //    {
        //        string functionName;
        //        ulong displacement;
        //        uint line;
        //        string file;
        //        try
        //        {
        //            symbols.GetNameByOffset(frame.InstructionOffset, out functionName, out displacement);
        //        }
        //        catch (COMException)
        //        {
        //            functionName = "Unknown";
        //            displacement = ulong.MaxValue;
        //        }

        //        try
        //        {
        //            symbols.GetLineByOffset(frame.InstructionOffset, out line, out file, out displacement);
        //        }
        //        catch (COMException)
        //        {
        //            line = uint.MaxValue;
        //            file = "Unknown";
        //            displacement = ulong.MaxValue;
        //        }

        //        symbols.SetScope(frame.InstructionOffset, frame, null);
        //        DebugSymbolGroup symbolGroup = symbols.GetScopeSymbolGroup(GroupScope.Arguments);

        //        for (uint i = 0; i < symbolGroup.NumberSymbols; i++)
        //        {
        //            functionName += " " + symbolGroup.GetSymbolName(i);
        //            DebugSymbolEntry symbolEntry = symbolGroup.GetSymbolEntryInformation(i);
        //            functionName += "(" + symbols.GetTypeName(symbolEntry.ModuleBase, symbolEntry.TypeId) + ")";
        //            byte[] buffer = new byte[symbolEntry.Size];
        //            uint bytesRead;
        //            try
        //            {
        //                bool success = symbols.ReadTypedDataVirtual(symbolEntry.Offset, symbolEntry.ModuleBase, symbolEntry.TypeId, buffer, out bytesRead);
        //                if (success)
        //                {
        //                    if (4 == bytesRead)
        //                    {
        //                        uint v = buffer[3];
        //                        v <<= 8;
        //                        v |= buffer[2];
        //                        v <<= 8;
        //                        v |= buffer[1];
        //                        v <<= 8;
        //                        v |= buffer[0];
        //                        //functionName += "=0x" + v.ToString("X8");
        //                    }
        //                    else if (8 == bytesRead)
        //                    {
        //                        ulong v = buffer[7];
        //                        for (int q = 6; q >= 0; q--)
        //                        {
        //                            v <<= 8;
        //                            v |= buffer[q];
        //                        }
        //                        functionName += "=0x" + v.ToString("X16");
        //                    }
        //                    else
        //                    {
        //                        functionName += "size=" + bytesRead;
        //                    }
        //                }
        //            }
        //            catch (COMException)
        //            {
        //            }
        //        }

        //        this.WriteLine(functionName);
        //    }
        //}
        #endregion
    }

    public delegate void UpdateProgressDelegate(double progress);
}



