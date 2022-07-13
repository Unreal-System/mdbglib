#pragma once

#include "DebugOutputEventArgs.h"

using namespace System;

namespace MS
{
	namespace Debuggers
	{
		namespace DbgEng
		{
			ref class Debuggee; //Just tell it that there is such a thing that Debuggee

			class OutputCallbackWrapper : public IDebugOutputCallbacksWide
			{
			private:
				LONG  m_refCount;
				gcroot<Debuggee^> *m_pdbg; 

			public:
				OutputCallbackWrapper(MS::Debuggers::DbgEng::Debuggee^ dbg);

				STDMETHOD_(ULONG, AddRef)(THIS);
				STDMETHOD_(ULONG, Release)(THIS);
				STDMETHOD(QueryInterface)(THIS_
					IN REFIID interfaceId,
					OUT PVOID* ppInterface);

				STDMETHOD(Output)(
					THIS_ 
					__in ULONG Mask,
					__in PCWSTR Text);

				~OutputCallbackWrapper() 
				{
					if(NULL != m_pdbg)
						delete m_pdbg; 
				}
			};
		}
	}
}