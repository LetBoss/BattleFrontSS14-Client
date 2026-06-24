// Decompiled with JetBrains decompiler
// Type: Content.Shared.TurretController.SharedDeployableTurretControllerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access;
using Content.Shared.Access.Systems;
using Content.Shared.Popups;
using Content.Shared.Turrets;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.TurretController;

public abstract class SharedDeployableTurretControllerSystem : EntitySystem
{
  [Dependency]
  private AccessReaderSystem _accessreader;
  [Dependency]
  private TurretTargetSettingsSystem _turretTargetingSettings;
  [Dependency]
  private SharedUserInterfaceSystem _userInterfaceSystem;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DeployableTurretControllerComponent, DeployableTurretArmamentSettingChangedMessage>(new EntityEventRefHandler<DeployableTurretControllerComponent, DeployableTurretArmamentSettingChangedMessage>(this.OnArmamentSettingChanged));
    this.SubscribeLocalEvent<DeployableTurretControllerComponent, DeployableTurretExemptAccessLevelChangedMessage>(new EntityEventRefHandler<DeployableTurretControllerComponent, DeployableTurretExemptAccessLevelChangedMessage>(this.OnExemptAccessLevelsChanged));
  }

  private void OnArmamentSettingChanged(
    Entity<DeployableTurretControllerComponent> ent,
    ref DeployableTurretArmamentSettingChangedMessage args)
  {
    if (this.IsUserAllowedAccess(ent, args.Actor))
      this.ChangeArmamentSetting(ent, args.ArmamentState, new EntityUid?(args.Actor));
    BoundUserInterface bui;
    if (!this._userInterfaceSystem.TryGetOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) DeployableTurretControllerUiKey.Key, out bui))
      return;
    bui.Update<DeployableTurretControllerBoundInterfaceState>();
  }

  private void OnExemptAccessLevelsChanged(
    Entity<DeployableTurretControllerComponent> ent,
    ref DeployableTurretExemptAccessLevelChangedMessage args)
  {
    if (this.IsUserAllowedAccess(ent, args.Actor))
      this.ChangeExemptAccessLevels(ent, args.AccessLevels, args.Enabled, new EntityUid?(args.Actor));
    BoundUserInterface bui;
    if (!this._userInterfaceSystem.TryGetOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) DeployableTurretControllerUiKey.Key, out bui))
      return;
    bui.Update<DeployableTurretControllerBoundInterfaceState>();
  }

  protected virtual void ChangeArmamentSetting(
    Entity<DeployableTurretControllerComponent> ent,
    int armamentState,
    EntityUid? user = null)
  {
    ent.Comp.ArmamentState = armamentState;
    this.Dirty<DeployableTurretControllerComponent>(ent);
    this._appearance.SetData((EntityUid) ent, (Enum) TurretControllerVisuals.ControlPanel, (object) armamentState);
  }

  protected virtual void ChangeExemptAccessLevels(
    Entity<DeployableTurretControllerComponent> ent,
    HashSet<ProtoId<AccessLevelPrototype>> exemptions,
    bool enabled,
    EntityUid? user = null)
  {
    TurretTargetSettingsComponent comp;
    if (!this.TryComp<TurretTargetSettingsComponent>((EntityUid) ent, out comp))
      return;
    Entity<TurretTargetSettingsComponent> ent1 = new Entity<TurretTargetSettingsComponent>((EntityUid) ent, comp);
    foreach (ProtoId<AccessLevelPrototype> exemption in exemptions)
    {
      if (ent.Comp.AccessLevels.Contains(exemption))
        this._turretTargetingSettings.SetAccessLevelExemption(ent1, exemption, enabled);
    }
    this.Dirty<TurretTargetSettingsComponent>(ent1);
  }

  public bool IsUserAllowedAccess(Entity<DeployableTurretControllerComponent> ent, EntityUid user)
  {
    if (this._accessreader.IsAllowed(user, (EntityUid) ent))
      return true;
    this._popup.PopupClient(this.Loc.GetString("turret-controls-access-denied"), (EntityUid) ent, new EntityUid?(user));
    this._audio.PlayPredicted(ent.Comp.AccessDeniedSound, (EntityUid) ent, new EntityUid?(user));
    return false;
  }
}
