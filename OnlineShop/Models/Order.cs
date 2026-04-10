using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OnlineShop.Models
{
    public class Order : INotifyPropertyChanged
    {
        private int _id;
        private string _fullName;
        private string _email;
        private string _deliveryAddress;
        private List<OrderItem> _orderItems;
        private decimal _totalAmount;
        private DateTime _orderDate;
        private string _status;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set
            {
                _deliveryAddress = value;
                OnPropertyChanged();
            }
        }

        public List<OrderItem> OrderItems
        {
            get => _orderItems;
            set
            {
                _orderItems = value;
                OnPropertyChanged();
                UpdateTotal();
            }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged();
            }
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            set
            {
                _orderDate = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public Order()
        {
            OrderDate = DateTime.Now;
            OrderItems = new List<OrderItem>();
            Status = "Новый";
        }

        private void UpdateTotal()
        {
            TotalAmount = 0;
            foreach (var item in OrderItems)
            {
                TotalAmount += item.Total;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}