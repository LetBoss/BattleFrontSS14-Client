using System;
using System.Collections.Generic;
using Content.Shared._PUBG.BattlePass;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
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

	public PubgGameEndEvent(int placement, int kills, int deaths, int damageDealt, int damageTaken, int survivalTime, Dictionary<string, int> weaponDamage, int coinsEarned, int ratingChange, int currentRating, int newRating, int newCoins, string? killerUsername = null, string killerRank = "N", List<BattlePassTaskInfo>? completedTasks = null, int xpGained = 0, int playerLevel = 1, int playerXp = 0, int playerMaxXp = 100, bool isPartyMode = false, List<PubgPartyStatsEntry>? partyStats = null)
	{
		Placement = placement;
		Kills = kills;
		Deaths = deaths;
		DamageDealt = damageDealt;
		DamageTaken = damageTaken;
		SurvivalTime = survivalTime;
		WeaponDamage = weaponDamage;
		CoinsEarned = coinsEarned;
		RatingChange = ratingChange;
		CurrentRating = currentRating;
		NewRating = newRating;
		NewCoins = newCoins;
		KillerUsername = killerUsername;
		KillerRank = killerRank;
		CompletedTasks = completedTasks ?? new List<BattlePassTaskInfo>();
		XpGained = xpGained;
		PlayerLevel = playerLevel;
		PlayerXp = playerXp;
		PlayerMaxXp = playerMaxXp;
		IsPartyMode = isPartyMode;
		PartyStats = partyStats ?? new List<PubgPartyStatsEntry>();
	}
}
