using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.StationAi;

[Serializable]
[NetSerializable]
public sealed class StationAiRadialMessage : BoundUserInterfaceMessage
{
	public BaseStationAiAction Event;
}
