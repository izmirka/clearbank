using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public interface IPaymentValidationService
    {
        bool IsPaymentValidForAccount(Account account, PaymentScheme paymentScheme, decimal amount);
    }
}