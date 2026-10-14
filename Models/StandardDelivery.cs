namespace DeliveryConsole.Models;

public class StandardDelivery : DeliveryMethod
{
    public StandardDelivery(string name, double ratePerKg)
        : base(name, ratePerKg) { }

    public override double CalculateCost(double weightKg)
    {
        ValidateWeight(weightKg);
        return ValidateCost(weightKg * RatePerKg);
    }

    public override string GetDescription()
    {
        return $"[Стандартна] {base.GetDescription()} | за 1 кг: {ReferenceCost:F2} грн";
    }
}
