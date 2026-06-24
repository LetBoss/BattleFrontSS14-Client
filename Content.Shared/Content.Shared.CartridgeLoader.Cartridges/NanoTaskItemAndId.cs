using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
[DataRecord]
public sealed class NanoTaskItemAndId
{
	public readonly int Id;

	public readonly NanoTaskItem Data;

	public NanoTaskItemAndId(int id, NanoTaskItem data)
	{
		Id = id;
		Data = data;
	}
}
