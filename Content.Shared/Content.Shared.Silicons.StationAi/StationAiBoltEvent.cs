using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.StationAi;

[Serializable]
[NetSerializable]
public sealed class StationAiBoltEvent : BaseStationAiAction
{
	public bool Bolted;
}
