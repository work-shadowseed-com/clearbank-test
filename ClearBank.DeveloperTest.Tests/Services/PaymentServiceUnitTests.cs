using ClearBank.DeveloperTest.Data;
using NUnit.Framework;
using ClearBank.DeveloperTest.Validators;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;

namespace ClearBank.DeveloperTest.Tests.Services
{
    [TestFixture]
    public class PaymentServiceUnitTests
    {
        private PaymentService _paymentServiceInTest;
        private MakePaymentRequest _makePaymentRequest;
        private Mock<IAccountDataStore> _accountDataStoreMock;
        private Mock<IPaymentSchemeValidatorService> _paymentSchemeValidatorServiceMock;
        private Account _account;

        [SetUp]
        public void Setup()
        {
            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _paymentSchemeValidatorServiceMock = new Mock<IPaymentSchemeValidatorService>();

            _paymentServiceInTest = new PaymentService(_accountDataStoreMock.Object, _paymentSchemeValidatorServiceMock.Object);

            _account = new Account();
            _makePaymentRequest = new MakePaymentRequest();

            _accountDataStoreMock.Setup((accountDataStore => accountDataStore.GetAccount(It.IsAny<string>()))).Returns(_account);
        }

        [Test]
        public void MakePayment_GetsAccountByDebtorAccountNumber()
        {
            _makePaymentRequest.DebtorAccountNumber = "123";

            _paymentServiceInTest.MakePayment(_makePaymentRequest);

            _accountDataStoreMock.Verify(accountDataStore => accountDataStore.GetAccount(_makePaymentRequest.DebtorAccountNumber), Times.Once);
        }

        [Test]
        public void MakePayment_ReturnsFalseIfAccountNotFound()
        {
            _accountDataStoreMock.Setup((accountDataStore => accountDataStore.GetAccount(It.IsAny<string>()))).Returns((Account) null);

            var result = _paymentServiceInTest.MakePayment(_makePaymentRequest);

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void MakePayment_IfAccountNotFoundUpdateAccountIsNotCalled()
        {
            _accountDataStoreMock.Setup((accountDataStore => accountDataStore.GetAccount(It.IsAny<string>()))).Returns((Account)null);

            _paymentServiceInTest.MakePayment(_makePaymentRequest);

            _accountDataStoreMock.Verify(accountDataStore => accountDataStore.UpdateAccount(null), Times.Never);
        }

        [TestCase(PaymentScheme.Bacs)]
        [TestCase(PaymentScheme.Chaps)]
        [TestCase(PaymentScheme.FasterPayments)]
        public void MakePayment_PaymentValidationServiceIsCalledWithCorrectPaymentScheme(PaymentScheme paymentScheme)
        {
            _makePaymentRequest.PaymentScheme = paymentScheme;

            _paymentServiceInTest.MakePayment(_makePaymentRequest);

            _paymentSchemeValidatorServiceMock.Verify(validatorService => validatorService.GetPaymentSchemeValidator(paymentScheme), Times.Once);
        }

        [Test]
        public void MakePayment_ReturnsFalseIfValidatorServiceReturnsFalse()
        {
            var mockValidator = new Mock<IPaymentRequestValidator>();
            mockValidator.Setup(validator => validator.IsValid(It.IsAny<Account>(), It.IsAny<MakePaymentRequest>())).Returns(false);
            _paymentSchemeValidatorServiceMock.Setup(validatorService => validatorService.GetPaymentSchemeValidator(It.IsAny<PaymentScheme>())).Returns(mockValidator.Object);

            var result = _paymentServiceInTest.MakePayment(_makePaymentRequest);

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void MakePayment_DoesNotUpdateAccountIfValidatorServiceReturnsFalse()
        {
            var mockValidator = new Mock<IPaymentRequestValidator>();
            mockValidator.Setup(validator => validator.IsValid(It.IsAny<Account>(), It.IsAny<MakePaymentRequest>())).Returns(false);
            _paymentSchemeValidatorServiceMock.Setup(validatorService => validatorService.GetPaymentSchemeValidator(It.IsAny<PaymentScheme>())).Returns(mockValidator.Object);

            _paymentServiceInTest.MakePayment(_makePaymentRequest);
            
            _accountDataStoreMock.Verify(accountDataStore => accountDataStore.UpdateAccount(_account), Times.Never);
        }

        [Test]
        public void MakePayment_ReturnsTrueIfValidatorServiceReturnsTrue()
        {
            var mockValidator = new Mock<IPaymentRequestValidator>();
            mockValidator.Setup(validator => validator.IsValid(It.IsAny<Account>(), It.IsAny<MakePaymentRequest>())).Returns(true);
            _paymentSchemeValidatorServiceMock.Setup(validatorService => validatorService.GetPaymentSchemeValidator(It.IsAny<PaymentScheme>())).Returns(mockValidator.Object);

            var result = _paymentServiceInTest.MakePayment(_makePaymentRequest);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void MakePayment_UpdatesAccountCorrectlyIfValidatorServiceReturnsTrue()
        {
            var mockValidator = new Mock<IPaymentRequestValidator>();
            mockValidator.Setup(validator => validator.IsValid(It.IsAny<Account>(), It.IsAny<MakePaymentRequest>())).Returns(true);
            _paymentSchemeValidatorServiceMock.Setup(validatorService => validatorService.GetPaymentSchemeValidator(It.IsAny<PaymentScheme>())).Returns(mockValidator.Object);
            _account.Balance = 100;
            _makePaymentRequest.Amount = 90;

            _paymentServiceInTest.MakePayment(_makePaymentRequest);

            _accountDataStoreMock.Verify(accountDataStore => accountDataStore.UpdateAccount(_account), Times.Once());
            Assert.That(_account.Balance, Is.EqualTo(10));
        }
    }
}
