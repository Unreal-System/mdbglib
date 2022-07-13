/*
//TestGetDbgObject();
            //TestEvaluate();
            //TestSymbolInfo();
            //TestModules();
            //TestGlobal();
            TestGetStackTrace();
            Console.WriteLine("Press Enter to exit ...");
            Console.ReadLine();

         static bool TestAll() 
        {
            bool b = true;
            b &= TestGetDbgObject();
            //b &= TestGlobal();
            b &= TestEvaluate();
            b &= TestSymbolInfo();
            b &= TestModules();
            b &= TestModule();
            b &= TestGetStackTrace();
            return b;
        }
        //A helper function that checks the dump in trunk/dumps/TestApp/1/TestAppAV.dmp and returns a Debuggee object that points to this dump
        static Debuggee TestAppAV() 
        {
            string dumpFilePath = Path.GetFullPath(@"..\..\dumps\TestApp\1\TestAppAV.dmp");
            if (!CheckDump(dumpFilePath))
                return null;

            return Debuggee.OpenDumpFile(dumpFilePath, Path.GetFullPath(@"..\..\dumps\TestApp\1\"));
        }

        //This function shows how to get the stack trace for the current event in a dump file
        static bool TestGetStackTrace() 
        {
            using (Debuggee dbg = new Debuggee())
            {
                dbg.SetSymbolPath(@"..\..\dumps\TestApp\1\");
                dbg.OpenDumpFile(@"..\..\dumps\TestApp\1\TestAppAV.dmp");
                dbg.WaitForEvent(0);
                //Or better yet you could replace the previous 5 lines with:
                //using(Debuggee dbg = Debuggee.OpenDumpFile(@"..\..\dumps\TestApp\1\TestAppAV.dmp", @"..\..\dumps\TestApp\1\"))){

                foreach (DbgFrame f in dbg.GetEventStackTrace())
                {
                    Console.Write(f.FunctionName);
                    Console.Write("(");
                    foreach (DbgArgument a in f.Arguments) 
                    {
                        Console.Write(a.Name);
                    }
                    Console.WriteLine(")");
                }
            }
            return true;
        }

        //Interesting function that reads the contents of a global variable
        static bool TestGetDbgObject()
        {
            using (Debuggee dbg = TestAppAV())
            {
                if (null == dbg)
                    return false;

                //DbgType gsType;
                //DbgModule gsModule;
                //dbg.GetSymbolInfo("TestApplication!g_string", out gsType, out gsModule);
                //Console.WriteLine("type.Name = " + gsType.Name);
                //Console.WriteLine("type.Size = " + gsType.Size);

                DbgValue s = dbg.Evaluate("TestApplication!g_string");
                //I knot that this is an offset to a char* so I'm getting the object
                ulong offset = s.ToUInt64(null);
                DbgObject o = dbg.GetDbgObject(offset, "TestApplication", "char");
                Console.WriteLine(o.ReadAsStringASCII());
            }
            return true;
        }

        //Get values of 2 global variables
        static bool TestEvaluate() 
        {
            using (Debuggee dbg = TestAppAV()) 
            {
                if (null == dbg)
                    return false;

                DbgValue s = dbg.Evaluate("TestApplication!g_string");
                Console.WriteLine("s.Type = " + s.Type.ToString());
                DbgValue b = dbg.Evaluate("TestApplication!g_bool");
                Console.WriteLine("b.Type = " + b.Type.ToString());
                
            }
            return true;
        }
        //static bool TestGlobal() 
        //{
        //    using (Debuggee dbg = TestAppAV())
        //    {
        //        if (null == dbg)
        //            return false;

        //        DbgModule m = dbg.GetModule("TestApplication");
        //        DbgObject ps = m.Global.ReadField("g_string");
        //        DbgObject s = ps.Dereference();
        //        Console.WriteLine("g_string = " + s.ReadAsString());

        //    }
        //    return true;
        //}

        //Test how getting information for a given symbol works
        //For more info see: GetSymbolTypeId function, http://msdn.microsoft.com/en-us/library/cc266234.aspx
        static bool TestSymbolInfo() 
        {
            using (Debuggee dbg = TestAppAV())
            {
                if (null == dbg)
                    return false;

                DbgType type;
                DbgModule module;

                Console.WriteLine("=== Info about TestApplication!g_bool");
                dbg.GetSymbolInfo("TestApplication!g_bool", out type, out module);
                Console.WriteLine("type.Name = " + type.Name);
                Console.WriteLine("type.Size = " + type.Size);
                WriteModule(module);

                Console.WriteLine();

                Console.WriteLine("=== Info about TestApplication!g_string");
                dbg.GetSymbolInfo("TestApplication!g_string", out type, out module);
                Console.WriteLine("type.Name = " + type.Name);
                Console.WriteLine("type.Size = " + type.Size);

                WriteModule(module);
            }
            return true;
        }

        //Test the work with modules
        static bool TestModules() 
        {
            using (Debuggee dbg = TestAppAV())
            {
                if (null == dbg)
                    return false;

                foreach (DbgModule m in dbg.Modules)
                {
                    WriteModule(m);
                }
                //uint count = dbg.ModuleCount;
                //for (uint i = 0; i < count; i++)
                //{
                //    DbgModule m = dbg.GetModule(i);
                //    WriteModule(m);
                //}
            }
            return true;
        }
        static bool TestModule() 
        {
            using (Debuggee dbg = TestAppAV())
            {
                if (null == dbg)
                    return false;

                DbgModule m = dbg.GetModuleByName("TestApplication");

                WriteModule(m);
            }
            return true;
        }

        //Write all information about a module
        static void WriteModule(DbgModule m) 
        {
            Console.WriteLine("=== Module: " + m.Name + " ===");
            Console.WriteLine("Base            = " + m.Base);
            Console.WriteLine("Checksum        = " + m.Checksum);
            Console.WriteLine("Flags           = " + m.Flags);
            Console.WriteLine("ImageName       = " + m.ImageName);
            Console.WriteLine("LoadedImageName = " + m.LoadedImageName);
            Console.WriteLine("MappedImageName = " + m.MappedImageName);
            //Console.WriteLine("Name            = " + m.Name);
            Console.WriteLine("Size            = " + m.Size);
            Console.WriteLine("SymbolFileName  = " + m.SymbolFileName);
            Console.WriteLine("SymbolType      = " + m.SymbolType);
            Console.WriteLine("TimeStamp       = " + m.TimeStamp); //Not quite works
        }

        static bool CheckDump(string dumpFilePath) 
        {
            if (File.Exists(dumpFilePath))
                return true;

            Console.WriteLine("Dump file : "+ dumpFilePath+" cannot be found. Make sure you have propper enlistments in dumps folder and you extracted the archive.");
            return false;
        }

 */