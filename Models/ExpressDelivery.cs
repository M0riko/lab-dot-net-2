using DeliveryConsole.Exceptions;

namespace DeliveryConsole.Models;

public class ExpressDelivery : DeliveryMethod
{
    private double _speedMultiplier;
    private double _surcharge;

    public double SpeedMultiplier
    {
        get => _speedMultiplier;
        set
        {
            if (!double.IsFinite(value) || value <= 0)
                throw new InvalidDeliveryDataException("Коефіцієнт швидкості має бути більшим за нуль.");

            _speedMultiplier = value;
        }
    }

    public double Surcharge
    {
        get => _surcharge;
        set
        {
            if (!double.IsFinite(value) || value < 0)
                throw new InvalidDeliveryDataException("Надбавка не може бути від'ємною.");

            _surcharge = value;
        }
    }

    public ExpressDelivery(string name, double ratePerKg,
                           double speedMultiplier, double surcharge)
        : base(name, ratePerKg)
    {
        SpeedMultiplier = speedMultiplier;
        Surcharge = surcharge;

        // Не дозволяємо додати до довідника тариф із некоректною контрольною вартістю.
        _ = ReferenceCost;
    }

    public override double CalculateCost(double weightKg)
    {
        ValidateWeight(weightKg);
        return ValidateCost(weightKg * RatePerKg * SpeedMultiplier + Surcharge);
    }

    public override string GetDescription()
    {
        return $"[Експрес] {base.GetDescription()} | коеф.: {SpeedMultiplier:F2}" +
               $" | надбавка: {Surcharge:F2} грн | за 1 кг: {ReferenceCost:F2} грн";
    }
}
