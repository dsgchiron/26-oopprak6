using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace SerializationLab
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        public string SecretWarehouseCode { get; set; } = string.Empty;
    }

    public class Student
    {
        public string Name { get; set; } = string.Empty;
        public double AverageGrade { get; set; }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }
    }

    class Program
    {
        static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };

        static string lastSerializedProductJson = string.Empty;

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("\n=======================================================");
                Console.WriteLine("      ПРАКТИЧНА РОБОТА: СЕРІАЛІЗАЦІЯ JSON");
                Console.WriteLine("=======================================================");
                Console.WriteLine("1. Завдання 1 та 4: Серіалізувати Product (із JsonIgnore)");
                Console.WriteLine("2. Завдання 2: Десеріалізувати JSON товару та вивести властивості");
                Console.WriteLine("3. Завдання 3: Зберегти 5 студентів у students.json і знайти макс. бал");
                Console.WriteLine("4. Завдання 5: Зчитування JSON-файлу з обробленням винятків");
                Console.WriteLine("5. Завдання 6: Консольний «Каталог книг» (підменю)");
                Console.WriteLine("0. Вихід");
                Console.WriteLine("=======================================================");
                Console.Write("Оберіть пункт меню: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunTask1And4();
                        break;
                    case "2":
                        RunTask2();
                        break;
                    case "3":
                        RunTask3();
                        break;
                    case "4":
                        RunTask5();
                        break;
                    case "5":
                        RunTask6BookCatalog();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Введіть цифру від 0 до 5.");
                        break;
                }
            }
        }

        static void RunTask1And4()
        {
            Console.WriteLine("\n--- ЗАВДАННЯ 1 та 4: Серіалізація класу Product ---");

            var product = new Product
            {
                Id = 101,
                Name = "Механічна клавіатура",
                Price = 2499.99m,
                Quantity = 12,
                SecretWarehouseCode = "SEC-999-A"
            };

            Console.WriteLine($"Створено об'єкт у пам'яті. Секретний код: {product.SecretWarehouseCode}");

            lastSerializedProductJson = JsonSerializer.Serialize(product, jsonOptions);

            Console.WriteLine("\nОтриманий JSON:");
            Console.WriteLine(lastSerializedProductJson);

            File.WriteAllText("product.json", lastSerializedProductJson);
            Console.WriteLine("\n[Інфо] Збережено у файл 'product.json'.");
        }

        static void RunTask2()
        {
            Console.WriteLine("\n--- ЗАВДАННЯ 2: Десеріалізація JSON товару ---");

            if (string.IsNullOrEmpty(lastSerializedProductJson))
            {
                Console.WriteLine("Виконуємо Завдання 1 для отримання JSON...");
                RunTask1And4();
            }

            Product? restoredProduct = JsonSerializer.Deserialize<Product>(lastSerializedProductJson);

            if (restoredProduct != null)
            {
                Console.WriteLine("\nВластивості відновленого об'єкта Product:");
                Console.WriteLine($"- ID:        {restoredProduct.Id}");
                Console.WriteLine($"- Назва:     {restoredProduct.Name}");
                Console.WriteLine($"- Ціна:      {restoredProduct.Price} грн");
                Console.WriteLine($"- Кількість: {restoredProduct.Quantity} шт.");
                Console.WriteLine($"- Секретний код: '{restoredProduct.SecretWarehouseCode}'");
            }
        }

        static void RunTask3()
        {
            Console.WriteLine("\n--- ЗАВДАННЯ 3: Список студентів (students.json) ---");

            var students = new List<Student>
            {
                new Student { Name = "Олександр Коваленко", AverageGrade = 85.5 },
                new Student { Name = "Марія Шевченко", AverageGrade = 95.8 },
                new Student { Name = "Дмитро Бондаренко", AverageGrade = 78.2 },
                new Student { Name = "Анна Мельник", AverageGrade = 91.4 },
                new Student { Name = "Іван Ткаченко", AverageGrade = 88.0 }
            };

            string fileName = "students.json";

            string json = JsonSerializer.Serialize(students, jsonOptions);
            File.WriteAllText(fileName, json);
            Console.WriteLine($"Список студентів збережено у файл '{fileName}'.");

            string fileContent = File.ReadAllText(fileName);
            List<Student>? restoredStudents = JsonSerializer.Deserialize<List<Student>>(fileContent);

            if (restoredStudents != null && restoredStudents.Count > 0)
            {
                Console.WriteLine("\nВідновлений список студентів:");
                foreach (var s in restoredStudents)
                {
                    Console.WriteLine($"- {s.Name}: {s.AverageGrade}");
                }

                Student bestStudent = restoredStudents.OrderByDescending(s => s.AverageGrade).First();
                Console.WriteLine($"\n>>> Найвищий середній бал: {bestStudent.AverageGrade} ({bestStudent.Name})");
            }
        }

        static void RunTask5()
        {
            Console.WriteLine("\n--- ЗАВДАННЯ 5: Зчитування JSON з обробленням винятків ---");
            Console.WriteLine("1. Зчитати існуючий файл 'students.json'");
            Console.WriteLine("2. Зчитати неіснуючий файл 'missing.json'");
            Console.WriteLine("3. Зчитати пошкоджений файл 'broken.json'");
            Console.Write("Ваш вибір: ");

            string? subChoice = Console.ReadLine();
            string targetFile = "students.json";

            if (subChoice == "2")
            {
                targetFile = "missing.json";
            }
            else if (subChoice == "3")
            {
                targetFile = "broken.json";
                File.WriteAllText(targetFile, "{ \"Name\": \"Студент\", НЕКОРЕКТНИЙ_СИНТАКСИС ");
            }
            else if (!File.Exists("students.json"))
            {
                RunTask3();
            }

            try
            {
                Console.WriteLine($"\nСпроба зчитати файл '{targetFile}'...");
                string content = File.ReadAllText(targetFile);
                var data = JsonSerializer.Deserialize<List<Student>>(content);
                Console.WriteLine($"[УСПІХ] Файл коректно зчитано! Знайдено записів: {data?.Count ?? 0}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"[ПОМИЛКА FileNotFoundException] Файл '{targetFile}' не знайдено!");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[ПОМИЛКА JsonException] Файл містить некоректний JSON!");
                Console.WriteLine($"Деталі: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОМИЛКА] {ex.Message}");
            }
        }

        static void RunTask6BookCatalog()
        {
            string booksFile = "books.json";
            List<Book> catalog = LoadBooks(booksFile);

            while (true)
            {
                Console.WriteLine("\n--- ЗАВДАННЯ 6: КАТАЛОГ КНИГ ---");
                Console.WriteLine("1. Показати всі книги");
                Console.WriteLine("2. Додати нову книгу");
                Console.WriteLine("3. Пошук книги за назвою або автором");
                Console.WriteLine("4. Видалити книгу за ID");
                Console.WriteLine("0. Повернутися до головного меню");
                Console.Write("Оберіть дію: ");

                string? action = Console.ReadLine();

                switch (action)
                {
                    case "1":
                        ShowBooks(catalog);
                        break;
                    case "2":
                        AddBook(catalog, booksFile);
                        break;
                    case "3":
                        SearchBooks(catalog);
                        break;
                    case "4":
                        DeleteBook(catalog, booksFile);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірна команда.");
                        break;
                }
            }
        }

        static List<Book> LoadBooks(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var initialBooks = new List<Book>
                {
                    new Book { Id = 1, Title = "Тигролови", Author = "Іван Багряний", Year = 1944 },
                    new Book { Id = 2, Title = "Тіні забутих предків", Author = "Михайло Коцюбинський", Year = 1911 }
                };
                SaveBooks(initialBooks, filePath);
                return initialBooks;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка читання] {ex.Message}");
                return new List<Book>();
            }
        }

        static void SaveBooks(List<Book> books, string filePath)
        {
            string json = JsonSerializer.Serialize(books, jsonOptions);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"[Збережено у '{filePath}']");
        }

        static void ShowBooks(List<Book> books)
        {
            if (books.Count == 0)
            {
                Console.WriteLine("\nКаталог порожній.");
                return;
            }

            Console.WriteLine("\nСписок книг:");
            foreach (var b in books)
            {
                Console.WriteLine($"  ID: {b.Id} | \"{b.Title}\" — {b.Author} ({b.Year} р.)");
            }
        }

        static void AddBook(List<Book> books, string filePath)
        {
            int nextId = books.Count > 0 ? books.Max(b => b.Id) + 1 : 1;

            Console.Write("Введіть назву книги: ");
            string title = Console.ReadLine() ?? "Без назви";

            Console.Write("Введіть автора: ");
            string author = Console.ReadLine() ?? "Невідомий автор";

            Console.Write("Введіть рік видання: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                year = DateTime.Now.Year;
            }

            books.Add(new Book { Id = nextId, Title = title, Author = author, Year = year });
            Console.WriteLine("Книгу додано!");
            SaveBooks(books, filePath);
        }

        static void SearchBooks(List<Book> books)
        {
            Console.Write("Введіть текст для пошуку: ");
            string query = (Console.ReadLine() ?? "").ToLower();

            var results = books.Where(b =>
                b.Title.ToLower().Contains(query) ||
                b.Author.ToLower().Contains(query)).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("Нічого не знайдено.");
            }
            else
            {
                Console.WriteLine($"\nЗнайдено ({results.Count}):");
                ShowBooks(results);
            }
        }

        static void DeleteBook(List<Book> books, string filePath)
        {
            ShowBooks(books);
            if (books.Count == 0) return;

            Console.Write("\nВведіть ID книги для видалення: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var bookToRemove = books.FirstOrDefault(b => b.Id == id);
                if (bookToRemove != null)
                {
                    books.Remove(bookToRemove);
                    Console.WriteLine($"Книгу \"{bookToRemove.Title}\" видалено.");
                    SaveBooks(books, filePath);
                }
                else
                {
                    Console.WriteLine("Книгу з таким ID не знайдено.");
                }
            }
            else
            {
                Console.WriteLine("Некоректний ID.");
            }
        }
    }
}