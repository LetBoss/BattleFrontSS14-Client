using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Silicons.StationAi;

[ByRefEvent]
public record struct GetStationAiRadialEvent()
{
	public List<StationAiRadial> Actions = new List<StationAiRadial>();
}
