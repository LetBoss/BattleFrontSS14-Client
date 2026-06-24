using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
[DataRecord]
public sealed class NanoTaskUpdateTask : INanoTaskUiMessagePayload
{
	public readonly NanoTaskItemAndId Item;

	public NanoTaskUpdateTask(NanoTaskItemAndId item)
	{
		Item = item;
	}
}
