using System;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems;

[Serializable]
[NetSerializable]
public sealed class AgentIDCardJobIconChangedMessage : BoundUserInterfaceMessage
{
	public ProtoId<JobIconPrototype> JobIconId { get; }

	public AgentIDCardJobIconChangedMessage(ProtoId<JobIconPrototype> jobIconId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		JobIconId = jobIconId;
	}
}
