using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Weapons;

[Serializable]
[NetSerializable]
public sealed class ShellTypeEntry
{
	public readonly EntProtoId ProtoId;

	public readonly string Name;

	public readonly string Description;

	public readonly bool IsAvailable;

	public ShellTypeEntry(EntProtoId protoId, string name, string description, bool isAvailable)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ProtoId = protoId;
		Name = name;
		Description = description;
		IsAvailable = isAvailable;
	}
}
