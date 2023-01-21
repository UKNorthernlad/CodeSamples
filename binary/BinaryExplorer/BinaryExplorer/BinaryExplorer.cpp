// https://docs.microsoft.com/en-us/windows/win32/debug/pe-format
// http://bytepointer.com/resources/pietrek_peering_inside_pe.htm
// https://docs.microsoft.com/en-us/archive/msdn-magazine/2002/february/inside-windows-win32-portable-executable-file-format-in-detail
// https://docs.microsoft.com/en-us/archive/msdn-magazine/2002/march/inside-windows-an-in-depth-look-into-the-win32-portable-executable-file-format-part-2
// https://docs.microsoft.com/en-us/archive/msdn-magazine/2002/march/windows-2000-loader-what-goes-on-inside-windows-2000-solving-the-mysteries-of-the-loader
// https://resources.infosecinstitute.com/topic/the-import-directory-part-1/
// https://resources.infosecinstitute.com/topic/the-import-directory-part-2/
// https://tech-zealots.com/malware-analysis/journey-towards-import-address-table-of-an-executable-file/
// https://tech-zealots.com/malware-analysis/pe-portable-executable-structure-malware-analysis-part-2/

#include <stdio.h>
#include <errno.h>
#include <windows.h>
#include <winnt.h>

#define BUFF 102400

#pragma warning(disable : 4996)

int main()
{
    FILE* file;
    //char headerBuffer[BUFF];
    void* headerBuffer = malloc(BUFF);

    file = fopen("..\\..\\compiletest\\main.exe", "rb");
    if (file == NULL) {
        perror("Can't open file.");
        return 1;
    }

    if (headerBuffer != NULL)
    {
        int bytesRead = fread(headerBuffer, 1, BUFF, file);
    }
    else
    {
        perror("Memory allocation failure.");
        return 1;
    }

    // DOS Header
    PIMAGE_DOS_HEADER pDOSHeader = (PIMAGE_DOS_HEADER)((PBYTE)headerBuffer);
    
    // PE Header
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format
    PIMAGE_NT_HEADERS32 pNTHeader32 = (PIMAGE_NT_HEADERS32)((PBYTE)pDOSHeader + pDOSHeader->e_lfanew);
    
    // COFF Header File aka NT File Header
    // https://docs.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-image_file_header
    PIMAGE_FILE_HEADER NTFileHeader = (PIMAGE_FILE_HEADER)(&pNTHeader32->FileHeader);

    if (NTFileHeader->Machine == IMAGE_FILE_MACHINE_I386)
    {
        printf("CPU Type = Intel 386 or later processors and compatible processors\n");
    }

    printf("Number of Sections = %u\n", NTFileHeader->NumberOfSections);

    printf("Size of Optional Header = %u\n", NTFileHeader->SizeOfOptionalHeader);

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#optional-header-standard-fields-image-only
    PIMAGE_OPTIONAL_HEADER32 NTOptionalFileHeader = (PIMAGE_OPTIONAL_HEADER32)(&pNTHeader32->OptionalHeader);

    switch (NTOptionalFileHeader->Magic)
    {
    case IMAGE_NT_OPTIONAL_HDR32_MAGIC:
        printf("This is PE32 file.\n");
        break;
    case IMAGE_NT_OPTIONAL_HDR64_MAGIC:
        printf("This is PE32+ (64bit) file.\n");
        break;
    default:
        break;
    }

    DWORD ImageBase = NTOptionalFileHeader->ImageBase;
    DWORD AddressOfEntryPoint = NTOptionalFileHeader->AddressOfEntryPoint;
    DWORD inMemoryEntryPoint = ImageBase + AddressOfEntryPoint;
  
    printf("Execution Entry Point 0x%x\n", AddressOfEntryPoint);
    printf("Load Base Address 0x%x\n", ImageBase);
    printf("inMemoryEntryPoint Address 0x%x\n", inMemoryEntryPoint);

    printf("Sections:\n");

    PIMAGE_SECTION_HEADER section = (PIMAGE_SECTION_HEADER)(pNTHeader32 + 1);
    
    for (size_t i = 0; i < NTFileHeader->NumberOfSections; i++, section++)
    {  
        BYTE* name = section->Name;
        DWORD sectionInMemoryAddress = section->VirtualAddress;
        DWORD sectionInBinaryAddress = section->PointerToRawData;
        DWORD sizeOfSection = section->SizeOfRawData;

        printf("   %s\n", name);
        printf("       Location in binary: 0x%x\n", sectionInBinaryAddress); // location of data in binary
        printf("       Location in memory: 0x%x\n", sectionInMemoryAddress); // location of data once loaded into memory
        printf("       Size of section: %u\n", sizeOfSection);

        if (!strcmp((char*)section->Name, ".idata"))
        {
            PIMAGE_DATA_DIRECTORY pImportTable = &NTOptionalFileHeader->DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT];
            DWORD firstImportDescriptorStructInMemory = pImportTable->VirtualAddress;

            printf("       first DLL Image Import Descriptor Struct in memory: 0x%x\n", firstImportDescriptorStructInMemory); // location of data in binary
 
            DWORD firstImportDescriptorOffset = (firstImportDescriptorStructInMemory - sectionInMemoryAddress);
            
            
            PIMAGE_IMPORT_DESCRIPTOR firstImportDescriptorStructInBinary = (PIMAGE_IMPORT_DESCRIPTOR)(sectionInBinaryAddress + firstImportDescriptorOffset);
            DWORD location = (DWORD)firstImportDescriptorStructInBinary;

            printf("       first DLL Image Import Descriptor Struct in binary: 0x%x\n", location);
 
            PIMAGE_IMPORT_DESCRIPTOR importDescriptor = (PIMAGE_IMPORT_DESCRIPTOR)(((BYTE*)headerBuffer + location));
            
            printf("\n");
            while (importDescriptor->OriginalFirstThunk != 0)
            {
                DWORD originalFirstThunk = importDescriptor->OriginalFirstThunk; // Holds the RVA of the Import Lookup table.
                //DWORD originalFirstThunkOffSet = originalFirstThunk - sectionInMemoryAddress + sectionInBinaryAddress;
                //PIMAGE_THUNK_DATA thunkData = (PIMAGE_THUNK_DATA)((PBYTE)headerBuffer + originalFirstThunkOffSet);
                
                DWORD name = importDescriptor->Name; // RVA to a string which is the name of the DLL to import.
                DWORD dllNameOffSet = name - sectionInMemoryAddress + sectionInBinaryAddress;
                PCHAR dllName = (PCHAR)(((BYTE*)headerBuffer + dllNameOffSet));

                //DWORD firstThunk = importDescriptor->FirstThunk; 

                printf("         Name: 0x%x (%s)\n", name, dllName);
                printf("         OriginalFirstThunk: 0x%x\n", originalFirstThunk);



                printf("\n");
                importDescriptor++;
            }
        }
    }     
}


