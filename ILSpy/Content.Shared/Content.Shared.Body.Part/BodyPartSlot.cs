using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Body.Part;

[Serializable]
[NetSerializable]
[DataRecord]
public struct BodyPartSlot(string id, BodyPartType type)
{
	public string Id = id;

	public BodyPartType Type = type;
}
