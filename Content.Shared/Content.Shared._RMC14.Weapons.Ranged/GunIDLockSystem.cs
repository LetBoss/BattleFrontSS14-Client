using System;
using Content.Shared._RMC14.Medical.Defibrillator;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Interaction.Components;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class GunIDLockSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GunIDLockComponent, GotEquippedHandEvent>((EntityEventRefHandler<GunIDLockComponent, GotEquippedHandEvent>)OnHold, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunIDLockComponent, AttemptShootEvent>((EntityEventRefHandler<GunIDLockComponent, AttemptShootEvent>)OnShootAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunIDLockComponent, ExaminedEvent>((EntityEventRefHandler<GunIDLockComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunIDLockComponent, GetItemActionsEvent>((EntityEventRefHandler<GunIDLockComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunIDLockComponent, ToggleActionEvent>((EntityEventRefHandler<GunIDLockComponent, ToggleActionEvent>)OnGunIDLockToggle, (Type[])null, (Type[])null);
	}

	private void OnHold(Entity<GunIDLockComponent> ent, ref GotEquippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		CheckUserRevivability(ent);
		if (ent.Comp.User == EntityUid.Invalid)
		{
			RegisterNewUser(ent, args.User);
		}
	}

	private void OnGetActions(Entity<GunIDLockComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (args.InHands)
		{
			args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionID));
		}
	}

	private void OnGunIDLockToggle(Entity<GunIDLockComponent> ent, ref ToggleActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = Entity<ActionComponent>.op_Implicit(args.Action);
		EntityUid? action = ent.Comp.Action;
		if (!action.HasValue || val != action.GetValueOrDefault())
		{
			return;
		}
		if (args.Performer != ent.Comp.User)
		{
			string popup = base.Loc.GetString("rmc-id-lock-unauthorized");
			_popup.PopupClient(popup, args.Performer, args.Performer, PopupType.SmallCaution);
			return;
		}
		if (ent.Comp.Locked)
		{
			ent.Comp.Locked = false;
			string popup2 = base.Loc.GetString("rmc-id-lock-toggle-lock", (ValueTuple<string, object>)("action", base.Loc.GetString("rmc-id-lock-toggle-off")), (ValueTuple<string, object>)("gun", ent.Owner));
			_popup.PopupClient(popup2, args.Performer, args.Performer);
			_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<GunIDLockComponent>.op_Implicit(ent), (EntityUid?)args.Performer, (AudioParams?)null);
			_actions.SetIcon(Entity<ActionComponent>.op_Implicit(ent.Comp.Action.Value), (SpriteSpecifier?)(object)ent.Comp.UnlockedIcon);
		}
		else
		{
			ent.Comp.Locked = true;
			string popup3 = base.Loc.GetString("rmc-id-lock-toggle-lock", (ValueTuple<string, object>)("action", base.Loc.GetString("rmc-id-lock-toggle-on")), (ValueTuple<string, object>)("gun", ent.Owner));
			_popup.PopupClient(popup3, args.Performer, args.Performer);
			_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<GunIDLockComponent>.op_Implicit(ent), (EntityUid?)args.Performer, (AudioParams?)null);
			_actions.SetIcon(Entity<ActionComponent>.op_Implicit(ent.Comp.Action.Value), (SpriteSpecifier?)(object)ent.Comp.LockedIcon);
		}
		((EntitySystem)this).Dirty<GunIDLockComponent>(ent, (MetaDataComponent)null);
	}

	private void OnShootAttempt(Entity<GunIDLockComponent> ent, ref AttemptShootEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			CheckUserRevivability(ent);
			if (ent.Comp.User == EntityUid.Invalid)
			{
				RegisterNewUserCombat(ent, args.User);
			}
			if (!((EntitySystem)this).HasComp<BypassInteractionChecksComponent>(args.User) && ent.Comp.Locked && !(ent.Comp.User == args.User))
			{
				args.Cancelled = true;
				string popup = base.Loc.GetString("rmc-shoot-id-lock-unauthorized");
				_popup.PopupClient(popup, args.User, args.User, PopupType.SmallCaution);
			}
		}
	}

	private void OnExamine(Entity<GunIDLockComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		CheckUserRevivability(ent);
		using (args.PushGroup("GunIDLockComponent"))
		{
			if (ent.Comp.User == EntityUid.Invalid)
			{
				args.PushMarkup(base.Loc.GetString("rmc-examine-text-id-lock-no-user"));
			}
			else if (ent.Comp.User == args.Examiner)
			{
				if (ent.Comp.Locked)
				{
					args.PushMarkup(base.Loc.GetString("rmc-examine-text-id-lock", (ValueTuple<string, object>)("color", base.Loc.GetString("rmc-id-lock-color-authorized")), (ValueTuple<string, object>)("name", ent.Comp.User)));
				}
				else
				{
					args.PushMarkup(base.Loc.GetString("rmc-examine-text-id-lock-unlocked", (ValueTuple<string, object>)("color", base.Loc.GetString("rmc-id-lock-color-authorized")), (ValueTuple<string, object>)("name", ent.Comp.User)));
				}
			}
			else if (ent.Comp.Locked)
			{
				args.PushMarkup(base.Loc.GetString("rmc-examine-text-id-lock", (ValueTuple<string, object>)("color", base.Loc.GetString("rmc-id-lock-color-unauthorized")), (ValueTuple<string, object>)("name", ent.Comp.User)));
			}
			else
			{
				args.PushMarkup(base.Loc.GetString("rmc-examine-text-id-lock-unlocked", (ValueTuple<string, object>)("color", base.Loc.GetString("rmc-id-lock-color-unauthorized")), (ValueTuple<string, object>)("name", ent.Comp.User)));
			}
		}
	}

	private void RegisterNewUser(Entity<GunIDLockComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.User = user;
		((EntitySystem)this).Dirty<GunIDLockComponent>(ent, (MetaDataComponent)null);
		string popup = base.Loc.GetString("rmc-id-lock-authorization", (ValueTuple<string, object>)("gun", ent.Owner));
		_popup.PopupClient(popup, user, PopupType.Medium);
	}

	private void RegisterNewUserCombat(Entity<GunIDLockComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.User = user;
		((EntitySystem)this).Dirty<GunIDLockComponent>(ent, (MetaDataComponent)null);
		string popup = base.Loc.GetString("rmc-id-lock-authorization-combat", (ValueTuple<string, object>)("gun", ent.Owner));
		_popup.PopupClient(popup, user, user);
	}

	private void ClearUser(Entity<GunIDLockComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.User = EntityUid.Invalid;
		((EntitySystem)this).Dirty<GunIDLockComponent>(ent, (MetaDataComponent)null);
	}

	private void CheckUserRevivability(Entity<GunIDLockComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!(ent.Comp.User == EntityUid.Invalid))
		{
			if (((EntitySystem)this).TerminatingOrDeleted(ent.Comp.User, (MetaDataComponent)null))
			{
				ClearUser(ent);
			}
			if (((EntitySystem)this).HasComp<RMCDefibrillatorBlockedComponent>(ent.Comp.User))
			{
				ClearUser(ent);
			}
			PerishableComponent perish = default(PerishableComponent);
			if (((EntitySystem)this).TryComp<PerishableComponent>(ent.Comp.User, ref perish) && perish != null && perish.Stage >= 4)
			{
				ClearUser(ent);
			}
		}
	}
}
