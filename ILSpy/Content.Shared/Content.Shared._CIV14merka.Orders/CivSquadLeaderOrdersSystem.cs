using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Marines.Orders;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._CIV14merka.Orders;

public abstract class CivSquadLeaderOrdersSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private EvasionSystem _evasionSystem;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private IGameTiming _timing;

	private readonly HashSet<Entity<CivTeamMemberComponent>> _receivers = new HashSet<Entity<CivTeamMemberComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CivSquadLeaderOrdersComponent, FocusActionEvent>((EntityEventRefHandler<CivSquadLeaderOrdersComponent, FocusActionEvent>)OnFocusAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivSquadLeaderOrdersComponent, HoldActionEvent>((EntityEventRefHandler<CivSquadLeaderOrdersComponent, HoldActionEvent>)OnHoldAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivSquadLeaderOrdersComponent, MoveActionEvent>((EntityEventRefHandler<CivSquadLeaderOrdersComponent, MoveActionEvent>)OnMoveAction, (Type[])null, (Type[])null);
	}

	protected virtual void OnFocusAction(Entity<CivSquadLeaderOrdersComponent> orders, ref FocusActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (HandleAction<FocusOrderComponent>(orders))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	protected virtual void OnHoldAction(Entity<CivSquadLeaderOrdersComponent> orders, ref HoldActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (HandleAction<HoldOrderComponent>(orders))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	protected virtual void OnMoveAction(Entity<CivSquadLeaderOrdersComponent> orders, ref MoveActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (HandleAction<MoveOrderComponent>(orders))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool HandleAction<T>(Entity<CivSquadLeaderOrdersComponent> orders) where T : IOrderComponent, new()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		CivTeamMemberComponent teamMember = default(CivTeamMemberComponent);
		if (!((EntitySystem)this).TryComp<CivTeamMemberComponent>(Entity<CivSquadLeaderOrdersComponent>.op_Implicit(orders), ref teamMember))
		{
			return false;
		}
		TransformComponent xform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(Entity<CivSquadLeaderOrdersComponent>.op_Implicit(orders), ref xform) || _mobState.IsDead(Entity<CivSquadLeaderOrdersComponent>.op_Implicit(orders)))
		{
			return false;
		}
		TimeSpan duration = orders.Comp.Duration * 2.0;
		SharedActionsSystem actions = _actions;
		EntityUid? focusActionEntity = orders.Comp.FocusActionEntity;
		actions.SetCooldown(focusActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(focusActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), orders.Comp.Cooldown);
		SharedActionsSystem actions2 = _actions;
		focusActionEntity = orders.Comp.MoveActionEntity;
		actions2.SetCooldown(focusActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(focusActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), orders.Comp.Cooldown);
		SharedActionsSystem actions3 = _actions;
		focusActionEntity = orders.Comp.HoldActionEntity;
		actions3.SetCooldown(focusActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(focusActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), orders.Comp.Cooldown);
		_receivers.Clear();
		_entityLookup.GetEntitiesInRange<CivTeamMemberComponent>(xform.Coordinates, (float)orders.Comp.OrderRange, _receivers, (LookupFlags)110);
		foreach (Entity<CivTeamMemberComponent> receiver in _receivers)
		{
			if (receiver.Comp.TeamId == teamMember.TeamId && !_mobState.IsDead(Entity<CivTeamMemberComponent>.op_Implicit(receiver)))
			{
				AddOrder<T>(Entity<CivTeamMemberComponent>.op_Implicit(receiver), 1, duration);
			}
		}
		return true;
	}

	private void AddOrder<T>(EntityUid receiver, int multiplier, TimeSpan duration) where T : IOrderComponent, new()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		T comp = ((EntitySystem)this).EnsureComp<T>(receiver);
		comp.Received.Add((multiplier, time + duration));
		comp.Received.Sort(((FixedPoint2 Multiplier, TimeSpan ExpiresAt) a, (FixedPoint2 Multiplier, TimeSpan ExpiresAt) b) => a.CompareTo(b));
		_movementSpeed.RefreshMovementSpeedModifiers(receiver);
		_evasionSystem.RefreshEvasionModifiers(receiver);
	}
}
