// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.DecalPlacer.DecalPlacerUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.Sponsor;
using Content.Client.Decals.UI;
using Content.Client.Gameplay;
using Content.Client.Sandbox;
using Content.Shared.Decals;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.DecalPlacer;

public sealed class DecalPlacerUIController : 
  UIController,
  IOnStateExited<GameplayState>,
  IOnSystemChanged<SandboxSystem>,
  IOnSystemLoaded<SandboxSystem>,
  IOnSystemUnloaded<SandboxSystem>
{
  [Dependency]
  private IPrototypeManager _prototypes;
  [UISystemDependency]
  private readonly SandboxSystem _sandbox;
  private DecalPlacerWindow? _window;

  public void ToggleWindow()
  {
    this.EnsureWindow();
    if (((BaseWindow) this._window).IsOpen)
      ((BaseWindow) this._window).Close();
    else if (this._sandbox.SandboxAllowed)
    {
      ((BaseWindow) this._window).Open();
    }
    else
    {
      if (!this.EntityManager.System<SponsorSandboxSystem>().State.AllowSpawnDecals)
        return;
      ((BaseWindow) this._window).Open();
    }
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._window == null)
      return;
    if (!((Control) this._window).Disposed)
      ((Control) this._window).Orphan();
    this._window = (DecalPlacerWindow) null;
  }

  public void OnSystemLoaded(SandboxSystem system)
  {
    this._sandbox.SandboxDisabled += new Action(this.CloseWindow);
    this._prototypes.PrototypesReloaded += new Action<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded);
  }

  public void OnSystemUnloaded(SandboxSystem system)
  {
    this._sandbox.SandboxDisabled -= new Action(this.CloseWindow);
    this._prototypes.PrototypesReloaded -= new Action<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded);
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<DecalPrototype>())
      return;
    this.ReloadPrototypes();
  }

  private void ReloadPrototypes()
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._window.Populate(this._prototypes.EnumeratePrototypes<DecalPrototype>());
  }

  private void EnsureWindow()
  {
    DecalPlacerWindow window = this._window;
    if (window != null && !((Control) window).Disposed)
      return;
    this._window = this.UIManager.CreateWindow<DecalPlacerWindow>();
    LayoutContainer.SetAnchorPreset((Control) this._window, (LayoutContainer.LayoutPreset) 4, false);
    this.ReloadPrototypes();
  }

  private void CloseWindow()
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    ((BaseWindow) this._window).Close();
  }
}
