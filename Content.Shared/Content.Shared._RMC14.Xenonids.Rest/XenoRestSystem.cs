using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Xenonids.Charge;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Content.Shared._RMC14.Xenonids.Crest;
using Content.Shared._RMC14.Xenonids.Fling;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared._RMC14.Xenonids.Gut;
using Content.Shared._RMC14.Xenonids.Headbutt;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Lunge;
using Content.Shared._RMC14.Xenonids.Punch;
using Content.Shared._RMC14.Xenonids.Screech;
using Content.Shared._RMC14.Xenonids.Stomp;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Rest;

public sealed class XenoRestSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoRestActionEvent>((EntityEventRefHandler<XenoComponent, XenoRestActionEvent>)OnXenoRestAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, UpdateCanMoveEvent>((EntityEventRefHandler<XenoRestingComponent, UpdateCanMoveEvent>)OnXenoRestingCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, AttackAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, AttackAttemptEvent>)OnXenoRestingMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoSecreteStructureAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoSecreteStructureAttemptEvent>)OnXenoSecreteStructureAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoHeadbuttAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoHeadbuttAttemptEvent>)OnXenoRestingHeadbuttAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoFortifyAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoFortifyAttemptEvent>)OnXenoRestingFortifyAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoTailSweepAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoTailSweepAttemptEvent>)OnXenoRestingTailSweepAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoToggleCrestAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoToggleCrestAttemptEvent>)OnXenoRestingToggleCrestAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoLeapAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoLeapAttemptEvent>)OnXenoRestingLeapAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoLungeAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoLungeAttemptEvent>)OnXenoRestingLungeAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoPunchAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoPunchAttemptEvent>)OnXenoRestingPunchAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoFlingAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoFlingAttemptEvent>)OnXenoRestingFlingAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoChargeAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoChargeAttemptEvent>)OnXenoRestingChargettempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoStompAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoStompAttemptEvent>)OnXenoRestingStompAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoGutAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoGutAttemptEvent>)OnXenoRestingGutAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, XenoScreechAttemptEvent>((EntityEventRefHandler<XenoRestingComponent, XenoScreechAttemptEvent>)OnXenoRestingScreechAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, EvasionRefreshModifiersEvent>((EntityEventRefHandler<XenoRestingComponent, EvasionRefreshModifiersEvent>)OnXenoRestingEvasionRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, AttemptMobCollideEvent>((EntityEventRefHandler<XenoRestingComponent, AttemptMobCollideEvent>)OnXenoRestingMobCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRestingComponent, AttemptMobTargetCollideEvent>((EntityEventRefHandler<XenoRestingComponent, AttemptMobTargetCollideEvent>)OnXenoRestingMobTargetCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionBlockIfRestingComponent, RMCActionUseAttemptEvent>((EntityEventRefHandler<ActionBlockIfRestingComponent, RMCActionUseAttemptEvent>)OnXenoRestingActionUseAttempt, (Type[])null, (Type[])null);
	}

	private void OnXenoRestingActionUseAttempt(Entity<ActionBlockIfRestingComponent> ent, ref RMCActionUseAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			EntityUid user = args.User;
			if (((EntitySystem)this).HasComp<XenoRestingComponent>(user))
			{
				args.Cancelled = true;
				_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Popup)), user, user, PopupType.SmallCaution);
			}
		}
	}

	private void OnXenoRestingCanMove(Entity<XenoRestingComponent> xeno, ref UpdateCanMoveEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnXenoRestAction(Entity<XenoComponent> xeno, ref XenoRestActionEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState)
		{
			return;
		}
		XenoRestAttemptEvent attempt = default(XenoRestAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoRestAttemptEvent>(Entity<XenoComponent>.op_Implicit(xeno), ref attempt, false);
		if (!attempt.Cancelled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (((EntitySystem)this).HasComp<XenoRestingComponent>(Entity<XenoComponent>.op_Implicit(xeno)))
			{
				((EntitySystem)this).RemComp<XenoRestingComponent>(Entity<XenoComponent>.op_Implicit(xeno));
				_appearance.SetData(Entity<XenoComponent>.op_Implicit(xeno), (Enum)XenoVisualLayers.Base, (object)XenoRestState.NotResting, (AppearanceComponent)null);
				_actions.SetToggled(args.Action.AsNullable(), toggled: false);
			}
			else
			{
				((EntitySystem)this).AddComp<XenoRestingComponent>(Entity<XenoComponent>.op_Implicit(xeno));
				_appearance.SetData(Entity<XenoComponent>.op_Implicit(xeno), (Enum)XenoVisualLayers.Base, (object)XenoRestState.Resting, (AppearanceComponent)null);
				_actions.SetToggled(args.Action.AsNullable(), toggled: true);
			}
			_actionBlocker.UpdateCanMove(Entity<XenoComponent>.op_Implicit(xeno));
			XenoRestEvent ev = new XenoRestEvent(((EntitySystem)this).HasComp<XenoRestingComponent>(Entity<XenoComponent>.op_Implicit(xeno)));
			((EntitySystem)this).RaiseLocalEvent<XenoRestEvent>(Entity<XenoComponent>.op_Implicit(xeno), ref ev, false);
		}
	}

	private void OnXenoRestingMeleeHit(Entity<XenoRestingComponent> xeno, ref AttackAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnXenoSecreteStructureAttempt(Entity<XenoRestingComponent> xeno, ref XenoSecreteStructureAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-secrete"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingHeadbuttAttempt(Entity<XenoRestingComponent> xeno, ref XenoHeadbuttAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-headbutt"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingFortifyAttempt(Entity<XenoRestingComponent> xeno, ref XenoFortifyAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-fortify"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingTailSweepAttempt(Entity<XenoRestingComponent> xeno, ref XenoTailSweepAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-tail-sweep"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingToggleCrestAttempt(Entity<XenoRestingComponent> xeno, ref XenoToggleCrestAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-toggle-crest"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingLeapAttempt(Entity<XenoRestingComponent> xeno, ref XenoLeapAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-leap"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingLungeAttempt(Entity<XenoRestingComponent> xeno, ref XenoLungeAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-lunge"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingPunchAttempt(Entity<XenoRestingComponent> xeno, ref XenoPunchAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-punch"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingFlingAttempt(Entity<XenoRestingComponent> xeno, ref XenoFlingAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-fling"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingChargettempt(Entity<XenoRestingComponent> xeno, ref XenoChargeAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-charge"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingStompAttempt(Entity<XenoRestingComponent> xeno, ref XenoStompAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-stomp"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingGutAttempt(Entity<XenoRestingComponent> xeno, ref XenoGutAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-gut"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingScreechAttempt(Entity<XenoRestingComponent> xeno, ref XenoScreechAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rest-cant-screech"), Entity<XenoRestingComponent>.op_Implicit(xeno), Entity<XenoRestingComponent>.op_Implicit(xeno));
		args.Cancelled = true;
	}

	private void OnXenoRestingEvasionRefresh(Entity<XenoRestingComponent> xeno, ref EvasionRefreshModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!(xeno.Owner != args.Entity.Owner))
		{
			args.Evasion += (FixedPoint2)(-15);
		}
	}

	private void OnXenoRestingMobCollide(Entity<XenoRestingComponent> ent, ref AttemptMobCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void OnXenoRestingMobTargetCollide(Entity<XenoRestingComponent> ent, ref AttemptMobTargetCollideEvent args)
	{
		args.Cancelled = true;
	}

	public bool IsResting(Entity<XenoRestingComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ((EntitySystem)this).Resolve<XenoRestingComponent>(Entity<XenoRestingComponent>.op_Implicit(ent), ref ent.Comp, false);
	}
}
