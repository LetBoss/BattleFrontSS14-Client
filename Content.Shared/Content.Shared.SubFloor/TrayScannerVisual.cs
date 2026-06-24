using System;
using Robust.Shared.Serialization;

namespace Content.Shared.SubFloor;

[Serializable]
[NetSerializable]
public enum TrayScannerVisual : sbyte
{
	Visual,
	On,
	Off
}
