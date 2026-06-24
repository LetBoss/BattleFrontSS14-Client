using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared._CIV14merka.Mortar;

[ByRefEvent]
public record struct CivMortarInterceptAttemptEvent(int TeamId, MapCoordinates Source, MapCoordinates Target, float PopupRange)
{
	public bool Cancelled = false;
}
