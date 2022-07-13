// dbgengtest.cpp : Defines the entry point for the console application.
//

#include "dbgeng.h"
#include "stdafx.h"
#include <iostream>

using namespace std;

void CheckHR(HRESULT hr);
bool TestGetStackTrace();

int _tmain(int argc, _TCHAR* argv[])
{
	TestGetStackTrace();
	return 0;
}

bool TestGetStackTrace()
{
	PCSTR pSymbolPath = "..\\dumps\\TestApp\\1\\";
	PCSTR pDumpFilePath = "..\\dumps\\TestApp\\1\\TestAppAV.dmp";

	IDebugClient5* debugClient = NULL;
	IDebugSymbols3* debugSymbols = NULL;
	IDebugControl4* debugControl = NULL;
	PDEBUG_STACK_FRAME pStackFrames = NULL;
	ULONG maxFrames = 100;
	__try
	{
		CheckHR(DebugCreate( __uuidof(IDebugClient5), (void **)&debugClient ));
		CheckHR(debugClient->QueryInterface(__uuidof(IDebugSymbols3),(void **)&debugSymbols));
		CheckHR(debugClient->QueryInterface(__uuidof(IDebugControl4),(void **)&debugControl));

		CheckHR(debugSymbols->SetSymbolPath(pSymbolPath));
		CheckHR(debugClient->OpenDumpFile(pDumpFilePath));
		CheckHR(debugControl->WaitForEvent(0, 0));

		CheckHR(debugSymbols->SetScopeFromStoredEvent());

		pStackFrames = new _DEBUG_STACK_FRAME[maxFrames];

		ULONG framesFilled;
		CheckHR(debugControl->GetStackTrace(0, 0, 0, pStackFrames, maxFrames, &framesFilled));

		for(ULONG i=0; i<framesFilled; i++)
		{
			PSTR pFunctionName = NULL;
			__try
			{
				ULONG nameSize;
				CheckHR(debugSymbols->GetNameByOffset(pStackFrames[i].InstructionOffset, NULL, 0, &nameSize, NULL));
				pFunctionName = new CHAR[nameSize];
				CheckHR(debugSymbols->GetNameByOffset(pStackFrames[i].InstructionOffset, pFunctionName, nameSize, NULL, NULL));
				printf("%s \n",pFunctionName);
			}
			__finally
			{
				delete[] pFunctionName;
			}
		}
	}
	__finally
	{
		debugClient->Release();
		debugSymbols->Release();
		debugControl->Release();
		delete[] pStackFrames;
	}
	return true;
}

class myException{};
void CheckHR(HRESULT hr)
{
	if(hr < 0)
	{
		throw myException();
	}
}
