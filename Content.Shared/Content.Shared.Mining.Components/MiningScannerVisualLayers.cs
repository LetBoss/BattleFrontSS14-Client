using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Mining.Components;

[Serializable]
[NetSerializable]
public enum MiningScannerVisualLayers : byte
{
	Overlay
}
