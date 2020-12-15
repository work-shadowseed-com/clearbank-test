using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validators;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentSchemeValidatorService : IPaymentSchemeValidatorService
    {
        private readonly List<IPaymentRequestValidator> _paymentSchemeValidators;

        public PaymentSchemeValidatorService(IPaymentRequestValidator[] paymentRequestValidators)
        {
            _paymentSchemeValidators = paymentRequestValidators.ToList();
        }

        public IPaymentRequestValidator GetPaymentSchemeValidator(PaymentScheme paymentScheme)
        {
            return _paymentSchemeValidators.FirstOrDefault(validator => validator.Scheme == paymentScheme);
        }
    }
}