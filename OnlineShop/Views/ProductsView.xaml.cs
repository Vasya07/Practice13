using System.Windows;
using System.Windows.Controls;
using OnlineShop.Models;

namespace OnlineShop.Views
{
    public partial class ProductsView : UserControl
    {
        public ProductsView()
        {
            InitializeComponent();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Product product)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.AddToCart(product);
                    MessageBox.Show($"Товар \"{product.Name}\" добавлен в корзину!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}