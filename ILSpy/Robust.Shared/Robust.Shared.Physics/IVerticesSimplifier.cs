using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Physics;

[NotContentImplementable]
public interface IVerticesSimplifier
{
	List<Vector2> Simplify(List<Vector2> vertices, float tolerance);
}
