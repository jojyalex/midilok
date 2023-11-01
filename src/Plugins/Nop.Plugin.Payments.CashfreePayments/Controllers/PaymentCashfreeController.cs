using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.CashfreePayments.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.CashfreePayments.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class PaymentCashfreeController : BasePaymentController
    {
        #region Fields
        
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IWorkContext _workContext;
        private readonly CashfreePaymentSettings _cashfreePaymentSettings;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly OrderSettings _orderSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public PaymentCashfreeController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IOrderProcessingService orderProcessingService,
            IWorkContext workContext,
            CashfreePaymentSettings cashfreePaymentSettings,
            ICustomerService customerService,
            IOrderService orderService,
            OrderSettings orderSettings,
            ILogger logger)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _orderProcessingService = orderProcessingService;
            _workContext = workContext;
            _cashfreePaymentSettings = cashfreePaymentSettings;
            _customerService = customerService;
            _orderService = orderService;
            _orderSettings = orderSettings;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Redirection(string sessionId)
        {
            var model = new ResponseModel();    
            model.payment_session_id = sessionId;
            model.ActiveEnvt = Convert.ToInt32(_cashfreePaymentSettings.ActiveEnvironment);
            return View("~/Plugins/Payments.CashfreePayments/Views/RedirectView.cshtml",model);
        }

        public async Task<IActionResult> CancelRedirect()
        {
            var order = await _orderService.GetOrderByIdAsync(Convert.ToInt32(30));
            await _orderProcessingService.CancelOrderAsync(order, true);
            return RedirectToRoute("Homepage");
        }


        [AuthorizeAdmin]
        [Area(AreaNames.Admin)] 
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var cashfreePaymentSettings = await _settingService.LoadSettingAsync<CashfreePaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                ActiveEnvironmentId = Convert.ToInt32(cashfreePaymentSettings.ActiveEnvironment),
                PaymentTypeId = Convert.ToInt32(cashfreePaymentSettings.PaymentType),
                Title = cashfreePaymentSettings.Title,
                AppID = cashfreePaymentSettings.AppID,
                SecretKey = cashfreePaymentSettings.SecretKey,
                Description = cashfreePaymentSettings.Description,
                PaymentMethods = cashfreePaymentSettings.PaymentMethods,
                ActiveEnvironmentValues = await cashfreePaymentSettings.ActiveEnvironment.ToSelectListAsync(),
                PaymentTypeValues = await cashfreePaymentSettings.PaymentType.ToSelectListAsync(),
                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.ActiveEnvironmentId_OverrideForStore = await _settingService.SettingExistsAsync(cashfreePaymentSettings, x => x.ActiveEnvironment, storeScope);
                model.PaymentTypeId_OverrideForStore = await _settingService.SettingExistsAsync(cashfreePaymentSettings, x => x.PaymentType, storeScope);
                model.Title_OverrideForStore = await _settingService.SettingExistsAsync(cashfreePaymentSettings, x => x.Title, storeScope);
                model.AppID_OverrideForStore = await _settingService.SettingExistsAsync(cashfreePaymentSettings, x => x.AppID, storeScope);
                model.SecretKey_OverrideForStore = await _settingService.SettingExistsAsync(cashfreePaymentSettings, x => x.SecretKey, storeScope);
                model.Description_OverrideForStore = await _settingService.SettingExistsAsync(cashfreePaymentSettings, x => x.Description, storeScope);
                model.PaymentMethods_OverrideForStore = await _settingService.SettingExistsAsync(cashfreePaymentSettings, x => x.PaymentMethods, storeScope);
            }

            return View("~/Plugins/Payments.CashfreePayments/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)] 
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //if (!ModelState.IsValid)
            //    return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var cashfreePaymentSettings = await _settingService.LoadSettingAsync<CashfreePaymentSettings>(storeScope);

            //save settings
            cashfreePaymentSettings.ActiveEnvironment = (ActiveEnvironment)model.ActiveEnvironmentId;
            cashfreePaymentSettings.PaymentType = (PaymentType)model.PaymentTypeId;
            cashfreePaymentSettings.Title = model.Title;
            cashfreePaymentSettings.AppID = model.AppID;
            cashfreePaymentSettings.SecretKey = model.SecretKey;
            cashfreePaymentSettings.Description = model.Description;
            cashfreePaymentSettings.PaymentMethods = model.PaymentMethods;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            await _settingService.SaveSettingOverridablePerStoreAsync(cashfreePaymentSettings, x => x.ActiveEnvironment, model.ActiveEnvironmentId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(cashfreePaymentSettings, x => x.PaymentType, model.PaymentTypeId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(cashfreePaymentSettings, x => x.Title, model.Title_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(cashfreePaymentSettings, x => x.AppID, model.AppID_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(cashfreePaymentSettings, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(cashfreePaymentSettings, x => x.Description, model.Description_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(cashfreePaymentSettings, x => x.PaymentMethods, model.PaymentMethods_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        public async Task<IActionResult> HandleResponse(string order_id)
        {
            //get current customer
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();


            //get the order by order_id
            var order = await _orderService.GetOrderByIdAsync(Convert.ToInt32(order_id));
            if (order == null)
                return RedirectToRoute("Homepage");

            var client = new HttpClient();
            var url = _cashfreePaymentSettings.ActiveEnvironment == 0 ?
                new Uri("https://sandbox.cashfree.com/pg/orders/" + order_id) :
               new Uri("https://api.cashfree.com/pg/orders/" + order_id);

            _cashfreePaymentSettings.ApiVersion = "2022-09-01";
            client.DefaultRequestHeaders.Add("x-api-version", _cashfreePaymentSettings.ApiVersion);
            client.DefaultRequestHeaders.Add("x-client-id", _cashfreePaymentSettings.AppID);
            client.DefaultRequestHeaders.Add("x-client-secret", _cashfreePaymentSettings.SecretKey);

            //get the order details from cashfree
            var result = await client.GetAsync(url);
            var json = result.Content.ReadAsStringAsync().Result;
            dynamic result2 = JsonConvert.DeserializeObject(json);
            string order_status = result2.order_status;//The order status -ACTIVE, PAID, EXPIRED
            string cf_order_id = result2.cf_order_id;
            var paymentstatus = PaymentStatus.Pending;

            //get payment details of order
            var client2 = new HttpClient();
            client2.DefaultRequestHeaders.Add("x-api-version", _cashfreePaymentSettings.ApiVersion);
            client2.DefaultRequestHeaders.Add("x-client-id", _cashfreePaymentSettings.AppID);
            client2.DefaultRequestHeaders.Add("x-client-secret", _cashfreePaymentSettings.SecretKey);
            //var paymentLink = result2.payments.url;
            var paymentUrl = _cashfreePaymentSettings.ActiveEnvironment == 0 ?
                new Uri("https://sandbox.cashfree.com/pg/orders/" + order_id + "/payments") :
               new Uri("https://api.cashfree.com/pg/orders/" + order_id + "/payments");
            var paymentResult = await client2.GetAsync(paymentUrl);
            var paymentJson = paymentResult.Content.ReadAsStringAsync().Result;
            dynamic paymentResult2 = JsonConvert.DeserializeObject(paymentJson);
            //string a = paymentResult2.cf_payment_id;
            //string isCaptured = paymentResult2.is_captured;
            //string paymentStatus = paymentResult2.payment_status;
            //var paymentstat = PaymentStatus.Pending;
            //var paystatus = GetPaymentStatus(paymentStatus);
            ///payment status ==SUCCESS,FAILED,MOT-ATTEMPTED,PENDING,FLAGGED,CAMCELLED,VOID,USER-DROPPED


            if (order_status == "ACTIVE" || order_status == "active")
            {
                //update payment_status=pending when cancel the transaction
                if (order != null)
                    return RedirectToRoute("OrderDetails", new { orderId = order.Id });

                return RedirectToRoute("Homepage");
            }
            else if (order_status == "EXPIRED" || order_status == "expired")
            {
                //cancel order
                await _orderProcessingService.CancelOrderAsync(order, true);
                return RedirectToRoute("Homepage");

            }
            else if (order_status == "PAID")
            {

                // paymentstatus = paystatus;
                paymentstatus = PaymentStatus.Paid;
                //updating ordernote and payment_status in Order
                await ProcessPaymentAsync(order.Id, paymentstatus, cf_order_id);
            }


            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        }

        protected virtual async Task ProcessPaymentAsync(int orderId, PaymentStatus newPaymentStatus, string cf_order_id)
        {

            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                await _logger.ErrorAsync("Cashfree Order is not found");
                return;
            }

            //order note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = orderId,
                Note = $"Order status has been changed to{newPaymentStatus}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            switch (newPaymentStatus)
            {
                case PaymentStatus.Authorized:
                    if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                        await _orderProcessingService.MarkAsAuthorizedAsync(order);
                    break;
                case PaymentStatus.Paid:
                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionId = cf_order_id;
                        await _orderService.UpdateOrderAsync(order);

                        await _orderProcessingService.MarkOrderAsPaidAsync(order);
                    }

                    break;
                case PaymentStatus.Voided:
                    if (_orderProcessingService.CanVoidOffline(order))
                        await _orderProcessingService.VoidOfflineAsync(order);

                    break;
            }

        }

        #endregion
    }
}