namespace Robust.Shared.Physics.Systems;

internal ref struct WorldRayCastContext
{
	public RayCastSystem System;

	public SharedPhysicsSystem Physics;

	public CastResult fcn;

	public QueryFilter Filter;

	public float Fraction;

	public RayResult Result;
}
