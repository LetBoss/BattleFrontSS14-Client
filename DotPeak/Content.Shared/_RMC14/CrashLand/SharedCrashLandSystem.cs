// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.CrashLand.SharedCrashLandSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Rules;
using Content.Shared.ActionBlocker;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Maps;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.ParaDrop;
using Content.Shared.Physics;
using Content.Shared.Shuttles.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.CrashLand;

public abstract class SharedCrashLandSystem : EntitySystem
{
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  protected ActionBlockerSystem Blocker;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  protected DamageableSystem Damageable;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private TurfSystem _turf;
  protected static readonly ProtoId<DamageTypePrototype> CrashLandDamageType = (ProtoId<DamageTypePrototype>) "Blunt";
  protected const int CrashLandDamageAmount = 10000;
  private bool _crashLandEnabled;
  private Robust.Shared.GameObjects.EntityQuery<CrashLandableComponent> _crashLandableQuery;

  public override void Initialize()
  {
    this._crashLandableQuery = this.GetEntityQuery<CrashLandableComponent>();
    this.SubscribeLocalEvent<CrashLandableComponent, EntParentChangedMessage>(new EntityEventRefHandler<CrashLandableComponent, EntParentChangedMessage>(this.OnCrashLandableParentChanged));
    this.SubscribeLocalEvent<CrashLandOnTouchComponent, StartCollideEvent>(new EntityEventRefHandler<CrashLandOnTouchComponent, StartCollideEvent>(this.OnCrashLandOnTouchStartCollide));
    this.SubscribeLocalEvent<DeleteCrashLandableOnTouchComponent, StartCollideEvent>(new EntityEventRefHandler<DeleteCrashLandableOnTouchComponent, StartCollideEvent>(this.OnDeleteCrashLandableOnTouchStartCollide));
    this.SubscribeLocalEvent<CrashLandingComponent, UpdateCanMoveEvent>(new EntityEventRefHandler<CrashLandingComponent, UpdateCanMoveEvent>(this.OnUpdateCanMove));
    this.Subs.CVar<bool>(this._config, RMCCVars.RMCFTLCrashLand, (Action<bool>) (v => this._crashLandEnabled = v), true);
  }

  private void OnCrashLandableParentChanged(
    Entity<CrashLandableComponent> crashLandable,
    ref EntParentChangedMessage args)
  {
    if (!this._crashLandEnabled || !this.HasComp<FTLMapComponent>(args.Transform.ParentUid))
      return;
    EntityUid? oldParent1 = args.OldParent;
    if (!oldParent1.HasValue)
      return;
    PullerComponent comp;
    CrashLandableComponent component;
    if (this.TryComp<PullerComponent>((EntityUid) crashLandable, out comp) && comp.Pulling.HasValue && this._crashLandableQuery.TryComp(comp.Pulling.Value, out component))
    {
      EntityUid crashing = comp.Pulling.Value;
      oldParent1 = args.OldParent;
      EntityUid oldParent2 = oldParent1.Value;
      if (this.ShouldCrash(crashing, oldParent2))
        this.TryCrashLand((Entity<CrashLandableComponent>) (comp.Pulling.Value, component), true);
    }
    EntityUid crashing1 = (EntityUid) crashLandable;
    oldParent1 = args.OldParent;
    EntityUid oldParent3 = oldParent1.Value;
    if (!this.ShouldCrash(crashing1, oldParent3))
      return;
    this.TryCrashLand((Entity<CrashLandableComponent>) crashLandable.Owner, true);
  }

  private void OnCrashLandOnTouchStartCollide(
    Entity<CrashLandOnTouchComponent> ent,
    ref StartCollideEvent args)
  {
    CrashLandableComponent component;
    if (!this._crashLandEnabled || !this._crashLandableQuery.TryGetComponent(args.OtherEntity, out component))
      return;
    AttemptCrashLandEvent args1 = new AttemptCrashLandEvent(args.OtherEntity);
    this.RaiseLocalEvent<AttemptCrashLandEvent>((EntityUid) ent, ref args1);
    if (args1.Cancelled)
      return;
    this.TryCrashLand((Entity<CrashLandableComponent>) (args.OtherEntity, component), true);
  }

  private void OnDeleteCrashLandableOnTouchStartCollide(
    Entity<DeleteCrashLandableOnTouchComponent> ent,
    ref StartCollideEvent args)
  {
    if (this._net.IsClient || !this._crashLandEnabled || !this._crashLandableQuery.HasComp(args.OtherEntity))
      return;
    this.QueueDel(new EntityUid?(args.OtherEntity));
  }

  private void OnUpdateCanMove(Entity<CrashLandingComponent> ent, ref UpdateCanMoveEvent args)
  {
    args.Cancel();
  }

  private bool ShouldCrash(EntityUid crashing, EntityUid oldParent)
  {
    AttemptCrashLandEvent args = new AttemptCrashLandEvent(crashing);
    this.RaiseLocalEvent<AttemptCrashLandEvent>(oldParent, ref args);
    return !args.Cancelled;
  }

  public void ApplyFallingDamage(EntityUid uid)
  {
    DamageSpecifier damage = new DamageSpecifier()
    {
      DamageDict = {
        [(string) SharedCrashLandSystem.CrashLandDamageType] = (FixedPoint2) 10000
      }
    };
    this.Damageable.TryChangeDamage(new EntityUid?(uid), damage);
  }

  public bool IsLandableTile(Entity<MapGridComponent> grid, TileRef tileRef)
  {
    Vector2i gridIndices = tileRef.GridIndices;
    EntityCoordinates local = this._mapSystem.GridTileToLocal((EntityUid) grid, (MapGridComponent) grid, gridIndices);
    if (this._turf.GetContentTileDefinition(tileRef).ID == "Space" || this._turf.IsSpace(tileRef) || this._turf.IsTileBlocked(tileRef, CollisionGroup.MobMask) || !this._area.CanParadrop(local))
      return false;
    Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> entityQuery = this.GetEntityQuery<PhysicsComponent>();
    bool flag = true;
    AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator((EntityUid) grid, grid.Comp, gridIndices);
    EntityUid? uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      PhysicsComponent component;
      if (entityQuery.TryGetComponent(uid, out component) && component.BodyType == BodyType.Static && component.Hard && (component.CollisionLayer & 2) != 0)
      {
        flag = false;
        break;
      }
    }
    return flag;
  }

  public bool TryGetCrashLandLocation(out EntityCoordinates location)
  {
    location = new EntityCoordinates();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCPlanetComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCPlanetComponent>();
    EntityUid uid;
    MapGridComponent comp;
    while (entityQueryEnumerator.MoveNext(out uid, out RMCPlanetComponent _) && this.TryComp<MapGridComponent>(uid, out comp))
    {
      TransformComponent transformComponent = this.Transform(uid);
      location = transformComponent.Coordinates;
      for (int index = 0; index < 250; ++index)
      {
        int num1 = this._random.Next(-200, 200);
        int num2 = this._random.Next(-200, 200);
        Vector2i vector2i;
        // ISSUE: explicit constructor call
        ((Vector2i) ref vector2i).\u002Ector(num1, num2);
        TileRef tile;
        if (this._mapSystem.TryGetTileRef(uid, comp, vector2i, out tile) && this.IsLandableTile((Entity<MapGridComponent>) (uid, comp), tile))
        {
          location = this._mapSystem.GridTileToLocal(uid, comp, vector2i);
          return true;
        }
      }
    }
    return false;
  }

  public void TryCrashLand(Entity<CrashLandableComponent?> crashLandable, bool doDamage)
  {
    EntityCoordinates location;
    if (this._net.IsClient || !this.TryGetCrashLandLocation(out location))
      return;
    this.TryCrashLand((Entity<CrashLandableComponent>) crashLandable.Owner, doDamage, location);
  }

  public void TryCrashLand(
    Entity<CrashLandableComponent?> crashLandable,
    bool doDamage,
    EntityCoordinates location)
  {
    if (this._net.IsClient || !this.Resolve<CrashLandableComponent>((EntityUid) crashLandable, ref crashLandable.Comp, false) || this.HasComp<CrashLandingComponent>((EntityUid) crashLandable))
      return;
    SkyFallingComponent fallingComponent = this.EnsureComp<SkyFallingComponent>((EntityUid) crashLandable);
    fallingComponent.TargetCoordinates = new EntityCoordinates?(location);
    this.Dirty((EntityUid) crashLandable, (IComponent) fallingComponent);
    CrashLandingComponent landingComponent = this.EnsureComp<CrashLandingComponent>((EntityUid) crashLandable);
    landingComponent.DoDamage = doDamage;
    landingComponent.RemainingTime = crashLandable.Comp.CrashDuration;
    this.Dirty((EntityUid) crashLandable, (IComponent) landingComponent);
    this.Blocker.UpdateCanMove((EntityUid) crashLandable);
    crashLandable.Comp.LastCrash = new TimeSpan?(this._timing.CurTime);
    this.Dirty<CrashLandableComponent>(crashLandable);
    this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) crashLandable);
    CrashLandStartedEvent args = new CrashLandStartedEvent();
    this.RaiseLocalEvent<CrashLandStartedEvent>((EntityUid) crashLandable, ref args);
  }

  public override void Update(float frameTime)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<CrashLandableComponent, CrashLandingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CrashLandableComponent, CrashLandingComponent>();
    EntityUid uid;
    CrashLandableComponent comp1;
    CrashLandingComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!this.HasComp<SkyFallingComponent>(uid))
      {
        comp2.RemainingTime -= frameTime;
        if ((double) comp2.RemainingTime <= 0.0)
        {
          if (comp2.DoDamage)
            this.ApplyFallingDamage(uid);
          CrashLandedEvent args = new CrashLandedEvent(comp2.DoDamage);
          this.RaiseLocalEvent<CrashLandedEvent>(uid, ref args);
          if (this._net.IsServer)
            this._audio.PlayPvs(comp1.CrashSound, uid);
          this.RemComp<CrashLandingComponent>(uid);
          this.Blocker.UpdateCanMove(uid);
        }
      }
    }
  }
}
