using System.Windows;
using System.Windows.Controls;

namespace OnlineShop.Views
{
    public partial class CheckoutView : UserControl
    {
        public CheckoutView()
        {
            InitializeComponent();
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.PlaceOrder(
                    txtFullName.Text,
                    txtEmail.Text,
                    txtAddress.Text
                );
            }
        }
    }
}