using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Medical.Surgery;

[Serializable]
[NetSerializable]
public sealed class CMSurgeryStepChosenBuiMsg(NetEntity part, EntProtoId surgery, EntProtoId step) : BoundUserInterfaceMessage
{
	public readonly NetEntity Part = part;

	public readonly EntProtoId Surgery = surgery;

	public readonly EntProtoId Step = step;
}
