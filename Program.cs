// https://learn.microsoft.com/ru-ru/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace?view=sql-server-ver16
using Microsoft.Data.SqlClient; // новий пакет для підключення до SQL Server в .NET 9-10, раніше називався System.Data.SqlClient 
// !!! dotnet add package Microsoft.Data.SqlClient
// скрипт БД: https://gist.github.com/sunmeat/59dc33337af869024a7b18602b556b00

/*
// PostgreSQL (найпопулярніший вибір для відкритих проєктів)
/// !!! dotnet add package Npgsql
using Npgsql;

// MySQL / MariaDB (сучасний та швидкий варіант)
/// !!! dotnet add package MySqlConnector
using MySqlConnector;

// SQLite (вбудована, файлова база, дуже зручна для локальних додатків)
/// !!! dotnet add package Microsoft.Data.Sqlite
using Microsoft.Data.Sqlite;

// Oracle Database (офіційний managed driver, без встановлення клієнта)
/// !!! dotnet add package Oracle.ManagedDataAccess
using Oracle.ManagedDataAccess.Client;

// Firebird (рідше, але стабільний варіант)
/// !!! dotnet add package FirebirdSql.Data.FirebirdClient
using FirebirdSql.Data.FirebirdClient;
*/

class Program
{
    static void Main()
    {
        Console.Title = "Підключення до БД";
        Console.OutputEncoding = System.Text.Encoding.UTF8; // кирилиця, в БД все українською

        // рядок підключення до БД, база має існувати!
        string connectionString = "Server=localhost; Database=Store; Integrated Security=True; TrustServerCertificate=True;";

        // SQL-запит для вибірки даних з таблиці Product
        string query = "SELECT id, name, price FROM Product"; // в ADO.NET запити пишуться руками :(

        // створення підключення
        using (var connection = new SqlConnection(connectionString))
        {
            try
            {
                // відкриття з'єднання з БД
                connection.Open();

                // створення команди для виконання SQL-запиту
                using (var command = new SqlCommand(query, connection))
                {
                    // виконання команди та читання даних за допомогою SqlDataReader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // перевірка, чи є взагалі дані
                        if (reader.HasRows)
                        {
                            // читання рядків даних та виведення їх на екран
                            while (reader.Read())
                            {
                                // читання значень поточного рядка
                                int id = reader.GetInt32(0); // читання id
                                string name = reader.GetString(1); // читання name
                                double price = reader.GetDouble(2); // читання price

                                Console.WriteLine($"ID: {id}, Назва товару: {name}, Ціна: {price}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Нема даних в таблиці.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
            }
        }

        Console.ReadLine();
    }
}

/*
У цьому прикладі з'єднання з базою даних працює в режимі підключення (або "connected" mode):
- У коді рядок підключення Integrated Security=True вказує на використання Windows Authentication,
що передбачає створення постійного з'єднання із сервером для виконання запитів. Це означає, що запити
виконуються на сервері в приєднаному режимі, де запити виконуються безпосередньо до бази даних,
а не через локальні кешовані дані або "вимкнений" режим.
- Використовуються методи, які виконують запити безпосередньо до бази даних, наприклад, ExecuteReader().
Це типова поведінка для приєднаного режиму, коли всі дані виходять "на льоту" і необхідно підтримувати
постійне з'єднання із сервером.
- У коді використовується конструкція using, яка автоматично керує підключенням. Це також є
характерним для "приєднаного" режиму, тому що кожне підключення відкривається та закривається в рамках
одного запиту, а не зберігається протягом усієї сесії.

Приєднаний режим:
- Відкриває з'єднання з базою даних.
- Запити виконуються у реальному часі.
- Відповіді надходять із сервера одразу.
- Потрібне постійне підключення до сервера.

Вимкнений режим:
– Спочатку витягуються дані у пам'ять, а потім з ними можна працювати локально.
- Не потрібне постійне підключення.
- Використовується, наприклад, DataSet чи DataTable.
*/