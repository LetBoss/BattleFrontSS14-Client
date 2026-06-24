// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.JobPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Item;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Medal;
using Content.Shared._RMC14.Prototypes;
using Content.Shared.Access;
using Content.Shared.Guidebook;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.StatusIcon;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Roles;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class JobPrototype : IPrototype, IInheritingPrototype, ICMSpecific
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedRoleSystem)}, Other = AccessPermissions.None)]
  public HashSet<JobRequirement>? Requirements;
  [DataField("antagAdvantage", false, 1, false, false, null)]
  public int AntagAdvantage;
  [DataField("jobEntity", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string? JobEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? JobPreviewEntity;
  [DataField(null, false, 1, false, false, null)]
  public bool Whitelisted;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<GuideEntryPrototype>>? Guides;
  [DataField(null, false, 1, false, false, null)]
  public bool HasSquad;
  [DataField(null, false, 1, false, false, null)]
  public bool HasIcon = true;
  [DataField(null, false, 1, false, false, null)]
  public bool Hidden;
  [DataField(null, false, 1, false, false, null)]
  public int? OverwatchSortPriority;
  [DataField(null, false, 1, false, false, null)]
  public bool OverwatchShowName;
  [DataField(null, false, 1, false, false, null)]
  public string? OverwatchRoleName;
  [DataField(null, false, 1, false, false, null)]
  public string? SpawnMenuRoleName;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<RankPrototype>, HashSet<JobRequirement>?>? Ranks;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<RMCPlaytimeMedalType, EntProtoId>? Medals;
  [DataField(null, false, 1, false, false, null)]
  public float RoleWeight;
  [DataField(null, false, 1, false, false, null)]
  public LocId? Greeting;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobPrototype>? UseLoadoutOfJob;
  [DataField(null, false, 1, false, false, null)]
  [NeverPushInheritance]
  public bool BasePlaytimeTracker;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobPrototype>? WhitelistParent;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<CamouflageType, ProtoId<StartingGearPrototype>>? CamouflageStartingGear;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("playTimeTracker", false, 1, true, false, typeof (PrototypeIdSerializer<PlayTimeTrackerPrototype>))]
  public string PlayTimeTracker { get; private set; } = string.Empty;

  [DataField("supervisors", false, 1, false, false, null)]
  public string Supervisors { get; private set; } = "nobody";

  [DataField("name", false, 1, false, false, null)]
  public string Name { get; private set; } = string.Empty;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string LocalizedName => Loc.GetString(this.Name);

  [DataField("description", false, 1, false, false, null)]
  public string? Description { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string? LocalizedDescription
  {
    get => this.Description != null ? Loc.GetString(this.Description) : (string) null;
  }

  [DataField("joinNotifyCrew", false, 1, false, false, null)]
  public bool JoinNotifyCrew { get; private set; }

  [DataField("requireAdminNotify", false, 1, false, false, null)]
  public bool RequireAdminNotify { get; private set; }

  [DataField("setPreference", false, 1, false, false, null)]
  public bool SetPreference { get; private set; } = true;

  [DataField(null, false, 1, false, false, null)]
  public bool ApplyTraits { get; private set; } = true;

  [DataField(null, false, 1, false, false, null)]
  public bool? OverrideConsoleVisibility { get; private set; }

  [DataField("canBeAntag", false, 1, false, false, null)]
  public bool CanBeAntag { get; private set; } = true;

  [DataField("weight", false, 1, false, false, null)]
  public int Weight { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public int? DisplayWeight { get; private set; }

  public int RealDisplayWeight => this.DisplayWeight ?? this.Weight;

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StartingGearPrototype>? StartingGear { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobIconPrototype> Icon { get; private set; } = (ProtoId<JobIconPrototype>) "JobIconUnknown";

  [DataField("special", false, 1, false, true, null)]
  public JobSpecial[] Special { get; private set; } = System.Array.Empty<JobSpecial>();

  [DataField("access", false, 1, false, false, null)]
  public IReadOnlyCollection<ProtoId<AccessLevelPrototype>> Access { get; private set; } = (IReadOnlyCollection<ProtoId<AccessLevelPrototype>>) System.Array.Empty<ProtoId<AccessLevelPrototype>>();

  [DataField("accessGroups", false, 1, false, false, null)]
  public IReadOnlyCollection<ProtoId<AccessGroupPrototype>> AccessGroups { get; private set; } = (IReadOnlyCollection<ProtoId<AccessGroupPrototype>>) System.Array.Empty<ProtoId<AccessGroupPrototype>>();

  [DataField("extendedAccess", false, 1, false, false, null)]
  public IReadOnlyCollection<ProtoId<AccessLevelPrototype>> ExtendedAccess { get; private set; } = (IReadOnlyCollection<ProtoId<AccessLevelPrototype>>) System.Array.Empty<ProtoId<AccessLevelPrototype>>();

  [DataField("extendedAccessGroups", false, 1, false, false, null)]
  public IReadOnlyCollection<ProtoId<AccessGroupPrototype>> ExtendedAccessGroups { get; private set; } = (IReadOnlyCollection<ProtoId<AccessGroupPrototype>>) System.Array.Empty<ProtoId<AccessGroupPrototype>>();

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<JobPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool IsCM { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StartingGearPrototype>? DummyStartingGear { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier LatejoinArrivalSound { get; private set; } = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/ARES/sound_misc_boatswain.ogg");

  [DataField(null, false, 1, false, false, null)]
  public int MarineAuthorityLevel { get; private set; }
}
