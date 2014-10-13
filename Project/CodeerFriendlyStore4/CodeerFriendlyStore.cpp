#include "stdafx.h"
#include <windows.h>
using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;

/**
	@brief	������
	@param pStartInfo �J�n���
	@return ����
*/
DWORD __stdcall Initialize(void* pStartInfo)
{
	String^ startInfo = Marshal::PtrToStringUni((System::IntPtr)pStartInfo);

	//�Z�p���[�^
	String^ Separator = "###";

	//�E�B���h�E�n���h���ƃA�Z���u�����̂ɕ�����B
	int index = startInfo->IndexOf(Separator);
	if (index == -1) {
		return 1;
	}
	String^ handle = startInfo->Substring(0, index);
	String^ asmName = startInfo->Substring(index + Separator->Length);

	//�A�Z���u������SystemStarterInApp���擾����
	Assembly^ assembly = Assembly::Load(asmName);

	//���O���ߑł�
	Type^ type = assembly->GetType("Codeer.Friendly.Store.Core.SystemStarterInApp");

	//�J�n�֐��Ăяo��
	MethodInfo^ start = type->GetMethod("Start");

	array<Object^>^ args = gcnew array<Object^> { handle };

	start->Invoke(nullptr, args);
	return 1;
}
