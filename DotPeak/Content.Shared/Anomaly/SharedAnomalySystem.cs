// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.SharedAnomalySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Prototypes;
using Content.Shared.Database;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Anomaly;

public abstract class SharedAnomalySystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  protected IRobustRandom Random;
  [Dependency]
  protected ISharedAdminLogManager AdminLog;
  [Dependency]
  protected SharedAudioSystem Audio;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  protected SharedPopupSystem Popup;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedMapSystem _map;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnomalyComponent, MeleeThrowOnHitStartEvent>(new EntityEventRefHandler<AnomalyComponent, MeleeThrowOnHitStartEvent>((object) this, __methodptr(OnAnomalyThrowStart)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnomalyComponent, LandEvent>(new EntityEventRefHandler<AnomalyComponent, LandEvent>((object) this, __methodptr(OnLand)), (Type[]) null, (Type[]) null);
  }

  private void OnAnomalyThrowStart(Entity<AnomalyComponent> ent, ref MeleeThrowOnHitStartEvent args)
  {
    CorePoweredThrowerComponent throwerComponent;
    PhysicsComponent physicsComponent1;
    if (!this.TryComp<CorePoweredThrowerComponent>(args.Weapon, ref throwerComponent) || !this.TryComp<PhysicsComponent>(Entity<AnomalyComponent>.op_Implicit(ent), ref physicsComponent1))
      return;
    PhysicsComponent physicsComponent2;
    if (this.TryComp<PhysicsComponent>(Entity<AnomalyComponent>.op_Implicit(ent), ref physicsComponent2) && physicsComponent2.BodyType == 4)
      this._physics.SetBodyType(Entity<AnomalyComponent>.op_Implicit(ent), (BodyType) 8, (FixturesComponent) null, physicsComponent1, (TransformComponent) null);
    this.ChangeAnomalyStability(Entity<AnomalyComponent>.op_Implicit(ent), this.Random.NextFloat(throwerComponent.StabilityPerThrow.X, throwerComponent.StabilityPerThrow.Y), ent.Comp);
  }

  private void OnLand(Entity<AnomalyComponent> ent, ref LandEvent args)
  {
    PhysicsComponent physicsComponent;
    if (!this.TryComp<PhysicsComponent>(Entity<AnomalyComponent>.op_Implicit(ent), ref physicsComponent) || physicsComponent.BodyType != 8)
      return;
    this._physics.SetBodyType(Entity<AnomalyComponent>.op_Implicit(ent), (BodyType) 4, (FixturesComponent) null, (PhysicsComponent) null, (TransformComponent) null);
  }

  public void DoAnomalyPulse(EntityUid uid, AnomalyComponent? component = null)
  {
    if (!this.Resolve<AnomalyComponent>(uid, ref component, true) || !this.Timing.IsFirstTimePredicted)
      return;
    this.RefreshPulseTimer(uid, component);
    if (this._net.IsServer)
      this.Log.Info($"Performing anomaly pulse. Entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
    if ((double) component.Stability > (double) component.GrowthThreshold)
      this.ChangeAnomalySeverity(uid, this.GetSeverityIncreaseFromGrowth(component), component);
    float change = this.Random.NextFloat(component.PulseStabilityVariation.X * component.Severity, component.PulseStabilityVariation.Y * component.Severity);
    this.ChangeAnomalyStability(uid, change, component);
    ISharedAdminLogManager adminLog = this.AdminLog;
    LogStringHandler logStringHandler = new LogStringHandler(31 /*0x1F*/, 2);
    logStringHandler.AppendLiteral("Anomaly ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" pulsed with severity ");
    logStringHandler.AppendFormatted<float>(component.Severity, "component.Severity");
    logStringHandler.AppendLiteral(".");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.Anomaly, LogImpact.Medium, ref local);
    if (this._net.IsServer)
      this.Audio.PlayPvs(component.PulseSound, uid, new AudioParams?());
    AnomalyPulsingComponent pulsingComponent = this.EnsureComp<AnomalyPulsingComponent>(uid);
    pulsingComponent.EndTime = this.Timing.CurTime + pulsingComponent.PulseDuration;
    this.Appearance.SetData(uid, (Enum) AnomalyVisuals.IsPulsing, (object) true, (AppearanceComponent) null);
    float PowerModifier = 1f;
    if (component.CurrentBehavior.HasValue)
    {
      IPrototypeManager prototype = this._prototype;
      ProtoId<AnomalyBehaviorPrototype>? currentBehavior = component.CurrentBehavior;
      string str = currentBehavior.HasValue ? ProtoId<AnomalyBehaviorPrototype>.op_Implicit(currentBehavior.GetValueOrDefault()) : (string) null;
      PowerModifier = prototype.Index<AnomalyBehaviorPrototype>(str).PulsePowerModifier;
    }
    AnomalyPulseEvent anomalyPulseEvent = new AnomalyPulseEvent(uid, component.Stability, component.Severity, PowerModifier);
    this.RaiseLocalEvent<AnomalyPulseEvent>(uid, ref anomalyPulseEvent, true);
  }

  public void RefreshPulseTimer(EntityUid uid, AnomalyComponent? component = null)
  {
    if (!this.Resolve<AnomalyComponent>(uid, ref component, true))
      return;
    float num = this.Random.NextFloat(-component.PulseVariation, component.PulseVariation) + 1f;
    component.NextPulseTime = this.Timing.CurTime + this.GetPulseLength(component) * (double) num;
  }

  public void StartSupercriticalEvent(Entity<AnomalyComponent?> ent)
  {
    if (this.HasComp<AnomalySupercriticalComponent>(Entity<AnomalyComponent>.op_Implicit(ent)) || !this.Resolve<AnomalyComponent>(Entity<AnomalyComponent>.op_Implicit(ent), ref ent.Comp, true))
      return;
    ISharedAdminLogManager adminLog = this.AdminLog;
    LogStringHandler logStringHandler = new LogStringHandler(35, 1);
    logStringHandler.AppendLiteral("Anomaly ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner)), "ToPrettyString(ent.Owner)");
    logStringHandler.AppendLiteral(" began to go supercritical.");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.Anomaly, LogImpact.High, ref local);
    if (this._net.IsServer)
      this.Log.Info($"Anomaly is going supercritical. Entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner))}");
    this.Audio.PlayPvs(ent.Comp.SupercriticalSoundAtAnimationStart, this.Transform(Entity<AnomalyComponent>.op_Implicit(ent)).Coordinates, new AudioParams?());
    AnomalySupercriticalComponent supercriticalComponent = this.AddComp<AnomalySupercriticalComponent>(Entity<AnomalyComponent>.op_Implicit(ent));
    supercriticalComponent.EndTime = this.Timing.CurTime + supercriticalComponent.SupercriticalDuration;
    this.Appearance.SetData(Entity<AnomalyComponent>.op_Implicit(ent), (Enum) AnomalyVisuals.Supercritical, (object) true, (AppearanceComponent) null);
    this.Dirty(Entity<AnomalyComponent>.op_Implicit(ent), (IComponent) supercriticalComponent, (MetaDataComponent) null);
  }

  public void DoAnomalySupercriticalEvent(EntityUid uid, AnomalyComponent? component = null)
  {
    if (!this.Resolve<AnomalyComponent>(uid, ref component, true) || !this.Timing.IsFirstTimePredicted)
      return;
    if (this._net.IsServer)
    {
      this.Audio.PlayPvs(component.SupercriticalSound, this.Transform(uid).Coordinates, new AudioParams?());
      this.Log.Info($"Raising supercritical event. Entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
    }
    float PowerModifier = 1f;
    if (component.CurrentBehavior.HasValue)
    {
      IPrototypeManager prototype = this._prototype;
      ProtoId<AnomalyBehaviorPrototype>? currentBehavior = component.CurrentBehavior;
      string str = currentBehavior.HasValue ? ProtoId<AnomalyBehaviorPrototype>.op_Implicit(currentBehavior.GetValueOrDefault()) : (string) null;
      PowerModifier = prototype.Index<AnomalyBehaviorPrototype>(str).PulsePowerModifier;
    }
    AnomalySupercriticalEvent supercriticalEvent = new AnomalySupercriticalEvent(uid, PowerModifier);
    this.RaiseLocalEvent<AnomalySupercriticalEvent>(uid, ref supercriticalEvent, true);
    this.EndAnomaly(uid, component, true, logged: true);
  }

  public void EndAnomaly(
    EntityUid uid,
    AnomalyComponent? component = null,
    bool supercritical = false,
    bool spawnCore = true,
    bool logged = false)
  {
    if (logged)
    {
      if (this._net.IsServer)
        this.Log.Info($"Ending anomaly. Entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
      ISharedAdminLogManager adminLog = this.AdminLog;
      int impact = supercritical ? 1 : -1;
      LogStringHandler logStringHandler = new LogStringHandler(10, 2);
      logStringHandler.AppendLiteral("Anomaly ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted(supercritical ? "went supercritical" : "decayed");
      logStringHandler.AppendLiteral(".");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Anomaly, (LogImpact) impact, ref local);
    }
    if (!this.Resolve<AnomalyComponent>(uid, ref component, true))
      return;
    AnomalyShutdownEvent anomalyShutdownEvent = new AnomalyShutdownEvent(uid, supercritical);
    this.RaiseLocalEvent<AnomalyShutdownEvent>(uid, ref anomalyShutdownEvent, true);
    if (this.Terminating(uid, (MetaDataComponent) null) || this._net.IsClient)
      return;
    if (spawnCore)
    {
      EntProtoId? nullable = supercritical ? component.CorePrototype : component.CoreInertPrototype;
      this._transform.PlaceNextTo(Entity<TransformComponent>.op_Implicit(this.Spawn(nullable.HasValue ? EntProtoId.op_Implicit(nullable.GetValueOrDefault()) : (string) null, this.Transform(uid).Coordinates)), Entity<TransformComponent>.op_Implicit(uid));
    }
    if (component.DeleteEntity)
      this.QueueDel(new EntityUid?(uid));
    else
      this.RemCompDeferred<AnomalySupercriticalComponent>(uid);
  }

  public void ChangeAnomalyStability(EntityUid uid, float change, AnomalyComponent? component = null)
  {
    if (!this.Resolve<AnomalyComponent>(uid, ref component, true))
      return;
    float num = component.Stability + change;
    component.Stability = Math.Clamp(num, 0.0f, 1f);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    AnomalyStabilityChangedEvent stabilityChangedEvent = new AnomalyStabilityChangedEvent(uid, component.Stability, component.Severity);
    this.RaiseLocalEvent<AnomalyStabilityChangedEvent>(uid, ref stabilityChangedEvent, true);
  }

  public void ChangeAnomalySeverity(EntityUid uid, float change, AnomalyComponent? component = null)
  {
    if (!this.Resolve<AnomalyComponent>(uid, ref component, true))
      return;
    float num = component.Severity + change;
    if ((double) num >= 1.0)
      this.StartSupercriticalEvent(Entity<AnomalyComponent>.op_Implicit((uid, component)));
    component.Severity = Math.Clamp(num, 0.0f, 1f);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    AnomalySeverityChangedEvent severityChangedEvent = new AnomalySeverityChangedEvent(uid, component.Stability, component.Severity);
    this.RaiseLocalEvent<AnomalySeverityChangedEvent>(uid, ref severityChangedEvent, true);
  }

  public void ChangeAnomalyHealth(EntityUid uid, float change, AnomalyComponent? component = null)
  {
    if (!this.Resolve<AnomalyComponent>(uid, ref component, true))
      return;
    float num = component.Health + change;
    if ((double) num < 0.0)
    {
      this.EndAnomaly(uid, component, logged: true);
    }
    else
    {
      component.Health = Math.Clamp(num, 0.0f, 1f);
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
      AnomalyHealthChangedEvent healthChangedEvent = new AnomalyHealthChangedEvent(uid, component.Health);
      this.RaiseLocalEvent<AnomalyHealthChangedEvent>(uid, ref healthChangedEvent, true);
    }
  }

  public TimeSpan GetPulseLength(AnomalyComponent component)
  {
    float num = Math.Clamp((component.Stability - component.GrowthThreshold) / component.GrowthThreshold, 0.0f, 1f);
    TimeSpan pulseLength = (component.MaxPulseLength - component.MinPulseLength) * (double) num + component.MinPulseLength;
    if (component.CurrentBehavior.HasValue)
    {
      AnomalyBehaviorPrototype behaviorPrototype = this._prototype.Index<AnomalyBehaviorPrototype>(component.CurrentBehavior.Value);
      pulseLength *= (double) behaviorPrototype.PulseFrequencyModifier;
    }
    return pulseLength;
  }

  private float GetSeverityIncreaseFromGrowth(AnomalyComponent component)
  {
    return (float) (1.0 + (double) Math.Max(component.Stability - component.GrowthThreshold, 0.0f) * 10.0) * component.SeverityGrowthCoefficient;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<AnomalyComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<AnomalyComponent>();
    EntityUid uid1;
    AnomalyComponent component1;
    while (entityQueryEnumerator1.MoveNext(ref uid1, ref component1))
    {
      if ((double) component1.Stability < (double) component1.DecayThreshold)
        this.ChangeAnomalyHealth(uid1, component1.HealthChangePerSecond * frameTime, component1);
      if (this.Timing.CurTime > component1.NextPulseTime)
        this.DoAnomalyPulse(uid1, component1);
    }
    EntityQueryEnumerator<AnomalyPulsingComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<AnomalyPulsingComponent>();
    EntityUid entityUid;
    AnomalyPulsingComponent pulsingComponent;
    while (entityQueryEnumerator2.MoveNext(ref entityUid, ref pulsingComponent))
    {
      if (this.Timing.CurTime > pulsingComponent.EndTime)
      {
        this.Appearance.SetData(entityUid, (Enum) AnomalyVisuals.IsPulsing, (object) false, (AppearanceComponent) null);
        this.RemComp(entityUid, (IComponent) pulsingComponent);
      }
    }
    EntityQueryEnumerator<AnomalySupercriticalComponent, AnomalyComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<AnomalySupercriticalComponent, AnomalyComponent>();
    EntityUid uid2;
    AnomalySupercriticalComponent supercriticalComponent;
    AnomalyComponent component2;
    while (entityQueryEnumerator3.MoveNext(ref uid2, ref supercriticalComponent, ref component2))
    {
      if (!(this.Timing.CurTime <= supercriticalComponent.EndTime))
        this.DoAnomalySupercriticalEvent(uid2, component2);
    }
  }

  public List<TileRef>? GetSpawningPoints(
    EntityUid uid,
    float stability,
    float severity,
    AnomalySpawnSettings settings,
    float powerModifier = 1f)
  {
    TransformComponent transformComponent = this.Transform(uid);
    MapGridComponent mapGridComponent;
    if (!this.TryComp<MapGridComponent>(transformComponent.GridUid, ref mapGridComponent))
      return (List<TileRef>) null;
    int num1 = (int) ((double) MathHelper.Lerp((float) settings.MinAmount, (float) settings.MaxAmount, severity * stability * powerModifier) + 0.5);
    Vector2 worldPosition = this._transform.GetWorldPosition(uid);
    List<TileRef> list = this._map.GetTilesIntersecting(transformComponent.GridUid.Value, mapGridComponent, new Box2(worldPosition + new Vector2(-settings.MaxRange), worldPosition + new Vector2(settings.MaxRange)), true, (Predicate<TileRef>) null).ToList<TileRef>();
    if (list.Count == 0)
      return (List<TileRef>) null;
    EntityQuery<PhysicsComponent> entityQuery = this.GetEntityQuery<PhysicsComponent>();
    List<TileRef> spawningPoints = new List<TileRef>();
    while (spawningPoints.Count < num1 && list.Count != 0)
    {
      TileRef tileRef = RandomExtensions.Pick<TileRef>(this.Random, (IReadOnlyList<TileRef>) list);
      float num2 = Vector2.Distance(this._map.GridTileToWorldPos(transformComponent.GridUid.Value, mapGridComponent, tileRef.GridIndices), worldPosition);
      if ((double) num2 > (double) settings.MaxRange || (double) num2 < (double) settings.MinRange)
      {
        list.Remove(tileRef);
      }
      else
      {
        if (!settings.CanSpawnOnEntities)
        {
          bool flag = true;
          foreach (EntityUid anchoredEntity in this._map.GetAnchoredEntities(transformComponent.GridUid.Value, mapGridComponent, tileRef.GridIndices))
          {
            PhysicsComponent physicsComponent;
            if (entityQuery.TryGetComponent(anchoredEntity, ref physicsComponent) && physicsComponent.BodyType == 4 && physicsComponent.Hard && (physicsComponent.CollisionLayer & 2) != 0)
            {
              flag = false;
              break;
            }
          }
          if (!flag)
          {
            list.Remove(tileRef);
            continue;
          }
        }
        spawningPoints.Add(tileRef);
      }
    }
    return spawningPoints;
  }
}
