// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stun.RMCStunOnTriggerComponent
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Stun;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCSizeStunSystem)})]
public sealed class RMCStunOnTriggerComponent : 
  Component,
  ISerializationGenerated<RMCStunOnTriggerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 7f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Stun;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Paralyze = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Flash = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FlashAdditionalStunTime = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Probability = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<RMCStunOnTriggerFilter>? Filters;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCStunOnTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCStunOnTriggerComponent) target1;
    if (serialization.TryCustomCopy<RMCStunOnTriggerComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Stun, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Stun, hookCtx, context);
    target.Stun = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Paralyze, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Paralyze, hookCtx, context);
    target.Paralyze = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Flash, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Flash, hookCtx, context);
    target.Flash = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FlashAdditionalStunTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.FlashAdditionalStunTime, hookCtx, context);
    target.FlashAdditionalStunTime = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Probability, ref target7, hookCtx, false, context))
      target7 = this.Probability;
    target.Probability = target7;
    List<RMCStunOnTriggerFilter> target8 = (List<RMCStunOnTriggerFilter>) null;
    if (!serialization.TryCustomCopy<List<RMCStunOnTriggerFilter>>(this.Filters, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<RMCStunOnTriggerFilter>>(this.Filters, hookCtx, context);
    target.Filters = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCStunOnTriggerComponent target,
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
    RMCStunOnTriggerComponent target1 = (RMCStunOnTriggerComponent) target;
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
    RMCStunOnTriggerComponent target1 = (RMCStunOnTriggerComponent) target;
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
    RMCStunOnTriggerComponent target1 = (RMCStunOnTriggerComponent) target;
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
  virtual RMCStunOnTriggerComponent Component.Instantiate() => new RMCStunOnTriggerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCStunOnTriggerComponent_AutoState : IComponentState
  {
    public float Range;
    public TimeSpan Stun;
    public TimeSpan Paralyze;
    public TimeSpan Flash;
    public TimeSpan FlashAdditionalStunTime;
    public float Probability;
    public List<RMCStunOnTriggerFilter>? Filters;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCStunOnTriggerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCStunOnTriggerComponent, ComponentGetState>(new ComponentEventRefHandler<RMCStunOnTriggerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCStunOnTriggerComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCStunOnTriggerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCStunOnTriggerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCStunOnTriggerComponent.RMCStunOnTriggerComponent_AutoState()
      {
        Range = component.Range,
        Stun = component.Stun,
        Paralyze = component.Paralyze,
        Flash = component.Flash,
        FlashAdditionalStunTime = component.FlashAdditionalStunTime,
        Probability = component.Probability,
        Filters = component.Filters
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCStunOnTriggerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCStunOnTriggerComponent.RMCStunOnTriggerComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.Stun = current.Stun;
      component.Paralyze = current.Paralyze;
      component.Flash = current.Flash;
      component.FlashAdditionalStunTime = current.FlashAdditionalStunTime;
      component.Probability = current.Probability;
      component.Filters = current.Filters == null ? (List<RMCStunOnTriggerFilter>) null : new List<RMCStunOnTriggerFilter>((IEnumerable<RMCStunOnTriggerFilter>) current.Filters);
    }
  }
}
