using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WPF_Brickstore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Part> parts = [];
        string folder;

        public MainWindow()
        {
            InitializeComponent();
            dgParts.ItemsSource = parts;
        }

        private void FilterParts()
        {
            if (parts.Count > 0)
            {
                dgParts.ItemsSource = parts.Where(x =>
                    (tbName.Text == "" || x.Name.Contains(tbName.Text, StringComparison.CurrentCultureIgnoreCase))
                    && (tbId.Text == "" || x.Id.Contains(tbId.Text, StringComparison.CurrentCultureIgnoreCase))
                    && (cbCategory.SelectedIndex <= 0 || x.Category == cbCategory.SelectedItem.ToString())
                );

                SetCategories();
            }
        }

        private void SetCategories()
        {
            cbCategory.ItemsSource = parts.Where(x =>
                    (tbName.Text == "" || x.Name.Contains(tbName.Text, StringComparison.CurrentCultureIgnoreCase))
                    && (tbId.Text == "" || x.Id.Contains(tbId.Text, StringComparison.CurrentCultureIgnoreCase)))
                .Select(x => x.Category).Distinct().Order().Prepend("Összes kategória");

            if (cbCategory.SelectedIndex == -1) cbCategory.SelectedIndex = 0;
        }

        private void ClearFilters()
        {
            tbName.Text = "";
            tbId.Text = "";
            cbCategory.SelectedIndex = 0;
        }

        private void OpenFile(string path)
        {
            List<Part> newParts = [];

            try
            {
                XDocument xaml = XDocument.Load(path);
                foreach (var item in xaml.Descendants("Item"))
                {
                    newParts.Add(new Part(
                        item.Element("ItemID").Value,
                        item.Element("ItemName").Value,
                        item.Element("CategoryName").Value,
                        item.Element("ColorName").Value,
                        Convert.ToInt32(item.Element("Qty").Value)
                    ));
                }
            }
            catch
            {
                MessageBox.Show("A kiválasztott fájl nem megfelelő formátum!");
                return;
            }

            parts = new(newParts);
            dgParts.ItemsSource = parts;

            lblFile.Content = $"Betöltött fájl: {path} ({parts.Count} elem)";

            SetCategories();
            ClearFilters();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog ofd = new();

            if (ofd.ShowDialog() == true)
            {
                folder = ofd.FolderName;
                lbFiles.ItemsSource = Directory.GetFiles(ofd.FolderName).Where(x => x.EndsWith(".bsx")).Select(x => x.Split('\\').Last());
            }
        }

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterParts();
        }

        private void tbId_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterParts();
        }

        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterParts();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFilters();
        }

        private void lbFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbFiles.SelectedIndex != -1)
            {
                OpenFile(System.IO.Path.Join(folder, lbFiles.SelectedItem.ToString()));
            }
        }
    }
}