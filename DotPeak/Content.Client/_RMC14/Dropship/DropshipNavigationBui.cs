// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.DropshipNavigationBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared._RMC14.Dropship;
using Content.Shared.Doors.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Timing;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Dropship;

public sealed class DropshipNavigationBui : BoundUserInterface
{
  [Dependency]
  private IEntityManager _entities;
  [Dependency]
  private IGameTiming _timing;
  [Robust.Shared.ViewVariables.ViewVariables]
  private DropshipNavigationWindow? _window;
  private readonly Dictionary<DropshipButton, string> _destinations = new Dictionary<DropshipButton, string>();
  private NetEntity? _selected;

  public DropshipNavigationBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<DropshipNavigationBui>(this);
  }

  protected virtual void Open()
  {
    base.Open();
    this.OpenWindow();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    this.OpenWindow();
    switch (state)
    {
      case DropshipNavigationDestinationsBuiState destinations:
        this.Set(destinations);
        break;
      case DropshipNavigationTravellingBuiState travelling:
        this.Set(travelling);
        break;
    }
  }

  private void OpenWindow()
  {
    if (this._window != null)
      return;
    this._window = BoundUserInterfaceExt.CreateWindow<DropshipNavigationWindow>((BoundUserInterface) this);
    this._window.OnClose += new Action(this.OnClose);
    this.SetFlightHeader("Flight Controls");
    this.SetDoorHeader("Door Controls");
    TransformComponent transformComponent;
    MetaDataComponent metaDataComponent;
    if (this._entities.TryGetComponent<TransformComponent>(this.Owner, ref transformComponent) && this._entities.TryGetComponent<MetaDataComponent>(transformComponent.ParentUid, ref metaDataComponent))
      this._window.Title = $"{metaDataComponent.EntityName} {this._window.Title}";
    ((BaseButton) this._window.CancelButton.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SetLaunchDisabled(true);
      this.SetCancelDisabled(true);
      this._selected = new NetEntity?();
      this.ResetDestinationButtons();
      this.CancelFlyby();
    });
    ((BaseButton) this._window.LaunchButton.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      if (this._selected.HasValue)
        this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipNavigationLaunchMsg(this._selected.Value));
      this.SetLaunchDisabled(true);
      this._selected = new NetEntity?();
      this.ResetDestinationButtons();
    });
    ((BaseButton) this._window.LockdownButton.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipLockdownMsg(DoorLocation.None)));
    ((BaseButton) this._window.LockdownButtonAft.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipLockdownMsg(DoorLocation.Aft)));
    ((BaseButton) this._window.LockdownButtonPort.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipLockdownMsg(DoorLocation.Port)));
    ((BaseButton) this._window.LockdownButtonStarboard.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipLockdownMsg(DoorLocation.Starboard)));
    this._entities.System<DropshipSystem>().Uis.Add(this);
  }

  private void OnClose()
  {
    this._entities.System<DropshipSystem>().Uis.Remove(this);
    this.Close();
  }

  private void Set(
    DropshipNavigationDestinationsBuiState destinations)
  {
    if (this._window == null)
      return;
    this.SetFlightHeader("Flight Controls");
    ((Control) this._window.DestinationsContainer).Visible = true;
    ((Control) this._window.ProgressBarContainer).Visible = false;
    ((Control) this._window.CancelButton).Visible = true;
    ((Control) this._window.LaunchButton).Visible = true;
    ((Control) this._window.DestinationsContainer).DisposeAllChildren();
    this._destinations.Clear();
    NetEntity? flyBy1 = destinations.FlyBy;
    if (flyBy1.HasValue)
    {
      NetEntity flyBy = flyBy1.GetValueOrDefault();
      string name = "Flyby";
      DropshipButton key = DestinationButton(name, false, (Action) (() => this._selected = new NetEntity?(flyBy)));
      this._destinations[key] = name;
      ((Control) this._window.DestinationsContainer).AddChild((Control) key);
    }
    foreach (Destination destination1 in destinations.Destinations)
    {
      Destination destination = destination1;
      string name = destination.Name;
      if (destination.Primary)
        name += " (Primary)";
      DropshipButton key = DestinationButton(name, destination.Occupied, (Action) (() => this._selected = new NetEntity?(destination.Id)));
      this._destinations[key] = name;
      ((Control) this._window.DestinationsContainer).AddChild((Control) key);
    }
    this.RefreshDoorLockStatus(destinations.DoorLockStatus);

    DropshipButton DestinationButton(string name, bool disabled, Action onPressed)
    {
      DropshipButton button = new DropshipButton();
      button.Text = name;
      button.Disabled = disabled;
      button.BorderColor = Color.Transparent;
      button.BorderThickness = new Thickness(0.0f);
      ((BaseButton) button.Button).ToggleMode = false;
      ((BaseButton) button.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this.ResetDestinationButtons();
        button.Text = "> " + name;
        this.SetLaunchDisabled(false);
        this.SetCancelDisabled(false);
        onPressed();
      });
      return button;
    }
  }

  private void Set(DropshipNavigationTravellingBuiState travelling)
  {
    if (this._window == null)
      return;
    ((Control) this._window.DestinationsContainer).Visible = false;
    ((Control) this._window.ProgressBarContainer).Visible = true;
    ((Control) this._window.LaunchButton).Visible = false;
    ((Control) this._window.ProgressBar).Margin = new Thickness(0.0f, 5f, 0.0f, 0.0f);
    if (travelling.Destination == travelling.DepartureLocation)
      ((Control) this._window.CancelButton).Visible = true;
    else
      ((Control) this._window.CancelButton).Visible = false;
    TimeSpan timeSpan = travelling.Time.End - this._timing.CurTime;
    double num = Math.Ceiling(timeSpan.TotalSeconds);
    if (num < 0.01)
      num = 0.0;
    string destination = travelling.Destination;
    switch (travelling.State)
    {
      case FTLState.Invalid:
      case FTLState.Available:
      case FTLState.Available | FTLState.Starting:
        return;
      case FTLState.Starting:
        this.SetFlightHeader("Launch in progress");
        this._window.ProgressBarHeader.SetMarkup(Msg($"Launching in T-{num}s to {destination}"));
        this.SetLockDownDisabled(false);
        break;
      case FTLState.Travelling:
        this.SetFlightHeader("In flight: " + destination);
        this._window.ProgressBarHeader.SetMarkup(Msg($"Time until destination: T-{num}s"));
        this.SetLockDownDisabled(true);
        this.SetCancelDisabled(false);
        break;
      case FTLState.Arriving:
        this.SetFlightHeader("Final Approach: " + destination);
        this._window.ProgressBarHeader.SetMarkup(Msg($"Time until landing: T-{num}s"));
        this.SetLockDownDisabled(true);
        this.SetCancelDisabled(true);
        break;
      case FTLState.Cooldown:
        this.SetFlightHeader("Refueling in progress");
        this._window.ProgressBarHeader.SetMarkup(Msg($"Ready to launch in T-{num}s"));
        this.SetLockDownDisabled(false);
        this.SetCancelDisabled(true);
        break;
      default:
        return;
    }
    this.RefreshDoorLockStatus(travelling.DoorLockStatus);
    StartEndTime time = travelling.Time;
    ((Range) this._window.ProgressBar).MinValue = 0.0f;
    ProgressBar progressBar = this._window.ProgressBar;
    timeSpan = time.Length;
    double totalSeconds = timeSpan.TotalSeconds;
    ((Range) progressBar).MaxValue = (float) totalSeconds;
    ((Range) this._window.ProgressBar).SetAsRatio(1f - time.ProgressAt(this._timing.CurTime));

    static string Msg(string msg) => $"[color=#02E74E][bold]{msg}[/bold][/color]";
  }

  private void SetFlightHeader(string label)
  {
    DropshipNavigationWindow window = this._window;
    if (window == null)
      return;
    window.Header.SetMarkup($"[color=#0BDC49][font size=16][bold]{label}[/bold][/font][/color]");
  }

  private void SetDoorHeader(string label)
  {
    DropshipNavigationWindow window = this._window;
    if (window == null)
      return;
    window.DoorHeader.SetMarkup($"[color=#0BDC49][font size=16][bold]{label}[/bold][/font][/color]");
  }

  private void SetLaunchDisabled(bool disabled)
  {
    if (this._window == null)
      return;
    ((BaseButton) this._window.LaunchButton.Button).Disabled = disabled;
  }

  private void SetCancelDisabled(bool disabled)
  {
    if (this._window == null)
      return;
    ((BaseButton) this._window.CancelButton.Button).Disabled = disabled;
  }

  private void SetLockDownDisabled(bool disabled)
  {
    if (this._window == null)
      return;
    ((BaseButton) this._window.LockdownButton.Button).Disabled = disabled;
    ((BaseButton) this._window.LockdownButtonAft.Button).Disabled = disabled;
    ((BaseButton) this._window.LockdownButtonPort.Button).Disabled = disabled;
    ((BaseButton) this._window.LockdownButtonStarboard.Button).Disabled = disabled;
  }

  private void ResetDestinationButtons()
  {
    if (this._window == null)
      return;
    foreach (Control child in ((Control) this._window.DestinationsContainer).Children)
    {
      string str;
      if (child is DropshipButton key && this._destinations.TryGetValue(key, out str))
        key.Text = str;
    }
  }

  private void CancelFlyby()
  {
    if (this._window == null)
      return;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipNavigationCancelMsg());
  }

  private void RefreshDoorLockStatus(Dictionary<DoorLocation, bool> dooorLockStatus)
  {
    if (this._window == null)
      return;
    bool flag1;
    dooorLockStatus.TryGetValue(DoorLocation.Aft, out flag1);
    bool flag2;
    dooorLockStatus.TryGetValue(DoorLocation.Port, out flag2);
    bool flag3;
    dooorLockStatus.TryGetValue(DoorLocation.Starboard, out flag3);
    this._window.LockdownButton.Text = flag1 & flag2 & flag3 ? "Lift Lockdown" : "Lockdown";
    this._window.LockdownButtonAft.Text = flag1 ? "Unlock Aft" : "Lock Aft";
    this._window.LockdownButtonPort.Text = flag2 ? "Unlock Port" : "Lock Port";
    this._window.LockdownButtonStarboard.Text = flag3 ? "Unlock Starboard" : "Lock Starboard";
  }

  public virtual void Update()
  {
    if (this._window == null || ((Control) this._window).Disposed || !(this.State is DropshipNavigationTravellingBuiState state))
      return;
    this.Set(state);
  }
}
