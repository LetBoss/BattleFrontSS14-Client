using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Coordinates;

public static class EntityCoordinatesExtensions
{
	public static EntityCoordinates ToCoordinates(this EntityUid id)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return new EntityCoordinates(id, new Vector2(0f, 0f));
	}

	public static EntityCoordinates ToCoordinates(this EntityUid id, Vector2 offset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return new EntityCoordinates(id, offset);
	}

	public static EntityCoordinates ToCoordinates(this EntityUid id, float x, float y)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return new EntityCoordinates(id, x, y);
	}
}
