// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Construction.Upgrades.RMCConstructionUpgradeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Stacks;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Construction.Upgrades;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCUpgradeSystem)})]
public sealed class RMCConstructionUpgradeComponent : 
  Component,
  ISerializationGenerated<RMCConstructionUpgradeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<StackPrototype>? Material = (ProtoId<StackPrototype>?) "CMSteel";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Amount = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId UpgradedEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId UpgradedPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId FailurePopup = (LocId) "rmc-construction-no-metal";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCConstructionUpgradeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCConstructionUpgradeComponent) target1;
    if (serialization.TryCustomCopy<RMCConstructionUpgradeComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<StackPrototype>? target2 = new ProtoId<StackPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<StackPrototype>?>(this.Material, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<StackPrototype>?>(this.Material, hookCtx, context);
    target.Material = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref target3, hookCtx, false, context))
      target3 = this.Amount;
    target.Amount = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.UpgradedEntity, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.UpgradedEntity, hookCtx, context);
    target.UpgradedEntity = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.UpgradedPopup, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.UpgradedPopup, hookCtx, context);
    target.UpgradedPopup = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.FailurePopup, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.FailurePopup, hookCtx, context);
    target.FailurePopup = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCConstructionUpgradeComponent target,
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
    RMCConstructionUpgradeComponent target1 = (RMCConstructionUpgradeComponent) target;
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
    RMCConstructionUpgradeComponent target1 = (RMCConstructionUpgradeComponent) target;
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
    RMCConstructionUpgradeComponent target1 = (RMCConstructionUpgradeComponent) target;
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
  virtual RMCConstructionUpgradeComponent Component.Instantiate()
  {
    return new RMCConstructionUpgradeComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCConstructionUpgradeComponent_AutoState : IComponentState
  {
    public ProtoId<StackPrototype>? Material;
    public int Amount;
    public EntProtoId UpgradedEntity;
    public LocId UpgradedPopup;
    public LocId FailurePopup;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCConstructionUpgradeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCConstructionUpgradeComponent, ComponentGetState>(new ComponentEventRefHandler<RMCConstructionUpgradeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCConstructionUpgradeComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCConstructionUpgradeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCConstructionUpgradeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCConstructionUpgradeComponent.RMCConstructionUpgradeComponent_AutoState()
      {
        Material = component.Material,
        Amount = component.Amount,
        UpgradedEntity = component.UpgradedEntity,
        UpgradedPopup = component.UpgradedPopup,
        FailurePopup = component.FailurePopup
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCConstructionUpgradeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCConstructionUpgradeComponent.RMCConstructionUpgradeComponent_AutoState current))
        return;
      component.Material = current.Material;
      component.Amount = current.Amount;
      component.UpgradedEntity = current.UpgradedEntity;
      component.UpgradedPopup = current.UpgradedPopup;
      component.FailurePopup = current.FailurePopup;
    }
  }
}
