// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Maturing.XenoMaturingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
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
namespace Content.Shared._RMC14.Xenonids.Maturing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoMaturingSystem)})]
public sealed class XenoMaturingComponent : 
  Component,
  ISerializationGenerated<XenoMaturingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromMinutes(10L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan MatureAt;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 CritThreshold;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 DeadThreshold;
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry AddComponents = new ComponentRegistry();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> AddActions = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string BaseName = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoMaturingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoMaturingComponent) target1;
    if (serialization.TryCustomCopy<XenoMaturingComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MatureAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.MatureAt, hookCtx, context);
    target.MatureAt = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.CritThreshold, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.CritThreshold, hookCtx, context);
    target.CritThreshold = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DeadThreshold, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.DeadThreshold, hookCtx, context);
    target.DeadThreshold = target5;
    ComponentRegistry target6 = (ComponentRegistry) null;
    if (this.AddComponents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.AddComponents, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ComponentRegistry>(this.AddComponents, hookCtx, context);
    target.AddComponents = target6;
    List<EntProtoId> target7 = (List<EntProtoId>) null;
    if (this.AddActions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.AddActions, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<EntProtoId>>(this.AddActions, hookCtx, context);
    target.AddActions = target7;
    string target8 = (string) null;
    if (this.BaseName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BaseName, ref target8, hookCtx, false, context))
      target8 = this.BaseName;
    target.BaseName = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoMaturingComponent target,
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
    XenoMaturingComponent target1 = (XenoMaturingComponent) target;
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
    XenoMaturingComponent target1 = (XenoMaturingComponent) target;
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
    XenoMaturingComponent target1 = (XenoMaturingComponent) target;
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
  virtual XenoMaturingComponent Component.Instantiate() => new XenoMaturingComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoMaturingComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoMaturingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoMaturingComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoMaturingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.MatureAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoMaturingComponent_AutoState : IComponentState
  {
    public TimeSpan Delay;
    public TimeSpan MatureAt;
    public FixedPoint2 CritThreshold;
    public FixedPoint2 DeadThreshold;
    public 
    #nullable enable
    List<EntProtoId> AddActions;
    public string BaseName;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoMaturingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoMaturingComponent, ComponentGetState>(new ComponentEventRefHandler<XenoMaturingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoMaturingComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoMaturingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoMaturingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoMaturingComponent.XenoMaturingComponent_AutoState()
      {
        Delay = component.Delay,
        MatureAt = component.MatureAt,
        CritThreshold = component.CritThreshold,
        DeadThreshold = component.DeadThreshold,
        AddActions = component.AddActions,
        BaseName = component.BaseName
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoMaturingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoMaturingComponent.XenoMaturingComponent_AutoState current))
        return;
      component.Delay = current.Delay;
      component.MatureAt = current.MatureAt;
      component.CritThreshold = current.CritThreshold;
      component.DeadThreshold = current.DeadThreshold;
      component.AddActions = current.AddActions == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.AddActions);
      component.BaseName = current.BaseName;
    }
  }
}
