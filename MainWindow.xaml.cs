using Microsoft.Win32;
using System.Collections.ObjectModel;
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
                    && (cbCategory.SelectedIndex == 0 || x.Category == cbCategory.SelectedItem.ToString())
                );
            }
        }

        private void ClearFilters()
        {
            tbName.Text = "";
            tbId.Text = "";
            cbCategory.SelectedIndex = 0;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "BrickStore fájl (.bsx)|*.bsx";

            if (ofd.ShowDialog() == true)
            {
                List<Part> newParts = [];

                try
                {
                    XDocument xaml = XDocument.Load(ofd.FileName);
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
                catch {
                    MessageBox.Show("A kiválasztott fájl nem megfelelő formátum!");
                    return;
                }

                parts = new(newParts);
                dgParts.ItemsSource = parts;

                lblFile.Content = $"Betöltött fájl: {ofd.FileName} ({parts.Count} elem)";

                cbCategory.ItemsSource = parts.Select(x => x.Category).Distinct().Order().Prepend("Összes kategória");
                ClearFilters();
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
    }
}