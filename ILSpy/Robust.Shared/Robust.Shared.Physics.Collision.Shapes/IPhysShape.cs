using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics.Collision.Shapes;

[NotContentImplementable]
public interface IPhysShape : IEquatable<IPhysShape>
{
	int ChildCount { get; }

	float Radius { get; set; }

	ShapeType ShapeType { get; }

	Box2 ComputeAABB(Transform transform, int childIndex);
}
