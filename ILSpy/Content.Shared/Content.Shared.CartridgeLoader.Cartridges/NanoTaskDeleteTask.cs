using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
[DataRecord]
public sealed class NanoTaskDeleteTask : INanoTaskUiMessagePayload
{
	public readonly int Id;

	public NanoTaskDeleteTask(int id)
	{
		Id = id;
	}
}
