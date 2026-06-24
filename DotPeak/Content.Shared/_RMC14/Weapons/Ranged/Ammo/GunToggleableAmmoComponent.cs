// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Ammo.GunToggleableAmmoComponent
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
namespace Content.Shared._RMC14.Weapons.Ranged.Ammo;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (GunToggleableAmmoSystem)})]
public sealed class GunToggleableAmmoComponent : 
  Component,
  ISerializationGenerated<GunToggleableAmmoComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public List<GunToggleableAmmoSetting> Settings = new List<GunToggleableAmmoSetting>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Setting;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionToggleAmmo";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunToggleableAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunToggleableAmmoComponent) target1;
    if (serialization.TryCustomCopy<GunToggleableAmmoComponent>(this, ref target, hookCtx, false, context))
      return;
    List<GunToggleableAmmoSetting> target2 = (List<GunToggleableAmmoSetting>) null;
    if (this.Settings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<GunToggleableAmmoSetting>>(this.Settings, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<GunToggleableAmmoSetting>>(this.Settings, hookCtx, context);
    target.Settings = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Setting, ref target3, hookCtx, false, context))
      target3 = this.Setting;
    target.Setting = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.ToggleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunToggleableAmmoComponent target,
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
    GunToggleableAmmoComponent target1 = (GunToggleableAmmoComponent) target;
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
    GunToggleableAmmoComponent target1 = (GunToggleableAmmoComponent) target;
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
    GunToggleableAmmoComponent target1 = (GunToggleableAmmoComponent) target;
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
  virtual GunToggleableAmmoComponent Component.Instantiate() => new GunToggleableAmmoComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunToggleableAmmoComponent_AutoState : IComponentState
  {
    public List<GunToggleableAmmoSetting> Settings;
    public int Setting;
    public EntProtoId ActionId;
    public NetEntity? Action;
    public SoundSpecifier ToggleSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunToggleableAmmoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunToggleableAmmoComponent, ComponentGetState>(new ComponentEventRefHandler<GunToggleableAmmoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunToggleableAmmoComponent, ComponentHandleState>(new ComponentEventRefHandler<GunToggleableAmmoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunToggleableAmmoComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunToggleableAmmoComponent.GunToggleableAmmoComponent_AutoState()
      {
        Settings = component.Settings,
        Setting = component.Setting,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        ToggleSound = component.ToggleSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunToggleableAmmoComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunToggleableAmmoComponent.GunToggleableAmmoComponent_AutoState current))
        return;
      component.Settings = current.Settings == null ? (List<GunToggleableAmmoSetting>) null : new List<GunToggleableAmmoSetting>((IEnumerable<GunToggleableAmmoSetting>) current.Settings);
      component.Setting = current.Setting;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<GunToggleableAmmoComponent>(current.Action, uid);
      component.ToggleSound = current.ToggleSound;
    }
  }
}
