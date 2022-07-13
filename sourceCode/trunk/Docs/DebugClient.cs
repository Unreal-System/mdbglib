public sealed class DebugClient : IDebugEvents, IDebugIO, IDisposable
{
    // Fields
    private EventHandler<BreakpointEventArgs> _onBreakpointDebugEvent;
    private EventHandler<CreateProcessEventArgs> _onCreateProcessDebugEvent;
    private EventHandler<CreateThreadEventArgs> _onCreateThreadDebugEvent;
    private EventHandler<DebugEndInputEventArgs> _onDebugEndInput;
    private EventHandler<DebuggeeStateChangeEventArgs> _onDebuggeeStateChangeDebugEvent;
    private EventHandler<DebugOutputEventArgs> _onDebugOutput;
    private EventHandler<DebugStartInputEventArgs> _onDebugStartInput;
    private EventHandler<EngineStateChangeEventArgs> _onEngineStateChangeDebugEvent;
    private EventHandler<ExceptionEventArgs> _onExceptionDebugEvent;
    private EventHandler<ExitProcessEventArgs> _onExitProcessDebugEvent;
    private EventHandler<ExitThreadEventArgs> _onExitThreadDebugEvent;
    private EventHandler<LoadModuleEventArgs> _onLoadModuleDebugEvent;
    private EventHandler<SessionStatusEventArgs> _onSessionStatusDebugEvent;
    private EventHandler<SymbolStateChangeEventArgs> _onSymbolStateChangeDebugEvent;
    private EventHandler<SystemErrorEventArgs> _onSystemErrorDebugEvent;
    private EventHandler<UnloadModuleEventArgs> _onUnloadModuleDebugEvent;
    private uint modopt(IsConst) _threadAffinity;
    public const uint AnyId = uint.MaxValue;
    public const ulong DefaultServerId = 0L;
    private uint eventDelegCount;
    [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
    private unsafe IDebugClient5* innerClient;
    [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
    private unsafe EventCallbacksToEvent* nativeEventCallbacks;
    [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
    private unsafe InputCallbacksToEvent* nativeInCallbacks;
    [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
    private unsafe OutputCallbacksToEvent* nativeOutCallbacks;
    [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
    private unsafe IDebugEventCallbacksWide* oldEventCallbacks;
    [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
    private unsafe IDebugInputCallbacks* oldInputCallbacks;
    [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
    private unsafe IDebugOutputCallbacksWide* oldOutputCallbacks;

    // Events
    public event EventHandler<BreakpointEventArgs> BreakpointHit
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::BreakpointEventArgs ^> >(ref this._onBreakpointDebugEvent, handler, DebugEvent.Breakpoint);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::BreakpointEventArgs ^> >(ref this._onBreakpointDebugEvent, handler, DebugEvent.Breakpoint);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::BreakpointEventArgs ^>,Microsoft::Debuggers::DbgEng::BreakpointEventArgs>(this._onBreakpointDebugEvent, sender, args);
        }
    }

    public event EventHandler<DebuggeeStateChangeEventArgs> DebuggeeStateChanged
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::DebuggeeStateChangeEventArgs ^> >(ref this._onDebuggeeStateChangeDebugEvent, handler, DebugEvent.DebuggeeChange);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::DebuggeeStateChangeEventArgs ^> >(ref this._onDebuggeeStateChangeDebugEvent, handler, DebugEvent.DebuggeeChange);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::DebuggeeStateChangeEventArgs ^>,Microsoft::Debuggers::DbgEng::DebuggeeStateChangeEventArgs>(this._onDebuggeeStateChangeDebugEvent, sender, args);
        }
    }

    public event EventHandler<DebugEndInputEventArgs> DebugInputEnded
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this._onDebugEndInput = (EventHandler<DebugEndInputEventArgs>) Delegate.Combine(this._onDebugEndInput, handler);
            if ((((this._onDebugStartInput == null) ? 0 : this._onDebugStartInput.GetInvocationList().Length) + this._onDebugEndInput.GetInvocationList().Length) == 1)
            {
                if (this.nativeInCallbacks == null)
                {
                    InputCallbacksToEvent* eventPtr2;
                    InputCallbacksToEvent* eventPtr = @new(12);
                    try
                    {
                        eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.InputCallbacksToEvent.{ctor}(eventPtr, this);
                    }
                    fault
                    {
                        delete((void*) eventPtr);
                    }
                    this.nativeInCallbacks = eventPtr2;
                }
                int modopt(IsLong) hr = **(((int*) this.innerClient))[0x80](this.innerClient, this.nativeInCallbacks);
                if (hr < 0)
                {
                    DebugTools.ThrowDbgEngException(hr);
                }
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            int modopt(IsConst) num2 = (((this._onDebugStartInput == null) ? 0 : this._onDebugStartInput.GetInvocationList().Length) + this._onDebugEndInput.GetInvocationList().Length) - 1;
            this._onDebugEndInput = (EventHandler<DebugEndInputEventArgs>) Delegate.Remove(this._onDebugEndInput, handler);
            if (num2 == 0)
            {
                int modopt(IsLong) hr = **(((int*) this.innerClient))[0x80](this.innerClient, this.oldInputCallbacks);
                if (hr < 0)
                {
                    DebugTools.ThrowDbgEngException(hr);
                }
            }
        }
        raise
        {
            EventHandler<DebugEndInputEventArgs> handler = null;
            handler = this._onDebugEndInput;
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }

    public event EventHandler<DebugStartInputEventArgs> DebugInputStarted
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            int length;
            this._onDebugStartInput = (EventHandler<DebugStartInputEventArgs>) Delegate.Combine(this._onDebugStartInput, handler);
            if (this._onDebugEndInput != null)
            {
                length = this._onDebugEndInput.GetInvocationList().Length;
            }
            else
            {
                length = 0;
            }
            if ((this._onDebugStartInput.GetInvocationList().Length + length) == 1)
            {
                if (this.nativeInCallbacks == null)
                {
                    InputCallbacksToEvent* eventPtr2;
                    InputCallbacksToEvent* eventPtr = @new(12);
                    try
                    {
                        eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.InputCallbacksToEvent.{ctor}(eventPtr, this);
                    }
                    fault
                    {
                        delete((void*) eventPtr);
                    }
                    this.nativeInCallbacks = eventPtr2;
                }
                int modopt(IsLong) hr = **(((int*) this.innerClient))[0x80](this.innerClient, this.nativeInCallbacks);
                if (hr < 0)
                {
                    DebugTools.ThrowDbgEngException(hr);
                }
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            int length;
            if (this._onDebugEndInput != null)
            {
                length = this._onDebugEndInput.GetInvocationList().Length;
            }
            else
            {
                length = 0;
            }
            int modopt(IsConst) num3 = (this._onDebugStartInput.GetInvocationList().Length + length) - 1;
            this._onDebugStartInput = (EventHandler<DebugStartInputEventArgs>) Delegate.Remove(this._onDebugStartInput, handler);
            if (num3 == 0)
            {
                int modopt(IsLong) hr = **(((int*) this.innerClient))[0x80](this.innerClient, this.oldInputCallbacks);
                if (hr < 0)
                {
                    DebugTools.ThrowDbgEngException(hr);
                }
            }
        }
        raise
        {
            EventHandler<DebugStartInputEventArgs> handler = null;
            handler = this._onDebugStartInput;
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }

    public event EventHandler<DebugOutputEventArgs> DebugOutput
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this._onDebugOutput = (EventHandler<DebugOutputEventArgs>) Delegate.Combine(this._onDebugOutput, handler);
            if (this._onDebugOutput.GetInvocationList().Length == 1)
            {
                if (this.nativeOutCallbacks == null)
                {
                    OutputCallbacksToEvent* eventPtr2;
                    OutputCallbacksToEvent* eventPtr = @new(12);
                    try
                    {
                        eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.OutputCallbacksToEvent.{ctor}(eventPtr, this);
                    }
                    fault
                    {
                        delete((void*) eventPtr);
                    }
                    this.nativeOutCallbacks = eventPtr2;
                }
                int modopt(IsLong) hr = **(((int*) this.innerClient))[0x128](this.innerClient, this.nativeOutCallbacks);
                if (hr < 0)
                {
                    DebugTools.ThrowDbgEngException(hr);
                }
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            int modopt(IsConst) num2 = this._onDebugOutput.GetInvocationList().Length - 1;
            this._onDebugOutput = (EventHandler<DebugOutputEventArgs>) Delegate.Remove(this._onDebugOutput, handler);
            if (num2 == 0)
            {
                int modopt(IsLong) hr = **(((int*) this.innerClient))[0x128](this.innerClient, this.oldOutputCallbacks);
                if (hr < 0)
                {
                    DebugTools.ThrowDbgEngException(hr);
                }
            }
        }
        raise
        {
            EventHandler<DebugOutputEventArgs> handler = null;
            handler = this._onDebugOutput;
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }

    public event EventHandler<EngineStateChangeEventArgs> EngineStateChanged
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::EngineStateChangeEventArgs ^> >(ref this._onEngineStateChangeDebugEvent, handler, DebugEvent.EngineChange);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::EngineStateChangeEventArgs ^> >(ref this._onEngineStateChangeDebugEvent, handler, DebugEvent.EngineChange);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::EngineStateChangeEventArgs ^>,Microsoft::Debuggers::DbgEng::EngineStateChangeEventArgs>(this._onEngineStateChangeDebugEvent, sender, args);
        }
    }

    public event EventHandler<ExceptionEventArgs> ExceptionHit
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExceptionEventArgs ^> >(ref this._onExceptionDebugEvent, handler, DebugEvent.Exception);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExceptionEventArgs ^> >(ref this._onExceptionDebugEvent, handler, DebugEvent.Exception);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::ExceptionEventArgs ^>,Microsoft::Debuggers::DbgEng::ExceptionEventArgs>(this._onExceptionDebugEvent, sender, args);
        }
    }

    public event EventHandler<LoadModuleEventArgs> ModuleLoaded
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::LoadModuleEventArgs ^> >(ref this._onLoadModuleDebugEvent, handler, DebugEvent.LoadModule);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::LoadModuleEventArgs ^> >(ref this._onLoadModuleDebugEvent, handler, DebugEvent.LoadModule);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::LoadModuleEventArgs ^>,Microsoft::Debuggers::DbgEng::LoadModuleEventArgs>(this._onLoadModuleDebugEvent, sender, args);
        }
    }

    public event EventHandler<UnloadModuleEventArgs> ModuleUnloaded
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::UnloadModuleEventArgs ^> >(ref this._onUnloadModuleDebugEvent, handler, DebugEvent.UnloadModule);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::UnloadModuleEventArgs ^> >(ref this._onUnloadModuleDebugEvent, handler, DebugEvent.UnloadModule);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::UnloadModuleEventArgs ^>,Microsoft::Debuggers::DbgEng::UnloadModuleEventArgs>(this._onUnloadModuleDebugEvent, sender, args);
        }
    }

    public event EventHandler<CreateProcessEventArgs> ProcessCreated
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateProcessEventArgs ^> >(ref this._onCreateProcessDebugEvent, handler, DebugEvent.CreateProcess);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateProcessEventArgs ^> >(ref this._onCreateProcessDebugEvent, handler, DebugEvent.CreateProcess);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateProcessEventArgs ^>,Microsoft::Debuggers::DbgEng::CreateProcessEventArgs>(this._onCreateProcessDebugEvent, sender, args);
        }
    }

    public event EventHandler<ExitProcessEventArgs> ProcessExited
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitProcessEventArgs ^> >(ref this._onExitProcessDebugEvent, handler, DebugEvent.ExitProcess);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitProcessEventArgs ^> >(ref this._onExitProcessDebugEvent, handler, DebugEvent.ExitProcess);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitProcessEventArgs ^>,Microsoft::Debuggers::DbgEng::ExitProcessEventArgs>(this._onExitProcessDebugEvent, sender, args);
        }
    }

    public event EventHandler<SessionStatusEventArgs> SessionStatusChanged
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SessionStatusEventArgs ^> >(ref this._onSessionStatusDebugEvent, handler, DebugEvent.SessionStatus);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SessionStatusEventArgs ^> >(ref this._onSessionStatusDebugEvent, handler, DebugEvent.SessionStatus);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::SessionStatusEventArgs ^>,Microsoft::Debuggers::DbgEng::SessionStatusEventArgs>(this._onSessionStatusDebugEvent, sender, args);
        }
    }

    public event EventHandler<SymbolStateChangeEventArgs> SymbolStateChanged
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SymbolStateChangeEventArgs ^> >(ref this._onSymbolStateChangeDebugEvent, handler, DebugEvent.SymbolChange);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SymbolStateChangeEventArgs ^> >(ref this._onSymbolStateChangeDebugEvent, handler, DebugEvent.SymbolChange);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::SymbolStateChangeEventArgs ^>,Microsoft::Debuggers::DbgEng::SymbolStateChangeEventArgs>(this._onSymbolStateChangeDebugEvent, sender, args);
        }
    }

    public event EventHandler<SystemErrorEventArgs> SystemErrorRaised
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SystemErrorEventArgs ^> >(ref this._onSystemErrorDebugEvent, handler, DebugEvent.SystemError);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SystemErrorEventArgs ^> >(ref this._onSystemErrorDebugEvent, handler, DebugEvent.SystemError);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::SystemErrorEventArgs ^>,Microsoft::Debuggers::DbgEng::SystemErrorEventArgs>(this._onSystemErrorDebugEvent, sender, args);
        }
    }

    public event EventHandler<CreateThreadEventArgs> ThreadCreated
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateThreadEventArgs ^> >(ref this._onCreateThreadDebugEvent, handler, DebugEvent.CreateThread);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateThreadEventArgs ^> >(ref this._onCreateThreadDebugEvent, handler, DebugEvent.CreateThread);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateThreadEventArgs ^>,Microsoft::Debuggers::DbgEng::CreateThreadEventArgs>(this._onCreateThreadDebugEvent, sender, args);
        }
    }

    public event EventHandler<ExitThreadEventArgs> ThreadExited
    {
        [MethodImpl(MethodImplOptions.Synchronized)] add
        {
            this.AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitThreadEventArgs ^> >(ref this._onExitThreadDebugEvent, handler, DebugEvent.ExitThread);
        }
        [MethodImpl(MethodImplOptions.Synchronized)] remove
        {
            this.RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitThreadEventArgs ^> >(ref this._onExitThreadDebugEvent, handler, DebugEvent.ExitThread);
        }
        raise
        {
            this.RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitThreadEventArgs ^>,Microsoft::Debuggers::DbgEng::ExitThreadEventArgs>(this._onExitThreadDebugEvent, sender, args);
        }
    }

    // Methods
    public unsafe DebugClient()
    {
        void* voidPtr;
        this.eventDelegCount = 0;
        this.oldOutputCallbacks = null;
        this.oldInputCallbacks = null;
        this.oldEventCallbacks = null;
        this.innerClient = null;
        this._threadAffinity = GetCurrentThreadId();
        int modopt(IsLong) hr = DebugCreate(&_GUID_e3acb9d7_7ec2_4f0c_a0da_e81e0cbbe628, &voidPtr);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        this.innerClient = (IDebugClient5*) voidPtr;
        this.InitNativeCallbacks();
    }

    internal unsafe DebugClient(IDebugClient* client)
    {
        void* voidPtr;
        this.eventDelegCount = 0;
        this.oldOutputCallbacks = null;
        this.oldInputCallbacks = null;
        this.oldEventCallbacks = null;
        this.innerClient = null;
        this._threadAffinity = GetCurrentThreadId();
        int modopt(IsLong) hr = **(*((int*) client))(client, &_GUID_e3acb9d7_7ec2_4f0c_a0da_e81e0cbbe628, &voidPtr);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        this.innerClient = (IDebugClient5*) voidPtr;
        this.InitNativeCallbacks();
    }

    internal unsafe DebugClient(IDebugClient5* client)
    {
        this.eventDelegCount = 0;
        this.oldOutputCallbacks = null;
        this.oldInputCallbacks = null;
        this.oldEventCallbacks = null;
        this.innerClient = client;
        this._threadAffinity = GetCurrentThreadId();
        **(((int*) client))[4](client);
        this.InitNativeCallbacks();
    }

    public unsafe DebugClient(string remoteOptions)
    {
        PSTRWrapperUni uni = null;
        this.eventDelegCount = 0;
        this.oldOutputCallbacks = null;
        this.oldInputCallbacks = null;
        this.oldEventCallbacks = null;
        this.innerClient = null;
        this._threadAffinity = GetCurrentThreadId();
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(remoteOptions);
        try
        {
            void* voidPtr;
            uni = uni2;
            int modopt(IsLong) hr = DebugConnectWide((ushort modopt(IsConst)*) uni.SzPtr, &_GUID_e3acb9d7_7ec2_4f0c_a0da_e81e0cbbe628, &voidPtr);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            this.innerClient = (IDebugClient5*) voidPtr;
            this.InitNativeCallbacks();
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    private unsafe void ~DebugClient()
    {
        if (this.innerClient != null)
        {
            **(((int*) this.innerClient))[0x128](this.innerClient, this.oldOutputCallbacks);
            **(((int*) this.innerClient))[0x80](this.innerClient, this.oldInputCallbacks);
            **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
        }
        if (this.oldOutputCallbacks != null)
        {
            **(((int*) this.oldOutputCallbacks))[8](this.oldOutputCallbacks);
        }
        if (this.oldInputCallbacks != null)
        {
            **(((int*) this.oldInputCallbacks))[8](this.oldInputCallbacks);
        }
        if (this.oldEventCallbacks != null)
        {
            **(((int*) this.oldEventCallbacks))[8](this.oldEventCallbacks);
        }
        if (this.nativeOutCallbacks != null)
        {
            void* voidPtr3;
            OutputCallbacksToEvent* nativeOutCallbacks = this.nativeOutCallbacks;
            if (nativeOutCallbacks != null)
            {
                voidPtr3 = Microsoft.Debuggers.DbgEng.OutputCallbacksToEvent.__delDtor(nativeOutCallbacks, 1);
            }
            else
            {
                voidPtr3 = null;
            }
        }
        if (this.nativeInCallbacks != null)
        {
            void* voidPtr2;
            InputCallbacksToEvent* nativeInCallbacks = this.nativeInCallbacks;
            if (nativeInCallbacks != null)
            {
                voidPtr2 = Microsoft.Debuggers.DbgEng.InputCallbacksToEvent.__delDtor(nativeInCallbacks, 1);
            }
            else
            {
                voidPtr2 = null;
            }
        }
        if (this.nativeEventCallbacks != null)
        {
            void* voidPtr;
            EventCallbacksToEvent* nativeEventCallbacks = this.nativeEventCallbacks;
            if (nativeEventCallbacks != null)
            {
                voidPtr = Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.__delDtor(nativeEventCallbacks, 1);
            }
            else
            {
                voidPtr = null;
            }
        }
        if (this.innerClient != null)
        {
            **(((int*) this.innerClient))[8](this.innerClient);
        }
    }

    public unsafe void AbandonCurrentProcess()
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[220](this.innerClient);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public void AddDumpInformationFile(SafeFileHandle fileHandle)
    {
        this.AddDumpInformationFile(null, fileHandle, DumpInfoFile.PageFile);
    }

    public void AddDumpInformationFile(string infoFile)
    {
        this.AddDumpInformationFile(infoFile, DumpInfoFile.PageFile);
    }

    private unsafe void AddDumpInformationFile(string infoFile, DumpInfoFile fileType)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(infoFile);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0xf8](this.innerClient, uni.SzPtr, 0L, fileType);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public void AddDumpInformationFile(string infoFile, SafeFileHandle fileHandle)
    {
        this.AddDumpInformationFile(infoFile, fileHandle, DumpInfoFile.PageFile);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods"), SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
    private unsafe void AddDumpInformationFile(string infoFile, SafeFileHandle fileHandle, DumpInfoFile fileType)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(infoFile);
        try
        {
            int modopt(IsLong) num;
            uni = uni2;
            if ((fileHandle != null) && !fileHandle.IsInvalid)
            {
                bool success = false;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    fileHandle.DangerousAddRef(ref success);
                    IntPtr handle = fileHandle.DangerousGetHandle();
                    num = **(((int*) this.innerClient))[0xf8](this.innerClient, uni.SzPtr, handle.ToInt64(), fileType);
                    if (num < 0)
                    {
                        DebugTools.ThrowDbgEngException(num);
                    }
                }
                finally
                {
                    if (success)
                    {
                        fileHandle.DangerousRelease();
                    }
                }
            }
            else
            {
                num = **(((int*) this.innerClient))[0xf8](this.innerClient, uni.SzPtr, 0L, fileType);
                if (num < 0)
                {
                    DebugTools.ThrowDbgEngException(num);
                }
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::BreakpointEventArgs ^> >(ref EventHandler<BreakpointEventArgs> thisEvent, EventHandler<BreakpointEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<BreakpointEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateProcessEventArgs ^> >(ref EventHandler<CreateProcessEventArgs> thisEvent, EventHandler<CreateProcessEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<CreateProcessEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateThreadEventArgs ^> >(ref EventHandler<CreateThreadEventArgs> thisEvent, EventHandler<CreateThreadEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<CreateThreadEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::DebuggeeStateChangeEventArgs ^> >(ref EventHandler<DebuggeeStateChangeEventArgs> thisEvent, EventHandler<DebuggeeStateChangeEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<DebuggeeStateChangeEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::EngineStateChangeEventArgs ^> >(ref EventHandler<EngineStateChangeEventArgs> thisEvent, EventHandler<EngineStateChangeEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<EngineStateChangeEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExceptionEventArgs ^> >(ref EventHandler<ExceptionEventArgs> thisEvent, EventHandler<ExceptionEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<ExceptionEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitProcessEventArgs ^> >(ref EventHandler<ExitProcessEventArgs> thisEvent, EventHandler<ExitProcessEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<ExitProcessEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitThreadEventArgs ^> >(ref EventHandler<ExitThreadEventArgs> thisEvent, EventHandler<ExitThreadEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<ExitThreadEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::LoadModuleEventArgs ^> >(ref EventHandler<LoadModuleEventArgs> thisEvent, EventHandler<LoadModuleEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<LoadModuleEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SessionStatusEventArgs ^> >(ref EventHandler<SessionStatusEventArgs> thisEvent, EventHandler<SessionStatusEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<SessionStatusEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SymbolStateChangeEventArgs ^> >(ref EventHandler<SymbolStateChangeEventArgs> thisEvent, EventHandler<SymbolStateChangeEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<SymbolStateChangeEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SystemErrorEventArgs ^> >(ref EventHandler<SystemErrorEventArgs> thisEvent, EventHandler<SystemErrorEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<SystemErrorEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private unsafe void AddEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::UnloadModuleEventArgs ^> >(ref EventHandler<UnloadModuleEventArgs> thisEvent, EventHandler<UnloadModuleEventArgs> handler, DebugEvent interest)
    {
        this.eventDelegCount++;
        thisEvent = (EventHandler<UnloadModuleEventArgs>) Delegate.Combine(thisEvent, handler);
        if ((this.eventDelegCount == 1) && (this.nativeEventCallbacks == null))
        {
            EventCallbacksToEvent* eventPtr2;
            EventCallbacksToEvent* eventPtr = @new(0x10);
            try
            {
                eventPtr2 = (eventPtr == null) ? null : Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.{ctor}(eventPtr, this, (uint modopt(IsLong)) interest);
            }
            fault
            {
                delete((void*) eventPtr);
            }
            this.nativeEventCallbacks = eventPtr2;
        }
        Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.AddInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        int modopt(IsLong) hr = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public unsafe void AttachKernel(KernelAttachMode attachMode, string connectionOptions)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(connectionOptions);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x108](this.innerClient, attachMode, uni.SzPtr);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public void AttachProcess(uint processId, ProcessAttachMode attachMode)
    {
        this.AttachProcess(0L, processId, attachMode);
    }

    public unsafe void AttachProcess(ulong serverId, uint processId, ProcessAttachMode attachMode)
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x30](this.innerClient, serverId, processId, attachMode);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public unsafe ulong ConnectProcessServer(string remoteOptions)
    {
        PSTRWrapperUni uni = null;
        ulong num3;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(remoteOptions);
        try
        {
            ulong num4;
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[280](this.innerClient, uni.SzPtr, &num4);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            num3 = num4;
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
        return num3;
    }

    public unsafe void ConnectSession(ConnectSessionMode connectMode, uint historyLimit)
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x54](this.innerClient, connectMode, historyLimit);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    private static PSTRWrapperUni ConvertEnvToPSTR(IDictionary<string, string> env)
    {
        StreamWriter writer = null;
        MemoryStream stream = null;
        StreamReader reader = null;
        string str = null;
        string[] array = null;
        PSTRWrapperUni uni;
        if ((env == null) || (env.Count <= 0))
        {
            return new PSTRWrapperUni(null);
        }
        stream = new MemoryStream();
        writer = new StreamWriter(stream);
        array = new string[env.Count];
        env.Keys.CopyTo(array, 0);
        for (int i = 0; i < env.Count; i++)
        {
            str = array[i];
            writer.Write("{0}={1}\0", str, env[str]);
        }
        writer.Write("\0");
        writer.Flush();
        stream.Seek(0L, SeekOrigin.Begin);
        reader = new StreamReader(stream);
        try
        {
            uni = new PSTRWrapperUni(reader.ReadToEnd());
        }
        finally
        {
            reader.Close();
        }
        return uni;
    }

    public unsafe DebugClient CreateClient()
    {
        IDebugClient* clientPtr;
        DebugClient client;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[120](this.innerClient, &clientPtr);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        try
        {
            client = new DebugClient(clientPtr);
        }
        finally
        {
            **(((int*) clientPtr))[8](clientPtr);
        }
        return client;
    }

    public void CreateProcess(string commandLine, CreateProcessOptions createOptions)
    {
        this.CreateProcess(0L, commandLine, createOptions);
    }

    public unsafe void CreateProcess(ulong serverId, string commandLine, CreateProcessOptions createOptions)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(commandLine);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0xe8](this.innerClient, serverId, uni.SzPtr, createOptions);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public void CreateProcess(string commandLine, DebugCreateProcessOptions createOptions, string initialDirectory, IDictionary<string, string> environment)
    {
        this.CreateProcess(0L, commandLine, createOptions, initialDirectory, environment);
    }

    public unsafe void CreateProcess(ulong serverId, string commandLine, DebugCreateProcessOptions createOptions, string initialDirectory, IDictionary<string, string> environment)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni uni2 = null;
        PSTRWrapperUni uni3 = null;
        PSTRWrapperUni modopt(IsConst) uni5 = new PSTRWrapperUni(commandLine);
        try
        {
            uni2 = uni5;
            PSTRWrapperUni modopt(IsConst) uni4 = new PSTRWrapperUni(initialDirectory);
            try
            {
                uni = uni4;
                uni3 = ConvertEnvToPSTR(environment);
                try
                {
                    ref DebugCreateProcessOptions modopt(IsExplicitlyDereferenced) pinned optionsRef = (ref DebugCreateProcessOptions modopt(IsExplicitlyDereferenced)) &createOptions;
                    try
                    {
                        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x148](this.innerClient, serverId, uni2.SzPtr, optionsRef, sizeof(DebugCreateProcessOptions), uni.SzPtr, uni3.SzPtr);
                        if (hr < 0)
                        {
                            DebugTools.ThrowDbgEngException(hr);
                        }
                    }
                    fault
                    {
                        optionsRef = 0;
                    }
                    optionsRef = 0;
                }
                finally
                {
                    IDisposable disposable = uni3;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
            fault
            {
                uni.Dispose();
            }
            uni.Dispose();
        }
        fault
        {
            uni2.Dispose();
        }
        uni2.Dispose();
    }

    public void CreateProcessAndAttach(string commandLine, CreateProcessOptions createOptions, uint processId, ProcessAttachMode attachMode)
    {
        this.CreateProcessAndAttach(0L, commandLine, createOptions, processId, attachMode);
    }

    public unsafe void CreateProcessAndAttach(ulong serverId, string commandLine, CreateProcessOptions createOptions, uint processId, ProcessAttachMode attachMode)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(commandLine);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0xec](this.innerClient, serverId, uni.SzPtr, createOptions, processId, attachMode);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public void CreateProcessAndAttach(string commandLine, DebugCreateProcessOptions createOptions, string initialDirectory, IDictionary<string, string> environment, uint processId, ProcessAttachMode attachMode)
    {
        this.CreateProcessAndAttach(0L, commandLine, createOptions, initialDirectory, environment, processId, attachMode);
    }

    public unsafe void CreateProcessAndAttach(ulong serverId, string commandLine, DebugCreateProcessOptions createOptions, string initialDirectory, IDictionary<string, string> environment, uint processId, ProcessAttachMode attachMode)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni uni2 = null;
        PSTRWrapperUni uni3 = null;
        PSTRWrapperUni modopt(IsConst) uni5 = new PSTRWrapperUni(commandLine);
        try
        {
            uni2 = uni5;
            PSTRWrapperUni modopt(IsConst) uni4 = new PSTRWrapperUni(initialDirectory);
            try
            {
                uni = uni4;
                uni3 = ConvertEnvToPSTR(environment);
                try
                {
                    ref DebugCreateProcessOptions modopt(IsExplicitlyDereferenced) pinned optionsRef = (ref DebugCreateProcessOptions modopt(IsExplicitlyDereferenced)) &createOptions;
                    try
                    {
                        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x150](this.innerClient, serverId, uni2.SzPtr, optionsRef, sizeof(DebugCreateProcessOptions), uni.SzPtr, uni3.SzPtr, processId, attachMode);
                        if (hr < 0)
                        {
                            DebugTools.ThrowDbgEngException(hr);
                        }
                    }
                    fault
                    {
                        optionsRef = 0;
                    }
                    optionsRef = 0;
                }
                finally
                {
                    IDisposable disposable = uni3;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
            fault
            {
                uni.Dispose();
            }
            uni.Dispose();
        }
        fault
        {
            uni2.Dispose();
        }
        uni2.Dispose();
    }

    public unsafe void DetachCurrentProcess()
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0xd8](this.innerClient);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public unsafe void DetachProcesses()
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[100](this.innerClient);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public unsafe void DisconnectProcessServer(ulong serverId)
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x20](this.innerClient, serverId);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public bool DispatchCallbacks()
    {
        return this.DispatchCallbacks(uint.MaxValue);
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public bool DispatchCallbacks(TimeSpan timeout)
    {
        return this.DispatchCallbacks((uint) timeout.TotalMilliseconds);
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public unsafe bool DispatchCallbacks(uint timeout)
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x70](this.innerClient, timeout);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        if (1 == hr)
        {
            return false;
        }
        return true;
    }

    public sealed override void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose([MarshalAs(UnmanagedType.U1)] bool flag1)
    {
        if (flag1)
        {
            this.~DebugClient();
        }
        else
        {
            base.Finalize();
        }
    }

    public unsafe void EndProcessServer(ulong serverId)
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[200](this.innerClient, serverId);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public unsafe void EndSession(EndSessionMode endMode)
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x68](this.innerClient, endMode);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public void ExitDispatch(DebugClient client)
    {
        UnmanagedCliWrap.ExitDispatchFrom(this, client);
    }

    public unsafe void FlushCallbacks()
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0xbc](this.innerClient);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public unsafe void GetDumpFile(uint index, out string name, out SafeFileHandle dumpFileHandle, out DumpInfoFile fileType)
    {
        uint modopt(IsLong) num3;
        uint modopt(IsLong) num4;
        ulong num5 = 0L;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x100](this.innerClient, index, 0, 0, &num3, &num5, &num4);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        IntPtr preexistingHandle = (IntPtr) ((int) num5);
        dumpFileHandle = new SafeFileHandle(preexistingHandle, false);
        ushort* numPtr = null;
        try
        {
            uint modopt(IsLong) num2 = num3 + 1;
            numPtr = new[]((num2 > 0x7fffffff) ? uint.MaxValue : ((uint) (num2 * 2)));
            hr = **(((int*) this.innerClient))[260](this.innerClient, index, numPtr, num3 + 1, 0, 0, &num4);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            name = new string((char*) numPtr);
            fileType = num4;
        }
        finally
        {
            if (numPtr != null)
            {
                delete[]((void*) numPtr);
            }
        }
    }

    public unsafe uint GetExitCode()
    {
        uint modopt(IsLong) num3;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x6c](this.innerClient, &num3);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        if (1 == hr)
        {
            throw new InvalidOperationException("Process is still running.");
        }
        return num3;
    }

    public unsafe string GetIdentity()
    {
        uint modopt(IsLong) num4;
        string str;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0xac](this.innerClient, 0, 0, &num4);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        ushort* numPtr2 = null;
        try
        {
            byte num2;
            uint modopt(IsLong) num3 = num4 + 1;
            ushort* numPtr = new[]((num3 > 0x7fffffff) ? uint.MaxValue : ((uint) (num3 * 2)));
            hr = **(((int*) this.innerClient))[0x134](this.innerClient, numPtr, num4, 0);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            if (hr != 1)
            {
                num2 = 1;
            }
            else
            {
                num2 = 0;
            }
            Debug.Assert((bool) num2, "Buffer should be of the required size");
            str = new string((char*) numPtr);
        }
        finally
        {
            if (numPtr2 != null)
            {
                delete[]((void*) numPtr2);
            }
        }
        return str;
    }

    public unsafe string GetKernelConnectionOptions()
    {
        uint modopt(IsLong) num3;
        string str;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x10](this.innerClient, 0, 0, &num3);
        if (hr < 0)
        {
            if (hr == -2147418113)
            {
                throw new InvalidOperationException("Current target is not a non-local live kernel target.");
            }
            DebugTools.ThrowDbgEngException(hr);
        }
        uint modopt(IsLong) num2 = num3 + 1;
        ushort* numPtr = new[]((num2 > 0x7fffffff) ? uint.MaxValue : ((uint) (num2 * 2)));
        try
        {
            hr = **(((int*) this.innerClient))[0x10c](this.innerClient, numPtr, num3, 0);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            str = new string((char*) numPtr);
        }
        finally
        {
            delete[]((void*) numPtr);
        }
        return str;
    }

    public OutputModes GetOtherOutputMask(DebugClient other)
    {
        return UnmanagedCliWrap.GetOtherOutputMask(this, other);
    }

    public unsafe string GetOutputLinePrefix()
    {
        uint modopt(IsLong) num3;
        string str;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0xa4](this.innerClient, 0, 0, &num3);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        ushort* numPtr2 = null;
        try
        {
            uint modopt(IsLong) num2 = num3 + 1;
            ushort* numPtr = new[]((num2 > 0x7fffffff) ? uint.MaxValue : ((uint) (num2 * 2)));
            hr = **(((int*) this.innerClient))[300](this.innerClient, numPtr, num3, 0);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            str = new string((char*) numPtr);
        }
        finally
        {
            if (numPtr2 != null)
            {
                delete[]((void*) numPtr2);
            }
        }
        return str;
    }

    public unsafe string GetQuitLockString()
    {
        uint modopt(IsLong) num3;
        string str;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x16c](this.innerClient, 0, 0, &num3);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        uint modopt(IsLong) num2 = num3 + 1;
        ushort* numPtr = new[]((num2 > 0x7fffffff) ? uint.MaxValue : ((uint) (num2 * 2)));
        try
        {
            hr = **(((int*) this.innerClient))[0x174](this.innerClient, numPtr, num3, 0);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            str = new string((char*) numPtr);
        }
        finally
        {
            delete[]((void*) numPtr);
        }
        return str;
    }

    public void GetRunningProcessDescription(uint systemId, ProcessDescription descriptionOptions, out string exeName, out string description)
    {
        this.GetRunningProcessDescription(0L, systemId, descriptionOptions, out exeName, out description);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider")]
    public unsafe void GetRunningProcessDescription(ulong serverId, uint systemId, ProcessDescription descriptionOptions, out string exeName, out string description)
    {
        uint modopt(IsLong) num2;
        uint modopt(IsLong) num3;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0xe4](this.innerClient, serverId, systemId, descriptionOptions, 0, 0, &num3, 0, 0, &num2);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        ushort* numPtr2 = null;
        ushort* numPtr = null;
        try
        {
            byte num4;
            uint modopt(IsLong) num7;
            uint modopt(IsLong) num8;
            uint modopt(IsLong) num6 = num3;
            numPtr = new[]((num6 > 0x7fffffff) ? uint.MaxValue : ((uint) (num6 * 2)));
            uint modopt(IsLong) num5 = num2;
            numPtr2 = new[]((num5 > 0x7fffffff) ? uint.MaxValue : ((uint) (num5 * 2)));
            hr = **(((int*) this.innerClient))[0xe4](this.innerClient, serverId, systemId, descriptionOptions, numPtr, num3, &num8, numPtr2, num2, &num7);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            exeName = new string((char*) numPtr);
            description = new string((char*) numPtr2);
            object[] args = new object[] { (uint) num3, (uint) num2, (uint) num8, (uint) num7, exeName, description };
            if (hr != 1)
            {
                num4 = 1;
            }
            else
            {
                num4 = 0;
            }
            Debug.Assert((bool) num4, "All buffers should be of sufficient size", string.Format("Current size: Name={0}, Desc={1}; Requested size: Name={2}, Desc={3}; Buffers: Name={4}, Desc={5}", args));
        }
        finally
        {
            if (numPtr != null)
            {
                delete[]((void*) numPtr);
            }
            if (numPtr2 != null)
            {
                delete[]((void*) numPtr2);
            }
        }
    }

    public uint GetRunningProcessSystemIdByExecutableName(string exeName, ProcessNameMatch match)
    {
        return this.GetRunningProcessSystemIdByExecutableName(0L, exeName, match);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider")]
    public unsafe uint GetRunningProcessSystemIdByExecutableName(ulong serverId, string exeName, ProcessNameMatch match)
    {
        PSTRWrapperUni uni = null;
        uint num3;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(exeName);
        try
        {
            uint modopt(IsLong) num4;
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0xe0](this.innerClient, serverId, uni.SzPtr, match, &num4);
            if (hr < 0)
            {
                if (hr == -2147467262)
                {
                    throw new ArgumentException(string.Format("No process matched the given executable file name {0}", exeName), "exeName");
                }
                DebugTools.ThrowDbgEngException(hr);
            }
            if (hr == 1)
            {
                throw new ArgumentException(string.Format("More than one process matched the given executable file name {0}", exeName), "exeName");
            }
            num3 = num4;
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
        return num3;
    }

    public uint[] GetRunningProcessSystemIds()
    {
        return this.GetRunningProcessSystemIds(0L);
    }

    public unsafe uint[] GetRunningProcessSystemIds(ulong serverId)
    {
        uint[] numArray = null;
        uint modopt(IsLong) num2;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x24](this.innerClient, serverId, 0, 0, &num2);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        numArray = new uint[num2];
        ref uint modopt(IsExplicitlyDereferenced) pinned numRef = (ref uint modopt(IsExplicitlyDereferenced)) &(numArray[0]);
        hr = **(((int*) this.innerClient))[0x24](this.innerClient, serverId, numRef, num2, 0);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        return numArray;
    }

    private unsafe void InitNativeCallbacks()
    {
        void* voidPtr3;
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x124](this.innerClient, &voidPtr3);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        this.oldOutputCallbacks = (IDebugOutputCallbacksWide*) voidPtr3;
        if (this.oldOutputCallbacks != null)
        {
            **(((int*) this.oldOutputCallbacks))[4](this.oldOutputCallbacks);
        }
        void* voidPtr2 = null;
        hr = **(((int*) this.innerClient))[0x7c](this.innerClient, &voidPtr2);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        this.oldInputCallbacks = (IDebugInputCallbacks*) voidPtr2;
        if (this.oldInputCallbacks != null)
        {
            **(((int*) this.oldInputCallbacks))[4](this.oldInputCallbacks);
        }
        void* voidPtr = null;
        hr = **(((int*) this.innerClient))[0x13c](this.innerClient, &voidPtr);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        this.oldEventCallbacks = (IDebugEventCallbacksWide*) voidPtr;
        if (this.oldEventCallbacks != null)
        {
            **(((int*) this.oldEventCallbacks))[4](this.oldEventCallbacks);
        }
    }

    internal DebugStatus OnBreakpointHit(DebugBreakpoint bp)
    {
        BreakpointEventArgs args = null;
        args = new BreakpointEventArgs(bp);
        this.raise_BreakpointHit(this, args);
        return args.EventStatus;
    }

    internal void OnDebuggeeStateChanged(DebuggeeStateChange change, ulong argument)
    {
        this.raise_DebuggeeStateChanged(this, new DebuggeeStateChangeEventArgs(change, argument));
    }

    internal void OnDebugInputEnded()
    {
        this.raise_DebugInputEnded(this, new DebugEndInputEventArgs());
    }

    internal void OnDebugOutput(OutputModes outputMask, string text)
    {
        this.raise_DebugOutput(this, new DebugOutputEventArgs(outputMask, text));
    }

    internal void OnDebugStartInput(uint bufferSize)
    {
        this.raise_DebugInputStarted(this, new DebugStartInputEventArgs(bufferSize));
    }

    internal void OnEngineStateChanged(EngineStateChange change, ulong argument)
    {
        this.raise_EngineStateChanged(this, new EngineStateChangeEventArgs(change, argument));
    }

    internal DebugStatus OnExceptionHit(ExceptionRecord exceptionRecord, [MarshalAs(UnmanagedType.U1)] bool firstChance)
    {
        ExceptionEventArgs args = null;
        args = new ExceptionEventArgs(exceptionRecord, firstChance);
        this.raise_ExceptionHit(this, args);
        return args.EventStatus;
    }

    internal DebugStatus OnModuleLoaded(SafeFileHandle imageFileHandle, ulong baseOffset, uint moduleSize, string moduleName, string imageName, uint checkSum, uint timeDateStamp)
    {
        LoadModuleEventArgs args = null;
        args = new LoadModuleEventArgs(imageFileHandle, baseOffset, moduleSize, moduleName, imageName, checkSum, timeDateStamp);
        this.raise_ModuleLoaded(this, args);
        return args.EventStatus;
    }

    internal DebugStatus OnModuleUnloaded(string imageBaseName, ulong baseOffset)
    {
        UnloadModuleEventArgs args = null;
        args = new UnloadModuleEventArgs(imageBaseName, baseOffset);
        this.raise_ModuleUnloaded(this, args);
        return args.EventStatus;
    }

    internal DebugStatus OnProcessCreated(SafeFileHandle imageFileHandle, SafeHandle handle, ulong baseOffset, uint moduleSize, string moduleName, string imageName, uint checkSum, uint timeDateStamp, SafeHandle initialThreadHandle, ulong threadDataOffset, ulong startOffset)
    {
        CreateProcessEventArgs args = null;
        args = new CreateProcessEventArgs(imageFileHandle, handle, baseOffset, moduleSize, moduleName, imageName, checkSum, timeDateStamp, initialThreadHandle, threadDataOffset, startOffset);
        this.raise_ProcessCreated(this, args);
        return args.EventStatus;
    }

    internal DebugStatus OnProcessExited(uint exitCode)
    {
        ExitProcessEventArgs args = null;
        args = new ExitProcessEventArgs(exitCode);
        this.raise_ProcessExited(this, args);
        return args.EventStatus;
    }

    internal void OnSessionStatusChanged(SessionStatus status)
    {
        this.raise_SessionStatusChanged(this, new SessionStatusEventArgs(status));
    }

    internal void OnSymbolStateChanged(SymbolStateChange change, ulong argument)
    {
        this.raise_SymbolStateChanged(this, new SymbolStateChangeEventArgs(change, argument));
    }

    internal DebugStatus OnSystemErrorRaised(uint error, uint level)
    {
        SystemErrorEventArgs args = null;
        args = new SystemErrorEventArgs(error, level);
        this.raise_SystemErrorRaised(this, args);
        return args.EventStatus;
    }

    internal DebugStatus OnThreadCreated(SafeHandle handle, ulong dataOffset, ulong startOffset)
    {
        CreateThreadEventArgs args = null;
        args = new CreateThreadEventArgs(handle, dataOffset, startOffset);
        this.raise_ThreadCreated(this, args);
        return args.EventStatus;
    }

    internal DebugStatus OnThreadExited(uint exitCode)
    {
        ExitThreadEventArgs args = null;
        args = new ExitThreadEventArgs(exitCode);
        this.raise_ThreadExited(this, args);
        return args.EventStatus;
    }

    public void OpenDumpFile(SafeFileHandle dumpFileHandle)
    {
        this.OpenDumpFile(null, dumpFileHandle);
    }

    public unsafe void OpenDumpFile(string dumpFile)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(dumpFile);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[240](this.innerClient, uni.SzPtr, 0L);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods"), SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
    public unsafe void OpenDumpFile(string dumpFile, SafeFileHandle dumpFileHandle)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(dumpFile);
        try
        {
            int modopt(IsLong) num;
            uni = uni2;
            if ((dumpFileHandle != null) && !dumpFileHandle.IsInvalid)
            {
                bool success = false;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    dumpFileHandle.DangerousAddRef(ref success);
                    IntPtr handle = dumpFileHandle.DangerousGetHandle();
                    num = **(((int*) this.innerClient))[240](this.innerClient, uni.SzPtr, handle.ToInt64());
                    if (num < 0)
                    {
                        DebugTools.ThrowDbgEngException(num);
                    }
                }
                finally
                {
                    if (success)
                    {
                        dumpFileHandle.DangerousRelease();
                    }
                }
            }
            else
            {
                IntPtr ptr = dumpFileHandle.DangerousGetHandle();
                num = **(((int*) this.innerClient))[240](this.innerClient, uni.SzPtr, ptr.ToInt64());
                if (num < 0)
                {
                    DebugTools.ThrowDbgEngException(num);
                }
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public void OutputIdentity(OutputModes output, string format)
    {
        this.OutputIdentity(output, OutputIdentity.Default, format);
    }

    private unsafe void OutputIdentity(OutputModes output, OutputIdentity identity, string format)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(format);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x138](this.innerClient, output, identity, uni.SzPtr);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public unsafe void OutputServers(OutputModes output, string machine, ServersOutput serversOutput)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(machine);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x120](this.innerClient, output, uni.SzPtr, serversOutput);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public unsafe void PopOutputLinePrefix(ulong prefixHandle)
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x15c](this.innerClient, prefixHandle);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public unsafe ulong PushOutputLinePrefix(string newPrefix)
    {
        PSTRWrapperUni uni = null;
        ulong num3;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(newPrefix);
        try
        {
            ulong num4;
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x158](this.innerClient, uni.SzPtr, &num4);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            num3 = num4;
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
        return num3;
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::BreakpointEventArgs ^>,Microsoft::Debuggers::DbgEng::BreakpointEventArgs>(EventHandler<BreakpointEventArgs> thisEvent, object sender, BreakpointEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateProcessEventArgs ^>,Microsoft::Debuggers::DbgEng::CreateProcessEventArgs>(EventHandler<CreateProcessEventArgs> thisEvent, object sender, CreateProcessEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateThreadEventArgs ^>,Microsoft::Debuggers::DbgEng::CreateThreadEventArgs>(EventHandler<CreateThreadEventArgs> thisEvent, object sender, CreateThreadEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::DebuggeeStateChangeEventArgs ^>,Microsoft::Debuggers::DbgEng::DebuggeeStateChangeEventArgs>(EventHandler<DebuggeeStateChangeEventArgs> thisEvent, object sender, DebuggeeStateChangeEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::EngineStateChangeEventArgs ^>,Microsoft::Debuggers::DbgEng::EngineStateChangeEventArgs>(EventHandler<EngineStateChangeEventArgs> thisEvent, object sender, EngineStateChangeEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::ExceptionEventArgs ^>,Microsoft::Debuggers::DbgEng::ExceptionEventArgs>(EventHandler<ExceptionEventArgs> thisEvent, object sender, ExceptionEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitProcessEventArgs ^>,Microsoft::Debuggers::DbgEng::ExitProcessEventArgs>(EventHandler<ExitProcessEventArgs> thisEvent, object sender, ExitProcessEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitThreadEventArgs ^>,Microsoft::Debuggers::DbgEng::ExitThreadEventArgs>(EventHandler<ExitThreadEventArgs> thisEvent, object sender, ExitThreadEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::LoadModuleEventArgs ^>,Microsoft::Debuggers::DbgEng::LoadModuleEventArgs>(EventHandler<LoadModuleEventArgs> thisEvent, object sender, LoadModuleEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::SessionStatusEventArgs ^>,Microsoft::Debuggers::DbgEng::SessionStatusEventArgs>(EventHandler<SessionStatusEventArgs> thisEvent, object sender, SessionStatusEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::SymbolStateChangeEventArgs ^>,Microsoft::Debuggers::DbgEng::SymbolStateChangeEventArgs>(EventHandler<SymbolStateChangeEventArgs> thisEvent, object sender, SymbolStateChangeEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::SystemErrorEventArgs ^>,Microsoft::Debuggers::DbgEng::SystemErrorEventArgs>(EventHandler<SystemErrorEventArgs> thisEvent, object sender, SystemErrorEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private void RaiseDebugEvent<System::EventHandler<Microsoft::Debuggers::DbgEng::UnloadModuleEventArgs ^>,Microsoft::Debuggers::DbgEng::UnloadModuleEventArgs>(EventHandler<UnloadModuleEventArgs> thisEvent, object sender, UnloadModuleEventArgs args)
    {
        if (thisEvent != null)
        {
            thisEvent(sender, args);
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::BreakpointEventArgs ^> >(ref EventHandler<BreakpointEventArgs> thisEvent, EventHandler<BreakpointEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<BreakpointEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateProcessEventArgs ^> >(ref EventHandler<CreateProcessEventArgs> thisEvent, EventHandler<CreateProcessEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<CreateProcessEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::CreateThreadEventArgs ^> >(ref EventHandler<CreateThreadEventArgs> thisEvent, EventHandler<CreateThreadEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<CreateThreadEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::DebuggeeStateChangeEventArgs ^> >(ref EventHandler<DebuggeeStateChangeEventArgs> thisEvent, EventHandler<DebuggeeStateChangeEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<DebuggeeStateChangeEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::EngineStateChangeEventArgs ^> >(ref EventHandler<EngineStateChangeEventArgs> thisEvent, EventHandler<EngineStateChangeEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<EngineStateChangeEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExceptionEventArgs ^> >(ref EventHandler<ExceptionEventArgs> thisEvent, EventHandler<ExceptionEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<ExceptionEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitProcessEventArgs ^> >(ref EventHandler<ExitProcessEventArgs> thisEvent, EventHandler<ExitProcessEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<ExitProcessEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::ExitThreadEventArgs ^> >(ref EventHandler<ExitThreadEventArgs> thisEvent, EventHandler<ExitThreadEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<ExitThreadEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::LoadModuleEventArgs ^> >(ref EventHandler<LoadModuleEventArgs> thisEvent, EventHandler<LoadModuleEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<LoadModuleEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SessionStatusEventArgs ^> >(ref EventHandler<SessionStatusEventArgs> thisEvent, EventHandler<SessionStatusEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<SessionStatusEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SymbolStateChangeEventArgs ^> >(ref EventHandler<SymbolStateChangeEventArgs> thisEvent, EventHandler<SymbolStateChangeEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<SymbolStateChangeEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::SystemErrorEventArgs ^> >(ref EventHandler<SystemErrorEventArgs> thisEvent, EventHandler<SystemErrorEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<SystemErrorEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    private unsafe void RemoveEventCallbacksHandler<System::EventHandler<Microsoft::Debuggers::DbgEng::UnloadModuleEventArgs ^> >(ref EventHandler<UnloadModuleEventArgs> thisEvent, EventHandler<UnloadModuleEventArgs> handler, DebugEvent interest)
    {
        int modopt(IsLong) num;
        int modopt(IsConst) num2 = thisEvent.GetInvocationList().Length - 1;
        this.eventDelegCount--;
        thisEvent = (EventHandler<UnloadModuleEventArgs>) Delegate.Remove(thisEvent, handler);
        if (this.eventDelegCount == 0)
        {
            num = **(((int*) this.innerClient))[320](this.innerClient, this.oldEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
            Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
        }
        else
        {
            if (num2 == 0)
            {
                Microsoft.Debuggers.DbgEng.EventCallbacksToEvent.RemoveInterestMask(this.nativeEventCallbacks, (uint modopt(IsLong)) interest);
            }
            num = **(((int*) this.innerClient))[320](this.innerClient, this.nativeEventCallbacks);
            if (num < 0)
            {
                DebugTools.ThrowDbgEngException(num);
            }
        }
    }

    public unsafe void SetKernelConnectionOptions(string val)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(val);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x110](this.innerClient, uni.SzPtr);
            if (hr < 0)
            {
                if (hr == -2147418113)
                {
                    throw new InvalidOperationException("Current target is not a non-local live kernel target.");
                }
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public void SetOtherOutputMask(DebugClient other, OutputModes outputMask)
    {
        UnmanagedCliWrap.SetOtherOutputMask(this, other, outputMask);
    }

    public unsafe void SetOutputLinePrefix(string prefix)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(prefix);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x130](this.innerClient, uni.SzPtr);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public unsafe void SetQuitLockString(string lockString)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(lockString);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x178](this.innerClient, uni.SzPtr);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public unsafe void StartProcessServer(DebugClass debugClass, string options)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(options);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x114](this.innerClient, debugClass, uni.SzPtr, 0);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public unsafe void StartServer(string options)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(options);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x11c](this.innerClient, uni.SzPtr);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public unsafe void TerminateCurrentProcess()
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0xd4](this.innerClient);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    public unsafe void TerminateProcesses()
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0x60](this.innerClient);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public bool WaitForProcessServerEnd(TimeSpan timeout)
    {
        return this.WaitForProcessServerEnd((uint) timeout.TotalMilliseconds);
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public unsafe bool WaitForProcessServerEnd(uint timeout)
    {
        int modopt(IsLong) hr = **(((int*) this.innerClient))[0xcc](this.innerClient, timeout);
        if (hr < 0)
        {
            DebugTools.ThrowDbgEngException(hr);
        }
        return (0 == hr);
    }

    public unsafe void WriteDumpFile(string dumpFile, DumpQualifier dumpQualifier)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni modopt(IsConst) uni2 = new PSTRWrapperUni(dumpFile);
        try
        {
            uni = uni2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0xf4](this.innerClient, uni.SzPtr, 0L, dumpQualifier, 0, 0);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
        fault
        {
            uni.Dispose();
        }
        uni.Dispose();
    }

    public void WriteDumpFile(SafeFileHandle dumpFileHandle, DumpQualifier dumpQualifier, DumpFormats format, string comment)
    {
        this.WriteDumpFile(null, dumpFileHandle, dumpQualifier, format, comment);
    }

    public void WriteDumpFile(string dumpFile, DumpQualifier dumpQualifier, DumpFormats format, string comment)
    {
        this.WriteDumpFile(dumpFile, null, dumpQualifier, format, comment);
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods"), SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
    public unsafe void WriteDumpFile(string dumpFile, SafeFileHandle dumpFileHandle, DumpQualifier dumpQualifier, DumpFormats format, string comment)
    {
        PSTRWrapperUni uni = null;
        PSTRWrapperUni uni2 = null;
        PSTRWrapperUni modopt(IsConst) uni4 = new PSTRWrapperUni(dumpFile);
        try
        {
            uni2 = uni4;
            PSTRWrapperUni modopt(IsConst) uni3 = new PSTRWrapperUni(comment);
            try
            {
                int modopt(IsLong) num;
                uni = uni3;
                if ((dumpFileHandle != null) && !dumpFileHandle.IsInvalid)
                {
                    bool success = false;
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                        dumpFileHandle.DangerousAddRef(ref success);
                        IntPtr handle = dumpFileHandle.DangerousGetHandle();
                        num = **(((int*) this.innerClient))[0xf4](this.innerClient, uni2.SzPtr, handle.ToInt64(), dumpQualifier, format, uni.SzPtr);
                        if (num < 0)
                        {
                            DebugTools.ThrowDbgEngException(num);
                        }
                    }
                    finally
                    {
                        if (success)
                        {
                            dumpFileHandle.DangerousRelease();
                        }
                    }
                }
                else
                {
                    num = **(((int*) this.innerClient))[0xf4](this.innerClient, uni2.SzPtr, 0L, dumpQualifier, format, uni.SzPtr);
                    if (num < 0)
                    {
                        DebugTools.ThrowDbgEngException(num);
                    }
                }
            }
            fault
            {
                uni.Dispose();
            }
            uni.Dispose();
        }
        fault
        {
            uni2.Dispose();
        }
        uni2.Dispose();
    }

    // Properties
    public DebugProcessOptions DebugProcessOptions
    {
        get
        {
            uint modopt(IsLong) num2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[60](this.innerClient, &num2);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            return num2;
        }
        set
        {
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x48](this.innerClient, value);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
    }

    internal IUnknown* InnerNativeClient
    {
        get
        {
            return (IUnknown*) this.innerClient;
        }
    }

    public bool IsKernelDebuggerEnabled
    {
        [return: MarshalAs(UnmanagedType.U1)]
        get
        {
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0xd0](this.innerClient);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            return (hr == 0);
        }
    }

    public uint NumberDumpFiles
    {
        get
        {
            uint modopt(IsLong) num3;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0xfc](this.innerClient, &num3);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            return num3;
        }
    }

    public OutputModes OutputMask
    {
        get
        {
            uint modopt(IsLong) num2;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[140](this.innerClient, &num2);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            return num2;
        }
        set
        {
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x90](this.innerClient, value);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
    }

    public uint OutputWidth
    {
        get
        {
            uint modopt(IsLong) num3;
            int modopt(IsLong) hr = **(((int*) this.innerClient))[0x9c](this.innerClient, &num3);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            return num3;
        }
        set
        {
            int modopt(IsLong) hr = **(((int*) this.innerClient))[160](this.innerClient, value);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
    }

    public uint ThreadAffinity
    {
        get
        {
            return this._threadAffinity;
        }
    }

    // Nested Types
    [StructLayout(LayoutKind.Sequential, Size=1)]
    private struct UnmanagedCliWrap
    {
        public static unsafe void ExitDispatchFrom(DebugClient fromCli, DebugClient targetCli)
        {
            int modopt(IsLong) hr = **(((int*) fromCli.innerClient))[0x74](fromCli.innerClient, targetCli.innerClient);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }

        public static unsafe OutputModes GetOtherOutputMask(DebugClient thisCli, DebugClient otherCli)
        {
            uint modopt(IsLong) num2;
            int modopt(IsLong) hr = **(((int*) thisCli.innerClient))[0x94](thisCli.innerClient, otherCli.innerClient, &num2);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
            return num2;
        }

        public static unsafe void SetOtherOutputMask(DebugClient thisCli, DebugClient otherCli, OutputModes outputMask)
        {
            int modopt(IsLong) hr = **(((int*) thisCli.innerClient))[0x98](thisCli.innerClient, otherCli.innerClient, outputMask);
            if (hr < 0)
            {
                DebugTools.ThrowDbgEngException(hr);
            }
        }
    }
}

 
Collapse Methods
 
