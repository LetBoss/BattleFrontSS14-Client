// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Rotate.XenoRotateComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Rotate;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoRotateSystem)})]
public sealed class XenoRotateComponent : 
  Component,
  ISerializationGenerated<XenoRotateComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FirstRotation = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Direction TargetDirection;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Direction? OriginalDirection;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(0.4);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextRotation;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoRotateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoRotateComponent) target1;
    if (serialization.TryCustomCopy<XenoRotateComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.FirstRotation, ref target2, hookCtx, false, context))
      target2 = this.FirstRotation;
    target.FirstRotation = target2;
    Direction target3 = (Direction) 0;
    if (!serialization.TryCustomCopy<Direction>(this.TargetDirection, ref target3, hookCtx, false, context))
      target3 = this.TargetDirection;
    target.TargetDirection = target3;
    Direction? target4 = new Direction?();
    if (!serialization.TryCustomCopy<Direction?>(this.OriginalDirection, ref target4, hookCtx, false, context))
      target4 = this.OriginalDirection;
    target.OriginalDirection = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextRotation, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextRotation, hookCtx, context);
    target.NextRotation = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoRotateComponent target,
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
    XenoRotateComponent target1 = (XenoRotateComponent) target;
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
    XenoRotateComponent target1 = (XenoRotateComponent) target;
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
    XenoRotateComponent target1 = (XenoRotateComponent) target;
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
  virtual XenoRotateComponent Component.Instantiate() => new XenoRotateComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoRotateComponent_AutoState : IComponentState
  {
    public bool FirstRotation;
    public Direction TargetDirection;
    public Direction? OriginalDirection;
    public TimeSpan Delay;
    public TimeSpan NextRotation;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoRotateComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoRotateComponent, ComponentGetState>(new ComponentEventRefHandler<XenoRotateComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoRotateComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoRotateComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoRotateComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoRotateComponent.XenoRotateComponent_AutoState()
      {
        FirstRotation = component.FirstRotation,
        TargetDirection = component.TargetDirection,
        OriginalDirection = component.OriginalDirection,
        Delay = component.Delay,
        NextRotation = component.NextRotation
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoRotateComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoRotateComponent.XenoRotateComponent_AutoState current))
        return;
      component.FirstRotation = current.FirstRotation;
      component.TargetDirection = current.TargetDirection;
      component.OriginalDirection = current.OriginalDirection;
      component.Delay = current.Delay;
      component.NextRotation = current.NextRotation;
    }
  }
}
