// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.Components.ScatteringGrenadeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Explosion.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Explosion.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedScatteringGrenadeSystem), typeof (RMCSharedScatteringGrenadeSystem)})]
public sealed class ScatteringGrenadeComponent : 
  Component,
  ISerializationGenerated<ScatteringGrenadeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float DirectionAngle = -90f;
  [DataField(null, false, 1, false, false, null)]
  public float ReboundAngle = 180f;
  [DataField(null, false, 1, false, false, null)]
  public float SpreadAngle = 360f;
  [DataField(null, false, 1, false, false, null)]
  public bool ToggleContents;
  [DataField(null, false, 1, false, false, null)]
  public bool TriggerOnWallCollide;
  [DataField(null, false, 1, false, false, null)]
  public bool DirectHitTrigger;
  public Container Container;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? FillPrototype;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [AutoNetworkedField]
  public int UnspawnedCount;
  [DataField(null, false, 1, false, false, null)]
  public int Capacity = 3;
  [DataField(null, false, 1, false, false, null)]
  public bool TriggerContents = true;
  [DataField(null, false, 1, false, false, null)]
  public float DelayBeforeTriggerContents = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float IntervalBetweenTriggersMax;
  [DataField(null, false, 1, false, false, null)]
  public float IntervalBetweenTriggersMin;
  [DataField(null, false, 1, false, false, null)]
  public bool RandomAngle;
  [DataField(null, false, 1, false, false, null)]
  public float Velocity = 5f;
  [DataField(null, false, 1, false, false, null)]
  public float Distance = 1f;
  [DataField(null, false, 1, false, false, null)]
  public bool RandomDistance;
  [DataField(null, false, 1, false, false, null)]
  public float RandomThrowDistanceMax = 2.5f;
  [DataField(null, false, 1, false, false, null)]
  public float RandomThrowDistanceMin;
  public bool IsTriggered;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public int Count => this.UnspawnedCount + this.Container.ContainedEntities.Count;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ScatteringGrenadeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ScatteringGrenadeComponent) target1;
    if (serialization.TryCustomCopy<ScatteringGrenadeComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DirectionAngle, ref target2, hookCtx, false, context))
      target2 = this.DirectionAngle;
    target.DirectionAngle = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReboundAngle, ref target3, hookCtx, false, context))
      target3 = this.ReboundAngle;
    target.ReboundAngle = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpreadAngle, ref target4, hookCtx, false, context))
      target4 = this.SpreadAngle;
    target.SpreadAngle = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ToggleContents, ref target5, hookCtx, false, context))
      target5 = this.ToggleContents;
    target.ToggleContents = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.TriggerOnWallCollide, ref target6, hookCtx, false, context))
      target6 = this.TriggerOnWallCollide;
    target.TriggerOnWallCollide = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.DirectHitTrigger, ref target7, hookCtx, false, context))
      target7 = this.DirectHitTrigger;
    target.DirectHitTrigger = target7;
    EntityWhitelist target8 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target8, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target8 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target8, hookCtx, context);
    }
    target.Whitelist = target8;
    EntProtoId? target9 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.FillPrototype, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId?>(this.FillPrototype, hookCtx, context);
    target.FillPrototype = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.Capacity, ref target10, hookCtx, false, context))
      target10 = this.Capacity;
    target.Capacity = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.TriggerContents, ref target11, hookCtx, false, context))
      target11 = this.TriggerContents;
    target.TriggerContents = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DelayBeforeTriggerContents, ref target12, hookCtx, false, context))
      target12 = this.DelayBeforeTriggerContents;
    target.DelayBeforeTriggerContents = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IntervalBetweenTriggersMax, ref target13, hookCtx, false, context))
      target13 = this.IntervalBetweenTriggersMax;
    target.IntervalBetweenTriggersMax = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IntervalBetweenTriggersMin, ref target14, hookCtx, false, context))
      target14 = this.IntervalBetweenTriggersMin;
    target.IntervalBetweenTriggersMin = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.RandomAngle, ref target15, hookCtx, false, context))
      target15 = this.RandomAngle;
    target.RandomAngle = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Velocity, ref target16, hookCtx, false, context))
      target16 = this.Velocity;
    target.Velocity = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Distance, ref target17, hookCtx, false, context))
      target17 = this.Distance;
    target.Distance = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.RandomDistance, ref target18, hookCtx, false, context))
      target18 = this.RandomDistance;
    target.RandomDistance = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RandomThrowDistanceMax, ref target19, hookCtx, false, context))
      target19 = this.RandomThrowDistanceMax;
    target.RandomThrowDistanceMax = target19;
    float target20 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RandomThrowDistanceMin, ref target20, hookCtx, false, context))
      target20 = this.RandomThrowDistanceMin;
    target.RandomThrowDistanceMin = target20;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ScatteringGrenadeComponent target,
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
    ScatteringGrenadeComponent target1 = (ScatteringGrenadeComponent) target;
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
    ScatteringGrenadeComponent target1 = (ScatteringGrenadeComponent) target;
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
    ScatteringGrenadeComponent target1 = (ScatteringGrenadeComponent) target;
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
  virtual ScatteringGrenadeComponent Component.Instantiate() => new ScatteringGrenadeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ScatteringGrenadeComponent_AutoState : IComponentState
  {
    public int UnspawnedCount;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ScatteringGrenadeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ScatteringGrenadeComponent, ComponentGetState>(new ComponentEventRefHandler<ScatteringGrenadeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ScatteringGrenadeComponent, ComponentHandleState>(new ComponentEventRefHandler<ScatteringGrenadeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ScatteringGrenadeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ScatteringGrenadeComponent.ScatteringGrenadeComponent_AutoState()
      {
        UnspawnedCount = component.UnspawnedCount
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ScatteringGrenadeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ScatteringGrenadeComponent.ScatteringGrenadeComponent_AutoState current))
        return;
      component.UnspawnedCount = current.UnspawnedCount;
    }
  }
}
