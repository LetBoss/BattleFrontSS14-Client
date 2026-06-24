using System.Numerics;

namespace Robust.Shared.Physics.Collision;

internal record struct ClipVertex
{
	public ContactID ID;

	public Vector2 V;
}
