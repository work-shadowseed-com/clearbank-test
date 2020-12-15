using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validators
{
    public class FasterPaymentsPaymentRequestValidator : IPaymentRequestValidator
    {
        public PaymentScheme Scheme
        {
            get { return PaymentScheme.FasterPayments; }
        }

        public bool IsValid(Account account, MakePaymentRequest request)
        {
            if (account == null)
            {
                return false;
            }
            else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
            {
                return false;
            }
            else if (account.Balance < request.Amount)
            {
                return false;
            }

            return true;
        }
    }
}