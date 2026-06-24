// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Evacuation.DetonatingEvacuationComputerComponent
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
namespace Content.Shared._RMC14.Evacuation;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedEvacuationSystem)})]
public sealed class DetonatingEvacuationComputerComponent : 
  Component,
  ISerializationGenerated<DetonatingEvacuationComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan DetonateAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Detonated;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan EjectAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Ejected;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DetonatingEvacuationComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DetonatingEvacuationComputerComponent) target1;
    if (serialization.TryCustomCopy<DetonatingEvacuationComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DetonateAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.DetonateAt, hookCtx, context);
    target.DetonateAt = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Detonated, ref target3, hookCtx, false, context))
      target3 = this.Detonated;
    target.Detonated = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EjectAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.EjectAt, hookCtx, context);
    target.EjectAt = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Ejected, ref target5, hookCtx, false, context))
      target5 = this.Ejected;
    target.Ejected = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DetonatingEvacuationComputerComponent target,
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
    DetonatingEvacuationComputerComponent target1 = (DetonatingEvacuationComputerComponent) target;
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
    DetonatingEvacuationComputerComponent target1 = (DetonatingEvacuationComputerComponent) target;
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
    DetonatingEvacuationComputerComponent target1 = (DetonatingEvacuationComputerComponent) target;
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
  virtual DetonatingEvacuationComputerComponent Component.Instantiate()
  {
    return new DetonatingEvacuationComputerComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DetonatingEvacuationComputerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DetonatingEvacuationComputerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DetonatingEvacuationComputerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DetonatingEvacuationComputerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.DetonateAt += args.PausedTime;
      component.EjectAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DetonatingEvacuationComputerComponent_AutoState : IComponentState
  {
    public TimeSpan DetonateAt;
    public bool Detonated;
    public TimeSpan EjectAt;
    public bool Ejected;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DetonatingEvacuationComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DetonatingEvacuationComputerComponent, ComponentGetState>(new ComponentEventRefHandler<DetonatingEvacuationComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DetonatingEvacuationComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<DetonatingEvacuationComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      DetonatingEvacuationComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DetonatingEvacuationComputerComponent.DetonatingEvacuationComputerComponent_AutoState()
      {
        DetonateAt = component.DetonateAt,
        Detonated = component.Detonated,
        EjectAt = component.EjectAt,
        Ejected = component.Ejected
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DetonatingEvacuationComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DetonatingEvacuationComputerComponent.DetonatingEvacuationComputerComponent_AutoState current))
        return;
      component.DetonateAt = current.DetonateAt;
      component.Detonated = current.Detonated;
      component.EjectAt = current.EjectAt;
      component.Ejected = current.Ejected;
    }
  }
}
