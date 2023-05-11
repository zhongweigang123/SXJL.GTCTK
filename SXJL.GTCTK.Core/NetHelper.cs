using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace RWS.Core
{
    public class NetHelper
    {
        public static string[] GetIPAddresses(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            IPAddress[] ipadrlist = Dns.GetHostAddresses(Dns.GetHostName());
            return ipadrlist.Where(t => t.AddressFamily == addressFamily).GroupBy(t => t.ToString()).Select(t => t.Key).ToArray();
        }

        public static string GetIPAddressesToStr(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            return string.Join(" ", GetIPAddresses(addressFamily));
        }

        /// <summary>
        /// 获得本机IP
        /// </summary>
        /// <param name="first">ip地址开头，例如A类10，B类172，C类192……</param>
        /// <returns></returns>
        public static string GetIP(string first)
        {
            return GetIPAddresses().FirstOrDefault(t => t.StartsWith(first));
        }
        /// <summary>
        /// Ping
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool Ping(string ip)
        {
            try
            {
                return new Ping().Send(ip).Status == IPStatus.Success;
            }
            catch { return false; }
        }

        public static void SocketSend(int port, string host, string Content)
        {
            if (Ping(host))
            {
                try
                {
                    IPAddress ip = IPAddress.Parse(host);
                    IPEndPoint ipe = new IPEndPoint(ip, port);
                    Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    c.Connect(ipe);
                    byte[] bs = Encoding.Unicode.GetBytes(Content);
                    _ = c.Send(bs, bs.Length, 0);
                    c.Close();
                }
                catch { }
            }
        }
    }
}