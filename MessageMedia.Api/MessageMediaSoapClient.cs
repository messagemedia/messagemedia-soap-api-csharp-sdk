using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMedia.Api
{
    /// <summary>
    /// The MessageMediaSoapClient provides a wrapper to the SOAP API to facilitate simpler interaction.
    /// </summary>
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
        /// <param name="from">From phone number - also known as Origin</param>
        /// <param name="to">To phone number</param>
        /// <param name="message">The content of the message</param>
        /// <param name="messageId">Your message identifier</param>
        /// <returns>SendMessagesResultType object.</returns>
        public SendMessagesResultType SendMessage(string from, string to, string message, uint messageId)
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
            messageType.origin = from;

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

            // Create an array to store all the recipient messages.
            MessageType[] messages = new MessageType[1];
            messages[0] = messageType;

            // Setup the send message body
            SendMessagesBodyType sendMessageBody = new SendMessagesBodyType();
            sendMessageBody.messages = new MessageListType();
            sendMessageBody.messages.sendMode = MessageSendModeType.normal;
            sendMessageBody.messages.message = messages;

            return messageMediaSoapService.sendMessages(authentication, sendMessageBody);
        }

        /// <summary>
        /// This method takes a batch of messages which have been consructed and sends them.
        /// </summary>
        /// <param name="sendMessageBody">The object which contains the batch of messages.</param>
        /// <returns>SendMessagesResultType</returns>
        public SendMessagesResultType SendMessage(SendMessagesBodyType sendMessageBody)
        {
            return messageMediaSoapService.sendMessages(authentication, sendMessageBody);
        }

        public CheckRepliesResultType CheckReplies(uint maximumReplies = 100)
        {
            return messageMediaSoapService.checkReplies(authentication, new CheckRepliesBodyType { maximumReplies = maximumReplies, maximumRepliesSpecified = (uint)maximumReplies > 0 ? true : false });
        }

        /// <summary>
        /// Confirms receipt of a list or reply receipts.
        /// </summary>
        /// <param name="listOfReceiptIds"></param>
        /// <returns></returns>
        public ConfirmRepliesResultType ConfirmReplies(List<uint> listOfReceiptIds)
        {
            // Take the list of receiptId's and prepare them for submission to the SOAP API in the expected format.
            ConfirmItemType[] confirmItemType = new ConfirmItemType[listOfReceiptIds.Count];
            int i = 0;
            foreach (var item in listOfReceiptIds)
            {
                confirmItemType[i] = new ConfirmItemType { receiptId = item };
                i++;
            }

            return messageMediaSoapService.confirmReplies(authentication, new ConfirmRepliesBodyType() { replies = confirmItemType });
        }

        public CheckReportsResultType CheckReports(uint maximumReports = 100)
        {
            return messageMediaSoapService.checkReports(authentication, new CheckReportsBodyType() { maximumReports = maximumReports, maximumReportsSpecified = (uint)maximumReports > 0 ? true : false });
        }

        public ConfirmReportsResultType ConfirmReports(List<uint> listOfReceiptIds)
        {
            // Take the list of receiptId's and prepare them for submission to the SOAP API in the expected format.
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
        /// Confirm a single report. Better to send a batch of receiptIds to the ConfirmReports method which accepts a list of receiptIds.
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        //public ConfirmReportsResultType ConfirmReport(uint receiptId)
        //{
        //    ConfirmItemType[] confirmItemType = new ConfirmItemType[1];
        //    confirmItemType[0] = new ConfirmItemType { receiptId = receiptId };

        //    return messageMediaSoapService.confirmReports(authentication, new ConfirmReportsBodyType() { reports = confirmItemType });
        //}

        public DeleteScheduledMessagesResultType DeleteScheduledMessages(List<uint> listOfMessageIds)
        {
            // Take the list of receiptId's and prepare them for submission to the SOAP API in the expected format.
            MessageIdType[] messageIdType = new MessageIdType[listOfMessageIds.Count];
            int i = 0;
            foreach (var item in listOfMessageIds)
            {
                messageIdType[i] = new MessageIdType { messageId = item };
                i++;
            }

            return messageMediaSoapService.deleteScheduledMessages(authentication, new DeleteScheduledMessagesBodyType() { messages = messageIdType });
        }

        public GetBlockedNumbersResultType GetBlockedNumbers(uint maximumRecipients = 100)
        {
            return messageMediaSoapService.getBlockedNumbers(authentication, new GetBlockedNumbersBodyType() { maximumRecipients = maximumRecipients, maximumRecipientsSpecified = (uint)maximumRecipients > 0 ? true : false });
        }

        public BlockNumbersResultType BlockNumbers(RecipientType[] recipientType)
        {
            return messageMediaSoapService.blockNumbers(authentication, new BlockNumbersBodyType() { recipients = recipientType });
        }

        public UnblockNumbersResultType UnblockNumbers(RecipientType[] recipientType)
        {
            return messageMediaSoapService.unblockNumbers(authentication, new UnblockNumbersBodyType() { recipients = recipientType });
        }
    }
}