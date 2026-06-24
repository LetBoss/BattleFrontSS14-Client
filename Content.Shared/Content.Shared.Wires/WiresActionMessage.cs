using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires;

[Serializable]
[NetSerializable]
public sealed class WiresActionMessage : BoundUserInterfaceMessage
{
	public readonly int Id;

	public readonly WiresAction Action;

	public WiresActionMessage(int id, WiresAction action)
	{
		Id = id;
		Action = action;
	}
}
