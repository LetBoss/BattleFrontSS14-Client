using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Ame.Components;

[Serializable]
[NetSerializable]
public enum AmeControllerState
{
	On,
	Warning,
	Critical,
	Fuck,
	Off
}
