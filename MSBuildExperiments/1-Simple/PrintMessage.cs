using System;  

partial class HelloWorld  
{
  static void PrintMessage()  
  {  
    #if DEBUG  
    Console.WriteLine("WE ARE IN THE DEBUG CONFIGURATION");  
    #endif  
    
    Console.WriteLine("Hello from the PrintMessage() function.");  
    }  
}  
