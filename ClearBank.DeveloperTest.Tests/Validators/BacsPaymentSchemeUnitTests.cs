using NUnit.Framework;
using ClearBank.DeveloperTest.Validators;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.Validators
{

    [TestFixture]
    public class BacsPaymentSchemeUnitTests
    {
        private MakePaymentRequest _makePaymentRequest;
        private BacsPaymentRequestValidator _validatorInTest;
        private Account _account;

        [SetUp]
        public void Setup()
        {
            _makePaymentRequest = new MakePaymentRequest();
            _validatorInTest = new BacsPaymentRequestValidator();
            _account = new Account()
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };
        }

        [Test]
        public void Scheme_ShouldReturnCorrectEnum()
        {
            var paymentScheme = _validatorInTest.Scheme;

            Assert.That(paymentScheme, Is.EqualTo(PaymentScheme.Bacs));
        }

        [Test]
        public void IsValid_ShouldReturnFalseIfAccountIsNull()
        {
            var result = _validatorInTest.IsValid(null, _makePaymentRequest);

            Assert.That(result, Is.False);
        }

        [TestCase(AllowedPaymentSchemes.Bacs)]
        [TestCase(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps)]
        [TestCase(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments)]
        [TestCase(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments)]
        public void IsValid_ShouldOnlyReturnTrueIfAccountAllowsScheme(AllowedPaymentSchemes accountPaymentSchemes)
        {
            _account.AllowedPaymentSchemes = accountPaymentSchemes;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.True);
        }

        [TestCase(AllowedPaymentSchemes.Chaps)]
        [TestCase(AllowedPaymentSchemes.FasterPayments)]
        [TestCase(AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments)]
        public void IsValid_ShouldReturnFalseIfAccountDoesNotAllowScheme(AllowedPaymentSchemes accountPaymentSchemes)
        {
            _account.AllowedPaymentSchemes = accountPaymentSchemes;

            var result = _validatorInTest.IsValid(_account, _makePaymentRequest);

            Assert.That(result, Is.False);
        }
    }
}
