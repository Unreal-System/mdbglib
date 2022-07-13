Abandoned, please contact mogoreanu~@tt~gmail.com if you're interested to take over the work

mdbglib is a managed library for interacting with Debugger Engine API (dbgeng.dll), which is supplied by Microsoft as part of Debugging Tools for Windows.

mdbglib will help you if you need to:
# Learn how to use Debugger Engine API from managed code (mostly C#)
# Extract information from a dump
# Extract information from the memory of a running application
# Modify virtual memory of a running application
# Write Powershell scripts to interact with a Debugger engine
# (possibly somewhere in the future) write your own debugger.

mdbglib provides both a thin and a thick wrapper around Debugger Engine API. 
# The thin wrapper gives access to Debugger Engine API from managed code doing the required type conversions. It is easier to use for people familiar with Debugger Engine API or for people who want to achieve some custom, advanced behavior
# The thick wrapper tends to provide more .NET like, object oriented, experience in working with Debugger Engine API. It hides calls to the undelying API and provides easy to use objects such as DbgThread, DbgFrame, DbgModule, DbgObject, DbgType, ... Think about the think wrapper compared to dbgeng API as of Windows Forms compared to Win32

Please see the code examples inside mdbglibtest folder for a quick intro.

Foders description:
# ASDumpAnalyzer - the project that this library was first intended for. It is used to exctract queries from Microsoft SQL Server Analysis Services crash dumps
# dbgengtest - a native project that mimics the functions implemented inside mdbglibtest using direct access to Debugger Engine API. Its reason is to show the difference
# Docs - some useful links
# dumps - the dumps and applications that were created for unit tests
# mdbglib - the actual project. It is written in C+++/CLI (managed C+++) 
# mdbglib_cs - an early version of the library that used to reference mdbgeng.dll - the managed wrapper around dbgeng.dll which is not yet publicly available
# mdbglibtest - unit tests for mdbglib, examples of different types of operations that can be performed using mdbglib
# PowerShellScripts - examples of PowerShell scripts that execute tasks using mdbglib

There is ongoing work on a readme and FAQ files, meanwhile post your questions/suggestions here. The project is in "work in progress" stage so any requests are welcome.

The best way to get the latest version is to check out the latest version from: https://mdbglib.svn.codeplex.com/svn/trunk

Comments and suggestions are welcome.

So, lets debug,
Nicolae Mogoreanu