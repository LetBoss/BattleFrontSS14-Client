// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Parasite.TrapParasiteComponent
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
namespace Content.Shared._RMC14.Xenonids.Parasite;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class TrapParasiteComponent : 
  Component,
  ISerializationGenerated<TrapParasiteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan JumpTime = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DisableTime = TimeSpan.FromSeconds(0.25);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? LeapAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? DisableAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NormalLeapDelay;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TrapParasiteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TrapParasiteComponent) target1;
    if (serialization.TryCustomCopy<TrapParasiteComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.JumpTime, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.JumpTime, hookCtx, context);
    target.JumpTime = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DisableTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DisableTime, hookCtx, context);
    target.DisableTime = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LeapAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.LeapAt, hookCtx, context);
    target.LeapAt = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.DisableAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.DisableAt, hookCtx, context);
    target.DisableAt = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NormalLeapDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NormalLeapDelay, hookCtx, context);
    target.NormalLeapDelay = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TrapParasiteComponent target,
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
    TrapParasiteComponent target1 = (TrapParasiteComponent) target;
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
    TrapParasiteComponent target1 = (TrapParasiteComponent) target;
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
    TrapParasiteComponent target1 = (TrapParasiteComponent) target;
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
  virtual TrapParasiteComponent Component.Instantiate() => new TrapParasiteComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TrapParasiteComponent_AutoState : IComponentState
  {
    public TimeSpan JumpTime;
    public TimeSpan DisableTime;
    public TimeSpan? LeapAt;
    public TimeSpan? DisableAt;
    public TimeSpan NormalLeapDelay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TrapParasiteComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TrapParasiteComponent, ComponentGetState>(new ComponentEventRefHandler<TrapParasiteComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TrapParasiteComponent, ComponentHandleState>(new ComponentEventRefHandler<TrapParasiteComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TrapParasiteComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TrapParasiteComponent.TrapParasiteComponent_AutoState()
      {
        JumpTime = component.JumpTime,
        DisableTime = component.DisableTime,
        LeapAt = component.LeapAt,
        DisableAt = component.DisableAt,
        NormalLeapDelay = component.NormalLeapDelay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TrapParasiteComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TrapParasiteComponent.TrapParasiteComponent_AutoState current))
        return;
      component.JumpTime = current.JumpTime;
      component.DisableTime = current.DisableTime;
      component.LeapAt = current.LeapAt;
      component.DisableAt = current.DisableAt;
      component.NormalLeapDelay = current.NormalLeapDelay;
    }
  }
}
