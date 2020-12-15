using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validators;

namespace ClearBank.DeveloperTest.Services
{
    public interface IPaymentSchemeValidatorService
    {
        IPaymentRequestValidator GetPaymentSchemeValidator(PaymentScheme paymentScheme);
    }
}