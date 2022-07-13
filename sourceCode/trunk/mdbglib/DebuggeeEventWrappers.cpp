#include "StdAfx.h"
#include "DebuggeeEventWrappers.h"
#include "Debuggee.h"

namespace MS
{
	namespace Debuggers
	{
		namespace DbgEng
		{
			OutputCallbackWrapper::OutputCallbackWrapper(Debuggee ^dbg) : m_refCount(0), m_pdbg(new gcroot<Debuggee^>(dbg))
			{
			} 
			STDMETHODIMP OutputCallbackWrapper::Output(ULONG mask, PCWSTR text)
			{
				if(NULL == this->m_pdbg) //Very unlikely considering the only constructor
					return S_FALSE;

				DbgEng::OutputFlags mmask = static_cast<DbgEng::OutputFlags>(mask);
				String ^mtext = gcnew String(text);
				(*(this->m_pdbg))->OnDebugOutput(mmask, mtext);
				return S_OK;
			}

			STDMETHODIMP_(ULONG) OutputCallbackWrapper::AddRef(THIS) 
			{
				InterlockedIncrement(&m_refCount);
				return m_refCount;
			}

			STDMETHODIMP_(ULONG) OutputCallbackWrapper::Release(THIS) 
			{
				LONG retVal;
				InterlockedDecrement(&m_refCount);
				retVal = m_refCount;
				if (retVal == 0) 
				{
					delete this;
				}
				return retVal;
			}

			STDMETHODIMP OutputCallbackWrapper::QueryInterface(THIS_
				IN REFIID interfaceId,
				OUT PVOID* ppInterface) 
			{
				*ppInterface = 0;
				HRESULT res = E_NOINTERFACE;
				if (TRUE == IsEqualIID(interfaceId, __uuidof(IUnknown)) || TRUE == IsEqualIID(interfaceId, __uuidof(IDebugOutputCallbacks))) 
				{
					*ppInterface = (IDebugOutputCallbacks*) this;
					AddRef();
					res = S_OK;
				}
				return res;
			}
		}
	}
}