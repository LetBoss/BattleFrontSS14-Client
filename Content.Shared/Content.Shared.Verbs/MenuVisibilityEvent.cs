using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Verbs;

[ByRefEvent]
public record struct MenuVisibilityEvent
{
	public MapCoordinates TargetPos;

	public MenuVisibility Visibility;
}
