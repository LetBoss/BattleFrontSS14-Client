using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Dynamics.Contacts;

namespace Robust.Shared.GameObjects;

public abstract class SharedDebugPhysicsSystem : EntitySystem
{
	public virtual void HandlePreSolve(Contact contact, in Manifold oldManifold)
	{
	}
}
