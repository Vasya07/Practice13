using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OnlineShop.Models
{
    public class OrderItem : INotifyPropertyChanged
    {
        private int _id;
        private int _orderId;
        private int _productId;
        private string _productName;
        private int _quantity;
        private decimal _price;
        private decimal _total;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public int OrderId
        {
            get => _orderId;
            set
            {
                _orderId = value;
                OnPropertyChanged();
            }
        }

        public int ProductId
        {
            get => _productId;
            set
            {
                _productId = value;
                OnPropertyChanged();
            }
        }

        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged();
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                Total = Quantity * Price;
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
                Total = Quantity * Price;
            }
        }

        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}