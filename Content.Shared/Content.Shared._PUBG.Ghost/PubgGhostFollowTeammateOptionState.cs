using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Ghost;

[Serializable]
[NetSerializable]
public sealed class PubgGhostFollowTeammateOptionState
{
	public NetEntity Entity { get; }

	public string Name { get; }

	public int SlotIndex { get; }

	public PubgGhostFollowTeammateOptionState(NetEntity entity, string name, int slotIndex)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		Name = name;
		SlotIndex = slotIndex;
	}
}
