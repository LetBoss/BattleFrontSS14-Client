// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.Components.XATDamageThresholdReachedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XATDamageThresholdReachedSystem)})]
public sealed class XATDamageThresholdReachedComponent : 
  Component,
  ISerializationGenerated<XATDamageThresholdReachedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier AccumulatedDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2> TypesNeeded = new Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2> GroupsNeeded = new Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XATDamageThresholdReachedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XATDamageThresholdReachedComponent) target1;
    if (serialization.TryCustomCopy<XATDamageThresholdReachedComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.AccumulatedDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.AccumulatedDamage, ref target2, hookCtx, false, context))
    {
      if (this.AccumulatedDamage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.AccumulatedDamage, ref target2, hookCtx, context, true);
    }
    target.AccumulatedDamage = target2;
    Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2> target3 = (Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2>) null;
    if (this.TypesNeeded == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2>>(this.TypesNeeded, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2>>(this.TypesNeeded, hookCtx, context);
    target.TypesNeeded = target3;
    Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2> target4 = (Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>) null;
    if (this.GroupsNeeded == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>>(this.GroupsNeeded, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>>(this.GroupsNeeded, hookCtx, context);
    target.GroupsNeeded = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XATDamageThresholdReachedComponent target,
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
    XATDamageThresholdReachedComponent target1 = (XATDamageThresholdReachedComponent) target;
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
    XATDamageThresholdReachedComponent target1 = (XATDamageThresholdReachedComponent) target;
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
    XATDamageThresholdReachedComponent target1 = (XATDamageThresholdReachedComponent) target;
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
  virtual XATDamageThresholdReachedComponent Component.Instantiate()
  {
    return new XATDamageThresholdReachedComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XATDamageThresholdReachedComponent_AutoState : IComponentState
  {
    public DamageSpecifier AccumulatedDamage;
    public Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2> TypesNeeded;
    public Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2> GroupsNeeded;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XATDamageThresholdReachedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XATDamageThresholdReachedComponent, ComponentGetState>(new ComponentEventRefHandler<XATDamageThresholdReachedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XATDamageThresholdReachedComponent, ComponentHandleState>(new ComponentEventRefHandler<XATDamageThresholdReachedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XATDamageThresholdReachedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XATDamageThresholdReachedComponent.XATDamageThresholdReachedComponent_AutoState()
      {
        AccumulatedDamage = component.AccumulatedDamage,
        TypesNeeded = component.TypesNeeded,
        GroupsNeeded = component.GroupsNeeded
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XATDamageThresholdReachedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XATDamageThresholdReachedComponent.XATDamageThresholdReachedComponent_AutoState current))
        return;
      component.AccumulatedDamage = current.AccumulatedDamage;
      component.TypesNeeded = current.TypesNeeded == null ? (Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2>) null : new Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2>((IDictionary<ProtoId<DamageTypePrototype>, FixedPoint2>) current.TypesNeeded);
      component.GroupsNeeded = current.GroupsNeeded == null ? (Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>) null : new Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>((IDictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>) current.GroupsNeeded);
    }
  }
}
