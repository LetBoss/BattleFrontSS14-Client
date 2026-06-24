// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sericulture.SericultureComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Sericulture;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedSericultureSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class SericultureComponent : 
  Component,
  ISerializationGenerated<SericultureComponent>,
  ISerializationGenerated
{
  [DataField("popupText", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public string PopupText = "sericulture-failure-hunger";
  [DataField(null, false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public EntProtoId EntityProduced;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public EntProtoId Action = (EntProtoId) "ActionSericulture";
  [AutoNetworkedField]
  [DataField("actionEntity", false, 1, false, false, null)]
  public EntityUid? ActionEntity;
  [DataField("productionLength", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float ProductionLength = 3f;
  [DataField("hungerCost", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float HungerCost = 5f;
  [DataField("minHungerThreshold", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public HungerThreshold MinHungerThreshold = HungerThreshold.Okay;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SericultureComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SericultureComponent) target1;
    if (serialization.TryCustomCopy<SericultureComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.PopupText == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PopupText, ref target2, hookCtx, false, context))
      target2 = this.PopupText;
    target.PopupText = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EntityProduced, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.EntityProduced, hookCtx, context);
    target.EntityProduced = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Action, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.Action, hookCtx, context);
    target.Action = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ActionEntity, hookCtx, context);
    target.ActionEntity = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ProductionLength, ref target6, hookCtx, false, context))
      target6 = this.ProductionLength;
    target.ProductionLength = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HungerCost, ref target7, hookCtx, false, context))
      target7 = this.HungerCost;
    target.HungerCost = target7;
    HungerThreshold target8 = HungerThreshold.Dead;
    if (!serialization.TryCustomCopy<HungerThreshold>(this.MinHungerThreshold, ref target8, hookCtx, false, context))
      target8 = this.MinHungerThreshold;
    target.MinHungerThreshold = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SericultureComponent target,
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
    SericultureComponent target1 = (SericultureComponent) target;
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
    SericultureComponent target1 = (SericultureComponent) target;
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
    SericultureComponent target1 = (SericultureComponent) target;
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
  virtual SericultureComponent Component.Instantiate() => new SericultureComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SericultureComponent_AutoState : IComponentState
  {
    public string PopupText;
    public EntProtoId EntityProduced;
    public EntProtoId Action;
    public NetEntity? ActionEntity;
    public float ProductionLength;
    public float HungerCost;
    public HungerThreshold MinHungerThreshold;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SericultureComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SericultureComponent, ComponentGetState>(new ComponentEventRefHandler<SericultureComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SericultureComponent, ComponentHandleState>(new ComponentEventRefHandler<SericultureComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SericultureComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SericultureComponent.SericultureComponent_AutoState()
      {
        PopupText = component.PopupText,
        EntityProduced = component.EntityProduced,
        Action = component.Action,
        ActionEntity = this.GetNetEntity(component.ActionEntity),
        ProductionLength = component.ProductionLength,
        HungerCost = component.HungerCost,
        MinHungerThreshold = component.MinHungerThreshold
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SericultureComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SericultureComponent.SericultureComponent_AutoState current))
        return;
      component.PopupText = current.PopupText;
      component.EntityProduced = current.EntityProduced;
      component.Action = current.Action;
      component.ActionEntity = this.EnsureEntity<SericultureComponent>(current.ActionEntity, uid);
      component.ProductionLength = current.ProductionLength;
      component.HungerCost = current.HungerCost;
      component.MinHungerThreshold = current.MinHungerThreshold;
    }
  }
}
