using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Crayon;

[Serializable]
[NetSerializable]
public sealed class CrayonSelectMessage : BoundUserInterfaceMessage
{
	public readonly string State;

	public CrayonSelectMessage(string selected)
	{
		State = selected;
	}
}
