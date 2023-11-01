using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.CashfreePayments
{
    /// <summary>
    /// Represents settings of manual payment plugin
    /// </summary>
    public class CashfreePaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets  Appid
        /// </summary>
        public string? AppID { get; set; }

        /// <summary>
        /// Gets or sets SecretKey
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// Gets or sets Description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets ActiveEnvironment 
        /// </summary>
        public ActiveEnvironment ActiveEnvironment { get; set; }

        /// <summary>
        /// Gets or sets PaymentMethods
        /// </summary>
        public string? PaymentMethods { get; set; }

        /// <summary>
        /// Gets or sets ApiVersion
        /// </summary>
        public string? ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Gets or sets a subscription status after processing
        /// </summary>
        public SubscriptionStatus NewSubscriptionStatus { get; set; } = SubscriptionStatus.PENDING;
    }
}
