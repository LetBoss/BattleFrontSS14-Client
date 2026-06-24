using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Thief;

[Serializable]
[NetSerializable]
public sealed class ThiefBackpackChangeSetMessage : BoundUserInterfaceMessage
{
	public readonly int SetNumber;

	public ThiefBackpackChangeSetMessage(int setNumber)
	{
		SetNumber = setNumber;
	}
}
