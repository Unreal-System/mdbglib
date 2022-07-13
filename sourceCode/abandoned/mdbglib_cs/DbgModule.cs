using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mdbglib
{
    public class DbgModule
    {
        private DebuggeeInfo m_info;
        private string m_name;
        private string m_imageName;
        private string m_loadedImageName;
        private uint m_moduleIndex;
        private ulong? m_base;


        public DbgModule(DebuggeeInfo info, uint moduleIndex) 
        {
            this.m_info = info;
            this.m_moduleIndex = moduleIndex;
        }

        private void LoadModuleNames() 
        {
            this.m_info.Symbols.GetModuleNamesByIndex(this.m_moduleIndex, out this.m_imageName, out this.m_name, out this.m_loadedImageName);
        }

        public string Name
        {
            get 
            {
                if (null == this.m_name) 
                {
                    this.LoadModuleNames();
                }
                return this.m_name;    
            }
            internal set 
            {
                this.m_name = value;
            }
        }

        public string LoadedImageName 
        {
            get 
            {
                if (this.m_loadedImageName == null) 
                {
                    this.LoadModuleNames();
                }
                return this.m_loadedImageName;
            }
        }

        public ulong Base
        {
            get 
            {
                if (null == this.m_base) 
                {
                    this.m_base = this.m_info.Symbols.GetModuleByIndex(this.m_moduleIndex);
                }
                return (ulong)this.m_base;
            }
            internal set 
            {
                this.m_base = value;
            }
        }
        public uint ModuleIndex
        {
            get 
            {
                return this.m_moduleIndex;
            }
        }

    }
}
