using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components;

[Serializable]
[NetSerializable]
public enum AirAlarmMode
{
	None,
	Filtering,
	WideFiltering,
	Fill,
	Panic
}
