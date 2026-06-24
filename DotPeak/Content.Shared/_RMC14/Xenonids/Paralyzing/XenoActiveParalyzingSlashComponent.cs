// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Paralyzing.XenoActiveParalyzingSlashComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Paralyzing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoParalyzingSlashSystem)})]
public sealed class XenoActiveParalyzingSlashComponent : 
  Component,
  ISerializationGenerated<XenoActiveParalyzingSlashComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan ExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DazeTime = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ParalyzeDelay = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ParalyzeDuration = TimeSpan.FromSeconds(4L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoActiveParalyzingSlashComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoActiveParalyzingSlashComponent) target1;
    if (serialization.TryCustomCopy<XenoActiveParalyzingSlashComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpireAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ExpireAt, hookCtx, context);
    target.ExpireAt = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DazeTime, hookCtx, context);
    target.DazeTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParalyzeDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.ParalyzeDelay, hookCtx, context);
    target.ParalyzeDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParalyzeDuration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ParalyzeDuration, hookCtx, context);
    target.ParalyzeDuration = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoActiveParalyzingSlashComponent target,
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
    XenoActiveParalyzingSlashComponent target1 = (XenoActiveParalyzingSlashComponent) target;
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
    XenoActiveParalyzingSlashComponent target1 = (XenoActiveParalyzingSlashComponent) target;
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
    XenoActiveParalyzingSlashComponent target1 = (XenoActiveParalyzingSlashComponent) target;
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
  virtual XenoActiveParalyzingSlashComponent Component.Instantiate()
  {
    return new XenoActiveParalyzingSlashComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoActiveParalyzingSlashComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoActiveParalyzingSlashComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoActiveParalyzingSlashComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoActiveParalyzingSlashComponent component,
      ref EntityUnpausedEvent args)
    {
      component.ExpireAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoActiveParalyzingSlashComponent_AutoState : IComponentState
  {
    public TimeSpan ExpireAt;
    public TimeSpan DazeTime;
    public TimeSpan ParalyzeDelay;
    public TimeSpan ParalyzeDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoActiveParalyzingSlashComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoActiveParalyzingSlashComponent, ComponentGetState>(new ComponentEventRefHandler<XenoActiveParalyzingSlashComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoActiveParalyzingSlashComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoActiveParalyzingSlashComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoActiveParalyzingSlashComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoActiveParalyzingSlashComponent.XenoActiveParalyzingSlashComponent_AutoState()
      {
        ExpireAt = component.ExpireAt,
        DazeTime = component.DazeTime,
        ParalyzeDelay = component.ParalyzeDelay,
        ParalyzeDuration = component.ParalyzeDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoActiveParalyzingSlashComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoActiveParalyzingSlashComponent.XenoActiveParalyzingSlashComponent_AutoState current))
        return;
      component.ExpireAt = current.ExpireAt;
      component.DazeTime = current.DazeTime;
      component.ParalyzeDelay = current.ParalyzeDelay;
      component.ParalyzeDuration = current.ParalyzeDuration;
    }
  }
}
