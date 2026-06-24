using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Body.Part;

[Serializable]
[NetSerializable]
public enum BodyPartType
{
	Other,
	Torso,
	Head,
	Arm,
	Hand,
	Leg,
	Foot,
	Tail
}
