using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using OnlineShop.Models;

namespace OnlineShop.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OnlineShopDB"].ConnectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Создаем базу данных если ее нет
                var createDbCommand = new SqlCommand(
                    @"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'OnlineShopDB')
                      CREATE DATABASE OnlineShopDB",
                    connection);
                createDbCommand.ExecuteNonQuery();

                // Переключаемся на созданную базу
                connection.ChangeDatabase("OnlineShopDB");

                // Создаем таблицы
                CreateTables(connection);

                // Добавляем тестовые данные
                SeedTestData(connection);
            }
        }

        private void CreateTables(SqlConnection connection)
        {
            var commands = new[]
            {
                @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                  WHERE TABLE_NAME = 'Products')
                CREATE TABLE Products (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Price DECIMAL(18,2) NOT NULL,
                    Description NVARCHAR(500),
                    ImageUrl NVARCHAR(200),
                    StockQuantity INT NOT NULL DEFAULT 0
                )",

                @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                  WHERE TABLE_NAME = 'Orders')
                CREATE TABLE Orders (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    FullName NVARCHAR(100) NOT NULL,
                    Email NVARCHAR(100) NOT NULL,
                    DeliveryAddress NVARCHAR(200) NOT NULL,
                    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
                    TotalAmount DECIMAL(18,2) NOT NULL
                )",

                @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                  WHERE TABLE_NAME = 'OrderItems')
                CREATE TABLE OrderItems (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    OrderId INT NOT NULL,
                    ProductId INT NOT NULL,
                    Quantity INT NOT NULL,
                    Price DECIMAL(18,2) NOT NULL,
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                )"
            };

            foreach (var cmdText in commands)
            {
                using (var command = new SqlCommand(cmdText, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void SeedTestData(SqlConnection connection)
        {
            // Проверяем, есть ли уже данные
            using (var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Products", connection))
            {
                var count = (int)checkCommand.ExecuteScalar();
                if (count > 0) return;
            }

            var products = new[]
            {
                new Product { Name = "Ноутбук HP Pavilion", Price = 45999.99m, Description = "15.6 дюймов, Intel Core i5", ImageUrl = "https://via.placeholder.com/300x200", StockQuantity = 10 },
                new Product { Name = "Смартфон Samsung Galaxy", Price = 34999.99m, Description = "6.7 дюймов, 128GB, 5G", ImageUrl = "https://via.placeholder.com/300x200", StockQuantity = 15 },
                new Product { Name = "Наушники Sony", Price = 19999.99m, Description = "Беспроводные с шумоподавлением", ImageUrl = "https://via.placeholder.com/300x200", StockQuantity = 25 },
                new Product { Name = "Планшет Apple iPad Air", Price = 59999.99m, Description = "10.9 дюймов, 256GB, Wi-Fi", ImageUrl = "https://via.placeholder.com/300x200", StockQuantity = 8 },
                new Product { Name = "Умные часы Apple Watch", Price = 29999.99m, Description = "Series 8, 45mm, GPS", ImageUrl = "https://via.placeholder.com/300x200", StockQuantity = 12 },
                new Product { Name = "Фотоаппарат Canon EOS", Price = 79999.99m, Description = "Зеркальная камера, 24.2MP", ImageUrl = "https://via.placeholder.com/300x200", StockQuantity = 5 }
            };

            foreach (var product in products)
            {
                using (var command = new SqlCommand(
                    "INSERT INTO Products (Name, Price, Description, ImageUrl, StockQuantity) " +
                    "VALUES (@Name, @Price, @Description, @ImageUrl, @StockQuantity)",
                    connection))
                {
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Методы для работы с продуктами
        public List<Product> GetProducts()
        {
            var products = new List<Product>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM Products", connection))
            {
                connection.Open();
                connection.ChangeDatabase("OnlineShopDB");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"]?.ToString(),
                            ImageUrl = reader["ImageUrl"]?.ToString(),
                            StockQuantity = (int)reader["StockQuantity"]
                        });
                    }
                }
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM Products WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                connection.ChangeDatabase("OnlineShopDB");

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Product
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"]?.ToString(),
                            ImageUrl = reader["ImageUrl"]?.ToString(),
                            StockQuantity = (int)reader["StockQuantity"]
                        };
                    }
                }
            }

            return null;
        }

        // Методы для работы с заказами
        public int SaveOrder(Order order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.ChangeDatabase("OnlineShopDB");

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Сохраняем заказ
                        var orderCommand = new SqlCommand(
                            @"INSERT INTO Orders (FullName, Email, DeliveryAddress, OrderDate, TotalAmount) 
                              VALUES (@FullName, @Email, @DeliveryAddress, @OrderDate, @TotalAmount);
                              SELECT SCOPE_IDENTITY();",
                            connection, transaction);

                        orderCommand.Parameters.AddWithValue("@FullName", order.FullName);
                        orderCommand.Parameters.AddWithValue("@Email", order.Email);
                        orderCommand.Parameters.AddWithValue("@DeliveryAddress", order.DeliveryAddress);
                        orderCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        orderCommand.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);

                        var orderId = Convert.ToInt32(orderCommand.ExecuteScalar());

                        // Сохраняем элементы заказа
                        foreach (var item in order.OrderItems)
                        {
                            var itemCommand = new SqlCommand(
                                @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price) 
                                  VALUES (@OrderId, @ProductId, @Quantity, @Price)",
                                connection, transaction);

                            itemCommand.Parameters.AddWithValue("@OrderId", orderId);
                            itemCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                            itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            itemCommand.Parameters.AddWithValue("@Price", item.Price);

                            itemCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return orderId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<Order> GetOrders()
        {
            var orders = new List<Order>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                @"SELECT o.*, oi.*, p.Name as ProductName 
                  FROM Orders o
                  LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
                  LEFT JOIN Products p ON oi.ProductId = p.Id
                  ORDER BY o.OrderDate DESC",
                connection))
            {
                connection.Open();
                connection.ChangeDatabase("OnlineShopDB");

                using (var reader = command.ExecuteReader())
                {
                    Order currentOrder = null;

                    while (reader.Read())
                    {
                        var orderId = (int)reader["Id"];

                        if (currentOrder == null || currentOrder.Id != orderId)
                        {
                            currentOrder = new Order
                            {
                                Id = orderId,
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                DeliveryAddress = reader["DeliveryAddress"].ToString(),
                                OrderDate = (DateTime)reader["OrderDate"],
                                TotalAmount = (decimal)reader["TotalAmount"]
                            };
                            orders.Add(currentOrder);
                        }

                        // Добавляем элемент заказа, если он есть
                        if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                        {
                            currentOrder.OrderItems.Add(new OrderItem
                            {
                                Id = (int)reader["Id"],
                                OrderId = orderId,
                                ProductId = (int)reader["ProductId"],
                                Quantity = (int)reader["Quantity"],
                                Price = (decimal)reader["Price"]
                            });
                        }
                    }
                }
            }

            return orders;
        }
    }
}