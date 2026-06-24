using Robust.Client.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Pinpointer.UI;

public struct NavMapBlip(EntityCoordinates coordinates, Texture texture, Color color, bool blinks, bool selectable = true, float scale = 1f)
{
	public EntityCoordinates Coordinates = coordinates;

	public Texture Texture = texture;

	public Color Color = color;

	public bool Blinks = blinks;

	public bool Selectable = selectable;

	public float Scale = scale;
}
