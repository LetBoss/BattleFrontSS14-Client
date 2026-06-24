// Decompiled with JetBrains decompiler
// Type: Content.Client.TurretController.TurretControllerWindowBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Access;
using Content.Shared.TurretController;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.TurretController;

public sealed class TurretControllerWindowBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private TurretControllerWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<TurretControllerWindow>((BoundUserInterface) this);
    this._window.SetOwner(this.Owner);
    this._window.OpenCentered();
    this._window.OnAccessLevelsChangedEvent += new Action<HashSet<ProtoId<AccessLevelPrototype>>, bool>(this.OnAccessLevelChanged);
    this._window.OnArmamentSettingChangedEvent += new Action<TurretControllerWindow.TurretArmamentSetting>(this.OnArmamentSettingChanged);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is DeployableTurretControllerBoundInterfaceState state1))
      return;
    this._window?.UpdateState(state1);
  }

  private void OnAccessLevelChanged(
    HashSet<ProtoId<AccessLevelPrototype>> accessLevels,
    bool enabled)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new DeployableTurretExemptAccessLevelChangedMessage(accessLevels, enabled));
  }

  private void OnArmamentSettingChanged(
    TurretControllerWindow.TurretArmamentSetting setting)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new DeployableTurretArmamentSettingChangedMessage((int) setting));
  }
}
