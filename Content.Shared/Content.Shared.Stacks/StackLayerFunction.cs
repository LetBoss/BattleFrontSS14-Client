using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Stacks;

[Serializable]
[NetSerializable]
public enum StackLayerFunction : byte
{
	None,
	Threshold
}
