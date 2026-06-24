// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemToggle.Components.ItemToggleMeleeWeaponComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Item.ItemToggle.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ItemToggleMeleeWeaponComponent : 
  Component,
  ISerializationGenerated<ItemToggleMeleeWeaponComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ActivatedSoundOnHit;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DeactivatedSoundOnHit;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ActivatedSoundOnHitNoDamage;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DeactivatedSoundOnHitNoDamage;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ActivatedSoundOnSwing;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DeactivatedSoundOnSwing;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? ActivatedDamage;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? DeactivatedDamage;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DeactivatedSecret;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemToggleMeleeWeaponComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemToggleMeleeWeaponComponent) target1;
    if (serialization.TryCustomCopy<ItemToggleMeleeWeaponComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ActivatedSoundOnHit, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.ActivatedSoundOnHit, hookCtx, context);
    target.ActivatedSoundOnHit = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeactivatedSoundOnHit, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.DeactivatedSoundOnHit, hookCtx, context);
    target.DeactivatedSoundOnHit = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ActivatedSoundOnHitNoDamage, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.ActivatedSoundOnHitNoDamage, hookCtx, context);
    target.ActivatedSoundOnHitNoDamage = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeactivatedSoundOnHitNoDamage, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.DeactivatedSoundOnHitNoDamage, hookCtx, context);
    target.DeactivatedSoundOnHitNoDamage = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ActivatedSoundOnSwing, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.ActivatedSoundOnSwing, hookCtx, context);
    target.ActivatedSoundOnSwing = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeactivatedSoundOnSwing, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.DeactivatedSoundOnSwing, hookCtx, context);
    target.DeactivatedSoundOnSwing = target7;
    DamageSpecifier target8 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ActivatedDamage, ref target8, hookCtx, false, context))
    {
      if (this.ActivatedDamage == null)
        target8 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ActivatedDamage, ref target8, hookCtx, context);
    }
    target.ActivatedDamage = target8;
    DamageSpecifier target9 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.DeactivatedDamage, ref target9, hookCtx, false, context))
    {
      if (this.DeactivatedDamage == null)
        target9 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.DeactivatedDamage, ref target9, hookCtx, context);
    }
    target.DeactivatedDamage = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeactivatedSecret, ref target10, hookCtx, false, context))
      target10 = this.DeactivatedSecret;
    target.DeactivatedSecret = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemToggleMeleeWeaponComponent target,
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
    ItemToggleMeleeWeaponComponent target1 = (ItemToggleMeleeWeaponComponent) target;
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
    ItemToggleMeleeWeaponComponent target1 = (ItemToggleMeleeWeaponComponent) target;
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
    ItemToggleMeleeWeaponComponent target1 = (ItemToggleMeleeWeaponComponent) target;
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
  virtual ItemToggleMeleeWeaponComponent Component.Instantiate()
  {
    return new ItemToggleMeleeWeaponComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ItemToggleMeleeWeaponComponent_AutoState : IComponentState
  {
    public SoundSpecifier? ActivatedSoundOnHit;
    public SoundSpecifier? DeactivatedSoundOnHit;
    public SoundSpecifier? ActivatedSoundOnHitNoDamage;
    public SoundSpecifier? DeactivatedSoundOnHitNoDamage;
    public SoundSpecifier? ActivatedSoundOnSwing;
    public SoundSpecifier? DeactivatedSoundOnSwing;
    public DamageSpecifier? ActivatedDamage;
    public DamageSpecifier? DeactivatedDamage;
    public bool DeactivatedSecret;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ItemToggleMeleeWeaponComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ItemToggleMeleeWeaponComponent, ComponentGetState>(new ComponentEventRefHandler<ItemToggleMeleeWeaponComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ItemToggleMeleeWeaponComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemToggleMeleeWeaponComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ItemToggleMeleeWeaponComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ItemToggleMeleeWeaponComponent.ItemToggleMeleeWeaponComponent_AutoState()
      {
        ActivatedSoundOnHit = component.ActivatedSoundOnHit,
        DeactivatedSoundOnHit = component.DeactivatedSoundOnHit,
        ActivatedSoundOnHitNoDamage = component.ActivatedSoundOnHitNoDamage,
        DeactivatedSoundOnHitNoDamage = component.DeactivatedSoundOnHitNoDamage,
        ActivatedSoundOnSwing = component.ActivatedSoundOnSwing,
        DeactivatedSoundOnSwing = component.DeactivatedSoundOnSwing,
        ActivatedDamage = component.ActivatedDamage,
        DeactivatedDamage = component.DeactivatedDamage,
        DeactivatedSecret = component.DeactivatedSecret
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ItemToggleMeleeWeaponComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ItemToggleMeleeWeaponComponent.ItemToggleMeleeWeaponComponent_AutoState current))
        return;
      component.ActivatedSoundOnHit = current.ActivatedSoundOnHit;
      component.DeactivatedSoundOnHit = current.DeactivatedSoundOnHit;
      component.ActivatedSoundOnHitNoDamage = current.ActivatedSoundOnHitNoDamage;
      component.DeactivatedSoundOnHitNoDamage = current.DeactivatedSoundOnHitNoDamage;
      component.ActivatedSoundOnSwing = current.ActivatedSoundOnSwing;
      component.DeactivatedSoundOnSwing = current.DeactivatedSoundOnSwing;
      component.ActivatedDamage = current.ActivatedDamage;
      component.DeactivatedDamage = current.DeactivatedDamage;
      component.DeactivatedSecret = current.DeactivatedSecret;
    }
  }
}
