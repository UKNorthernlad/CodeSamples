using System;  

partial class HelloWorld  
{
  static void Main()  
  {  
    #if DEBUG  
    Console.WriteLine("WE ARE IN THE DEBUG CONFIGURATION");  
    #endif  
    
    Console.WriteLine("Hello from the Main() function.");

    PrintMessage();  
    }  
}  
