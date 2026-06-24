// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ManageHive.Boons.HiveKingCocoonComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ManageHive.Boons;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (HiveBoonSystem)})]
public sealed class HiveKingCocoonComponent : 
  Component,
  ISerializationGenerated<HiveKingCocoonComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TimeLeft = TimeSpan.FromMinutes(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int LastPylons;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int RequiredPylons = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FirstWarning;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool VoteStarted;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FinalWarning;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn = (EntProtoId) "RMCXenoKing";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HiveKingCocoonComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HiveKingCocoonComponent) target1;
    if (serialization.TryCustomCopy<HiveKingCocoonComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeLeft, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.TimeLeft, hookCtx, context);
    target.TimeLeft = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.LastPylons, ref target3, hookCtx, false, context))
      target3 = this.LastPylons;
    target.LastPylons = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.RequiredPylons, ref target4, hookCtx, false, context))
      target4 = this.RequiredPylons;
    target.RequiredPylons = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.FirstWarning, ref target5, hookCtx, false, context))
      target5 = this.FirstWarning;
    target.FirstWarning = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.VoteStarted, ref target6, hookCtx, false, context))
      target6 = this.VoteStarted;
    target.VoteStarted = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.FinalWarning, ref target7, hookCtx, false, context))
      target7 = this.FinalWarning;
    target.FinalWarning = target7;
    EntProtoId target8 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HiveKingCocoonComponent target,
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
    HiveKingCocoonComponent target1 = (HiveKingCocoonComponent) target;
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
    HiveKingCocoonComponent target1 = (HiveKingCocoonComponent) target;
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
    HiveKingCocoonComponent target1 = (HiveKingCocoonComponent) target;
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
  virtual HiveKingCocoonComponent Component.Instantiate() => new HiveKingCocoonComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HiveKingCocoonComponent_AutoState : IComponentState
  {
    public TimeSpan TimeLeft;
    public int LastPylons;
    public int RequiredPylons;
    public bool FirstWarning;
    public bool VoteStarted;
    public bool FinalWarning;
    public EntProtoId Spawn;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HiveKingCocoonComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HiveKingCocoonComponent, ComponentGetState>(new ComponentEventRefHandler<HiveKingCocoonComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HiveKingCocoonComponent, ComponentHandleState>(new ComponentEventRefHandler<HiveKingCocoonComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HiveKingCocoonComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HiveKingCocoonComponent.HiveKingCocoonComponent_AutoState()
      {
        TimeLeft = component.TimeLeft,
        LastPylons = component.LastPylons,
        RequiredPylons = component.RequiredPylons,
        FirstWarning = component.FirstWarning,
        VoteStarted = component.VoteStarted,
        FinalWarning = component.FinalWarning,
        Spawn = component.Spawn
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HiveKingCocoonComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HiveKingCocoonComponent.HiveKingCocoonComponent_AutoState current))
        return;
      component.TimeLeft = current.TimeLeft;
      component.LastPylons = current.LastPylons;
      component.RequiredPylons = current.RequiredPylons;
      component.FirstWarning = current.FirstWarning;
      component.VoteStarted = current.VoteStarted;
      component.FinalWarning = current.FinalWarning;
      component.Spawn = current.Spawn;
    }
  }
}
