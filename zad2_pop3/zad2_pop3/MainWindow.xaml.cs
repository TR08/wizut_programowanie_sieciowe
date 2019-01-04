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
        //private string _lastEmailUID = "";
        int _counterTotal = 0, _counter = 0;

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
                AutoRefreshCheckbox.IsEnabled = true;
                AutoRefreshCheckbox.IsChecked = true;
                _counter = 0;
            }
            else
            {
                _pop3.Disconnect();
                ConnectionButton.Content = "Connect";
                _connected = false;
                RefreshButton.IsEnabled = false;
                AutoRefreshCheckbox.IsChecked = false;
                AutoRefreshCheckbox.IsEnabled = false;
            }
        }

        private void ClearButtonClicked(object sender, RoutedEventArgs e)
        {
            ClearEmailsList();
        }

        private void RefreshButtonClicked(object sender, RoutedEventArgs e)
        {
            SwitchConnectionButton();
            _pop3.Disconnect(true);
            _pop3 = new POP3();
            _pop3.Connect(_server, _port, _username, _password, true);
            List < MailHeader > newEmails = _pop3.UIDL();
            _pop3.Retr(newEmails);
            SwitchConnectionButton();

            for (int i=0; i < newEmails.Count; i++)
            {
                DisplayEmail(newEmails[i].number + " " + newEmails[i].title);
            }
            //if (newEmails.Count > 0) _lastEmailUID = newEmails[newEmails.Count - 1].uid;
            if (_emails.Count != 0) _counter += newEmails.Count;
            _counterTotal += newEmails.Count;
            _emails = _emails.Concat(newEmails).ToList<MailHeader>();
            UpdateCounters(_counterTotal + "/" + _counter);
        }

        private void SwitchConnectionButton()
        {
            if (ConnectionButton.Dispatcher.CheckAccess())
            {
                if (ConnectionButton.IsEnabled)
                {
                    ConnectionButton.Content = "Updating";
                    ConnectionButton.IsEnabled = false;
                }
                else
                {
                    ConnectionButton.Content = "Disconnect";
                    ConnectionButton.IsEnabled = true;
                }
            }
            else
            {
                ConnectionButton.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new ControlChecker(SwitchConnectionButton));
            }
        }

        private void UpdateCounters(string counters)
        {
            if (this.CounterField.Dispatcher.CheckAccess())
            {
                CounterField.Text = counters;
            }
            else
            {
                this.CounterField.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new TBXTextChanger(this.UpdateCounters), counters);
            }
        }

        public void DisplayEmail(string title)
        {
            if (this.EmailsList.Dispatcher.CheckAccess())
            {
                string old = EmailsList.Text;
                EmailsList.Text = title + "\n" + old;
            }
            else
            {
                this.EmailsList.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new TBXTextChanger(this.DisplayEmail), title);
            }
        }

        public void UpdateStatus(string status)
        {
            if (this.StatusLabel.Dispatcher.CheckAccess())
            {
                StatusLabel.Content = status;
            }
            else
            {
                this.StatusLabel.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new TBXTextChanger(this.UpdateStatus), status);
            }
        }

        private void AutoRefreshCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                RefreshButtonClicked(null, null);
                System.Threading.Thread.Sleep(_interval * 1000);
                IsAutoChecked();
            });
        }

        private void IsAutoChecked()
        {
            if (this.AutoRefreshCheckbox.Dispatcher.CheckAccess())
            {
                if (AutoRefreshCheckbox.IsChecked == true) AutoRefreshCheckbox_Checked(null, null);
            }
            else
            {
                this.AutoRefreshCheckbox.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new ControlChecker(IsAutoChecked));
            }
        }

        private delegate void TBXTextChanger(string status);
        private delegate void ControlChecker();

        public void UpdateListStatus(string status)
        {
            if (this.UpdateListLabel.Dispatcher.CheckAccess())
            {
                UpdateListLabel.Content = status;
            }
            else
            {
                this.UpdateListLabel.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new TBXTextChanger(this.UpdateListStatus), status);
            }
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
