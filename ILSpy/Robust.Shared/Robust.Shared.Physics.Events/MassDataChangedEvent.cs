using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;

namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public readonly record struct MassDataChangedEvent(Entity<PhysicsComponent, FixturesComponent> Entity, float OldMass, float OldInertia, Vector2 OldCenter)
{
	public float NewMass => Entity.Comp1._mass;

	public float NewInertia => Entity.Comp1._inertia;

	public Vector2 NewCenter => Entity.Comp1._localCenter;

	public bool MassChanged => NewMass != OldMass;

	public bool InertiaChanged => NewInertia != OldInertia;

	public bool CenterChanged => NewCenter != OldCenter;
}
