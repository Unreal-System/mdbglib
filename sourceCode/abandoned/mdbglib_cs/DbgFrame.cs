using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Debuggers.DbgEng;
using System.Runtime.InteropServices;

namespace mdbglib
{
    public class DbgFrame
    {
        //Locals
        //Function Parameters

        private DebuggeeInfo m_info;
        private DebugStackFrame m_frame;

        private string m_functionName = null;
        private ulong? m_displacement = null;
        private uint? m_line = null;
        private string m_file = null;
        private DbgArgument[] m_arguments = null;

        internal DbgFrame(DebuggeeInfo info, DebugStackFrame frame) 
        {
            this.m_info = info;
            this.m_frame = frame;
        }

        private void GetFunctionNameAndDisplacement()
        {
            try
            {
                ulong displacement;
                this.m_info.Symbols.GetNameByOffset(this.m_frame.InstructionOffset, out this.m_functionName, out displacement);
                this.m_displacement = displacement;
            }
            catch (COMException)
            {
                this.m_functionName = "!Unknown";
                this.m_displacement = null;
            }
        }

        public string FunctionName 
        {
            get 
            {
                if (null != this.m_functionName)
                    return this.m_functionName;

                this.GetFunctionNameAndDisplacement();
                return this.m_functionName;
            }
        }

        public ulong Displacement 
        {
            get 
            {
                if (null != this.m_displacement)
                    return (ulong)this.m_displacement;

                this.GetFunctionNameAndDisplacement();
                return (ulong)this.m_displacement;
            }
        }


        private void GetFileAndLine()
        {
            try
            {
                uint line;
                string file;
                ulong displacement;
                this.m_info.Symbols.GetLineByOffset(this.m_frame.FrameOffset, out line, out file, out displacement);
                this.m_file = file;
                this.m_line = line;
                this.m_displacement = displacement;
            }
            catch (COMException) 
            {
                this.m_file = "!Unknown";
                this.m_line = 0;
                this.m_displacement = null;
            }
        }

        public string File 
        {
            get 
            {
                if (null != this.m_file)
                    return this.m_file;

                this.GetFileAndLine();
                return this.m_file;
            }
        }
        public uint Line 
        {
            get 
            {
                if (null != this.m_line)
                    return (uint)this.m_line;

                this.GetFileAndLine();
                return (uint)this.m_line;
            }
        }

        public DbgArgument[] Arguments 
        {
            get 
            {
                if (null != this.m_arguments)
                    return this.m_arguments;

                //Set scope to the current frame in call stack
                this.m_info.Symbols.SetScope(this.m_frame.InstructionOffset, this.m_frame, null);
                //Get Function Arguments
                DebugSymbolGroup symbolGroup = this.m_info.Symbols.GetScopeSymbolGroup(GroupScope.Arguments);

                DbgArgument[] result = new DbgArgument[symbolGroup.NumberSymbols];

                for (uint i = 0; i < symbolGroup.NumberSymbols; i++)
                {
                    DebugSymbolEntry symbolEntry = symbolGroup.GetSymbolEntryInformation(i);
                    DbgArgument a = new DbgArgument(this.m_info, symbolEntry.Offset, symbolEntry.ModuleBase, symbolEntry.TypeId);
                    a.Type.Size = symbolEntry.Size;
                    a.Name = symbolGroup.GetSymbolName(i);
                    result[i] = a;
                }

                this.m_arguments = result;
                return this.m_arguments;
            }
        }
    }
}
