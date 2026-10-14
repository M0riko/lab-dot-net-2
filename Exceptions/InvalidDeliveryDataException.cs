namespace DeliveryConsole.Exceptions;

public class InvalidDeliveryDataException : Exception
{
    public InvalidDeliveryDataException() { }

    public InvalidDeliveryDataException(string message) : base(message) { }

    public InvalidDeliveryDataException(string message, Exception innerException)
        : base(message, innerException) { }
}
