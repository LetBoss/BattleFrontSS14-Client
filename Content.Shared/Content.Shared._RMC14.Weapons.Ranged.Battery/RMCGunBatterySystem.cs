using System;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Weapons.Ranged.Battery;

public sealed class RMCGunBatterySystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	private EntityQuery<GunDrainBatteryOnShootComponent> _gunDrainBatteryQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_gunDrainBatteryQuery = ((EntitySystem)this).GetEntityQuery<GunDrainBatteryOnShootComponent>();
		((EntitySystem)this).SubscribeLocalEvent<GunDrainBatteryOnShootComponent, AttemptShootEvent>((EntityEventRefHandler<GunDrainBatteryOnShootComponent, AttemptShootEvent>)OnDrainBatteryAttemptShoot, (Type[])null, (Type[])null);
	}

	private void OnDrainBatteryAttemptShoot(Entity<GunDrainBatteryOnShootComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !ent.Comp.Powered)
		{
			args.Cancelled = true;
			_popup.PopupClient(base.Loc.GetString("rmc-low-power"), args.User, args.User, PopupType.MediumCaution);
		}
	}

	public void SetPowered(Entity<GunDrainBatteryOnShootComponent> gun, bool powered)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		gun.Comp.Powered = powered;
		((EntitySystem)this).Dirty<GunDrainBatteryOnShootComponent>(gun, (MetaDataComponent)null);
		if (!gun.Comp.Powered)
		{
			GunUnpoweredEvent ev = default(GunUnpoweredEvent);
			((EntitySystem)this).RaiseLocalEvent<GunUnpoweredEvent>(Entity<GunDrainBatteryOnShootComponent>.op_Implicit(gun), ref ev, false);
		}
	}

	public void RefreshBatteryDrain(Entity<GunDrainBatteryOnShootComponent?> gun)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && _gunDrainBatteryQuery.Resolve(Entity<GunDrainBatteryOnShootComponent>.op_Implicit(gun), ref gun.Comp, false))
		{
			GunGetBatteryDrainEvent ev = new GunGetBatteryDrainEvent(gun.Comp.BaseDrain);
			((EntitySystem)this).RaiseLocalEvent<GunGetBatteryDrainEvent>(Entity<GunDrainBatteryOnShootComponent>.op_Implicit(gun), ref ev, false);
			gun.Comp.Drain = ev.Drain;
			((EntitySystem)this).Dirty<GunDrainBatteryOnShootComponent>(gun, (MetaDataComponent)null);
		}
	}
}
