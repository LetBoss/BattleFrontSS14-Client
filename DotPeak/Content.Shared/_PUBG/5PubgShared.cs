// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.PubgGameEndEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.BattlePass;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG;

[NetSerializable]
[Serializable]
public sealed class PubgGameEndEvent : EntityEventArgs
{
  public int Placement { get; }

  public int Kills { get; }

  public int Deaths { get; }

  public int DamageDealt { get; }

  public int DamageTaken { get; }

  public int SurvivalTime { get; }

  public Dictionary<string, int> WeaponDamage { get; }

  public int CoinsEarned { get; }

  public int RatingChange { get; }

  public int CurrentRating { get; }

  public int NewRating { get; }

  public int NewCoins { get; }

  public string? KillerUsername { get; }

  public string KillerRank { get; }

  public List<BattlePassTaskInfo> CompletedTasks { get; }

  public int XpGained { get; }

  public int PlayerLevel { get; }

  public int PlayerXp { get; }

  public int PlayerMaxXp { get; }

  public bool IsPartyMode { get; }

  public List<PubgPartyStatsEntry> PartyStats { get; }

  public PubgGameEndEvent(
    int placement,
    int kills,
    int deaths,
    int damageDealt,
    int damageTaken,
    int survivalTime,
    Dictionary<string, int> weaponDamage,
    int coinsEarned,
    int ratingChange,
    int currentRating,
    int newRating,
    int newCoins,
    string? killerUsername = null,
    string killerRank = "N",
    List<BattlePassTaskInfo>? completedTasks = null,
    int xpGained = 0,
    int playerLevel = 1,
    int playerXp = 0,
    int playerMaxXp = 100,
    bool isPartyMode = false,
    List<PubgPartyStatsEntry>? partyStats = null)
  {
    this.Placement = placement;
    this.Kills = kills;
    this.Deaths = deaths;
    this.DamageDealt = damageDealt;
    this.DamageTaken = damageTaken;
    this.SurvivalTime = survivalTime;
    this.WeaponDamage = weaponDamage;
    this.CoinsEarned = coinsEarned;
    this.RatingChange = ratingChange;
    this.CurrentRating = currentRating;
    this.NewRating = newRating;
    this.NewCoins = newCoins;
    this.KillerUsername = killerUsername;
    this.KillerRank = killerRank;
    this.CompletedTasks = completedTasks ?? new List<BattlePassTaskInfo>();
    this.XpGained = xpGained;
    this.PlayerLevel = playerLevel;
    this.PlayerXp = playerXp;
    this.PlayerMaxXp = playerMaxXp;
    this.IsPartyMode = isPartyMode;
    this.PartyStats = partyStats ?? new List<PubgPartyStatsEntry>();
  }
}
