using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Prying;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Prying.Systems;

public sealed class PryingSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<DoorComponent, GetVerbsEvent<AlternativeVerb>>)OnDoorAltVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, DoorPryDoAfterEvent>((ComponentEventHandler<DoorComponent, DoorPryDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, InteractUsingEvent>((ComponentEventHandler<DoorComponent, InteractUsingEvent>)TryPryDoor, (Type[])null, (Type[])null);
	}

	private void TryPryDoor(EntityUid uid, DoorComponent comp, InteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryPry(uid, args.User, out var _, args.Used);
		}
	}

	private void OnDoorAltVerb(EntityUid uid, DoorComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		PryingComponent pryingComponent = default(PryingComponent);
		if (args.CanInteract && args.CanAccess && ((EntitySystem)this).TryComp<PryingComponent>(args.User, ref pryingComponent) && CanPry(uid, args.User, out string _))
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("door-pry"),
				Impact = LogImpact.Low,
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					TryPry(uid, args.User, out var _, args.User);
				}
			});
		}
	}

	public bool TryPry(EntityUid target, EntityUid user, out DoAfterId? id, EntityUid tool)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		id = null;
		PryingComponent comp = null;
		if (!((EntitySystem)this).Resolve<PryingComponent>(tool, ref comp, false))
		{
			return false;
		}
		if (!comp.Enabled)
		{
			return false;
		}
		if (!CanPry(target, user, out string message, comp))
		{
			if (!string.IsNullOrWhiteSpace(message))
			{
				_popup.PopupClient(base.Loc.GetString(message), target, user);
			}
			return true;
		}
		StartPry(target, user, tool, comp.SpeedModifier, out id);
		return true;
	}

	public bool TryPry(EntityUid target, EntityUid user, out DoAfterId? id)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		id = null;
		PryUnpoweredComponent unpoweredComp = default(PryUnpoweredComponent);
		if (!((EntitySystem)this).TryComp<PryUnpoweredComponent>(target, ref unpoweredComp) || !CanPry(target, user, out string _, null, unpoweredComp))
		{
			return true;
		}
		if (((EntitySystem)this).HasComp<RMCUserPryingRequiresToolComponent>(user))
		{
			_popup.PopupClient("You can't pry that with your bare hands!", target, user, PopupType.SmallCaution);
			return true;
		}
		if (!((EntitySystem)this).HasComp<PryingComponent>(user))
		{
			return true;
		}
		float modifier = ((EntitySystem)this).CompOrNull<PryingComponent>(user)?.SpeedModifier ?? unpoweredComp.PryModifier;
		return StartPry(target, user, null, modifier, out id);
	}

	private bool CanPry(EntityUid target, EntityUid user, out string? message, PryingComponent? comp = null, PryUnpoweredComponent? unpoweredComp = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		BeforePryEvent canev;
		if (comp != null || ((EntitySystem)this).Resolve<PryingComponent>(user, ref comp, false))
		{
			canev = new BeforePryEvent(user, comp.PryPowered, comp.Force, StrongPry: true);
		}
		else
		{
			if (!((EntitySystem)this).Resolve<PryUnpoweredComponent>(target, ref unpoweredComp, true))
			{
				message = null;
				return false;
			}
			canev = new BeforePryEvent(user, PryPowered: false, Force: false, StrongPry: false);
		}
		((EntitySystem)this).RaiseLocalEvent<BeforePryEvent>(target, ref canev, false);
		message = canev.Message;
		return !canev.Cancelled;
	}

	private bool StartPry(EntityUid target, EntityUid user, EntityUid? tool, float toolModifier, [NotNullWhen(true)] out DoAfterId? id)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		GetPryTimeModifierEvent modEv = new GetPryTimeModifierEvent(user);
		((EntitySystem)this).RaiseLocalEvent<GetPryTimeModifierEvent>(target, ref modEv, false);
		DoAfterArgs obj = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, TimeSpan.FromSeconds(modEv.BaseTime * modEv.PryTimeModifier / toolModifier), new DoorPryDoAfterEvent(), target, target, tool)
		{
			BreakOnDamage = false,
			BreakOnMove = true
		};
		EntityUid? val = tool;
		EntityUid val2 = user;
		obj.NeedHand = !val.HasValue || val.GetValueOrDefault() != val2;
		obj.ForceVisible = !tool.HasValue;
		DoAfterArgs doAfterArgs = obj;
		val = tool;
		val2 = user;
		if ((!val.HasValue || val.GetValueOrDefault() != val2) && tool.HasValue)
		{
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(18, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler.AppendLiteral(" is using ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(tool.Value)), "ToPrettyString(tool.Value)");
			handler.AppendLiteral(" to pry ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "ToPrettyString(target)");
			adminLog.Add(LogType.Action, LogImpact.Low, ref handler);
		}
		else
		{
			ISharedAdminLogManager adminLog2 = _adminLog;
			LogStringHandler handler2 = new LogStringHandler(11, 2);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler2.AppendLiteral(" is prying ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "ToPrettyString(target)");
			adminLog2.Add(LogType.Action, LogImpact.Low, ref handler2);
		}
		RMCDoorPryEvent doorpry = new RMCDoorPryEvent(user);
		((EntitySystem)this).RaiseLocalEvent<RMCDoorPryEvent>(target, ref doorpry, false);
		return _doAfterSystem.TryStartDoAfter(doAfterArgs, out id);
	}

	private void OnDoAfter(EntityUid uid, DoorComponent door, DoorPryDoAfterEvent args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || !args.Target.HasValue)
		{
			return;
		}
		PryingComponent comp = default(PryingComponent);
		((EntitySystem)this).TryComp<PryingComponent>(args.Used, ref comp);
		if (!CanPry(uid, args.User, out string message, comp))
		{
			if (!string.IsNullOrWhiteSpace(message))
			{
				_popup.PopupClient(base.Loc.GetString(message), uid, args.User);
			}
			return;
		}
		if (args.Used.HasValue && comp != null)
		{
			_audioSystem.PlayPredicted(comp.UseSound, args.Used.Value, (EntityUid?)args.User, (AudioParams?)null);
		}
		PriedEvent ev = new PriedEvent(args.User);
		((EntitySystem)this).RaiseLocalEvent<PriedEvent>(uid, ref ev, false);
	}
}
