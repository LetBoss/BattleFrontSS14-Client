using Robust.Shared.Physics.Components;

namespace Robust.Shared.Physics.Dynamics.Joints;

public sealed class JointEdge
{
	public Joint Joint { get; set; }

	public JointEdge? Next { get; set; }

	public PhysicsComponent Other { get; set; }

	public JointEdge? Prev { get; set; }
}
