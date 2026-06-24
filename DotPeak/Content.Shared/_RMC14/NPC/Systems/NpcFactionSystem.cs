// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.Systems.NpcFactionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.NPC.Systems;

public sealed class NpcFactionSystem : EntitySystem
{
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private SharedTransformSystem _xform;
  private FrozenDictionary<string, FactionData> _factions = FrozenDictionary<string, FactionData>.Empty;
  private Robust.Shared.GameObjects.EntityQuery<FactionExceptionComponent> _exceptionQuery;
  private Robust.Shared.GameObjects.EntityQuery<FactionExceptionTrackerComponent> _trackerQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<NpcFactionMemberComponent, ComponentStartup>(new EntityEventRefHandler<NpcFactionMemberComponent, ComponentStartup>(this.OnFactionStartup));
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnProtoReload));
    this.InitializeException();
    this.RefreshFactions();
  }

  private void OnProtoReload(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<NpcFactionPrototype>())
      return;
    this.RefreshFactions();
  }

  private void OnFactionStartup(Entity<NpcFactionMemberComponent> ent, ref ComponentStartup args)
  {
    this.RefreshFactions(ent);
  }

  private void RefreshFactions(Entity<NpcFactionMemberComponent> ent)
  {
    ent.Comp.FriendlyFactions.Clear();
    ent.Comp.HostileFactions.Clear();
    foreach (ProtoId<NpcFactionPrototype> faction in ent.Comp.Factions)
    {
      FactionData factionData;
      if (this._factions.TryGetValue((string) faction, out factionData))
      {
        ent.Comp.FriendlyFactions.UnionWith((IEnumerable<ProtoId<NpcFactionPrototype>>) factionData.Friendly);
        ent.Comp.HostileFactions.UnionWith((IEnumerable<ProtoId<NpcFactionPrototype>>) factionData.Hostile);
      }
    }
    if (ent.Comp.AddFriendlyFactions != null)
      ent.Comp.FriendlyFactions.UnionWith((IEnumerable<ProtoId<NpcFactionPrototype>>) ent.Comp.AddFriendlyFactions);
    if (ent.Comp.AddHostileFactions == null)
      return;
    ent.Comp.HostileFactions.UnionWith((IEnumerable<ProtoId<NpcFactionPrototype>>) ent.Comp.AddHostileFactions);
  }

  public bool IsMember(Entity<NpcFactionMemberComponent?> ent, [ForbidLiteral] string faction)
  {
    return this.Resolve<NpcFactionMemberComponent>((EntityUid) ent, ref ent.Comp, false) && ent.Comp.Factions.Contains((ProtoId<NpcFactionPrototype>) faction);
  }

  public bool IsMemberOfAny(
    Entity<NpcFactionMemberComponent?> ent,
    [ForbidLiteral] IEnumerable<ProtoId<NpcFactionPrototype>> factions)
  {
    if (!this.Resolve<NpcFactionMemberComponent>((EntityUid) ent, ref ent.Comp, false))
      return false;
    foreach (ProtoId<NpcFactionPrototype> faction in factions)
    {
      if (ent.Comp.Factions.Contains(faction))
        return true;
    }
    return false;
  }

  public void AddFaction(Entity<NpcFactionMemberComponent?> ent, [ForbidLiteral] string faction, bool dirty = true)
  {
    if (!this._proto.HasIndex<NpcFactionPrototype>(faction))
    {
      this.Log.Error("Unable to find faction " + faction);
    }
    else
    {
      ref NpcFactionMemberComponent local = ref ent.Comp;
      if (local == null)
        local = this.EnsureComp<NpcFactionMemberComponent>((EntityUid) ent);
      if (!ent.Comp.Factions.Add((ProtoId<NpcFactionPrototype>) faction) || !dirty)
        return;
      this.RefreshFactions((Entity<NpcFactionMemberComponent>) ((EntityUid) ent, ent.Comp));
    }
  }

  public void AddFactions(
    Entity<NpcFactionMemberComponent?> ent,
    [ForbidLiteral] HashSet<ProtoId<NpcFactionPrototype>> factions,
    bool dirty = true)
  {
    ref NpcFactionMemberComponent local = ref ent.Comp;
    if (local == null)
      local = this.EnsureComp<NpcFactionMemberComponent>((EntityUid) ent);
    foreach (ProtoId<NpcFactionPrototype> faction in factions)
    {
      if (!this._proto.HasIndex<NpcFactionPrototype>(faction))
        this.Log.Error($"Unable to find faction {faction}");
      else
        ent.Comp.Factions.Add(faction);
    }
    if (!dirty)
      return;
    this.RefreshFactions((Entity<NpcFactionMemberComponent>) ((EntityUid) ent, ent.Comp));
  }

  public void RemoveFaction(Entity<NpcFactionMemberComponent?> ent, [ForbidLiteral] string faction, bool dirty = true)
  {
    if (!this._proto.HasIndex<NpcFactionPrototype>(faction))
    {
      this.Log.Error("Unable to find faction " + faction);
    }
    else
    {
      if (!this.Resolve<NpcFactionMemberComponent>((EntityUid) ent, ref ent.Comp, false) || !ent.Comp.Factions.Remove((ProtoId<NpcFactionPrototype>) faction) || !dirty)
        return;
      this.RefreshFactions((Entity<NpcFactionMemberComponent>) ((EntityUid) ent, ent.Comp));
    }
  }

  public void ClearFactions(Entity<NpcFactionMemberComponent?> ent, bool dirty = true)
  {
    if (!this.Resolve<NpcFactionMemberComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    ent.Comp.Factions.Clear();
    if (!dirty)
      return;
    this.RefreshFactions((Entity<NpcFactionMemberComponent>) ((EntityUid) ent, ent.Comp));
  }

  public IEnumerable<EntityUid> GetNearbyHostiles(
    Entity<NpcFactionMemberComponent?, FactionExceptionComponent?> ent,
    float range)
  {
    if (!this.Resolve<NpcFactionMemberComponent>((EntityUid) ent, ref ent.Comp1, false))
      return (IEnumerable<EntityUid>) Array.Empty<EntityUid>();
    IEnumerable<EntityUid> first = this.GetNearbyFactions((EntityUid) ent, range, ent.Comp1.HostileFactions).Where<EntityUid>((Func<EntityUid, bool>) (target => !this.IsEntityFriendly((Entity<NpcFactionMemberComponent>) ((EntityUid) ent, ent.Comp1), (Entity<NpcFactionMemberComponent>) target)));
    if (!this.Resolve<FactionExceptionComponent>((EntityUid) ent, ref ent.Comp2, false))
      return first;
    (EntityUid, FactionExceptionComponent) faction = (ent.Owner, ent.Comp2);
    return first.Union<EntityUid>(this.GetHostiles((Entity<FactionExceptionComponent>) faction)).Where<EntityUid>((Func<EntityUid, bool>) (target => !this.IsIgnored((Entity<FactionExceptionComponent>) faction, target)));
  }

  public IEnumerable<EntityUid> GetNearbyFriendlies(
    Entity<NpcFactionMemberComponent?> ent,
    float range)
  {
    return !this.Resolve<NpcFactionMemberComponent>((EntityUid) ent, ref ent.Comp, false) ? (IEnumerable<EntityUid>) Array.Empty<EntityUid>() : this.GetNearbyFactions((EntityUid) ent, range, ent.Comp.FriendlyFactions);
  }

  private IEnumerable<EntityUid> GetNearbyFactions(
    EntityUid entity,
    float range,
    [ForbidLiteral] HashSet<ProtoId<NpcFactionPrototype>> factions)
  {
    NpcFactionSystem npcFactionSystem = this;
    TransformComponent transformComponent = npcFactionSystem.Transform(entity);
    foreach (Entity<NpcFactionMemberComponent> entity1 in npcFactionSystem._lookup.GetEntitiesInRange<NpcFactionMemberComponent>(npcFactionSystem._xform.GetMapCoordinates((Entity<TransformComponent>) (entity, transformComponent)), range))
    {
      if (!(entity1.Owner == entity) && factions.Overlaps((IEnumerable<ProtoId<NpcFactionPrototype>>) entity1.Comp.Factions))
        yield return entity1.Owner;
    }
  }

  public bool IsEntityFriendly(
    Entity<NpcFactionMemberComponent?> ent,
    Entity<NpcFactionMemberComponent?> other)
  {
    if (!this.Resolve<NpcFactionMemberComponent>((EntityUid) ent, ref ent.Comp, false) || !this.Resolve<NpcFactionMemberComponent>((EntityUid) other, ref other.Comp, false))
      return false;
    return ent.Comp.Factions.Overlaps((IEnumerable<ProtoId<NpcFactionPrototype>>) other.Comp.Factions) || ent.Comp.FriendlyFactions.Overlaps((IEnumerable<ProtoId<NpcFactionPrototype>>) other.Comp.Factions);
  }

  public bool IsFactionFriendly([ForbidLiteral] string target, [ForbidLiteral] string with)
  {
    return this._factions[target].Friendly.Contains((ProtoId<NpcFactionPrototype>) with) && this._factions[with].Friendly.Contains((ProtoId<NpcFactionPrototype>) target);
  }

  public bool IsFactionFriendly([ForbidLiteral] string target, Entity<NpcFactionMemberComponent?> with)
  {
    if (!this.Resolve<NpcFactionMemberComponent>((EntityUid) with, ref with.Comp, false))
      return false;
    return with.Comp.Factions.All<ProtoId<NpcFactionPrototype>>((Func<ProtoId<NpcFactionPrototype>, bool>) (x => this.IsFactionFriendly(target, (string) x))) || with.Comp.FriendlyFactions.Contains((ProtoId<NpcFactionPrototype>) target);
  }

  public bool IsFactionHostile([ForbidLiteral] string target, [ForbidLiteral] string with)
  {
    return this._factions[target].Hostile.Contains((ProtoId<NpcFactionPrototype>) with) && this._factions[with].Hostile.Contains((ProtoId<NpcFactionPrototype>) target);
  }

  public bool IsFactionHostile([ForbidLiteral] string target, Entity<NpcFactionMemberComponent?> with)
  {
    if (!this.Resolve<NpcFactionMemberComponent>((EntityUid) with, ref with.Comp, false))
      return false;
    return with.Comp.Factions.All<ProtoId<NpcFactionPrototype>>((Func<ProtoId<NpcFactionPrototype>, bool>) (x => this.IsFactionHostile(target, (string) x))) || with.Comp.HostileFactions.Contains((ProtoId<NpcFactionPrototype>) target);
  }

  public bool IsFactionNeutral([ForbidLiteral] string target, [ForbidLiteral] string with)
  {
    return !this.IsFactionFriendly(target, with) && !this.IsFactionHostile(target, with);
  }

  public void MakeFriendly([ForbidLiteral] string source, [ForbidLiteral] string target)
  {
    FactionData factionData;
    if (!this._factions.TryGetValue(source, out factionData))
      this.Log.Error("Unable to find faction " + source);
    else if (!this._factions.ContainsKey(target))
    {
      this.Log.Error("Unable to find faction " + target);
    }
    else
    {
      factionData.Friendly.Add((ProtoId<NpcFactionPrototype>) target);
      factionData.Hostile.Remove((ProtoId<NpcFactionPrototype>) target);
      this.RefreshFactions();
    }
  }

  public void MakeHostile([ForbidLiteral] string source, [ForbidLiteral] string target)
  {
    FactionData factionData;
    if (!this._factions.TryGetValue(source, out factionData))
      this.Log.Error("Unable to find faction " + source);
    else if (!this._factions.ContainsKey(target))
    {
      this.Log.Error("Unable to find faction " + target);
    }
    else
    {
      factionData.Friendly.Remove((ProtoId<NpcFactionPrototype>) target);
      factionData.Hostile.Add((ProtoId<NpcFactionPrototype>) target);
      this.RefreshFactions();
    }
  }

  private void RefreshFactions()
  {
    this._factions = this._proto.EnumeratePrototypes<NpcFactionPrototype>().ToFrozenDictionary<NpcFactionPrototype, string, FactionData>((Func<NpcFactionPrototype, string>) (faction => faction.ID), (Func<NpcFactionPrototype, FactionData>) (faction => new FactionData()
    {
      Friendly = faction.Friendly.ToHashSet<ProtoId<NpcFactionPrototype>>(),
      Hostile = faction.Hostile.ToHashSet<ProtoId<NpcFactionPrototype>>()
    }));
    AllEntityQueryEnumerator<NpcFactionMemberComponent> entityQueryEnumerator = this.AllEntityQuery<NpcFactionMemberComponent>();
    EntityUid uid;
    NpcFactionMemberComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.FriendlyFactions.Clear();
      comp1.HostileFactions.Clear();
      this.RefreshFactions((Entity<NpcFactionMemberComponent>) (uid, comp1));
    }
  }

  public void InitializeException()
  {
    this._exceptionQuery = this.GetEntityQuery<FactionExceptionComponent>();
    this._trackerQuery = this.GetEntityQuery<FactionExceptionTrackerComponent>();
    this.SubscribeLocalEvent<FactionExceptionComponent, ComponentShutdown>(new EntityEventRefHandler<FactionExceptionComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<FactionExceptionTrackerComponent, ComponentShutdown>(new EntityEventRefHandler<FactionExceptionTrackerComponent, ComponentShutdown>(this.OnTrackerShutdown));
  }

  private void OnShutdown(Entity<FactionExceptionComponent> ent, ref ComponentShutdown args)
  {
    foreach (EntityUid hostile in ent.Comp.Hostiles)
    {
      FactionExceptionTrackerComponent component;
      if (this._trackerQuery.TryGetComponent(hostile, out component))
        component.Entities.Remove((EntityUid) ent);
    }
    foreach (EntityUid uid in ent.Comp.Ignored)
    {
      FactionExceptionTrackerComponent component;
      if (this._trackerQuery.TryGetComponent(uid, out component))
        component.Entities.Remove((EntityUid) ent);
    }
  }

  private void OnTrackerShutdown(
    Entity<FactionExceptionTrackerComponent> ent,
    ref ComponentShutdown args)
  {
    foreach (EntityUid entity in ent.Comp.Entities)
    {
      FactionExceptionComponent component;
      if (this._exceptionQuery.TryGetComponent(entity, out component))
      {
        component.Ignored.Remove((EntityUid) ent);
        component.Hostiles.Remove((EntityUid) ent);
      }
    }
  }

  public bool IsIgnored(Entity<FactionExceptionComponent?> ent, EntityUid target)
  {
    return this.Resolve<FactionExceptionComponent>((EntityUid) ent, ref ent.Comp, false) && ent.Comp.Ignored.Contains(target);
  }

  public IEnumerable<EntityUid> GetHostiles(Entity<FactionExceptionComponent?> ent)
  {
    return !this.Resolve<FactionExceptionComponent>((EntityUid) ent, ref ent.Comp, false) ? (IEnumerable<EntityUid>) Array.Empty<EntityUid>() : (IEnumerable<EntityUid>) ent.Comp.Hostiles;
  }

  public void IgnoreEntity(
    Entity<FactionExceptionComponent?> ent,
    Entity<FactionExceptionTrackerComponent?> target)
  {
    ref FactionExceptionComponent local1 = ref ent.Comp;
    if (local1 == null)
      local1 = this.EnsureComp<FactionExceptionComponent>((EntityUid) ent);
    ent.Comp.Ignored.Add((EntityUid) target);
    ref FactionExceptionTrackerComponent local2 = ref target.Comp;
    if (local2 == null)
      local2 = this.EnsureComp<FactionExceptionTrackerComponent>((EntityUid) target);
    target.Comp.Entities.Add((EntityUid) ent);
  }

  public void IgnoreEntities(Entity<FactionExceptionComponent?> ent, IEnumerable<EntityUid> ignored)
  {
    ref FactionExceptionComponent local = ref ent.Comp;
    if (local == null)
      local = this.EnsureComp<FactionExceptionComponent>((EntityUid) ent);
    foreach (EntityUid target in ignored)
      this.IgnoreEntity(ent, (Entity<FactionExceptionTrackerComponent>) target);
  }

  public void AggroEntity(
    Entity<FactionExceptionComponent?> ent,
    Entity<FactionExceptionTrackerComponent?> target)
  {
    ref FactionExceptionComponent local1 = ref ent.Comp;
    if (local1 == null)
      local1 = this.EnsureComp<FactionExceptionComponent>((EntityUid) ent);
    ent.Comp.Hostiles.Add((EntityUid) target);
    ref FactionExceptionTrackerComponent local2 = ref target.Comp;
    if (local2 == null)
      local2 = this.EnsureComp<FactionExceptionTrackerComponent>((EntityUid) target);
    target.Comp.Entities.Add((EntityUid) ent);
  }

  public void DeAggroEntity(Entity<FactionExceptionComponent?> ent, EntityUid target)
  {
    FactionExceptionTrackerComponent component;
    if (!this.Resolve<FactionExceptionComponent>((EntityUid) ent, ref ent.Comp, false) || !ent.Comp.Hostiles.Remove(target) || !this._trackerQuery.TryGetComponent(target, out component))
      return;
    component.Entities.Remove((EntityUid) ent);
  }

  public void AggroEntities(Entity<FactionExceptionComponent?> ent, IEnumerable<EntityUid> entities)
  {
    ref FactionExceptionComponent local = ref ent.Comp;
    if (local == null)
      local = this.EnsureComp<FactionExceptionComponent>((EntityUid) ent);
    foreach (EntityUid entity in entities)
      this.AggroEntity(ent, (Entity<FactionExceptionTrackerComponent>) entity);
  }

  public FrozenDictionary<string, FactionData> GetFactions() => this._factions;

  public void RealMakeNeutral(string source, string target)
  {
    FactionData factionData;
    if (!this._factions.TryGetValue(source, out factionData))
      this.Log.Error("Unable to find faction " + source);
    else if (!this._factions.ContainsKey(target))
    {
      this.Log.Error("Unable to find faction " + target);
    }
    else
    {
      factionData.Friendly.Remove((ProtoId<NpcFactionPrototype>) target);
      factionData.Hostile.Remove((ProtoId<NpcFactionPrototype>) target);
      this.RealRefreshFactions();
    }
  }

  public void RealMakeFriendly(string source, string target)
  {
    FactionData factionData;
    if (!this._factions.TryGetValue(source, out factionData))
      this.Log.Error("Unable to find faction " + source);
    else if (!this._factions.ContainsKey(target))
    {
      this.Log.Error("Unable to find faction " + target);
    }
    else
    {
      factionData.Friendly.Add((ProtoId<NpcFactionPrototype>) target);
      factionData.Hostile.Remove((ProtoId<NpcFactionPrototype>) target);
      this.RealRefreshFactions();
    }
  }

  public void RealMakeHostile(string source, string target)
  {
    FactionData factionData;
    if (!this._factions.TryGetValue(source, out factionData))
      this.Log.Error("Unable to find faction " + source);
    else if (!this._factions.ContainsKey(target))
    {
      this.Log.Error("Unable to find faction " + target);
    }
    else
    {
      factionData.Friendly.Remove((ProtoId<NpcFactionPrototype>) target);
      factionData.Hostile.Add((ProtoId<NpcFactionPrototype>) target);
      this.RealRefreshFactions();
    }
  }

  private void RealRefreshFactions()
  {
    AllEntityQueryEnumerator<NpcFactionMemberComponent> entityQueryEnumerator = this.AllEntityQuery<NpcFactionMemberComponent>();
    EntityUid uid;
    NpcFactionMemberComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.FriendlyFactions.Clear();
      comp1.HostileFactions.Clear();
      this.RefreshFactions((Entity<NpcFactionMemberComponent>) (uid, comp1));
    }
  }
}
