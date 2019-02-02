using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace zad4_ftp
{
    public class FTP : System.Net.Sockets.TcpClient
    {
        protected string _server;//, _user, _pass;
        protected int _port;

        public FTP(string server, int port)
        {
            _server = server;
            _port = port;
        }

        public virtual void Connect()
        {
            Connect(_server, _port);
            
            //if (!WriteAndRead("CWD /root\r\n", "250")) return;
            //if (!WriteAndRead("PASV\r\n", "227")) return;
        }

        protected bool WriteAndRead(string msg, string code)
        {
            string rsp;
            Write(msg);
            rsp = Read();
            return ValidateResponse(rsp, code);
        }

        protected void Write(string msg)
        {
            NetworkStream stream = GetStream();
            byte[] buffer = new byte[1024];
            buffer = Encoding.ASCII.GetBytes(msg);
            stream.Write(buffer, 0, buffer.Length);
        }

        protected string Read()
        {
            NetworkStream stream = GetStream();
            byte[] buffer = new byte[1024];
            int i = stream.Read(buffer, 0, 1024);
            if (i == 0) return "";
            return Encoding.ASCII.GetString(buffer, 0, i);
        }

        protected bool ValidateResponse(string rsp, string check, string status = "Could not connect")
        {
            if (rsp == "" && check != "") return false;
            if (rsp.Substring(0, 3) != check)
            {
                MainWindow.AppWindow.SetStatus(0, rsp);
                return false;
            }
            MainWindow.AppWindow.SetStatus(0, rsp);
            return true;
        }
    }
}
