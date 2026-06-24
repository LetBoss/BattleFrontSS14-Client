using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Directions;

public static class SharedDirectionExtensions
{
	public static EntityCoordinates Offset(this EntityCoordinates coordinates, Direction direction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ((EntityCoordinates)(ref coordinates)).Offset(DirectionExtensions.ToVec(direction));
	}
}
