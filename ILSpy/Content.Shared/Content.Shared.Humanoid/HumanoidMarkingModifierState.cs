using System;
using System.Collections.Generic;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid;

[Serializable]
[NetSerializable]
public sealed class HumanoidMarkingModifierState : BoundUserInterfaceState
{
	public MarkingSet MarkingSet { get; }

	public string Species { get; }

	public Sex Sex { get; }

	public Color SkinColor { get; }

	public Color EyeColor { get; }

	public Color? HairColor { get; }

	public Color? FacialHairColor { get; }

	public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers { get; }

	public HumanoidMarkingModifierState(MarkingSet markingSet, string species, Sex sex, Color skinColor, Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> customBaseLayers)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		MarkingSet = markingSet;
		Species = species;
		Sex = sex;
		SkinColor = skinColor;
		CustomBaseLayers = customBaseLayers;
	}
}
