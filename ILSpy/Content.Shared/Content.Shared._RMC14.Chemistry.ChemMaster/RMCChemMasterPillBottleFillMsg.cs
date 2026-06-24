using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Chemistry.ChemMaster;

[Serializable]
[NetSerializable]
public sealed class RMCChemMasterPillBottleFillMsg(NetEntity bottle, bool fill) : BoundUserInterfaceMessage
{
	public readonly NetEntity Bottle = bottle;

	public readonly bool Fill = fill;
}
