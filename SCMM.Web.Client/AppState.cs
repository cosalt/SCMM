﻿using Microsoft.Extensions.Logging;
using SCMM.Web.Client.Shared.Storage;
using SCMM.Web.Data.Models.Domain.Currencies;
using SCMM.Web.Data.Models.Domain.Languages;
using SCMM.Web.Data.Models.Domain.Profiles;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SCMM.Web.Client
{
    public class AppState
    {
        public const string HttpHeaderLanguage = "language";
        public const string HttpHeaderCurrency = "currency";
        public const string DefaultLanguage = "English";
        public const string DefaultCurrency = "USD";

        private readonly ILogger<AppState> Logger;
        private readonly LocalStorageService Storage;

        public AppState(ILogger<AppState> logger, LocalStorageService storage)
        {
            this.Logger = logger;
            this.Storage = storage;
        }

        public event EventHandler Changed;

        public string LanguageId { get; set; }

        public LanguageDetailedDTO Language => Profile?.Language;

        public string CurrencyId { get; set; }

        public CurrencyDetailedDTO Currency => Profile?.Currency;

        public ProfileDetailedDTO Profile { get; set; }

        public bool IsAuthenticated => (
            Profile != null && Profile.Id != Guid.Empty
        );

        public bool IsInRole(string role)
        {
            return Profile?.Roles?.Any(x => String.Equals(x, role, StringComparison.OrdinalIgnoreCase)) == true;
        }

        public bool Is(string steamId)
        {
            return (Profile?.SteamId == steamId || Profile?.ProfileId == steamId) == true;
        }

        public void AddHeadersTo(HttpClient client)
        {
            if (!String.IsNullOrEmpty(LanguageId))
            {
                client.DefaultRequestHeaders.Remove(HttpHeaderLanguage);
                client.DefaultRequestHeaders.Add(HttpHeaderLanguage, LanguageId);
            }
            if (!String.IsNullOrEmpty(CurrencyId))
            {
                client.DefaultRequestHeaders.Remove(HttpHeaderCurrency);
                client.DefaultRequestHeaders.Add(HttpHeaderCurrency, CurrencyId);
            }
        }

        // TODO: Store this info as a cookie with a fixed-expiry
        public async Task<bool> ReadFromStorageAsync()
        {
            try
            {
                LanguageId = await Storage.GetAsync<string>(nameof(LanguageId));
                CurrencyId = await Storage.GetAsync<string>(nameof(CurrencyId));
                return (!String.IsNullOrEmpty(LanguageId) && !String.IsNullOrEmpty(CurrencyId));
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to load state from storage");
                return false;
            }
        }

        // TODO: Store this info as a cookie with a fixed-expiry
        public async Task WriteToStorageAsync()
        {
            try
            {
                if (!String.IsNullOrEmpty(LanguageId))
                {
                    await Storage.SetAsync<string>(nameof(LanguageId), LanguageId);
                }
                else
                {
                    await Storage.RemoveAsync(nameof(LanguageId));
                }
                if (!String.IsNullOrEmpty(CurrencyId))
                {
                    await Storage.SetAsync<string>(nameof(CurrencyId), CurrencyId);
                }
                else
                {
                    await Storage.RemoveAsync(nameof(CurrencyId));
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to save state to storage");
            }
        }

        public Task TryGuessLocalityAsync()
        {
            try
            {
                // Just assign defaults
                LanguageId = DefaultLanguage;
                CurrencyId = DefaultCurrency;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to auto-detect locality, falling back to default");
            }

            return Task.CompletedTask;
        }

        public async Task RefreshAsync(HttpClient http)
        {
            try
            {
                AddHeadersTo(http);
                Profile = await http.GetFromJsonAsync<ProfileDetailedDTO>(
                    $"api/profile"
                );
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to refresh the profile state");
                Profile = null;
            }
            finally
            {
                Changed?.Invoke(this, new EventArgs());
            }
        }

        public void ChangeCurrency(CurrencyDetailedDTO currency)
        {
            if (currency == null)
            {
                return;
            }

            CurrencyId = currency.Name;
            if (Profile != null)
            {
                Profile.Currency = currency;
            }

            Changed?.Invoke(this, new EventArgs());
        }
    }
}
