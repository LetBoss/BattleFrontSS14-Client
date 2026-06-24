// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.ThermalCloak.ThermalCloakComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Armor.ThermalCloak;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ThermalCloakComponent : 
  Component,
  ISerializationGenerated<ThermalCloakComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist = new EntityWhitelist();
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ForcedCooldown = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Opacity = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? CloakSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? UncloakSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RestrictWeapons;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HideNightVision = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BlockFriendlyFire = true;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<HumanoidVisualLayers> CloakedHideLayers = new HashSet<HumanoidVisualLayers>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UncloakWeaponLock = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionToggleCloak";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId CloakEffect = (EntProtoId) "RMCEffectCloak";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId UncloakEffect = (EntProtoId) "RMCEffectUncloak";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ThermalCloakComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ThermalCloakComponent) target1;
    if (serialization.TryCustomCopy<ThermalCloakComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ForcedCooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.ForcedCooldown, hookCtx, context);
    target.ForcedCooldown = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Opacity, ref target5, hookCtx, false, context))
      target5 = this.Opacity;
    target.Opacity = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CloakSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.CloakSound, hookCtx, context);
    target.CloakSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UncloakSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.UncloakSound, hookCtx, context);
    target.UncloakSound = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.RestrictWeapons, ref target8, hookCtx, false, context))
      target8 = this.RestrictWeapons;
    target.RestrictWeapons = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.HideNightVision, ref target9, hookCtx, false, context))
      target9 = this.HideNightVision;
    target.HideNightVision = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockFriendlyFire, ref target10, hookCtx, false, context))
      target10 = this.BlockFriendlyFire;
    target.BlockFriendlyFire = target10;
    HashSet<HumanoidVisualLayers> target11 = (HashSet<HumanoidVisualLayers>) null;
    if (this.CloakedHideLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(this.CloakedHideLayers, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(this.CloakedHideLayers, hookCtx, context);
    target.CloakedHideLayers = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UncloakWeaponLock, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.UncloakWeaponLock, hookCtx, context);
    target.UncloakWeaponLock = target12;
    EntProtoId target13 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target13;
    EntityUid? target14 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target14;
    EntProtoId target15 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.CloakEffect, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntProtoId>(this.CloakEffect, hookCtx, context);
    target.CloakEffect = target15;
    EntProtoId target16 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.UncloakEffect, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<EntProtoId>(this.UncloakEffect, hookCtx, context);
    target.UncloakEffect = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ThermalCloakComponent target,
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
    ThermalCloakComponent target1 = (ThermalCloakComponent) target;
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
    ThermalCloakComponent target1 = (ThermalCloakComponent) target;
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
    ThermalCloakComponent target1 = (ThermalCloakComponent) target;
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
  virtual ThermalCloakComponent Component.Instantiate() => new ThermalCloakComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ThermalCloakComponent_AutoState : IComponentState
  {
    public EntityWhitelist? Whitelist;
    public TimeSpan Cooldown;
    public TimeSpan ForcedCooldown;
    public float Opacity;
    public SoundSpecifier? CloakSound;
    public SoundSpecifier? UncloakSound;
    public bool RestrictWeapons;
    public bool HideNightVision;
    public bool BlockFriendlyFire;
    public TimeSpan UncloakWeaponLock;
    public EntProtoId ActionId;
    public NetEntity? Action;
    public EntProtoId CloakEffect;
    public EntProtoId UncloakEffect;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ThermalCloakComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ThermalCloakComponent, ComponentGetState>(new ComponentEventRefHandler<ThermalCloakComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ThermalCloakComponent, ComponentHandleState>(new ComponentEventRefHandler<ThermalCloakComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ThermalCloakComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ThermalCloakComponent.ThermalCloakComponent_AutoState()
      {
        Whitelist = component.Whitelist,
        Cooldown = component.Cooldown,
        ForcedCooldown = component.ForcedCooldown,
        Opacity = component.Opacity,
        CloakSound = component.CloakSound,
        UncloakSound = component.UncloakSound,
        RestrictWeapons = component.RestrictWeapons,
        HideNightVision = component.HideNightVision,
        BlockFriendlyFire = component.BlockFriendlyFire,
        UncloakWeaponLock = component.UncloakWeaponLock,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        CloakEffect = component.CloakEffect,
        UncloakEffect = component.UncloakEffect
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ThermalCloakComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ThermalCloakComponent.ThermalCloakComponent_AutoState current))
        return;
      component.Whitelist = current.Whitelist;
      component.Cooldown = current.Cooldown;
      component.ForcedCooldown = current.ForcedCooldown;
      component.Opacity = current.Opacity;
      component.CloakSound = current.CloakSound;
      component.UncloakSound = current.UncloakSound;
      component.RestrictWeapons = current.RestrictWeapons;
      component.HideNightVision = current.HideNightVision;
      component.BlockFriendlyFire = current.BlockFriendlyFire;
      component.UncloakWeaponLock = current.UncloakWeaponLock;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<ThermalCloakComponent>(current.Action, uid);
      component.CloakEffect = current.CloakEffect;
      component.UncloakEffect = current.UncloakEffect;
    }
  }
}
