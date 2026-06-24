using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Ame.Components;

[Serializable]
[NetSerializable]
public enum AmeCoreState
{
	Off,
	Weak,
	Strong
}
