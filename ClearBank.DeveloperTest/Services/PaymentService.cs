using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStore _accountDataStore;
        private readonly IPaymentValidationService _paymentValidationService;

        public PaymentService(IAccountDataStore accountDataStore, IPaymentValidationService paymentValidationService)
        {
            _accountDataStore = accountDataStore;
            _paymentValidationService = paymentValidationService;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var account = _accountDataStore.GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult();

            if (account == null || !_paymentValidationService.IsPaymentValidForAccount(account, request.PaymentScheme, request.Amount))
            {
                return result;
            }

            account.Balance -= request.Amount;
            _accountDataStore.UpdateAccount(account);

            result.Success = true;

            return result;
        }
    }
}