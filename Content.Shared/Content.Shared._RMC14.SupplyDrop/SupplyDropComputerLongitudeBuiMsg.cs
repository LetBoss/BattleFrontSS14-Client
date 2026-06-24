using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.SupplyDrop;

[Serializable]
[NetSerializable]
public sealed class SupplyDropComputerLongitudeBuiMsg(int longitude) : BoundUserInterfaceMessage
{
	public int Longitude = longitude;
}
