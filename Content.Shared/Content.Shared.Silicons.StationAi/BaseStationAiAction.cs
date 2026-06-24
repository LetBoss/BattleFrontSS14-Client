using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.StationAi;

[Serializable]
[NetSerializable]
public abstract class BaseStationAiAction
{
	[field: NonSerialized]
	public EntityUid User { get; set; }
}
