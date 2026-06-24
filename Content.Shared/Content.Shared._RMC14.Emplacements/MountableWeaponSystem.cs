using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Overheat;
using Content.Shared.Foldable;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Emplacements;

public sealed class MountableWeaponSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedWeaponMountSystem _weaponMount;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MountableWeaponComponent, AttemptShootEvent>((EntityEventRefHandler<MountableWeaponComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MountableWeaponComponent, TakeAmmoEvent>((EntityEventRefHandler<MountableWeaponComponent, TakeAmmoEvent>)OnTakeAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MountableWeaponComponent, OverheatedEvent>((EntityEventRefHandler<MountableWeaponComponent, OverheatedEvent>)OnOverheated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MountableWeaponComponent, HeatGainedEvent>((EntityEventRefHandler<MountableWeaponComponent, HeatGainedEvent>)OnHeatGained, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MountableWeaponComponent, RMCBeforeMuzzleFlashEvent>((EntityEventRefHandler<MountableWeaponComponent, RMCBeforeMuzzleFlashEvent>)OnBeforeMuzzleFlash, (Type[])null, (Type[])null);
	}

	private void OnAttemptShoot(Entity<MountableWeaponComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (!args.ToCoordinates.HasValue)
		{
			return;
		}
		if (ent.Comp.RequiresMount && !ent.Comp.MountedTo.HasValue)
		{
			args.Cancelled = true;
		}
		else if (ent.Comp.MountedTo.HasValue)
		{
			EntityUid mountEntity = ((EntitySystem)this).GetEntity(ent.Comp.MountedTo.Value);
			Vector2 mountPosition = _transform.GetWorldPosition(Entity<MountableWeaponComponent>.op_Implicit(ent));
			Angle targetDirection = Angle.FromWorldVec(_transform.ToWorldPosition(args.ToCoordinates.Value, true) - mountPosition);
			Angle weaponFront = _transform.GetWorldRotation(mountEntity);
			Angle val = Angle.ShortestDistance(ref weaponFront, ref targetDirection);
			if (Math.Abs(((Angle)(ref val)).Degrees) > (double)((float)ent.Comp.ShootArc / 2f))
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnTakeAmmo(Entity<MountableWeaponComponent> ent, ref TakeAmmoEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.MountedTo.HasValue && _weaponMount.TryGetWeaponAmmo(Entity<MountableWeaponComponent>.op_Implicit(ent), out var ammoCount, out var _))
		{
			WeaponMountComponentVisualLayers ammoSpriteKey = WeaponMountComponentVisualLayers.MountedAmmo;
			FoldableComponent foldableComp = default(FoldableComponent);
			if (((EntitySystem)this).TryComp<FoldableComponent>(Entity<MountableWeaponComponent>.op_Implicit(ent), ref foldableComp) && foldableComp.IsFolded)
			{
				ammoSpriteKey = WeaponMountComponentVisualLayers.FoldedAmmo;
			}
			_appearance.SetData(((EntitySystem)this).GetEntity(ent.Comp.MountedTo.Value), (Enum)ammoSpriteKey, (object)(ammoCount - 1 > 0), (AppearanceComponent)null);
		}
	}

	private void OnOverheated(Entity<MountableWeaponComponent> ent, ref OverheatedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.MountedTo.HasValue)
		{
			MountableWeaponRelayedEvent<OverheatedEvent> ev = new MountableWeaponRelayedEvent<OverheatedEvent>(args);
			((EntitySystem)this).RaiseLocalEvent<MountableWeaponRelayedEvent<OverheatedEvent>>(((EntitySystem)this).GetEntity(ent.Comp.MountedTo.Value), ref ev, false);
		}
	}

	private void OnHeatGained(Entity<MountableWeaponComponent> ent, ref HeatGainedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.MountedTo.HasValue)
		{
			MountableWeaponRelayedEvent<HeatGainedEvent> ev = new MountableWeaponRelayedEvent<HeatGainedEvent>(args);
			((EntitySystem)this).RaiseLocalEvent<MountableWeaponRelayedEvent<HeatGainedEvent>>(((EntitySystem)this).GetEntity(ent.Comp.MountedTo.Value), ref ev, false);
		}
	}

	private void OnBeforeMuzzleFlash(Entity<MountableWeaponComponent> ent, ref RMCBeforeMuzzleFlashEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.MountedTo.HasValue)
		{
			args.Weapon = ((EntitySystem)this).GetEntity(ent.Comp.MountedTo.Value);
		}
	}

	public bool TryGetWeaponMount(EntityUid weapon, [NotNullWhen(true)] out EntityUid? mountEntity, MountableWeaponComponent? mountable = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		mountEntity = null;
		if (!((EntitySystem)this).Resolve<MountableWeaponComponent>(weapon, ref mountable, false))
		{
			return false;
		}
		if (!mountable.MountedTo.HasValue)
		{
			return false;
		}
		mountEntity = ((EntitySystem)this).GetEntity(mountable.MountedTo.Value);
		return true;
	}
}
