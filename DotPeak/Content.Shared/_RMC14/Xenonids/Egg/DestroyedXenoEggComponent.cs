// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Egg.DestroyedXenoEggComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Egg;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class DestroyedXenoEggComponent : 
  Component,
  ISerializationGenerated<DestroyedXenoEggComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AnimationState = "egg_exploding";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AnimationTime = TimeSpan.FromSeconds(0.7);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Layer = "egg";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DestroyedXenoEggComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DestroyedXenoEggComponent) target1;
    if (serialization.TryCustomCopy<DestroyedXenoEggComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.AnimationState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AnimationState, ref target2, hookCtx, false, context))
      target2 = this.AnimationState;
    target.AnimationState = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AnimationTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.AnimationTime, hookCtx, context);
    target.AnimationTime = target3;
    string target4 = (string) null;
    if (this.Layer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Layer, ref target4, hookCtx, false, context))
      target4 = this.Layer;
    target.Layer = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DestroyedXenoEggComponent target,
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
    DestroyedXenoEggComponent target1 = (DestroyedXenoEggComponent) target;
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
    DestroyedXenoEggComponent target1 = (DestroyedXenoEggComponent) target;
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
    DestroyedXenoEggComponent target1 = (DestroyedXenoEggComponent) target;
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
  virtual DestroyedXenoEggComponent Component.Instantiate() => new DestroyedXenoEggComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DestroyedXenoEggComponent_AutoState : IComponentState
  {
    public string AnimationState;
    public TimeSpan AnimationTime;
    public string Layer;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DestroyedXenoEggComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DestroyedXenoEggComponent, ComponentGetState>(new ComponentEventRefHandler<DestroyedXenoEggComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DestroyedXenoEggComponent, ComponentHandleState>(new ComponentEventRefHandler<DestroyedXenoEggComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DestroyedXenoEggComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DestroyedXenoEggComponent.DestroyedXenoEggComponent_AutoState()
      {
        AnimationState = component.AnimationState,
        AnimationTime = component.AnimationTime,
        Layer = component.Layer
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DestroyedXenoEggComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DestroyedXenoEggComponent.DestroyedXenoEggComponent_AutoState current))
        return;
      component.AnimationState = current.AnimationState;
      component.AnimationTime = current.AnimationTime;
      component.Layer = current.Layer;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, DestroyedXenoEggComponent>(uid, component, ref args1);
    }
  }
}
