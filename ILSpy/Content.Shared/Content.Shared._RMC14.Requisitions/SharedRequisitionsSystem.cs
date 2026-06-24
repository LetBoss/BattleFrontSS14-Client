using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Requisitions.Components;
using Content.Shared._RMC14.Scaling;
using Content.Shared.Climbing.Components;
using Content.Shared.GameTicking;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Requisitions;

public abstract class SharedRequisitionsSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private FixtureSystem _fixtures;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedMapSystem _map;

	private MapId? _purchasesMap;

	public int Starting { get; private set; }

	public int StartingDollarsPerMarine { get; private set; }

	public int PointsScale { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineScaleChangedEvent>((EntityEventRefHandler<MarineScaleChangedEvent>)OnMarineScaleChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequisitionsElevatorComponent, StepTriggerAttemptEvent>((EntityEventRefHandler<RequisitionsElevatorComponent, StepTriggerAttemptEvent>)OnElevatorStepTriggerAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequisitionsRailingComponent, MapInitEvent>((EntityEventRefHandler<RequisitionsRailingComponent, MapInitEvent>)OnRailingMapInit, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCRequisitionsStartingBalance, (Action<int>)delegate(int v)
		{
			Starting = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCRequisitionsStartingDollarsPerMarine, (Action<int>)delegate(int v)
		{
			StartingDollarsPerMarine = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCRequisitionsPointsScale, (Action<int>)delegate(int v)
		{
			PointsScale = v;
		}, true);
	}

	private void OnMarineScaleChanged(ref MarineScaleChangedEvent ev)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!(ev.Delta <= 0.0))
		{
			EntityQueryEnumerator<RequisitionsAccountComponent> accounts = ((EntitySystem)this).EntityQueryEnumerator<RequisitionsAccountComponent>();
			EntityUid uid = default(EntityUid);
			RequisitionsAccountComponent account = default(RequisitionsAccountComponent);
			while (accounts.MoveNext(ref uid, ref account))
			{
				account.Balance += (int)ev.Delta * PointsScale;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)account, (MetaDataComponent)null);
			}
		}
	}

	private void OnElevatorStepTriggerAttempt(Entity<RequisitionsElevatorComponent> elevator, ref StepTriggerAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (elevator.Comp.Mode == RequisitionsElevatorMode.Raised)
		{
			args.Cancelled = true;
		}
	}

	private void OnRailingMapInit(Entity<RequisitionsRailingComponent> railing, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateRailing(railing);
	}

	private void UpdateRailing(Entity<RequisitionsRailingComponent> railing)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		FixturesComponent fixtures = default(FixturesComponent);
		if (!((EntitySystem)this).TryComp<FixturesComponent>(Entity<RequisitionsRailingComponent>.op_Implicit(railing), ref fixtures))
		{
			return;
		}
		Fixture fixture = _fixtures.GetFixtureOrNull(Entity<RequisitionsRailingComponent>.op_Implicit(railing), railing.Comp.Fixture, fixtures);
		if (fixture != null)
		{
			RequisitionsRailingMode mode = railing.Comp.Mode;
			bool flag = ((mode == RequisitionsRailingMode.Raised || mode == RequisitionsRailingMode.Raising) ? true : false);
			bool hard = flag;
			_physics.SetHard(Entity<RequisitionsRailingComponent>.op_Implicit(railing), fixture, hard, (FixturesComponent)null);
			if (hard)
			{
				((EntitySystem)this).EnsureComp<ClimbableComponent>(Entity<RequisitionsRailingComponent>.op_Implicit(railing));
			}
			else
			{
				((EntitySystem)this).RemCompDeferred<ClimbableComponent>(Entity<RequisitionsRailingComponent>.op_Implicit(railing));
			}
		}
	}

	protected void SetRailingMode(Entity<RequisitionsRailingComponent> railing, RequisitionsRailingMode mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (railing.Comp.Mode != mode)
		{
			railing.Comp.Mode = mode;
			((EntitySystem)this).Dirty<RequisitionsRailingComponent>(railing, (MetaDataComponent)null);
			UpdateRailing(railing);
		}
	}

	public void ChangeBudget(int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RequisitionsAccountComponent> accountQuery = ((EntitySystem)this).EntityQueryEnumerator<RequisitionsAccountComponent>();
		EntityUid uid = default(EntityUid);
		RequisitionsAccountComponent comp = default(RequisitionsAccountComponent);
		while (accountQuery.MoveNext(ref uid, ref comp))
		{
			comp.Balance += amount;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
		SendUIStateAll();
	}

	protected void SendUIStateAll()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RequisitionsComputerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RequisitionsComputerComponent>();
		EntityUid uid = default(EntityUid);
		RequisitionsComputerComponent computer = default(RequisitionsComputerComponent);
		while (query.MoveNext(ref uid, ref computer))
		{
			SendUIState(Entity<RequisitionsComputerComponent>.op_Implicit((uid, computer)));
		}
	}

	protected void SendUIState(Entity<RequisitionsComputerComponent> computer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		Entity<RequisitionsElevatorComponent>? elevator = GetElevator(computer);
		RequisitionsElevatorMode? platformLowered = elevator?.Comp.NextMode ?? elevator?.Comp.Mode;
		bool busy = elevator?.Comp.Busy ?? false;
		int balance = ((EntitySystem)this).CompOrNull<RequisitionsAccountComponent>(computer.Comp.Account)?.Balance ?? 0;
		bool full = elevator.HasValue && IsFull(elevator.Value);
		RequisitionsBuiState state = new RequisitionsBuiState(platformLowered, busy, balance, full);
		_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(computer.Owner), (Enum)RequisitionsUIKey.Key, (BoundUserInterfaceState)(object)state);
	}

	protected bool IsFull(Entity<RequisitionsElevatorComponent> elevator)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return elevator.Comp.Orders.Count >= GetElevatorCapacity(elevator);
	}

	protected int GetElevatorCapacity(Entity<RequisitionsElevatorComponent> elevator)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)MathF.Floor(elevator.Comp.Radius * 2f + 1f);
		return num * num;
	}

	protected Entity<RequisitionsElevatorComponent>? GetElevator(Entity<RequisitionsComputerComponent> computer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		List<Entity<RequisitionsElevatorComponent, TransformComponent>> elevators = new List<Entity<RequisitionsElevatorComponent, TransformComponent>>();
		EntityQueryEnumerator<RequisitionsElevatorComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RequisitionsElevatorComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		RequisitionsElevatorComponent elevator = default(RequisitionsElevatorComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref elevator, ref xform))
		{
			elevators.Add(Entity<RequisitionsElevatorComponent, TransformComponent>.op_Implicit((uid, elevator, xform)));
		}
		if (elevators.Count == 0)
		{
			return null;
		}
		if (elevators.Count == 1)
		{
			return Entity<RequisitionsElevatorComponent, TransformComponent>.op_Implicit(elevators[0]);
		}
		MapCoordinates computerCoords = _transform.GetMapCoordinates(Entity<RequisitionsComputerComponent>.op_Implicit(computer), (TransformComponent)null);
		Entity<RequisitionsElevatorComponent>? closest = null;
		float closestDistance = float.MaxValue;
		EntityUid val = default(EntityUid);
		RequisitionsElevatorComponent requisitionsElevatorComponent = default(RequisitionsElevatorComponent);
		TransformComponent val2 = default(TransformComponent);
		foreach (Entity<RequisitionsElevatorComponent, TransformComponent> item in elevators)
		{
			item.Deconstruct(ref val, ref requisitionsElevatorComponent, ref val2);
			EntityUid uid2 = val;
			RequisitionsElevatorComponent elevator2 = requisitionsElevatorComponent;
			TransformComponent xform2 = val2;
			MapCoordinates elevatorCoords = _transform.GetMapCoordinates(uid2, xform2);
			if (!(computerCoords.MapId != elevatorCoords.MapId))
			{
				float distance = (elevatorCoords.Position - computerCoords.Position).LengthSquared();
				if (closestDistance > distance)
				{
					closestDistance = distance;
					closest = Entity<RequisitionsElevatorComponent>.op_Implicit((uid2, elevator2));
				}
			}
		}
		return closest ?? new Entity<RequisitionsElevatorComponent>?(Entity<RequisitionsElevatorComponent, TransformComponent>.op_Implicit(elevators[0]));
	}

	public void StartAccount(Entity<RequisitionsAccountComponent> account, double scale, float marines)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!account.Comp.Started)
		{
			account.Comp.Started = true;
			int startingPoints = Starting;
			int scalePoints = (int)((double)PointsScale * scale);
			int perMarinePoints = (int)((float)StartingDollarsPerMarine * marines);
			account.Comp.Balance = startingPoints + scalePoints + perMarinePoints;
			((EntitySystem)this).Dirty<RequisitionsAccountComponent>(account, (MetaDataComponent)null);
		}
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_purchasesMap = null;
	}

	public void CreateSpecialDelivery(EntProtoId proto)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		MapId map = EnsurePurchasesMap();
		EntityUid delivery = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(proto), new MapCoordinates(Vector2.Zero, map), (ComponentRegistry)null, default(Angle));
		((EntitySystem)this).EnsureComp<RequisitionsCustomDeliveryComponent>(delivery);
	}

	private MapId EnsurePurchasesMap()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (_purchasesMap.HasValue)
		{
			return _purchasesMap.Value;
		}
		MapId map = default(MapId);
		_map.CreateMap(ref map, true);
		_purchasesMap = map;
		return map;
	}
}
