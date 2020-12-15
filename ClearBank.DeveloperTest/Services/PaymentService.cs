using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validators;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStore _accountDataStore;
        private readonly IPaymentSchemeValidatorService _paymentSchemeValidatorService;

        public PaymentService(IAccountDataStore accountDataStore, IPaymentSchemeValidatorService paymentSchemeValidatorService)
        {
            _accountDataStore = accountDataStore;
            _paymentSchemeValidatorService = paymentSchemeValidatorService;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var result = new MakePaymentResult();

            Account account = _accountDataStore.GetAccount(request.DebtorAccountNumber);
            if (account != null)
            {
                IPaymentRequestValidator paymentRequestValidator = _paymentSchemeValidatorService.GetPaymentSchemeValidator(request.PaymentScheme);
                if (paymentRequestValidator == null)
                {
                    result.Success = false;
                }
                else
                {
                    result.Success = paymentRequestValidator.IsValid(account, request);
                }
            }

            if (result.Success)
            {
                account.Balance -= request.Amount;
                _accountDataStore.UpdateAccount(account);
            }

            return result;
        }
    }
}
