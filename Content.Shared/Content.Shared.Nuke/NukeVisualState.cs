using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Nuke;

[Serializable]
[NetSerializable]
public enum NukeVisualState
{
	Idle,
	Armed,
	YoureFucked
}
