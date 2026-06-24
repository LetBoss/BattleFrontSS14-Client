// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCWeaponAccuracyComponent
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
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMGunSystem)})]
public sealed class RMCWeaponAccuracyComponent : 
  Component,
  ISerializationGenerated<RMCWeaponAccuracyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 AccuracyMultiplier = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 AccuracyMultiplierUnwielded = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ModifiedAccuracyMultiplier = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RangeFlat;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RangeFlatModified;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCWeaponAccuracyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCWeaponAccuracyComponent) target1;
    if (serialization.TryCustomCopy<RMCWeaponAccuracyComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.AccuracyMultiplier, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.AccuracyMultiplier, hookCtx, context);
    target.AccuracyMultiplier = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.AccuracyMultiplierUnwielded, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.AccuracyMultiplierUnwielded, hookCtx, context);
    target.AccuracyMultiplierUnwielded = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ModifiedAccuracyMultiplier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.ModifiedAccuracyMultiplier, hookCtx, context);
    target.ModifiedAccuracyMultiplier = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RangeFlat, ref target5, hookCtx, false, context))
      target5 = this.RangeFlat;
    target.RangeFlat = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RangeFlatModified, ref target6, hookCtx, false, context))
      target6 = this.RangeFlatModified;
    target.RangeFlatModified = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCWeaponAccuracyComponent target,
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
    RMCWeaponAccuracyComponent target1 = (RMCWeaponAccuracyComponent) target;
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
    RMCWeaponAccuracyComponent target1 = (RMCWeaponAccuracyComponent) target;
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
    RMCWeaponAccuracyComponent target1 = (RMCWeaponAccuracyComponent) target;
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
  virtual RMCWeaponAccuracyComponent Component.Instantiate() => new RMCWeaponAccuracyComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCWeaponAccuracyComponent_AutoState : IComponentState
  {
    public FixedPoint2 AccuracyMultiplier;
    public FixedPoint2 AccuracyMultiplierUnwielded;
    public FixedPoint2 ModifiedAccuracyMultiplier;
    public float RangeFlat;
    public float RangeFlatModified;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCWeaponAccuracyComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCWeaponAccuracyComponent, ComponentGetState>(new ComponentEventRefHandler<RMCWeaponAccuracyComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCWeaponAccuracyComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCWeaponAccuracyComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCWeaponAccuracyComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCWeaponAccuracyComponent.RMCWeaponAccuracyComponent_AutoState()
      {
        AccuracyMultiplier = component.AccuracyMultiplier,
        AccuracyMultiplierUnwielded = component.AccuracyMultiplierUnwielded,
        ModifiedAccuracyMultiplier = component.ModifiedAccuracyMultiplier,
        RangeFlat = component.RangeFlat,
        RangeFlatModified = component.RangeFlatModified
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCWeaponAccuracyComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCWeaponAccuracyComponent.RMCWeaponAccuracyComponent_AutoState current))
        return;
      component.AccuracyMultiplier = current.AccuracyMultiplier;
      component.AccuracyMultiplierUnwielded = current.AccuracyMultiplierUnwielded;
      component.ModifiedAccuracyMultiplier = current.ModifiedAccuracyMultiplier;
      component.RangeFlat = current.RangeFlat;
      component.RangeFlatModified = current.RangeFlatModified;
    }
  }
}
