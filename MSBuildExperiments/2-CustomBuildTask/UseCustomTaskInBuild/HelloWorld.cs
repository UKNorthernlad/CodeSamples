using System;  

class HelloWorld  
{
  static void Main()  
  {  
    #if DEBUG  
    Console.WriteLine("WE ARE IN THE DEBUG CONFIGURATION");  
    #endif  
    
    Console.WriteLine("Hello, world!");  
    }  
}  
