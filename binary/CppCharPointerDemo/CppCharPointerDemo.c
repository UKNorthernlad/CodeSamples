#include <stdio.h>

void printLocalArray()
{
    char letter = 'A';
    char letters[] = "AAAAAAAAAAA";

    printf("Letter  is --> %c \n", letter);
    printf("Letters is --> %s \n\n", letters);

    int* pLetter = letter;
    int* pLetters = letters;

    printf("Letter value (decimal)  = %u \n", letter);
    printf("Letter value (hex)      = %x \n", letter);
    printf("Letter memory location  = %p \n", &letter);
    printf("Letters memory location = %p \n\n", letters);
}

void printLocalPointer()
{
    char* letter = "AB";
    printf("Message is              = %s \n", letter);
    printf("Message memory location = %p \n\n", letter);

    letter = letter + 252;
    printf("Message is              = %s \n", letter);
    printf("Message memory location = %p \n\n", letter);
}
int main()
{
    printLocalArray();
    printLocalPointer();
}

