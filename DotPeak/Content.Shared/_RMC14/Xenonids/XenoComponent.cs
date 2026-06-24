// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.XenoComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared.Access;
using Content.Shared.Alert;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Eye;
using Content.Shared.Roles;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (XenoSystem)})]
public sealed class XenoComponent : 
  Component,
  ISerializationGenerated<XenoComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public ProtoId<JobPrototype> Role;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> ActionIds = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId, EntityUid> Actions = new Dictionary<EntProtoId, EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<AccessLevelPrototype>> AccessLevels = new HashSet<ProtoId<AccessLevelPrototype>>()
  {
    (ProtoId<AccessLevelPrototype>) "CMAccessXeno"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Tier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 HudOffset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ContributesToVictory = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CountedInSlots = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BypassTierCount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UnlockAt = TimeSpan.FromSeconds(60L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<AlertPrototype> ArmorAlert = (ProtoId<AlertPrototype>) "XenoArmor";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SpawnAtLeaderPoint;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmoteSoundsPrototype>? EmoteSounds = (ProtoId<EmoteSoundsPrototype>?) "Xeno";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MuteOnSpawn;
  public EmoteSoundsPrototype? Sounds;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public VisibilityFlags Visibility = VisibilityFlags.Xeno;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoPheromones? IgnorePheromones;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoComponent) target1;
    if (serialization.TryCustomCopy<XenoComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<JobPrototype> target2 = new ProtoId<JobPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<JobPrototype>>(this.Role, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<JobPrototype>>(this.Role, hookCtx, context);
    target.Role = target2;
    List<EntProtoId> target3 = (List<EntProtoId>) null;
    if (this.ActionIds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.ActionIds, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<EntProtoId>>(this.ActionIds, hookCtx, context);
    target.ActionIds = target3;
    Dictionary<EntProtoId, EntityUid> target4 = (Dictionary<EntProtoId, EntityUid>) null;
    if (this.Actions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId, EntityUid>>(this.Actions, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<EntProtoId, EntityUid>>(this.Actions, hookCtx, context);
    target.Actions = target4;
    HashSet<ProtoId<AccessLevelPrototype>> target5 = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    if (this.AccessLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.AccessLevels, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.AccessLevels, hookCtx, context);
    target.AccessLevels = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.Tier, ref target6, hookCtx, false, context))
      target6 = this.Tier;
    target.Tier = target6;
    Vector2 target7 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.HudOffset, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Vector2>(this.HudOffset, hookCtx, context);
    target.HudOffset = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.ContributesToVictory, ref target8, hookCtx, false, context))
      target8 = this.ContributesToVictory;
    target.ContributesToVictory = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.CountedInSlots, ref target9, hookCtx, false, context))
      target9 = this.CountedInSlots;
    target.CountedInSlots = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.BypassTierCount, ref target10, hookCtx, false, context))
      target10 = this.BypassTierCount;
    target.BypassTierCount = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnlockAt, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.UnlockAt, hookCtx, context);
    target.UnlockAt = target11;
    ProtoId<AlertPrototype> target12 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.ArmorAlert, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.ArmorAlert, hookCtx, context);
    target.ArmorAlert = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.SpawnAtLeaderPoint, ref target13, hookCtx, false, context))
      target13 = this.SpawnAtLeaderPoint;
    target.SpawnAtLeaderPoint = target13;
    ProtoId<EmoteSoundsPrototype>? target14 = new ProtoId<EmoteSoundsPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<EmoteSoundsPrototype>?>(this.EmoteSounds, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<ProtoId<EmoteSoundsPrototype>?>(this.EmoteSounds, hookCtx, context);
    target.EmoteSounds = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.MuteOnSpawn, ref target15, hookCtx, false, context))
      target15 = this.MuteOnSpawn;
    target.MuteOnSpawn = target15;
    VisibilityFlags target16 = VisibilityFlags.None;
    if (!serialization.TryCustomCopy<VisibilityFlags>(this.Visibility, ref target16, hookCtx, false, context))
      target16 = this.Visibility;
    target.Visibility = target16;
    XenoPheromones? target17 = new XenoPheromones?();
    if (!serialization.TryCustomCopy<XenoPheromones?>(this.IgnorePheromones, ref target17, hookCtx, false, context))
      target17 = this.IgnorePheromones;
    target.IgnorePheromones = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoComponent target,
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
    XenoComponent target1 = (XenoComponent) target;
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
    XenoComponent target1 = (XenoComponent) target;
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
    XenoComponent target1 = (XenoComponent) target;
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
  virtual XenoComponent Component.Instantiate() => new XenoComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoComponent_AutoState : IComponentState
  {
    public ProtoId<JobPrototype> Role;
    public List<EntProtoId> ActionIds;
    public Dictionary<EntProtoId, NetEntity> Actions;
    public HashSet<ProtoId<AccessLevelPrototype>> AccessLevels;
    public int Tier;
    public Vector2 HudOffset;
    public bool ContributesToVictory;
    public bool CountedInSlots;
    public bool BypassTierCount;
    public TimeSpan UnlockAt;
    public ProtoId<AlertPrototype> ArmorAlert;
    public bool SpawnAtLeaderPoint;
    public ProtoId<EmoteSoundsPrototype>? EmoteSounds;
    public bool MuteOnSpawn;
    public VisibilityFlags Visibility;
    public XenoPheromones? IgnorePheromones;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoComponent, ComponentGetState>(new ComponentEventRefHandler<XenoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, XenoComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoComponent.XenoComponent_AutoState()
      {
        Role = component.Role,
        ActionIds = component.ActionIds,
        Actions = this.GetNetEntityDictionary<EntProtoId>(component.Actions),
        AccessLevels = component.AccessLevels,
        Tier = component.Tier,
        HudOffset = component.HudOffset,
        ContributesToVictory = component.ContributesToVictory,
        CountedInSlots = component.CountedInSlots,
        BypassTierCount = component.BypassTierCount,
        UnlockAt = component.UnlockAt,
        ArmorAlert = component.ArmorAlert,
        SpawnAtLeaderPoint = component.SpawnAtLeaderPoint,
        EmoteSounds = component.EmoteSounds,
        MuteOnSpawn = component.MuteOnSpawn,
        Visibility = component.Visibility,
        IgnorePheromones = component.IgnorePheromones
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoComponent.XenoComponent_AutoState current))
        return;
      component.Role = current.Role;
      component.ActionIds = current.ActionIds == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.ActionIds);
      this.EnsureEntityDictionary<XenoComponent, EntProtoId>(current.Actions, uid, component.Actions);
      component.AccessLevels = current.AccessLevels == null ? (HashSet<ProtoId<AccessLevelPrototype>>) null : new HashSet<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) current.AccessLevels);
      component.Tier = current.Tier;
      component.HudOffset = current.HudOffset;
      component.ContributesToVictory = current.ContributesToVictory;
      component.CountedInSlots = current.CountedInSlots;
      component.BypassTierCount = current.BypassTierCount;
      component.UnlockAt = current.UnlockAt;
      component.ArmorAlert = current.ArmorAlert;
      component.SpawnAtLeaderPoint = current.SpawnAtLeaderPoint;
      component.EmoteSounds = current.EmoteSounds;
      component.MuteOnSpawn = current.MuteOnSpawn;
      component.Visibility = current.Visibility;
      component.IgnorePheromones = current.IgnorePheromones;
    }
  }
}
