using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OnlineShop.Models
{
    public class CartItem : INotifyPropertyChanged
    {
        private Product _product;
        private int _quantity;
        private decimal _totalPrice;

        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged();
                RecalculateTotal();
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 0) return;
                _quantity = value;
                OnPropertyChanged();
                RecalculateTotal();
            }
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged();
            }
        }

        private void RecalculateTotal()
        {
            if (Product != null)
            {
                TotalPrice = Quantity * Product.Price;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}