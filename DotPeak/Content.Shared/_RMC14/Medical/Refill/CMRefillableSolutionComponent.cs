// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Refill.CMRefillableSolutionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Medical.Refill;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMRefillableSolutionSystem)})]
public sealed class CMRefillableSolutionComponent : 
  Component,
  ISerializationGenerated<CMRefillableSolutionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public string Solution = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> Reagents = new Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMRefillableSolutionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMRefillableSolutionComponent) target1;
    if (serialization.TryCustomCopy<CMRefillableSolutionComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target2, hookCtx, false, context))
      target2 = this.Solution;
    target.Solution = target2;
    Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> target3 = (Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>) null;
    if (this.Reagents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>>(this.Reagents, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>>(this.Reagents, hookCtx, context);
    target.Reagents = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMRefillableSolutionComponent target,
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
    CMRefillableSolutionComponent target1 = (CMRefillableSolutionComponent) target;
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
    CMRefillableSolutionComponent target1 = (CMRefillableSolutionComponent) target;
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
    CMRefillableSolutionComponent target1 = (CMRefillableSolutionComponent) target;
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
  virtual CMRefillableSolutionComponent Component.Instantiate()
  {
    return new CMRefillableSolutionComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMRefillableSolutionComponent_AutoState : IComponentState
  {
    public string Solution;
    public Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> Reagents;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMRefillableSolutionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMRefillableSolutionComponent, ComponentGetState>(new ComponentEventRefHandler<CMRefillableSolutionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMRefillableSolutionComponent, ComponentHandleState>(new ComponentEventRefHandler<CMRefillableSolutionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMRefillableSolutionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMRefillableSolutionComponent.CMRefillableSolutionComponent_AutoState()
      {
        Solution = component.Solution,
        Reagents = component.Reagents
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMRefillableSolutionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMRefillableSolutionComponent.CMRefillableSolutionComponent_AutoState current))
        return;
      component.Solution = current.Solution;
      component.Reagents = current.Reagents == null ? (Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>) null : new Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>((IDictionary<ProtoId<ReagentPrototype>, FixedPoint2>) current.Reagents);
    }
  }
}
