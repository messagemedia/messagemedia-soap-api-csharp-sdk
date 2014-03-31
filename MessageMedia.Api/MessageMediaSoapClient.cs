using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMedia.Api
{
    public class MessageMediaSoapClient
    {
        MessageMediaSoapService messageMediaSoapService;
        AuthenticationType authentication;

        public MessageMediaSoapClient(string userId, string password)
        {
            authentication = new AuthenticationType() { userId = userId, password = password };
            messageMediaSoapService = new MessageMediaSoapService();
        }

        public CheckUserResultType GetUserInfo()
        {
            return messageMediaSoapService.checkUser(authentication);
        }
        
        /// <summary>
        /// This example demonstrates how to wrap the complexity associated with packaging up a message into a single method call for sending a single message.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <param name="messageId"></param>
        /// <returns>SendMessagesResultType object.</returns>
        public SendMessagesResultType SendMessage(string from, string to, string message, int messageId)
        {
            // Construct the message
            MessageType messageType = new MessageType();
            messageType.content = message;
            messageType.deliveryReport = false;
            messageType.format = MessageFormatType.SMS;
            messageType.validityPeriod = 1;
            messageType.sequenceNumber = 1;
            messageType.scheduledSpecified = false;
            //messageType.scheduled = DateTime.Now;
            messageType.origin = "This is the origin";

            // Add the tags
            MessageTagType[] tags = new MessageTagType[1];
            tags[0] = new MessageTagType { name = "My Tag Name", Value = "My Tag Value" };
            messageType.tags = tags;
            
            // Add the recipients
            RecipientType[] recipientType = new RecipientType[1];
            recipientType[0] = new RecipientType { uid = (uint)messageId, Value = to};
            messageType.recipients = recipientType;

            // Setup the message type - what is this for?
            MessageType[] messages = new MessageType[1];
            messages[0] = messageType;

            // Setup the send message body
            SendMessagesBodyType sendMessageBody = new SendMessagesBodyType();
            sendMessageBody.messages = new MessageListType();
            sendMessageBody.messages.sendMode = MessageSendModeType.normal;
            sendMessageBody.messages.message = messages;

            return SendMessage(sendMessageBody);
        }

        public SendMessagesResultType SendMessage(SendMessagesBodyType sendMessageBody)
        {
            SendMessagesResultType result;

            try
            {
                result = messageMediaSoapService.sendMessages(authentication, sendMessageBody);
                uint sent = result.sent;
                uint failed = result.failed;

                foreach (var error in result.errors)
                {
                    Debug.WriteLine(error.code);
                    Debug.WriteLine(error.content);
                    Debug.WriteLine(error.sequenceNumber);
                    foreach (var recipient in error.recipients)
                    {
                        Debug.WriteLine(recipient.uid);
                        Debug.WriteLine(recipient.Value);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public CheckRepliesResultType CheckReplies()
        {
            return messageMediaSoapService.checkReplies(authentication, new CheckRepliesBodyType { maximumReplies = 10, maximumRepliesSpecified = true });
        }


    }
}