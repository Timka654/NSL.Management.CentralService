using NSL.Management.CentralService.ExternalClient.Data.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSL.Management.CentralService.ExternalClient
{
    public class CentralServiceClient
    {
        private string baseUrl = "http://176.105.203.101:12100/";

        public string BaseUrl
        {
            get => baseUrl;
            set { baseUrl = value; if (!baseUrl.EndsWith('/')) baseUrl += "/"; }
        }

        public int LogReportRequestTimeout { get; set; } = 10;
        public int MetricsReportRequestTimeout { get; set; } = 10;

        public required Guid ServerId { get; init; }

        public required string ServerToken { get; init; }


        private const string ApiBaseUrl = "api/sync";

        private const string LogReportUrl = $"{ApiBaseUrl}/LogReport";

        public async Task<bool> LogReportAsync(SyncReportLogsRequestModel data)
        {
            if (!data.Logs.Any())
                return true;

            using var client = CreateClient();

            client.Timeout = TimeSpan.FromSeconds(LogReportRequestTimeout);

            try
            {
                using var response = await client.PostAsync(LogReportUrl, GetJsonContent(data));

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
            }

            return false;
        }

        private const string MetricsReportUrl = $"{ApiBaseUrl}/MetricsReport";

        public async Task<bool> MetricsReportAsync(SyncReportMetricsRequestModel data)
        {
            if (!data.Metrics.Any())
                return true;

            using var client = CreateClient();

            client.Timeout = TimeSpan.FromSeconds(MetricsReportRequestTimeout);

            try
            {
                using var response = await client.PostAsync(MetricsReportUrl, GetJsonContent(data));

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
            }

            return false;
        }

        private StringContent GetJsonContent(object value)
        {
            return new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "text/json");
        }


        private HttpClient CreateClient()
        {
            var hc = new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(20)
            };

            hc.DefaultRequestHeaders.Add("server_id", ServerId.ToString());
            hc.DefaultRequestHeaders.Add("identity_token", ServerToken);


            return hc;
        }

    }
}
