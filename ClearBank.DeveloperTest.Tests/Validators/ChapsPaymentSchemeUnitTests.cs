using NUnit.Framework;
using ClearBank.DeveloperTest.Validators;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.Validators
{
    [TestFixture]
    public class ChapsPaymentSchemeUnitTests
    {
        private MakePaymentRequest _makePaymentRequest;
        private ChapsPaymentRequestValidator _validatorInTest;
        private Account _account;

        [SetUp]
        public void Setup()
        {
            _makePaymentRequest = new MakePaymentRequest();
            _validatorInTest = new ChapsPaymentRequestValidator();
            _account = new Account()
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live
            };
        }

        [Test]
        public void Scheme_ShouldReturnCorrectEnum()
        {
            var paymentScheme = _validatorInTest.Scheme;

            Assert.That(paymentScheme, Is.EqualTo(PaymentScheme.Chaps));
        }

        [Test]
        public void IsValid_ShouldReturnFalseIfAccountIsNull()
        {
            var result = _validatorInTest.IsValid(null, _makePaymentRequest);

            Assert.That(result, Is.False);
        }

        [TestCase(AllowedPaymentSchemes.Chaps)]
        [TestCase(AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.Bacs)]
        [TestCase(AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments)]
        [TestCase(AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments)]
        public void IsValid_ShouldOnlyReturnTrueIfAccountAllowsScheme(AllowedPaymentSchemes accountPaymentSchemes)
        {
            _account.AllowedPaymentSchemes = accountPaymentSchemes;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.True);
        }

        [TestCase(AllowedPaymentSchemes.Bacs)]
        [TestCase(AllowedPaymentSchemes.FasterPayments)]
        [TestCase(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments)]
        public void IsValid_ShouldReturnFalseIfAccountDoesNotAllowScheme(AllowedPaymentSchemes accountPaymentSchemes)
        {
            _account.AllowedPaymentSchemes = accountPaymentSchemes;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.False);
        }

        [TestCase(AccountStatus.InboundPaymentsOnly)]
        [TestCase(AccountStatus.Disabled)]
        public void IsValid_ShouldReturnFalseIfAccountStatusIsNotLive(AccountStatus invalidAccountStatus)
        {
            _account.Status = invalidAccountStatus;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.False);
        }

        public void IsValid_ShouldReturnTrueIfAccountStatusIsLive()
        {
            _account.Status = AccountStatus.Live;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.True);
        }
    }
}
