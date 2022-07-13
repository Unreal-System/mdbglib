using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Debuggers.DbgEng;

namespace mdbglib
{
    public struct DebuggeeInfo
    {
        public DebugClient Client;
        public DebugSymbols Symbols;
        public DebugControl Control;
        public DebugDataSpaces DataSpaces;
        public DebugSystemObjects SystemObjects;

        public void SetCurrentThread(uint threadId)
        {
            if (this.SystemObjects.CurrentThreadId != threadId)
                this.Control.Execute("~" + threadId + " s");
        }
    }
}
