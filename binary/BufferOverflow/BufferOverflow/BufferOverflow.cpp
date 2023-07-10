// References
//    https://www.codeproject.com/articles/1854/compiler-security-checks-in-depth
//    https://kallanreed.wordpress.com/2015/02/14/disabling-the-stack-cookie-generation-in-visual-studio-2013/
//    https://docs.microsoft.com/en-us/cpp/build/reference/gs-buffer-security-check?
// 
// The following setting will enable the generation of assembly code (*.cod file) which when studied will help you to understand why the overflow buffer needs to be the size it is.
//    Project Properties->Configuration Properties->C / C++ -> Output Files -> Assembler Output ===== Assembly, Machine Code and Source (/FAcs)
//
// This will ensure the base address of the code segment is the same each time the module is loaded. Without it, you can't hard code the RET address.
//    Project Properties->Configuration Properties-> Linker -> Advanced -> Randomized Base Address ===== No
//
// Disable debugging of Just My Code. The generated assembly will now only contain our demo code and nothing auto generated.
//    Project Properties->Configuration Properties > C/C++ > General -> Support Just My Code Debugging ===== No
// 
// Turn off the generation of stack cookies/canaries.
//    Project Properties->Configuration Properties->C / C++-> Code Generation -> Security Check ===== Disable Security Check /GS, i.e. /GS-

// X64 assembly starter guide - https://www.intel.com/content/dam/develop/external/us/en/documents/introduction-to-x64-assembly-181178.pdf
// X64 assembly reference guide - https://www.intel.com/content/www/us/en/content-details/782149/intel-64-and-ia-32-architectures-software-developer-s-manual-volume-1-basic-architecture.html?wapkw=ia32

#include <stdio.h>
#include <string.h>
#include <Windows.h>
#include <winnt.h>


// Stops the compiler complaining about a potential buffer overflow.
#pragma warning(disable : 4996)

void function2()
{
	// 0x00000001400117d0
	puts("This is the stuff which should never run");
}

void function1()
{
	puts("Normal Execution Path");

	char foo[8]; // we ask for space for 8 bytes on the stack, but ...
	// ...space for 70h bytes is actually reserved by the compiler.
	// However only 50h bytes of those are actually used (which is why Src below is 80 bytes long.)
	// This must be for some important alignment related stuff for efficency etc.

	PVOID bufferLocation = foo;

	// The Ret address might be different when you build the source - be prepared to update it.
	// strncpy will copy the source string into the destination buffer but will terminate on a null source byte. Remaining bytes in the destination are filled with nulls.
	//      Buf   Src                                                                                EBP                                RET to function2()                       
	strncpy(foo, "AAAAAAAABBBBBBBBCCCCCCCCDDDDDDDDEEEEEEEEFFFFFFFFGGGGGGGGHHHHHHHHIIIIIIIIJJJJJJJJ" "\x01\x02\x03\x04\x05\x06\x07\x08" "\xd0\x17\x01\x40\x01\x00\x00\x00", 96);

}

struct _TEB {
	NT_TIB NtTib;
};


int main(int argc, char* argv[])
{
	puts("Starting in Main!");

	// Start start and finish addressess (normally 3 x 4k pages).
	PVOID stackBase = NtCurrentTeb()->NtTib.StackBase;
	PVOID stackLimit = NtCurrentTeb()->NtTib.StackLimit;

	function1();

    //STARTUPINFOA si;
	//PROCESS_INFORMATION pi;
	//ZeroMemory(&si, sizeof(si));
	//si.cb = sizeof(si);
	//ZeroMemory(&pi, sizeof(pi));
	//CreateProcessA(NULL, (LPSTR)"powershell.exe", NULL, NULL, false, NULL, NULL, NULL, &si, &pi);
}