using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentValidationService : IPaymentValidationService
    {
        public bool IsPaymentValidForAccount(Account account, PaymentScheme paymentScheme, decimal amount)
        {
            switch (paymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        return false;
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments)
                        || account.Balance < amount)
                    {
                        return false;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps)
                        || account.Status != AccountStatus.Live)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}