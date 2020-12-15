using NUnit.Framework;
using ClearBank.DeveloperTest.Validators;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.Validators
{
    [TestFixture]
    public class FasterPaymentsPaymentSchemeUnitTests
    {
        private MakePaymentRequest _makePaymentRequest;
        private FasterPaymentsPaymentRequestValidator _validatorInTest;
        private Account _account;

        [SetUp]
        public void Setup()
        {
            _makePaymentRequest = new MakePaymentRequest();
            _validatorInTest = new FasterPaymentsPaymentRequestValidator();
            _account = new Account()
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
            };
        }

        [Test]
        public void Scheme_ShouldReturnCorrectEnum()
        {
            var paymentScheme = _validatorInTest.Scheme;

            Assert.That(paymentScheme, Is.EqualTo(PaymentScheme.FasterPayments));
        }

        [Test]
        public void IsValid_ShouldReturnFalseIfAccountIsNull()
        {
            var result = _validatorInTest.IsValid(null, _makePaymentRequest);

            Assert.That(result, Is.False);
        }

        [TestCase(AllowedPaymentSchemes.FasterPayments)]
        [TestCase(AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs)]
        [TestCase(AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps)]
        [TestCase(AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps)]
        public void IsValid_ShouldOnlyReturnTrueIfAccountAllowsScheme(AllowedPaymentSchemes accountPaymentSchemes)
        {
            _account.AllowedPaymentSchemes = accountPaymentSchemes;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.True);
        }

        [TestCase(AllowedPaymentSchemes.Bacs)]
        [TestCase(AllowedPaymentSchemes.Chaps)]
        [TestCase(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps)]
        public void IsValid_ShouldReturnFalseIfAccountDoesNotAllowScheme(AllowedPaymentSchemes accountPaymentSchemes)
        {
            _account.AllowedPaymentSchemes = accountPaymentSchemes;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.False);
        }

        [TestCase(100, 100)]
        [TestCase(100, 99)]
        public void IsValid_ShouldReturnTrueIfAccountBalanceIsMoreOrEqualToRequest(decimal balance, decimal request)
        {
            _account.Balance = balance;
            _makePaymentRequest.Amount = request;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValid_ShouldReturnFalseIfAccountBalanceIsLessThanRequest()
        {
            _account.Balance = 100;
            _makePaymentRequest.Amount = 101;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.False);
        }
    }
}
