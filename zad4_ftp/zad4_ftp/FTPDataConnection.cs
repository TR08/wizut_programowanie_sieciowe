using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zad4_ftp
{
    class FTPDataConnection : FTP
    {
        public FTPDataConnection(string server, int port) : base(server, port)
        {
            
        }

        public string GetList()
        {
            return Read();
        }
    }
}
