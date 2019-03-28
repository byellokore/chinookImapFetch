using System;

using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MimeKit;
using System.Text.RegularExpressions;
using System.Linq;


namespace chinookImapFetch
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (var client = new ImapClient())
            {
                // Basic IMAP connection
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("imap.gmail.com", 993, true);

                client.Authenticate("anothertest201802@gmail.com", "Chinook@8");

                // Access email Inbox
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                Console.WriteLine("Total messages: {0}", inbox.Count);

                // Regex to match Sales from 6000 to 7999
                Regex rgxSubject = new Regex(@"\b(6[0-9][0-9][0-9]|7[0-9][0-9][0-9])\b");

                /* Carrier BoL Regex:
                 * COSCO        => ((CCLU|CSLU|COSU|CBHU)\d{10})
                 * Evergreen    => (EGLV\d{12})
                 * OOCL         => (OOLU\d{10})
                 * Matson       => (MATS\d{10})
                 * Maersk       => (MAEU\d{9})
                 * K-Line       => (KKLUAA(A|1)\d{6})
                 * MSC          => (MSCUA(A|1)\d{6})
                 * Hapag-Lloyd  => (HLCUAA(A|1)\d{9})
                 * CMA CGM      => ((CMAU|CMDU)AAA\d{7})
                 * Hyundai      => (HMDUAAAA\d{7})|(HDMU\w{4}\d{7})|(QSWB\d{7})
                 * NYK          => (NYKSAAAA\d{8})
                 * Yang Ming    => (YMLU(E|B|T|W)\d{9})
                 * UASC Lines   => (UASUAAAAA\d{6})
                 * ZIM          => ((ZIMU|SSPH)AAA\d{4,7})
                 * MOL          => (MOLU\d{11}A)
                 * Hamburg Süd  => (SUDU\d{5}\w{7})
                 * APL          => ((APLU0|APLU)\d{8})
                 * SM Lines     => (SMLM\w{4}\d{8})
                 * Turkon Lines => (TRKU\w{6}\d{6})\b")
                 */

                // Regex to Match BoL
                Regex rgxLading = new Regex(@"\b(QSWB\d{7})|(TRKU\w{6}\d{6})|(SMLM\w{4}\d{8})|(HDMU\w{4}\d{7})|((APLU0|APLU)\d{8})|(SUDU\d{5}\w{7})|(MOLU\d{11}A)|((ZIMU|SSPH)AAA\d{4,7})|(UASUAAAAA\d{6})|(YMLU(E|B|T|W)\d{9})|(NYKSAAAA\d{8})|(HMDUAAAA\d{7})|((CMAU|CMDU)AAA\d{7})|(HLCUAA(A|1)\d{9})|(MSCUA(A|1)\d{6})|(MATS\d{10})|(MAEU\d{9})|(KKLUAA(A|1)\d{6})|(EGLV\d{12})|(OOLU\d{10})|((CCLU|CSLU|COSU|CBHU)\d{10})\b");



                //Regex to Match Container, will also check the false positive match of QSWB BoL that has the same 4 characters and seven numbers of a Container.
                Regex rgxContainer = new Regex(@"\b(?!QSWB)(\w{4}\d{7})\b");

                var sales = new SalesOrderList();

               

                //Fetch Subjects to save time and bandwidth, before we match the subject we don't really need the full message download.
                foreach (var summary in inbox.Fetch(0, inbox.Count, MessageSummaryItems.Full | MessageSummaryItems.UniqueId))
                {
                    //will stop if message has no subject - TO DO
                    if (rgxSubject.IsMatch(summary.Envelope.Subject))
                    {
                        var textBody = (TextPart)inbox.GetBodyPart(summary.UniqueId, summary.TextBody);

                        if(rgxLading.IsMatch(textBody.Text))
                        {
                            if (rgxContainer.IsMatch(textBody.Text))
                            {
                                string saleNumber = rgxSubject.Match(summary.Envelope.Subject).ToString();
                                string bol = rgxLading.Match(textBody.Text).ToString();
                                string container = rgxContainer.Match(textBody.Text).ToString();
                                SaleOrder matchedSale;

                                //Download the complete message to save attachment filename.
                                MimeMessage completeMessage = inbox.GetMessage(summary.UniqueId);

                                //Unique SaleNumber Constraint check, will try to find an object with the same
                                //SaleNumber
                                if (sales.Contains(new SaleOrder(Int32.Parse(saleNumber), bol, container)))
                                {
                                    matchedSale = sales.GetById(Int32.Parse(saleNumber));
                                    matchedSale.ContainerNumber = container;
                                    matchedSale.BillLading = bol;
                                }
                                else
                                {
                                    matchedSale = new SaleOrder (Int32.Parse(saleNumber), bol, container);
                                    sales.Add(matchedSale);
                                }
                                //if any attachament it will Iterate through and save filename in the SaleOrder Object.
                                foreach (var attachment in completeMessage.Attachments)
                                {
                                    var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                                    matchedSale.Attachments.Add(fileName);
                                }

                                Console.WriteLine("Sale Order: {0} | Lading: {1} | Container: {2} ",
                                                  matchedSale.SaleNumber, matchedSale.BillLading, matchedSale.ContainerNumber);
                            }
                            else
                            {
                                Console.WriteLine("Matched Sale Order and BoL, Container not finded!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Matched Sale Order, BoL not finded!");
                        }
                       
                    }else
                    {
                        Console.WriteLine("Not Matched!");
                    }

                }

                Console.WriteLine();
                Console.WriteLine("| STORED OBJECT LIST |");

                foreach (var sale in sales)
                {

                    Console.WriteLine("Sale Order: {0} | Lading: {1} | Container: {2} ",
                                      sale.SaleNumber, sale.BillLading, sale.ContainerNumber);
                    foreach (var file in sale.Attachments)
                    {
                        Console.WriteLine("File name {0}", file);
                    }

                }
                client.Disconnect(true);
            }
        }
    }
}
