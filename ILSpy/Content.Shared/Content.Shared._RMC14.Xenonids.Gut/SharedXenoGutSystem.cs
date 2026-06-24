using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Gibbing;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Xenonids.Gut;

public sealed class SharedXenoGutSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedBodySystem _bodySystem;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private RMCGibSystem _rmcGib;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoGutComponent, XenoGutActionEvent>((EntityEventRefHandler<XenoGutComponent, XenoGutActionEvent>)OnXenoGutAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoGutComponent, XenoGutDoAfterEvent>((EntityEventRefHandler<XenoGutComponent, XenoGutDoAfterEvent>)OnXenoGutDoAfterEvent, (Type[])null, (Type[])null);
	}

	private void OnXenoGutAction(Entity<XenoGutComponent> xeno, ref XenoGutActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		if (args.Target == xeno.Owner || ((EntitySystem)this).HasComp<XenoComponent>(args.Target) || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		XenoGutAttemptEvent attempt = default(XenoGutAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoGutAttemptEvent>(Entity<XenoGutComponent>.op_Implicit(xeno), ref attempt, false);
		if (!attempt.Cancelled)
		{
			EntityUid target = args.Target;
			if (((EntitySystem)this).HasComp<BodyComponent>(target) && _xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
			{
				((HandledEntityEventArgs)args).Handled = true;
				XenoGutDoAfterEvent ev = new XenoGutDoAfterEvent();
				DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoGutComponent>.op_Implicit(xeno), xeno.Comp.Delay, ev, Entity<XenoGutComponent>.op_Implicit(xeno), target)
				{
					BreakOnMove = true,
					BlockDuplicate = true,
					DuplicateCondition = DuplicateConditions.SameEvent
				};
				string selfMsg = base.Loc.GetString("rmc-gut-start-self");
				string othersMsg = base.Loc.GetString("rmc-gut-start-others", (ValueTuple<string, object>)("user", xeno.Owner), (ValueTuple<string, object>)("target", args.Target));
				_popup.PopupPredicted(selfMsg, othersMsg, xeno.Owner, xeno.Owner, PopupType.LargeCaution);
				_doAfter.TryStartDoAfter(doAfter);
				_jitter.DoJitter(args.Target, xeno.Comp.Delay, refresh: true, 14f, 5f, forceValueChange: true);
			}
		}
	}

	private void OnXenoGutDoAfterEvent(Entity<XenoGutComponent> xeno, ref XenoGutDoAfterEvent args)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target;
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				BodyComponent body = default(BodyComponent);
				if (target2 == xeno.Owner || ((EntitySystem)this).HasComp<XenoComponent>(target2) || !((EntitySystem)this).TryComp<BodyComponent>(target2, ref body) || !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
				{
					return;
				}
				((HandledEntityEventArgs)args).Handled = true;
				if (_net.IsServer)
				{
					_rmcGib.ScatterInventoryItems(target2);
					_bodySystem.GibBody(target2, gibOrgans: true, body);
					_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoGutComponent>.op_Implicit(xeno), (AudioParams?)null);
				}
				string selfMsg = base.Loc.GetString("rmc-gut-finish-self");
				string othersMsg = base.Loc.GetString("rmc-gut-finish-others", (ValueTuple<string, object>)("user", xeno.Owner), (ValueTuple<string, object>)("target", args.Target));
				_popup.PopupPredicted(selfMsg, othersMsg, xeno.Owner, xeno.Owner, PopupType.LargeCaution);
				{
					foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoGutActionEvent>(Entity<XenoGutComponent>.op_Implicit(xeno)))
					{
						_actions.SetIfBiggerCooldown(action.AsNullable(), xeno.Comp.Cooldown);
					}
					return;
				}
			}
		}
		target = args.Target;
		if (target.HasValue)
		{
			EntityUid cancelledTarget = target.GetValueOrDefault();
			_statusEffects.TryRemoveStatusEffect(cancelledTarget, "Jitter");
		}
	}
}
