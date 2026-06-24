using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Loadout;

[Serializable]
[NetSerializable]
public sealed class PubgLoadoutActionRequestMessage : EntityEventArgs
{
	public PubgLoadoutActionType Action { get; }

	public NetEntity Item { get; }

	public PubgLoadoutSection TargetSection { get; }

	public NetEntity TargetWeapon { get; }

	public string TargetSlotId { get; }

	public PubgLoadoutActionRequestMessage(PubgLoadoutActionType action, NetEntity item, PubgLoadoutSection targetSection = PubgLoadoutSection.Other, NetEntity targetWeapon = default(NetEntity), string targetSlotId = "")
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Action = action;
		Item = item;
		TargetSection = targetSection;
		TargetWeapon = targetWeapon;
		TargetSlotId = targetSlotId;
	}
}
