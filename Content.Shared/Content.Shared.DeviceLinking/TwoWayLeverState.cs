using System;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceLinking;

[Serializable]
[NetSerializable]
public enum TwoWayLeverState : byte
{
	Middle,
	Right,
	Left
}
