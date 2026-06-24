using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
[DataRecord]
public sealed class NanoTaskPrintTask : INanoTaskUiMessagePayload
{
	public readonly NanoTaskItem Item;

	public NanoTaskPrintTask(NanoTaskItem item)
	{
		Item = item;
	}
}
