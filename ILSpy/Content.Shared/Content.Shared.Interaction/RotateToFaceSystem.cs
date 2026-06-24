using System;
using System.Numerics;
using Content.Shared._RMC14.Interaction;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle.Components;
using Content.Shared.Rotatable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared.Interaction;

public sealed class RotateToFaceSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlockerSystem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCInteractionSystem _rmcInteraction;

	public bool TryRotateTo(EntityUid uid, Angle goalRotation, float frameTime, Angle tolerance, double rotationSpeed = 3.4028234663852886E+38, TransformComponent? xform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(uid, ref xform, true))
		{
			return true;
		}
		_rmcInteraction.TryCapWorldRotation(Entity<MaxRotationComponent, TransformComponent>.op_Implicit((ValueTuple<EntityUid, MaxRotationComponent, TransformComponent>)(uid, null, xform)), ref goalRotation);
		if (rotationSpeed < 3.4028234663852886E+38)
		{
			Angle worldRot = _transform.GetWorldRotation(xform);
			double rotationDiff = Angle.ShortestDistance(ref worldRot, ref goalRotation).Theta;
			double maxRotate = rotationSpeed * (double)frameTime;
			if (Math.Abs(rotationDiff) > maxRotate)
			{
				Angle goalTheta = worldRot + Angle.op_Implicit((double)Math.Sign(rotationDiff) * maxRotate);
				TryFaceAngle(uid, goalTheta, xform);
				rotationDiff = Angle.op_Implicit(goalRotation - goalTheta);
				if (Math.Abs(rotationDiff) > Angle.op_Implicit(tolerance))
				{
					return false;
				}
				return true;
			}
			TryFaceAngle(uid, goalRotation, xform);
		}
		else
		{
			TryFaceAngle(uid, goalRotation, xform);
		}
		return true;
	}

	public bool TryFaceCoordinates(EntityUid user, Vector2 coordinates, TransformComponent? xform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(user, ref xform, true))
		{
			return false;
		}
		Vector2 diff = coordinates - _transform.GetMapCoordinates(user, xform).Position;
		if (diff.LengthSquared() <= 0.01f)
		{
			return true;
		}
		Angle diffAngle = Angle.FromWorldVec(diff);
		return TryFaceAngle(user, diffAngle);
	}

	public bool TryFaceAngle(EntityUid user, Angle diffAngle, TransformComponent? xform = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!_actionBlockerSystem.CanChangeDirection(user))
		{
			return false;
		}
		BuckleComponent buckle = default(BuckleComponent);
		if (((EntitySystem)this).TryComp<BuckleComponent>(user, ref buckle))
		{
			EntityUid? buckledTo = buckle.BuckledTo;
			if (buckledTo.HasValue)
			{
				EntityUid strap = buckledTo.GetValueOrDefault();
				RotatableComponent rotatable = default(RotatableComponent);
				if (!((EntitySystem)this).TryComp<RotatableComponent>(strap, ref rotatable) || !rotatable.RotateWhileAnchored)
				{
					return false;
				}
				_transform.SetWorldRotation(((EntitySystem)this).Transform(strap), diffAngle);
				return true;
			}
		}
		if (!((EntitySystem)this).Resolve(user, ref xform, true))
		{
			return false;
		}
		_transform.SetWorldRotation(xform, diffAngle);
		return true;
	}
}
