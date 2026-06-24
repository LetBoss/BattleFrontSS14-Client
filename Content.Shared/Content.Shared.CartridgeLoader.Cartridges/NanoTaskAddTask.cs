using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
[DataRecord]
public sealed class NanoTaskAddTask : INanoTaskUiMessagePayload
{
	public readonly NanoTaskItem Item;

	public NanoTaskAddTask(NanoTaskItem item)
	{
		Item = item;
	}
}
