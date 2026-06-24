using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Body.Part;

[Serializable]
[NetSerializable]
[DataRecord]
public struct OrganSlot(string id)
{
	public string Id = id;
}
