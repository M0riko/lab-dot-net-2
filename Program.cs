using System.Globalization;
using System.Text;
using DeliveryConsole.Exceptions;
using DeliveryConsole.Models;

namespace DeliveryConsole;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        List<DeliveryMethod> methods = new()
        {
            new StandardDelivery("До відділення", 30),
            new StandardDelivery("Економ-доставка", 20),
            new ExpressDelivery("Експрес-кур'єр", 40, 1.5, 50)
        };

        bool isRunning = true;

        while (isRunning)
        {
            ShowMenu();
            int choice = ReadInt("Ваш вибір: ", 0, 4);

            switch (choice)
            {
                case 1:
                    AddDeliveryMethod(methods);
                    break;
                case 2:
                    CalculateDeliveryCost(methods);
                    break;
                case 3:
                    ShowDeliveryMethods(methods);
                    break;
                case 4:
                    SortDeliveryMethods(methods);
                    break;
                case 0:
                    isRunning = false;
                    break;
            }
        }

        Console.WriteLine("Роботу програми завершено.");
    }

    static void ShowMenu()
    {
        Console.WriteLine("\n========== СПОСОБИ ДОСТАВКИ ==========");
        Console.WriteLine("1. Додати спосіб доставки");
        Console.WriteLine("2. Розрахувати вартість доставки");
        Console.WriteLine("3. Показати всі способи доставки");
        Console.WriteLine("4. Сортувати за вартістю для 1 кг");
        Console.WriteLine("0. Вихід");
    }

    static void AddDeliveryMethod(List<DeliveryMethod> methods)
    {
        Console.WriteLine("\n1. Стандартна доставка");
        Console.WriteLine("2. Експрес-доставка");
        int type = ReadInt("Оберіть тип доставки: ", 1, 2);

        Console.Write("Назва способу доставки: ");
        string name = Console.ReadLine() ?? "";
        double rate = ReadDouble("Ставка за 1 кг (грн): ");

        try
        {
            DeliveryMethod method;

            if (type == 1)
            {
                method = new StandardDelivery(name, rate);
            }
            else
            {
                double multiplier = ReadDouble("Коефіцієнт швидкості: ");
                double surcharge = ReadDouble("Надбавка (грн): ");
                method = new ExpressDelivery(name, rate, multiplier, surcharge);
            }

            methods.Add(method);
            Console.WriteLine($"Спосіб доставки «{method.Name}» успішно додано.");
        }
        catch (InvalidDeliveryDataException ex)
        {
            Console.WriteLine($"Помилка даних: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Помилка аргументу: {ex.Message}");
        }
    }

    static void CalculateDeliveryCost(List<DeliveryMethod> methods)
    {
        if (methods.Count == 0)
        {
            Console.WriteLine("Немає способів доставки для розрахунку.");
            return;
        }

        ShowDeliveryMethods(methods);
        int index = ReadInt("Номер способу доставки: ", 1, methods.Count) - 1;
        double weight = ReadDouble("Вага посилки (кг): ");

        try
        {
            DeliveryMethod method = methods[index];
            double cost = method.CalculateCost(weight);
            Console.WriteLine($"\nСпосіб: {method.Name}");
            Console.WriteLine($"Вага: {weight:F2} кг");
            Console.WriteLine($"Вартість доставки: {cost:F2} грн");
        }
        catch (InvalidDeliveryDataException ex)
        {
            Console.WriteLine($"Помилка даних: {ex.Message}");
        }
    }

    static void ShowDeliveryMethods(List<DeliveryMethod> methods)
    {
        Console.WriteLine("\n--- ДОВІДНИК СПОСОБІВ ДОСТАВКИ ---");

        if (methods.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return;
        }

        int number = 1;
        foreach (DeliveryMethod method in methods)
        {
            // Поліморфізм: викликається override відповідного похідного класу.
            Console.WriteLine($"{number++}. {method.GetDescription()}");
        }
    }

    static void SortDeliveryMethods(List<DeliveryMethod> methods)
    {
        if (methods.Count == 0)
        {
            Console.WriteLine("Немає даних для сортування.");
            return;
        }

        methods.Sort(); // Використовує IComparable<DeliveryMethod>.
        Console.WriteLine("\nСпособи відсортовано за вартістю доставки 1 кг (за зростанням).");
        ShowDeliveryMethods(methods);
    }

    static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Некоректне введення. Вкажіть ціле число від {min} до {max}.");
        }
    }

    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim().Replace(',', '.');

            if (double.TryParse(input, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out double value) && double.IsFinite(value))
                return value;

            Console.WriteLine("Некоректне введення. Вкажіть число (наприклад, 2,5 або 2.5).");
        }
    }
}
