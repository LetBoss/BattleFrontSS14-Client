// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.SpitToggle.XenoToggleSpitComponent
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
namespace Content.Shared._RMC14.Xenonids.SpitToggle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoToggleSpitComponent : 
  Component,
  ISerializationGenerated<XenoToggleSpitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseAcid;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 NeuroCost = (FixedPoint2) 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 AcidCost = (FixedPoint2) 25;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId NeuroProto = (EntProtoId) "XenoQueenNeuroSpitProjectile";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId AcidProto = (EntProtoId) "XenoChargedSpitProjectile";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoToggleSpitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoToggleSpitComponent) target1;
    if (serialization.TryCustomCopy<XenoToggleSpitComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseAcid, ref target2, hookCtx, false, context))
      target2 = this.UseAcid;
    target.UseAcid = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.NeuroCost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.NeuroCost, hookCtx, context);
    target.NeuroCost = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.AcidCost, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.AcidCost, hookCtx, context);
    target.AcidCost = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.NeuroProto, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.NeuroProto, hookCtx, context);
    target.NeuroProto = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AcidProto, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.AcidProto, hookCtx, context);
    target.AcidProto = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoToggleSpitComponent target,
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
    XenoToggleSpitComponent target1 = (XenoToggleSpitComponent) target;
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
    XenoToggleSpitComponent target1 = (XenoToggleSpitComponent) target;
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
    XenoToggleSpitComponent target1 = (XenoToggleSpitComponent) target;
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
  virtual XenoToggleSpitComponent Component.Instantiate() => new XenoToggleSpitComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoToggleSpitComponent_AutoState : IComponentState
  {
    public bool UseAcid;
    public FixedPoint2 NeuroCost;
    public FixedPoint2 AcidCost;
    public EntProtoId NeuroProto;
    public EntProtoId AcidProto;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoToggleSpitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoToggleSpitComponent, ComponentGetState>(new ComponentEventRefHandler<XenoToggleSpitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoToggleSpitComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoToggleSpitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoToggleSpitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoToggleSpitComponent.XenoToggleSpitComponent_AutoState()
      {
        UseAcid = component.UseAcid,
        NeuroCost = component.NeuroCost,
        AcidCost = component.AcidCost,
        NeuroProto = component.NeuroProto,
        AcidProto = component.AcidProto
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoToggleSpitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoToggleSpitComponent.XenoToggleSpitComponent_AutoState current))
        return;
      component.UseAcid = current.UseAcid;
      component.NeuroCost = current.NeuroCost;
      component.AcidCost = current.AcidCost;
      component.NeuroProto = current.NeuroProto;
      component.AcidProto = current.AcidProto;
    }
  }
}
