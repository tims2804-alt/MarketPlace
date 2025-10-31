using System;
using System.Linq;
using MarketPlace2.Context;
using MarketPlace2.Entities;



namespace MarketPlace2
{

    class Program
    {
        static User currentUser = null;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            using (var db = new MyDbContext())
            {
                Console.WriteLine("--- Добро пожаловать в Интернет магазин ---");

                while (true)
                {
                    if (currentUser == null)
                    {
                        Console.WriteLine("\n1)Регистрация\n2)Вход\n3)Просмотр товаров\n0)Выход");
                        Console.Write("Выберите действие: ");
                        var choice = Console.ReadLine();

                        switch (choice)
                        {
                            case "1":
                                Register(db);
                                break;
                            case "2":
                                Login(db);
                                break;
                            case "3":
                                ViewProducts(db);
                                break;
                            case "0":
                                return;
                            default:
                                Console.WriteLine("Неверный ввод");
                                break;

                        }
                    }
                    else
                    {
                        Console.WriteLine($"\n Аккаунт: {currentUser.Username}");
                        Console.WriteLine("1) Просмотр товаров");
                        Console.WriteLine("2) Добавить товар в корзину");
                        Console.WriteLine("3) Посмотреть корзину");
                        Console.WriteLine("4) Выйти из аккаунта");
                        Console.WriteLine("0) Выход из программы");
                        Console.Write("Выберите действие: ");
                        var choice = Console.ReadLine();
                        switch (choice)
                        {
                            case "1":
                                ViewProducts(db);
                                break;
                            case "2":
                                AddToCart(db);
                                break;
                            case "3":
                                ViewCart(db);
                                break;
                            case "4":
                                LogOut(db);
                                break;
                            case "0":
                                return;
                            default:
                                Console.WriteLine("Неверный ввод");
                                break;
                            
                        }
                    }
                }

                static void Register(MyDbContext db)
                {
                    Console.WriteLine("\n--- Регистрация ---");
                    Console.Write("Введите имя пользователя: ");
                    string userName = Console.ReadLine();
                    Console.Write("Введите Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Введите пароль: ");
                    string password = Console.ReadLine();
                    
                    if (!email.Contains("@") || !email.Contains("."))
                    {
                        Console.WriteLine("Некорректный email!");
                        return;
                    }

                    if (db.Users.Any(u => u.Username == userName))
                    {
                        Console.WriteLine("Пользователь с таким именем уже существует!!!");
                    }

                    if (db.Users.Any(u => u.Email == email))
                    {
                        Console.WriteLine("Пользователь с таким email уже существует!!!");
                    }

                    if (password.Length < 4)
                    {
                        Console.WriteLine("Пароль должен быть не короче 4 символов!!!");
                        return;
                    }

                    Console.Write("Повторите пароль: ");
                    string confirmPassword = Console.ReadLine();
                    if (confirmPassword != password)
                    {
                        Console.WriteLine("Пароли не совпадают!!!");
                    }

                    var user = new User
                    {
                        Username = userName,
                        Email = email,
                        PasswordHash = password,
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    Console.WriteLine("Регистрация успешна! Теперь войдите в систему");

                }

                static void Login(MyDbContext db)
                {
                    Console.WriteLine("\n--- Вход ---");
                    Console.Write("Введите имя пользователя: ");
                    string userName = Console.ReadLine();
                    Console.Write("Введите пароль: ");
                    string password = Console.ReadLine();

                    var user = db.Users.FirstOrDefault(u => u.Username == userName && u.PasswordHash == password);

                    if (user == null)
                    {
                        Console.WriteLine("Неверное имя пользователя или пароль!!!");
                        return;
                    }

                    currentUser = user;
                    Console.WriteLine($"Добро пожаловать, {currentUser.Username}");


                }

                static void ViewProducts(MyDbContext db)
                {
                    Console.Clear();
                    Console.WriteLine("=== 📦 Список всех товаров ===");

                    Console.WriteLine("\nВыберите сортировку:");
                    Console.WriteLine("1) По цене (возрастание)");
                    Console.WriteLine("2) По цене (убывание)");
                    Console.WriteLine("3) По названию (A–Z)");
                    Console.WriteLine("4) Без сортировки");
                    Console.Write("➡ Ваш выбор: ");

                    string choice = Console.ReadLine();

                    var products = db.Products.AsQueryable();

                    switch (choice)
                    {
                        case "1":
                            products = products.OrderBy(p => p.Price);
                            break;
                        case "2":
                            products = products.OrderByDescending(p => p.Price);
                            break;
                        case "3":
                            products = products.OrderBy(p => p.Name);
                            break;
                        default:
                            break;
                    }

                    Console.WriteLine("\n📋 Доступные товары:\n");
                    
                    if (!products.Any())
                    {
                        Console.WriteLine("Товары отсутствуют!!!");
                        return;
                    }

                    foreach (var p in products)
                    {
                        Console.WriteLine($"ID: {p.ProductId} | {p.Name} — {p.Price}₽");
                        Console.WriteLine($"Описание: {p.Description}");

                        if (p.StockQuantity < 1000)
                            Console.WriteLine($"Остаток: {p.StockQuantity} шт.");
                        else
                            Console.WriteLine("В наличии!!!");

                        Console.WriteLine(); // Пустая строка для красоты
                    }
                }


                static void LogOut(MyDbContext db)
                {
                    Console.WriteLine($"До свидания, {currentUser.Username} ");
                    currentUser = null;
                } 
                
                static void AddToCart(MyDbContext db)
                {
                    ViewProducts(db); // Показываем список товаров

                    Console.WriteLine("\nВведите ID продукта, который хотите добавить в корзину:");
                    if (!int.TryParse(Console.ReadLine(), out int productId))
                    {
                        Console.WriteLine("Некорректный ID!");
                        return;
                    }

                    var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
                    if (product == null)
                    {
                        Console.WriteLine("Товар не найден!");
                        return;
                    }

                    Console.WriteLine($"\n Вы выбрали: {product.Name}");
                    Console.WriteLine($"Цена: {product.Price}₽");
                    Console.WriteLine($"Доступно на складе: {product.StockQuantity} шт.\n");

                    Console.Write("Введите количество для добавления в корзину: ");
                    if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Количество должно быть больше 0!");
                        return;
                    }

                    if (quantity > product.StockQuantity)
                    {
                        Console.WriteLine(" Недостаточно товара на складе!");
                        return;
                    }

                    var existingItem = db.CartItems.FirstOrDefault(
                        c => c.CartId == currentUser.UserId && c.ProductId == productId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                        Console.WriteLine($" Количество товара '{product.Name}' увеличено до {existingItem.Quantity} шт. в корзине.");
                    }
                    else
                    {
                        var cartItem = new CartItem
                        {
                            CartId = currentUser.UserId,
                            ProductId = productId,
                            Quantity = quantity
                        };

                        db.CartItems.Add(cartItem);
                        Console.WriteLine($" Товар '{product.Name}' добавлен в корзину ({quantity} шт.)!");
                    }

                    db.SaveChanges();
                }

                static void ViewCart(MyDbContext db)
                {
                    Console.WriteLine("\n--- 🛒 Ваша корзина ---");

                    var cartItems = db.CartItems
                        .Where(c => c.CartId == currentUser.UserId)
                        .ToList();

                    if (!cartItems.Any())
                    {
                        Console.WriteLine("Корзина пуста!");
                        return;
                    }

                    decimal total = 0;

                    foreach (var item in cartItems)
                    {
                        var product = db.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                        if (product != null)
                        {
                            decimal itemTotal = product.Price * item.Quantity;
                            total += itemTotal;

                            Console.WriteLine($"ID: {product.ProductId} | {product.Name}");
                            Console.WriteLine($"Цена: {product.Price}₽ | Кол-во: {item.Quantity} | Сумма: {itemTotal}₽");
                            Console.WriteLine("-----------------------------------");
                        }
                    }

                    Console.WriteLine($" Общая сумма: {total}₽");
                    Console.WriteLine("\n1) Оформить заказ");
                    Console.WriteLine("2) Удалить товар из корзины");
                    Console.WriteLine("0) Вернуться в меню");
                    Console.Write("Выберите действие: ");
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            PlaceOrder(db, cartItems);
                            break;
                        case "2":
                            RemoveFromCart(db);
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Неверный ввод!");
                            break;
                        }
                    }

                    static void PlaceOrder(MyDbContext db, List<CartItem> cartItems)
                    {
                        bool allAvailable = true;

                        foreach (var item in cartItems)
                        {
                            var product = db.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                            if (product == null || product.StockQuantity < item.Quantity)
                            {
                                Console.WriteLine($" Недостаточно товара '{product?.Name ?? "Не найден"}' на складе!");
                                allAvailable = false;
                            }
                        }

                        if (!allAvailable)
                        {
                            Console.WriteLine(" Заказ не оформлен. Проверьте наличие товаров!");
                            return;
                        }

                        foreach (var item in cartItems)
                        {
                            var product = db.Products.First(p => p.ProductId == item.ProductId);
                            product.StockQuantity -= item.Quantity; // уменьшаем остаток

                            db.CartItems.Remove(item); // очищаем корзину
                        }

                        db.SaveChanges();

                        Console.WriteLine(" Заказ успешно оформлен!");
                    }
                    
                    static void RemoveFromCart(MyDbContext db)
                    {
                        Console.Write("\nВведите ID товара, который хотите удалить из корзины: ");
                        if (!int.TryParse(Console.ReadLine(), out int productId))
                        {
                            Console.WriteLine("Некорректный ввод!");
                            return;
                        }

                        var cartItem = db.CartItems.FirstOrDefault(
                            c => c.CartId == currentUser.UserId && c.ProductId == productId);

                        if (cartItem == null)
                        {
                            Console.WriteLine(" Такого товара нет в корзине!");
                            return;
                        }

                        Console.WriteLine($"В корзине {cartItem.Quantity} шт. этого товара.");
                        Console.Write("Введите количество, которое хотите удалить: ");
                        if (!int.TryParse(Console.ReadLine(), out int removeQuantity) || removeQuantity <= 0)
                        {
                            Console.WriteLine("Количество должно быть больше 0!");
                            return;
                        }

                        if (removeQuantity >= cartItem.Quantity)
                        {
                            db.CartItems.Remove(cartItem);
                            Console.WriteLine("🗑 Товар полностью удалён из корзины!");
                        }
                        else
                        {
                            cartItem.Quantity -= removeQuantity;
                            Console.WriteLine($" Из корзины удалено {removeQuantity} шт. Товара осталось: {cartItem.Quantity} шт.");
                        }

                        db.SaveChanges();
                }
            }
        }
    }
}




