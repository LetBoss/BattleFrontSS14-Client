using System;
using Robust.Shared.Serialization;

namespace Content.Shared.TextScreen;

[Serializable]
[NetSerializable]
public enum TextScreenVisuals : byte
{
	DefaultText,
	ScreenText,
	TargetTime,
	Color
}
