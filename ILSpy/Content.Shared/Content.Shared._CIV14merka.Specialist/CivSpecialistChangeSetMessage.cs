using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Specialist;

[Serializable]
[NetSerializable]
public sealed class CivSpecialistChangeSetMessage : BoundUserInterfaceMessage
{
	public readonly int SetNumber;

	public CivSpecialistChangeSetMessage(int setNumber)
	{
		SetNumber = setNumber;
	}
}
