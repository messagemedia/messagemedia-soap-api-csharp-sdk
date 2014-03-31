using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMedia.Api.Console
{
    class Program
    {
        // Replace the following values wtih your userId and password.
        private static string userId = "MessageUPtyLt892";
        private static string password = "secret";

        // For this example we will use a default from and to number.
        private static string sentFromNumber = "+61412345678";
        private static string sentToNumber = "+61412345678";

        static void Main(string[] args)
        {
            CheckUserInfo();
            SendMessage();
            CheckReplies();
        }

        /// <summary>
        /// Example demonstrates how to quickly send a single message with the default settings.
        /// </summary>
        public static void SendMessage()
        {
            try
            {
                int messageId = 1234567890;

                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                var result = client.SendMessage(sentFromNumber, sentToNumber, "Test Message", messageId);
                DisplaySendMessageResult(result);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Unexpected Error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Example shows how to send a batch of messages. It's possible to send multiple individual messages in a single batch.
        /// </summary>
        public static void SendMultipleMessages()
        {
            try
            {
                // As the sender you are able to define an id that represents the message.
                // TODO: Confirm if this must be unique
                uint messageId = 1234567890;
                
                // Define how many messages you plan to send as this figure will be used to initialise various arrays.
                int totalMessagesBeingSent = 1;

                // Setup the various objects required to send a message.
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);

                // If you are going to be tagging the message then create a MessageTagType object.
                MessageTagType[] tags = new MessageTagType[totalMessagesBeingSent];

                // The RecipientType object is used to store the messageId and destination phone number.
                RecipientType[] recipientType = new RecipientType[totalMessagesBeingSent];

                // Create an array to store all the recipient messages.
                MessageType[] messages = new MessageType[totalMessagesBeingSent];

                // Construct the message
                MessageType message1 = new MessageType();
                message1.content = "Content of Message 1 to Recipient 1";
                message1.deliveryReport = false;
                message1.format = MessageFormatType.SMS;
                message1.validityPeriod = 1;
                message1.sequenceNumber = 1;
                message1.scheduledSpecified = false;
                //messageType.scheduled = DateTime.Now;
                message1.origin = "Origin 1";

                // It is possible to add Tags to an individual message; this might be useful if wanting to identify a particular campaign.
                // The recipient is not able to see the tags.
                // TODO: Confirm limits imposed on tag length and quantity
                tags[0] = new MessageTagType { name = "My Tag Name", Value = "My Tag Value" };
                message1.tags = tags;

                // Add the recipients
                // TODO: Confirm the limits imposed upon recipient quantity
                recipientType[0] = new RecipientType { uid = messageId, Value = sentToNumber };
                message1.recipients = recipientType;

                // Add the message to the messages array.
                messages[0] = message1;

                // The batch of messages are sent using a SendMessagesBodyType object.
                SendMessagesBodyType sendMessageBody = new SendMessagesBodyType();
                // Initiate the messages list so that it is not null.
                sendMessageBody.messages = new MessageListType();
                // Define the send behaviour of the messages.
                // TODO: Define what the different options mean.
                sendMessageBody.messages.sendMode = MessageSendModeType.normal;
                sendMessageBody.messages.message = messages;

                System.Console.WriteLine("Sending {0} messages", totalMessagesBeingSent);

                var result = client.SendMessage(sendMessageBody);

                DisplaySendMessageResult(result);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Unexpected Error: {0}", ex.Message);
            }
        }

        private static void DisplaySendMessageResult(SendMessagesResultType result)
        {
            System.Console.WriteLine("Messages sent: {0}", result.sent);
            System.Console.WriteLine("Messages failed: {0}", result.failed);
            System.Console.WriteLine("Messages scheduled: {0}", result.scheduled);
            System.Console.WriteLine("Errors total: {0}", result.errors.Length);

            foreach (var error in result.errors)
            {
                System.Console.WriteLine("Error code: {0}", error.code);
                System.Console.WriteLine("Error content: {0}", error.content);
                System.Console.WriteLine("Error sequence number: {0}", error.sequenceNumber);
                foreach (var recipient in error.recipients)
                {
                    System.Console.WriteLine("Error recipient: Uid: {0} Value: {1}", recipient.uid, recipient.Value);
                }
            }
        }

        public static void CheckUserInfo()
        {
            try
            {
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                var result = client.GetUserInfo();

                System.Console.WriteLine("Credit limit: {0}", result.accountDetails.creditLimit);
                System.Console.WriteLine("Remaining credit: {0}", result.accountDetails.creditRemaining);

                string type = result.accountDetails.type;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        public static void CheckReplies()
        {
            MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
            var reply = client.CheckReplies();

            System.Console.WriteLine("Remaining credit: {0}", reply.remaining);
            System.Console.WriteLine("Returned: {0}", reply.returned);
            foreach (var item in reply.replies)
            {
                System.Console.WriteLine("Reply receipt id: {0}", item.receiptId);
                System.Console.WriteLine("Reply uid: {0}", item.uid);
                System.Console.WriteLine("Reply received date time: {0}", item.received);
                System.Console.WriteLine("Reply origin: {0}", item.origin);
                System.Console.WriteLine("Reply content: {0}", item.content);
            }

            foreach (var item in reply.replies)
            {
                System.Console.WriteLine(item.content);
                System.Console.WriteLine(item.format);
                System.Console.WriteLine(item.origin);
                System.Console.WriteLine(item.receiptId);
                System.Console.WriteLine(item.received);
                System.Console.WriteLine(item.uid);
            }
        }
    }
}
