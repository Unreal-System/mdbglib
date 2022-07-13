#include "StdAfx.h"
#include "Debuggee.h"
#include "Tools.h"
#include "DebugOutputEventArgs.h"
#include "DebuggeeEventWrappers.h"

using namespace System;
using namespace Runtime::InteropServices;

namespace MS
{
	namespace Debuggers
	{
		namespace DbgEng
		{
#pragma region Constructors/Destructor/Finalizer
			Debuggee::Debuggee()
			{
/*
HRESULT
  DebugCreate(
    IN REFIID  InterfaceId,
    OUT PVOID *  Interface
    );
*/
				DbgClient* debugClient = NULL;
				Tools::CheckHR(DebugCreate( __uuidof(DbgClient), (void **)&debugClient ));
				this->m_info = gcnew DebuggeeInfo(debugClient);

				this->disposed = false;
			}
			Debuggee::Debuggee(System::String ^remoteOptions)
			{
//HRESULT
//  DebugConnectWide(
//    IN PCWSTR  RemoteOptions,
//    IN REFIID  InterfaceId,
//    OUT PVOID *  Interface
//    );
				pin_ptr<const wchar_t> pRemoteOptions = PtrToStringChars(remoteOptions);
				DbgClient* debugClient = NULL;
				Tools::CheckHR(DebugConnectWide(pRemoteOptions, __uuidof(DbgClient), (void **)&debugClient ));
				this->m_info = gcnew DebuggeeInfo(debugClient);
				this->disposed = false;
			}

			
			Debuggee::~Debuggee() //Dispose
			{
				//Clean managed resources here
				if(NULL != m_info->Client)
				{
					Tools::CheckHR(this->m_info->Client->SetOutputCallbacksWide((IDebugOutputCallbacksWide*)this->m_oldOutputCallbacks));
					//Tools::CheckHR(this->m_info->Client->SetEventCallbacks(NULL));
					Tools::CheckHR(this->m_info->Client->EndSession(DEBUG_END_ACTIVE_DETACH));
				}
				this->m_info->~DebuggeeInfo();
				this->!Debuggee();
				this->disposed = true;
				//No need to call SuppressFinalize, the compiler does it for you
			}
			Debuggee::!Debuggee() //Finalize method
			{
				//Clean unmanaged resources here
				if(NULL != this->m_oldOutputCallbacks)
					this->m_oldOutputCallbacks->Release();

				if(NULL != this->m_nativeOutCall)
					this->m_nativeOutCall->Release();
			}

			Debuggee^ Debuggee::OpenDumpFile(System::String ^dumpFilePath, System::String ^symbolPath)
			{
				Debuggee^ result = gcnew Debuggee();
				result->SymbolPath::set(symbolPath);
				result->OpenDumpFile(dumpFilePath);
				result->WaitForEvent(0);
				return result;
			}
#pragma endregion

#pragma region Symbols Location
			void Debuggee::SymbolPath::set(String ^symbolPath)
			{
				this->m_info->DebugSymbols->SetSymbolPath(symbolPath);
			}
			String ^Debuggee::SymbolPath::get()
			{
				return this->m_info->DebugSymbols->GetSymbolPath();
			}
			void Debuggee::SymbolPathAppend(String ^symbolPath)
			{
				this->m_info->DebugSymbols->AppendSymbolPath(symbolPath);
			}
#pragma endregion

#pragma region Dump Files

			void Debuggee::OpenDumpFile(String ^dumpFilePath)
			{
				this->m_info->DebugClient->OpenDumpFile(dumpFilePath, 0);
			}

			void Debuggee::OpenDumpFile(IntPtr handle)
			{
				this->m_info->DebugClient->OpenDumpFile(nullptr, handle.ToInt64());
			}

			void Debuggee::AddDumpFile(String ^dumpFilePath)
			{
				this->m_info->DebugClient->AddDumpFileInformation(dumpFilePath, 0);
			}

			void Debuggee::AddDumpFile(IntPtr handle)
			{
				this->m_info->DebugClient->AddDumpFileInformation(nullptr, handle.ToInt64());
			}

			void Debuggee::WriteDumpFile(String ^path, DumpType type, DumpFlags flags, String ^comment)
			{
				this->m_info->DebugClient->WriteDumpFile(path, 0, (ULONG)type, (ULONG)flags, comment);
			}

			String ^Debuggee::GetDumpFile(ULONG index)
			{
				return this->m_info->DebugClient->GetDumpFile(index);
			}
#pragma endregion

#pragma region Attach
			void Debuggee::CreateProcess(String ^commandLine, String ^initialDirectory)
			{
				this->m_info->DebugClient->CreateProcess(commandLine, initialDirectory);
			}
			void Debuggee::CreateAndAttachProcess(String ^commandLine, String ^initialDirectory)
			{
				this->m_info->DebugClient->CreateProcessAndAttach(commandLine, initialDirectory);
			}
			void Debuggee::AttachProcess(ULONG processId, AttachFlags flags)
			{
				//TODO:Check flags here
				this->m_info->DebugClient->AttachProcess(0, processId, (ULONG)flags);
			}
			void Debuggee::DetachCurrentProcess()
			{
				this->m_info->DebugClient->DetachCurrentProcess();
			}
			void Debuggee::DetachProcesses()
			{
				this->m_info->DebugClient->DetachProcesses();
			}
#pragma endregion

#pragma region Threads
			array<DbgThread^>^ Debuggee::Threads::get()
			{
				array<DbgThread^>^ result = nullptr;

				ULONG ulNumberThreads;
				PULONG pulIds;
				PULONG pulSysIds;
				try
				{
					Tools::CheckHR(this->m_info->SystemObjects->GetNumberThreads(&ulNumberThreads));
					pulIds = new ULONG[ulNumberThreads];
					pulSysIds = new ULONG[ulNumberThreads];
					Tools::CheckHR(this->m_info->SystemObjects->GetThreadIdsByIndex(0, ulNumberThreads, pulIds, pulSysIds));

					result = gcnew array<DbgThread^>(ulNumberThreads);

					for(ULONG i=0; i<ulNumberThreads; i++)
					{
						result[i] = gcnew DbgThread(this->m_info, pulIds[i], pulSysIds[i]);
					}
				}
				finally
				{
					delete[] pulIds;
					delete[] pulSysIds;
				}

				return result;
			}
			ULONG Debuggee::EventThreadId::get()
			{
				return this->m_info->DebugSystemObjects->GetEventThreadId();
			}
			DbgThread^ Debuggee::EventThread::get()
			{
				ULONG eventThreadId = this->m_info->DebugSystemObjects->GetEventThreadId();
				throw gcnew NotImplementedException("");
			}
			ULONG Debuggee::EventProcessId::get()
			{
				throw gcnew NotImplementedException("");
			}
			DbgProcess^ Debuggee::EventProcess::get()
			{
				throw gcnew NotImplementedException("");
			}
			DbgThread^ Debuggee::CurrentThread::get()
			{
				ULONG threadId = this->m_info->DebugSystemObjects->GetCurrentThreadId();
				ULONG threadSystemId = this->m_info->DebugSystemObjects->GetCurrentThreadSystemId();
				DbgThread^ result = gcnew DbgThread(this->m_info, threadId, threadSystemId);
				return result;
			}
			ULONG Debuggee::CurrentThreadId::get()
			{
				return this->m_info->DebugSystemObjects->GetCurrentThreadId();
			}
			void Debuggee::CurrentThreadId::set(ULONG threadId)
			{
				this->m_info->DebugSystemObjects->SetCurrentThreadId(threadId);
			}
			array<DbgFrame^>^ Debuggee::GetEventStackTrace()
			{
				this->m_info->DebugSymbols->SetScopeFromStoredEvent();
				return this->m_info->DebugControl->GetStackTrace(1000);
			}
#pragma endregion

#pragma region Events
			void Debuggee::DebugOutput::add(EventHandler<DebugOutputEventArgs^>^ handler)
			{
				LONG beforeLength = this->m_debugOutput == nullptr ? 0 : this->m_debugOutput->GetInvocationList()->Length;
				this->m_debugOutput = static_cast<EventHandler<DebugOutputEventArgs^>^>(Delegate::Combine(this->m_debugOutput, handler));
				LONG afterLength = this->m_debugOutput == nullptr ? 0 : this->m_debugOutput->GetInvocationList()->Length;
				if(1 == afterLength && 0 == beforeLength) //Not sure if it makes sense. What hapens if I add the same handler to an event twice?
				{
					if(NULL == this->m_nativeOutCall)
					{
						this->m_nativeOutCall = new OutputCallbackWrapper(this);
						this->m_nativeOutCall->AddRef();
					}
					IDebugOutputCallbacksWide *oldOutputCallbacks;
					Tools::CheckHR(this->m_info->Client->GetOutputCallbacksWide((IDebugOutputCallbacksWide**)&oldOutputCallbacks));
					this->m_oldOutputCallbacks = oldOutputCallbacks;
					Tools::CheckHR(this->m_info->Client->SetOutputCallbacksWide((IDebugOutputCallbacksWide*)this->m_nativeOutCall));
				}
			}
			void Debuggee::DebugOutput::remove(EventHandler<DebugOutputEventArgs^>^ handler)
			{
				LONG beforeLength = this->m_debugOutput == nullptr ? 0 : this->m_debugOutput->GetInvocationList()->Length;
				this->m_debugOutput = static_cast<EventHandler<DebugOutputEventArgs^>^>(Delegate::Remove(this->m_debugOutput, handler));
				LONG afterLength = this->m_debugOutput == nullptr ? 0 : this->m_debugOutput->GetInvocationList()->Length;
				if(1 == beforeLength && 0 == afterLength)
				{
					Tools::CheckHR(this->m_info->Client->SetOutputCallbacksWide((IDebugOutputCallbacksWide*)this->m_oldOutputCallbacks));
				}
			}

			void Debuggee::DebugOutput::raise(Object ^sender, DebugOutputEventArgs ^args)
			{
				if(nullptr != this->m_debugOutput)
					this->m_debugOutput(this, args);
			}

			void Debuggee::OnDebugOutput(DbgEng::OutputFlags outputFlags, String ^text)
			{
				if(nullptr != this->m_debugOutput)
					this->m_debugOutput(this, gcnew DebugOutputEventArgs(outputFlags, text));
			}
#pragma endregion

#pragma region Output
			void Debuggee::Output(DbgEng::OutputFlags mmode, String ^mtext)
			{
				pin_ptr<const wchar_t> text = PtrToStringChars(mtext);
				Tools::CheckHR(this->m_info->Control->OutputWide((ULONG)mmode, text));
			}
#pragma endregion

#pragma region Modules
			DbgModule^ Debuggee::GetModuleByBase(ULONG64 moduleBase)
			{
				return gcnew DbgModule(this->m_info, moduleBase);
			}
			DbgModule^ Debuggee::GetModuleByName(String ^name)
			{
				ULONG64 base;
				ULONG index;
				this->m_info->DebugSymbols->GetModuleByModuleName(name, index, base);
				return gcnew DbgModule(this->m_info, base, index);
			}
			DbgModule^ Debuggee::GetModuleByIndex(ULONG index)
			{
/*
HRESULT
  IDebugSymbols::GetModuleByIndex(
    IN ULONG  Index,
    OUT PULONG64  Base
    );
	*/
				ULONG64 base;
				Tools::CheckHR(this->m_info->Symbols->GetModuleByIndex(index, &base));
				return gcnew DbgModule(this->m_info, base, index);
			}

			ULONG Debuggee::ModuleCount::get()
			{
/*
HRESULT
  IDebugSymbols::GetNumberModules(
    OUT PULONG  Loaded,
    OUT PULONG  Unloaded
    );
*/
				ULONG nrLoaded;
				ULONG nrUnloaded;
				Tools::CheckHR(this->m_info->Symbols->GetNumberModules(&nrLoaded, &nrUnloaded));
				return nrLoaded + nrUnloaded;
			}

			array<DbgModule^>^ Debuggee::Modules::get()
			{
/*
HRESULT
  IDebugSymbols::GetModuleParameters(
    IN ULONG  Count,
    IN OPTIONAL PULONG64  Bases,
    IN ULONG  Start,
    OUT PDEBUG_MODULE_PARAMETERS  Params
    );
*/
				ULONG count = this->ModuleCount;

				PDEBUG_MODULE_PARAMETERS params = NULL;
				try
				{
					params = new _DEBUG_MODULE_PARAMETERS[count];
					Tools::CheckHR(this->m_info->Symbols->GetModuleParameters(count, NULL, 0, params));

					array<DbgModule^>^ result = gcnew array<DbgModule^>(count);
					for(ULONG i=0;i<count;i++)
					{
						result[i] = gcnew DbgModule(this->m_info, params[i], i);
					}
					return result;
				}
				finally
				{
					delete[] params;
				}
			}
#pragma endregion

#pragma region Objects
			DbgObject^ Debuggee::GetDbgObject(ULONG64 offset, ULONG64 moduleBase, ULONG typeId)
			{
				return gcnew DbgObject(this->m_info, offset, moduleBase, typeId);
			}
			DbgObject^ Debuggee::GetDbgObject(ULONG64 offset, String^ module, String^ type)
			{
				ULONG64 moduleBase;
				ULONG moduleIndex;
				this->m_info->DebugSymbols->GetModuleByModuleName(module, moduleIndex, moduleBase);
				ULONG typeId = this->m_info->DebugSymbols->GetTypeId(moduleBase, type);
				return gcnew DbgObject(this->m_info, offset, moduleBase, typeId);
			}
#pragma endregion

			void Debuggee::WaitForEvent(ULONG timeout)
			{
				this->m_info->DebugControl->WaitForEvent(timeout);
			}
			//Not tested
			ULONG Debuggee::GetExitCode()
			{
				this->CheckDisposed();

				ULONG exitCode = 0;
				Tools::CheckHR(this->m_info->Client->GetExitCode(&exitCode));
				return exitCode;
			}
			DebugStatus Debuggee::GetExecutionStatus()
			{
				this->CheckDisposed();
				return this->m_info->DebugControl->GetExecutionStatus();
			}
			void Debuggee::Execute(String ^command)
			{
				this->CheckDisposed();
				return this->m_info->DebugControl->Execute(DebugOutputControl::ThisClient, command, DebugExecuteFlags::Default);
			}

			ULONG Debuggee::ProcessCount::get()
			{
				return this->m_info->DebugSystemObjects->GetNumberProcesses();
			}
			String^ Debuggee::ProcessExecutableName::get()
			{
				return this->m_info->DebugSystemObjects->GetCurrentProcessExecutableName();
			}
			ULONG Debuggee::CurrentProcessId::get()
			{
				return this->m_info->DebugSystemObjects->GetCurrentProcessId();
			}
			void Debuggee::CurrentProcessId::set(ULONG value)
			{
				return this->m_info->DebugSystemObjects->SetCurrentProcessId(value);
			}
			String^ Debuggee::ImagePath::get()
			{
				return this->m_info->DebugSymbols->GetImagePath();
			}
			void Debuggee::ImagePath::set(String^ value)
			{
				return this->m_info->DebugSymbols->SetImagePath(value);
			}

			DbgValue^ Debuggee::Evaluate(String ^expression)
			{
				//GetExpression("msvbvm60!g_itlsebthread");  
				return this->m_info->DebugControl->Evaluate(expression, DbgValueType::Invalid, NULL, DbgExpressionType::CPP);
			}
			void Debuggee::GetSymbolInfo(String ^symbol, [Runtime::InteropServices::Out] DbgType ^%type, [Runtime::InteropServices::Out] DbgModule ^%module)
			{
				ULONG typeId;
				ULONG64 moduleBase;
				this->m_info->DebugSymbols->GetSymbolType(symbol, typeId, moduleBase);
				type = gcnew DbgType(this->m_info, moduleBase, typeId);
				module = gcnew DbgModule(this->m_info, moduleBase);
			}
		}
	}
}

