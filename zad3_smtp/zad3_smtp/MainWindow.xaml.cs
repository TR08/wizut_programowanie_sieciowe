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

namespace zad3_smtp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _to, _sub, _msg, _username, _password;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchGUI();
            GetValues();
            SendMsg();
            SwitchGUI();
        }

        private void SendMsg()
        {
            
        }

        private void GetValues()
        {
            _to = ToTextbox.Text;
            _sub = SubjectTextbox.Text;
            _msg = MessageTextbox.Text;
            _username = UsernameTextbox.Text;
            _password = Passwordbox.Password;
        }

        private void SetStatus(bool status, string err="")
        {
            if (status)
            {
                StatusLabel.Content = "Successfully sent";
                StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 190, 0));
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
    }
}
