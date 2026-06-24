// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Roles.Ranks.SharedRankSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared.Dataset;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Marines.Roles.Ranks;

public abstract class SharedRankSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private IEntityManager _entMan;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RankComponent, ExaminedEvent>(new EntityEventRefHandler<RankComponent, ExaminedEvent>(this.OnRankExamined));
  }

  private void OnRankExamined(Entity<RankComponent> ent, ref ExaminedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.Examiner))
      return;
    using (args.PushGroup(nameof (SharedRankSystem), 1))
    {
      EntityUid owner = ent.Owner;
      string rankString = this.GetRankString(owner, hasPaygrade: true);
      if (rankString == null)
        return;
      string markup = this.Loc.GetString("rmc-rank-component-examine", ("user", (object) owner), ("rank", (object) rankString));
      args.PushMarkup(markup);
    }
  }

  public void SetRank(EntityUid uid, RankPrototype from)
  {
    this.SetRank(uid, (ProtoId<RankPrototype>) from.ID);
  }

  public void SetRank(EntityUid uid, ProtoId<RankPrototype> from)
  {
    RankComponent rankComponent = this.EnsureComp<RankComponent>(uid);
    rankComponent.Rank = new ProtoId<RankPrototype>?(from);
    this.Dirty(uid, (IComponent) rankComponent);
  }

  public RankPrototype? GetRank(EntityUid uid)
  {
    RankComponent comp;
    return this.TryComp<RankComponent>(uid, out comp) ? this.GetRank(comp) : (RankPrototype) null;
  }

  public RankPrototype? GetRank(RankComponent component)
  {
    RankPrototype prototype;
    return this._prototypes.TryIndex<RankPrototype>(component.Rank, out prototype) && prototype != null ? prototype : (RankPrototype) null;
  }

  public string? GetRankString(EntityUid uid, bool isShort = false, bool hasPaygrade = false)
  {
    RankPrototype rank = this.GetRank(uid);
    if (rank == null)
      return (string) null;
    if (isShort)
    {
      HumanoidAppearanceComponent comp;
      if (rank.FemalePrefix == null || rank.MalePrefix == null || !this.TryComp<HumanoidAppearanceComponent>(uid, out comp))
        return rank.Prefix;
      string rankString;
      switch (comp.Gender)
      {
        case Gender.Female:
          rankString = rank.FemalePrefix;
          break;
        case Gender.Male:
          rankString = rank.MalePrefix;
          break;
        default:
          rankString = rank.Prefix;
          break;
      }
      return rankString;
    }
    return hasPaygrade && rank.Paygrade != null ? $"({this.Loc.GetString(rank.Paygrade)}) {this.Loc.GetString(rank.Name)}" : rank.Name;
  }

  public string? GetSpeakerRankName(EntityUid uid)
  {
    string rankString = this.GetRankString(uid, true);
    return rankString == null ? (string) null : $"{rankString} {this.Name(uid)}";
  }

  public string? GetSpeakerFullRankName(EntityUid uid)
  {
    string rankString = this.GetRankString(uid);
    return rankString == null ? (string) null : $"{rankString} {this.Name(uid)}";
  }

  public List<EntityUid>? GetEntitiesWithHighestRank(
    List<EntityUid> entities,
    ProtoId<DatasetPrototype> rankHierarchyId)
  {
    List<EntityUid> entityUidList = new List<EntityUid>();
    DatasetPrototype prototype1;
    if (!this._prototypes.TryIndex<DatasetPrototype>(rankHierarchyId, out prototype1))
      return (List<EntityUid>) null;
    List<string> list = prototype1.Values.ToList<string>();
    if (list.Count == 0)
    {
      this.Log.Error($"The rank hierarchy dataset '{rankHierarchyId}' has an invalid value: empty. The highest rank cannot be determined.");
      return (List<EntityUid>) null;
    }
    Dictionary<EntityUid, int> source = new Dictionary<EntityUid, int>();
    int highestRank = -1;
    foreach (EntityUid entity in entities)
    {
      RankComponent comp;
      RankPrototype prototype2;
      if (this.TryComp<RankComponent>(entity, out comp) && comp.Rank.HasValue && this._prototypes.TryIndex<RankPrototype>(comp.Rank, out prototype2))
      {
        int num = list.IndexOf(prototype2.ID);
        if (num != -1)
        {
          source[entity] = num;
          if (num > highestRank)
            highestRank = num;
        }
      }
    }
    return highestRank == -1 ? (List<EntityUid>) null : source.Where<KeyValuePair<EntityUid, int>>((Func<KeyValuePair<EntityUid, int>, bool>) (pair => pair.Value == highestRank)).Select<KeyValuePair<EntityUid, int>, EntityUid>((Func<KeyValuePair<EntityUid, int>, EntityUid>) (pair => pair.Key)).ToList<EntityUid>();
  }

  public bool HasInvalidRank(EntityUid entity, ProtoId<RankPrototype> invalidRankId = default (ProtoId<RankPrototype>))
  {
    RankComponent component;
    if (!this._entMan.TryGetComponent<RankComponent>(entity, out component) || !component.Rank.HasValue)
      return true;
    if (invalidRankId != new ProtoId<RankPrototype>())
    {
      ProtoId<RankPrototype>? rank = component.Rank;
      ProtoId<RankPrototype> protoId = invalidRankId;
      if ((rank.HasValue ? (rank.GetValueOrDefault() == protoId ? 1 : 0) : 0) != 0)
        return true;
    }
    return false;
  }
}
