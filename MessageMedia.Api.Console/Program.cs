/*
Copyright 2014 MessageMedia
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License. 
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
        // Replace the values in the App.config
        private static string userId = ConfigurationManager.AppSettings.Get("Username");
        private static string password = ConfigurationManager.AppSettings.Get("Password");
        private static string recipient1 = ConfigurationManager.AppSettings.Get("Recipient1");
        private static string recipient2 = ConfigurationManager.AppSettings.Get("Recipient2");
        // Make sure date in your app.config file is in the future and of the format e.g. 2014-08-07 4:00PM
        private static string scheduledTimeString = ConfigurationManager.AppSettings.Get("ScheduledDateTime");

        static void Main(string[] args)
        {
            
            // Note: These are examples only, uncomment the method you'd like to test.
            CheckUserInfo();
            //SendMessage();
            //SendScheduledMessage();
            //ConstructAndSendBatchMessage();
            CheckReplies();
            CheckReports();

            // Halt console program
            System.Console.WriteLine("\nHit any key to continue");
            System.Console.ReadLine();

        }

        #region Account & User Info
        /// <summary>
        /// Example demonstrates how to get account and credit remaining information.
        /// </summary>
        public static void CheckUserInfo()
        {
            System.Console.WriteLine("--EXECUTING CheckUserInfo()...");
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

        #region SendMessage
        /// <summary>
        /// Example demonstrates how to quickly send a single message with the default settings.
        /// </summary>
        public static void SendMessage()
        {
            System.Console.WriteLine("EXECUTING SendMessage()\n Sending to Recipient 1.");
            try
            {       
                uint messageId = 123456789;
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);
                string message = "SendMessage executed.\n";
                // The first parameter specifies a sentFromNumber - this needs to be configured for your account. Speak to MessageMedia support for more details. 
                var result = client.SendMessage("", recipient1, message, messageId);
                DisplaySendMessageResult(result);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        }
        #endregion

        #region SendScheduledMessage
        /// <summary>
        /// Example demonstrates how to quickly send a single message with the default settings and a scheduled time in the future to recipient 1
        /// </summary>
        public static void SendScheduledMessage()
        {
            System.Console.WriteLine("EXECUTING SendScheduledMessage()\nSending a single message to recipient 1 at a scheduled time in the future.\n");
            try
            {
                uint messageId = 123456789;
                string message = "SendScheduledMessage executed.\n";

                // Check whether the date time in App.config is valid otherwise put in today's time
                System.Console.WriteLine("Scheduling Message");
                DateTime messageScheduledDateTime;
                if (!DateTime.TryParse(scheduledTimeString, out messageScheduledDateTime))
                {
                    messageScheduledDateTime = DateTime.Now;
                }
                
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);

                var result = client.SendScheduledMessage("", recipient1, message, messageId, messageScheduledDateTime);
                DisplaySendMessageResult(result);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        }
        #endregion

        #region ConstructAndSendBatchMessage
        /// <summary>
        /// Example shows how to construct messages using MessageType and how to send to multiple recipients
        /// </summary>
        public static void ConstructAndSendBatchMessage()
        {
            System.Console.WriteLine("EXECUTING ConstructAndSendBatchMessage()\nConstruct a message and send to multiple recipients.");
            try
            {
                // Define how many messages you plan to send as this figure will be used to initialise various arrays.
                int totalMessagesBeingSent = 1;
                      
                // Setup the various objects required to send a message.
                MessageMediaSoapClient client = new MessageMediaSoapClient(userId, password);

                // Create an array to store all the recipient messages.
                MessageType[] messages = new MessageType[totalMessagesBeingSent];
                      
                MessageType message = new MessageType();

                #region Message Properties

                // (Optional) This attribute specifies a user-defined unique ID that is assigned to a message-recipient pair. The uid is an unsigned integer that uniquely identifies a message sent to a particular recipient.
                // uid values are used for three things: to identify a message-recipient in the case of an error; to match a reply message to the sent message it is in response to; and to match a delivery report to the sent message it is in response to.
                // If no uid value is specified a default value of zero is assigned.
                uint messageId = 123456789;
                // Note: This needs to be enabled for your account in order to send from a dedicated numberplease contact MessageMedia Support for details
                // (Optional) This element specifies the message source address. The specified address will be used wherever possible, however due to limitations with various carriers, legislation etc, the final message is not guaranteed to come from the specified address.
                message.origin = "";
                // Delivery reports when requested (TRUE) will allow customers to see if a message has been delivered. 
                // Please note you can view the report within our MessageMedia Manager interface and requesting a deliveryReport incurs additional fees per message. 
                message.deliveryReport = false;
                message.format = MessageFormatType.SMS;
                message.validityPeriod = 1;
                // (Optional) This attribute specifies a sequence number that is assigned to the message and is used to identify the message if an error occurs. Each message error in the response will specify the sequence number of the message that caused the error. Sequence numbers should be unique within the request. 1 to 2147483647.
                message.sequenceNumber = 1;
                message.content = "ConstructAndSendBatchMessage executed.\n";

                #endregion
                
                #region Add Message Recipients
                // Add the recipients
                // The RecipientType object is used to store the messageId and destination phone number.
                System.Console.Write("Sending to Recipient 1 and Recipient 2\n");
                int numRecipients = 2;
                RecipientType[] messageRecipientList = new RecipientType[numRecipients];
                messageRecipientList[0] = new RecipientType { uid = messageId, Value = recipient1 };
                messageRecipientList[1] = new RecipientType { uid = messageId, Value = recipient2 };
                message.recipients = messageRecipientList;

                #endregion
                
                #region Message Tags
                // It is possible to add Tags to an individual message; this might be useful if wanting to identify a particular campaign or cost centre.
                // Note: This needs to be enabled for the account, contact MessageMedia Support for more details

                // Add the tags - if supported by your account type
                //MessageTagType[] tags = new MessageTagType[1];
                //tags[0] = new MessageTagType { name = "My Tag Name", Value = "My Tag Value" };
                //message.tags = tags; 
                #endregion

                #region Send Constructed Message and Display
                // Add the message to the messages array.
                messages[0] = message;
           
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
                #endregion

                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
        }
        #endregion

        #region DisplaySendMessageResult
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

        #region Replies
        /// <summary>
        /// Example demonstrates how to fetch replies.
        /// </summary>
        /// <remarks>You must then confirm receipt of each reply using the ConfirmReplies method.</remarks>
        public static void CheckReplies()
        {
            System.Console.WriteLine("\n\nEXECUTING CHECK REPLIES...");
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

        #region Delete Scheduled Messages
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