using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Marines.Orders;

public abstract class SharedMarineOrdersSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private EvasionSystem _evasionSystem;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private IGameTiming _timing;

	private readonly HashSet<Entity<MarineComponent>> _receivers = new HashSet<Entity<MarineComponent>>();

	private EntityQuery<MoveOrderArmorComponent> _moveOrderArmorQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_moveOrderArmorQuery = ((EntitySystem)this).GetEntityQuery<MoveOrderArmorComponent>();
		((EntitySystem)this).SubscribeLocalEvent<MoveOrderComponent, EntityUnpausedEvent>((EntityEventRefHandler<MoveOrderComponent, EntityUnpausedEvent>)OnUnpause<MoveOrderComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FocusOrderComponent, EntityUnpausedEvent>((EntityEventRefHandler<FocusOrderComponent, EntityUnpausedEvent>)OnUnpause<FocusOrderComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HoldOrderComponent, EntityUnpausedEvent>((EntityEventRefHandler<HoldOrderComponent, EntityUnpausedEvent>)OnUnpause<HoldOrderComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineOrdersComponent, FocusActionEvent>((EntityEventRefHandler<MarineOrdersComponent, FocusActionEvent>)OnAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineOrdersComponent, HoldActionEvent>((EntityEventRefHandler<MarineOrdersComponent, HoldActionEvent>)OnAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineOrdersComponent, MoveActionEvent>((EntityEventRefHandler<MarineOrdersComponent, MoveActionEvent>)OnAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MoveOrderComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<MoveOrderComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovement, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MoveOrderComponent, ComponentShutdown>((EntityEventRefHandler<MoveOrderComponent, ComponentShutdown>)OnMoveShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MoveOrderComponent, EvasionRefreshModifiersEvent>((EntityEventRefHandler<MoveOrderComponent, EvasionRefreshModifiersEvent>)OnMoveOrderEvasionRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MoveOrderComponent, DidEquipEvent>((EntityEventRefHandler<MoveOrderComponent, DidEquipEvent>)OnMoveOrderDidEquip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MoveOrderComponent, DidUnequipEvent>((EntityEventRefHandler<MoveOrderComponent, DidUnequipEvent>)OnMoveOrderDidUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HoldOrderComponent, DamageModifyEvent>((EntityEventRefHandler<HoldOrderComponent, DamageModifyEvent>)OnDamageModify, (Type[])null, (Type[])null);
	}

	private void OnDamageModify(Entity<HoldOrderComponent> orders, ref DamageModifyEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		HoldOrderComponent comp = orders.Comp;
		if (comp.Received.Count == 0)
		{
			return;
		}
		Dictionary<string, FixedPoint2> damage = args.Damage.DamageDict;
		FixedPoint2 multiplier = 1 - comp.DamageModifier * comp.Received[0].Multiplier;
		foreach (ProtoId<DamageTypePrototype> type in comp.DamageTypes)
		{
			if (damage.TryGetValue(ProtoId<DamageTypePrototype>.op_Implicit(type), out var amount))
			{
				damage[ProtoId<DamageTypePrototype>.op_Implicit(type)] = amount * multiplier;
			}
		}
	}

	private void OnUnpause<T>(Entity<T> orders, ref EntityUnpausedEvent args) where T : IComponent, IOrderComponent
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		T comp = orders.Comp;
		for (int i = 0; i < comp.Received.Count; i++)
		{
			(FixedPoint2, TimeSpan) received = comp.Received[i];
			comp.Received[i] = (received.Item1, received.Item2 + args.PausedTime);
		}
	}

	private void OnRefreshMovement(Entity<MoveOrderComponent> orders, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		MoveOrderComponent comp = orders.Comp;
		if (comp.Received.Count != 0)
		{
			float speed = 1f + (comp.MoveSpeedModifier * comp.Received[0].Multiplier).Float();
			args.ModifySpeed(speed, speed);
		}
	}

	private void OnMoveShutdown(Entity<MoveOrderComponent> uid, ref ComponentShutdown ev)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<MoveOrderComponent>.op_Implicit(uid));
		_evasionSystem.RefreshEvasionModifiers(uid.Owner);
	}

	private void OnMoveOrderEvasionRefresh(Entity<MoveOrderComponent> entity, ref EvasionRefreshModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!(entity.Owner != args.Entity.Owner) && entity.Comp.Received.Count != 0)
		{
			args.Evasion += entity.Comp.Received[0].Multiplier * entity.Comp.EvasionModifier;
		}
	}

	private void OnMoveOrderDidEquip(Entity<MoveOrderComponent> ent, ref DidEquipEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<MoveOrderComponent>.op_Implicit(ent));
	}

	private void OnMoveOrderDidUnequip(Entity<MoveOrderComponent> ent, ref DidUnequipEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<MoveOrderComponent>.op_Implicit(ent));
	}

	protected virtual void OnAction(Entity<MarineOrdersComponent> orders, ref FocusActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnAction<FocusOrderComponent>(orders, args);
	}

	protected virtual void OnAction(Entity<MarineOrdersComponent> orders, ref HoldActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnAction<HoldOrderComponent>(orders, args);
	}

	protected virtual void OnAction(Entity<MarineOrdersComponent> orders, ref MoveActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnAction<MoveOrderComponent>(orders, args);
	}

	private void OnAction<T>(Entity<MarineOrdersComponent> orders, InstantActionEvent args) where T : IOrderComponent, new()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && HandleAction<T>(orders))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool HandleAction<T>(Entity<MarineOrdersComponent> orders) where T : IOrderComponent, new()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(Entity<MarineOrdersComponent>.op_Implicit(orders), ref xform) || _mobState.IsDead(Entity<MarineOrdersComponent>.op_Implicit(orders)))
		{
			return false;
		}
		int level = Math.Max(1, _skills.GetSkill(Entity<SkillsComponent>.op_Implicit(orders.Owner), orders.Comp.Skill));
		TimeSpan duration = orders.Comp.Duration * (level + 1);
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
		_entityLookup.GetEntitiesInRange<MarineComponent>(xform.Coordinates, (float)orders.Comp.OrderRange, _receivers, (LookupFlags)110);
		foreach (Entity<MarineComponent> receiver in _receivers)
		{
			if (!_mobState.IsDead(Entity<MarineComponent>.op_Implicit(receiver)))
			{
				AddOrder<T>(receiver, level, duration);
			}
		}
		return true;
	}

	private void AddOrder<T>(Entity<MarineComponent> receiver, int multiplier, TimeSpan duration) where T : IOrderComponent, new()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		T comp = ((EntitySystem)this).EnsureComp<T>(Entity<MarineComponent>.op_Implicit(receiver));
		comp.Received.Add((multiplier, time + duration));
		comp.Received.Sort(((FixedPoint2 Multiplier, TimeSpan ExpiresAt) a, (FixedPoint2 Multiplier, TimeSpan ExpiresAt) b) => a.CompareTo(b));
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<MarineComponent>.op_Implicit(receiver));
		_evasionSystem.RefreshEvasionModifiers(Entity<MarineComponent>.op_Implicit(receiver));
	}

	private void RemoveExpired<T>() where T : IComponent, IOrderComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<T> query = ((EntitySystem)this).EntityQueryEnumerator<T>();
		TimeSpan time = _timing.CurTime;
		EntityUid uid = default(EntityUid);
		T comp = default(T);
		while (query.MoveNext(ref uid, ref comp))
		{
			for (int i = comp.Received.Count - 1; i >= 0; i--)
			{
				if (comp.Received[i].ExpiresAt < time)
				{
					comp.Received.RemoveAt(i);
				}
			}
			if (comp.Received.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<T>(uid);
			}
		}
	}

	public void StartActionUseDelay(Entity<MarineOrdersComponent> orders)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actions = _actions;
		EntityUid? holdActionEntity = orders.Comp.HoldActionEntity;
		actions.StartUseDelay(holdActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(holdActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SharedActionsSystem actions2 = _actions;
		holdActionEntity = orders.Comp.MoveActionEntity;
		actions2.StartUseDelay(holdActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(holdActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SharedActionsSystem actions3 = _actions;
		holdActionEntity = orders.Comp.FocusActionEntity;
		actions3.StartUseDelay(holdActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(holdActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		RemoveExpired<MoveOrderComponent>();
		RemoveExpired<FocusOrderComponent>();
		RemoveExpired<HoldOrderComponent>();
	}
}
