// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.HiveConstructionNodeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoConstructionSystem)})]
public sealed class HiveConstructionNodeComponent : 
  Component,
  ISerializationGenerated<HiveConstructionNodeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 InitialPlasmaCost;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaStored;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BlockOtherNodes = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HiveConstructionNodeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HiveConstructionNodeComponent) target1;
    if (serialization.TryCustomCopy<HiveConstructionNodeComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.InitialPlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.InitialPlasmaCost, hookCtx, context);
    target.InitialPlasmaCost = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaStored, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.PlasmaStored, hookCtx, context);
    target.PlasmaStored = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockOtherNodes, ref target6, hookCtx, false, context))
      target6 = this.BlockOtherNodes;
    target.BlockOtherNodes = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HiveConstructionNodeComponent target,
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
    HiveConstructionNodeComponent target1 = (HiveConstructionNodeComponent) target;
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
    HiveConstructionNodeComponent target1 = (HiveConstructionNodeComponent) target;
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
    HiveConstructionNodeComponent target1 = (HiveConstructionNodeComponent) target;
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
  virtual HiveConstructionNodeComponent Component.Instantiate()
  {
    return new HiveConstructionNodeComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HiveConstructionNodeComponent_AutoState : IComponentState
  {
    public FixedPoint2 InitialPlasmaCost;
    public FixedPoint2 PlasmaCost;
    public FixedPoint2 PlasmaStored;
    public EntProtoId Spawn;
    public bool BlockOtherNodes;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HiveConstructionNodeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HiveConstructionNodeComponent, ComponentGetState>(new ComponentEventRefHandler<HiveConstructionNodeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HiveConstructionNodeComponent, ComponentHandleState>(new ComponentEventRefHandler<HiveConstructionNodeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HiveConstructionNodeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HiveConstructionNodeComponent.HiveConstructionNodeComponent_AutoState()
      {
        InitialPlasmaCost = component.InitialPlasmaCost,
        PlasmaCost = component.PlasmaCost,
        PlasmaStored = component.PlasmaStored,
        Spawn = component.Spawn,
        BlockOtherNodes = component.BlockOtherNodes
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HiveConstructionNodeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HiveConstructionNodeComponent.HiveConstructionNodeComponent_AutoState current))
        return;
      component.InitialPlasmaCost = current.InitialPlasmaCost;
      component.PlasmaCost = current.PlasmaCost;
      component.PlasmaStored = current.PlasmaStored;
      component.Spawn = current.Spawn;
      component.BlockOtherNodes = current.BlockOtherNodes;
    }
  }
}
