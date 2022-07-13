using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Debuggers.DbgEng;
using DBGENG = Microsoft.Debuggers.DbgEng;

namespace mdbglib
{
    public class Debuggee :IDisposable
    {
        private bool m_disposed = false;

        private DebuggeeInfo m_info;

        private Dictionary<string, DbgModule> m_modules;

        private event EventHandler<DebugOutputEventArgs> m_debugOutput;

        public Debuggee(string dumpFilePath, string symbols) 
        {
            this.m_info = new DebuggeeInfo();
            this.m_info.Client = new DebugClient();
            this.m_info.Symbols = new DebugSymbols(this.m_info.Client);
            this.m_info.Control = new DebugControl(this.m_info.Client);
            this.m_info.DataSpaces = new DebugDataSpaces(this.m_info.Client);
            this.m_info.SystemObjects = new DebugSystemObjects(this.m_info.Client);

            this.m_info.Symbols.SetSymbolPath(symbols);
            this.m_info.Client.OpenDumpFile(dumpFilePath);
            this.m_info.Control.WaitForEvent();

            this.m_info.Client.DebugOutput += new EventHandler<Microsoft.Debuggers.DbgEng.DebugOutputEventArgs>(this.m_client_DebugOutput);
        }

        #region Modules 

        public DbgModule GetModuleByName(string name) 
        {
            if (null == m_modules)
                m_modules = new Dictionary<string, DbgModule>();

            DbgModule result;
            if (this.m_modules.TryGetValue(name, out result))
                return result;

            uint moduleIndex;
            ulong moduleBase;
            this.m_info.Symbols.GetModuleByModuleName(name, 0, out moduleIndex, out moduleBase);

            result = new DbgModule(this.m_info, moduleIndex);
            result.Name = name;
            result.Base = moduleBase;

            this.m_modules.Add(name,result);
            return result;
        }

        #endregion

        #region Output

        public event EventHandler<DebugOutputEventArgs> DebugOutput 
        {
            add 
            {
                this.m_debugOutput += value;
            }
            remove 
            {
                this.m_debugOutput -= value;
            }
        }

        protected void OnDebugOutput(DebugOutputEventArgs e)
        {
            if (null != this.m_debugOutput)
                this.m_debugOutput(this, e);
        }

        protected void m_client_DebugOutput(object sender, DBGENG.DebugOutputEventArgs e)
        {
            DebugOutputEventArgs args = new DebugOutputEventArgs();
            args.Output = e.Output;
            this.OnDebugOutput(args);
        }

        public void Output(string outputText) 
        {
            this.m_info.Control.Output(OutputModes.Normal, outputText);
        }

        #endregion

        #region Threads

        public DbgThread GetThread(uint threadId) 
        {
            //Not very efficient, but whatever

            foreach (DbgThread t in this.Threads) 
            {
                if (t.ThreadId == threadId)
                    return t;
            }
            return null;
        }

        public DbgThread GetEventThread()
        {
            return GetThread(this.m_info.SystemObjects.EventThread);
        }

        public DbgThread[] Threads 
        {
            get
            {
                uint[] ids;
                uint[] sysIds;

                this.m_info.SystemObjects.GetAllThreadIds(out ids, out sysIds);

                bool changes;
                do
                {
                    changes = false;
                    for(int i=1;i<ids.Length;i++)
                    {
                        if (ids[i] < ids[i - 1]) 
                        {
                            changes = true;
                            uint tmp = ids[i]; ids[i] = ids[i - 1]; ids[i - 1] = tmp;
                            tmp = sysIds[i]; sysIds[i] = sysIds[i - 1]; sysIds[i - 1] = tmp;
                        }
                    }
                }
                while (changes);

                DbgThread[] result = new DbgThread[ids.Length];

                for (int i = 0; i < ids.Length; i++) 
                {
                    result[i] = new DbgThread(this.m_info, ids[i], sysIds[i]);
                }
                return result;
            }
            
        }

        public DbgThread GetCurrentThread() 
        {
            uint curThreadSystemId = this.m_info.SystemObjects.CurrentThreadSystemId;
            uint curThreadId = this.m_info.SystemObjects.CurrentThreadId;
            
            DbgThread result = new DbgThread(this.m_info, curThreadId, curThreadSystemId);

            return result;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!this.m_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.
                    this.m_info.Client.DebugOutput -= new EventHandler<Microsoft.Debuggers.DbgEng.DebugOutputEventArgs>(this.m_client_DebugOutput);
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                if (null != this.m_info.SystemObjects)
                    this.m_info.SystemObjects.Dispose();
                if (null != this.m_info.Control)
                    this.m_info.Control.Dispose();
                if (null != this.m_info.DataSpaces)
                    this.m_info.DataSpaces.Dispose();
                if (null != this.m_info.Symbols)
                    this.m_info.Symbols.Dispose();
                if (null != this.m_info.Client)
                    this.m_info.Client.Dispose();

                // Note disposing has been done.
                this.m_disposed = true;

            }
        }


        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~Debuggee()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

        #region Execute

        public bool Execute(string command) 
        {
            bool b = this.m_info.Control.Execute(command);
            return b;
        }

        //public void WaitForEvent() 
        //{
        //}

        #endregion
    }

    public class DebugOutputEventArgs : System.EventArgs// : DBGENG.DebugOutputEventArgs
    {
        public string Output { get; set; }
    }
}
