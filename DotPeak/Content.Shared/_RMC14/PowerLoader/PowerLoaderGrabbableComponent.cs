// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.PowerLoader.PowerLoaderGrabbableComponent
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
namespace Content.Shared._RMC14.PowerLoader;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (PowerLoaderSystem)})]
public sealed class PowerLoaderGrabbableComponent : 
  Component,
  ISerializationGenerated<PowerLoaderGrabbableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId VirtualRight;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId VirtualLeft;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PowerLoaderGrabbableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PowerLoaderGrabbableComponent) target1;
    if (serialization.TryCustomCopy<PowerLoaderGrabbableComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.VirtualRight, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.VirtualRight, hookCtx, context);
    target.VirtualRight = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.VirtualLeft, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.VirtualLeft, hookCtx, context);
    target.VirtualLeft = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PowerLoaderGrabbableComponent target,
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
    PowerLoaderGrabbableComponent target1 = (PowerLoaderGrabbableComponent) target;
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
    PowerLoaderGrabbableComponent target1 = (PowerLoaderGrabbableComponent) target;
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
    PowerLoaderGrabbableComponent target1 = (PowerLoaderGrabbableComponent) target;
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
  virtual PowerLoaderGrabbableComponent Component.Instantiate()
  {
    return new PowerLoaderGrabbableComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PowerLoaderGrabbableComponent_AutoState : IComponentState
  {
    public TimeSpan Delay;
    public EntProtoId VirtualRight;
    public EntProtoId VirtualLeft;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PowerLoaderGrabbableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PowerLoaderGrabbableComponent, ComponentGetState>(new ComponentEventRefHandler<PowerLoaderGrabbableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PowerLoaderGrabbableComponent, ComponentHandleState>(new ComponentEventRefHandler<PowerLoaderGrabbableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PowerLoaderGrabbableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PowerLoaderGrabbableComponent.PowerLoaderGrabbableComponent_AutoState()
      {
        Delay = component.Delay,
        VirtualRight = component.VirtualRight,
        VirtualLeft = component.VirtualLeft
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PowerLoaderGrabbableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PowerLoaderGrabbableComponent.PowerLoaderGrabbableComponent_AutoState current))
        return;
      component.Delay = current.Delay;
      component.VirtualRight = current.VirtualRight;
      component.VirtualLeft = current.VirtualLeft;
    }
  }
}
