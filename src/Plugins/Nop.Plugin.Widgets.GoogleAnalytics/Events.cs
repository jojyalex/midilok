using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Widgets.GoogleAnalytics.Api;
using Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    public class EventConsumer :
        IConsumer<OrderPaidEvent>,
        IConsumer<OrderRefundedEvent>
    {
        private readonly CurrencySettings _currencySettings;
        private readonly GoogleAnalyticsHttpClient _googleAnalyticsHttpClient;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWidgetPluginManager _widgetPluginManager;

        public EventConsumer(
            CurrencySettings currencySettings, 
            GoogleAnalyticsHttpClient googleAnalyticsHttpClient,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            IHttpContextAccessor httpContextAccessor,
            ILogger logger,
            IOrderService orderService,
            IProductService productService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWidgetPluginManager widgetPluginManager)
        {
            _currencySettings = currencySettings;
            _googleAnalyticsHttpClient = googleAnalyticsHttpClient;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _orderService = orderService;
            _productService = productService;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _widgetPluginManager = widgetPluginManager;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<bool> IsPluginEnabledAsync()
        {
            return await _widgetPluginManager.IsPluginActiveAsync(GoogleAnalyticsDefaults.SystemName);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ProcessOrderEventAsync(Order order, string eventName)
        {
            try
            {
                //settings per store
                var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
                var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);
                var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
                var orderId = order.CustomOrderNumber;
                var orderShipping = googleAnalyticsSettings.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax;
                var orderTax = order.OrderTax;
                var orderTotal = order.OrderTotal;

                //try to get cookie
                var httpContext = _httpContextAccessor.HttpContext;
                httpContext.Request.Cookies.TryGetValue(GoogleAnalyticsDefaults.ClientIdCookiesName, out var clientId);

                var gaRequest = new EventRequest
                {
                    ClientId = clientId,
                    UserId = order.CustomerId.ToString(),
                    TimestampMicros = (DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).Ticks / 10
                };


                var events = new List<Event>();
                var gaEvent = new Event
                {
                    Name = eventName
                };
                events.Add(gaEvent);

                var gaParams = new Parameters
                {
                    Currency = currency,
                    TransactionId = orderId,
                    Value = orderTotal,
                    Shipping = orderShipping
                };

                var items = new List<Item>();

                foreach (var item in await _orderService.GetOrderItemsAsync(order.Id))
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    var sku = await _productService.FormatSkuAsync(product, item.AttributesXml);
                    if (string.IsNullOrEmpty(sku))
                        sku = product.Id.ToString();
                    //get category
                    var category = (await _categoryService.GetCategoryByIdAsync((await _categoryService.GetProductCategoriesByProductIdAsync(product.Id)).FirstOrDefault()?.CategoryId ?? 0))?.Name;
                    if (string.IsNullOrEmpty(category))
                        category = "No category";
                    var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;

                    var gaItem = new Item
                    {
                        ItemId = sku,
                        ItemName = product.Name,
                        Affiliation = store.Name,
                        ItemCategory = category,
                        Price = unitPrice,
                        Quantity = item.Quantity
                    };

                    items.Add(gaItem);
                }
                gaParams.Items = items;
                gaEvent.Params = gaParams;                
                gaRequest.Events = events;

                await _googleAnalyticsHttpClient.RequestAsync(gaRequest);
            }
            catch (Exception ex)
            {
                await _logger.InsertLogAsync(LogLevel.Error, "Google Analytics. Error canceling transaction from server side", ex.ToString());
            }
        }

        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventMessage">The event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
        {
            //ensure the plugin is installed and active
            if (!await IsPluginEnabledAsync())
                return;

            var order = eventMessage.Order;

            //settings per store
            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
            var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

            //ecommerce is disabled
            if (!googleAnalyticsSettings.EnableEcommerce)
                return;

            bool sendRequest;
            if (googleAnalyticsSettings.UseJsToSendEcommerceInfo)
            {
                //if we use JS to notify GA about new orders (even when they are placed), then we should always notify GA about deleted orders
                //but ignore already cancelled orders (do not duplicate request to GA)
                sendRequest = order.OrderStatus != OrderStatus.Cancelled;
            }
            else
            {
                //if we use HTTP requests to notify GA about new orders (only when they are paid), then we should notify GA about deleted AND paid orders
                sendRequest = order.PaymentStatus == PaymentStatus.Paid;
            }

            if (sendRequest)
                await ProcessOrderEventAsync(order, GoogleAnalyticsDefaults.OrderRefundedEventName);
        }

        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventMessage">The event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            //ensure the plugin is installed and active
            if (!await IsPluginEnabledAsync())
                return;

            var order = eventMessage.Order;

            //settings per store
            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
            var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

            //ecommerce is disabled
            if (!googleAnalyticsSettings.EnableEcommerce)
                return;

            //we use HTTP requests to notify GA about new orders (only when they are paid)
            var sendRequest = !googleAnalyticsSettings.UseJsToSendEcommerceInfo;

            if (sendRequest)
                await ProcessOrderEventAsync(order, GoogleAnalyticsDefaults.OrderPaidEventName);
        }
    }
}