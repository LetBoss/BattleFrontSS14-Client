using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.StationAi;

[Serializable]
[NetSerializable]
public sealed class StationAiElectrifiedEvent : BaseStationAiAction
{
	public bool Electrified;
}
