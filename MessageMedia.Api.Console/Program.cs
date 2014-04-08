/*
Copyright 2014 MessageMedia
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
*/
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MessageMedia.Api.Console
{
    class Program
    {
        // Replace the values in the App.config with your userId and password.
        private static string userId = ConfigurationManager.AppSettings.Get("Username");
        private static string password = ConfigurationManager.AppSettings.Get("Password");

        // For this example we will use a default from and to number - enter the values in the App.config
        private static string sentFromNumber = ConfigurationManager.AppSettings.Get("SentFromNumber");
        private static string sentToNumber = ConfigurationManager.AppSettings.Get("SentToNumber");

        static void Main(string[] args)
        {
            CheckUserInfo();
            SendMessage();
            SendMultipleMessages();
            CheckReplies();
            CheckReports();
        }

        #region Sending Messages
        /// <summary>
        /// Example demonstrates how to quickly send a single message with the default settings.
        /// </summary>
        public static void SendMessage()
        {
            System.Console.WriteLine("EXECUTING SEND MESSAGE...");
            try
            {
                uint messageId = 1234567890;

                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                var result = client.SendMessage(sentFromNumber, sentToNumber, "Test Message", messageId);
                DisplaySendMessageResult(result);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Example shows how to send a batch of messages. It's possible to send multiple individual messages in a single batch.
        /// </summary>
        public static void SendMultipleMessages()
        {
            System.Console.WriteLine("EXECUTING SEND MULTIPLE MESSAGES...");
            try
            {
                // Define how many messages you plan to send as this figure will be used to initialise various arrays.
                int totalMessagesBeingSent = 2;

                // Setup the various objects required to send a message.
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);

                // If you are going to be tagging the message then create a MessageTagType object.
                MessageTagType[] tags = new MessageTagType[totalMessagesBeingSent];

                // The RecipientType object is used to store the messageId and destination phone number.
                RecipientType[] recipientType = new RecipientType[totalMessagesBeingSent];

                // Create an array to store all the recipient messages.
                MessageType[] messages = new MessageType[totalMessagesBeingSent];

                #region Construct Message 1
                // Construct the message
                MessageType message1 = new MessageType();
                message1.content = "Content of Message 1 to Recipient 1";
                message1.deliveryReport = false;
                message1.format = MessageFormatType.SMS;
                message1.validityPeriod = 1;
                // (Optional) This attribute specifies a sequence number that is assigned to the message and is used to identify the message if an error occurs. Each message error in the response will specify the sequence number of the message that caused the error. Sequence numbers should be unique within the request. 1 to 2147483647.
                message1.sequenceNumber = 1;
                message1.scheduledSpecified = false;
                message1.scheduled = DateTime.Now;
                // (Optional) This element specifies the message source address. The specified address will be used wherever possible, however due to limitations with various carriers, legislation etc, the final message is not guaranteed to come from the specified address.
                message1.origin = "Origin_1";

                // It is possible to add Tags to an individual message; this might be useful if wanting to identify a particular campaign or cost centre.
                #region Message Tags
                // Add the tags - if supported by your account type
                //MessageTagType[] tags = new MessageTagType[1];
                //tags[0] = new MessageTagType { name = "My Tag Name", Value = "My Tag Value" };
                //message1.tags = tags; 
                #endregion

                // (Optional) This attribute specifies a user-defined unique ID that is assigned to a message-recipient pair. The uid is an unsigned integer that uniquely identifies a message sent to a particular recipient.
                // uid values are used for three things: to identify a message-recipient in the case of an error; to match a reply message to the sent message it is in response to; and to match a delivery report to the sent message it is in response to.
                // If no uid value is specified a default value of zero is assigned.
                uint message1Id = 1234567890;

                // Add the recipients
                // TODO: Confirm the limits imposed upon recipient quantity
                recipientType[0] = new RecipientType { uid = message1Id, Value = sentToNumber };
                message1.recipients = recipientType;
                #endregion

                #region Construct Message 2
                // Construct the message
                MessageType message2 = new MessageType();
                message2.content = "Content of Message 2 to Recipient 2";
                message2.deliveryReport = false;
                message2.format = MessageFormatType.SMS;
                message2.validityPeriod = 1;
                // (Optional) This attribute specifies a sequence number that is assigned to the message and is used to identify the message if an error occurs. Each message error in the response will specify the sequence number of the message that caused the error. Sequence numbers should be unique within the request. 1 to 2147483647.
                message2.sequenceNumber = 1;
                message2.scheduledSpecified = false;
                message2.scheduled = DateTime.Now;
                // (Optional) This element specifies the message source address. The specified address will be used wherever possible, however due to limitations with various carriers, legislation etc, the final message is not guaranteed to come from the specified address.
                message2.origin = "Origin_2";

                // It is possible to add Tags to an individual message; this might be useful if wanting to identify a particular campaign or cost centre.
                #region Message Tags
                // Add the tags - if supported by your account type
                //MessageTagType[] tags = new MessageTagType[1];
                //tags[0] = new MessageTagType { name = "My Tag Name", Value = "My Tag Value" };
                //message2.tags = tags; 
                #endregion

                // (Optional) This attribute specifies a user-defined unique ID that is assigned to a message-recipient pair. The uid is an unsigned integer that uniquely identifies a message sent to a particular recipient.
                // uid values are used for three things: to identify a message-recipient in the case of an error; to match a reply message to the sent message it is in response to; and to match a delivery report to the sent message it is in response to.
                // If no uid value is specified a default value of zero is assigned.
                uint message2Id = 0987654321;

                // Add the recipients
                // TODO: Confirm the limits imposed upon recipient quantity
                recipientType[0] = new RecipientType { uid = message2Id, Value = sentToNumber };
                message2.recipients = recipientType;
                #endregion

                // Add the message to the messages array.
                messages[0] = message1;
                messages[1] = message2;

                // The batch of messages are sent using a SendMessagesBodyType object.
                SendMessagesBodyType sendMessageBody = new SendMessagesBodyType();
                // Initiate the messages list so that it is not null.
                sendMessageBody.messages = new MessageListType();
                // Define the send behaviour of the messages.             
                // "dropAll" – to drop (not send) the requested messages, and return a result indicating that messages were sent / scheduled successfully or failed to send at random.
                // "dropAllWithErrors" – to drop (not send) the requested messages, and return a result indicating that all messages failed to send.
                // "dropAllWithSuccess" – to drop (not send) the requested messages, but return a result indicating all messages were sent / scheduled successfully.
                // "normal" – to send the requested messages as normal.
                sendMessageBody.messages.sendMode = MessageSendModeType.normal;
                // Attach the messages
                sendMessageBody.messages.message = messages;

                System.Console.WriteLine("Sending {0} message(s)", totalMessagesBeingSent);

                var result = client.SendMessage(sendMessageBody);

                DisplaySendMessageResult(result);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Private method used for rendering the results to the console screen.
        /// </summary>
        /// <param name="result"></param>
        private static void DisplaySendMessageResult(SendMessagesResultType result)
        {
            System.Console.WriteLine("Messages sent: {0}", result.sent);
            System.Console.WriteLine("Messages failed: {0}", result.failed);
            System.Console.WriteLine("Messages scheduled: {0}", result.scheduled);

            if (result.errors == null) return;

            System.Console.WriteLine("Errors total: {0}", result.errors.Length);

            foreach (var error in result.errors)
            {
                System.Console.WriteLine("Error code: {0}", error.code);
                System.Console.WriteLine("Error content: {0}", error.content);
                System.Console.WriteLine("Error sequence number: {0}", error.sequenceNumber);

                if (error.recipients != null)
                {
                    foreach (var recipient in error.recipients)
                    {
                        System.Console.WriteLine("Error recipient: Uid: {0} Value: {1}", recipient.uid, recipient.Value);
                    }
                }
            }
        } 
        #endregion

        #region Account & User Info
        /// <summary>
        /// Example demonstrates how to get account and credit remaining information.
        /// </summary>
        public static void CheckUserInfo()
        {
            System.Console.WriteLine("EXECUTING CHECK USER INFO...");
            try
            {
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                var result = client.GetUserInfo();

                System.Console.WriteLine("Credit limit: {0}", result.accountDetails.creditLimit);
                System.Console.WriteLine("Remaining credit: {0}", result.accountDetails.creditRemaining);
                System.Console.WriteLine("Account type: {0}", result.accountDetails.type);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        } 
        #endregion

        #region Replies
        /// <summary>
        /// Example demonstrates how to fetch replies.
        /// </summary>
        /// <remarks>You must then confirm receipt of each reply using the ConfirmReplies method.</remarks>
        public static void CheckReplies()
        {
            System.Console.WriteLine("EXECUTING CHECK REPLIES...");
            try
            {
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                var reply = client.CheckReplies();

                System.Console.WriteLine("Remaining replies: {0}", reply.remaining);
                System.Console.WriteLine("Replies returned: {0}", reply.returned);

                if (reply.replies == null) return;

                // Create a list to hold the receipts of the replies you want to confirm you have received.
                List<uint> listOfReceiptIds = new List<uint>();

                foreach (var item in reply.replies)
                {
                    System.Console.WriteLine("Reply receipt id: {0}", item.receiptId);
                    System.Console.WriteLine("Reply uid: {0}", item.uid);
                    System.Console.WriteLine("Reply received date time: {0}", item.received);
                    System.Console.WriteLine("Reply origin: {0}", item.origin);
                    System.Console.WriteLine("Reply content: {0}", item.content);
                    System.Console.WriteLine("Reply format: {0}", item.format);

                    listOfReceiptIds.Add(item.receiptId);
                }

                // Confirm the receipt of each reply
                if (listOfReceiptIds.Count > 0) ConfirmReplies(listOfReceiptIds);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Example demonstrates how to confirm receipt of a list of replies.
        /// </summary>
        /// <param name="listOfReceiptIds">List of receiptId's</param>
        private static void ConfirmReplies(List<uint> listOfReceiptIds)
        {
            System.Console.WriteLine("EXECUTING CONFIRM REPLIES....");
            if (listOfReceiptIds.Count == 0) return;
            try
            {
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                var reply = client.ConfirmReplies(listOfReceiptIds);

                System.Console.WriteLine("Replies confirmed: {0}", reply.confirmed);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        } 
        #endregion

        #region Reports
        /// <summary>
        /// Example demonstrates how to fetch reports on delivery status.
        /// </summary>
        /// <remarks>You must then confirm receipt of each report using the ConfirmReports method.</remarks>
        public static void CheckReports()
        {
            System.Console.WriteLine("EXECUTING CHECK REPORTS...");
            try
            {
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                var reply = client.CheckReports();

                System.Console.WriteLine("Remaining replies: {0}", reply.remaining);
                System.Console.WriteLine("Replies returned: {0}", reply.returned);

                if (reply.reports == null) return;

                // Create a list to hold the receipts of the reports you want to confirm you have received.
                List<uint> listOfReceiptIds = new List<uint>();

                foreach (var item in reply.reports)
                {
                    System.Console.WriteLine("Reply receipt id: {0}", item.receiptId);
                    System.Console.WriteLine("Reply uid: {0}", item.uid);
                    System.Console.WriteLine("Reply received date time: {0}", item.timestamp);
                    System.Console.WriteLine("Reply status: {0}", item.status);
                    System.Console.WriteLine("Reply recipient: {0}", item.recipient);

                    // Add the receipt of the report for each delivered message (or for every status if you choose)
                    if (item.status == DeliveryStatusType.delivered)
                    {
                        listOfReceiptIds.Add(item.receiptId);
                    }
                }

                // Confirm the receipt of each report
                if (listOfReceiptIds.Count > 0) ConfirmReports(listOfReceiptIds);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Example demonstrates how to confirm receipt of a list of reports.
        /// </summary>
        /// <param name="listOfReceiptIds">List of receiptId's</param>
        private static void ConfirmReports(List<uint> listOfReceiptIds)
        {
            System.Console.WriteLine("EXECUTING CONFIRM REPORTS....");
            if (listOfReceiptIds.Count == 0) return;
            try
            {
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                var reply = client.ConfirmReports(listOfReceiptIds);

                System.Console.WriteLine("Reports confirmed: {0}", reply.confirmed);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        } 
        #endregion

        #region Scheduled Messages
        /// <summary>
        /// Delete a selection of scheduled messages. You will need to store which messageId's have been scheduled when you originally submit them.
        /// </summary>
        /// <param name="listOfMessageIds"></param>
        public static void DeleteScheduledMessages(List<uint> listOfMessageIds)
        {
            System.Console.WriteLine("EXECUTING DELETE SCHEDULED MESSAGES....");

            try
            {
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                var reply = client.DeleteScheduledMessages(listOfMessageIds);

                System.Console.WriteLine("Messages unscheduled: {0}", reply.unscheduled);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        } 
        #endregion

        #region Block & Unblock Numbers
        /// <summary>
        /// Example shows how to block numbers.
        /// </summary>
        public static void BlockNumbers()
        {
            System.Console.WriteLine("EXECUTING BLOCK NUMBERS....");

            try
            {
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);

                RecipientType[] recipientType = new RecipientType[2];
                recipientType[0] = new RecipientType() { uid = 1, Value = "+614123456789" };
                recipientType[1] = new RecipientType() { uid = 2, Value = "+614987654321" };

                var reply = client.BlockNumbers(recipientType);

                System.Console.WriteLine("Numbers blocked: {0}", reply.blocked);

                if (reply.errors == null) return;

                System.Console.WriteLine("Errors total: {0}", reply.errors.Length);

                foreach (var error in reply.errors)
                {
                    System.Console.WriteLine("Error code: {0}", error.code);
                    System.Console.WriteLine("Error content: {0}", error.content);
                    System.Console.WriteLine("Error sequence number: {0}", error.sequenceNumber);

                    if (error.recipients != null)
                    {
                        foreach (var recipient in error.recipients)
                        {
                            System.Console.WriteLine("Error recipient: Uid: {0} Value: {1}", recipient.uid, recipient.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        public static void UnblockNumbers()
        {
            System.Console.WriteLine("EXECUTING UNBLOCK NUMBERS....");

            try
            {
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);

                RecipientType[] recipientType = new RecipientType[2];
                recipientType[0] = new RecipientType() { uid = 1, Value = "+614123456789" };
                recipientType[1] = new RecipientType() { uid = 2, Value = "+614987654321" };

                var reply = client.UnblockNumbers(recipientType);

                System.Console.WriteLine("Numbers unblocked: {0}", reply.unblocked);

                if (reply.errors == null) return;

                System.Console.WriteLine("Errors total: {0}", reply.errors.Length);

                foreach (var error in reply.errors)
                {
                    System.Console.WriteLine("Error code: {0}", error.code);
                    System.Console.WriteLine("Error content: {0}", error.content);
                    System.Console.WriteLine("Error sequence number: {0}", error.sequenceNumber);

                    if (error.recipients != null)
                    {
                        foreach (var recipient in error.recipients)
                        {
                            System.Console.WriteLine("Error recipient: Uid: {0} Value: {1}", recipient.uid, recipient.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        } 
        #endregion
    }
}
