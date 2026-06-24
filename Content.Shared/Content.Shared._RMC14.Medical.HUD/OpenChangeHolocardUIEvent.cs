using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Medical.HUD;

[Serializable]
[NetSerializable]
public sealed class OpenChangeHolocardUIEvent : BoundUserInterfaceMessage
{
	public NetEntity Owner;

	public NetEntity Target;

	public OpenChangeHolocardUIEvent(NetEntity owner, NetEntity target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Owner = owner;
		Target = target;
	}
}
