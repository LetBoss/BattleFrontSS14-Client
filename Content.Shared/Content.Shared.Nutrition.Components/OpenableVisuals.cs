using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Nutrition.Components;

[Serializable]
[NetSerializable]
public enum OpenableVisuals : byte
{
	Opened,
	Layer
}
