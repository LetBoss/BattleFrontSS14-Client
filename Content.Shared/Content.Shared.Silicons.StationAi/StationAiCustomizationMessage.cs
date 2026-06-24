using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.StationAi;

[Serializable]
[NetSerializable]
public sealed class StationAiCustomizationMessage : BoundUserInterfaceMessage
{
	public readonly ProtoId<StationAiCustomizationGroupPrototype> GroupProtoId;

	public readonly ProtoId<StationAiCustomizationPrototype> CustomizationProtoId;

	public StationAiCustomizationMessage(ProtoId<StationAiCustomizationGroupPrototype> groupProtoId, ProtoId<StationAiCustomizationPrototype> customizationProtoId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		GroupProtoId = groupProtoId;
		CustomizationProtoId = customizationProtoId;
	}
}
