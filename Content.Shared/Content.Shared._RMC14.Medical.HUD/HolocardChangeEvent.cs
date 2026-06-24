using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Medical.HUD;

[Serializable]
[NetSerializable]
public sealed class HolocardChangeEvent(NetEntity owner, HolocardStatus newHolocardStatus) : BoundUserInterfaceMessage
{
	public HolocardStatus NewHolocardStatus = newHolocardStatus;

	public NetEntity Owner = owner;
}
