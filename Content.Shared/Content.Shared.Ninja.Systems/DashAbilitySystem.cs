using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Ninja.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Ninja.Systems;

public sealed class DashAbilitySystem : EntitySystem
{
	[Dependency]
	private ActionContainerSystem _actionContainer;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedChargesSystem _sharedCharges;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private PullingSystem _pullingSystem;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DashAbilityComponent, GetItemActionsEvent>((EntityEventRefHandler<DashAbilityComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DashAbilityComponent, DashEvent>((EntityEventRefHandler<DashAbilityComponent, DashEvent>)OnDash, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DashAbilityComponent, MapInitEvent>((EntityEventRefHandler<DashAbilityComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<DashAbilityComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Entity<DashAbilityComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		DashAbilityComponent dashAbilityComponent = default(DashAbilityComponent);
		val.Deconstruct(ref val2, ref dashAbilityComponent);
		EntityUid uid = val2;
		DashAbilityComponent comp = dashAbilityComponent;
		_actionContainer.EnsureAction(uid, ref comp.DashActionEntity, EntProtoId<WorldTargetActionComponent>.op_Implicit(comp.DashAction));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	private void OnGetActions(Entity<DashAbilityComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (CheckDash(Entity<DashAbilityComponent>.op_Implicit(ent), args.User))
		{
			args.AddAction(ent.Comp.DashActionEntity);
		}
	}

	private void OnDash(Entity<DashAbilityComponent> ent, ref DashEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		Entity<DashAbilityComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		DashAbilityComponent dashAbilityComponent = default(DashAbilityComponent);
		val.Deconstruct(ref val2, ref dashAbilityComponent);
		EntityUid uid = val2;
		EntityUid user = args.Performer;
		if (!CheckDash(uid, user))
		{
			return;
		}
		if (!_hands.IsHolding(Entity<HandsComponent>.op_Implicit(user), uid, out string _))
		{
			_popup.PopupClient(base.Loc.GetString("dash-ability-not-held", (ValueTuple<string, object>)("item", uid)), user, user);
			return;
		}
		MapCoordinates origin = _transform.GetMapCoordinates(user, (TransformComponent)null);
		MapCoordinates target = _transform.ToMapCoordinates(args.Target, true);
		if (!_examine.InRangeUnOccluded(origin, target, 100f, null))
		{
			_popup.PopupClient(base.Loc.GetString("dash-ability-cant-see", (ValueTuple<string, object>)("item", uid)), user, user);
			return;
		}
		if (!_sharedCharges.TryUseCharge(Entity<LimitedChargesComponent>.op_Implicit(uid)))
		{
			_popup.PopupClient(base.Loc.GetString("dash-ability-no-charges", (ValueTuple<string, object>)("item", uid)), user, user);
			return;
		}
		PullableComponent pull = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(user, ref pull) && _pullingSystem.IsPulled(user, pull))
		{
			_pullingSystem.TryStopPull(user, pull);
		}
		PullerComponent puller = default(PullerComponent);
		PullableComponent pullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullerComponent>(user, ref puller) && ((EntitySystem)this).TryComp<PullableComponent>(puller.Pulling, ref pullable))
		{
			_pullingSystem.TryStopPull(puller.Pulling.Value, pullable);
		}
		TransformComponent xform = ((EntitySystem)this).Transform(user);
		_transform.SetCoordinates(user, xform, args.Target, (Angle?)null, true, (TransformComponent)null, (TransformComponent)null);
		_transform.AttachToGridOrMap(user, xform);
		((HandledEntityEventArgs)args).Handled = true;
	}

	public bool CheckDash(EntityUid uid, EntityUid user)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		CheckDashEvent ev = new CheckDashEvent(user);
		((EntitySystem)this).RaiseLocalEvent<CheckDashEvent>(uid, ref ev, false);
		return !ev.Cancelled;
	}
}
