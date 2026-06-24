// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Melee.MeleeReceivedMultiplierComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Melee;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCMeleeWeaponSystem)})]
public sealed class MeleeReceivedMultiplierComponent : 
  Component,
  ISerializationGenerated<MeleeReceivedMultiplierComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier XenoDamage;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 OtherMultiplier;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MeleeReceivedMultiplierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MeleeReceivedMultiplierComponent) target1;
    if (serialization.TryCustomCopy<MeleeReceivedMultiplierComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.XenoDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.XenoDamage, ref target2, hookCtx, false, context))
    {
      if (this.XenoDamage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.XenoDamage, ref target2, hookCtx, context, true);
    }
    target.XenoDamage = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.OtherMultiplier, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.OtherMultiplier, hookCtx, context);
    target.OtherMultiplier = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MeleeReceivedMultiplierComponent target,
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
    MeleeReceivedMultiplierComponent target1 = (MeleeReceivedMultiplierComponent) target;
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
    MeleeReceivedMultiplierComponent target1 = (MeleeReceivedMultiplierComponent) target;
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
    MeleeReceivedMultiplierComponent target1 = (MeleeReceivedMultiplierComponent) target;
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
  virtual MeleeReceivedMultiplierComponent Component.Instantiate()
  {
    return new MeleeReceivedMultiplierComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MeleeReceivedMultiplierComponent_AutoState : IComponentState
  {
    public DamageSpecifier XenoDamage;
    public FixedPoint2 OtherMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MeleeReceivedMultiplierComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MeleeReceivedMultiplierComponent, ComponentGetState>(new ComponentEventRefHandler<MeleeReceivedMultiplierComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MeleeReceivedMultiplierComponent, ComponentHandleState>(new ComponentEventRefHandler<MeleeReceivedMultiplierComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MeleeReceivedMultiplierComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MeleeReceivedMultiplierComponent.MeleeReceivedMultiplierComponent_AutoState()
      {
        XenoDamage = component.XenoDamage,
        OtherMultiplier = component.OtherMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MeleeReceivedMultiplierComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MeleeReceivedMultiplierComponent.MeleeReceivedMultiplierComponent_AutoState current))
        return;
      component.XenoDamage = current.XenoDamage;
      component.OtherMultiplier = current.OtherMultiplier;
    }
  }
}
