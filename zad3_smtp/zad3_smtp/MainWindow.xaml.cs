using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

namespace zad3_smtp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;
        private string _to, _sub, _msg, _from, _server, _username, _password;
        int _port;
        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchGUI();
            GetConnectionData();
            GetValues();
            SMTP smtp = new SMTP(_server, _port);
            smtp.Send(_to, _sub, _msg, _from, _username, _password);
            SwitchGUI();
        }

        private void GetValues()
        {
            _to = ToTextbox.Text;
            _sub = SubjectTextbox.Text;
            _msg = MessageTextbox.Text;
        }

        public void SetStatus(byte status, string err="")
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
                StatusLabel.Content = "Error: "+err;
                StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(210, 0, 0));
            }
        }

        private void SwitchGUI()
        {
            if (SendButton.IsEnabled)
            {
                SendButton.Content = "Sending...";
                SendButton.IsEnabled = false;
                ToTextbox.IsEnabled = false;
                SubjectTextbox.IsEnabled = false;
                MessageTextbox.IsEnabled = false;
            }
            else
            {
                SendButton.Content = "Send";
                SendButton.IsEnabled = true;
                ToTextbox.IsEnabled = true;
                SubjectTextbox.IsEnabled = true;
                MessageTextbox.IsEnabled = true;
            }
        }

        private void GetConnectionData()
        {
            XmlDocument config = new XmlDocument();
            config.Load("App.config");
            _server = config.DocumentElement.SelectSingleNode("/ConnectionData/Server").InnerText;
            _from = config.DocumentElement.SelectSingleNode("/ConnectionData/email_from").InnerText;
            _port = Convert.ToInt32(config.DocumentElement.SelectSingleNode("/ConnectionData/Port").InnerText);
            _password = config.DocumentElement.SelectSingleNode("/ConnectionData/Password").InnerText;
            _username = config.DocumentElement.SelectSingleNode("/ConnectionData/Username").InnerText;
        }
    }
}
