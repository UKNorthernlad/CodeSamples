#include <stdio.h>
#include <Windows.h>

void main(int argc, int argv[])
{
	char c0[] = { 'A','B' }; // no terminating null - this will probably print out more that just "AB".
	char c1[] = { 'A','B','\0'};
	char c2[] = "AB";
	char c3[3] = "AB"; // 3rd char for terminating null

	wchar_t c4[] = { 'A','B' }; // no terminating null - this will probably print out more that just "AB".
	wchar_t c5[] = { 'A','B','\0' };
	wchar_t c6[] = L"AB";
	wchar_t c7[3] = L"AB"; // 3rd char for terminating null

	PCWSTR somestring = "Some String\n"; // Pointer to a constant wide string 


	printf("%s %p\n", c0, c0);
	printf("%s %p\n", c1, c1);
	printf("%s %p\n", c2, c2);
	printf("%s %p\n", c3, c3);

	printf(&c0);
	printf(&c1);
	printf(&c2);
	printf(&c3);

	wprintf("%s %p\n", c4, c4);
	wprintf("%s %p\n", c5, c5);
	wprintf("%s %p\n", c6, c6);
	wprintf("%s %p\n", c7, c7);

	wprintf("%s", somestring);
}