using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Debuggers.DbgEng;
using System.Runtime.InteropServices;

namespace mdbglib
{
    //This class has no cache, possibly should improve in the future
    public class DbgThread
    {
        private DebuggeeInfo m_info;

        private uint m_threadId;
        private uint m_threadSystemId;

        internal DbgThread(DebuggeeInfo info, uint threadId, uint threadSystemId) 
        {
            this.m_info = info;

            this.m_threadId = threadId;
            this.m_threadSystemId = threadSystemId;
        }

        public uint ThreadId 
        {
            get 
            {
                return this.m_threadId;
            }
        }

        public uint ThreadSystemId 
        {
            get
            {
                return this.m_threadSystemId;
            }
        }

        public DbgFrame[] GetStackTrace (uint maxFrames)
        {
            try
            {
                if(this.m_info.SystemObjects.CurrentThreadId != this.m_threadId)
                    this.m_info.SetCurrentThread(this.m_threadId);

                DebugStackTrace trace = this.m_info.Control.GetStackTrace(maxFrames);
                DbgFrame[] result = new DbgFrame[trace.Count];

                for (int i = 0; i < trace.Count; i++)
                {
                    DebugStackFrame f = trace[i];
                    DbgFrame newFrame = new DbgFrame(this.m_info, f);
                    result[i] = newFrame;
                }
                return result;
            }
            catch (COMException) 
            {
                throw;
            }
        }
        public DbgFrame[] GetStackTrace() 
        {
            return this.GetStackTrace(10000);
        }

        private DbgFrame[] m_stackTrace = null;

        public DbgFrame[] StackTrace 
        {
            get 
            {
                if (null == this.m_stackTrace)
                    this.m_stackTrace = this.GetStackTrace();
                
                return this.m_stackTrace;
            }
        }
    }
}
