// TestApplication.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "iostream"

using namespace std;

void AccessViolation(int *x, int y);

bool g_bool = true;
char *g_string = "Some global string";

int _tmain(int argc, _TCHAR* argv[])
{
	int x;
	cout << "1. Access Violation\n";
	cin >> x;
	switch(x)
	{
		case 1: AccessViolation(NULL, 4); break;
		default: cout<< "Unknown command";
	}
	return 0;
}

void AccessViolation(int *x, int y)
{
	*(x) = 3;
}


