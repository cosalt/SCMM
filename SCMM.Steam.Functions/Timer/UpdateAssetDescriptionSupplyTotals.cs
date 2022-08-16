﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using SCMM.Steam.Data.Store;

namespace SCMM.Steam.Functions.Timer;

public class UpdateAssetDescriptionSupplyTotals
{
    private readonly SteamDbContext _db;

    public UpdateAssetDescriptionSupplyTotals(SteamDbContext db)
    {
        _db = db;
    }

    [Function("Update-Asset-Description-Supply-Totals")]
    public async Task Run([TimerTrigger("0 20 * * * *")] /* every hour, 20 minutes past the hour */ TimerInfo timerInfo, FunctionContext context)
    {
        using (var transaction = await _db.Database.BeginTransactionAsync())
        {
            _db.Database.SetCommandTimeout(300); // 5min
            await _db.Database.ExecuteSqlInterpolatedAsync(@$"
                ; WITH AssetDescriptionSupplyTotals ([Id], SupplyTotalOwnersEstimated, SupplyTotalOwnersKnown, SupplyTotalInvestorsEstimated, SupplyTotalInvestorsKnown, SupplyTotalMarketsKnown)
                AS (
	                SELECT
		                a.Id,
		                (
			                a.SubscriptionsLifetime
		                ) AS SupplyTotalOwnersEstimated,
		                (
			                SELECT COUNT(DISTINCT(i.ProfileId))
			                FROM [SteamProfileInventoryItems] i 
			                WHERE i.DescriptionId = a.Id
		                ) AS SupplyTotalOwnersKnown,
		                (
			                a.SupplyTotalInvestorsEstimated
		                ) AS SupplyTotalInvestorsEstimated,
		                (
			                SELECT (SUM(i.Quantity) - COUNT(DISTINCT(i.ProfileId)))
			                FROM [SteamProfileInventoryItems] i 
			                WHERE i.DescriptionId = a.Id
		                ) AS SupplyTotalInvestorsKnown,
		                (
			                SELECT TOP 1 m.BuyPricesTotalSupply
			                FROM [SteamMarketItems] m
			                WHERE m.DescriptionId = a.Id
		                ) AS SupplyTotalMarketsKnown
	                FROM [SteamAssetDescriptions] a
                )
                UPDATE a
                SET
	                [SupplyTotalOwnersEstimated] = s.SupplyTotalOwnersEstimated,
	                [SupplyTotalOwnersKnown] = s.SupplyTotalOwnersKnown,
	                [SupplyTotalInvestorsKnown] = s.SupplyTotalInvestorsKnown,
	                [SupplyTotalMarketsKnown] = s.SupplyTotalMarketsKnown,
	                [SupplyTotalEstimated] = (
		                (ISNULL(s.SupplyTotalOwnersKnown, 0) + IIF(s.SupplyTotalOwnersKnown > s.SupplyTotalOwnersEstimated, 0, ISNULL((s.SupplyTotalOwnersEstimated - s.SupplyTotalOwnersKnown), 0))) +
		                (ISNULL(s.SupplyTotalInvestorsKnown, 0) + IIF(s.SupplyTotalInvestorsKnown > s.SupplyTotalInvestorsEstimated, 0, ISNULL((s.SupplyTotalInvestorsEstimated - s.SupplyTotalInvestorsKnown), 0))) +
		                ISNULL(s.SupplyTotalMarketsKnown, 0)
	                )
                FROM AssetDescriptionSupplyTotals s
	                INNER JOIN [SteamAssetDescriptions] a ON a.Id = s.Id
            ");

            await transaction.CommitAsync();
        }
    }
}
