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
        private string _server, _username, _password, _oldPath, _path = "/";
        public static MainWindow AppWindow;
        FTPControlConnection FTPControl;
        FTPDataConnection FTPData;
        bool _isRaw = true;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!GoBtn.IsEnabled)
            {
                PrepareConnection();
                if (!IsRawFTP())
                {
                    _isRaw = false;
                    AddColumnsForMLSD();
                }
                _path = "/";
                FTPPath.Text = _path;
                Go();
                ConnectBtn.Content = "Disconnect";
                FTPPath.IsEnabled = true;
                FTPList.IsEnabled = true;
                FTPTree.IsEnabled = true;
                GoBtn.IsEnabled = true;
            }
            else
            {
                ConnectBtn.Content = "Connect";
                FTPPath.IsEnabled = false;
                FTPList.IsEnabled = false;
                FTPTree.IsEnabled = false;
                GoBtn.IsEnabled = false;
            }
        }

        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            GoToCustom();
        }

        private void FTPPath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GoToCustom();
                FTPPath.CaretIndex = FTPPath.Text.Length;
            }
            else if (e.Key == Key.Escape)
            {
                FTPPath.Text = _path;
            }
        }

        private void GoToCustom()
        {
            if (FTPPath.Text != _path)
            {
                _path = FTPPath.Text;
                if (_path == "") _path = "/";
                else
                {
                    if (_path[0] != '/') _path = '/' + _path;
                    if (_path[_path.Length - 1] != '/') _path += '/';
                }
                Go();
                FTPPath.Text = _path;
            }
        }

        private bool PrepareConnection()
        {
            GetConnectionData();
            FTPControl = new FTPControlConnection(_server, 21, _username, _password);
            FTPControl.Connect();
            var rspAddress = FTPControl.Pasv().Split(':');
            if (rspAddress.Length < 2)
            {
                SetStatus(0, "Could not get data from the server.");
                _path = _oldPath;
                FTPPath.Text = _oldPath;
                return false;
            }
            FTPData = new FTPDataConnection(rspAddress[0], Int32.Parse(rspAddress[1]));
            FTPData.Connect();
            return true;
        }

        private void Go()
        {
            if (!PrepareConnection()) return;
            bool refTree = FillFTPList();
            FTPControl.Quit();
            FTPData.Quit();
            if (refTree) RefreshFTPTree();
        }

        private bool IsRawFTP()
        {
            var rsp = FTPControl.Help();
            rsp = rsp.ToLower();
            if (rsp.Contains("mlsd") || rsp.Contains("pureftpd")) return false;
            return true;
        }

        private bool FillFTPList()
        {
            FTPControl.MakeCurrentDir(_path);
            if (_isRaw) FTPControl.ListOfNames(_path);
            else FTPControl.ListAdvanced(_path);
            var RecivedData = FTPData.GetList();
            //StatusLabel.Content += "\n" + RecivedData;
            if (RecivedData == "")
            {
                SetStatus(0, "This is not a directory");
                _path = _oldPath;
                FTPPath.Text = _oldPath;
                return false;
            }
            FTPList.Items.Clear();
            AddDataToListView(RecivedData);
            return true;
        }

        private void AddColumnsForMLSD()
        {
            GridView myGridView = (GridView)FTPList.View;
            GridViewColumn gvc1 = new GridViewColumn
            {
                DisplayMemberBinding = new Binding("Size"),
                Header = "Size [B]",
                Width = 50
            };
            myGridView.Columns.Add(gvc1);
            gvc1 = new GridViewColumn
            {
                DisplayMemberBinding = new Binding("Permissions"),
                Header = "Permissions",
                Width = 80
            };
            myGridView.Columns.Add(gvc1);
            gvc1 = new GridViewColumn
            {
                DisplayMemberBinding = new Binding("ModDate"),
                Header = "ModDate",
                Width = 120
            };
            myGridView.Columns.Add(gvc1);
            gvc1 = new GridViewColumn
            {
                DisplayMemberBinding = new Binding("ItemType"),
                Header = "Type",
                Width = 50
            };
            myGridView.Columns.Add(gvc1);
        }

        private void AddDataToListView(string data, bool raw = true)
        {
            var rows = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //string[] cells = rows[6].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int start = 1;
            if (_path == "/") start = 2;
            for (int i = start; i < rows.Length; ++i)
            {
                if (!_isRaw)
                {
                    string[] cells = rows[i].Split(';');
                    string row = rows[i].Substring(0, rows[i].LastIndexOf(';')) + ";name=" + rows[i].Substring(rows[i].LastIndexOf(';') + 2);
                    Dictionary<string, string> rowsDict = row.Split(';').Select(value => value.Split('=')).ToDictionary(pair => pair[0], pair => pair[1]);
                    FTPList.Items.Add(new FTPListItem { Name = (rowsDict.ContainsKey("name") ? rowsDict["name"] : ""), Size = (rowsDict.ContainsKey("size") ? rowsDict["size"] : "" ), Permissions = (rowsDict.ContainsKey("UNIX.mode") ? rowsDict["UNIX.mode"] : ""), ModDate = (rowsDict.ContainsKey("modify") ? rowsDict["modify"].Substring(0, 4)+"-"+ rowsDict["modify"].Substring(4, 2)+"-"+ rowsDict["modify"].Substring(6, 2)+" "+ rowsDict["modify"].Substring(8, 2)+":" + rowsDict["modify"].Substring(10, 2) + ":" + rowsDict["modify"].Substring(12, 2) : ""), ItemType = (rowsDict.ContainsKey("type") ? rowsDict["type"] : "") });
                }
                else
                {
                    FTPList.Items.Add(new FTPListItem { Name = rows[i] }); //Size = cells[4], Permissions = cells[0], ModDate = cells[5] + " " + cells[6] + " " + cells[7] });
                }
            }
        }

        void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FTPListItem item = FTPList.SelectedItem as FTPListItem;
            var nodes = _path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string path = "/";
            for (int i = 0; i < nodes.Length - 1; ++i) path += nodes[i] + "/";
            if (item.Name != "..")
            {
                if (nodes.Length > 0) path += nodes[nodes.Length - 1] + "/";
                path += item.Name + "/";
            }
            _oldPath = _path;
            _path = path;
            FTPPath.Text = _path;
            Go();
        }

        private void RefreshFTPTree()
        {
            FTPTree.Items.Clear();
            bool isLast = false;
            string tempPath = _path;
            if (tempPath == "/") isLast = true;
            if (!PrepareConnection()) return;
            TreeViewItem TreeNode = null;
            string parent = "";
            do
            {
                FTPControl.ListOfNames(tempPath);
                var RecivedData = FTPData.GetList();
                //StatusLabel.Content = "\n" + RecivedData;
                FTPControl.Quit();
                FTPData.Quit();
                var nodes = RecivedData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                TreeNode = BuildTreeNode(nodes, TreeNode, parent);
                if (isLast) break;
                if (!PrepareConnection()) return;
                var parentTemp = tempPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (parentTemp.Length > 0) parent = parentTemp[parentTemp.Length - 1];
                else parent = "/";
                FTPControl.MakeParentAsCurrent();
                var temp1 = tempPath.Substring(0, tempPath.LastIndexOf('/'));
                var temp2 = temp1.Substring(0, temp1.LastIndexOf('/') + 1);
                tempPath = temp2;
                if (tempPath == "/") isLast = true;
                System.Threading.Thread.Sleep(500);
            } while (true);
            TreeNode.Header = "/";
            FTPTree.Items.Add(TreeNode);
        }

        private TreeViewItem BuildTreeNode(string[] nodes, TreeViewItem subTree = null, string parentOfSubName = "")
        {
            TreeViewItem parent = new TreeViewItem();
            for (int i = 2; i < nodes.Length; ++i)
            {
                TreeViewItem node = new TreeViewItem
                {
                    Header = nodes[i],
                    IsExpanded = true
                };
                parent.Items.Add(node);
                if (subTree != null && nodes[i] == parentOfSubName)
                {
                    subTree.Header = parentOfSubName;
                    subTree.MouseDoubleClick += FTPTreeItem_MouseDoubleClick;
                    parent.Items[i - 2] = subTree;
                }
            }
            parent.IsExpanded = true;
            parent.MouseDoubleClick += FTPTreeItem_MouseDoubleClick;
            return parent;
        }

        private void FTPTreeOnItemSelected(object sender, RoutedEventArgs e)
        {
            FTPTree.Tag = e.OriginalSource;
        }

        void FTPTreeItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FTPTree.SelectedItem != null)
            {
                TreeViewItem selectedTVI = FTPTree.Tag as TreeViewItem;
                string path = selectedTVI.Header.ToString();
                if (path != "/") path += "/";
                while (!selectedTVI.Parent.GetType().IsInstanceOfType(new TreeView()))
                {
                    var parentName = ((TreeViewItem)selectedTVI.Parent).Header.ToString();
                    if (parentName == "/") parentName = "";
                    path = parentName + "/" + path;
                    selectedTVI = (TreeViewItem)selectedTVI.Parent;
                }
                FTPPath.Text = path;
                GoToCustom();
            }
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
                StatusLabel.Content = err;
                StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 190, 0));
            }
            else if (status == 2)
            {
                StatusLabel.Content = "";
            }
            else
            {
                StatusLabel.Content = "Error: " + err;
                StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(210, 0, 0));
            }
        }
    }
}
