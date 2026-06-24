using System;
using Content.Shared._RMC14.Weapons.Ranged.Battery;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Weapons.Ranged.Recoil;

public sealed class GunToggleableRecoilSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private RMCGunBatterySystem _gunBattery;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableRecoilComponent, GetItemActionsEvent>((EntityEventRefHandler<GunToggleableRecoilComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableRecoilComponent, GunToggleRecoilActionEvent>((EntityEventRefHandler<GunToggleableRecoilComponent, GunToggleRecoilActionEvent>)OnToggleRecoil, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableRecoilComponent, GunGetBatteryDrainEvent>((EntityEventRefHandler<GunToggleableRecoilComponent, GunGetBatteryDrainEvent>)OnGetBatteryDrain, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableRecoilComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunToggleableRecoilComponent, GunRefreshModifiersEvent>)OnRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunToggleableRecoilComponent, GunUnpoweredEvent>((EntityEventRefHandler<GunToggleableRecoilComponent, GunUnpoweredEvent>)OnGunUnpowered, (Type[])null, (Type[])null);
	}

	private void OnGetItemActions(Entity<GunToggleableRecoilComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
		((EntitySystem)this).Dirty<GunToggleableRecoilComponent>(ent, (MetaDataComponent)null);
	}

	private void OnToggleRecoil(Entity<GunToggleableRecoilComponent> ent, ref GunToggleRecoilActionEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		ent.Comp.Active = !ent.Comp.Active;
		ActiveChanged(ent, args.Performer);
	}

	private void OnGetBatteryDrain(Entity<GunToggleableRecoilComponent> ent, ref GunGetBatteryDrainEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Active)
		{
			args.Drain += ent.Comp.BatteryDrain;
		}
	}

	private void OnRefreshModifiers(Entity<GunToggleableRecoilComponent> ent, ref GunRefreshModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Active)
		{
			args.MinAngle = Angle.Zero;
			args.MaxAngle = Angle.Zero;
			args.CameraRecoilScalar = 0f;
		}
	}

	private void OnGunUnpowered(Entity<GunToggleableRecoilComponent> ent, ref GunUnpoweredEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Active)
		{
			ent.Comp.Active = false;
			ActiveChanged(ent, null);
		}
	}

	private void ActiveChanged(Entity<GunToggleableRecoilComponent> ent, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Dirty<GunToggleableRecoilComponent>(ent, (MetaDataComponent)null);
		SharedActionsSystem actions = _actions;
		EntityUid? action = ent.Comp.Action;
		actions.SetToggled(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), ent.Comp.Active);
		_gunBattery.RefreshBatteryDrain(Entity<GunDrainBatteryOnShootComponent>.op_Implicit(ent.Owner));
		_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(ent.Owner));
		_audio.PlayPredicted(ent.Comp.ToggleSound, ent.Owner, user, (AudioParams?)null);
		if (user.HasValue)
		{
			string popup = (ent.Comp.Active ? base.Loc.GetString("rmc-toggleable-recoil-compensation-on", (ValueTuple<string, object>)("gun", ent.Owner)) : base.Loc.GetString("rmc-toggleable-recoil-compensation-off", (ValueTuple<string, object>)("gun", ent.Owner)));
			_popup.PopupClient(popup, user.Value, user.Value, PopupType.Large);
		}
	}
}
