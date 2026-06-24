using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Evolution;

[Serializable]
[NetSerializable]
public sealed class XenoStrainBuiMsg(EntProtoId choice) : BoundUserInterfaceMessage
{
	public readonly EntProtoId Choice = choice;
}
