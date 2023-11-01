
namespace Nop.Plugin.Payments.CashfreePayments
{
    /// <summary>
    /// Represents manual payment processor transaction mode
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// Authorize and capture
        /// </summary>
        AuthorizeAndCapture = 0,

        /// <summary>
        /// Authorize
        /// </summary>
        Authorize = 1

    }
}
