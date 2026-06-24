// Decompiled with JetBrains decompiler
// Type: Content.Shared.Vehicle.Components.VehicleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Whitelist;
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
namespace Content.Shared.Vehicle.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VehicleSystem)})]
public sealed class VehicleComponent : 
  Component,
  ISerializationGenerated<VehicleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Operator;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? OperatorWhitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool TransferDamage = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageModifierSet? TransferDamageModifier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public VehicleMovementKind MovementKind;
  [DataField(null, false, 1, false, false, null)]
  public float RunoverDamage = 8f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleComponent) target1;
    if (serialization.TryCustomCopy<VehicleComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Operator, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Operator, hookCtx, context);
    target.Operator = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.OperatorWhitelist, ref target3, hookCtx, false, context))
    {
      if (this.OperatorWhitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.OperatorWhitelist, ref target3, hookCtx, context);
    }
    target.OperatorWhitelist = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.TransferDamage, ref target4, hookCtx, false, context))
      target4 = this.TransferDamage;
    target.TransferDamage = target4;
    DamageModifierSet target5 = (DamageModifierSet) null;
    if (!serialization.TryCustomCopy<DamageModifierSet>(this.TransferDamageModifier, ref target5, hookCtx, true, context))
    {
      if (this.TransferDamageModifier == null)
        target5 = (DamageModifierSet) null;
      else
        serialization.CopyTo<DamageModifierSet>(this.TransferDamageModifier, ref target5, hookCtx, context);
    }
    target.TransferDamageModifier = target5;
    VehicleMovementKind target6 = VehicleMovementKind.Standard;
    if (!serialization.TryCustomCopy<VehicleMovementKind>(this.MovementKind, ref target6, hookCtx, false, context))
      target6 = this.MovementKind;
    target.MovementKind = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RunoverDamage, ref target7, hookCtx, false, context))
      target7 = this.RunoverDamage;
    target.RunoverDamage = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleComponent target,
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
    VehicleComponent target1 = (VehicleComponent) target;
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
    VehicleComponent target1 = (VehicleComponent) target;
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
    VehicleComponent target1 = (VehicleComponent) target;
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
  virtual VehicleComponent Component.Instantiate() => new VehicleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleComponent_AutoState : IComponentState
  {
    public NetEntity? Operator;
    public EntityWhitelist? OperatorWhitelist;
    public bool TransferDamage;
    public DamageModifierSet? TransferDamageModifier;
    public VehicleMovementKind MovementKind;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, VehicleComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleComponent.VehicleComponent_AutoState()
      {
        Operator = this.GetNetEntity(component.Operator),
        OperatorWhitelist = component.OperatorWhitelist,
        TransferDamage = component.TransferDamage,
        TransferDamageModifier = component.TransferDamageModifier,
        MovementKind = component.MovementKind
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleComponent.VehicleComponent_AutoState current))
        return;
      component.Operator = this.EnsureEntity<VehicleComponent>(current.Operator, uid);
      component.OperatorWhitelist = current.OperatorWhitelist;
      component.TransferDamage = current.TransferDamage;
      component.TransferDamageModifier = current.TransferDamageModifier;
      component.MovementKind = current.MovementKind;
    }
  }
}
