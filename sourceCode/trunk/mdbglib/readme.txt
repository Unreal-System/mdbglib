Quick mdbglib tutorial

1. The most important object to remember is MS.Debuggers.DbgEng.Debuggee
This is the entry point into the rest of information available about a running process or a dump file. It is very important to know that this object implements IDisposable so it should be disposed at the end of usage.
Also all objects returned by Debuggee are internally bound to the Debuggee object and cannot be used once the Debuggee is destroyed so remember to keep a link to the Debuggee object as long as you need to work with the debuggee or any objects returned by it.

A Debuggee object can be created using one of its constructors. Once it is created you cannot make any process until you connect it to a process or to a dump file and specify the right symbols here's an example:

using (Debuggee dbg = new Debuggee()) //Creates a new object
{
    dbg.SetSymbolPath(@"..\..\dumps\TestApp\1\"); //Sets the symbol path
    dbg.OpenDumpFile(@"..\..\dumps\TestApp\1\TestAppAV.dmp"); //Opens the dump
    dbg.WaitForEvent(0);
	
	//Action here...
}

2. DbgThread
