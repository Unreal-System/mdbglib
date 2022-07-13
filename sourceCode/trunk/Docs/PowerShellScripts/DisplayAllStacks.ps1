cls
$loadResult=[Reflection.Assembly]::LoadFrom("c:\svn\mdbglib.svn.codeplex.com\trunk\Win32\Debug\mdbglib.dll")
$dbg = New-Object MS.Debuggers.DbgEng.Debuggee("C:\svn\mdbglib.svn.codeplex.com\trunk\dumps\267166\d32b6e07-d4a8-4125-933e-9956a4175aaf.mdmpFull.mdmp", "C:\svn\mdbglib.svn.codeplex.com\trunk\dumps\267166\Symbols")
foreach($t in $dbg.Threads)
{
    write-host $t.ThreadId "Id:" $t.ThreadSystemId
    foreach($f in $t.GetStackTrace())
    {
        write-host $f.FunctionName "+0x" $f.Displacement.ToString("X") "[" $f.File "@" $f.Line "]"
    }
}