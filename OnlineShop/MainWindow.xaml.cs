using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using OnlineShop.Models;
using OnlineShop.Views;
using OnlineShop.Data;

namespace OnlineShop
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<CartItem> _cartItems;
        private decimal _cartTotalPrice;
        private ObservableCollection<Product> _products;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set
            {
                _cartItems = value;
                OnPropertyChanged();
            }
        }

        public decimal CartTotalPrice
        {
            get => _cartTotalPrice;
            set
            {
                _cartTotalPrice = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация коллекций
            InitializeProducts();
            CartItems = new ObservableCollection<CartItem>();
            CartTotalPrice = 0;

            // Подписка на изменения в корзине
            CartItems.CollectionChanged += (s, e) => UpdateCartTotal();

            // Загружаем ProductsView по умолчанию
            ShowProductsView();

            // Установка DataContext
            DataContext = this;
        }

        private void InitializeProducts()
        {
            var dbHelper = new DatabaseHelper();
            var productsList = dbHelper.GetProducts();
            Products = new ObservableCollection<Product>(productsList);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateCartTotal()
        {
            CartTotalPrice = CartItems.Sum(item => item.TotalPrice);
        }

        // Метод для добавления товара в корзину
        public void AddToCart(Product product)
        {
            if (product == null) return;

            // Проверяем, есть ли уже такой товар в корзине
            var existingItem = CartItems.FirstOrDefault(item => item.Product?.Id == product.Id);

            if (existingItem != null)
            {
                // Увеличиваем количество
                existingItem.Quantity++;
            }
            else
            {
                // Добавляем новый товар
                var newItem = new CartItem
                {
                    Product = product,
                    Quantity = 1
                };
                CartItems.Add(newItem);
            }
        }

        // Метод для удаления товара из корзины
        public void RemoveFromCart(CartItem item)
        {
            if (item != null && CartItems.Contains(item))
            {
                CartItems.Remove(item);
            }
        }

        // Метод для оформления заказа
        public void PlaceOrder(string fullName, string email, string address)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Создаем заказ
                var order = new Order
                {
                    FullName = fullName,
                    Email = email,
                    DeliveryAddress = address,
                    OrderDate = DateTime.Now,
                    TotalAmount = CartTotalPrice
                };

                // Конвертируем CartItems в OrderItems
                foreach (var cartItem in CartItems)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = cartItem.Product.Id,
                        ProductName = cartItem.Product.Name,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Product.Price
                    });
                }

                // Сохраняем заказ в базе данных
                var dbHelper = new DatabaseHelper();
                var orderId = dbHelper.SaveOrder(order);

                MessageBox.Show($"Заказ оформлен успешно!\nНомер заказа: {orderId}\n" +
                              $"Клиент: {order.FullName}\n" +
                              $"Сумма: {order.TotalAmount} руб.\n" +
                              $"Товаров: {CartItems.Sum(item => item.Quantity)} шт.",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Очищаем корзину после оформления
                CartItems.Clear();

                // Возвращаемся к товарам
                ShowProductsView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для показа страницы оформления заказа
        public void ShowCheckoutView()
        {
            var checkoutView = new CheckoutView();
            checkoutView.DataContext = this;
            MainContent.Content = checkoutView;
        }

        // Навигационные методы
        private void ShowProductsView()
        {
            var productsView = new ProductsView();
            productsView.DataContext = Products; // Передаем список продуктов
            MainContent.Content = productsView;
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowProductsView();
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new CartView();
            view.DataContext = this;
            MainContent.Content = view;
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCheckoutView();
        }
    }
}