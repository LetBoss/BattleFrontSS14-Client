// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Unrevivable.RMCRevivableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Medical.Unrevivable;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (RMCUnrevivableSystem)})]
public sealed class RMCRevivableComponent : 
  Component,
  ISerializationGenerated<RMCRevivableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool KillLarva = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UnrevivableDelay = TimeSpan.FromMinutes(5L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan UnrevivableAt = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId UnrevivableReasonMessage = (LocId) "rmc-defibrillator-unrevivable";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCRevivableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCRevivableComponent) target1;
    if (serialization.TryCustomCopy<RMCRevivableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.KillLarva, ref target2, hookCtx, false, context))
      target2 = this.KillLarva;
    target.KillLarva = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnrevivableDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.UnrevivableDelay, hookCtx, context);
    target.UnrevivableDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnrevivableAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.UnrevivableAt, hookCtx, context);
    target.UnrevivableAt = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.UnrevivableReasonMessage, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.UnrevivableReasonMessage, hookCtx, context);
    target.UnrevivableReasonMessage = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCRevivableComponent target,
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
    RMCRevivableComponent target1 = (RMCRevivableComponent) target;
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
    RMCRevivableComponent target1 = (RMCRevivableComponent) target;
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
    RMCRevivableComponent target1 = (RMCRevivableComponent) target;
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
  virtual RMCRevivableComponent Component.Instantiate() => new RMCRevivableComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCRevivableComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCRevivableComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RMCRevivableComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RMCRevivableComponent component,
      ref EntityUnpausedEvent args)
    {
      component.UnrevivableAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCRevivableComponent_AutoState : IComponentState
  {
    public bool KillLarva;
    public TimeSpan UnrevivableDelay;
    public TimeSpan UnrevivableAt;
    public LocId UnrevivableReasonMessage;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCRevivableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCRevivableComponent, ComponentGetState>(new ComponentEventRefHandler<RMCRevivableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCRevivableComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCRevivableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      RMCRevivableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCRevivableComponent.RMCRevivableComponent_AutoState()
      {
        KillLarva = component.KillLarva,
        UnrevivableDelay = component.UnrevivableDelay,
        UnrevivableAt = component.UnrevivableAt,
        UnrevivableReasonMessage = component.UnrevivableReasonMessage
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCRevivableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCRevivableComponent.RMCRevivableComponent_AutoState current))
        return;
      component.KillLarva = current.KillLarva;
      component.UnrevivableDelay = current.UnrevivableDelay;
      component.UnrevivableAt = current.UnrevivableAt;
      component.UnrevivableReasonMessage = current.UnrevivableReasonMessage;
    }
  }
}
