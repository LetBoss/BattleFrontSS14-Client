// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Laser.GunToggleableLaserComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Weapons.Ranged.Laser;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class GunToggleableLaserComponent : 
  Component,
  ISerializationGenerated<GunToggleableLaserComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public List<GunToggleableLaserSetting> Settings = new List<GunToggleableLaserSetting>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Setting;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionToggleLaser";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AimDurationMultiplier = 0.6f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpottedAimDurationMultiplierSubtraction = 0.15f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunToggleableLaserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunToggleableLaserComponent) target1;
    if (serialization.TryCustomCopy<GunToggleableLaserComponent>(this, ref target, hookCtx, false, context))
      return;
    List<GunToggleableLaserSetting> target2 = (List<GunToggleableLaserSetting>) null;
    if (this.Settings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<GunToggleableLaserSetting>>(this.Settings, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<GunToggleableLaserSetting>>(this.Settings, hookCtx, context);
    target.Settings = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Setting, ref target3, hookCtx, false, context))
      target3 = this.Setting;
    target.Setting = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target4, hookCtx, false, context))
      target4 = this.Active;
    target.Active = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.ToggleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AimDurationMultiplier, ref target8, hookCtx, false, context))
      target8 = this.AimDurationMultiplier;
    target.AimDurationMultiplier = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpottedAimDurationMultiplierSubtraction, ref target9, hookCtx, false, context))
      target9 = this.SpottedAimDurationMultiplierSubtraction;
    target.SpottedAimDurationMultiplierSubtraction = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunToggleableLaserComponent target,
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
    GunToggleableLaserComponent target1 = (GunToggleableLaserComponent) target;
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
    GunToggleableLaserComponent target1 = (GunToggleableLaserComponent) target;
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
    GunToggleableLaserComponent target1 = (GunToggleableLaserComponent) target;
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
  virtual GunToggleableLaserComponent Component.Instantiate() => new GunToggleableLaserComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunToggleableLaserComponent_AutoState : IComponentState
  {
    public List<GunToggleableLaserSetting> Settings;
    public int Setting;
    public bool Active;
    public EntProtoId ActionId;
    public NetEntity? Action;
    public SoundSpecifier ToggleSound;
    public float AimDurationMultiplier;
    public float SpottedAimDurationMultiplierSubtraction;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunToggleableLaserComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunToggleableLaserComponent, ComponentGetState>(new ComponentEventRefHandler<GunToggleableLaserComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunToggleableLaserComponent, ComponentHandleState>(new ComponentEventRefHandler<GunToggleableLaserComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunToggleableLaserComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunToggleableLaserComponent.GunToggleableLaserComponent_AutoState()
      {
        Settings = component.Settings,
        Setting = component.Setting,
        Active = component.Active,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        ToggleSound = component.ToggleSound,
        AimDurationMultiplier = component.AimDurationMultiplier,
        SpottedAimDurationMultiplierSubtraction = component.SpottedAimDurationMultiplierSubtraction
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunToggleableLaserComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunToggleableLaserComponent.GunToggleableLaserComponent_AutoState current))
        return;
      component.Settings = current.Settings == null ? (List<GunToggleableLaserSetting>) null : new List<GunToggleableLaserSetting>((IEnumerable<GunToggleableLaserSetting>) current.Settings);
      component.Setting = current.Setting;
      component.Active = current.Active;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<GunToggleableLaserComponent>(current.Action, uid);
      component.ToggleSound = current.ToggleSound;
      component.AimDurationMultiplier = current.AimDurationMultiplier;
      component.SpottedAimDurationMultiplierSubtraction = current.SpottedAimDurationMultiplierSubtraction;
    }
  }
}
