using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Payments.CashfreePayments.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = GetLanguageRoutePattern();

            endpointRouteBuilder.MapControllerRoute(name: CashfreeDefaults.ConfigurationRouteName,
                pattern: "Admin/PaymentCashfree/Configure",
                defaults: new { controller = "CyberSource", action = "Configure" });
            //notify_url
            endpointRouteBuilder.MapControllerRoute(CashfreeDefaults.NotifyUrlRouteName,
                "Plugins/PaymentCashfree/NotifyUrl",
                 new { controller = "PaymentCashfree", action = "NotifyUrl" });
            //return_url
            endpointRouteBuilder.MapControllerRoute(CashfreeDefaults.ReturnUrlRoutename,
                "Plugins/PaymentCashfree/HandleResponse",
                 new { controller = "PaymentCashfree", action = "HandleResponse" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}