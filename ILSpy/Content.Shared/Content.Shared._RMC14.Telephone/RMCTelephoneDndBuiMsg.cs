using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Telephone;

[Serializable]
[NetSerializable]
public sealed class RMCTelephoneDndBuiMsg(bool dnd) : BoundUserInterfaceMessage
{
	public readonly bool Dnd = dnd;
}
