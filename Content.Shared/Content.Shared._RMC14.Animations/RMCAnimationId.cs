using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Animations;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct RMCAnimationId(string Id) : ISelfSerialize
{
	public void Deserialize(string value)
	{
		Id = value;
	}

	public string Serialize()
	{
		return Id;
	}
}
