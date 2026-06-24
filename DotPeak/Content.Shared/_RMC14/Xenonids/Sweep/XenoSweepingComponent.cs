// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Sweep.XenoSweepingComponent
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
namespace Content.Shared._RMC14.Xenonids.Sweep;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoTailSweepSystem)})]
public sealed class XenoSweepingComponent : 
  Component,
  ISerializationGenerated<XenoSweepingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Direction? LastDirection;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(0.07);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextRotation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TotalRotations;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxRotations = 4;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSweepingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSweepingComponent) target1;
    if (serialization.TryCustomCopy<XenoSweepingComponent>(this, ref target, hookCtx, false, context))
      return;
    Direction? target2 = new Direction?();
    if (!serialization.TryCustomCopy<Direction?>(this.LastDirection, ref target2, hookCtx, false, context))
      target2 = this.LastDirection;
    target.LastDirection = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextRotation, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.NextRotation, hookCtx, context);
    target.NextRotation = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.TotalRotations, ref target5, hookCtx, false, context))
      target5 = this.TotalRotations;
    target.TotalRotations = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxRotations, ref target6, hookCtx, false, context))
      target6 = this.MaxRotations;
    target.MaxRotations = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSweepingComponent target,
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
    XenoSweepingComponent target1 = (XenoSweepingComponent) target;
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
    XenoSweepingComponent target1 = (XenoSweepingComponent) target;
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
    XenoSweepingComponent target1 = (XenoSweepingComponent) target;
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
  virtual XenoSweepingComponent Component.Instantiate() => new XenoSweepingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoSweepingComponent_AutoState : IComponentState
  {
    public Direction? LastDirection;
    public TimeSpan Delay;
    public TimeSpan NextRotation;
    public int TotalRotations;
    public int MaxRotations;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoSweepingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoSweepingComponent, ComponentGetState>(new ComponentEventRefHandler<XenoSweepingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoSweepingComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoSweepingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoSweepingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoSweepingComponent.XenoSweepingComponent_AutoState()
      {
        LastDirection = component.LastDirection,
        Delay = component.Delay,
        NextRotation = component.NextRotation,
        TotalRotations = component.TotalRotations,
        MaxRotations = component.MaxRotations
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoSweepingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoSweepingComponent.XenoSweepingComponent_AutoState current))
        return;
      component.LastDirection = current.LastDirection;
      component.Delay = current.Delay;
      component.NextRotation = current.NextRotation;
      component.TotalRotations = current.TotalRotations;
      component.MaxRotations = current.MaxRotations;
    }
  }
}
