using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validators
{
    public class ChapsPaymentRequestValidator : IPaymentRequestValidator
    {
        public PaymentScheme Scheme
        {
            get { return PaymentScheme.Chaps; }
        }

        public bool IsValid(Account account, MakePaymentRequest request)
        {
            if (account == null)
            {
                return false;
            }
            else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
            {
                return false;
            }
            else if (account.Status != AccountStatus.Live)
            {
                return false;
            }
            //Note: Nothing appears to check that the account has the balance avalible to deduct request.Amount as in the FasterPaymentsPaymentScheme. 

            return true;
        }
    }
}