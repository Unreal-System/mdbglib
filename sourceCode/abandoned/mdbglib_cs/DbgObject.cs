using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Debuggers.DbgEng;
using System.Runtime.InteropServices;

namespace mdbglib
{
    public class DbgObject
    {
        protected internal DebuggeeInfo m_info;
        private DbgType m_type;

        private ulong m_offset;
        private ulong m_moduleBase;
        private uint m_typeId;

        internal DbgObject(DebuggeeInfo info, ulong offset, ulong moduleBase, uint typeId) 
        {
            this.m_info = info;
            this.m_offset = offset;
            this.m_moduleBase = moduleBase;
            this.m_typeId = typeId;
        }

        public bool IsNull 
        {
            get 
            {
                return this.m_offset == 0;
            }
        }

        public DbgType Type 
        {
            get 
            {
                if (null == this.m_type) 
                {
                    this.m_type = new DbgType(this.m_info.Symbols, this.m_moduleBase, this.m_typeId);
                }
                return this.m_type;
            }
        }

        public DbgObject Dereference(string resultObjectType) 
        {
            if (this.IsNull)
                throw new NullReferenceException("Cannot dereference a object that points to a null");

            ulong[] buff = new ulong[1];

            try
            {
                this.m_info.DataSpaces.ReadVirtual<ulong>(this.m_offset, buff);
            }
            catch (Exception e)
            {
                if (e is COMException || e is InvalidCastException)
                    throw new DbgAccessViolationException("Error reading memory at the address: " + this.m_offset);
            }
            ulong objectOffset = buff[0];

            uint objectTypeId = this.m_info.Symbols.GetTypeId(this.m_moduleBase, resultObjectType);

            DbgObject obj = new DbgObject(this.m_info, objectOffset, this.m_moduleBase, objectTypeId);
            return obj;
        }

        public DbgObject Dereference() 
        {
            if (this.Type.Name.EndsWith("*"))
            {
                return this.Dereference(this.Type.Name.Substring(0, this.Type.Name.Length - 1));
            }
            else
            {
                throw new NotImplementedException("Cannot dereference a variable that is not of pointer type");
            }
        }

        public DbgObject Cast(string typeName) 
        {
            uint newType = this.m_info.Symbols.GetTypeId(this.m_moduleBase, typeName);

            return new DbgObject(this.m_info, this.m_offset, this.m_moduleBase, newType);
        }

        public DbgObject ReadField(string fieldName)
        {
            uint fieldTypeId;
            uint fieldOffset;
            this.m_info.Symbols.GetFieldTypeAndOffset(this.m_moduleBase, this.m_typeId, fieldName, out fieldTypeId, out fieldOffset);

            DbgObject obj = new DbgObject(this.m_info, this.m_offset + fieldOffset, this.m_moduleBase, fieldTypeId);
            return obj;
        }

        public string ReadAsString() 
        {
            if (this.IsNull)
                throw new NullReferenceException();

            return this.m_info.DataSpaces.ReadUnicodeStringVirtual(this.m_offset, uint.MaxValue);
        }
    }
}
