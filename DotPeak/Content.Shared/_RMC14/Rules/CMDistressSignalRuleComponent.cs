// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rules.CMDistressSignalRuleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Radio;
using Content.Shared.Roles;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Rules;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
public sealed class CMDistressSignalRuleComponent : 
  Component,
  ISerializationGenerated<CMDistressSignalRuleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> SquadIds = new List<EntProtoId>()
  {
    (EntProtoId) "SquadAlpha",
    (EntProtoId) "SquadBravo",
    (EntProtoId) "SquadCharlie",
    (EntProtoId) "SquadDelta"
  };
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> ExtraSquadIds = new List<EntProtoId>()
  {
    (EntProtoId) "SquadIntel",
    (EntProtoId) "SquadFORECON"
  };
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<EntProtoId, EntityUid> Squads = new Dictionary<EntProtoId, EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? XenoMap;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId HiveId = (EntProtoId) "CMXenoHive";
  [DataField(null, false, 1, false, false, null)]
  public EntityUid Hive;
  [DataField(null, false, 1, false, false, null)]
  public bool Hijack;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobPrototype> QueenJob = (ProtoId<JobPrototype>) "CMXenoQueen";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId QueenEnt = (EntProtoId) "CMXenoQueen";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobPrototype> XenoSelectableJob = (ProtoId<JobPrototype>) "CMXenoSelectableXeno";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId LarvaEnt = (EntProtoId) "CMXenoLarva";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<IFFFactionComponent> MarineFaction = (EntProtoId<IFFFactionComponent>) "FactionMarine";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<IFFFactionComponent> SurvivorFaction = (EntProtoId<IFFFactionComponent>) "FactionSurvivor";
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  public TimeSpan? QueenDiedCheck;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan QueenDiedDelay = TimeSpan.FromMinutes(10L);
  [DataField(null, false, 1, false, false, null)]
  public DistressSignalRuleResult? Result;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan? NextCheck;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CheckEvery = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? AbandonedAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AbandonedDelay = TimeSpan.FromMinutes(5L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier HijackSong = (SoundSpecifier) new SoundCollectionSpecifier("RMCHijack", new AudioParams?(AudioParams.Default.WithVolume(-8f)));
  [DataField(null, false, 1, false, false, null)]
  public bool HijackSongPlayed;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier MajorMarineAudio = (SoundSpecifier) new SoundCollectionSpecifier("RMCMarineMajor");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier MinorMarineAudio = (SoundSpecifier) new SoundCollectionSpecifier("RMCMarineMinor");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier MajorXenoAudio = (SoundSpecifier) new SoundCollectionSpecifier("RMCXenoMajor");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier MinorXenoAudio = (SoundSpecifier) new SoundCollectionSpecifier("RMCXenoMinor");
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? LandingZoneGas = (EntProtoId?) "RMCLandingZoneGas";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobPrototype> CivilianSurvivorJob = (ProtoId<JobPrototype>) "CMSurvivor";
  [DataField(null, false, 1, false, false, null)]
  public List<(ProtoId<JobPrototype> Job, int Amount)> SurvivorJobs = new List<(ProtoId<JobPrototype>, int)>()
  {
    ((ProtoId<JobPrototype>) "CMSurvivorEngineer", 4),
    ((ProtoId<JobPrototype>) "CMSurvivorDoctor", 3),
    ((ProtoId<JobPrototype>) "CMSurvivorSecurity", 2),
    ((ProtoId<JobPrototype>) "CMSurvivorCorporate", 2),
    ((ProtoId<JobPrototype>) "CMSurvivorScientist", 2),
    ((ProtoId<JobPrototype>) "CMSurvivor", -1)
  };
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<JobPrototype>> IgnoreMaximumSurvivorJobs = new List<ProtoId<JobPrototype>>()
  {
    (ProtoId<JobPrototype>) "RMCSurvivorCommandingOfficer"
  };
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype> Insert, int Amount)>>? SurvivorJobInserts;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>? SurvivorJobOverrides;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype> Special, int Amount)>>? SurvivorJobScenarios;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AresGreetingDelay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier AresGreetingAudio = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/ARES/ares_online.ogg");
  [DataField(null, false, 1, false, false, null)]
  public bool AresGreetingDone;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AresMapDelay = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  public bool AresMapDone;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? StartTime;
  [DataField(null, false, 1, false, false, null)]
  public bool ScalingDone;
  [DataField(null, false, 1, false, false, null)]
  public double Scale = 1.0;
  [DataField(null, false, 1, false, false, null)]
  public double MaxScale = 1.0;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? EndAtAllClear;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AllClearEndDelay = TimeSpan.FromMinutes(3L);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RadioChannelPrototype> AllClearChannel = (ProtoId<RadioChannelPrototype>) "MarineCommand";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RoundEndCheckDelay = TimeSpan.FromMinutes(1L);
  [DataField(null, false, 1, false, false, null)]
  public ResPath Thunderdome = new ResPath("/Maps/_RMC14/thunderdome.yml");
  public List<string> AuxiliaryMaps = new List<string>()
  {
    "/Maps/_RMC14/admin_fax.yml"
  };
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobPrototype> XenoSurvivorCorpseJob = (ProtoId<JobPrototype>) "CMSurvivorHost";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan XenoSurvivorCorpseBurstDelay = TimeSpan.FromSeconds(0L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? ForceEndAt;
  [DataField(null, false, 1, false, false, null)]
  public LocId? CustomRoundEndMessage;
  [DataField(null, false, 1, false, false, null)]
  public bool SpawnPlanet = true;
  [DataField(null, false, 1, false, false, null)]
  public bool SpawnSurvivors = true;
  [DataField(null, false, 1, false, false, null)]
  public bool SpawnXenos = true;
  [DataField(null, false, 1, false, false, null)]
  public bool DoJobSlotScaling = true;
  [DataField(null, false, 1, false, false, null)]
  public bool AutoEnd = true;
  [DataField(null, false, 1, false, false, null)]
  public bool StartARESAnnouncements = true;
  [DataField(null, false, 1, false, false, null)]
  public bool Bioscan = true;
  [DataField(null, false, 1, false, false, null)]
  public bool SetHunger = true;
  [DataField(null, false, 1, false, false, null)]
  public bool RequireXenoPlayers = true;
  [DataField(null, false, 1, false, false, null)]
  public bool QueenBoostRemoved;
  [DataField(null, false, 1, false, false, null)]
  public bool RecalculatedPower;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMDistressSignalRuleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMDistressSignalRuleComponent) target1;
    if (serialization.TryCustomCopy<CMDistressSignalRuleComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntProtoId> target2 = (List<EntProtoId>) null;
    if (this.SquadIds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.SquadIds, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntProtoId>>(this.SquadIds, hookCtx, context);
    target.SquadIds = target2;
    List<EntProtoId> target3 = (List<EntProtoId>) null;
    if (this.ExtraSquadIds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.ExtraSquadIds, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<EntProtoId>>(this.ExtraSquadIds, hookCtx, context);
    target.ExtraSquadIds = target3;
    Dictionary<EntProtoId, EntityUid> target4 = (Dictionary<EntProtoId, EntityUid>) null;
    if (this.Squads == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId, EntityUid>>(this.Squads, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<EntProtoId, EntityUid>>(this.Squads, hookCtx, context);
    target.Squads = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.XenoMap, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.XenoMap, hookCtx, context);
    target.XenoMap = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HiveId, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.HiveId, hookCtx, context);
    target.HiveId = target6;
    EntityUid target7 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Hive, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid>(this.Hive, hookCtx, context);
    target.Hive = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hijack, ref target8, hookCtx, false, context))
      target8 = this.Hijack;
    target.Hijack = target8;
    ProtoId<JobPrototype> target9 = new ProtoId<JobPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<JobPrototype>>(this.QueenJob, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<JobPrototype>>(this.QueenJob, hookCtx, context);
    target.QueenJob = target9;
    EntProtoId target10 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.QueenEnt, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntProtoId>(this.QueenEnt, hookCtx, context);
    target.QueenEnt = target10;
    ProtoId<JobPrototype> target11 = new ProtoId<JobPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<JobPrototype>>(this.XenoSelectableJob, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<ProtoId<JobPrototype>>(this.XenoSelectableJob, hookCtx, context);
    target.XenoSelectableJob = target11;
    EntProtoId target12 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.LarvaEnt, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntProtoId>(this.LarvaEnt, hookCtx, context);
    target.LarvaEnt = target12;
    EntProtoId<IFFFactionComponent> target13 = new EntProtoId<IFFFactionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<IFFFactionComponent>>(this.MarineFaction, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId<IFFFactionComponent>>(this.MarineFaction, hookCtx, context);
    target.MarineFaction = target13;
    EntProtoId<IFFFactionComponent> target14 = new EntProtoId<IFFFactionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<IFFFactionComponent>>(this.SurvivorFaction, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<EntProtoId<IFFFactionComponent>>(this.SurvivorFaction, hookCtx, context);
    target.SurvivorFaction = target14;
    TimeSpan? target15 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.QueenDiedCheck, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan?>(this.QueenDiedCheck, hookCtx, context);
    target.QueenDiedCheck = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.QueenDiedDelay, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.QueenDiedDelay, hookCtx, context);
    target.QueenDiedDelay = target16;
    DistressSignalRuleResult? target17 = new DistressSignalRuleResult?();
    if (!serialization.TryCustomCopy<DistressSignalRuleResult?>(this.Result, ref target17, hookCtx, false, context))
      target17 = this.Result;
    target.Result = target17;
    TimeSpan? target18 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextCheck, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<TimeSpan?>(this.NextCheck, hookCtx, context);
    target.NextCheck = target18;
    TimeSpan target19 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CheckEvery, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<TimeSpan>(this.CheckEvery, hookCtx, context);
    target.CheckEvery = target19;
    TimeSpan? target20 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.AbandonedAt, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<TimeSpan?>(this.AbandonedAt, hookCtx, context);
    target.AbandonedAt = target20;
    TimeSpan target21 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AbandonedDelay, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<TimeSpan>(this.AbandonedDelay, hookCtx, context);
    target.AbandonedDelay = target21;
    SoundSpecifier target22 = (SoundSpecifier) null;
    if (this.HijackSong == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HijackSong, ref target22, hookCtx, true, context))
      target22 = serialization.CreateCopy<SoundSpecifier>(this.HijackSong, hookCtx, context);
    target.HijackSong = target22;
    bool target23 = false;
    if (!serialization.TryCustomCopy<bool>(this.HijackSongPlayed, ref target23, hookCtx, false, context))
      target23 = this.HijackSongPlayed;
    target.HijackSongPlayed = target23;
    SoundSpecifier target24 = (SoundSpecifier) null;
    if (this.MajorMarineAudio == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MajorMarineAudio, ref target24, hookCtx, true, context))
      target24 = serialization.CreateCopy<SoundSpecifier>(this.MajorMarineAudio, hookCtx, context);
    target.MajorMarineAudio = target24;
    SoundSpecifier target25 = (SoundSpecifier) null;
    if (this.MinorMarineAudio == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MinorMarineAudio, ref target25, hookCtx, true, context))
      target25 = serialization.CreateCopy<SoundSpecifier>(this.MinorMarineAudio, hookCtx, context);
    target.MinorMarineAudio = target25;
    SoundSpecifier target26 = (SoundSpecifier) null;
    if (this.MajorXenoAudio == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MajorXenoAudio, ref target26, hookCtx, true, context))
      target26 = serialization.CreateCopy<SoundSpecifier>(this.MajorXenoAudio, hookCtx, context);
    target.MajorXenoAudio = target26;
    SoundSpecifier target27 = (SoundSpecifier) null;
    if (this.MinorXenoAudio == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MinorXenoAudio, ref target27, hookCtx, true, context))
      target27 = serialization.CreateCopy<SoundSpecifier>(this.MinorXenoAudio, hookCtx, context);
    target.MinorXenoAudio = target27;
    EntProtoId? target28 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.LandingZoneGas, ref target28, hookCtx, false, context))
      target28 = serialization.CreateCopy<EntProtoId?>(this.LandingZoneGas, hookCtx, context);
    target.LandingZoneGas = target28;
    ProtoId<JobPrototype> target29 = new ProtoId<JobPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<JobPrototype>>(this.CivilianSurvivorJob, ref target29, hookCtx, false, context))
      target29 = serialization.CreateCopy<ProtoId<JobPrototype>>(this.CivilianSurvivorJob, hookCtx, context);
    target.CivilianSurvivorJob = target29;
    List<(ProtoId<JobPrototype>, int)> target30 = (List<(ProtoId<JobPrototype>, int)>) null;
    if (this.SurvivorJobs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<(ProtoId<JobPrototype>, int)>>(this.SurvivorJobs, ref target30, hookCtx, true, context))
      target30 = serialization.CreateCopy<List<(ProtoId<JobPrototype>, int)>>(this.SurvivorJobs, hookCtx, context);
    target.SurvivorJobs = target30;
    List<ProtoId<JobPrototype>> target31 = (List<ProtoId<JobPrototype>>) null;
    if (this.IgnoreMaximumSurvivorJobs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<JobPrototype>>>(this.IgnoreMaximumSurvivorJobs, ref target31, hookCtx, true, context))
      target31 = serialization.CreateCopy<List<ProtoId<JobPrototype>>>(this.IgnoreMaximumSurvivorJobs, hookCtx, context);
    target.IgnoreMaximumSurvivorJobs = target31;
    Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>> target32 = (Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>) null;
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>(this.SurvivorJobInserts, ref target32, hookCtx, true, context))
      target32 = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>(this.SurvivorJobInserts, hookCtx, context);
    target.SurvivorJobInserts = target32;
    Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>> target33 = (Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>) null;
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>>(this.SurvivorJobOverrides, ref target33, hookCtx, true, context))
      target33 = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>>(this.SurvivorJobOverrides, hookCtx, context);
    target.SurvivorJobOverrides = target33;
    Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>> target34 = (Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>) null;
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>(this.SurvivorJobScenarios, ref target34, hookCtx, true, context))
      target34 = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>(this.SurvivorJobScenarios, hookCtx, context);
    target.SurvivorJobScenarios = target34;
    TimeSpan target35 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AresGreetingDelay, ref target35, hookCtx, false, context))
      target35 = serialization.CreateCopy<TimeSpan>(this.AresGreetingDelay, hookCtx, context);
    target.AresGreetingDelay = target35;
    SoundSpecifier target36 = (SoundSpecifier) null;
    if (this.AresGreetingAudio == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AresGreetingAudio, ref target36, hookCtx, true, context))
      target36 = serialization.CreateCopy<SoundSpecifier>(this.AresGreetingAudio, hookCtx, context);
    target.AresGreetingAudio = target36;
    bool target37 = false;
    if (!serialization.TryCustomCopy<bool>(this.AresGreetingDone, ref target37, hookCtx, false, context))
      target37 = this.AresGreetingDone;
    target.AresGreetingDone = target37;
    TimeSpan target38 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AresMapDelay, ref target38, hookCtx, false, context))
      target38 = serialization.CreateCopy<TimeSpan>(this.AresMapDelay, hookCtx, context);
    target.AresMapDelay = target38;
    bool target39 = false;
    if (!serialization.TryCustomCopy<bool>(this.AresMapDone, ref target39, hookCtx, false, context))
      target39 = this.AresMapDone;
    target.AresMapDone = target39;
    TimeSpan? target40 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.StartTime, ref target40, hookCtx, false, context))
      target40 = serialization.CreateCopy<TimeSpan?>(this.StartTime, hookCtx, context);
    target.StartTime = target40;
    bool target41 = false;
    if (!serialization.TryCustomCopy<bool>(this.ScalingDone, ref target41, hookCtx, false, context))
      target41 = this.ScalingDone;
    target.ScalingDone = target41;
    double target42 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Scale, ref target42, hookCtx, false, context))
      target42 = this.Scale;
    target.Scale = target42;
    double target43 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.MaxScale, ref target43, hookCtx, false, context))
      target43 = this.MaxScale;
    target.MaxScale = target43;
    TimeSpan? target44 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EndAtAllClear, ref target44, hookCtx, false, context))
      target44 = serialization.CreateCopy<TimeSpan?>(this.EndAtAllClear, hookCtx, context);
    target.EndAtAllClear = target44;
    TimeSpan target45 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AllClearEndDelay, ref target45, hookCtx, false, context))
      target45 = serialization.CreateCopy<TimeSpan>(this.AllClearEndDelay, hookCtx, context);
    target.AllClearEndDelay = target45;
    ProtoId<RadioChannelPrototype> target46 = new ProtoId<RadioChannelPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>>(this.AllClearChannel, ref target46, hookCtx, false, context))
      target46 = serialization.CreateCopy<ProtoId<RadioChannelPrototype>>(this.AllClearChannel, hookCtx, context);
    target.AllClearChannel = target46;
    TimeSpan target47 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RoundEndCheckDelay, ref target47, hookCtx, false, context))
      target47 = serialization.CreateCopy<TimeSpan>(this.RoundEndCheckDelay, hookCtx, context);
    target.RoundEndCheckDelay = target47;
    ResPath target48 = new ResPath();
    if (!serialization.TryCustomCopy<ResPath>(this.Thunderdome, ref target48, hookCtx, false, context))
      target48 = serialization.CreateCopy<ResPath>(this.Thunderdome, hookCtx, context);
    target.Thunderdome = target48;
    ProtoId<JobPrototype> target49 = new ProtoId<JobPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<JobPrototype>>(this.XenoSurvivorCorpseJob, ref target49, hookCtx, false, context))
      target49 = serialization.CreateCopy<ProtoId<JobPrototype>>(this.XenoSurvivorCorpseJob, hookCtx, context);
    target.XenoSurvivorCorpseJob = target49;
    TimeSpan target50 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.XenoSurvivorCorpseBurstDelay, ref target50, hookCtx, false, context))
      target50 = serialization.CreateCopy<TimeSpan>(this.XenoSurvivorCorpseBurstDelay, hookCtx, context);
    target.XenoSurvivorCorpseBurstDelay = target50;
    TimeSpan? target51 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ForceEndAt, ref target51, hookCtx, false, context))
      target51 = serialization.CreateCopy<TimeSpan?>(this.ForceEndAt, hookCtx, context);
    target.ForceEndAt = target51;
    LocId? target52 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.CustomRoundEndMessage, ref target52, hookCtx, false, context))
      target52 = serialization.CreateCopy<LocId?>(this.CustomRoundEndMessage, hookCtx, context);
    target.CustomRoundEndMessage = target52;
    bool target53 = false;
    if (!serialization.TryCustomCopy<bool>(this.SpawnPlanet, ref target53, hookCtx, false, context))
      target53 = this.SpawnPlanet;
    target.SpawnPlanet = target53;
    bool target54 = false;
    if (!serialization.TryCustomCopy<bool>(this.SpawnSurvivors, ref target54, hookCtx, false, context))
      target54 = this.SpawnSurvivors;
    target.SpawnSurvivors = target54;
    bool target55 = false;
    if (!serialization.TryCustomCopy<bool>(this.SpawnXenos, ref target55, hookCtx, false, context))
      target55 = this.SpawnXenos;
    target.SpawnXenos = target55;
    bool target56 = false;
    if (!serialization.TryCustomCopy<bool>(this.DoJobSlotScaling, ref target56, hookCtx, false, context))
      target56 = this.DoJobSlotScaling;
    target.DoJobSlotScaling = target56;
    bool target57 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoEnd, ref target57, hookCtx, false, context))
      target57 = this.AutoEnd;
    target.AutoEnd = target57;
    bool target58 = false;
    if (!serialization.TryCustomCopy<bool>(this.StartARESAnnouncements, ref target58, hookCtx, false, context))
      target58 = this.StartARESAnnouncements;
    target.StartARESAnnouncements = target58;
    bool target59 = false;
    if (!serialization.TryCustomCopy<bool>(this.Bioscan, ref target59, hookCtx, false, context))
      target59 = this.Bioscan;
    target.Bioscan = target59;
    bool target60 = false;
    if (!serialization.TryCustomCopy<bool>(this.SetHunger, ref target60, hookCtx, false, context))
      target60 = this.SetHunger;
    target.SetHunger = target60;
    bool target61 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireXenoPlayers, ref target61, hookCtx, false, context))
      target61 = this.RequireXenoPlayers;
    target.RequireXenoPlayers = target61;
    bool target62 = false;
    if (!serialization.TryCustomCopy<bool>(this.QueenBoostRemoved, ref target62, hookCtx, false, context))
      target62 = this.QueenBoostRemoved;
    target.QueenBoostRemoved = target62;
    bool target63 = false;
    if (!serialization.TryCustomCopy<bool>(this.RecalculatedPower, ref target63, hookCtx, false, context))
      target63 = this.RecalculatedPower;
    target.RecalculatedPower = target63;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMDistressSignalRuleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CMDistressSignalRuleComponent target1 = (CMDistressSignalRuleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CMDistressSignalRuleComponent target1 = (CMDistressSignalRuleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CMDistressSignalRuleComponent target1 = (CMDistressSignalRuleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CMDistressSignalRuleComponent Component.Instantiate()
  {
    return new CMDistressSignalRuleComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMDistressSignalRuleComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMDistressSignalRuleComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CMDistressSignalRuleComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CMDistressSignalRuleComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.QueenDiedCheck.HasValue)
        component.QueenDiedCheck = new TimeSpan?(component.QueenDiedCheck.Value + args.PausedTime);
      if (!component.NextCheck.HasValue)
        return;
      component.NextCheck = new TimeSpan?(component.NextCheck.Value + args.PausedTime);
    }
  }
}
