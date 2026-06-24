// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Ammo.PubgAmmoUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Ammo;

public sealed class PubgAmmoUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  private const int AmmoAnchorMargin = 15;
  private const float AmmoMarginBottom = -200f;
  private const float AmmoMarginLeft = 70f;
  private bool _ammoVisible;
  private bool _systemSubscribed;
  private PanelContainer? _ammoPanel;
  private Label? _currentAmmoLabel;
  private Label? _reserveAmmoLabel;
  private Label? _ammoTypeLabel;
  private bool _hasSnapshot;
  private int _snapshotCurrent;
  private int _snapshotReserve;
  private string _snapshotType = "";

  public virtual void Initialize()
  {
    base.Initialize();
    this.UIManager.GetUIController<GameplayStateLoadController>().OnScreenLoad += (Action) (() =>
    {
      this.RequestRefresh();
      if (!this._ammoVisible)
        return;
      this.EnsureUI();
    });
  }

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureSystemSubscribed();
    this.RequestRefresh();
    if (!this._ammoVisible)
      return;
    this.EnsureUI();
  }

  public void OnStateExited(GameplayState state)
  {
    this.UnsubscribeFromSystem();
    ((Control) this._ammoPanel)?.Orphan();
    this._ammoPanel = (PanelContainer) null;
    this._currentAmmoLabel = (Label) null;
    this._reserveAmmoLabel = (Label) null;
    this._ammoTypeLabel = (Label) null;
  }

  private void EnsureSystemSubscribed()
  {
    if (this._systemSubscribed)
      return;
    this.EntityManager.System<PubgAmmoUiSystem>().AmmoUpdated += new Action<PubgAmmoUpdateEvent>(this.OnAmmoUpdate);
    this._systemSubscribed = true;
  }

  private void UnsubscribeFromSystem()
  {
    if (!this._systemSubscribed)
      return;
    PubgAmmoUiSystem pubgAmmoUiSystem = this.EntityManager.SystemOrNull<PubgAmmoUiSystem>();
    if (pubgAmmoUiSystem != null)
      pubgAmmoUiSystem.AmmoUpdated -= new Action<PubgAmmoUpdateEvent>(this.OnAmmoUpdate);
    this._systemSubscribed = false;
  }

  private void RequestRefresh()
  {
    this.EnsureSystemSubscribed();
    this.EntityManager.System<PubgAmmoUiSystem>().RequestRefresh();
  }

  private void EnsureUI()
  {
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
      return;
    if (activeScreen.GetWidget<MainViewport>() == null)
      return;
    LayoutContainer control;
    try
    {
      control = ((Control) activeScreen).FindControl<LayoutContainer>("ViewportContainer");
    }
    catch (ArgumentException ex)
    {
      return;
    }
    if (this._ammoPanel == null)
    {
      BoxContainer boxContainer = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0,
        SeparationOverride = new int?(5)
      };
      Label label1 = new Label();
      label1.Text = "30";
      label1.FontColorOverride = new Color?(Color.White);
      ((Control) label1).MinWidth = 60f;
      ((Control) label1).HorizontalAlignment = (Control.HAlignment) 3;
      this._currentAmmoLabel = label1;
      ((Control) this._currentAmmoLabel).SetOnlyStyleClass("LabelHeading");
      Label label2 = new Label()
      {
        Text = "/",
        FontColorOverride = new Color?(Color.Gray)
      };
      Label label3 = new Label();
      label3.Text = "90";
      label3.FontColorOverride = new Color?(Color.Gray);
      ((Control) label3).MinWidth = 60f;
      this._reserveAmmoLabel = label3;
      Label label4 = new Label();
      label4.Text = "";
      label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#ffa500", new Color?()));
      ((Control) label4).Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
      this._ammoTypeLabel = label4;
      ((Control) boxContainer).AddChild((Control) this._currentAmmoLabel);
      ((Control) boxContainer).AddChild((Control) label2);
      ((Control) boxContainer).AddChild((Control) this._reserveAmmoLabel);
      ((Control) boxContainer).AddChild((Control) this._ammoTypeLabel);
      this._ammoPanel = new PanelContainer();
      StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
      {
        BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1a1a1aDD", new Color?()),
        BorderColor = Color.FromHex((ReadOnlySpan<char>) "#ffa500", new Color?()),
        BorderThickness = new Thickness(0.0f, 0.0f, 3f, 0.0f)
      };
      ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 10f);
      this._ammoPanel.PanelOverride = (StyleBox) styleBoxFlat;
      ((Control) this._ammoPanel).AddChild((Control) boxContainer);
      ((Control) control).AddChild((Control) this._ammoPanel);
      this.ApplyAmmoLayout(this._ammoPanel);
    }
    if (((Control) this._ammoPanel)?.Parent != control)
    {
      ((Control) this._ammoPanel)?.Orphan();
      if (this._ammoPanel != null)
      {
        ((Control) control).AddChild((Control) this._ammoPanel);
        this.ApplyAmmoLayout(this._ammoPanel);
      }
    }
    ((Control) this._ammoPanel)?.SetPositionLast();
    this.ApplySnapshotToLabels();
  }

  private void ApplySnapshotToLabels()
  {
    if (!this._hasSnapshot)
      return;
    if (this._currentAmmoLabel != null)
      this._currentAmmoLabel.Text = this._snapshotCurrent.ToString();
    if (this._reserveAmmoLabel != null)
      this._reserveAmmoLabel.Text = this._snapshotReserve.ToString();
    if (this._ammoTypeLabel == null)
      return;
    this._ammoTypeLabel.Text = this._snapshotType;
  }

  private void ApplyAmmoLayout(PanelContainer panel)
  {
    LayoutContainer.SetAnchorAndMarginPreset((Control) panel, (LayoutContainer.LayoutPreset) 2, (LayoutContainer.LayoutPresetMode) 0, 15);
    LayoutContainer.SetMarginBottom((Control) panel, -200f);
    LayoutContainer.SetMarginLeft((Control) panel, 70f);
  }

  private void OnAmmoUpdate(PubgAmmoUpdateEvent msg)
  {
    if (msg.CurrentAmmo == 0 && msg.MaxAmmo == 0 && msg.ReserveAmmo == 0)
    {
      this.HideAmmoUI();
    }
    else
    {
      this._hasSnapshot = true;
      this._snapshotCurrent = msg.CurrentAmmo;
      this._snapshotReserve = msg.ReserveAmmo;
      this._snapshotType = msg.AmmoType;
      this._ammoVisible = true;
      this.EnsureUI();
      int num;
      if (this._currentAmmoLabel != null)
      {
        Label currentAmmoLabel = this._currentAmmoLabel;
        num = msg.CurrentAmmo;
        string str = num.ToString();
        currentAmmoLabel.Text = str;
      }
      if (this._reserveAmmoLabel != null)
      {
        Label reserveAmmoLabel = this._reserveAmmoLabel;
        num = msg.ReserveAmmo;
        string str = num.ToString();
        reserveAmmoLabel.Text = str;
      }
      if (this._ammoTypeLabel == null)
        return;
      this._ammoTypeLabel.Text = msg.AmmoType;
    }
  }

  public void HideAmmoUI()
  {
    this._ammoVisible = false;
    this._hasSnapshot = false;
    ((Control) this._ammoPanel)?.Orphan();
    this._ammoPanel = (PanelContainer) null;
    this._currentAmmoLabel = (Label) null;
    this._reserveAmmoLabel = (Label) null;
    this._ammoTypeLabel = (Label) null;
  }
}
