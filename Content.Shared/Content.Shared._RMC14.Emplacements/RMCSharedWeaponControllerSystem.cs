using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Emplacements;

public abstract class RMCSharedWeaponControllerSystem : EntitySystem
{
	[Dependency]
	private SharedBuckleSystem _buckle;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedContainerSystem _container;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<WeaponControllerComponent, BeforeAttemptShootEvent>((EntityEventRefHandler<WeaponControllerComponent, BeforeAttemptShootEvent>)OnAdjustShotOrigin, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponControllerComponent, DismountActionEvent>((EntityEventRefHandler<WeaponControllerComponent, DismountActionEvent>)OnDismountAction, (Type[])null, (Type[])null);
	}

	private void OnAdjustShotOrigin(Entity<WeaponControllerComponent> ent, ref BeforeAttemptShootEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ControlledWeapon.HasValue)
		{
			BaseContainer container = default(BaseContainer);
			_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(((EntitySystem)this).GetEntity(ent.Comp.ControlledWeapon.Value)), ref container);
			if (container != null)
			{
				EntityUid mount = container.Owner;
				Angle rotation = _transform.GetWorldRotation(mount);
				Vector2 offset = args.Offset;
				Vector2 rotatedOffset = ((Angle)(ref rotation)).RotateVec(ref offset);
				EntityCoordinates coordinates = ((EntitySystem)this).Transform(mount).Coordinates;
				args.Origin = ((EntityCoordinates)(ref coordinates)).Offset(rotatedOffset);
				args.Handled = true;
			}
		}
	}

	private void OnDismountAction(Entity<WeaponControllerComponent> ent, ref DismountActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_buckle.Unbuckle(Entity<BuckleComponent>.op_Implicit(ent.Owner), Entity<WeaponControllerComponent>.op_Implicit(ent));
	}

	public bool TryGetControlledWeapon(EntityUid user, [NotNullWhen(true)] out EntityUid? controlledWeapon, [NotNullWhen(true)] out GunComponent? gunComp)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		gunComp = null;
		controlledWeapon = null;
		WeaponControllerComponent weaponController = default(WeaponControllerComponent);
		if (!((EntitySystem)this).TryComp<WeaponControllerComponent>(user, ref weaponController) || !weaponController.ControlledWeapon.HasValue)
		{
			return false;
		}
		controlledWeapon = ((EntitySystem)this).GetEntity(weaponController.ControlledWeapon.Value);
		GunComponent gunComponent = default(GunComponent);
		if (!((EntitySystem)this).TryComp<GunComponent>(controlledWeapon, ref gunComponent))
		{
			return false;
		}
		gunComp = gunComponent;
		return true;
	}
}
