using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Body.Part;

[Serializable]
[NetSerializable]
public enum BodyPartSymmetry
{
	None,
	Left,
	Right
}
