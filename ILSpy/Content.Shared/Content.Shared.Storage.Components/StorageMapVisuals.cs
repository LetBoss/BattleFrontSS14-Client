using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage.Components;

[Serializable]
[NetSerializable]
public enum StorageMapVisuals : sbyte
{
	InitLayers,
	LayerChanged
}
