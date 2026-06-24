// Decompiled with JetBrains decompiler
// Type: Content.Shared.StepTrigger.Components.StepTriggerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StepTrigger.Systems;
using Content.Shared.Whitelist;
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
namespace Content.Shared.StepTrigger.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (StepTriggerSystem)})]
public sealed class StepTriggerComponent : 
  Component,
  ISerializationGenerated<StepTriggerComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public HashSet<EntityUid> Colliding = new HashSet<EntityUid>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public HashSet<EntityUid> CurrentlySteppedOn = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float IntersectRatio = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RequiredTriggeredSpeed = 3.5f;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IgnoreWeightless;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool StepOn;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StepTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StepTriggerComponent) target1;
    if (serialization.TryCustomCopy<StepTriggerComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target2, hookCtx, false, context))
      target2 = this.Active;
    target.Active = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IntersectRatio, ref target3, hookCtx, false, context))
      target3 = this.IntersectRatio;
    target.IntersectRatio = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RequiredTriggeredSpeed, ref target4, hookCtx, false, context))
      target4 = this.RequiredTriggeredSpeed;
    target.RequiredTriggeredSpeed = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target5, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target5, hookCtx, context);
    }
    target.Blacklist = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreWeightless, ref target6, hookCtx, false, context))
      target6 = this.IgnoreWeightless;
    target.IgnoreWeightless = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.StepOn, ref target7, hookCtx, false, context))
      target7 = this.StepOn;
    target.StepOn = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StepTriggerComponent target,
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
    StepTriggerComponent target1 = (StepTriggerComponent) target;
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
    StepTriggerComponent target1 = (StepTriggerComponent) target;
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
    StepTriggerComponent target1 = (StepTriggerComponent) target;
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
  virtual StepTriggerComponent Component.Instantiate() => new StepTriggerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StepTriggerComponent_AutoState : IComponentState
  {
    public HashSet<NetEntity> Colliding;
    public HashSet<NetEntity> CurrentlySteppedOn;
    public bool Active;
    public float IntersectRatio;
    public float RequiredTriggeredSpeed;
    public bool IgnoreWeightless;
    public bool StepOn;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StepTriggerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StepTriggerComponent, ComponentGetState>(new ComponentEventRefHandler<StepTriggerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StepTriggerComponent, ComponentHandleState>(new ComponentEventRefHandler<StepTriggerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StepTriggerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StepTriggerComponent.StepTriggerComponent_AutoState()
      {
        Colliding = this.GetNetEntitySet(component.Colliding),
        CurrentlySteppedOn = this.GetNetEntitySet(component.CurrentlySteppedOn),
        Active = component.Active,
        IntersectRatio = component.IntersectRatio,
        RequiredTriggeredSpeed = component.RequiredTriggeredSpeed,
        IgnoreWeightless = component.IgnoreWeightless,
        StepOn = component.StepOn
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StepTriggerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StepTriggerComponent.StepTriggerComponent_AutoState current))
        return;
      this.EnsureEntitySet<StepTriggerComponent>(current.Colliding, uid, component.Colliding);
      this.EnsureEntitySet<StepTriggerComponent>(current.CurrentlySteppedOn, uid, component.CurrentlySteppedOn);
      component.Active = current.Active;
      component.IntersectRatio = current.IntersectRatio;
      component.RequiredTriggeredSpeed = current.RequiredTriggeredSpeed;
      component.IgnoreWeightless = current.IgnoreWeightless;
      component.StepOn = current.StepOn;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, StepTriggerComponent>(uid, component, ref args1);
    }
  }
}
