using DeliveryConsole.Exceptions;

namespace DeliveryConsole.Models;

public abstract class DeliveryMethod : IComparable<DeliveryMethod>
{
    private string _name = string.Empty;
    private double _ratePerKg;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва способу доставки не може бути порожньою.");

            _name = value.Trim();
        }
    }

    public double RatePerKg
    {
        get => _ratePerKg;
        set
        {
            if (!double.IsFinite(value) || value < 0)
                throw new InvalidDeliveryDataException("Ставка за кг не може бути від'ємною.");

            _ratePerKg = value;
        }
    }

    // Контрольна вартість доставки посилки вагою 1 кг.
    public double ReferenceCost => CalculateCost(1);

    protected DeliveryMethod(string name, double ratePerKg)
    {
        Name = name;
        RatePerKg = ratePerKg;
    }

    public abstract double CalculateCost(double weightKg);

    public virtual string GetDescription()
    {
        return $"{Name} | ставка: {RatePerKg:F2} грн/кг";
    }

    public int CompareTo(DeliveryMethod? other)
    {
        if (other is null)
            return 1;

        return ReferenceCost.CompareTo(other.ReferenceCost);
    }

    protected static void ValidateWeight(double weightKg)
    {
        // Нульова вага дозволена за умовою варіанта.
        if (!double.IsFinite(weightKg) || weightKg < 0)
            throw new InvalidDeliveryDataException("Вага посилки не може бути від'ємною.");
    }

    protected static double ValidateCost(double cost)
    {
        if (!double.IsFinite(cost))
            throw new InvalidDeliveryDataException("Результат обчислення виходить за допустимий діапазон.");

        return cost;
    }
}
