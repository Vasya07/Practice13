using OnlineShop.Models;
using System.Linq;
using System.Windows;

namespace OnlineShop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Проверяем и создаем базу данных
            using (var context = new DatabaseContext())
            {
                context.Database.CreateIfNotExists();

                // Добавляем тестовые данные, если таблица пуста
                if (!context.Products.Any())
                {
                    context.Products.AddRange(new[]
                    {
                        new Product
                        {
                            Name = "Ноутбук HP Pavilion",
                            Price = 45999.99m,
                            Description = "15.6 дюймов, Intel Core i5, 8GB RAM, 512GB SSD",
                            ImageUrl = "https://via.placeholder.com/300x200.png?text=Ноутбук",
                            StockQuantity = 10
                        },
                        new Product
                        {
                            Name = "Смартфон Samsung Galaxy",
                            Price = 34999.99m,
                            Description = "6.7 дюймов, 128GB, 5G, черный",
                            ImageUrl = "https://via.placeholder.com/300x200.png?text=Смартфон",
                            StockQuantity = 15
                        },
                        new Product
                        {
                            Name = "Наушники Sony",
                            Price = 19999.99m,
                            Description = "Беспроводные с шумоподавлением",
                            ImageUrl = "https://via.placeholder.com/300x200.png?text=Наушники",
                            StockQuantity = 25
                        }
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}