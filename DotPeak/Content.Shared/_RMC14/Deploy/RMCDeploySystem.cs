// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Deploy.RMCDeploySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared._RMC14.Xenonids.Spray;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Foldable;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Deploy;

public sealed class RMCDeploySystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private IEntityManager _entMan;
  [Dependency]
  private SharedTransformSystem _xform;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private FoldableSystem _foldable;
  [Dependency]
  private SharedBuckleSystem _buckle;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedDestructibleSystem _destructible;
  [Dependency]
  private SharedEntityStorageSystem _entityStorage;
  [Dependency]
  private SharedAudioSystem _audio;
  private List<EntityUid> _toDelete = new List<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCDeployableComponent, UseInHandEvent>(new EntityEventRefHandler<RMCDeployableComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<RMCDeployableComponent, RMCDeployDoAfterEvent>(new EntityEventRefHandler<RMCDeployableComponent, RMCDeployDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<RMCDeployableComponent, DoAfterAttemptEvent<RMCDeployDoAfterEvent>>(new EntityEventRefHandler<RMCDeployableComponent, DoAfterAttemptEvent<RMCDeployDoAfterEvent>>(this.OnDeployDoAfterAttempt));
    this.SubscribeLocalEvent<RMCDeployedEntityComponent, ComponentShutdown>(new EntityEventRefHandler<RMCDeployedEntityComponent, ComponentShutdown>(this.OnDeployedEntityShutdown));
    this.SubscribeLocalEvent<RMCDeployableComponent, ComponentShutdown>(new EntityEventRefHandler<RMCDeployableComponent, ComponentShutdown>(this.OnDeployableShutdown));
    this.SubscribeLocalEvent<RMCDeployedEntityComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCDeployedEntityComponent, InteractUsingEvent>(this.OnParentalCollapseInteractUsing));
    this.SubscribeLocalEvent<RMCDeployedEntityComponent, RMCParentalCollapseDoAfterEvent>(new EntityEventRefHandler<RMCDeployedEntityComponent, RMCParentalCollapseDoAfterEvent>(this.OnParentalCollapseDoAfter));
    this.SubscribeLocalEvent<RMCDeployableComponent, ExaminedEvent>(new EntityEventRefHandler<RMCDeployableComponent, ExaminedEvent>(this.OnDeployableExamined));
    this.SubscribeLocalEvent<RMCDeployedEntityComponent, ExaminedEvent>(new EntityEventRefHandler<RMCDeployedEntityComponent, ExaminedEvent>(this.OnDeployedExamined));
  }

  private void OnUseInHand(Entity<RMCDeployableComponent> ent, ref UseInHandEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.TryStartDeploy(ent, args.User);
  }

  private void TryStartDeploy(Entity<RMCDeployableComponent> ent, EntityUid user)
  {
    EntityUid owner = ent.Owner;
    RMCDeployableComponent comp1 = ent.Comp;
    if (this.HasAnyAcid(owner))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-deploy-popup-acid", ("entity", (object) ent)), owner, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      EntityUid? grid = this._xform.GetGrid((Entity<TransformComponent>) user);
      MapGridComponent comp2;
      if (!this.TryComp<MapGridComponent>(grid, out comp2) || comp1.FailIfNotSurface && !this.CheckSurface(grid.Value, owner, new EntityUid?(user)))
        return;
      Vector2 worldPosition = this._xform.GetWorldPosition(user);
      Vector2i tile = this._map.WorldToTile(grid.Value, comp2, worldPosition);
      Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(this._map.TileCenterToVector(grid.Value, comp2, tile), 0.0f);
      Box2 aabb = comp1.DeployArea.ComputeAABB(transform, 0);
      if (comp1.AreaBlockedCheck && this.IsAreaBlocked(aabb, owner, new EntityUid?(user), new Entity<RMCDeployableComponent>?(ent)))
        return;
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs(this._entMan, user, comp1.DeployTime, (DoAfterEvent) new RMCDeployDoAfterEvent(aabb), new EntityUid?(owner))
      {
        BreakOnMove = true,
        BreakOnDamage = true,
        BreakOnHandChange = true,
        NeedHand = true,
        AttemptFrequency = AttemptFrequency.EveryTick
      }))
        return;
      this._popup.PopupClient(this.Loc.GetString("rmc-deploy-popup-start"), ent.Owner, new EntityUid?(user));
      if (!this._netManager.IsServer)
        return;
      ent.Comp.CurrentDeployUser = new EntityUid?(user);
      this.Dirty<RMCDeployableComponent>(ent);
      this.RaiseNetworkEvent((EntityEventArgs) new RMCShowDeployAreaEvent(aabb, Color.Blue), user);
    }
  }

  private void OnDoAfter(Entity<RMCDeployableComponent> ent, ref RMCDeployDoAfterEvent ev)
  {
    if (this._netManager.IsClient)
      return;
    if (ev.Cancelled || ev.Handled)
    {
      ent.Comp.CurrentDeployUser = new EntityUid?();
      this.Dirty<RMCDeployableComponent>(ent);
      this.RaiseNetworkEvent((EntityEventArgs) new RMCHideDeployAreaEvent(), ev.Args.User);
    }
    else
    {
      ev.Handled = true;
      EntityUid user = ev.Args.User;
      EntityUid? grid = this._xform.GetGrid((Entity<TransformComponent>) user);
      MapGridComponent comp;
      if (!this.TryComp<MapGridComponent>(grid, out comp))
        return;
      Vector2 worldPosition = this._xform.GetWorldPosition(user);
      Vector2i tile = this._map.WorldToTile(grid.Value, comp, worldPosition);
      Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(this._map.TileCenterToVector(grid.Value, comp, tile), 0.0f);
      Box2 aabb = ent.Comp.DeployArea.ComputeAABB(transform, 0);
      Vector2 center = ((Box2) ref aabb).Center;
      this.DeploySetups(ent, center, user);
      if (ent.Comp.DeploySound != null)
        this._audio.PlayPvs(ent.Comp.DeploySound, user);
      ent.Comp.CurrentDeployUser = new EntityUid?();
      this.Dirty<RMCDeployableComponent>(ent);
      this.RaiseNetworkEvent((EntityEventArgs) new RMCHideDeployAreaEvent(), ev.Args.User);
    }
  }

  private void DeploySetups(Entity<RMCDeployableComponent> ent, Vector2 areaCenter, EntityUid user)
  {
    if (this._netManager.IsClient)
      return;
    this.EnsureComp<ContainerManagerComponent>(ent.Owner);
    BaseContainer container;
    if (this._container.TryGetContainer(ent.Owner, "storage", out container) && container.ContainedEntities.Count > 0)
      this.RedeployExistingEntities(ent, areaCenter, container);
    else
      this.DeployAllSetups(ent, areaCenter, user);
  }

  private void RedeployExistingEntities(
    Entity<RMCDeployableComponent> ent,
    Vector2 areaCenter,
    BaseContainer originalStorage)
  {
    if (this._netManager.IsClient)
      return;
    EntityUid? nullable = new EntityUid?();
    foreach (EntityUid entityUid in originalStorage.ContainedEntities.ToList<EntityUid>())
    {
      RMCDeployedEntityComponent comp1;
      if (this.TryComp<RMCDeployedEntityComponent>(entityUid, out comp1))
      {
        int setupIndex = comp1.SetupIndex;
        if (setupIndex >= 0 && setupIndex < ent.Comp.DeploySetups.Count)
        {
          RMCDeploySetup deploySetup = ent.Comp.DeploySetups[setupIndex];
          if (!deploySetup.NeverRedeployableSetup)
          {
            this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entityUid, originalStorage);
            FoldableComponent comp2;
            if (this.TryComp<FoldableComponent>(entityUid, out comp2))
              this._foldable.SetFolded(entityUid, comp2, false);
            Vector2 worldPos = areaCenter + deploySetup.Offset;
            this._xform.SetWorldPosition(entityUid, worldPos);
            this._xform.SetWorldRotation(entityUid, Angle.FromDegrees((double) deploySetup.Angle));
            if (deploySetup.Anchor)
            {
              TransformComponent transformComponent = this.Transform(entityUid);
              if (!transformComponent.Anchored)
                this._xform.AnchorEntity((Entity<TransformComponent>) (entityUid, transformComponent));
            }
            if (deploySetup.StorageOriginalEntity && !nullable.HasValue)
              nullable = new EntityUid?(entityUid);
          }
        }
      }
    }
    if (!nullable.HasValue)
      return;
    Container container = this._container.EnsureContainer<Container>(nullable.Value, "storage");
    if (this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) ent.Owner, (BaseContainer) container))
      return;
    this.Log.Error($"Failed to place original entity {ent.Owner} in container 'storage' of entity {nullable.Value}");
  }

  private void DeployAllSetups(
    Entity<RMCDeployableComponent> ent,
    Vector2 areaCenter,
    EntityUid user)
  {
    if (this._netManager.IsClient)
      return;
    EntityUid? nullable = new EntityUid?();
    foreach ((RMCDeploySetup rmcDeploySetup, int num) in ent.Comp.DeploySetups.Select<RMCDeploySetup, (RMCDeploySetup, int)>((Func<RMCDeploySetup, int, (RMCDeploySetup, int)>) ((s, idx) => (s, idx))))
    {
      Vector2 vector2 = areaCenter + rmcDeploySetup.Offset;
      this.Log.Debug($"RMCDeploySystem: Spawning entity {rmcDeploySetup.Prototype} at position {vector2}");
      EntityUid uid = this.Spawn((string) rmcDeploySetup.Prototype, new MapCoordinates(vector2, this._xform.GetMapId((Entity<TransformComponent>) user)), rotation: new Angle());
      this._xform.SetWorldPosition(uid, vector2);
      this._xform.SetWorldRotation(uid, Angle.FromDegrees((double) rmcDeploySetup.Angle));
      if (rmcDeploySetup.Anchor)
      {
        TransformComponent transformComponent = this.Transform(uid);
        if (!transformComponent.Anchored)
          this._xform.AnchorEntity((Entity<TransformComponent>) (uid, transformComponent));
      }
      RMCDeployedEntityComponent deployedEntityComponent = this.EnsureComp<RMCDeployedEntityComponent>(uid);
      deployedEntityComponent.OriginalEntity = ent.Owner;
      deployedEntityComponent.SetupIndex = num;
      this.Dirty(uid, (IComponent) deployedEntityComponent);
      if (rmcDeploySetup.StorageOriginalEntity && !nullable.HasValue)
        nullable = new EntityUid?(uid);
    }
    if (nullable.HasValue)
    {
      Container container = this._container.EnsureContainer<Container>(nullable.Value, "storage");
      if (this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) ent.Owner, (BaseContainer) container))
        return;
      this.Log.Error($"RMCDeploySystem: Failed to place original entity {ent.Owner} in container 'storage' of entity {nullable.Value}");
    }
    else
      this.Log.Error("RMCDeploySystem: Original entity with StorageOriginalEntity not found for placement");
  }

  private void OnDeployDoAfterAttempt(
    Entity<RMCDeployableComponent> ent,
    ref DoAfterAttemptEvent<RMCDeployDoAfterEvent> args)
  {
    RMCDeployableComponent comp = ent.Comp;
    EntityUid owner = ent.Owner;
    EntityUid user = args.Event.User;
    Box2 area = args.Event.Area;
    if (this.HasAnyAcid(owner))
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-deploy-popup-acid", ("entity", (object) ent)), user, user, PopupType.SmallCaution);
      args.Cancel();
    }
    else
    {
      if (!comp.AreaBlockedCheck || !this.IsAreaBlocked(area, owner, new EntityUid?(user), new Entity<RMCDeployableComponent>?(ent)))
        return;
      this._popup.PopupEntity(this.Loc.GetString("rmc-deploy-popup-blocked"), user, user, PopupType.SmallCaution);
      args.Cancel();
    }
  }

  private bool IsAreaBlocked(
    Box2 area,
    EntityUid ignore,
    EntityUid? user = null,
    Entity<RMCDeployableComponent>? ent = null)
  {
    RMCDeployableComponent comp1 = ent?.Comp;
    MapId mapId = this._xform.GetMapId((Entity<TransformComponent>) ignore);
    bool flag1 = false;
    bool flag2 = comp1 == null || comp1.FailIfNotSurface;
    EntityUid? grid = this._xform.GetGrid((Entity<TransformComponent>) ignore);
    if (!this.HasComp<MapGridComponent>(grid))
      return false;
    if (flag2 && grid.HasValue && !this.CheckSurface(grid.Value, ignore, user))
      return true;
    foreach (EntityUid uid in this._lookup.GetEntitiesIntersecting(mapId, area))
    {
      if (!(uid == ignore))
      {
        if (user.HasValue)
        {
          EntityUid entityUid = uid;
          EntityUid? nullable = user;
          if ((nullable.HasValue ? (entityUid == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            continue;
        }
        PhysicsComponent comp2;
        if (!this.IsGhostRelated(uid) && this.TryComp<PhysicsComponent>(uid, out comp2) && (comp2.CanCollide || comp2.Hard))
        {
          string entityName = this.MetaData(uid).EntityName;
          flag1 = true;
          break;
        }
      }
    }
    if (flag1 && user.HasValue && this._netManager.IsClient)
      this._popup.PopupClient(this.Loc.GetString("rmc-deploy-popup-blocked"), user.Value, new EntityUid?(user.Value), PopupType.SmallCaution);
    return flag1;
  }

  private bool IsGhostRelated(EntityUid uid)
  {
    EntityUid entityUid = uid;
    HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>();
    while (entityUidSet.Add(entityUid))
    {
      if (this.HasComp<GhostComponent>(entityUid))
        return true;
      BaseContainer container;
      if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) entityUid, out container))
      {
        entityUid = container.Owner;
      }
      else
      {
        TransformComponent comp;
        if (this.TryComp(entityUid, out comp) && comp.ParentUid.IsValid())
          entityUid = comp.ParentUid;
        else
          break;
      }
    }
    return false;
  }

  private bool CheckSurface(EntityUid gridUid, EntityUid ignore, EntityUid? user)
  {
    if (this.HasComp<RMCPlanetComponent>(gridUid))
      return true;
    if (user.HasValue && this._netManager.IsClient)
      this._popup.PopupClient(this.Loc.GetString("rmc-deploy-popup-surface"), ignore, new EntityUid?(user.Value), PopupType.SmallCaution);
    return false;
  }

  private void OnDeployedEntityShutdown(
    Entity<RMCDeployedEntityComponent> ent,
    ref ComponentShutdown args)
  {
    if (this._netManager.IsClient || ent.Comp.InShutdown)
      return;
    ent.Comp.InShutdown = true;
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
    RMCDeployableComponent comp;
    if (!this.Exists(ent.Comp.OriginalEntity) || !this.TryComp<RMCDeployableComponent>(ent.Comp.OriginalEntity, out comp) || comp == null || comp.DeploySetups[ent.Comp.SetupIndex].Mode != RMCDeploySetupMode.ReactiveParental)
      return;
    List<EntityUid> entityUidList = new List<EntityUid>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCDeployedEntityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCDeployedEntityComponent>();
    EntityUid uid;
    RMCDeployedEntityComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(comp1.OriginalEntity != ent.Comp.OriginalEntity) && comp1.SetupIndex != ent.Comp.SetupIndex)
      {
        switch (comp.DeploySetups[comp1.SetupIndex].Mode)
        {
          case RMCDeploySetupMode.Reactive:
          case RMCDeploySetupMode.ReactiveParental:
            entityUidList.Add(uid);
            continue;
          default:
            continue;
        }
      }
    }
    foreach (EntityUid owner in entityUidList)
      this._destructible.DestroyEntity(owner);
  }

  private void OnDeployableShutdown(Entity<RMCDeployableComponent> ent, ref ComponentShutdown args)
  {
    if (this._netManager.IsClient)
      return;
    if (ent.Comp.CurrentDeployUser.HasValue)
      this.RaiseNetworkEvent((EntityEventArgs) new RMCHideDeployAreaEvent(), ent.Comp.CurrentDeployUser.Value);
    this._toDelete.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCDeployedEntityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCDeployedEntityComponent>();
    EntityUid uid;
    RMCDeployedEntityComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(comp1.OriginalEntity != ent.Owner) && comp1.SetupIndex >= 0 && comp1.SetupIndex < ent.Comp.DeploySetups.Count)
      {
        RMCDeploySetup deploySetup = ent.Comp.DeploySetups[comp1.SetupIndex];
        if (deploySetup.StorageOriginalEntity)
        {
          comp1.InShutdown = true;
          this.Dirty(uid, (IComponent) comp1);
        }
        else if ((deploySetup.Mode == RMCDeploySetupMode.ReactiveParental || deploySetup.Mode == RMCDeploySetupMode.Reactive) && !comp1.InShutdown)
        {
          comp1.InShutdown = true;
          this.Dirty(uid, (IComponent) comp1);
          this._toDelete.Add(uid);
        }
      }
    }
    foreach (EntityUid owner in this._toDelete)
      this._destructible.DestroyEntity(owner);
  }

  private void OnParentalCollapseInteractUsing(
    Entity<RMCDeployedEntityComponent> ent,
    ref InteractUsingEvent args)
  {
    RMCDeployableComponent comp1;
    MetaDataComponent comp2;
    ItemToggleComponent comp3;
    if (args.Handled || !this.TryComp<RMCDeployableComponent>(ent.Comp.OriginalEntity, out comp1) || comp1.DeploySetups[ent.Comp.SetupIndex].Mode != RMCDeploySetupMode.ReactiveParental || !comp1.CollapseToolPrototype.HasValue || !this.TryComp(args.Used, out comp2) || comp2.EntityPrototype == null || (EntProtoId) comp2.EntityPrototype.ID != comp1.CollapseToolPrototype.Value || this.TryComp<ItemToggleComponent>(args.Used, out comp3) && !comp3.Activated)
      return;
    args.Handled = true;
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs(this._entMan, args.User, TimeSpan.FromSeconds((double) comp1.CollapseTime), (DoAfterEvent) new RMCParentalCollapseDoAfterEvent(), new EntityUid?(ent.Owner))
    {
      BreakOnMove = true,
      BreakOnDamage = true,
      BreakOnHandChange = true,
      NeedHand = true
    }))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-deployable-collapse-start"), args.User, new EntityUid?(args.User));
  }

  private void OnParentalCollapseDoAfter(
    Entity<RMCDeployedEntityComponent> ent,
    ref RMCParentalCollapseDoAfterEvent ev)
  {
    if (this._netManager.IsClient || ev.Cancelled || ev.Handled)
      return;
    ev.Handled = true;
    RMCDeployedEntityComponent comp1 = ent.Comp;
    EntityUid user = ev.Args.User;
    RMCDeployableComponent comp2;
    if (!this.TryComp<RMCDeployableComponent>(comp1.OriginalEntity, out comp2))
      return;
    EntityUid? nullable = new EntityUid?();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCDeployedEntityComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RMCDeployedEntityComponent>();
    EntityUid uid1;
    RMCDeployedEntityComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      RMCDeployableComponent comp3;
      BaseContainer container;
      if (!(comp1_1.OriginalEntity != comp1.OriginalEntity) && this.TryComp<RMCDeployableComponent>(comp1.OriginalEntity, out comp3) && comp3.DeploySetups[comp1_1.SetupIndex].Mode == RMCDeploySetupMode.ReactiveParental && this._container.TryGetContainer(uid1, "storage", out container) && container.Contains(comp1.OriginalEntity))
      {
        nullable = new EntityUid?(uid1);
        break;
      }
    }
    if (!nullable.HasValue)
      return;
    BaseContainer container1;
    if (this._container.TryGetContainer(nullable.Value, "storage", out container1) && container1.Contains(comp1.OriginalEntity))
    {
      this._container.Remove((Entity<TransformComponent, MetaDataComponent>) comp1.OriginalEntity, container1);
      Vector2 worldPosition = this._xform.GetWorldPosition(user);
      this._xform.SetWorldPosition(comp1.OriginalEntity, worldPosition);
    }
    Container container2 = this._container.EnsureContainer<Container>(comp1.OriginalEntity, "storage");
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCDeployedEntityComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCDeployedEntityComponent>();
    EntityUid uid2;
    RMCDeployedEntityComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!(comp1_2.OriginalEntity != comp1.OriginalEntity) && comp1_2.SetupIndex >= 0 && comp1_2.SetupIndex < comp2.DeploySetups.Count && !comp2.DeploySetups[comp1_2.SetupIndex].NeverRedeployableSetup && !(uid2 == comp1.OriginalEntity))
      {
        this._entityStorage.EmptyContents(uid2);
        this.TryUnbuckleAll(uid2);
        FoldableComponent comp4;
        if (this.TryComp<FoldableComponent>(uid2, out comp4))
          this._foldable.SetFolded(uid2, comp4, true);
        this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) uid2, (BaseContainer) container2);
      }
    }
    if (comp2.CollapseSound == null)
      return;
    this._audio.PlayPvs(comp2.CollapseSound, user);
  }

  private void TryUnbuckleAll(EntityUid entity)
  {
    StrapComponent comp;
    if (!this.TryComp<StrapComponent>(entity, out comp) || comp.BuckledEntities.Count == 0)
      return;
    foreach (EntityUid uid in comp.BuckledEntities.ToArray<EntityUid>())
      this._buckle.Unbuckle((Entity<BuckleComponent>) (uid, this.CompOrNull<BuckleComponent>(uid)), new EntityUid?());
  }

  private bool HasAnyAcid(EntityUid uid)
  {
    return this.HasComp<TimedCorrodingComponent>(uid) || this.HasComp<DamageableCorrodingComponent>(uid) || this.HasComp<SprayAcidedComponent>(uid);
  }

  private void OnDeployableExamined(Entity<RMCDeployableComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("rmc-deployable-examine-hint"));
  }

  private void OnDeployedExamined(Entity<RMCDeployedEntityComponent> ent, ref ExaminedEvent args)
  {
    RMCDeployableComponent comp;
    if (!this.TryComp<RMCDeployableComponent>(ent.Comp.OriginalEntity, out comp) || ent.Comp.SetupIndex < 0 || ent.Comp.SetupIndex >= comp.DeploySetups.Count || comp.DeploySetups[ent.Comp.SetupIndex].Mode != RMCDeploySetupMode.ReactiveParental)
      return;
    EntProtoId? collapseToolPrototype = comp.CollapseToolPrototype;
    EntityPrototype prototype;
    if (!collapseToolPrototype.HasValue || !this._prototypeManager.TryIndex<EntityPrototype>((string) collapseToolPrototype.GetValueOrDefault(), out prototype))
      return;
    string name = prototype.Name;
    args.PushMarkup(this.Loc.GetString("rmc-deployed-collapse-hint", ("tool", (object) name)));
  }
}
