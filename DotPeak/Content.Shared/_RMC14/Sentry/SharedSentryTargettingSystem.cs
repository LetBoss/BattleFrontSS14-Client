// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sentry.SharedSentryTargetingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Inventory;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Sentry;

public abstract class SharedSentryTargetingSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private GunIFFSystem _iff;
  [Dependency]
  private SharedTransformSystem _xform;
  private const string SentryExcludedFaction = "RMCDumb";
  public static readonly Dictionary<string, EntProtoId<IFFFactionComponent>> SentryFactionToIff = new Dictionary<string, EntProtoId<IFFFactionComponent>>()
  {
    {
      "UNMC",
      (EntProtoId<IFFFactionComponent>) "FactionMarine"
    },
    {
      "CLF",
      (EntProtoId<IFFFactionComponent>) "FactionCLF"
    },
    {
      "SPP",
      (EntProtoId<IFFFactionComponent>) "FactionSPP"
    },
    {
      "Halcyon",
      (EntProtoId<IFFFactionComponent>) "FactionHalcyon"
    },
    {
      "WeYa",
      (EntProtoId<IFFFactionComponent>) "FactionWeYa"
    },
    {
      "Civilian",
      (EntProtoId<IFFFactionComponent>) "FactionSurvivor"
    },
    {
      "RoyalMarines",
      (EntProtoId<IFFFactionComponent>) "FactionRoyalMarines"
    },
    {
      "Bureau",
      (EntProtoId<IFFFactionComponent>) "FactionBureau"
    },
    {
      "TSE",
      (EntProtoId<IFFFactionComponent>) "FactionTSE"
    }
  };
  public static readonly HashSet<string> SentryAllowedFactions = SharedSentryTargetingSystem.SentryFactionToIff.Keys.ToHashSet<string>();
  private readonly HashSet<EntProtoId<IFFFactionComponent>> _friendlyIffBuffer = new HashSet<EntProtoId<IFFFactionComponent>>();
  private readonly HashSet<EntProtoId<IFFFactionComponent>> _targetIffBuffer = new HashSet<EntProtoId<IFFFactionComponent>>();
  private readonly HashSet<Entity<NpcFactionMemberComponent>> _factionLookupBuffer = new HashSet<Entity<NpcFactionMemberComponent>>();
  private readonly HashSet<Entity<UserIFFComponent>> _userIffLookupBuffer = new HashSet<Entity<UserIFFComponent>>();
  private readonly HashSet<EntityUid> _candidateLookupBuffer = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SentryTargetingComponent, MapInitEvent>(new EntityEventRefHandler<SentryTargetingComponent, MapInitEvent>(this.OnTargetingMapInit));
    this.SubscribeLocalEvent<SentryTargetingComponent, ComponentStartup>(new EntityEventRefHandler<SentryTargetingComponent, ComponentStartup>(this.OnTargetingStartup));
  }

  private void OnTargetingMapInit(Entity<SentryTargetingComponent> ent, ref MapInitEvent args)
  {
    NpcFactionMemberComponent comp;
    if (this.TryComp<NpcFactionMemberComponent>(ent.Owner, out comp) && comp.Factions.Count > 0)
      ent.Comp.OriginalFaction = (string) comp.Factions.First<ProtoId<NpcFactionPrototype>>();
    if (this.HasComp<GunIFFComponent>(ent.Owner) || !this.HasComp<GunComponent>(ent.Owner))
      return;
    this._iff.EnableIntrinsicIFF((EntityUid) ent);
  }

  private void OnTargetingStartup(Entity<SentryTargetingComponent> ent, ref ComponentStartup args)
  {
    if (ent.Comp.FriendlyFactions.Count == 0 && !string.IsNullOrEmpty(ent.Comp.OriginalFaction))
      ent.Comp.FriendlyFactions.Add(ent.Comp.OriginalFaction);
    if (!this._net.IsServer)
      return;
    this.ApplyTargeting(ent);
  }

  public void ApplyDeployerFactions(EntityUid sentry, EntityUid deployer)
  {
    SentryTargetingComponent targetingComponent = this.EnsureComp<SentryTargetingComponent>(sentry);
    targetingComponent.FriendlyFactions.Clear();
    targetingComponent.HumanoidAdded.Clear();
    HashSet<EntProtoId<IFFFactionComponent>> Factions = new HashSet<EntProtoId<IFFFactionComponent>>();
    GetIFFFactionEvent args = new GetIFFFactionEvent(SlotFlags.BELT | SlotFlags.IDCARD | SlotFlags.POCKET, Factions);
    this.RaiseLocalEvent<GetIFFFactionEvent>(deployer, ref args);
    if (Factions.Count > 0)
    {
      foreach ((string key, EntProtoId<IFFFactionComponent> entProtoId) in SharedSentryTargetingSystem.SentryFactionToIff)
      {
        if (Factions.Contains(entProtoId))
          targetingComponent.FriendlyFactions.Add(key);
      }
    }
    else
    {
      NpcFactionMemberComponent comp;
      if (this.TryComp<NpcFactionMemberComponent>(deployer, out comp))
      {
        foreach (ProtoId<NpcFactionPrototype> faction in comp.Factions)
        {
          if (faction != (ProtoId<NpcFactionPrototype>) "RMCDumb" && SharedSentryTargetingSystem.SentryAllowedFactions.Contains((string) faction))
            targetingComponent.FriendlyFactions.Add((string) faction);
        }
        if (comp.Factions.Count > 0)
          targetingComponent.OriginalFaction = (string) comp.Factions.First<ProtoId<NpcFactionPrototype>>();
      }
    }
    targetingComponent.DeployedFriendlyFactions.Clear();
    targetingComponent.DeployedFriendlyFactions.UnionWith((IEnumerable<string>) targetingComponent.FriendlyFactions);
    if (this._net.IsServer)
      this.ApplyTargeting((Entity<SentryTargetingComponent>) (sentry, targetingComponent));
    this.Dirty(sentry, (IComponent) targetingComponent);
  }

  public void SetFriendlyFactions(Entity<SentryTargetingComponent> ent, HashSet<string> factions)
  {
    ent.Comp.FriendlyFactions.Clear();
    ent.Comp.HumanoidAdded.Clear();
    HashSet<string> hashSet = factions.Where<string>((Func<string, bool>) (f => f != "RMCDumb" && f != "Humanoid" && SharedSentryTargetingSystem.SentryAllowedFactions.Contains(f))).ToHashSet<string>();
    if (factions.Contains("Humanoid"))
    {
      foreach (string humanoidFaction in this.GetHumanoidFactions())
      {
        if (hashSet.Add(humanoidFaction))
          ent.Comp.HumanoidAdded.Add(humanoidFaction);
      }
    }
    ent.Comp.FriendlyFactions.UnionWith((IEnumerable<string>) hashSet);
    if (this._net.IsServer)
      this.ApplyTargeting(ent);
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
  }

  public void ResetToDefault(Entity<SentryTargetingComponent> ent)
  {
    ent.Comp.FriendlyFactions.Clear();
    ent.Comp.HumanoidAdded.Clear();
    if (ent.Comp.DeployedFriendlyFactions.Count > 0)
      ent.Comp.FriendlyFactions.UnionWith((IEnumerable<string>) ent.Comp.DeployedFriendlyFactions);
    if (this._net.IsServer)
      this.ApplyTargeting(ent);
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
  }

  public void ToggleFaction(Entity<SentryTargetingComponent> ent, string faction, bool friendly)
  {
    switch (faction)
    {
      case "RMCDumb":
        break;
      case "Humanoid":
        this.ToggleHumanoid(ent, friendly);
        if (this._net.IsServer)
          this.ApplyTargeting(ent);
        this.Dirty(ent.Owner, (IComponent) ent.Comp);
        break;
      default:
        if (friendly)
          ent.Comp.FriendlyFactions.Add(faction);
        else
          ent.Comp.FriendlyFactions.Remove(faction);
        if (this._net.IsServer)
          this.ApplyTargeting(ent);
        this.Dirty(ent.Owner, (IComponent) ent.Comp);
        break;
    }
  }

  public void ToggleHumanoid(Entity<SentryTargetingComponent> ent, bool friendly)
  {
    if (friendly)
    {
      foreach (string humanoidFaction in this.GetHumanoidFactions())
      {
        if (ent.Comp.FriendlyFactions.Add(humanoidFaction))
          ent.Comp.HumanoidAdded.Add(humanoidFaction);
      }
    }
    else
    {
      foreach (string str in ent.Comp.HumanoidAdded)
        ent.Comp.FriendlyFactions.Remove(str);
      ent.Comp.HumanoidAdded.Clear();
    }
  }

  private void BuildFriendlyIff(SentryTargetingComponent comp)
  {
    this._friendlyIffBuffer.Clear();
    foreach (string friendlyFaction in comp.FriendlyFactions)
    {
      EntProtoId<IFFFactionComponent> entProtoId;
      if (SharedSentryTargetingSystem.SentryFactionToIff.TryGetValue(friendlyFaction, out entProtoId))
        this._friendlyIffBuffer.Add(entProtoId);
    }
  }

  private bool IsFriendlyByIff(EntityUid target)
  {
    this._targetIffBuffer.Clear();
    GetIFFFactionEvent args = new GetIFFFactionEvent(SlotFlags.IDCARD, this._targetIffBuffer);
    this.RaiseLocalEvent<GetIFFFactionEvent>(target, ref args);
    foreach (EntProtoId<IFFFactionComponent> entProtoId in this._targetIffBuffer)
    {
      if (this._friendlyIffBuffer.Contains(entProtoId))
        return true;
    }
    return false;
  }

  public bool IsValidTarget(Entity<SentryTargetingComponent> sentry, EntityUid target)
  {
    if (!this.HasComp<UserIFFComponent>(target) && !this.HasComp<NpcFactionMemberComponent>(target))
      return false;
    this.BuildFriendlyIff(sentry.Comp);
    int num = this.IsFriendlyByIff(target) ? 1 : 0;
    this._friendlyIffBuffer.Clear();
    this._targetIffBuffer.Clear();
    return num == 0;
  }

  public IEnumerable<EntityUid> GetNearbyIffHostiles(
    Entity<SentryTargetingComponent> ent,
    float range)
  {
    this.BuildFriendlyIff(ent.Comp);
    MapCoordinates mapCoordinates = this._xform.GetMapCoordinates((EntityUid) ent);
    this._candidateLookupBuffer.Clear();
    this._lookup.GetEntitiesInRange<UserIFFComponent>(mapCoordinates, range, this._userIffLookupBuffer);
    foreach (Entity<UserIFFComponent> entity in this._userIffLookupBuffer)
      this._candidateLookupBuffer.Add(entity.Owner);
    this._lookup.GetEntitiesInRange<NpcFactionMemberComponent>(mapCoordinates, range, this._factionLookupBuffer);
    foreach (Entity<NpcFactionMemberComponent> entity in this._factionLookupBuffer)
      this._candidateLookupBuffer.Add(entity.Owner);
    foreach (EntityUid target in this._candidateLookupBuffer)
    {
      if (!(target == ent.Owner) && !this.IsFriendlyByIff(target))
        yield return target;
    }
    this._candidateLookupBuffer.Clear();
    this._userIffLookupBuffer.Clear();
    this._factionLookupBuffer.Clear();
    this._friendlyIffBuffer.Clear();
    this._targetIffBuffer.Clear();
  }

  private void ApplyTargeting(Entity<SentryTargetingComponent> ent) => this.UpdateSentryIFF(ent);

  private void UpdateSentryIFF(Entity<SentryTargetingComponent> ent)
  {
    UserIFFComponent comp;
    if (!this.TryComp<UserIFFComponent>(ent.Owner, out comp))
      return;
    this._iff.ClearUserFactions((Entity<UserIFFComponent>) (ent.Owner, comp));
    foreach (string friendlyFaction in ent.Comp.FriendlyFactions)
    {
      EntProtoId<IFFFactionComponent> faction;
      if (SharedSentryTargetingSystem.SentryFactionToIff.TryGetValue(friendlyFaction, out faction))
        this._iff.AddUserFaction((Entity<UserIFFComponent>) (ent.Owner, comp), faction);
    }
  }

  public IEnumerable<string> GetHumanoidFactions()
  {
    return (IEnumerable<string>) SharedSentryTargetingSystem.SentryAllowedFactions;
  }

  public bool ContainsAllNonXeno(HashSet<string> friendlyFactions)
  {
    return this.GetHumanoidFactions().All<string>(new Func<string, bool>(friendlyFactions.Contains));
  }
}
