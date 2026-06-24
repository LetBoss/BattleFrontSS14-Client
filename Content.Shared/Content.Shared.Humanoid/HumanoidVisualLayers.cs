using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid;

[Serializable]
[NetSerializable]
public enum HumanoidVisualLayers : byte
{
	Special,
	Tail,
	Hair,
	FacialHair,
	UndergarmentTop,
	UndergarmentBottom,
	Chest,
	Head,
	Snout,
	HeadSide,
	HeadTop,
	Eyes,
	RArm,
	LArm,
	RHand,
	LHand,
	RLeg,
	LLeg,
	RFoot,
	LFoot,
	Handcuffs,
	StencilMask,
	Ensnare,
	Fire
}
