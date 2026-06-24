using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Crayon;

[Serializable]
[NetSerializable]
public sealed class CrayonUsedMessage : BoundUserInterfaceMessage
{
	public readonly string DrawnDecal;

	public CrayonUsedMessage(string drawn)
	{
		DrawnDecal = drawn;
	}
}
