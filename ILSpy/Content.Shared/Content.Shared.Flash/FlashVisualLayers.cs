using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Flash;

[Serializable]
[NetSerializable]
public enum FlashVisualLayers : byte
{
	BaseLayer,
	LightLayer
}
