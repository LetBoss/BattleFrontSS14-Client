// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gravity.SharedGravitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Gravity;

public abstract class SharedGravitySystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  private AlertsSystem _alerts;
  public static readonly ProtoId<AlertPrototype> WeightlessAlert = (ProtoId<AlertPrototype>) "Weightless";
  private Robust.Shared.GameObjects.EntityQuery<GravityComponent> _gravityQuery;
  protected const float GravityKick = 100f;
  protected const float ShakeCooldown = 0.2f;

  public bool IsWeightless(EntityUid uid, PhysicsComponent? body = null, TransformComponent? xform = null)
  {
    this.Resolve<PhysicsComponent>(uid, ref body, false);
    if ((body != null ? ((body.BodyType & BodyType.Static) != 0 ? 1 : 0) : 1) != 0)
      return false;
    MovementIgnoreGravityComponent comp;
    if (this.TryComp<MovementIgnoreGravityComponent>(uid, out comp))
      return comp.Weightless;
    IsWeightlessEvent args = new IsWeightlessEvent(uid);
    this.RaiseLocalEvent<IsWeightlessEvent>(uid, ref args);
    if (args.Handled)
      return args.IsWeightless;
    return !this.Resolve(uid, ref xform) || !this.EntityGridOrMapHaveGravity((Entity<TransformComponent>) (uid, xform));
  }

  public bool EntityOnGravitySupportingGridOrMap(Entity<TransformComponent?> entity)
  {
    ref TransformComponent local = ref entity.Comp;
    if (local == null)
      local = this.Transform((EntityUid) entity);
    return this._gravityQuery.HasComp(entity.Comp.GridUid) || this._gravityQuery.HasComp(entity.Comp.MapUid);
  }

  public bool EntityGridOrMapHaveGravity(Entity<TransformComponent?> entity)
  {
    ref TransformComponent local = ref entity.Comp;
    if (local == null)
      local = this.Transform((EntityUid) entity);
    GravityComponent component1;
    if (this._gravityQuery.TryComp(entity.Comp.GridUid, out component1) && component1.Enabled)
      return true;
    GravityComponent component2;
    return this._gravityQuery.TryComp(entity.Comp.MapUid, out component2) && component2.Enabled;
  }

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GridInitializeEvent>(new EntityEventHandler<GridInitializeEvent>(this.OnGridInit));
    this.SubscribeLocalEvent<AlertSyncEvent>(new EntityEventHandler<AlertSyncEvent>(this.OnAlertsSync));
    this.SubscribeLocalEvent<AlertsComponent, EntParentChangedMessage>(new ComponentEventRefHandler<AlertsComponent, EntParentChangedMessage>(this.OnAlertsParentChange));
    this.SubscribeLocalEvent<GravityChangedEvent>(new EntityEventRefHandler<GravityChangedEvent>(this.OnGravityChange));
    this.SubscribeLocalEvent<GravityComponent, ComponentGetState>(new ComponentEventRefHandler<GravityComponent, ComponentGetState>(this.OnGetState));
    this.SubscribeLocalEvent<GravityComponent, ComponentHandleState>(new ComponentEventRefHandler<GravityComponent, ComponentHandleState>(this.OnHandleState));
    this._gravityQuery = this.GetEntityQuery<GravityComponent>();
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    this.UpdateShake();
  }

  private void OnHandleState(
    EntityUid uid,
    GravityComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is SharedGravitySystem.GravityComponentState current) || component.EnabledVV == current.Enabled)
      return;
    component.EnabledVV = current.Enabled;
    GravityChangedEvent args1 = new GravityChangedEvent(uid, component.EnabledVV);
    this.RaiseLocalEvent<GravityChangedEvent>(uid, ref args1, true);
  }

  private void OnGetState(EntityUid uid, GravityComponent component, ref ComponentGetState args)
  {
    args.State = (IComponentState) new SharedGravitySystem.GravityComponentState(component.EnabledVV);
  }

  private void OnGravityChange(ref GravityChangedEvent ev)
  {
    AllEntityQueryEnumerator<AlertsComponent, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<AlertsComponent, TransformComponent>();
    EntityUid uid;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out AlertsComponent _, out comp2))
    {
      EntityUid? gridUid = comp2.GridUid;
      EntityUid changedGridIndex = ev.ChangedGridIndex;
      if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() != changedGridIndex ? 1 : 0) : 1) == 0)
      {
        if (!ev.HasGravity)
          this._alerts.ShowAlert(uid, SharedGravitySystem.WeightlessAlert);
        else
          this._alerts.ClearAlert(uid, SharedGravitySystem.WeightlessAlert);
      }
    }
  }

  private void OnAlertsSync(AlertSyncEvent ev)
  {
    if (this.IsWeightless(ev.Euid))
      this._alerts.ShowAlert(ev.Euid, SharedGravitySystem.WeightlessAlert);
    else
      this._alerts.ClearAlert(ev.Euid, SharedGravitySystem.WeightlessAlert);
  }

  private void OnAlertsParentChange(
    EntityUid uid,
    AlertsComponent component,
    ref EntParentChangedMessage args)
  {
    if (this.IsWeightless(uid))
      this._alerts.ShowAlert(uid, SharedGravitySystem.WeightlessAlert);
    else
      this._alerts.ClearAlert(uid, SharedGravitySystem.WeightlessAlert);
  }

  private void OnGridInit(GridInitializeEvent ev)
  {
    this.EnsureComp<GravityComponent>(ev.EntityUid);
  }

  private void UpdateShake()
  {
    TimeSpan curTime = this.Timing.CurTime;
    Robust.Shared.GameObjects.EntityQuery<GravityComponent> entityQuery = this.GetEntityQuery<GravityComponent>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<GravityShakeComponent> entityQueryEnumerator = this.EntityQueryEnumerator<GravityShakeComponent>();
    EntityUid uid;
    GravityShakeComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.NextShake <= curTime)
      {
        GravityComponent component;
        if (comp1.ShakeTimes == 0 || !entityQuery.TryGetComponent(uid, out component))
        {
          this.RemCompDeferred<GravityShakeComponent>(uid);
        }
        else
        {
          this.ShakeGrid(uid, component);
          --comp1.ShakeTimes;
          comp1.NextShake += TimeSpan.FromSeconds(0.20000000298023224);
          this.Dirty(uid, (IComponent) comp1);
        }
      }
    }
  }

  public void StartGridShake(EntityUid uid, GravityComponent? gravity = null)
  {
    if (this.Terminating(uid) || !this.Resolve<GravityComponent>(uid, ref gravity, false))
      return;
    GravityShakeComponent comp;
    if (!this.TryComp<GravityShakeComponent>(uid, out comp))
    {
      comp = this.AddComp<GravityShakeComponent>(uid);
      comp.NextShake = this.Timing.CurTime;
    }
    comp.ShakeTimes = 10;
    this.Dirty(uid, (IComponent) comp);
  }

  protected virtual void ShakeGrid(EntityUid uid, GravityComponent? comp = null)
  {
  }

  [NetSerializable]
  [Serializable]
  private sealed class GravityComponentState : ComponentState
  {
    public bool Enabled { get; }

    public GravityComponentState(bool enabled) => this.Enabled = enabled;
  }
}
