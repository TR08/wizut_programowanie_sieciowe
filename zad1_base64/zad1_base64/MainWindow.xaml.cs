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
using System.IO;
using System.Diagnostics;

namespace zad1_base64
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private char[] _b64;
        public MainWindow()
        {
            _b64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToCharArray();
            InitializeComponent();
        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlgWindow = new Microsoft.Win32.OpenFileDialog
            {
                //DefaultExt = ".txt",
                //Filter = "Text documents (.txt)|*.txt"
            };
            Nullable<bool> result = dlgWindow.ShowDialog();
            if (result == true)
            {
                string filename = dlgWindow.FileName;
                fileNameTextBox.Text = filename;
                startBtn.IsEnabled = true;
            }
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (encodeRadio.IsChecked==true)
            {
                string binData = GetBinaryData(fileNameTextBox.Text);
                string encodedData = EncodeBase64(binData);
                base64TxtBox.Text = encodedData;
            }
            else
            {
                string base64Data = GetBase64Data(fileNameTextBox.Text);
                base64TxtBox.Text = base64Data;
                string decodedData = DecodeBase64(base64Data);
                binaryTxtBox.Text = decodedData;
            }
            startBtn.IsEnabled = false;
        }

        private string GetBinaryData(string path)
        {
            //read file as bytes and return string of bits
            byte[] bFile = File.ReadAllBytes(path);
            binaryTxtBox.Text = "";
            string binData = "";
            for (int i = 0; i < bFile.Length; ++i)
            {
                string tmp = Convert.ToString(bFile[i], 2).PadLeft(8,'0');
                binaryTxtBox.Text += tmp + " ";
                binData += tmp;
            }
            return binData;
        }

        private string EncodeBase64(string binaryData)
        {
            //encode full 3-byte packs
            string base64 = "";
            string tmp = "";
            int sixPacks = 0;
            for (int i = 0; i < binaryData.Length; ++i)
            {
                tmp += binaryData[i];
                if ((i + 1) % 6 == 0)
                {
                    base64 += _b64[Convert.ToInt32(tmp, 2)];
                    tmp = "";
                    ++sixPacks;
                }
            }
            //check if there are leftovers and encode them
            if (tmp != "")
            {
                base64 += _b64[Convert.ToInt32(tmp.PadRight(6, '0'), 2)];
                ++sixPacks;
            }
            //complete last pack with '=' for lacking 6-bit parts
            if (sixPacks % 4 != 0)
                for (int i = 0; i < (4 - (sixPacks % 4)); ++i)
                    base64 += "=";
            //return encoded base64 string
            return base64;
        }

        private string GetBase64Data(string path)
        {
            string base64 = File.ReadAllText(path);
            return base64;
        }

        private string DecodeBase64(string base64Data)
        {

            return "";
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}