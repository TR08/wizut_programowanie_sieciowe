using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net;
//using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace zad4_ftp
{
    public class FTPControlConnection : FTP
    {
        private string _user, _pass;

        public FTPControlConnection(string server, int port, string username, string password) : base(server, port)
        {
            _user = username;
            _pass = password;
        }

        public override void Connect()
        {
            string rsp;
            Connect(_server, _port);
            rsp = Read();
            if (!ValidateResponse(rsp, "220")) return;

            if (!WriteAndRead("USER " + _user + "\r\n", "331")) return;
            if (!WriteAndRead("PASS " + _pass + "\r\n", "230")) return;
        }

        //private void Port()
        //{
        //    //nieużywane
        //    string ip = Dns.GetHostAddresses(Dns.GetHostName()).Where(address => address.AddressFamily == AddressFamily.InterNetwork).First().ToString();
        //    ip = ip.Replace('.', ',');
        //    MainWindow.AppWindow.StatusLabel.Content += "\nPORT " + ip + @",200,200\r\n";
        //    //9536
        //    if (!WriteAndRead("PORT " + ip + ",0,20\r\n", "200")) return;
        //}

        public string Pasv()
        {
            string rsp, msg;
            msg = "PASV\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "227")) return "";
            rsp = rsp.Substring(rsp.IndexOf('(') + 1);
            rsp = rsp.Substring(0, rsp.Length - 3);
            var temp = rsp.Split(',');
            rsp = temp[0] + '.' + temp[1] + '.' + temp[2] + '.' + temp[3] + ':' + (Int32.Parse(temp[4]) * 256 + Int32.Parse(temp[5]));
            MainWindow.AppWindow.StatusLabel.Content += "\nx" + rsp + "x";
            
            
            return rsp;
        }

        public void List(string path = "/")
        {
            if (!WriteAndRead("LIST " + path + "\r\n", "150")) return;
        }

        public void ListOfNames(string path = "/")
        {
            if (!WriteAndRead("NLST " + path + "\r\n", "150")) return;
        }

        public void ListAdvanced(string path = "/")
        {
            if (!WriteAndRead("MLSD " + path + "\r\n", "150")) return;
        }
    }
}
