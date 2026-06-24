using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Chemistry.ChemMaster;

[Serializable]
[NetSerializable]
public sealed class RMCChemMasterPillBottleColorMsg(RMCPillBottleColors color) : BoundUserInterfaceMessage
{
	public readonly RMCPillBottleColors Color = color;
}
