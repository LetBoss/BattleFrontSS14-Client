using System;
using System.Numerics;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class GunFireArcSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GunFireArcComponent, AttemptShootEvent>((EntityEventRefHandler<GunFireArcComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
	}

	private void OnAttemptShoot(Entity<GunFireArcComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || !_transform.IsValid(args.FromCoordinates) || !args.ToCoordinates.HasValue || !_transform.IsValid(args.ToCoordinates.Value))
		{
			return;
		}
		MapCoordinates fromMap = _transform.ToMapCoordinates(args.FromCoordinates, true);
		Vector2 direction = _transform.ToMapCoordinates(args.ToCoordinates.Value, true).Position - fromMap.Position;
		if (!(direction.LengthSquared() <= 0.0001f))
		{
			Angle aimAngle = DirectionExtensions.ToWorldAngle(direction);
			Angle facing = _transform.GetWorldRotation(ent.Owner);
			BaseContainer container = default(BaseContainer);
			if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(ent.Owner, null)), ref container))
			{
				facing = _transform.GetWorldRotation(container.Owner);
			}
			Angle center = facing + ent.Comp.AngleOffset;
			Angle halfArc = Angle.FromDegrees(((Angle)(ref ent.Comp.Arc)).Degrees / 2.0);
			Angle diff = Angle.ShortestDistance(ref aimAngle, ref center);
			if (!(Angle.op_Implicit(diff) <= Angle.op_Implicit(halfArc)) || !(Angle.op_Implicit(diff) >= Angle.op_Implicit(-halfArc)))
			{
				args.Cancelled = true;
				args.ResetCooldown = true;
				args.Message = base.Loc.GetString("rmc-gun-arc-blocked");
			}
		}
	}
}
