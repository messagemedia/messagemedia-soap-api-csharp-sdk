messagemedia-csharp
===================
This sample demonstrates how to call the SOAP Service at https://soap.m4u.com.au/?wsdl using C#.
The solution is built using Visual Studio 2013

Send a message with the following lines of code:

```
try
{
    MessageMediaSoapClient client = new MessageMediaSoapClient("your-userid", "your-password");
    var result = client.SendMessage("+61412345678", "+61987654321", "Test Message", 1234567890);
    System.Console.WriteLine(result.sent);
}
catch (Exception ex)
{
    System.Console.WriteLine("Error: {0}", ex.Message);
}
```

Start by looking at the entry point in  https://github.com/messagemedia/messagemedia-csharp/blob/master/MessageMedia.Api.Console/Program.cs
