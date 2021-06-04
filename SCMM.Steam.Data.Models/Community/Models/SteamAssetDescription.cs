﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace SCMM.Steam.Data.Models.Community.Models
{
    public class SteamAssetDescription
    {
        [JsonProperty("classid")]
        public ulong ClassId { get; set; }

        [JsonProperty("contextid")]
        public ulong ContextId { get; set; }

        [JsonProperty("instanceid")]
        public ulong InstanceId { get; set; }

        [JsonProperty("appid")]
        public ulong AppId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("market_name")]
        public string MarketName { get; set; }

        [JsonProperty("market_hash_name")]
        public string MarketNameHash { get; set; }

        [JsonProperty("marketable")]
        public bool Marketable { get; set; }

        [JsonProperty("market_marketable_restriction")]
        public uint MarketMarketableRestriction { get; set; }

        [JsonProperty("market_tradable_restriction")]
        public uint MarketTradableRestriction { get; set; }

        [JsonProperty("tradable")]
        public bool Tradable { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_color")]
        public string NameColor { get; set; }

        [JsonProperty("background_color")]
        public string BackgroundColor { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }

        [JsonProperty("icon_url_large")]
        public string IconUrlLarge { get; set; }

        [JsonProperty("descriptions")]
        public List<SteamAssetDescriptionDescription> Descriptions { get; set; }

        [JsonProperty("commodity")]
        public bool Commodity { get; set; }

        [JsonProperty("actions")]
        public List<SteamAssetAction> Actions { get; set; }
    }
}
