// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.Announce.MarineCommunicationsComputerBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.ControlComputer;
using Content.Shared._RMC14.Overwatch;
using Content.Shared._RMC14.TacticalMap;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Marines.Announce;

public sealed class MarineCommunicationsComputerBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private MarineCommunicationsComputerWindow? _window;
  private MarineAnnounceSystem? _marineAnnounce;
  private bool _confirmingEvacuation;

  protected virtual void Open()
  {
    base.Open();
    if (this._window != null)
      return;
    this._window = BoundUserInterfaceExt.CreateWindow<MarineCommunicationsComputerWindow>((BoundUserInterface) this);
    MarineCommunicationsComputerComponent computerComponent1;
    if (this.EntMan.TryGetComponent<MarineCommunicationsComputerComponent>(this.Owner, ref computerComponent1) && computerComponent1.CanGiveMedals)
    {
      ((BaseButton) this._window.MedalButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineControlComputerMedalMsg()));
      ((Control) this._window.MedalButton).Visible = true;
    }
    else
      ((Control) this._window.MedalButton).Visible = false;
    if (this.EntMan.HasComponent<TacticalMapComputerComponent>(this.Owner))
      ((BaseButton) this._window.TacticalMapButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineCommunicationsOpenMapMsg()));
    else
      ((Control) this._window.TacticalMapButton).Visible = false;
    MarineCommunicationsComputerComponent computerComponent2;
    if (this.EntMan.TryGetComponent<MarineCommunicationsComputerComponent>(this.Owner, ref computerComponent2) && computerComponent2.CanCreateEcho)
      ((BaseButton) this._window.EchoButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineCommunicationsEchoSquadMsg()));
    MarineCommunicationsComputerComponent computerComponent3;
    if (this.EntMan.TryGetComponent<MarineCommunicationsComputerComponent>(this.Owner, ref computerComponent3) && computerComponent3.CanInitiateEvac)
    {
      ((BaseButton) this._window.EvacuationButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        if (this._confirmingEvacuation)
        {
          this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineControlComputerToggleEvacuationMsg());
          this._confirmingEvacuation = false;
        }
        else
          this._confirmingEvacuation = true;
        this.OnStateUpdate();
      });
      ((Control) this._window.EvacuationButton).Visible = true;
    }
    else
      ((Control) this._window.EvacuationButton).Visible = false;
    if (this.EntMan.HasComponent<OverwatchConsoleComponent>(this.Owner))
      ((BaseButton) this._window.OverwatchButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineCommunicationsOverwatchMsg()));
    else
      ((Control) this._window.OverwatchButton).Visible = false;
    this._window.Text.OnTextChanged += (Action<TextEdit.TextEditEventArgs>) (args => this.OnTextChanged((int) Rope.CalcTotalLength(args.TextRope)));
    ((BaseButton) this._window.Send).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineCommunicationsComputerMsg(Rope.Collapse(this._window.Text.TextRope))));
    this.OnStateUpdate();
    this.OnTextChanged(0);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state) => this.OnStateUpdate();

  public void OnStateUpdate()
  {
    if (this._window == null)
      return;
    if (this.State is MarineCommunicationsComputerBuiState state)
    {
      ((Control) this._window.LandingZonesContainer).DisposeAllChildren();
      this._window.PlanetName.Text = state.Planet;
      this._window.OperationName.Text = state.Operation;
      foreach (LandingZone landingZone in state.LandingZones)
      {
        LandingZone zone = landingZone;
        ConfirmButton confirmButton1 = new ConfirmButton();
        confirmButton1.Text = zone.Name;
        ((Control) confirmButton1).StyleClasses.Add("OpenBoth");
        ConfirmButton confirmButton2 = confirmButton1;
        confirmButton2.OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineCommunicationsDesignatePrimaryLZMsg(zone.Id)));
        ((Control) this._window.LandingZonesContainer).AddChild((Control) confirmButton2);
      }
      ((Control) this._window.LandingZonesSection).Visible = state.LandingZones.Count > 0;
    }
    MarineCommunicationsComputerComponent computerComponent1;
    ((Control) this._window.EchoButton).Visible = this.EntMan.TryGetComponent<MarineCommunicationsComputerComponent>(this.Owner, ref computerComponent1) && computerComponent1.CanCreateEcho;
    this._window.EchoSeparator.Visible = ((Control) this._window.EchoButton).Visible;
    MarineControlComputerComponent computerComponent2;
    if (!this.EntMan.TryGetComponent<MarineControlComputerComponent>(this.Owner, ref computerComponent2))
      return;
    this._window.EvacuationButton.Text = !this._confirmingEvacuation ? (computerComponent2.Evacuating ? "Cancel Evacuation" : "Initiate Evacuation") : "Confirm?";
    ((BaseButton) this._window.EvacuationButton).Disabled = !computerComponent2.CanEvacuate;
  }

  private void OnTextChanged(int textLength)
  {
    if (this._window == null)
      return;
    if (this._marineAnnounce == null)
      this._marineAnnounce = this.EntMan.System<MarineAnnounceSystem>();
    this._window.CharacterCount.Text = $"{textLength} / {this._marineAnnounce.CharacterLimit}";
    ((BaseButton) this._window.Send).Disabled = textLength == 0;
  }
}
