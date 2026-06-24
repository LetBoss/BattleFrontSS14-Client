// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCWeaponDamageFalloffComponent
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
public sealed class RMCWeaponDamageFalloffComponent : 
  Component,
  ISerializationGenerated<RMCWeaponDamageFalloffComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 FalloffMultiplier = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ModifiedFalloffMultiplier = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RangeFlat;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RangeFlatModified;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCWeaponDamageFalloffComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCWeaponDamageFalloffComponent) target1;
    if (serialization.TryCustomCopy<RMCWeaponDamageFalloffComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.FalloffMultiplier, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.FalloffMultiplier, hookCtx, context);
    target.FalloffMultiplier = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ModifiedFalloffMultiplier, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.ModifiedFalloffMultiplier, hookCtx, context);
    target.ModifiedFalloffMultiplier = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RangeFlat, ref target4, hookCtx, false, context))
      target4 = this.RangeFlat;
    target.RangeFlat = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RangeFlatModified, ref target5, hookCtx, false, context))
      target5 = this.RangeFlatModified;
    target.RangeFlatModified = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCWeaponDamageFalloffComponent target,
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
    RMCWeaponDamageFalloffComponent target1 = (RMCWeaponDamageFalloffComponent) target;
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
    RMCWeaponDamageFalloffComponent target1 = (RMCWeaponDamageFalloffComponent) target;
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
    RMCWeaponDamageFalloffComponent target1 = (RMCWeaponDamageFalloffComponent) target;
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
  virtual RMCWeaponDamageFalloffComponent Component.Instantiate()
  {
    return new RMCWeaponDamageFalloffComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCWeaponDamageFalloffComponent_AutoState : IComponentState
  {
    public FixedPoint2 FalloffMultiplier;
    public FixedPoint2 ModifiedFalloffMultiplier;
    public float RangeFlat;
    public float RangeFlatModified;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCWeaponDamageFalloffComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCWeaponDamageFalloffComponent, ComponentGetState>(new ComponentEventRefHandler<RMCWeaponDamageFalloffComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCWeaponDamageFalloffComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCWeaponDamageFalloffComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCWeaponDamageFalloffComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCWeaponDamageFalloffComponent.RMCWeaponDamageFalloffComponent_AutoState()
      {
        FalloffMultiplier = component.FalloffMultiplier,
        ModifiedFalloffMultiplier = component.ModifiedFalloffMultiplier,
        RangeFlat = component.RangeFlat,
        RangeFlatModified = component.RangeFlatModified
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCWeaponDamageFalloffComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCWeaponDamageFalloffComponent.RMCWeaponDamageFalloffComponent_AutoState current))
        return;
      component.FalloffMultiplier = current.FalloffMultiplier;
      component.ModifiedFalloffMultiplier = current.ModifiedFalloffMultiplier;
      component.RangeFlat = current.RangeFlat;
      component.RangeFlatModified = current.RangeFlatModified;
    }
  }
}
