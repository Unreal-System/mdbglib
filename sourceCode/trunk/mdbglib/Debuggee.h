#pragma once

#include "stdafx.h"
#include "DebugOutputEventArgs.h"
#include "DbgThread.h"
#include "DbgProcess.h"
#include "DbgModule.h"

using namespace System;
using namespace System::Collections;

namespace MS
{
	namespace Debuggers
	{
		namespace DbgEng
		{
			public ref class Debuggee : public System::IDisposable
			{
			private:
				DebuggeeInfo^ m_info;
				bool disposed;

				void CheckDisposed()
				{
					if(disposed)
						throw gcnew Exception("Debuggee object is disposed");
				}
				EventHandler<DebugOutputEventArgs^>^ m_debugOutput;

#pragma region Events
				IDebugOutputCallbacksWide *m_oldOutputCallbacks;
				//IDebugInputCallbacks *m_oldInputCallbacks;
				//IDebugEventCallbacks *m_oldEventCallbacksWide;

				OutputCallbackWrapper *m_nativeOutCall;
#pragma endregion

			internal:
				virtual void OnDebugOutput(DbgEng::OutputFlags outputFlags, String ^text);
			protected:
				!Debuggee(); //Finalize method
			public:

				Debuggee();
				Debuggee(System::String ^remoteOptions);
				static Debuggee^ OpenDumpFile(System::String ^dumpFilePath, System::String ^symbolPath);
				~Debuggee(); //Dispose

#pragma region Symbols Location
				property String ^SymbolPath
				{
					String^ get();
					void set(String ^value);
				}
				void SymbolPathAppend(String ^symbolPath);
#pragma endregion
#pragma region Dump Files
				void OpenDumpFile(String ^dumpFilePath);
				void OpenDumpFile(IntPtr handle);
				void AddDumpFile(String ^dumpFilePath);
				void AddDumpFile(IntPtr handle);
				void WriteDumpFile(String ^path, DumpType type, DumpFlags flags, String ^comment);
				String ^GetDumpFile(ULONG index);
#pragma endregion
#pragma region Create/Attach/Detach
				void CreateProcess(String ^commandLine, String ^initialDirectory);
				void CreateAndAttachProcess(String ^commandLine, String ^initialDirectory);
				void AttachProcess(ULONG processId, AttachFlags flags);
				void DetachCurrentProcess();
				void DetachProcesses();
#pragma endregion
#pragma region Threads
				property array<DbgThread^>^ Threads
				{
					array<DbgThread^>^ get();
				}
				property ULONG EventThreadId
				{
					ULONG get();
				}
				property DbgThread^ EventThread
				{
					DbgThread^ get();
				}
				property ULONG EventProcessId
				{
					ULONG get();
				}
				property DbgProcess^ EventProcess
				{
					DbgProcess^ get();
				}
				property DbgThread^ CurrentThread
				{
					DbgThread^ get();
				}
				property ULONG CurrentThreadId
				{
					ULONG get();
					void set(ULONG threadId);
				}
				array<DbgFrame^>^ GetEventStackTrace();
#pragma endregion
#pragma region Events
				event EventHandler<DebugOutputEventArgs^>^ DebugOutput
				{
					void add(EventHandler<DebugOutputEventArgs^>^ handler);
					void remove(EventHandler<DebugOutputEventArgs^>^ handler);
				internal:
					void raise(Object ^sender, DebugOutputEventArgs^ args);
				}
#pragma endregion
#pragma region Output
				void Output(OutputFlags flags, String ^text);
#pragma endregion
#pragma region Modules
				DbgModule^ GetModuleByBase(ULONG64 moduleBase);
				DbgModule^ GetModuleByName(String ^name);
				DbgModule^ GetModuleByIndex(ULONG index);
				property ULONG ModuleCount { ULONG get(); }
				property array<DbgModule^>^ Modules { array<DbgModule^>^ get(); }
#pragma endregion
#pragma region Objects
				DbgObject^ GetDbgObject(ULONG64 offset, ULONG64 moduleBase, ULONG typeId);
				DbgObject^ GetDbgObject(ULONG64 offset, String^ module, String^ type);
#pragma endregion
#pragma region Thin wrapper access
				property MS::Debuggers::DbgEng::DebugSymbols^ DebugSymbols
				{
					MS::Debuggers::DbgEng::DebugSymbols^ get(){ return this->m_info->DebugSymbols; }
				}
				property MS::Debuggers::DbgEng::DebugSystemObjects^ DebugSystemObjects
				{
					MS::Debuggers::DbgEng::DebugSystemObjects^ get(){ return this->m_info->DebugSystemObjects; }
				}
				property MS::Debuggers::DbgEng::DebugControl^ DebugControl
				{
					MS::Debuggers::DbgEng::DebugControl^ get(){ return this->m_info->DebugControl; }
				}
				property MS::Debuggers::DbgEng::DebugDataSpaces^ DebugDataSpaces
				{
					MS::Debuggers::DbgEng::DebugDataSpaces^ get(){ return this->m_info->DebugDataSpaces; }
				}
				property MS::Debuggers::DbgEng::DebugClient^ DebugClient
				{
					MS::Debuggers::DbgEng::DebugClient^ get(){ return this->m_info->DebugClient; }
				}
#pragma endregion
				ULONG GetExitCode();
				DebugStatus GetExecutionStatus();
				void Execute(String ^command);
				property ULONG ProcessCount
				{
					ULONG get();
				}
				property String^ ProcessExecutableName
				{
					String^ get();
				}
				property ULONG CurrentProcessId
				{
					ULONG get();
					void set(ULONG value);
				}
				property String^ ImagePath
				{
					String^ get();
					void set(String^ value);
				}
				void WaitForEvent(/*ULONG flags, */ULONG timeout); //No flags are currently supported
				DbgValue^ Evaluate(String ^expression);
				void GetSymbolInfo(String ^symbol, [Runtime::InteropServices::Out] DbgType ^%type, [Runtime::InteropServices::Out] DbgModule ^%module);
			};
		}
	}
}
