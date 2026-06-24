using System;
using Content.Shared._RMC14.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Construction;

[Serializable]
[NetSerializable]
public sealed class RMCConstructionBuiMsg(ProtoId<RMCConstructionPrototype> build, int amount) : BoundUserInterfaceMessage
{
	public readonly ProtoId<RMCConstructionPrototype> Build = build;

	public readonly int Amount = amount;
}
