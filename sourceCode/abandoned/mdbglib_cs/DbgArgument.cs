using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Debuggers.DbgEng;

namespace mdbglib
{
    public class DbgArgument : DbgObject
    {
        private string m_name;
        
        internal DbgArgument(DebuggeeInfo info, ulong offset, ulong moduleBase, uint typeId)
            : base(info, offset, moduleBase, typeId)
        {

        }

        public string Name
        {
            get
            {
                //if (null == m_name)
                //{
                //    try
                //    {
                //        string name;
                //        ulong displacement;

                //        this.m_info.Symbols.GetFieldName(this.m_offset, out name, out displacement);

                //        this.m_name = name;
                //    }
                //    catch (COMException) 
                //    {
                //        this.m_name = "!Unknown";
                //    }
                //}
                return this.m_name;
            }
            set
            {
                this.m_name = value;
            }
        }

    }
}
