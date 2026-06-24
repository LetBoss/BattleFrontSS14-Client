using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Disposal;

[Serializable]
[NetSerializable]
public sealed class TargetSelectedMessage : BoundUserInterfaceMessage
{
	public readonly string? Target;

	public TargetSelectedMessage(string? target)
	{
		Target = target;
	}
}
