using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Moq;

namespace ClearBank.DeveloperTest.Tests
{
    [TestClass]
    public class PaymentServiceTests
    {
        private Mock<IAccountDataStore> _mockAccountDataStore;
        private Mock<IPaymentValidationService> _mockPaymentValidationService;
        private IPaymentService _paymentService;
        private Account _account;

        [TestInitialize]
        public void Setup()
        {
            _mockAccountDataStore = new();
            _mockPaymentValidationService = new();
            _account = new();

            _mockAccountDataStore
                .Setup(x => x.GetAccount(It.IsAny<string>()))
                .Returns(_account);

            _mockPaymentValidationService
                .Setup(x => x.IsPaymentValidForAccount(It.IsAny<Account>(), It.IsAny<PaymentScheme>(), It.IsAny<decimal>()))
                .Returns(true);

            _paymentService = new PaymentService(_mockAccountDataStore.Object, _mockPaymentValidationService.Object);
        }

        [TestMethod]
        public void MakePayment_AccountISNull_ReturnsUnsuccessful()
        {
            _mockAccountDataStore
                .Setup(x => x.GetAccount(It.IsAny<string>()))
                .Returns((Account)null);

            var actual = _paymentService.MakePayment(new MakePaymentRequest());

            actual.Success.Should().BeFalse();
        }

        [TestMethod]
        public void MakePayment_AccountIsNotValidForPayment_ReturnsUnsuccessful()
        {
            _mockPaymentValidationService
                .Setup(x => x.IsPaymentValidForAccount(It.IsAny<Account>(), It.IsAny<PaymentScheme>(), It.IsAny<decimal>()))
                .Returns(false);

            var actual = _paymentService.MakePayment(new MakePaymentRequest());

            actual.Success.Should().BeFalse();
        }

        [TestMethod]
        public void MakePayment_AccountIsValidForPayment_ReturnsSuccessful()
        {
            var actual = _paymentService.MakePayment(new MakePaymentRequest());

            actual.Success.Should().BeTrue();
        }

        [TestMethod]
        public void MakePayment_AccountIsValidForPayment_AccountBalanceIsUpdated()
        {
            var expectedBalanceDiffrence = 1;
            var paymentRequest = new MakePaymentRequest { Amount = -expectedBalanceDiffrence };

            var actual = _paymentService.MakePayment(paymentRequest);

            _account.Balance.Should().Be(expectedBalanceDiffrence);
        }

        [TestMethod]
        public void MakePayment_AccountIsValidForPayment_AccountIsUpdated()
        {
            var actual = _paymentService.MakePayment(new MakePaymentRequest());

            _mockAccountDataStore
                .Verify(x => x.UpdateAccount(_account), Times.Once);
        }
    }
}