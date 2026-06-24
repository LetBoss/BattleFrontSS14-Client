using System;
using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer;

[Serializable]
[NetSerializable]
public sealed class NavMapRegionOverlay(Enum uiKey, List<(Vector2i, Vector2i)> gridCoords)
{
	public Enum UiKey = uiKey;

	public List<(Vector2i, Vector2i)> GridCoords = gridCoords;

	public Color Color = Color.White;
}
