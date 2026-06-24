// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hive.SharedXenoHiveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Hive;

public abstract class SharedXenoHiveSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedNightVisionSystem _nightVision;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedXenoAnnounceSystem _xenoAnnounce;
  private Robust.Shared.GameObjects.EntityQuery<HiveComponent> _query;
  private Robust.Shared.GameObjects.EntityQuery<HiveMemberComponent> _memberQuery;

  public override void Initialize()
  {
    this._query = this.GetEntityQuery<HiveComponent>();
    this._memberQuery = this.GetEntityQuery<HiveMemberComponent>();
    this.SubscribeLocalEvent<DropshipHijackStartEvent>(new EntityEventRefHandler<DropshipHijackStartEvent>(this.OnDropshipHijackStart));
    this.SubscribeLocalEvent<HiveComponent, MapInitEvent>(new EntityEventRefHandler<HiveComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<XenoEvolutionGranterComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoEvolutionGranterComponent, MobStateChangedEvent>(this.OnGranterMobStateChanged));
    this.SubscribeLocalEvent<AutoAssignHiveComponent, ComponentStartup>(new EntityEventRefHandler<AutoAssignHiveComponent, ComponentStartup>(this.OnAutoAssignHiveAdded));
    this.SubscribeLocalEvent<HiveGunComponent, AmmoShotEvent>(new EntityEventRefHandler<HiveGunComponent, AmmoShotEvent>(this.OnHiveGunShot));
  }

  private void OnDropshipHijackStart(ref DropshipHijackStartEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveComponent>();
    EntityUid uid1;
    HiveComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1))
    {
      if (!comp1.HijackSurged)
      {
        comp1.HijackSurged = true;
        this.Dirty(uid1, (IComponent) comp1);
        EntityUid uid2 = this.Spawn((string) null, MapCoordinates.Nullspace, rotation: new Angle());
        EvolutionOverrideComponent overrideComponent = this.EnsureComp<EvolutionOverrideComponent>(uid2);
        overrideComponent.Amount = (FixedPoint2) 10;
        this.Dirty(uid2, (IComponent) overrideComponent);
        this.EnsureComp<TimedDespawnComponent>(uid2).Lifetime = 180f;
        break;
      }
    }
  }

  private void OnGranterMobStateChanged(
    Entity<XenoEvolutionGranterComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead)
      return;
    Entity<HiveComponent>? hive = this.GetHive((Entity<HiveMemberComponent>) ent.Owner);
    if (!hive.HasValue)
      return;
    Entity<HiveComponent> valueOrDefault = hive.GetValueOrDefault();
    valueOrDefault.Comp.LastQueenDeath = new TimeSpan?(this._timing.CurTime);
    valueOrDefault.Comp.CurrentQueen = new EntityUid?();
    valueOrDefault.Comp.AnnouncedQueenDeathCooldownOver = false;
    valueOrDefault.Comp.NewQueenAt = new TimeSpan?(this._timing.CurTime + valueOrDefault.Comp.NewQueenCooldown);
    this.Dirty<HiveComponent>(valueOrDefault);
  }

  private void OnMapInit(Entity<HiveComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.AnnouncedUnlocks.Clear();
    ent.Comp.Unlocks.Clear();
    ent.Comp.AnnouncementsLeft.Clear();
    foreach (Robust.Shared.Prototypes.EntityPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<Robust.Shared.Prototypes.EntityPrototype>())
    {
      XenoComponent component;
      if (enumeratePrototype.TryGetComponent<XenoComponent>(out component, this._compFactory) && !(component.UnlockAt == TimeSpan.Zero) && !enumeratePrototype.HasComponent<XenoHiddenComponent>(this._compFactory))
      {
        ent.Comp.Unlocks.GetOrNew<TimeSpan, List<EntProtoId>>(component.UnlockAt).Add((EntProtoId) enumeratePrototype.ID);
        if (!ent.Comp.AnnouncementsLeft.Contains(component.UnlockAt))
          ent.Comp.AnnouncementsLeft.Add(component.UnlockAt);
      }
    }
    foreach (KeyValuePair<TimeSpan, List<EntProtoId>> unlock in ent.Comp.Unlocks)
      unlock.Value.Sort();
    ent.Comp.AnnouncementsLeft.Sort();
  }

  public Entity<HiveComponent>? GetHive(Entity<HiveMemberComponent?> member)
  {
    if (!this._memberQuery.Resolve((EntityUid) member, ref member.Comp, false))
      return new Entity<HiveComponent>?();
    EntityUid? hive = member.Comp.Hive;
    if (hive.HasValue)
    {
      EntityUid valueOrDefault = hive.GetValueOrDefault();
      if (!this.TerminatingOrDeleted(valueOrDefault))
      {
        HiveComponent component;
        return !this._query.TryComp(valueOrDefault, out component) ? new Entity<HiveComponent>?() : new Entity<HiveComponent>?((Entity<HiveComponent>) (valueOrDefault, component));
      }
    }
    return new Entity<HiveComponent>?();
  }

  public Entity<HiveComponent>? GetHiveByName(string hiveName)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveComponent>();
    EntityUid uid;
    HiveComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (this.MetaData(uid).EntityName == hiveName)
        return new Entity<HiveComponent>?((Entity<HiveComponent>) (uid, comp1));
    }
    return new Entity<HiveComponent>?();
  }

  public bool HasHive(Entity<HiveMemberComponent?> member) => this.GetHive(member).HasValue;

  public void SetHive(Entity<HiveMemberComponent?> member, EntityUid? hive)
  {
    HiveMemberComponent hiveMemberComponent = member.Comp ?? this.EnsureComp<HiveMemberComponent>((EntityUid) member);
    EntityUid? hive1 = hiveMemberComponent.Hive;
    EntityUid? nullable1 = hive1;
    EntityUid? nullable2 = hive;
    if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    Entity<HiveComponent>? Hive = new Entity<HiveComponent>?();
    HiveComponent component;
    if (this._query.TryComp(hive, out component))
      Hive = new Entity<HiveComponent>?((Entity<HiveComponent>) (hive.Value, component));
    else if (hive.HasValue)
    {
      this.Log.Error($"Tried to set hive of {this.ToPrettyString(new EntityUid?((EntityUid) member))} to bad hive entity {this.ToPrettyString(hive)}");
      return;
    }
    hiveMemberComponent.Hive = hive;
    this.Dirty((EntityUid) member, (IComponent) hiveMemberComponent);
    if (this.HasComp<XenoEvolutionGranterComponent>((EntityUid) member) && Hive.HasValue)
      this.SetHiveQueen((EntityUid) member, Hive.Value);
    HiveChangedEvent args = new HiveChangedEvent(Hive, hive1);
    this.RaiseLocalEvent<HiveChangedEvent>((EntityUid) member, ref args);
  }

  public void SetSameHive(Entity<HiveMemberComponent?> src, Entity<HiveMemberComponent?> dest)
  {
    Entity<HiveComponent>? hive = this.GetHive(src);
    if (!hive.HasValue)
      return;
    Entity<HiveComponent> valueOrDefault = hive.GetValueOrDefault();
    this.SetHive(dest, new EntityUid?((EntityUid) valueOrDefault));
  }

  public bool FromSameHive(Entity<HiveMemberComponent?> a, Entity<HiveMemberComponent?> b)
  {
    Entity<HiveComponent>? hive = this.GetHive(a);
    if (!hive.HasValue)
      return false;
    Entity<HiveComponent> valueOrDefault = hive.GetValueOrDefault();
    return this.IsMember(b, new EntityUid?((EntityUid) valueOrDefault));
  }

  public bool IsMember(Entity<HiveMemberComponent?> member, EntityUid? hive)
  {
    if (hive.HasValue)
    {
      Entity<HiveComponent>? hive1 = this.GetHive(member);
      if (hive1.HasValue)
      {
        EntityUid owner = hive1.GetValueOrDefault().Owner;
        EntityUid? nullable = hive;
        return nullable.HasValue && owner == nullable.GetValueOrDefault();
      }
    }
    return false;
  }

  public bool HasHiveQueen(Entity<HiveComponent> hive) => hive.Comp.CurrentQueen.HasValue;

  public bool SetHiveQueen(EntityUid queen, Entity<HiveComponent> hive)
  {
    hive.Comp.CurrentQueen = new EntityUid?(queen);
    this.Dirty<HiveComponent>(hive);
    return true;
  }

  public bool HasHiveCore(Entity<HiveComponent> hive) => this.GetHiveCore(hive).HasValue;

  public EntityUid? GetHiveCore(Entity<HiveComponent> hive)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveCoreComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveCoreComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out HiveCoreComponent _))
    {
      Entity<HiveComponent>? hive1 = this.GetHive((Entity<HiveMemberComponent>) uid);
      Entity<HiveComponent> entity = hive;
      if ((hive1.HasValue ? (hive1.GetValueOrDefault() == entity ? 1 : 0) : 0) != 0)
        return new EntityUid?(uid);
    }
    return new EntityUid?();
  }

  public void ResetHiveCoreCooldown(Entity<HiveComponent> hive)
  {
    hive.Comp.NewCoreAt = new TimeSpan?(this._timing.CurTime);
    this.Dirty<HiveComponent>(hive);
  }

  public bool TryGetStructureLimit(
    Entity<HiveComponent> hive,
    EntProtoId structureProtoId,
    out int limit)
  {
    return hive.Comp.HiveStructureSlots.TryGetValue(structureProtoId, out limit);
  }

  public void SetSeeThroughContainers(Entity<HiveComponent?> hive, bool see)
  {
    if (!this._query.Resolve((EntityUid) hive, ref hive.Comp, false))
      return;
    hive.Comp.SeeThroughContainers = see;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoComponent, HiveMemberComponent, NightVisionComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoComponent, HiveMemberComponent, NightVisionComponent>();
    EntityUid uid;
    HiveMemberComponent comp2;
    NightVisionComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out XenoComponent _, out comp2, out comp3))
    {
      EntityUid? hive1 = comp2.Hive;
      EntityUid entityUid = (EntityUid) hive;
      if ((hive1.HasValue ? (hive1.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0)
        this._nightVision.SetSeeThroughContainers((Entity<NightVisionComponent>) (uid, comp3), see);
    }
  }

  public void AnnounceNeedsOvipositorToSameHive(Entity<HiveMemberComponent?> xeno)
  {
    Entity<HiveComponent>? hive1 = this.GetHive(xeno);
    if (!hive1.HasValue)
      return;
    Entity<HiveComponent> valueOrDefault = hive1.GetValueOrDefault();
    if (valueOrDefault.Comp.GotOvipositorPopup)
      return;
    valueOrDefault.Comp.GotOvipositorPopup = true;
    this.Dirty<HiveComponent>(valueOrDefault);
    string message = "Enough time has passed, we require the Queen in oviposition for evolution.";
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoComponent, HiveMemberComponent, ActorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoComponent, HiveMemberComponent, ActorComponent>();
    EntityUid uid;
    HiveMemberComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out XenoComponent _, out comp2, out ActorComponent _))
    {
      if (!(uid == xeno.Owner))
      {
        EntityUid? hive2 = comp2.Hive;
        EntityUid entityUid = (EntityUid) valueOrDefault;
        if ((hive2.HasValue ? (hive2.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0)
          this._popup.PopupEntity(message, uid, uid, PopupType.LargeCaution);
      }
    }
    this._xenoAnnounce.AnnounceToHive(new EntityUid(), (EntityUid) valueOrDefault, message);
  }

  public bool TryGetTierLimit(Entity<HiveComponent?> hive, int tier, out FixedPoint2 value)
  {
    value = new FixedPoint2();
    return this._query.Resolve((EntityUid) hive, ref hive.Comp, false) && hive.Comp.TierLimits.TryGetValue(tier, out value);
  }

  public bool TryGetFreeSlots(Entity<HiveComponent?> hive, EntProtoId caste, out int value)
  {
    value = 0;
    return this._query.Resolve((EntityUid) hive, ref hive.Comp, false) && hive.Comp.FreeSlots.TryGetValue(caste, out value);
  }

  public void IncreaseBurrowedLarva(int amount)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveComponent>();
    EntityUid uid;
    HiveComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      this.IncreaseBurrowedLarva((Entity<HiveComponent>) (uid, comp1), amount);
  }

  public void IncreaseBurrowedLarva(Entity<HiveComponent> hive, int amount)
  {
    this.SetHiveBurrowedLarva(hive, hive.Comp.BurrowedLarva + amount);
  }

  private void SetHiveBurrowedLarva(Entity<HiveComponent> hive, int larva)
  {
    hive.Comp.BurrowedLarva = larva;
    this.Dirty<HiveComponent>(hive);
    BurrowedLarvaChangedEvent args = new BurrowedLarvaChangedEvent(larva);
    this.RaiseLocalEvent<BurrowedLarvaChangedEvent>((EntityUid) hive, ref args, true);
  }

  public bool JoinBurrowedLarva(Entity<HiveComponent> hive, ICommonSession session)
  {
    if (this._net.IsClient || hive.Comp.BurrowedLarva <= 0)
      return false;
    EntityUid? larva = new EntityUid?();
    if (!TrySpawnAt<HiveCoreComponent>() && !TrySpawnAt<XenoEvolutionGranterComponent>() && !TrySpawnAt<XenoComponent>() || !larva.HasValue)
      return false;
    this.IncreaseBurrowedLarva(hive, -1);
    this._xeno.MakeXeno((Entity<XenoComponent>) larva.Value);
    this.SetHive((Entity<HiveMemberComponent>) larva.Value, new EntityUid?((EntityUid) hive));
    this._mind.TransferTo((EntityUid) this._mind.CreateMind(new NetUserId?(session.UserId), this.Comp<MetaDataComponent>(larva.Value).EntityName), larva, true);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(34, 2);
    logStringHandler.AppendFormatted(session.Name, format: "player");
    logStringHandler.AppendLiteral(" took a burrowed larva from hive ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) hive)), nameof (hive), "ToPrettyString(hive)");
    logStringHandler.AppendLiteral(".");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCBurrowedLarva, ref local);
    return true;

    bool TrySpawnAt<T>() where T : notnull, Component
    {
      Robust.Shared.GameObjects.EntityQueryEnumerator<T, HiveMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<T, HiveMemberComponent>();
      EntityUid uid;
      HiveMemberComponent comp2;
      while (entityQueryEnumerator.MoveNext(out uid, out T _, out comp2))
      {
        EntityUid? hive1 = comp2.Hive;
        EntityUid hive2 = (EntityUid) hive;
        if ((hive1.HasValue ? (hive1.GetValueOrDefault() != hive2 ? 1 : 0) : 1) == 0 && !this._mobState.IsDead(uid))
        {
          EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(uid);
          larva = new EntityUid?(this.Spawn((string) hive.Comp.BurrowedLarvaId, moverCoordinates));
          this._transform.AttachToGridOrMap(larva.Value);
          return true;
        }
      }
      return false;
    }
  }

  private void OnAutoAssignHiveAdded(Entity<AutoAssignHiveComponent> ent, ref ComponentStartup args)
  {
    Entity<HiveComponent>? hiveByName = this.GetHiveByName(ent.Comp.Hive);
    if (!hiveByName.HasValue)
    {
      this.Log.Debug($"Tried to auto assign hive to {ent.Comp.Hive}, but no such hive was found");
    }
    else
    {
      Entity<HiveMemberComponent> owner = (Entity<HiveMemberComponent>) ent.Owner;
      Entity<HiveComponent>? nullable = hiveByName;
      EntityUid? hive = nullable.HasValue ? new EntityUid?((EntityUid) nullable.GetValueOrDefault()) : new EntityUid?();
      this.SetHive(owner, hive);
    }
  }

  private void OnHiveGunShot(Entity<HiveGunComponent> ent, ref AmmoShotEvent args)
  {
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
      this.SetSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) firedProjectile);
  }
}
