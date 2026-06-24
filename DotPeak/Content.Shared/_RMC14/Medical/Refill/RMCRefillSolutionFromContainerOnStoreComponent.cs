// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Refill.RMCRefillSolutionFromContainerOnStoreComponent
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
namespace Content.Shared._RMC14.Medical.Refill;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMRefillableSolutionSystem)})]
public sealed class RMCRefillSolutionFromContainerOnStoreComponent : 
  Component,
  ISerializationGenerated<RMCRefillSolutionFromContainerOnStoreComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "pressurized_reagent_canister";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanFlush;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FlushTime = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LayerOpacity = 0.75f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCRefillSolutionFromContainerOnStoreComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCRefillSolutionFromContainerOnStoreComponent) target1;
    if (serialization.TryCustomCopy<RMCRefillSolutionFromContainerOnStoreComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target2, hookCtx, false, context))
      target2 = this.ContainerId;
    target.ContainerId = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanFlush, ref target3, hookCtx, false, context))
      target3 = this.CanFlush;
    target.CanFlush = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FlushTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.FlushTime, hookCtx, context);
    target.FlushTime = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LayerOpacity, ref target5, hookCtx, false, context))
      target5 = this.LayerOpacity;
    target.LayerOpacity = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCRefillSolutionFromContainerOnStoreComponent target,
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
    RMCRefillSolutionFromContainerOnStoreComponent target1 = (RMCRefillSolutionFromContainerOnStoreComponent) target;
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
    RMCRefillSolutionFromContainerOnStoreComponent target1 = (RMCRefillSolutionFromContainerOnStoreComponent) target;
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
    RMCRefillSolutionFromContainerOnStoreComponent target1 = (RMCRefillSolutionFromContainerOnStoreComponent) target;
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
  virtual RMCRefillSolutionFromContainerOnStoreComponent Component.Instantiate()
  {
    return new RMCRefillSolutionFromContainerOnStoreComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCRefillSolutionFromContainerOnStoreComponent_AutoState : IComponentState
  {
    public string ContainerId;
    public bool CanFlush;
    public TimeSpan FlushTime;
    public float LayerOpacity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCRefillSolutionFromContainerOnStoreComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCRefillSolutionFromContainerOnStoreComponent, ComponentGetState>(new ComponentEventRefHandler<RMCRefillSolutionFromContainerOnStoreComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCRefillSolutionFromContainerOnStoreComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCRefillSolutionFromContainerOnStoreComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCRefillSolutionFromContainerOnStoreComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCRefillSolutionFromContainerOnStoreComponent.RMCRefillSolutionFromContainerOnStoreComponent_AutoState()
      {
        ContainerId = component.ContainerId,
        CanFlush = component.CanFlush,
        FlushTime = component.FlushTime,
        LayerOpacity = component.LayerOpacity
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCRefillSolutionFromContainerOnStoreComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCRefillSolutionFromContainerOnStoreComponent.RMCRefillSolutionFromContainerOnStoreComponent_AutoState current))
        return;
      component.ContainerId = current.ContainerId;
      component.CanFlush = current.CanFlush;
      component.FlushTime = current.FlushTime;
      component.LayerOpacity = current.LayerOpacity;
    }
  }
}
