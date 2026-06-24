using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Nutrition.Components;

[Serializable]
[NetSerializable]
public enum SealableVisuals : byte
{
	Sealed,
	Layer
}
