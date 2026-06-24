// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.SkinStateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Skin;

[NetSerializable]
[Serializable]
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

  public SkinStateMessage(
    Dictionary<string, List<string>> allItems,
    Dictionary<string, List<string>> unlockedItems,
    Dictionary<string, DateTime?> itemExpiresAt,
    Dictionary<string, int> recipePrices,
    List<SkinShopItemInfo> shopItems,
    Dictionary<string, string> currentOutfit,
    int playerCoins,
    int playerScrap,
    int playerPremiumCoins,
    List<string> allEmotes,
    List<string> unlockedEmotes,
    List<string> equippedEmotes,
    int maxEmoteSlots,
    int totalCaseDropSkins,
    int unlockedCaseDropSkins,
    int totalUniqueSkins,
    int totalEmotes,
    int availableEmotes,
    Dictionary<string, int> sponsorPermissions,
    List<SponsorPermissionDetailInfo> sponsorPermissionDetails,
    SponsorTierInfo? sponsorDisplayTier,
    List<SponsorActiveTierInfo> sponsorActiveTiers,
    SponsorDisplayMode sponsorDisplayMode,
    string? sponsorPreferredTierKey,
    int totalGames,
    int wins,
    int totalKills,
    int totalDamage,
    int avgSurvivalTime,
    int totalSurvivalTime,
    List<LeaderboardEntryInfo> leaderboard,
    int playerRank,
    int playerRating,
    int reputation,
    List<MatchHistoryInfo> matchHistory,
    int totalDeaths,
    int playerLevel = 1,
    int playerXp = 0,
    int playerMaxXp = 100)
  {
    this.AllItems = allItems;
    this.UnlockedItems = unlockedItems;
    this.ItemExpiresAt = itemExpiresAt;
    this.RecipePrices = recipePrices;
    this.ShopItems = shopItems;
    this.CurrentOutfit = currentOutfit;
    this.PlayerCoins = playerCoins;
    this.PlayerScrap = playerScrap;
    this.PlayerPremiumCoins = playerPremiumCoins;
    this.AllEmotes = allEmotes;
    this.UnlockedEmotes = unlockedEmotes;
    this.EquippedEmotes = equippedEmotes;
    this.MaxEmoteSlots = maxEmoteSlots;
    this.TotalCaseDropSkins = totalCaseDropSkins;
    this.UnlockedCaseDropSkins = unlockedCaseDropSkins;
    this.TotalUniqueSkins = totalUniqueSkins;
    this.TotalEmotes = totalEmotes;
    this.AvailableEmotes = availableEmotes;
    this.SponsorPermissions = sponsorPermissions;
    this.SponsorPermissionDetails = sponsorPermissionDetails;
    this.SponsorDisplayTier = sponsorDisplayTier;
    this.SponsorActiveTiers = sponsorActiveTiers;
    this.SponsorDisplayMode = sponsorDisplayMode;
    this.SponsorPreferredTierKey = sponsorPreferredTierKey;
    this.TotalGames = totalGames;
    this.Wins = wins;
    this.TotalKills = totalKills;
    this.TotalDamage = totalDamage;
    this.AvgSurvivalTime = avgSurvivalTime;
    this.TotalSurvivalTime = totalSurvivalTime;
    this.Leaderboard = leaderboard;
    this.PlayerRank = playerRank;
    this.PlayerRating = playerRating;
    this.Reputation = reputation;
    this.MatchHistory = matchHistory;
    this.TotalDeaths = totalDeaths;
    this.PlayerLevel = playerLevel;
    this.PlayerXp = playerXp;
    this.PlayerMaxXp = playerMaxXp;
  }
}
