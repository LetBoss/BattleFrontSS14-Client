// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Requisitions.SharedRequisitionsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
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
    base.Initialize();
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<MarineScaleChangedEvent>(new EntityEventRefHandler<MarineScaleChangedEvent>(this.OnMarineScaleChanged));
    this.SubscribeLocalEvent<RequisitionsElevatorComponent, StepTriggerAttemptEvent>(new EntityEventRefHandler<RequisitionsElevatorComponent, StepTriggerAttemptEvent>(this.OnElevatorStepTriggerAttempt));
    this.SubscribeLocalEvent<RequisitionsRailingComponent, MapInitEvent>(new EntityEventRefHandler<RequisitionsRailingComponent, MapInitEvent>(this.OnRailingMapInit));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCRequisitionsStartingBalance, (Action<int>) (v => this.Starting = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCRequisitionsStartingDollarsPerMarine, (Action<int>) (v => this.StartingDollarsPerMarine = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCRequisitionsPointsScale, (Action<int>) (v => this.PointsScale = v), true);
  }

  private void OnMarineScaleChanged(ref MarineScaleChangedEvent ev)
  {
    if (ev.Delta <= 0.0)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RequisitionsAccountComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RequisitionsAccountComponent>();
    EntityUid uid;
    RequisitionsAccountComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Balance += (int) ev.Delta * this.PointsScale;
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  private void OnElevatorStepTriggerAttempt(
    Entity<RequisitionsElevatorComponent> elevator,
    ref StepTriggerAttemptEvent args)
  {
    if (elevator.Comp.Mode != RequisitionsElevatorMode.Raised)
      return;
    args.Cancelled = true;
  }

  private void OnRailingMapInit(Entity<RequisitionsRailingComponent> railing, ref MapInitEvent args)
  {
    this.UpdateRailing(railing);
  }

  private void UpdateRailing(Entity<RequisitionsRailingComponent> railing)
  {
    FixturesComponent comp;
    if (!this.TryComp<FixturesComponent>((EntityUid) railing, out comp))
      return;
    Fixture fixtureOrNull = this._fixtures.GetFixtureOrNull((EntityUid) railing, railing.Comp.Fixture, comp);
    if (fixtureOrNull == null)
      return;
    bool flag1;
    switch (railing.Comp.Mode)
    {
      case RequisitionsRailingMode.Raised:
      case RequisitionsRailingMode.Raising:
        flag1 = true;
        break;
      default:
        flag1 = false;
        break;
    }
    bool flag2 = flag1;
    this._physics.SetHard((EntityUid) railing, fixtureOrNull, flag2);
    if (flag2)
      this.EnsureComp<ClimbableComponent>((EntityUid) railing);
    else
      this.RemCompDeferred<ClimbableComponent>((EntityUid) railing);
  }

  protected void SetRailingMode(
    Entity<RequisitionsRailingComponent> railing,
    RequisitionsRailingMode mode)
  {
    if (railing.Comp.Mode == mode)
      return;
    railing.Comp.Mode = mode;
    this.Dirty<RequisitionsRailingComponent>(railing);
    this.UpdateRailing(railing);
  }

  public void ChangeBudget(int amount)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RequisitionsAccountComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RequisitionsAccountComponent>();
    EntityUid uid;
    RequisitionsAccountComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Balance += amount;
      this.Dirty(uid, (IComponent) comp1);
    }
    this.SendUIStateAll();
  }

  protected void SendUIStateAll()
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RequisitionsComputerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RequisitionsComputerComponent>();
    EntityUid uid;
    RequisitionsComputerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      this.SendUIState((Entity<RequisitionsComputerComponent>) (uid, comp1));
  }

  protected void SendUIState(Entity<RequisitionsComputerComponent> computer)
  {
    Entity<RequisitionsElevatorComponent>? elevator = this.GetElevator(computer);
    RequisitionsElevatorMode? platformLowered = (RequisitionsElevatorMode?) elevator?.Comp.NextMode ?? elevator?.Comp.Mode;
    bool flag1 = elevator.HasValue && elevator.GetValueOrDefault().Comp.Busy;
    RequisitionsAccountComponent accountComponent = this.CompOrNull<RequisitionsAccountComponent>(computer.Comp.Account);
    int balance1 = accountComponent != null ? accountComponent.Balance : 0;
    bool flag2 = elevator.HasValue && this.IsFull(elevator.Value);
    int num1 = flag1 ? 1 : 0;
    int balance2 = balance1;
    int num2 = flag2 ? 1 : 0;
    RequisitionsBuiState state = new RequisitionsBuiState(platformLowered, num1 != 0, balance2, num2 != 0);
    this._ui.SetUiState((Entity<UserInterfaceComponent>) computer.Owner, (Enum) RequisitionsUIKey.Key, (BoundUserInterfaceState) state);
  }

  protected bool IsFull(Entity<RequisitionsElevatorComponent> elevator)
  {
    return elevator.Comp.Orders.Count >= this.GetElevatorCapacity(elevator);
  }

  protected int GetElevatorCapacity(Entity<RequisitionsElevatorComponent> elevator)
  {
    int num = (int) MathF.Floor((float) ((double) elevator.Comp.Radius * 2.0 + 1.0));
    return num * num;
  }

  protected Entity<RequisitionsElevatorComponent>? GetElevator(
    Entity<RequisitionsComputerComponent> computer)
  {
    List<Entity<RequisitionsElevatorComponent, TransformComponent>> entityList = new List<Entity<RequisitionsElevatorComponent, TransformComponent>>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RequisitionsElevatorComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RequisitionsElevatorComponent, TransformComponent>();
    EntityUid uid;
    RequisitionsElevatorComponent comp1_1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1_1, out comp2))
      entityList.Add((Entity<RequisitionsElevatorComponent, TransformComponent>) (uid, comp1_1, comp2));
    if (entityList.Count == 0)
      return new Entity<RequisitionsElevatorComponent>?();
    if (entityList.Count == 1)
      return new Entity<RequisitionsElevatorComponent>?((Entity<RequisitionsElevatorComponent>) entityList[0]);
    MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates((EntityUid) computer);
    Entity<RequisitionsElevatorComponent>? nullable = new Entity<RequisitionsElevatorComponent>?();
    float num1 = float.MaxValue;
    foreach ((EntityUid entityUid, RequisitionsElevatorComponent comp1_2, TransformComponent transformComponent) in entityList)
    {
      MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates(entityUid, transformComponent);
      if (!(mapCoordinates1.MapId != mapCoordinates2.MapId))
      {
        float num2 = (mapCoordinates2.Position - mapCoordinates1.Position).LengthSquared();
        if ((double) num1 > (double) num2)
        {
          num1 = num2;
          nullable = new Entity<RequisitionsElevatorComponent>?((Entity<RequisitionsElevatorComponent>) (entityUid, comp1_2));
        }
      }
    }
    return nullable ?? new Entity<RequisitionsElevatorComponent>?((Entity<RequisitionsElevatorComponent>) entityList[0]);
  }

  public void StartAccount(
    Entity<RequisitionsAccountComponent> account,
    double scale,
    float marines)
  {
    if (account.Comp.Started)
      return;
    account.Comp.Started = true;
    int starting = this.Starting;
    int num1 = (int) ((double) this.PointsScale * scale);
    int num2 = (int) ((double) this.StartingDollarsPerMarine * (double) marines);
    account.Comp.Balance = starting + num1 + num2;
    this.Dirty<RequisitionsAccountComponent>(account);
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
  {
    this._purchasesMap = new MapId?();
  }

  public void CreateSpecialDelivery(EntProtoId proto)
  {
    MapId mapId = this.EnsurePurchasesMap();
    this.EnsureComp<RequisitionsCustomDeliveryComponent>(this.Spawn((string) proto, new MapCoordinates(Vector2.Zero, mapId), rotation: new Angle()));
  }

  private MapId EnsurePurchasesMap()
  {
    if (this._purchasesMap.HasValue)
      return this._purchasesMap.Value;
    MapId mapId;
    this._map.CreateMap(out mapId);
    this._purchasesMap = new MapId?(mapId);
    return mapId;
  }
}
