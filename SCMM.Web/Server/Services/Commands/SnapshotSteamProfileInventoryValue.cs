﻿using CommandQuery;
using Microsoft.EntityFrameworkCore;
using SCMM.Web.Server.Data;
using SCMM.Web.Server.Data.Models.Steam;
using SCMM.Web.Server.Services.Queries;
using SCMM.Web.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SCMM.Web.Server.Services.Commands
{
    public class SnapshotSteamProfileInventoryValueRequest : ICommand
    {
        public string ProfileId { get; set; }

        public string CurrencyId { get; set; }

        public GetSteamProfileInventoryTotalsResponse InventoryTotals { get; set; }
    }

    public class SnapshotSteamProfileInventoryValue : ICommandHandler<SnapshotSteamProfileInventoryValueRequest>
    {
        private readonly ScmmDbContext _db;
        private readonly IQueryProcessor _queryProcessor;

        public SnapshotSteamProfileInventoryValue(ScmmDbContext db, IQueryProcessor queryProcessor)
        {
            _db = db;
            _queryProcessor = queryProcessor;
        }

        public async Task HandleAsync(SnapshotSteamProfileInventoryValueRequest request)
        {
            if (request.InventoryTotals == null)
            {
                return;
            }

            // Resolve the id
            var resolvedId = await _queryProcessor.ProcessAsync(new ResolveSteamIdRequest()
            {
                Id = request.ProfileId
            });

            // Load the profile
            var profile = _db.SteamProfiles.Find(resolvedId.Id);
            if (profile == null)
            {
                return;
            }

            // Load the currency
            var currency = _db.SteamCurrencies
                .AsNoTracking()
                .FirstOrDefault(x => x.SteamId == request.CurrencyId);

            if (currency == null)
            {
                return;
            }

            // Snapshot the inventory value if it has been more than an hour since the last snapshot
            profile.LastSnapshotInventoryOn = DateTimeOffset.Now;
            profile.InventorySnapshots.Add(new SteamProfileInventorySnapshot()
            {
                Profile = profile,
                Timestamp = DateTimeOffset.UtcNow,
                Currency = currency,
                InvestedValue = currency.CalculateExchange(request.InventoryTotals.TotalInvested ?? 0),
                MarketValue = currency.CalculateExchange(request.InventoryTotals.TotalMarketValue),
                TotalItems = request.InventoryTotals.TotalItems
            });
        }
    }
}
