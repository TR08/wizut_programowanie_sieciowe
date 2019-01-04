using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace zad2_pop3
{
    public class POP3 : System.Net.Sockets.TcpClient
    {
        public void Connect(string server, int port, string username, string password, bool refresh = false)
        {
            string msg, rsp;
            //connect
            Connect(server, port);
            rsp = Read();
            if (!ValidateResponse(rsp, "+OK")) return;
            //send username
            msg = "USER " + username + "\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "+OK")) return;
            //send pass
            msg = "PASS " + password + "\r\n";
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "+OK")) return;
            if (!refresh)
            {
                MainWindow.AppWindow.UpdateStatus("Connected as: " + username);
                MainWindow.AppWindow.ClearEmailsList();
            }
        }

        public void Disconnect(bool refresh=false)
        {
            string msg = "QUIT\r\n", rsp;
            Write(msg);
            rsp = Read();
            if (!ValidateResponse(rsp, "+OK", "Could not disconnect")) return;

            GetStream().Close();
            Close();
            if (!refresh)
            {
                MainWindow.AppWindow.UpdateStatus("Disconnected");
                MainWindow.AppWindow.ClearEmailsList();
            }
        }

        private bool ValidateResponse(string rsp, string check, string status="Could not connect")
        {
            if (rsp == "" && check != "") return false;
            if (rsp.Substring(0, 3) != check)
            {
                MainWindow.AppWindow.UpdateStatus(status);
                MainWindow.AppWindow.ClearEmailsList();
                MainWindow.AppWindow.DisplayEmail("Error message:\n" + rsp);
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
            int i = 0;
            for (; ;)
            {
                byte[] buf = new byte[2];
                int byteCount = stream.Read(buf, 0, 1);
                if (byteCount == 1)
                {
                    buffer[i++] = buf[0];
                    if (buf[0] == '\n')
                    {
                        i -= 2;
                        break;
                    }
                }
                else break;
            }
            return Encoding.ASCII.GetString(buffer, 0, i);
        }

        public List<MailHeader> UIDL()
        {
            List<MailHeader> curEmails = MainWindow.AppWindow.GetCurrentEmailsList();
            List<MailHeader> emails = new List<MailHeader>();
            List<MailHeader> newEmails = new List<MailHeader>();
            string msg = "UIDL\r\n", rsp;
            Write(msg);
            rsp = Read();
            if (rsp.Substring(0, 3) != "+OK")
            {
                MainWindow.AppWindow.UpdateListStatus("Could not update emails list");
                return null;
            }
            MainWindow.AppWindow.UpdateListStatus("");
            //get refreshed emails
            while (true)
            {
                rsp = Read();
                if (rsp == ".") break;
                else
                {
                    MailHeader email = new MailHeader();
                    string[] tmp = rsp.Split(' ');
                    email.number = Convert.ToInt32(tmp[0]);
                    email.uid = tmp[1];
                    emails.Add(email);
                }
            }
            //compare emails lists
            for (int i=0; i<emails.Count; ++i)
            {
                bool add = true;
                for(int j = 0; j < curEmails.Count; ++j)
                {
                    if (emails[i].uid == curEmails[j].uid)
                    {
                        add = false;
                        break;
                    }
                }
                if (add) newEmails.Add(emails[i]);
            }
            return newEmails;
        }

        public List<MailHeader> Retr(List<MailHeader> emails)
        {
            string msg, rsp;
            for (int i=0; i<emails.Count; ++i)
            {
                msg = "RETR " + emails[i].number + "\r\n";
                Write(msg);
                rsp = Read();
                if (!ValidateResponse(rsp, "+OK", "Could not disconnect")) continue;
                while (true)
                {
                    rsp = Read();
                    if (rsp == ".") break;
                    else
                    {
                        //Subject: Witaj w WP Poczcie!
                        if (rsp.Contains("Subject: "))
                        {
                            int start = rsp.IndexOf("Subject: ") + 9;
                            emails[i].title += DecodeQuotedPrintable(rsp.Substring(start));
                        }
                    }
                }
                emails[i].retrieved = true;
            }
            return emails;
        }

        private string DecodeQuotedPrintable(string encoded)
        {
            // jedyne użycie złożonej biblioteki do maili
            // dekodowanie przychodzących tytułów nie było przedmiotem zadania
            System.Net.Mail.Attachment attachment = System.Net.Mail.Attachment.CreateAttachmentFromString("", encoded);
            return attachment.Name;
        }
    }
}
