using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api
{
    /// <summary>
    /// Represents HTTP client to request Google Analytics 4 services
    /// </summary>
    public class GoogleAnalyticsHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public GoogleAnalyticsHttpClient(HttpClient httpClient,
            GoogleAnalyticsSettings googleAnalyticsSettings)
        {
            //configure client
            var query = new Dictionary<string, string>()
            {
                ["api_secret"] = googleAnalyticsSettings.ApiSecret,
                ["measurement_id"] = googleAnalyticsSettings.GoogleId
            };

            var uri = QueryHelpers.AddQueryString(GoogleAnalyticsDefaults.EndPointUrl, query);

            httpClient.BaseAddress = new Uri(uri);
            httpClient.Timeout = TimeSpan.FromSeconds(10);

            _httpClient = httpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Request API service
        /// </summary>
        /// <param name="request">Event Request</param>
        /// <returns>The asynchronous task whose result contains response details</returns>
        public async Task RequestAsync(EventRequest request)
        {
            try
            {
                var requestString = JsonConvert.SerializeObject(request);
                var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
                var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), null as Uri) { Content = requestContent };
                var httpResponse = await _httpClient.SendAsync(requestMessage);
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (AggregateException exception)
            {
                //rethrow actual exception
                throw exception.InnerException;
            }
        }

        #endregion
    }
}
