#include "stdafx.h"
#include <windows.h>
using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;

/**
	@brief	初期化
	@param pStartInfo 開始情報
	@return 結果
*/
DWORD __stdcall Initialize(void* pStartInfo)
{
	String^ startInfo = Marshal::PtrToStringUni((System::IntPtr)pStartInfo);

	//セパレータ
	String^ Separator = "###";

	//ウィンドウハンドルとアセンブリ名称に分ける。
	int index = startInfo->IndexOf(Separator);
	if (index == -1) {
		return 1;
	}
	String^ handle = startInfo->Substring(0, index);
	String^ asmName = startInfo->Substring(index + Separator->Length);

	//アセンブリからSystemStarterInAppを取得する
	Assembly^ assembly = Assembly::Load(asmName);

	//名前決め打ち
	Type^ type = assembly->GetType("Codeer.Friendly.Store.Core.SystemStarterInApp");

	//開始関数呼び出し
	MethodInfo^ start = type->GetMethod("Start");

	array<Object^>^ args = gcnew array<Object^> { handle };

	start->Invoke(nullptr, args);
	return 1;
}
