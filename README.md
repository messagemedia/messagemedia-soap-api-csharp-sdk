# MessageMedia C# SDK
This library provides a simple interface for sending and receiving messages using the [MessageMedia SOAP API](http://www.messagemedia.com.au/wp-content/uploads/2013/05/MessageMedia_Messaging_Web_Service.pdf?eacfbb).

If you have any issue using this sample code, or would like to report a defect, you could [create a new Issue](https://github.com/messagemedia/messagemedia-csharp/issues/new) in Github or [Contact us](http://www.messagemedia.com.au/contact-us).

## Installation
This sample demonstrates how to call the SOAP Service at https://soap.m4u.com.au/?wsdl using C#.

The solution is built using Visual Studio Professional 2013.

##Usage
Start by looking at the entry point in  
```
https://github.com/messagemedia/messagemedia-csharp/blob/master/MessageMedia.Api.Console/Program.cs
```

You should start by putting your username and password credentials in the App.config file:

```
	<add key="Username" value="USERNAME" />
	<add key="Password" value="PASSWORD" />
```
	
It is then recommended that you supply 2 recipients and a Date time in a format such as "2014-08-05 4:45PM" (this will be used for scheduling a message in the future):

```
    <add key="recipient1" value="RECIPIENT 1" />
    <add key="recipient2" value="RECIPIENT 2" />
    <add key="ScheduledDateTime" value="2014-08-07 5:02PM" />
```

Other available method calls:
         
* CheckUserInfo();
* SendMessage();
* SendScheduledMessage();
* ConstructAndSendBatchMessage();
* CheckReplies(); 
* CheckReports();


Please note that ConstructAndSendBatchMessage shows the ideal way to send to multiple recipients as it constructs a list of recipient numbers to send to.

## Contributing
We welcome contributions from our users. Contributing is easy:

  1.  Fork this repo
  2.  Create your feature branch (`git checkout -b my-new-feature`)
  3.  Commit your changes (`git commit -am 'Add some feature'`)
  4.  Push to the branch (`git push origin my-new-feature`)
  5.  Create a Pull Request