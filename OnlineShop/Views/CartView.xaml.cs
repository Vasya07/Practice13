using System.Windows;
using System.Windows.Controls;
using OnlineShop.Models;

namespace OnlineShop.Views
{
    public partial class CartView : UserControl
    {
        public CartView()
        {
            InitializeComponent();
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem item)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.RemoveFromCart(item);
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.ShowCheckoutView();
        }
    }
}