using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPartyMemberState
{
	public NetEntity Entity { get; }

	public string Username { get; }

	public int Level { get; }

	public float HpPercent { get; }

	public bool IsDead { get; }

	public bool IsInVoice { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public int SlotIndex { get; }

	public PubgPartyMemberState(NetEntity entity, string username, int level, float hpPercent, bool isDead, bool isInVoice, MapId mapId, Vector2 position, int slotIndex)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		Username = username;
		Level = level;
		HpPercent = hpPercent;
		IsDead = isDead;
		IsInVoice = isInVoice;
		MapId = mapId;
		Position = position;
		SlotIndex = slotIndex;
	}
}
