// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.Components.XATReactiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
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
[Access(new Type[] {typeof (XATReactiveSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class XATReactiveComponent : 
  Component,
  ISerializationGenerated<XATReactiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ReactionMethod> ReactionMethods = new List<ReactionMethod>()
  {
    ReactionMethod.Touch
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<ReagentPrototype>> Reagents = new HashSet<ProtoId<ReagentPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<ReactiveGroupPrototype>> ReactiveGroups = new HashSet<ProtoId<ReactiveGroupPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MinQuantity = (FixedPoint2) 5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XATReactiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XATReactiveComponent) target1;
    if (serialization.TryCustomCopy<XATReactiveComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ReactionMethod> target2 = (List<ReactionMethod>) null;
    if (this.ReactionMethods == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ReactionMethod>>(this.ReactionMethods, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ReactionMethod>>(this.ReactionMethods, hookCtx, context);
    target.ReactionMethods = target2;
    HashSet<ProtoId<ReagentPrototype>> target3 = (HashSet<ProtoId<ReagentPrototype>>) null;
    if (this.Reagents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<ReagentPrototype>>>(this.Reagents, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<ReagentPrototype>>>(this.Reagents, hookCtx, context);
    target.Reagents = target3;
    HashSet<ProtoId<ReactiveGroupPrototype>> target4 = (HashSet<ProtoId<ReactiveGroupPrototype>>) null;
    if (this.ReactiveGroups == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<ReactiveGroupPrototype>>>(this.ReactiveGroups, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<ProtoId<ReactiveGroupPrototype>>>(this.ReactiveGroups, hookCtx, context);
    target.ReactiveGroups = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MinQuantity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.MinQuantity, hookCtx, context);
    target.MinQuantity = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XATReactiveComponent target,
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
    XATReactiveComponent target1 = (XATReactiveComponent) target;
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
    XATReactiveComponent target1 = (XATReactiveComponent) target;
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
    XATReactiveComponent target1 = (XATReactiveComponent) target;
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
  virtual XATReactiveComponent Component.Instantiate() => new XATReactiveComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XATReactiveComponent_AutoState : IComponentState
  {
    public List<ReactionMethod> ReactionMethods;
    public HashSet<ProtoId<ReagentPrototype>> Reagents;
    public HashSet<ProtoId<ReactiveGroupPrototype>> ReactiveGroups;
    public FixedPoint2 MinQuantity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XATReactiveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XATReactiveComponent, ComponentGetState>(new ComponentEventRefHandler<XATReactiveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XATReactiveComponent, ComponentHandleState>(new ComponentEventRefHandler<XATReactiveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XATReactiveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XATReactiveComponent.XATReactiveComponent_AutoState()
      {
        ReactionMethods = component.ReactionMethods,
        Reagents = component.Reagents,
        ReactiveGroups = component.ReactiveGroups,
        MinQuantity = component.MinQuantity
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XATReactiveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XATReactiveComponent.XATReactiveComponent_AutoState current))
        return;
      component.ReactionMethods = current.ReactionMethods == null ? (List<ReactionMethod>) null : new List<ReactionMethod>((IEnumerable<ReactionMethod>) current.ReactionMethods);
      component.Reagents = current.Reagents == null ? (HashSet<ProtoId<ReagentPrototype>>) null : new HashSet<ProtoId<ReagentPrototype>>((IEnumerable<ProtoId<ReagentPrototype>>) current.Reagents);
      component.ReactiveGroups = current.ReactiveGroups == null ? (HashSet<ProtoId<ReactiveGroupPrototype>>) null : new HashSet<ProtoId<ReactiveGroupPrototype>>((IEnumerable<ProtoId<ReactiveGroupPrototype>>) current.ReactiveGroups);
      component.MinQuantity = current.MinQuantity;
    }
  }
}
