using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class PubgPartyStatsEntry
{
	public string Username { get; }

	public int Kills { get; }

	public int DamageDealt { get; }

	public int DamageTaken { get; }

	public NetEntity? PlayerNetEntity { get; }

	public PubgPartyStatsEntry(string username, int kills, int damageDealt, int damageTaken, NetEntity? playerNetEntity = null)
	{
		Username = username;
		Kills = kills;
		DamageDealt = damageDealt;
		DamageTaken = damageTaken;
		PlayerNetEntity = playerNetEntity;
	}
}
