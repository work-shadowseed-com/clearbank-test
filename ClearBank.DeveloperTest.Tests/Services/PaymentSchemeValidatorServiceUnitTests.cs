using NUnit.Framework;
using ClearBank.DeveloperTest.Validators;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;

namespace ClearBank.DeveloperTest.Tests.Services
{
    [TestFixture]
    public class PaymentSchemeValidatorServiceUnitTests
    {
        private PaymentSchemeValidatorService _paymentSchemeValidatorServiceInTest;
        private Mock<IPaymentRequestValidator> _bacPaymentRequestValidatorMock;
        private Mock<IPaymentRequestValidator> _chapsPaymentRequestValidatorMock;
        private Mock<IPaymentRequestValidator> _fasterPaymentsPaymentRequestValidatorMock;

        [SetUp]
        public void Setup()
        {
            _bacPaymentRequestValidatorMock = new Mock<IPaymentRequestValidator>();
            _bacPaymentRequestValidatorMock.SetupGet(validator => validator.Scheme).Returns(PaymentScheme.Bacs);

            _chapsPaymentRequestValidatorMock = new Mock<IPaymentRequestValidator>();
            _chapsPaymentRequestValidatorMock.SetupGet(validator => validator.Scheme).Returns(PaymentScheme.Chaps);

            _fasterPaymentsPaymentRequestValidatorMock = new Mock<IPaymentRequestValidator>();
            _fasterPaymentsPaymentRequestValidatorMock.SetupGet(validator => validator.Scheme).Returns(PaymentScheme.FasterPayments);
        }

        [TestCase(PaymentScheme.Bacs)]
        [TestCase(PaymentScheme.Chaps)]
        [TestCase(PaymentScheme.FasterPayments)]
        public void GetPaymentSchemeValidator_ShouldReturnCorrectValidator(PaymentScheme paymentSceme)
        {
            _paymentSchemeValidatorServiceInTest = new PaymentSchemeValidatorService(new [] { _bacPaymentRequestValidatorMock.Object, _chapsPaymentRequestValidatorMock.Object, _fasterPaymentsPaymentRequestValidatorMock.Object});

            var validator = _paymentSchemeValidatorServiceInTest.GetPaymentSchemeValidator(paymentSceme);

            Assert.That(validator, Is.Not.Null);
        }

        [Test]
        public void GetPaymentSchemeValidator_ShouldReturnNullIfNoValidatorExists()
        {
            _paymentSchemeValidatorServiceInTest = new PaymentSchemeValidatorService(new[] { _bacPaymentRequestValidatorMock.Object, _chapsPaymentRequestValidatorMock.Object });

            var validator = _paymentSchemeValidatorServiceInTest.GetPaymentSchemeValidator(PaymentScheme.FasterPayments);

            Assert.That(validator, Is.Null);
        }
    }
}
