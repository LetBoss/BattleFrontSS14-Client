using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable]
[NetSerializable]
public sealed class GhostRoleRadioMessage : BoundUserInterfaceMessage
{
	public ProtoId<GhostRolePrototype> ProtoId;

	public GhostRoleRadioMessage(ProtoId<GhostRolePrototype> protoId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ProtoId = protoId;
	}
}
