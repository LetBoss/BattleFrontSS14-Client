using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid;

[Serializable]
[NetSerializable]
public sealed class HumanoidMarkingModifierBaseLayersSetMessage : BoundUserInterfaceMessage
{
	public HumanoidVisualLayers Layer { get; }

	public CustomBaseLayerInfo? Info { get; }

	public bool ResendState { get; }

	public HumanoidMarkingModifierBaseLayersSetMessage(HumanoidVisualLayers layer, CustomBaseLayerInfo? info, bool resendState)
	{
		Layer = layer;
		Info = info;
		ResendState = resendState;
	}
}
