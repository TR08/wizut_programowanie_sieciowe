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

namespace zad2_pop3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _server, _username, _password;
        private int _port, _interval;
        public static MainWindow AppWindow;
        private bool _connected = false;
        private POP3 _pop3;
        private List<MailHeader> _emails = new List<MailHeader>();
        private string _lastEmailUID = "";

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
        }

        private void GetConnectionData()
        {
            XmlDocument config = new XmlDocument();
            config.Load("App.config");
            _server = config.DocumentElement.SelectSingleNode("/ConnectionData/Server").InnerText;
            _username = config.DocumentElement.SelectSingleNode("/ConnectionData/Username").InnerText;
            _password = config.DocumentElement.SelectSingleNode("/ConnectionData/Password").InnerText;
            _port = Convert.ToInt32(config.DocumentElement.SelectSingleNode("/ConnectionData/Port").InnerText);
            _interval = Convert.ToInt32(config.DocumentElement.SelectSingleNode("/ConnectionData/Interval").InnerText);
        }

        private void ConnectionButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!_connected)
            {
                _pop3 = new POP3();
                GetConnectionData();
                _pop3.Connect(_server, _port, _username, _password);
                _connected = true;
                ConnectionButton.Content = "Disconnect";
                RefreshButton.IsEnabled = true;
            }
            else
            {
                _pop3.Disconnect();
                ConnectionButton.Content = "Connect";
                _connected = false;
                RefreshButton.IsEnabled = false;
            }
        }

        private void ClearButtonClicked(object sender, RoutedEventArgs e)
        {
            ClearEmailsList();
        }

        private void RefreshButtonClicked(object sender, RoutedEventArgs e)
        {
            _pop3.Disconnect(true);
            _pop3 = new POP3();
            _pop3.Connect(_server, _port, _username, _password, true);
            _emails = _pop3.UIDL();
            for (int i=_emails.Count-1; i>=0; i--)
            {
                if (_emails[i].uid != _lastEmailUID) DisplayEmail(_emails[i].number + " " + _emails[i].uid);
                else break;
            }
            _lastEmailUID = _emails[_emails.Count - 1].uid;
        }

        public void DisplayEmail(string title)
        {
            EmailsList.Text += title + "\n";
        }

        public void UpdateStatus(string status)
        {
            StatusLabel.Content = status;
        }

        public void UpdateListStatus(string status)
        {
            UpdateListLabel.Content = status;
        }

        public void ClearEmailsList()
        {
            EmailsList.Text = "";
        }

        public List<MailHeader> GetCurrentEmailsList()
        {
            return _emails;
        }
    }
}
