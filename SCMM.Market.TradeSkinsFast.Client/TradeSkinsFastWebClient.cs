﻿using SCMM.Market.Client;
using System.Text.Json;

namespace SCMM.Market.TradeSkinsFast.Client
{
    public class TradeSkinsFastWebClient : MarketWebClient
    {
        private const string BaseUri = "https://tradeskinsfast.com/";

        public async Task<TradeSkinsFastBotsInventoryResult> PostBotsInventoryAsync(string appId)
        {
            using (var client = BuildHttpClient(new Uri(BaseUri)))
            {
                var url = $"{BaseUri}ajax/botsinventory";
                var payload = new FormUrlEncodedContent(new Dictionary<string, string>() {
                    { "appid", appId }
                });

                var response = await client.PostAsync(url, payload);
                response.EnsureSuccessStatusCode();

                var textJson = await response.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<TradeSkinsFastResponse<TradeSkinsFastBotsInventoryResult>>(textJson);
                return responseJson?.Response;
            }
        }
    }
}
