using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validators
{
    public interface IPaymentRequestValidator
    {
        PaymentScheme Scheme { get; }
        bool IsValid(Account account, MakePaymentRequest request);
    }
}