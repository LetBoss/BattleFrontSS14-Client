using System;
using Content.Shared.FixedPoint;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Projectile.Bone;

public sealed class XenoBoneChipsSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoProjectileSystem _xenoProjectile;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoBoneChipsComponent, XenoBoneChipsActionEvent>((EntityEventRefHandler<XenoBoneChipsComponent, XenoBoneChipsActionEvent>)OnXenoBoneSpursAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowedByBoneChipsComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<SlowedByBoneChipsComponent, RefreshMovementSpeedModifiersEvent>)OnSlowedBySpitRefreshMovement, (Type[])null, (Type[])null);
	}

	private void OnXenoBoneSpursAction(Entity<XenoBoneChipsComponent> xeno, ref XenoBoneChipsActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			XenoBoneChipsActionEvent obj = args;
			XenoProjectileSystem xenoProjectile = _xenoProjectile;
			EntityUid xeno2 = Entity<XenoBoneChipsComponent>.op_Implicit(xeno);
			EntityCoordinates target = args.Target;
			FixedPoint2 zero = FixedPoint2.Zero;
			EntProtoId projectileId = xeno.Comp.ProjectileId;
			Angle zero2 = Angle.Zero;
			float speed = xeno.Comp.Speed;
			EntityUid? entity = args.Entity;
			((HandledEntityEventArgs)obj).Handled = xenoProjectile.TryShoot(xeno2, target, zero, projectileId, null, 1, zero2, speed, null, entity);
		}
	}

	private void OnSlowedBySpitRefreshMovement(Entity<SlowedByBoneChipsComponent> slowed, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (slowed.Comp.ExpiresAt > _timing.CurTime)
		{
			args.ModifySpeed(slowed.Comp.Multiplier, slowed.Comp.Multiplier);
		}
	}
}
