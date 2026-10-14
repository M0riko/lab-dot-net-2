
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

// Власний клас винятку
class InvalidDeliveryException : Exception
{
    public InvalidDeliveryException() { }

    public InvalidDeliveryException(string message) : base(message) { }

    public InvalidDeliveryException(string message, Exception inner)
        : base(message, inner) { }
}

// Абстрактний базовий клас
abstract class DeliveryMethod : IComparable<DeliveryMethod>
{
    private string _name = "";
    private double _ratePerKg;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва не може бути порожньою.");

            _name = value.Trim();
        }
    }

    public double RatePerKg
    {
        get => _ratePerKg;
        set
        {
            if (!double.IsFinite(value) || value < 0)
                throw new InvalidDeliveryException("Ставка не може бути від'ємною.");

            _ratePerKg = value;
        }
    }

    protected DeliveryMethod(string name, double ratePerKg)
    {
        Name = name;
        RatePerKg = ratePerKg;
    }

    protected static void CheckWeight(double weight)
    {
        if (!double.IsFinite(weight) || weight < 0)
            throw new InvalidDeliveryException("Вага не може бути від'ємною.");
    }

    public abstract double CalculateCost(double weightKg);

    public double ReferenceCost => CalculateCost(1);

    public int CompareTo(DeliveryMethod? other)
    {
        if (other == null)
            return 1;

        return ReferenceCost.CompareTo(other.ReferenceCost);
    }

    public virtual void ShowInfo()
    {
        Console.WriteLine($"Назва: {Name}");
        Console.WriteLine($"Ставка за кг: {RatePerKg:F2} грн");
        Console.WriteLine($"Вартість 1 кг: {ReferenceCost:F2} грн");
    }
}

// Стандартна доставка
class StandardDelivery : DeliveryMethod
{
    public StandardDelivery(string name, double ratePerKg)
        : base(name, ratePerKg) { }

    public override double CalculateCost(double weightKg)
    {
        CheckWeight(weightKg);
        return weightKg * RatePerKg;
    }

    public override void ShowInfo()
    {
        Console.WriteLine("Тип: Стандартна доставка");
        base.ShowInfo();
    }
}

// Експрес-доставка
class ExpressDelivery : DeliveryMethod
{
    private double _speedMultiplier;
    private double _surcharge;

    public double SpeedMultiplier
    {
        get => _speedMultiplier;
        set
        {
            if (!double.IsFinite(value) || value <= 0)
                throw new InvalidDeliveryException("Коефіцієнт має бути більшим за 0.");

            _speedMultiplier = value;
        }
    }

    public double Surcharge
    {
        get => _surcharge;
        set
        {
            if (!double.IsFinite(value) || value < 0)
                throw new InvalidDeliveryException("Надбавка не може бути від'ємною.");

            _surcharge = value;
        }
    }

    public ExpressDelivery(string name, double rate, double multiplier, double surcharge)
        : base(name, rate)
    {
        SpeedMultiplier = multiplier;
        Surcharge = surcharge;
    }

    public override double CalculateCost(double weightKg)
    {
        CheckWeight(weightKg);

        return weightKg * RatePerKg * SpeedMultiplier + Surcharge;
    }

    public override void ShowInfo()
    {
        Console.WriteLine("Тип: Експрес-доставка");
        base.ShowInfo();
        Console.WriteLine($"Коефіцієнт швидкості: {SpeedMultiplier:F2}");
        Console.WriteLine($"Надбавка: {Surcharge:F2} грн");
    }
}

// Головний клас програми
class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        List<DeliveryMethod> deliveries = new()
        {
            new StandardDelivery("Звичайна доставка", 30),
            new StandardDelivery("Економ доставка", 20),
            new ExpressDelivery("Швидка доставка", 40, 1.5, 50)
        };

        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== МЕНЮ ==========");
            Console.WriteLine("1. Додати спосіб доставки");
            Console.WriteLine("2. Розрахувати вартість доставки");
            Console.WriteLine("3. Показати всі способи доставки");
            Console.WriteLine("4. Сортувати за вартістю");
            Console.WriteLine("0. Вихід");
            Console.WriteLine("==========================");

            int choice = ReadInt("Ваш вибір: ", 0, 4);

            switch (choice)
            {
                case 1:
                    AddDelivery(deliveries);
                    break;

                case 2:
                    CalculateDelivery(deliveries);
                    break;

                case 3:
                    ShowDeliveries(deliveries);
                    break;

                case 4:
                    SortDeliveries(deliveries);
                    break;

                case 0:
                    running = false;
                    Console.WriteLine("Програму завершено.");
                    break;
            }
        }
    }

    // Додавання способу доставки
    static void AddDelivery(List<DeliveryMethod> deliveries)
    {
        Console.WriteLine("\n1. Стандартна доставка");
        Console.WriteLine("2. Експрес-доставка");

        int type = ReadInt("Оберіть тип: ", 1, 2);

        Console.Write("Введіть назву: ");
        string name = Console.ReadLine() ?? "";

        double rate = ReadDouble("Ставка за кг: ");

        try
        {
            DeliveryMethod delivery;

            if (type == 1)
            {
                delivery = new StandardDelivery(name, rate);
            }
            else
            {
                double multiplier = ReadDouble("Коефіцієнт швидкості: ");
                double surcharge = ReadDouble("Надбавка: ");

                delivery = new ExpressDelivery(name, rate, multiplier, surcharge);
            }

            deliveries.Add(delivery);
            Console.WriteLine("Спосіб доставки успішно додано!");
        }
        catch (InvalidDeliveryException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    // Розрахунок вартості
    static void CalculateDelivery(List<DeliveryMethod> deliveries)
    {
        if (deliveries.Count == 0)
        {
            Console.WriteLine("Немає доступних способів доставки.");
            return;
        }

        ShowDeliveries(deliveries);

        int number = ReadInt("Оберіть номер доставки: ", 1, deliveries.Count);
        double weight = ReadDouble("Введіть вагу посилки (кг): ");

        try
        {
            DeliveryMethod delivery = deliveries[number - 1];
            double cost = delivery.CalculateCost(weight);

            Console.WriteLine("\n===== РЕЗУЛЬТАТ =====");
            Console.WriteLine($"Спосіб: {delivery.Name}");
            Console.WriteLine($"Вага: {weight:F2} кг");
            Console.WriteLine($"Вартість: {cost:F2} грн");
        }
        catch (InvalidDeliveryException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    // Перегляд способів доставки
    static void ShowDeliveries(List<DeliveryMethod> deliveries)
    {
        if (deliveries.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return;
        }

        Console.WriteLine("\n===== СПОСОБИ ДОСТАВКИ =====");

        for (int i = 0; i < deliveries.Count; i++)
        {
            Console.WriteLine($"\nСпосіб №{i + 1}");
            deliveries[i].ShowInfo();
        }
    }

    // Сортування за вартістю доставки 1 кг
    static void SortDeliveries(List<DeliveryMethod> deliveries)
    {
        if (deliveries.Count == 0)
        {
            Console.WriteLine("Немає даних для сортування.");
            return;
        }

        deliveries.Sort();

        Console.WriteLine("\nСпособи доставки відсортовано!");
        ShowDeliveries(deliveries);
    }

    // Введення цілого числа
    static int ReadInt(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(), out int number)
                && number >= min && number <= max)
            {
                return number;
            }

            Console.WriteLine($"Помилка! Введіть число від {min} до {max}.");
        }
    }

    // Введення дробового числа
    static double ReadDouble(string message)
    {
        while (true)
        {
            Console.Write(message);

            string input = (Console.ReadLine() ?? "").Replace(',', '.');

            if (double.TryParse(input, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out double number)
                && double.IsFinite(number))
            {
                return number;
            }

            Console.WriteLine("Помилка! Введіть коректне число.");
        }
    }
}
