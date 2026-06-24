using System;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared._RMC14.Weapons.Ranged;

public abstract class SharedPumpActionSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PumpActionComponent, ExaminedEvent>((EntityEventRefHandler<PumpActionComponent, ExaminedEvent>)OnExamined, new Type[1] { typeof(SharedGunSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PumpActionComponent, AttemptShootEvent>((EntityEventRefHandler<PumpActionComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PumpActionComponent, GunShotEvent>((EntityEventRefHandler<PumpActionComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PumpActionComponent, UniqueActionEvent>((EntityEventRefHandler<PumpActionComponent, UniqueActionEvent>)OnUniqueAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PumpActionComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<PumpActionComponent, EntRemovedFromContainerMessage>)OnEntRemovedFromContainer, (Type[])null, (Type[])null);
	}

	protected virtual void OnExamined(Entity<PumpActionComponent> ent, ref ExaminedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Examine)), 1);
	}

	protected virtual void OnAttemptShoot(Entity<PumpActionComponent> ent, ref AttemptShootEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (!args.Cancelled && (!((EntitySystem)this).TryComp<GunComponent>(ent.Owner, ref gun) || !gun.BurstActivated) && !ent.Comp.Pumped)
		{
			args.Cancelled = true;
		}
	}

	private void OnGunShot(Entity<PumpActionComponent> ent, ref GunShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Once)
		{
			ent.Comp.Pumped = false;
			((EntitySystem)this).Dirty<PumpActionComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnUniqueAction(Entity<PumpActionComponent> ent, ref UniqueActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && Pump(ent, args.UserUid))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnEntRemovedFromContainer(Entity<PumpActionComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId) && ent.Comp.Once)
		{
			ent.Comp.Pumped = false;
		}
	}

	public bool Pump(Entity<PumpActionComponent> ent, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(ent.Owner, ref gun) && gun.BurstActivated)
		{
			return true;
		}
		GetAmmoCountEvent ammo = default(GetAmmoCountEvent);
		((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(ent.Owner, ref ammo, false);
		if (ammo.Count <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("cm-gun-no-ammo-message"), user, user);
			return true;
		}
		if (!((Component)ent.Comp).Running || ent.Comp.Pumped)
		{
			return false;
		}
		ent.Comp.Pumped = true;
		((EntitySystem)this).Dirty<PumpActionComponent>(ent, (MetaDataComponent)null);
		_audio.PlayPredicted(ent.Comp.Sound, Entity<PumpActionComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
		return true;
	}
}
