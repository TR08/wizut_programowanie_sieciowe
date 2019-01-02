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
        private string _saveToPath;
        public MainWindow()
        {
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

                int idx = filename.LastIndexOf('\\');
                _saveToPath = filename.Substring(0, idx + 1);
                SetNewFileNameAndExt(1);

                binaryTxtBox.Text = "";
                base64TxtBox.Text = "";
                newFileName.IsEnabled = true;
                newFileExt.IsEnabled = true;
                startBtn.IsEnabled = true;
            }
        }

        private void SetNewFileNameAndExt(int mode=0)
        {
            int idx = fileNameTextBox.Text.LastIndexOf('\\');
            string curFileName = fileNameTextBox.Text.Substring(idx + 1);
            idx = curFileName.LastIndexOf('.');
            if (curFileName.Substring(idx + 1) != "b64")
            {
                newFileExt.Text = "b64";
                newFileName.Text = curFileName;
                encodeRadio.IsChecked = true;
                decodeRadio.IsEnabled = false;
                encodeRadio.IsEnabled = true;
            }
            else
            {
                string temp = curFileName.Substring(0, idx);
                idx = temp.LastIndexOf('.');
                newFileExt.Text = temp.Substring(idx + 1);
                newFileName.Text = temp.Substring(0, idx);
                decodeRadio.IsChecked = true;
                decodeRadio.IsEnabled = true;
                encodeRadio.IsEnabled = false;
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
                string saveToPath = _saveToPath+newFileName.Text+"."+newFileExt.Text;
                string base64Data = GetBase64Data(fileNameTextBox.Text);
                base64TxtBox.Text = base64Data;
                byte[] decodedData = DecodeBase64(base64Data);
                binaryTxtBox.Text = "";
                for (int i = 0; i < decodedData.Length; ++i)
                    binaryTxtBox.Text += decodedData[i].ToString("x") + " ";
                File.WriteAllBytes(saveToPath, decodedData);
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
            char[] b64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToCharArray();
            //encode full 3-byte packs
            string base64 = "";
            string tmp = "";
            int sixPacks = 0;
            for (int i = 0; i < binaryData.Length; ++i)
            {
                tmp += binaryData[i];
                if ((i + 1) % 6 == 0)
                {
                    base64 += b64[Convert.ToInt32(tmp, 2)];
                    tmp = "";
                    ++sixPacks;
                }
            }
            //check if there are leftovers and encode them
            if (tmp != "")
            {
                base64 += b64[Convert.ToInt32(tmp.PadRight(6, '0'), 2)];
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

        private byte[] DecodeBase64(string base64Data)
        {
            Dictionary<int, int> b64 = new Dictionary<int, int>();
            for (int i = 48; i < 58; ++i)
                b64.Add(i, i + 4); //0-9
            for (int i = 65; i < 91; ++i)
                b64.Add(i, i - 65); //A-Z
            for (int i = 97; i < 123; ++i)
                b64.Add(i, i - 71); //a-z
            b64.Add(43, 62); //'+'
            b64.Add(47, 63); //'/'

            //prepare array for bytes
            int count = base64Data.Length * 3 / 4;
            if (base64Data[base64Data.Length - 1] == '=')
            {
                --count;
                if (base64Data[base64Data.Length - 2] == '=') --count;
            }
            byte[] bytes = new byte[count];
            int k = 0;

            //decode
            for (int i=0; i<base64Data.Length; i += 4)
            {
                if (base64Data[i + 3] != '=')
                {
                    bytes[k++] = (byte)((b64[base64Data[i]] << 2) | (b64[base64Data[i + 1]] >> 4));
                    bytes[k++] = (byte)((b64[base64Data[i + 1]] << 4) | (b64[base64Data[i + 2]] >> 2));
                    bytes[k++] = (byte)((b64[base64Data[i + 2]] << 6) | b64[base64Data[i + 3]]);
                }
                else
                {
                    bytes[k++] = (byte)((b64[base64Data[i]] << 2) | (b64[base64Data[i + 1]] >> 4));
                    if (base64Data[i + 2] != '=')
                    {
                        bytes[k++] = (byte)((b64[base64Data[i + 1]] << 4) | (b64[base64Data[i + 2]] >> 2));
                    }
                }
            }
            return bytes;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}