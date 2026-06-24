// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.XenoConstructionAnimationComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoConstructionAnimationComponent : 
  Component,
  ISerializationGenerated<XenoConstructionAnimationComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AnimationTimeFinished = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AnimationTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TotalFrames;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoConstructionAnimationComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoConstructionAnimationComponent) target1;
    if (serialization.TryCustomCopy<XenoConstructionAnimationComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AnimationTimeFinished, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.AnimationTimeFinished, hookCtx, context);
    target.AnimationTimeFinished = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AnimationTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.AnimationTime, hookCtx, context);
    target.AnimationTime = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.TotalFrames, ref target4, hookCtx, false, context))
      target4 = this.TotalFrames;
    target.TotalFrames = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoConstructionAnimationComponent target,
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
    XenoConstructionAnimationComponent target1 = (XenoConstructionAnimationComponent) target;
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
    XenoConstructionAnimationComponent target1 = (XenoConstructionAnimationComponent) target;
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
    XenoConstructionAnimationComponent target1 = (XenoConstructionAnimationComponent) target;
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
  virtual XenoConstructionAnimationComponent Component.Instantiate()
  {
    return new XenoConstructionAnimationComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoConstructionAnimationComponent_AutoState : IComponentState
  {
    public TimeSpan AnimationTimeFinished;
    public TimeSpan AnimationTime;
    public int TotalFrames;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoConstructionAnimationComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoConstructionAnimationComponent, ComponentGetState>(new ComponentEventRefHandler<XenoConstructionAnimationComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoConstructionAnimationComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoConstructionAnimationComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoConstructionAnimationComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoConstructionAnimationComponent.XenoConstructionAnimationComponent_AutoState()
      {
        AnimationTimeFinished = component.AnimationTimeFinished,
        AnimationTime = component.AnimationTime,
        TotalFrames = component.TotalFrames
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoConstructionAnimationComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoConstructionAnimationComponent.XenoConstructionAnimationComponent_AutoState current))
        return;
      component.AnimationTimeFinished = current.AnimationTimeFinished;
      component.AnimationTime = current.AnimationTime;
      component.TotalFrames = current.TotalFrames;
    }
  }
}
