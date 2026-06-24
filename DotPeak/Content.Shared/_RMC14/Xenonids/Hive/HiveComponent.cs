// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hive.HiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Hive;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoHiveSystem), typeof (SharedXenoPylonSystem), typeof (XenoTunnelSystem)})]
public sealed class HiveComponent : 
  Component,
  ISerializationGenerated<HiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<int, FixedPoint2> TierLimits = new Dictionary<int, FixedPoint2>()
  {
    [2] = (FixedPoint2) 0.5,
    [3] = (FixedPoint2) 0.2
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId, int> FreeSlots = new Dictionary<EntProtoId, int>()
  {
    [(EntProtoId) "CMXenoHivelord"] = 1,
    [(EntProtoId) "CMXenoCarrier"] = 1,
    [(EntProtoId) "CMXenoBurrower"] = 1
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId, int> HiveStructureSlots = new Dictionary<EntProtoId, int>()
  {
    [(EntProtoId) "HiveCoreXeno"] = 1,
    [(EntProtoId) "HiveClusterXeno"] = 8,
    [(EntProtoId) "HivePylonXeno"] = 2,
    [(EntProtoId) "HiveEggMorpherXeno"] = 6,
    [(EntProtoId) "HiveRecoveryNodeXeno"] = 6
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<TimeSpan, List<EntProtoId>> Unlocks = new Dictionary<TimeSpan, List<EntProtoId>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntProtoId> AnnouncedUnlocks = new HashSet<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<TimeSpan> AnnouncementsLeft = new List<TimeSpan>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AnnouncedQueenDeathCooldownOver;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AnnouncedHiveCoreCooldownOver;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier AnnounceSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_distantroar_3.ogg", new AudioParams?(AudioParams.Default.WithVolume(-6f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier MarineAnnounceSound = (SoundSpecifier) new SoundCollectionSpecifier("XenoEchoRoar");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SeeThroughContainers;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CurrentQueen;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? LastQueenDeath;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NewQueenCooldown = TimeSpan.FromMinutes(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool GotOvipositorPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NewCoreCooldown = TimeSpan.FromMinutes(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan PreSetupCutoff = TimeSpan.FromMinutes(20L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? NewCoreAt;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? NewQueenAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HijackSurged;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, EntityUid> HiveTunnels = new Dictionary<string, EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BurrowedLarva;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BurrowedLarvaSlotFactor = 4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool LateJoinGainLarva;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 LateJoinMarines;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId BurrowedLarvaId = (EntProtoId) "CMXenoLarva";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HiveComponent) target1;
    if (serialization.TryCustomCopy<HiveComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<int, FixedPoint2> target2 = (Dictionary<int, FixedPoint2>) null;
    if (this.TierLimits == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, FixedPoint2>>(this.TierLimits, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<int, FixedPoint2>>(this.TierLimits, hookCtx, context);
    target.TierLimits = target2;
    Dictionary<EntProtoId, int> target3 = (Dictionary<EntProtoId, int>) null;
    if (this.FreeSlots == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId, int>>(this.FreeSlots, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<EntProtoId, int>>(this.FreeSlots, hookCtx, context);
    target.FreeSlots = target3;
    Dictionary<EntProtoId, int> target4 = (Dictionary<EntProtoId, int>) null;
    if (this.HiveStructureSlots == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId, int>>(this.HiveStructureSlots, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<EntProtoId, int>>(this.HiveStructureSlots, hookCtx, context);
    target.HiveStructureSlots = target4;
    Dictionary<TimeSpan, List<EntProtoId>> target5 = (Dictionary<TimeSpan, List<EntProtoId>>) null;
    if (this.Unlocks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<TimeSpan, List<EntProtoId>>>(this.Unlocks, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<TimeSpan, List<EntProtoId>>>(this.Unlocks, hookCtx, context);
    target.Unlocks = target5;
    HashSet<EntProtoId> target6 = (HashSet<EntProtoId>) null;
    if (this.AnnouncedUnlocks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntProtoId>>(this.AnnouncedUnlocks, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<HashSet<EntProtoId>>(this.AnnouncedUnlocks, hookCtx, context);
    target.AnnouncedUnlocks = target6;
    List<TimeSpan> target7 = (List<TimeSpan>) null;
    if (this.AnnouncementsLeft == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<TimeSpan>>(this.AnnouncementsLeft, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<TimeSpan>>(this.AnnouncementsLeft, hookCtx, context);
    target.AnnouncementsLeft = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.AnnouncedQueenDeathCooldownOver, ref target8, hookCtx, false, context))
      target8 = this.AnnouncedQueenDeathCooldownOver;
    target.AnnouncedQueenDeathCooldownOver = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.AnnouncedHiveCoreCooldownOver, ref target9, hookCtx, false, context))
      target9 = this.AnnouncedHiveCoreCooldownOver;
    target.AnnouncedHiveCoreCooldownOver = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.AnnounceSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AnnounceSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.AnnounceSound, hookCtx, context);
    target.AnnounceSound = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (this.MarineAnnounceSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MarineAnnounceSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.MarineAnnounceSound, hookCtx, context);
    target.MarineAnnounceSound = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.SeeThroughContainers, ref target12, hookCtx, false, context))
      target12 = this.SeeThroughContainers;
    target.SeeThroughContainers = target12;
    EntityUid? target13 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CurrentQueen, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntityUid?>(this.CurrentQueen, hookCtx, context);
    target.CurrentQueen = target13;
    TimeSpan? target14 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastQueenDeath, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan?>(this.LastQueenDeath, hookCtx, context);
    target.LastQueenDeath = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NewQueenCooldown, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.NewQueenCooldown, hookCtx, context);
    target.NewQueenCooldown = target15;
    bool target16 = false;
    if (!serialization.TryCustomCopy<bool>(this.GotOvipositorPopup, ref target16, hookCtx, false, context))
      target16 = this.GotOvipositorPopup;
    target.GotOvipositorPopup = target16;
    TimeSpan target17 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NewCoreCooldown, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan>(this.NewCoreCooldown, hookCtx, context);
    target.NewCoreCooldown = target17;
    TimeSpan target18 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PreSetupCutoff, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<TimeSpan>(this.PreSetupCutoff, hookCtx, context);
    target.PreSetupCutoff = target18;
    TimeSpan? target19 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NewCoreAt, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<TimeSpan?>(this.NewCoreAt, hookCtx, context);
    target.NewCoreAt = target19;
    TimeSpan? target20 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NewQueenAt, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<TimeSpan?>(this.NewQueenAt, hookCtx, context);
    target.NewQueenAt = target20;
    bool target21 = false;
    if (!serialization.TryCustomCopy<bool>(this.HijackSurged, ref target21, hookCtx, false, context))
      target21 = this.HijackSurged;
    target.HijackSurged = target21;
    Dictionary<string, EntityUid> target22 = (Dictionary<string, EntityUid>) null;
    if (this.HiveTunnels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, EntityUid>>(this.HiveTunnels, ref target22, hookCtx, true, context))
      target22 = serialization.CreateCopy<Dictionary<string, EntityUid>>(this.HiveTunnels, hookCtx, context);
    target.HiveTunnels = target22;
    int target23 = 0;
    if (!serialization.TryCustomCopy<int>(this.BurrowedLarva, ref target23, hookCtx, false, context))
      target23 = this.BurrowedLarva;
    target.BurrowedLarva = target23;
    int target24 = 0;
    if (!serialization.TryCustomCopy<int>(this.BurrowedLarvaSlotFactor, ref target24, hookCtx, false, context))
      target24 = this.BurrowedLarvaSlotFactor;
    target.BurrowedLarvaSlotFactor = target24;
    bool target25 = false;
    if (!serialization.TryCustomCopy<bool>(this.LateJoinGainLarva, ref target25, hookCtx, false, context))
      target25 = this.LateJoinGainLarva;
    target.LateJoinGainLarva = target25;
    FixedPoint2 target26 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.LateJoinMarines, ref target26, hookCtx, false, context))
      target26 = serialization.CreateCopy<FixedPoint2>(this.LateJoinMarines, hookCtx, context);
    target.LateJoinMarines = target26;
    EntProtoId target27 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.BurrowedLarvaId, ref target27, hookCtx, false, context))
      target27 = serialization.CreateCopy<EntProtoId>(this.BurrowedLarvaId, hookCtx, context);
    target.BurrowedLarvaId = target27;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HiveComponent target,
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
    HiveComponent target1 = (HiveComponent) target;
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
    HiveComponent target1 = (HiveComponent) target;
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
    HiveComponent target1 = (HiveComponent) target;
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
  virtual HiveComponent Component.Instantiate() => new HiveComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HiveComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HiveComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<HiveComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      HiveComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.NewCoreAt.HasValue)
        component.NewCoreAt = new TimeSpan?(component.NewCoreAt.Value + args.PausedTime);
      if (component.NewQueenAt.HasValue)
        component.NewQueenAt = new TimeSpan?(component.NewQueenAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HiveComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    Dictionary<int, FixedPoint2> TierLimits;
    public Dictionary<EntProtoId, int> FreeSlots;
    public Dictionary<EntProtoId, int> HiveStructureSlots;
    public Dictionary<TimeSpan, List<EntProtoId>> Unlocks;
    public HashSet<EntProtoId> AnnouncedUnlocks;
    public List<TimeSpan> AnnouncementsLeft;
    public bool AnnouncedQueenDeathCooldownOver;
    public bool AnnouncedHiveCoreCooldownOver;
    public SoundSpecifier AnnounceSound;
    public SoundSpecifier MarineAnnounceSound;
    public bool SeeThroughContainers;
    public NetEntity? CurrentQueen;
    public TimeSpan? LastQueenDeath;
    public TimeSpan NewQueenCooldown;
    public bool GotOvipositorPopup;
    public TimeSpan NewCoreCooldown;
    public TimeSpan PreSetupCutoff;
    public TimeSpan? NewCoreAt;
    public TimeSpan? NewQueenAt;
    public bool HijackSurged;
    public Dictionary<string, NetEntity> HiveTunnels;
    public int BurrowedLarva;
    public int BurrowedLarvaSlotFactor;
    public bool LateJoinGainLarva;
    public FixedPoint2 LateJoinMarines;
    public EntProtoId BurrowedLarvaId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HiveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HiveComponent, ComponentGetState>(new ComponentEventRefHandler<HiveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HiveComponent, ComponentHandleState>(new ComponentEventRefHandler<HiveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, HiveComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new HiveComponent.HiveComponent_AutoState()
      {
        TierLimits = component.TierLimits,
        FreeSlots = component.FreeSlots,
        HiveStructureSlots = component.HiveStructureSlots,
        Unlocks = component.Unlocks,
        AnnouncedUnlocks = component.AnnouncedUnlocks,
        AnnouncementsLeft = component.AnnouncementsLeft,
        AnnouncedQueenDeathCooldownOver = component.AnnouncedQueenDeathCooldownOver,
        AnnouncedHiveCoreCooldownOver = component.AnnouncedHiveCoreCooldownOver,
        AnnounceSound = component.AnnounceSound,
        MarineAnnounceSound = component.MarineAnnounceSound,
        SeeThroughContainers = component.SeeThroughContainers,
        CurrentQueen = this.GetNetEntity(component.CurrentQueen),
        LastQueenDeath = component.LastQueenDeath,
        NewQueenCooldown = component.NewQueenCooldown,
        GotOvipositorPopup = component.GotOvipositorPopup,
        NewCoreCooldown = component.NewCoreCooldown,
        PreSetupCutoff = component.PreSetupCutoff,
        NewCoreAt = component.NewCoreAt,
        NewQueenAt = component.NewQueenAt,
        HijackSurged = component.HijackSurged,
        HiveTunnels = this.GetNetEntityDictionary<string>(component.HiveTunnels),
        BurrowedLarva = component.BurrowedLarva,
        BurrowedLarvaSlotFactor = component.BurrowedLarvaSlotFactor,
        LateJoinGainLarva = component.LateJoinGainLarva,
        LateJoinMarines = component.LateJoinMarines,
        BurrowedLarvaId = component.BurrowedLarvaId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HiveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HiveComponent.HiveComponent_AutoState current))
        return;
      component.TierLimits = current.TierLimits == null ? (Dictionary<int, FixedPoint2>) null : new Dictionary<int, FixedPoint2>((IDictionary<int, FixedPoint2>) current.TierLimits);
      component.FreeSlots = current.FreeSlots == null ? (Dictionary<EntProtoId, int>) null : new Dictionary<EntProtoId, int>((IDictionary<EntProtoId, int>) current.FreeSlots);
      component.HiveStructureSlots = current.HiveStructureSlots == null ? (Dictionary<EntProtoId, int>) null : new Dictionary<EntProtoId, int>((IDictionary<EntProtoId, int>) current.HiveStructureSlots);
      component.Unlocks = current.Unlocks == null ? (Dictionary<TimeSpan, List<EntProtoId>>) null : new Dictionary<TimeSpan, List<EntProtoId>>((IDictionary<TimeSpan, List<EntProtoId>>) current.Unlocks);
      component.AnnouncedUnlocks = current.AnnouncedUnlocks == null ? (HashSet<EntProtoId>) null : new HashSet<EntProtoId>((IEnumerable<EntProtoId>) current.AnnouncedUnlocks);
      component.AnnouncementsLeft = current.AnnouncementsLeft == null ? (List<TimeSpan>) null : new List<TimeSpan>((IEnumerable<TimeSpan>) current.AnnouncementsLeft);
      component.AnnouncedQueenDeathCooldownOver = current.AnnouncedQueenDeathCooldownOver;
      component.AnnouncedHiveCoreCooldownOver = current.AnnouncedHiveCoreCooldownOver;
      component.AnnounceSound = current.AnnounceSound;
      component.MarineAnnounceSound = current.MarineAnnounceSound;
      component.SeeThroughContainers = current.SeeThroughContainers;
      component.CurrentQueen = this.EnsureEntity<HiveComponent>(current.CurrentQueen, uid);
      component.LastQueenDeath = current.LastQueenDeath;
      component.NewQueenCooldown = current.NewQueenCooldown;
      component.GotOvipositorPopup = current.GotOvipositorPopup;
      component.NewCoreCooldown = current.NewCoreCooldown;
      component.PreSetupCutoff = current.PreSetupCutoff;
      component.NewCoreAt = current.NewCoreAt;
      component.NewQueenAt = current.NewQueenAt;
      component.HijackSurged = current.HijackSurged;
      this.EnsureEntityDictionary<HiveComponent, string>(current.HiveTunnels, uid, component.HiveTunnels);
      component.BurrowedLarva = current.BurrowedLarva;
      component.BurrowedLarvaSlotFactor = current.BurrowedLarvaSlotFactor;
      component.LateJoinGainLarva = current.LateJoinGainLarva;
      component.LateJoinMarines = current.LateJoinMarines;
      component.BurrowedLarvaId = current.BurrowedLarvaId;
    }
  }
}
