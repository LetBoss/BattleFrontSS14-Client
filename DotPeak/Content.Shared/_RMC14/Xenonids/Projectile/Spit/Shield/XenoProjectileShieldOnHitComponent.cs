// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.Shield.XenoProjectileShieldOnHitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Shields;
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
namespace Content.Shared._RMC14.Xenonids.Projectile.Spit.Shield;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoSpitSystem)})]
public sealed class XenoProjectileShieldOnHitComponent : 
  Component,
  ISerializationGenerated<XenoProjectileShieldOnHitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoShieldSystem.ShieldType Shield = XenoShieldSystem.ShieldType.Praetorian;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Amount = (FixedPoint2) 15;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Max = (FixedPoint2) 45;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoProjectileShieldOnHitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoProjectileShieldOnHitComponent) target1;
    if (serialization.TryCustomCopy<XenoProjectileShieldOnHitComponent>(this, ref target, hookCtx, false, context))
      return;
    XenoShieldSystem.ShieldType target2 = XenoShieldSystem.ShieldType.Generic;
    if (!serialization.TryCustomCopy<XenoShieldSystem.ShieldType>(this.Shield, ref target2, hookCtx, false, context))
      target2 = this.Shield;
    target.Shield = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Amount, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.Amount, hookCtx, context);
    target.Amount = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Max, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.Max, hookCtx, context);
    target.Max = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoProjectileShieldOnHitComponent target,
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
    XenoProjectileShieldOnHitComponent target1 = (XenoProjectileShieldOnHitComponent) target;
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
    XenoProjectileShieldOnHitComponent target1 = (XenoProjectileShieldOnHitComponent) target;
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
    XenoProjectileShieldOnHitComponent target1 = (XenoProjectileShieldOnHitComponent) target;
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
  virtual XenoProjectileShieldOnHitComponent Component.Instantiate()
  {
    return new XenoProjectileShieldOnHitComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoProjectileShieldOnHitComponent_AutoState : IComponentState
  {
    public XenoShieldSystem.ShieldType Shield;
    public FixedPoint2 Amount;
    public FixedPoint2 Max;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoProjectileShieldOnHitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoProjectileShieldOnHitComponent, ComponentGetState>(new ComponentEventRefHandler<XenoProjectileShieldOnHitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoProjectileShieldOnHitComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoProjectileShieldOnHitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoProjectileShieldOnHitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoProjectileShieldOnHitComponent.XenoProjectileShieldOnHitComponent_AutoState()
      {
        Shield = component.Shield,
        Amount = component.Amount,
        Max = component.Max
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoProjectileShieldOnHitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoProjectileShieldOnHitComponent.XenoProjectileShieldOnHitComponent_AutoState current))
        return;
      component.Shield = current.Shield;
      component.Amount = current.Amount;
      component.Max = current.Max;
    }
  }
}
