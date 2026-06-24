using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
[DataRecord]
public sealed class PulledAccessLog
{
	public readonly TimeSpan Time;

	public readonly string Accessor;

	public PulledAccessLog(TimeSpan time, string accessor)
	{
		Time = time;
		Accessor = accessor;
	}
}
