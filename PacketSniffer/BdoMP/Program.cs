using Colorful;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;

namespace BdoMP
{
    class Program
    {

        public static Dictionary<int, List<string>> Items = new Dictionary<int, List<string>>();
        static void Main(string[] args)
        {
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            if (allDevices.Count == 0)
            {
                System.Console.WriteLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }

            // Print the list
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                System.Console.Write((i + 1) + ". " + device.Name);
                if (device.Description != null)
                    System.Console.WriteLine(" (" + device.Description + ")");
                else
                    System.Console.WriteLine(" (No description available)");
            }

            PacketDevice selectedDevice = allDevices[0];

            using (PacketCommunicator communicator =
                selectedDevice.Open(65536,                                  // portion of the packet to capture
                                                                            // 65536 guarantees that the whole packet will be captured on all the link layers
                                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                    1000))                                  // read timeout
            {
                System.Console.WriteLine("Listening on " + selectedDevice.Description + "...");

                // start the capture
                communicator.ReceivePackets(0, PacketHandler);
            }

            
        }

        private static void PacketHandler(Packet packet)
        {
            //System.Console.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);
            if (packet.Ethernet.IpV4.Source.ToString() == "49.236.159.70")
            {
                //System.Console.WriteLine();
                Marketplace(ByteArrayToString(packet.Buffer));
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static void Marketplace(string hex)
        {
                try
                {
                    HttpClient client = new HttpClient();
                    string[] bytes = Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => hex.Substring(x, 2)).ToArray();
                    if (bytes[0] != "30")
                        return;
                    for (int i = 54; i < bytes.Length;)
                    {
                        if (!(i + 3 > bytes.Length || i + 4 > bytes.Length))
                        {
                            //26 10 02
                            if ((bytes[i + 3] == "26" && bytes[i + 4] == "10" && bytes[i + 5] == "02"))
                            {
                                //System.Console.WriteLine(string.Format("MARKETPLACE {0} {1}", i, bytes[i]));
                                Int64 price = Int64.Parse(string.Format("{0}{1}{2}{3}", bytes[i + 12], bytes[i + 11], bytes[i + 10], bytes[i + 9]), System.Globalization.NumberStyles.HexNumber);
                                //81
                                int itemid = int.Parse(string.Format("{0}{1}", bytes[i + 18], bytes[i + 17]), System.Globalization.NumberStyles.HexNumber);
                                //84
                                int Enchant = int.Parse(bytes[i + 20], System.Globalization.NumberStyles.HexNumber);
                                //85
                                int Amount = int.Parse(string.Format("{0}{1}", bytes[i + 84], bytes[i + 83]), System.Globalization.NumberStyles.HexNumber);
                                if (price == 1)
                                {
                                    i += int.Parse(bytes[i], System.Globalization.NumberStyles.HexNumber);
                                    continue;
                                }
                                //client.PostAsync(string.Format("http://localhost:61073/api/AddItem/{0}/{1}/{2}/{3}/{4}", itemId, price, Enchant, Amount, "api"), new StringContent("", UnicodeEncoding.UTF8, "application/json"));

                                //System.Console.WriteLine(elapsedMs);

                                string EnchantName = "";
                                if (Enchant <= 15 && Enchant != 0)
                                    EnchantName = Enchant.ToString() + " ";
                                else if (Enchant > 15)
                                {
                                    switch (Enchant)
                                    {
                                        case 16:
                                            EnchantName = "PRI:";
                                            break;
                                        case 17:
                                            EnchantName = "DUO:";
                                            break;
                                        case 18:
                                            EnchantName = "TRI:";
                                            break;
                                        case 19:
                                            EnchantName = "TET:";
                                            break;
                                        case 20:
                                            EnchantName = "PEN:";
                                            break;
                                    }
                                }

                                ItemTransaction item = new ItemTransaction
                                {
                                    itemId = itemid,
                                    price = price,
                                    enchantment = Enchant,
                                    amount = Amount,
                                    registerTime = DateTime.Now.ToString()
                                };
                                System.Console.WriteLine(item.itemId);
                                client.PostAsync(string.Format("http://localhost:61073/api/AddItem/{0}", "api"), new StringContent(JsonConvert.SerializeObject(item), UnicodeEncoding.UTF8, "application/json"));
                                /*
                                if(price == 1)
                                {
                                    System.IO.File.WriteAllLines(@"D:\Packet\" + itemId + ".txt", bytes.Skip(i).Take(int.Parse(bytes[i], System.Globalization.NumberStyles.HexNumber)));
                                }
                                int spacer = 20;
                                int spacer2 = 30;
                                StyleSheet styleSheet = new StyleSheet(Color.Gray);
                                styleSheet.AddStyle(EnchantName + itemName, System.Drawing.ColorTranslator.FromHtml(itemColor));
                                Colorful.Console.WriteLineStyled(string.Format("Price {0}{4}  {2}{1}{5} Amount {3} ItemId {6}", string.Format("{0:n0}", price), itemName, EnchantName, Amount, new String(' ', spacer - (6 + string.Format("{0:n0}", price).ToString().Length)), new String(' ', spacer2 - (EnchantName.Length + itemName.Length)), itemId), styleSheet);
                                */
                                i += int.Parse(bytes[i], System.Globalization.NumberStyles.HexNumber);

                            }
                            else
                            {
                                if (bytes[i] == "00")
                                    break;
                                i += int.Parse(bytes[i], System.Globalization.NumberStyles.HexNumber); //Skip the bullshit
                            }

                        } else
                        {
                            break;
                        }
                    }
                } catch(Exception ex)
                {
                    return;
                }
        }

        public static string HexStringToString(string hexString)
        {
            if (hexString == null || (hexString.Length & 1) == 1)
            {
                throw new ArgumentException();
            }
            var sb = new StringBuilder();
            for (var i = 0; i < hexString.Length; i += 2)
            {
                var hexChar = hexString.Substring(i, 2);
                sb.Append((char)Convert.ToByte(hexChar, 16));
            }
            return sb.ToString();
        }
    }

    public class ItemTransaction
    {
        public int itemId { get; set; }
        public Int64 price { get; set; }
        public int enchantment { get; set; }
        public int amount { get; set; }
        public string registerTime { get; set; }
        public string color { get; set; }
    }
}
