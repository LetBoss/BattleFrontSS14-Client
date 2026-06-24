using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Implants;

[Serializable]
[NetSerializable]
public sealed class DeimplantChangeVerbMessage : BoundUserInterfaceMessage
{
	public readonly string? Implant;

	public DeimplantChangeVerbMessage(string? implant)
	{
		Implant = implant;
	}
}
