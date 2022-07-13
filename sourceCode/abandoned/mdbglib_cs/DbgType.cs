using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Debuggers.DbgEng;

namespace mdbglib
{
    public class DbgType
    {
        private DebugSymbols m_symbols;
        private ulong m_moduleBase;
        private uint m_typeId;
        private string m_name;
        private uint? m_size;

        public DbgType(DebugSymbols symbols, ulong moduleBase, uint typeId) 
        {
            this.m_symbols = symbols;
            this.m_moduleBase = moduleBase;
            this.m_typeId = typeId;
        }

        public string Name 
        {
            get 
            {
                if (null == this.m_name) 
                {
                    this.m_name = this.m_symbols.GetTypeName(this.m_moduleBase, this.m_typeId);
                }
                return this.m_name;
            }
        }

        public uint Size 
        {
            get 
            {
                if (null == this.m_size) 
                {
                    this.m_size = this.m_symbols.GetTypeSize(this.m_moduleBase, this.m_typeId);
                }
                return (uint)this.m_size;
            }
            internal set 
            {
                this.m_size = value;
            }
        }
    }
}
