using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OnlineShop.Models
{
    public class Product : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _description;
        private decimal _price;
        private string _imageUrl;
        private int _stockQuantity;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value;
                OnPropertyChanged();
            }
        }

        public int StockQuantity
        {
            get => _stockQuantity;
            set
            {
                _stockQuantity = value;
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