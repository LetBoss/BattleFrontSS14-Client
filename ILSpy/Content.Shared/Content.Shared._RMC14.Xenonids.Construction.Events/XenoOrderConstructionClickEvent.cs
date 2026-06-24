using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Construction.Events;

[Serializable]
[NetSerializable]
public sealed class XenoOrderConstructionClickEvent : EntityEventArgs
{
	public NetCoordinates Target { get; }

	public EntProtoId StructureId { get; }

	public XenoOrderConstructionClickEvent(NetCoordinates target, EntProtoId structureId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
		StructureId = structureId;
	}
}
