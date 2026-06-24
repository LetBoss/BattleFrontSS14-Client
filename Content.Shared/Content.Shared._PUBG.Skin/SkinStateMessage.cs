using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SkinStateMessage : EntityEventArgs
{
	public Dictionary<string, List<string>> AllItems { get; }

	public Dictionary<string, List<string>> UnlockedItems { get; }

	public Dictionary<string, DateTime?> ItemExpiresAt { get; }

	public Dictionary<string, int> RecipePrices { get; }

	public List<SkinShopItemInfo> ShopItems { get; }

	public Dictionary<string, string> CurrentOutfit { get; }

	public int PlayerCoins { get; }

	public int PlayerScrap { get; }

	public int PlayerPremiumCoins { get; }

	public List<string> AllEmotes { get; }

	public List<string> UnlockedEmotes { get; }

	public List<string> EquippedEmotes { get; }

	public int MaxEmoteSlots { get; }

	public int TotalCaseDropSkins { get; }

	public int UnlockedCaseDropSkins { get; }

	public int TotalUniqueSkins { get; }

	public int TotalEmotes { get; }

	public int AvailableEmotes { get; }

	public int TotalGames { get; }

	public int Wins { get; }

	public int TotalKills { get; }

	public int TotalDamage { get; }

	public int AvgSurvivalTime { get; }

	public int TotalSurvivalTime { get; }

	public Dictionary<string, int> SponsorPermissions { get; }

	public List<SponsorPermissionDetailInfo> SponsorPermissionDetails { get; }

	public SponsorTierInfo? SponsorDisplayTier { get; }

	public List<SponsorActiveTierInfo> SponsorActiveTiers { get; }

	public SponsorDisplayMode SponsorDisplayMode { get; }

	public string? SponsorPreferredTierKey { get; }

	public List<LeaderboardEntryInfo> Leaderboard { get; }

	public int PlayerRank { get; }

	public int PlayerRating { get; }

	public int Reputation { get; }

	public List<MatchHistoryInfo> MatchHistory { get; }

	public int TotalDeaths { get; }

	public int PlayerLevel { get; }

	public int PlayerXp { get; }

	public int PlayerMaxXp { get; }

	public SkinStateMessage(Dictionary<string, List<string>> allItems, Dictionary<string, List<string>> unlockedItems, Dictionary<string, DateTime?> itemExpiresAt, Dictionary<string, int> recipePrices, List<SkinShopItemInfo> shopItems, Dictionary<string, string> currentOutfit, int playerCoins, int playerScrap, int playerPremiumCoins, List<string> allEmotes, List<string> unlockedEmotes, List<string> equippedEmotes, int maxEmoteSlots, int totalCaseDropSkins, int unlockedCaseDropSkins, int totalUniqueSkins, int totalEmotes, int availableEmotes, Dictionary<string, int> sponsorPermissions, List<SponsorPermissionDetailInfo> sponsorPermissionDetails, SponsorTierInfo? sponsorDisplayTier, List<SponsorActiveTierInfo> sponsorActiveTiers, SponsorDisplayMode sponsorDisplayMode, string? sponsorPreferredTierKey, int totalGames, int wins, int totalKills, int totalDamage, int avgSurvivalTime, int totalSurvivalTime, List<LeaderboardEntryInfo> leaderboard, int playerRank, int playerRating, int reputation, List<MatchHistoryInfo> matchHistory, int totalDeaths, int playerLevel = 1, int playerXp = 0, int playerMaxXp = 100)
	{
		AllItems = allItems;
		UnlockedItems = unlockedItems;
		ItemExpiresAt = itemExpiresAt;
		RecipePrices = recipePrices;
		ShopItems = shopItems;
		CurrentOutfit = currentOutfit;
		PlayerCoins = playerCoins;
		PlayerScrap = playerScrap;
		PlayerPremiumCoins = playerPremiumCoins;
		AllEmotes = allEmotes;
		UnlockedEmotes = unlockedEmotes;
		EquippedEmotes = equippedEmotes;
		MaxEmoteSlots = maxEmoteSlots;
		TotalCaseDropSkins = totalCaseDropSkins;
		UnlockedCaseDropSkins = unlockedCaseDropSkins;
		TotalUniqueSkins = totalUniqueSkins;
		TotalEmotes = totalEmotes;
		AvailableEmotes = availableEmotes;
		SponsorPermissions = sponsorPermissions;
		SponsorPermissionDetails = sponsorPermissionDetails;
		SponsorDisplayTier = sponsorDisplayTier;
		SponsorActiveTiers = sponsorActiveTiers;
		SponsorDisplayMode = sponsorDisplayMode;
		SponsorPreferredTierKey = sponsorPreferredTierKey;
		TotalGames = totalGames;
		Wins = wins;
		TotalKills = totalKills;
		TotalDamage = totalDamage;
		AvgSurvivalTime = avgSurvivalTime;
		TotalSurvivalTime = totalSurvivalTime;
		Leaderboard = leaderboard;
		PlayerRank = playerRank;
		PlayerRating = playerRating;
		Reputation = reputation;
		MatchHistory = matchHistory;
		TotalDeaths = totalDeaths;
		PlayerLevel = playerLevel;
		PlayerXp = playerXp;
		PlayerMaxXp = playerMaxXp;
	}
}
