﻿using CommandQuery;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using SCMM.Discord.Bot.Server.Autocompleters;
using SCMM.Discord.Client.Attributes;
using SCMM.Discord.Client.Commands;
using SCMM.Discord.Client.Extensions;
using SCMM.Shared.API.Extensions;
using SCMM.Shared.Data.Models.Extensions;
using SCMM.Steam.API.Queries;
using SCMM.Steam.Data.Models.Enums;
using SCMM.Steam.Data.Models.Extensions;
using SCMM.Steam.Data.Store;
using System.Text;

namespace SCMM.Discord.Bot.Server.Modules;

[Global]
public class ItemModule : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly IConfiguration _configuration;
    private readonly SteamDbContext _db;
    private readonly ICommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;

    public ItemModule(IConfiguration configuration, SteamDbContext db, ICommandProcessor commandProcessor, IQueryProcessor queryProcessor)
    {
        _configuration = configuration;
        _db = db;
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
    }

    [SlashCommand("skin", "Show information about a skin")]
    public async Task<RuntimeResult> GetItemValueAsync(
        [Summary("name", "Name of the skin")][Autocomplete(typeof(ItemNameAutocompleteHandler))] string name,
        [Summary("currency", "Any supported three-letter currency code (e.g. USD, EUR, AUD)")][Autocomplete(typeof(CurrencyAutocompleteHandler))] string currencyId = null
    )
    {
        // If currency was not specified, default to the profile or guild currency (if any)
        if (string.IsNullOrEmpty(currencyId) && Context.User != null)
        {
            var discordId = Context.User.GetFullUsername();
            currencyId = await _db.SteamProfiles
                .Where(x => x.DiscordId == discordId)
                .Where(x => x.Currency != null)
                .Select(x => x.Currency.Name)
                .FirstOrDefaultAsync();
        } 
        if (string.IsNullOrEmpty(currencyId) && Context.Guild != null)
        {
            var guildId = Context.Guild.Id.ToString();
            var guild = await _db.DiscordGuilds
                .AsNoTracking()
                .Include(x => x.Configurations)
                .FirstOrDefaultAsync(x => x.DiscordId == guildId);
            if (guild != null)
            {
                currencyId = guild.Get(Steam.Data.Store.DiscordConfiguration.Currency).Value;
            }
        }

        // Load the currency
        var getCurrencyByName = await _queryProcessor.ProcessAsync(new GetCurrencyByNameRequest()
        {
            Name = currencyId
        });

        var currency = getCurrencyByName.Currency;
        if (currency == null)
        {
            return InteractionResult.Fail(
                $"Sorry, I don't support that currency."
            );
        }

        // Find the closest item name to the one specified (accounting for minor typos)
        var closestItemName = _db.SteamAssetDescriptions
            .Select(x => x.Name)
            .ToList()
            .Closest(x => x, name, maxDistance: 3);

        if (String.IsNullOrEmpty(closestItemName))
        {
            return InteractionResult.Fail(
                $"Sorry, I can't find any item with that name."
            );
        }

        // Load the item
        var item = await _db.SteamAssetDescriptions
            .Where(x => x.Name == closestItemName)
            .Include(x => x.App)
            .Include(x => x.StoreItem).ThenInclude(x => x.Stores)
            .Include(x => x.StoreItem).ThenInclude(x => x.Currency)
            .Include(x => x.MarketItem).ThenInclude(x => x.Currency)
            .FirstOrDefaultAsync();

        if (item == null)
        {
            return InteractionResult.Fail(
                $"Sorry, I can't find any item by that name."
            );
        }

        var prices = item.GetPrices(currency).ToList();
        var availablePrices = prices
            .Where(x => x.IsAvailable)
            .OrderBy(x => x.LowestPrice)
            .ToList();

        var description = new StringBuilder(item.Description);
        var storePrice = prices.FirstOrDefault(x => x.Type == PriceType.SteamStore);
        if (storePrice != null && item.TimeAccepted != null)
        {
            if (!description.ToString().EndsWith("."))
            {
                description.Append(".");
            }

            description.AppendLine();
            description.AppendLine();
            description.Append($"{(item.StoreItem.Stores.Count > 1 ? "First released" : "Released")} on {item.TimeAccepted.Value.ToString("d")}");
            if (!storePrice.IsAvailable)
            {
                description.Append($" for `{currency.ToPriceString(storePrice.LowestPrice)}`");
            }
            if (item.LifetimeSubscriptions > 0)
            {
                description.Append($" with more than {item.LifetimeSubscriptions.Value.ToQuantityString()} copies sold");
            }
            else
            {
                description.Append($", unknown number of sales");
            }
            if (!description.ToString().EndsWith("."))
            {
                description.Append(".");
            }
        }

        var fields = new List<EmbedFieldBuilder>();
        foreach (var price in availablePrices)
        {
            var priceAvailabilityFormatter = (!price.IsAvailable ? "~~" : null);
            var priceIsCheapest = (price.LowestPrice == availablePrices.Min(x => x.LowestPrice));
            var priceColorFormatter = String.Empty;
            if (priceIsCheapest)
            {
                priceColorFormatter = "yaml";
            }
            else
            {
                priceColorFormatter = "fix";
            }

            var priceName = new StringBuilder(price.Type.GetDisplayName());
            if (price.QuantityAvailable > 0)
            {
                priceName.Append($" ({price.QuantityAvailable.Value.ToQuantityString()})");
            }

            var priceValue = new StringBuilder();
            priceValue.Append(priceAvailabilityFormatter);
            priceValue.Append($"```{priceColorFormatter}");
            priceValue.AppendLine();
            priceValue.Append(currency.ToPriceString(price.LowestPrice));
            if (priceIsCheapest && availablePrices.Count > 1)
            {
                priceValue.Append(" (Best Deal)");
            }
            priceValue.AppendLine();
            priceValue.Append($"```");
            priceValue.Append(priceAvailabilityFormatter);

            fields.Add(
                new EmbedFieldBuilder()
                    .WithName(priceName.ToString())
                    .WithValue(priceValue.ToString())
                    .WithIsInline(false)
            );
        }

        var embed = new EmbedBuilder()
            .WithTitle(item.Name)
            .WithUrl($"{_configuration.GetWebsiteUrl()}/item/{Uri.EscapeDataString(item.Name)}")
            .WithDescription(description.ToString())
            .WithThumbnailUrl(item.IconUrl)
            .WithImageUrl(item.PreviewUrl ?? item.IconUrl)
            .WithColor(Convert.ToUInt32(item.ForegroundColour.Trim('#'), 16))
            .WithFooter(x => x.Text = _configuration.GetWebsiteUrl());

        if (fields.Any())
        {
            embed = embed.WithFields(fields);
        }

        return InteractionResult.Success(
            embed: embed.Build()
        );
    }
}
