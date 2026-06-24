using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Radio;

[Serializable]
[NetSerializable]
public enum RadioDeviceVisualLayers : byte
{
	Broadcasting,
	Speaker
}
