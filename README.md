messagemedia-csharp
===================
This sample demonstrates how to call the SOAP Service at https://soap.m4u.com.au/?wsdl using C#.
The solution is built using Visual Studio Professional 2013

Start by looking at the entry point in  https://github.com/messagemedia/messagemedia-csharp/blob/master/MessageMedia.Api.Console/Program.cs

You should start by putting your username and password credentials in the App.config file:

e.g.
	<add key="Username" value="USERNAME" />
	<add key="Password" value="PASSWORD" />
	
It is then recommended that you supply 2 recipients and a Date time in a format such as "2014-08-05 4:45PM" (this will be used for scheduling a message in the future)

e.g.
    <add key="recipient1" value="RECIPIENT 1" />
    <add key="recipient2" value="RECIPIENT 2" />
    <add key="ScheduledDateTime" value="2014-08-07 5:02PM" />
 
and then looking at the methods main calls:
            
CheckUserInfo();
SendMessage();
SendScheduledMessage();
ConstructAndSendBatchMessage();
CheckReplies(); 
CheckReports();

Please note that ConstructAndSendBatchMessage shows the ideal way to send to multiple recipients as it constructs a list of recipient numbers to send to.