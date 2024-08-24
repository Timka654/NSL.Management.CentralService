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

        public required Guid ServerId { get; init; }

        public required string ServerToken { get; init; }


        private const string ApiBaseUrl = "api/sync";

        private const string LogReportUrl = $"{ApiBaseUrl}/LogReport";

        public async Task<bool> LogReportAsync(SyncReportLogsRequestModel data)
        {
            if (!data.Logs.Any())
                return true;

            using var client = CreateClient();

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

        private StringContent GetJsonContent(object value)
        {
            return new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "text/json");
        }


        private HttpClient CreateClient()
        {
            var hc = new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl)
            };

            hc.DefaultRequestHeaders.Add("server_id", ServerId.ToString());
            hc.DefaultRequestHeaders.Add("identity_token", ServerToken);


            return hc;
        }

    }
}
