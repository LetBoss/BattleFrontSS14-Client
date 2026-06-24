using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Projectile.Parasite;

[Serializable]
[NetSerializable]
public sealed class XenoChangeParasiteReserveMessage : BoundUserInterfaceMessage
{
	public int NewReserve;

	public XenoChangeParasiteReserveMessage(int newReserve)
	{
		NewReserve = newReserve;
	}
}
