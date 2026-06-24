using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.SupplyDrop;

[Serializable]
[NetSerializable]
public sealed class SupplyDropComputerLatitudeBuiMsg(int latitude) : BoundUserInterfaceMessage
{
	public int Latitude = latitude;
}
