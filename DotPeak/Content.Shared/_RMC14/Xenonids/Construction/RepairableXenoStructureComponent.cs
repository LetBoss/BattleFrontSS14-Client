// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.RepairableXenoStructureComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Xenonids.Construction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoConstructionSystem)})]
public sealed class RepairableXenoStructureComponent : 
  Component,
  ISerializationGenerated<RepairableXenoStructureComponent>,
  ISerializationGenerated
{
  public FixedPoint2 StoredPlasma = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RepairLength = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 RepairPercent = (FixedPoint2) 1;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RepairableXenoStructureComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RepairableXenoStructureComponent) target1;
    if (serialization.TryCustomCopy<RepairableXenoStructureComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RepairLength, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.RepairLength, hookCtx, context);
    target.RepairLength = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.RepairPercent, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.RepairPercent, hookCtx, context);
    target.RepairPercent = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RepairableXenoStructureComponent target,
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
    RepairableXenoStructureComponent target1 = (RepairableXenoStructureComponent) target;
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
    RepairableXenoStructureComponent target1 = (RepairableXenoStructureComponent) target;
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
    RepairableXenoStructureComponent target1 = (RepairableXenoStructureComponent) target;
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
  virtual RepairableXenoStructureComponent Component.Instantiate()
  {
    return new RepairableXenoStructureComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RepairableXenoStructureComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public FixedPoint2 RepairPercent;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RepairableXenoStructureComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RepairableXenoStructureComponent, ComponentGetState>(new ComponentEventRefHandler<RepairableXenoStructureComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RepairableXenoStructureComponent, ComponentHandleState>(new ComponentEventRefHandler<RepairableXenoStructureComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RepairableXenoStructureComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RepairableXenoStructureComponent.RepairableXenoStructureComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        RepairPercent = component.RepairPercent
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RepairableXenoStructureComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RepairableXenoStructureComponent.RepairableXenoStructureComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.RepairPercent = current.RepairPercent;
    }
  }
}
