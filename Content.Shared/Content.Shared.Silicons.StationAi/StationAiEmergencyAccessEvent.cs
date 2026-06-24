using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.StationAi;

[Serializable]
[NetSerializable]
public sealed class StationAiEmergencyAccessEvent : BaseStationAiAction
{
	public bool EmergencyAccess;
}
