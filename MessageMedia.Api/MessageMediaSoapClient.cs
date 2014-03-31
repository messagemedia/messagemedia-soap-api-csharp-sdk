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
            messageType.deliveryReport = true;
            messageType.format = MessageFormatType.SMS;
            messageType.validityPeriod = 1;
            messageType.sequenceNumber = 1;
            messageType.scheduledSpecified = false;
            //messageType.scheduled = DateTime.Now;
            messageType.origin = "This is the origin";

            #region Message Tags
            // Add the tags - if supported by your account type
            //MessageTagType[] tags = new MessageTagType[1];
            //tags[0] = new MessageTagType { name = "My Tag Name", Value = "My Tag Value" };
            //messageType.tags = tags; 
            #endregion
            
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

            result = messageMediaSoapService.sendMessages(authentication, sendMessageBody);
            Debug.WriteLine("Messages sent: {0}", result.sent);
            Debug.WriteLine("Messages failed: {0}", result.failed);
            Debug.WriteLine("Messages scheduled: {0}", result.scheduled);

            if (result.errors == null) return result;

            foreach (var error in result.errors)
            {
                Debug.WriteLine("Error code: {0}", error.code);
                Debug.WriteLine("Error content: {0}", error.content);
                Debug.WriteLine("Error sequence number: {0}", error.sequenceNumber);
                foreach (var recipient in error.recipients)
                {
                    Debug.WriteLine("Error recipient uid: {0}", recipient.uid);
                    Debug.WriteLine("Error recipient value: {0}", recipient.Value);
                }
            }

            return result;
        }

        public CheckRepliesResultType CheckReplies(uint maximumReplies = 100)
        {
            return messageMediaSoapService.checkReplies(authentication, new CheckRepliesBodyType { maximumReplies = maximumReplies, maximumRepliesSpecified = (int)maximumReplies > 0 ? true : false });
        }

        public ConfirmRepliesResultType ConfirmReplies()
        {
            throw new NotImplementedException();
        }

        public CheckReportsResultType CheckReports(uint maximumReports = 100)
        {
            return messageMediaSoapService.checkReports(authentication, new CheckReportsBodyType() { maximumReports = maximumReports, maximumReportsSpecified = (int)maximumReports > 0 ? true : false });
        }

        public ConfirmReportsResultType ConfirmReports(List<uint> listOfReceiptIds)
        {
            ConfirmItemType[] confirmItemType = new ConfirmItemType[listOfReceiptIds.Count];
            int i = 0;
            foreach (var item in listOfReceiptIds)
            {
                confirmItemType[i] = new ConfirmItemType { receiptId = item };
                i++;
            }

            return messageMediaSoapService.confirmReports(authentication, new ConfirmReportsBodyType() { reports = confirmItemType });
        }

        /// <summary>
        /// Confirm a single report.
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        public ConfirmReportsResultType ConfirmReport(uint receiptId)
        {
            ConfirmItemType[] confirmItemType = new ConfirmItemType[1];
            confirmItemType[0] = new ConfirmItemType { receiptId = receiptId };

            return messageMediaSoapService.confirmReports(authentication, new ConfirmReportsBodyType() { reports = confirmItemType });
        }

        public DeleteScheduledMessagesResultType DeleteScheduledMessages()
        {
            throw new NotImplementedException();
        }

        public BlockNumbersResultType GetBlockNumbers()
        {
            throw new NotImplementedException();
        }

        public UnblockNumbersResultType UnblockNumbers()
        {
            throw new NotImplementedException();
        }

        public GetBlockedNumbersResultType GetBlockedNumbers()
        {
            throw new NotImplementedException();
        }
    }
}