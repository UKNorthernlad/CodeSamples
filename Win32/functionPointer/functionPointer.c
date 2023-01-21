#include <stdio.h>
#include <windows.h>

// This is the typedef for a function pointer.
// The function it points to should take 4 parameters and return an int.
// The new type will be called MYPROC.
typedef int(__cdecl* MYPROC)(HWND, LPCSTR, LPCSTR, UINT);

int main()
{
    // You could just call a function directly.
    // This is located in user32.dll which linked against by default - no need to add specific dependency.
    MessageBoxA(NULL, "text", "caption", MB_OK);

    // You could also dynamically load the DLL......
    MYPROC proc;
    BOOL success, result = FALSE;
    LPCSTR lpLibFileName = "User32.dll";
    HMODULE h = LoadLibraryA(lpLibFileName);

    if (h != NULL)
    {
        // .... and locate the function by name.
        // proc will contain the memory address of the loaded function.
        // Its is what's known as a function pointer - like a Delegate in .Net
        proc = (MYPROC)GetProcAddress(h, "MessageBoxA");

        // If the function address is valid, call the function.

        if (NULL != proc)
        {
            success = TRUE;
            LPCSTR caption = "Caption\0";
            LPCSTR message = "Message\0";
            // Call the function via a function pointer.
            (proc)(NULL, message, caption, MB_OK);
        }
        // Free the DLL module.

        result = FreeLibrary(h);
    }
}