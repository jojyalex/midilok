using Nop.Core;

namespace Nop.Plugin.Payments.CashfreePayments
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class CashfreeDefaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Payments.CashfreePayments";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets the cashfree js script URL
        /// </summary>
        public static string ScriptUrl => "https://sdk.cashfree.com/js/ui/2.0.0/cashfree.sandbox.js";


        /// <summary>
        /// Gets the test api base url
        /// </summary>
        public static string TestApiBaseUrl => "apitest.cybersource.com";

        /// <summary>
        /// Gets the live api base url
        /// </summary>
        public static string LiveApiBaseUrl => "api.cybersource.com";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Payments.CashfreePayments.Configure";

        /// <summary>
        /// Gets the test api base url
        /// </summary>
        public static string NotifyUrlRouteName => "Plugin.Payments.CashfreePayments.NotifyUrl";

        /// <summary>
        /// Gets the live api base url
        /// </summary>
        public static string ReturnUrlRoutename => "Plugin.Payments.CashfreePayments.HandleResponse";

        /// <summary>
        /// Gets the redirection route url
        /// </summary>
        public static string RedirectionUrlRoutename => "Plugin.Payments.CashfreePayments.Redirection";



        /// <summary>
        /// Gets the one page checkout route name
        /// </summary>
        public static string OnePageCheckoutRouteName => "CheckoutOnePage";

        /// <summary>
        /// Gets the checkout payment info route name
        /// </summary>
        public static string CheckoutPaymentInfoRouteName => "CheckoutPaymentInfo";

      


        

        /// <summary>
        /// Gets the session key to get or set order and payment statuses
        /// </summary>
        /// <remarks>0 - order GUID</remarks>
        public static string OrderStatusesSessionKey => "CyberSource.OrderStatuses-{0}";

       


        #region Response status

        public class ResponseStatus
        {
            /// <summary>
            /// Gets the AUTHORIZED response status
            /// </summary>
            public static string Authorized => "AUTHORIZED";

            /// <summary>
            /// Gets the AUTHORIZED_PENDING_REVIEW response status
            /// </summary>
            public static string AuthorizedPendingReview => "AUTHORIZED_PENDING_REVIEW";

            /// <summary>
            /// Gets the AUTHORIZED_RISK_DECLINED response status
            /// </summary>
            public static string AuthorizedRiskDeclined => "AUTHORIZED_RISK_DECLINED";

            /// <summary>
            /// Gets the DECLINED response status
            /// </summary>
            public static string Declined => "DECLINED";
        }

        #endregion

        #region Response error reason

        public class ResponseErrorReason
        {
            /// <summary>
            /// Gets the AVS_FAILED response error reason
            /// </summary>
            public static string AvsFailed => "AVS_FAILED";

            /// <summary>
            /// Gets the CV_FAILED response error reason
            /// </summary>
            public static string CvFailed => "CV_FAILED";

            /// <summary>
            /// Gets the DECISION_PROFILE_REVIEW response error reason
            /// </summary>
            public static string DecisionProfileReview => "DECISION_PROFILE_REVIEW";

            /// <summary>
            /// Gets the DECISION_PROFILE_REJECT response error reason
            /// </summary>
            public static string DecisionProfileReject => "DECISION_PROFILE_REJECT";
        }

        #endregion

        #region Decisions

        public class Decisions
        {
            /// <summary>
            /// Gets the accepted decision
            /// </summary>
            public static string Accepted => "ACCEPT";

            /// <summary>
            /// Gets the rejected decision
            /// </summary>
            public static string Rejected => "REJECT";
        }

        #endregion
    }
}