using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace zad4_ftp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _server, _username, _password;
        public static MainWindow AppWindow;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            GetConnectionData();
            FTPControlConnection FTPControl = new FTPControlConnection(_server, 21, _username, _password);
            FTPControl.Connect();
            var rspAddress = FTPControl.Pasv().Split(':');
            FTPDataConnection FTPData = new FTPDataConnection(rspAddress[0], Int32.Parse(rspAddress[1]));
            FTPData.Connect();
            FTPControl.List();
            StatusLabel.Content += "\n" + FTPData.GetList();
        }

        private void GetConnectionData()
        {
            XmlDocument config = new XmlDocument();
            config.Load("App.config");
            _server = config.DocumentElement.SelectSingleNode("/ConnectionData/Server").InnerText;
            _password = config.DocumentElement.SelectSingleNode("/ConnectionData/Password").InnerText;
            _username = config.DocumentElement.SelectSingleNode("/ConnectionData/Username").InnerText;
        }

        public void SetStatus(byte status, string err = "")
        {
            if (status == 1)
            {
                StatusLabel.Content = "Successfully sent";
                StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 190, 0));
            }
            else if (status == 2)
            {
                StatusLabel.Content = "";
            }
            else
            {
                StatusLabel.Content += "\nError: " + err;
                StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(210, 0, 0));
            }
        }
    }
}
