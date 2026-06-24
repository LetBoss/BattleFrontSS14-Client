using System.Collections.Generic;
using Content.Shared.Body.Part;

namespace Content.Shared.Humanoid;

public static class HumanoidVisualLayersExtension
{
	public static bool HasSexMorph(HumanoidVisualLayers layer)
	{
		return layer switch
		{
			HumanoidVisualLayers.Chest => true, 
			HumanoidVisualLayers.Head => true, 
			_ => false, 
		};
	}

	public static string GetSexMorph(HumanoidVisualLayers layer, Sex sex, string id)
	{
		if (!HasSexMorph(layer) || sex == Sex.Unsexed)
		{
			return id;
		}
		return $"{id}{sex}";
	}

	public static IEnumerable<HumanoidVisualLayers> Sublayers(HumanoidVisualLayers layer)
	{
		switch (layer)
		{
		case HumanoidVisualLayers.Head:
			yield return HumanoidVisualLayers.Head;
			yield return HumanoidVisualLayers.Eyes;
			yield return HumanoidVisualLayers.HeadSide;
			yield return HumanoidVisualLayers.HeadTop;
			yield return HumanoidVisualLayers.Hair;
			yield return HumanoidVisualLayers.FacialHair;
			yield return HumanoidVisualLayers.Snout;
			break;
		case HumanoidVisualLayers.LArm:
			yield return HumanoidVisualLayers.LArm;
			yield return HumanoidVisualLayers.LHand;
			break;
		case HumanoidVisualLayers.RArm:
			yield return HumanoidVisualLayers.RArm;
			yield return HumanoidVisualLayers.RHand;
			break;
		case HumanoidVisualLayers.LLeg:
			yield return HumanoidVisualLayers.LLeg;
			yield return HumanoidVisualLayers.LFoot;
			break;
		case HumanoidVisualLayers.RLeg:
			yield return HumanoidVisualLayers.RLeg;
			yield return HumanoidVisualLayers.RFoot;
			break;
		case HumanoidVisualLayers.Chest:
			yield return HumanoidVisualLayers.Chest;
			yield return HumanoidVisualLayers.Tail;
			break;
		}
	}

	public static HumanoidVisualLayers? ToHumanoidLayers(this BodyPartComponent part)
	{
		switch (part.PartType)
		{
		case BodyPartType.Torso:
			return HumanoidVisualLayers.Chest;
		case BodyPartType.Tail:
			return HumanoidVisualLayers.Tail;
		case BodyPartType.Head:
			return HumanoidVisualLayers.Head;
		case BodyPartType.Arm:
			switch (part.Symmetry)
			{
			case BodyPartSymmetry.Left:
				return HumanoidVisualLayers.LArm;
			case BodyPartSymmetry.Right:
				return HumanoidVisualLayers.RArm;
			}
			break;
		case BodyPartType.Hand:
			switch (part.Symmetry)
			{
			case BodyPartSymmetry.Left:
				return HumanoidVisualLayers.LHand;
			case BodyPartSymmetry.Right:
				return HumanoidVisualLayers.RHand;
			}
			break;
		case BodyPartType.Leg:
			switch (part.Symmetry)
			{
			case BodyPartSymmetry.Left:
				return HumanoidVisualLayers.LLeg;
			case BodyPartSymmetry.Right:
				return HumanoidVisualLayers.RLeg;
			}
			break;
		case BodyPartType.Foot:
			switch (part.Symmetry)
			{
			case BodyPartSymmetry.Left:
				return HumanoidVisualLayers.LFoot;
			case BodyPartSymmetry.Right:
				return HumanoidVisualLayers.RFoot;
			}
			break;
		}
		return null;
	}
}
