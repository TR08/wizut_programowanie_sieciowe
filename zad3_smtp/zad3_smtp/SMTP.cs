using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace zad3_smtp
{
    public class SMTP : System.Net.Sockets.TcpClient
    {
        public string _server;
        public int _port;
        public SMTP(string server, int port)
        {
            _server = server;
            _port = port;
        }

        public void Send(string to, string sub, string msgContent, string from, string username, string password)
        {
            MainWindow.AppWindow.SetStatus(2);
            string msg, rsp;
            //connect
            Connect(_server, _port);
            rsp = Read();
            if (!ValidateResponse(rsp, "220")) return;
            //greet
            msg = "EHLO wp.pl\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "250")) return;
            //authenticate
            msg = "AUTH PLAIN\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "334")) return;
            msg = EncodeBase64(Encoding.ASCII.GetBytes("\0programowanie_sieciowe@wp.pl\0Progr_Sieciowe123"))+"\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "235")) return;
            //initiate transaction
            msg = "MAIL FROM:<"+from+">\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "250")) return;
            //specify recipient
            msg = "RCPT TO:<" + to + "\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "250")) return;
            //initiate message transfer
            msg = "DATA\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "354")) return;
            //prepare and send message
            msg = "Subject: " + sub + "\r\n";
            msg += "To: " + to + "\r\n";
            msg += "From: " + from + "\r\n";
            msg += "\r\n" + msgContent;
            msg += "\r\n.\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "250")) return;
            //terminate session
            msg = "QUIT\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "221")) return;
            GetStream().Close();
            Close();
            MainWindow.AppWindow.SetStatus(1);
        }

        private bool ValidateResponse(string rsp, string check, string status = "Could not connect")
        {
            if (rsp == "" && check != "") return false;
            if (rsp.Substring(0, 3) != check)
            {
                MainWindow.AppWindow.SetStatus(0, rsp);
                return false;
            }
            return true;
        }

        private void Write(string msg)
        {
            NetworkStream stream = GetStream();
            byte[] buffer = new byte[1024];
            buffer = Encoding.ASCII.GetBytes(msg);
            stream.Write(buffer, 0, buffer.Length);
        }

        private string Read()
        {
            NetworkStream stream = GetStream();
            byte[] buffer = new byte[1024];
            int i = stream.Read(buffer, 0, 1024);
            if (i == 0) return "";
            return Encoding.ASCII.GetString(buffer, 0, i);
        }

        private string EncodeBase64(byte[] binaryData)
        {
            char[] b64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToCharArray();
            //encode full 3-byte packs
            int leftovers = binaryData.Length % 3;
            int count = binaryData.Length - (leftovers);
            char[] base64 = new char[(binaryData.Length + (3 - leftovers)) * 4 / 3];
            int k = 0;

            for (int i = 0; i < count; i += 3)
            {
                base64[k++] = b64[(binaryData[i] >> 2)];
                base64[k++] = b64[((byte)((binaryData[i] << 6) | (binaryData[i + 1] >> 2)) >> 2)];
                base64[k++] = b64[((byte)((binaryData[i + 1] << 4) | (binaryData[i + 2] >> 4)) >> 2)];
                base64[k++] = b64[(binaryData[i + 2] & (byte)63)];
            }
            //check if there are leftovers, encode them and add padding
            if (leftovers != 0)
            {
                base64[k++] = b64[(binaryData[count] >> 2)];
                if (leftovers == 2)
                {
                    base64[k++] = b64[((byte)((binaryData[count] << 6) | (binaryData[count + 1] >> 2)) >> 2)];
                    base64[k++] = b64[((byte)(binaryData[count + 1] << 2) & 60)];
                    base64[k++] = '=';
                }
                else
                {
                    base64[k++] = b64[((byte)(binaryData[count] << 6) & 63)];
                    base64[k++] = '=';
                    base64[k++] = '=';
                }
            }
            //return encoded base64 string
            return new string(base64);
        }
    }
}
