using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Weapons.Ranged.IFF;

public sealed class IFFToggleSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private RMCSelectiveFireSystem _fireSystem;

	[Dependency]
	private GunIFFSystem _iffSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<IFFToggleComponent, MapInitEvent>((EntityEventRefHandler<IFFToggleComponent, MapInitEvent>)OnStartup, (Type[])null, new Type[2]
		{
			typeof(RMCSelectiveFireSystem),
			typeof(SharedGunSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<IFFToggleComponent, GetItemActionsEvent>((EntityEventRefHandler<IFFToggleComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, new Type[1] { typeof(GunIFFSystem) });
		((EntitySystem)this).SubscribeLocalEvent<IFFToggleComponent, ToggleActionEvent>((EntityEventRefHandler<IFFToggleComponent, ToggleActionEvent>)OnActionToggle, (Type[])null, new Type[1] { typeof(GunIDLockSystem) });
	}

	public void OnStartup(Entity<IFFToggleComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		RMCSelectiveFireComponent comp = default(RMCSelectiveFireComponent);
		if (ent.Comp.ChangeStats && ((EntitySystem)this).TryComp<RMCSelectiveFireComponent>(Entity<IFFToggleComponent>.op_Implicit(ent), ref comp))
		{
			ent.Comp.BaseFireModes = comp.BaseFireModes;
			ent.Comp.BaseModifiers = new Dictionary<SelectiveFire, SelectiveFireModifierSet>(comp.Modifiers);
			SetStats(ent);
		}
	}

	public void SetStats(Entity<IFFToggleComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		_fireSystem.SetModifiers(Entity<RMCSelectiveFireComponent>.op_Implicit(ent.Owner), ent.Comp.IFFModifiers);
		_fireSystem.SetFireModes(Entity<GunComponent>.op_Implicit(ent.Owner), ent.Comp.IFFFireModes);
		((EntitySystem)this).Dirty<IFFToggleComponent>(ent, (MetaDataComponent)null);
	}

	public void ResetStats(Entity<IFFToggleComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		_fireSystem.SetModifiers(Entity<RMCSelectiveFireComponent>.op_Implicit(ent.Owner), ent.Comp.BaseModifiers);
		_fireSystem.SetFireModes(Entity<GunComponent>.op_Implicit(ent.Owner), ent.Comp.BaseFireModes);
		((EntitySystem)this).Dirty<IFFToggleComponent>(ent, (MetaDataComponent)null);
	}

	public void CheckStats(Entity<IFFToggleComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		RMCSelectiveFireComponent comp = default(RMCSelectiveFireComponent);
		if (((EntitySystem)this).TryComp<RMCSelectiveFireComponent>(ent.Owner, ref comp))
		{
			if (ent.Comp.Enabled && comp.BaseFireModes != ent.Comp.IFFFireModes)
			{
				SetStats(ent);
			}
			if (!ent.Comp.Enabled && comp.BaseFireModes != ent.Comp.BaseFireModes)
			{
				ResetStats(ent);
			}
		}
	}

	public void OnGetActions(Entity<IFFToggleComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (args.InHands)
		{
			args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionID));
		}
	}

	public void OnActionToggle(Entity<IFFToggleComponent> ent, ref ToggleActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = Entity<ActionComponent>.op_Implicit(args.Action);
		EntityUid? action = ent.Comp.Action;
		if (!action.HasValue || val != action.GetValueOrDefault())
		{
			return;
		}
		GunIDLockComponent comp = default(GunIDLockComponent);
		if (ent.Comp.RequireIDLock && ((EntitySystem)this).TryComp<GunIDLockComponent>(ent.Owner, ref comp) && comp.Locked && comp.User != args.Performer)
		{
			string popup = base.Loc.GetString("rmc-id-lock-unauthorized");
			_popup.PopupClient(popup, args.Performer, args.Performer, PopupType.SmallCaution);
			return;
		}
		if (ent.Comp.Enabled)
		{
			ent.Comp.Enabled = false;
			_iffSystem.SetIFFState(ent.Owner, ent.Comp.Enabled);
			string popup2 = base.Loc.GetString("rmc-iff-toggle", (ValueTuple<string, object>)("action", base.Loc.GetString("rmc-iff-toggle-off")), (ValueTuple<string, object>)("gun", ent.Owner));
			_popup.PopupClient(popup2, args.Performer, args.Performer);
			_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<IFFToggleComponent>.op_Implicit(ent), (EntityUid?)args.Performer, (AudioParams?)null);
			if (ent.Comp.ChangeStats)
			{
				ResetStats(ent);
			}
			_actions.SetIcon(Entity<ActionComponent>.op_Implicit(ent.Comp.Action.Value), (SpriteSpecifier?)(object)ent.Comp.DisabledIcon);
		}
		else
		{
			ent.Comp.Enabled = true;
			_iffSystem.SetIFFState(ent.Owner, ent.Comp.Enabled);
			string popup3 = base.Loc.GetString("rmc-iff-toggle", (ValueTuple<string, object>)("action", base.Loc.GetString("rmc-iff-toggle-on")), (ValueTuple<string, object>)("gun", ent.Owner));
			_popup.PopupClient(popup3, args.Performer, args.Performer);
			_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<IFFToggleComponent>.op_Implicit(ent), (EntityUid?)args.Performer, (AudioParams?)null);
			if (ent.Comp.ChangeStats)
			{
				SetStats(ent);
			}
			_actions.SetIcon(Entity<ActionComponent>.op_Implicit(ent.Comp.Action.Value), (SpriteSpecifier?)(object)ent.Comp.EnabledIcon);
		}
		((EntitySystem)this).Dirty<IFFToggleComponent>(ent, (MetaDataComponent)null);
	}
}
