// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Commendations.CommendationsManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Commendations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Commendations;

public sealed class CommendationsManager : IPostInjectInit
{
  [Dependency]
  private INetManager _net;
  private CommendationsWindow? _receivedWindow;
  private CommendationsWindow? _givenWindow;
  private readonly List<Commendation> _commendationsReceived = new List<Commendation>();
  private readonly List<Commendation> _commendationsGiven = new List<Commendation>();

  public void PostInject()
  {
    // ISSUE: method pointer
    this._net.RegisterNetMessage<CommendationsMsg>(new ProcessMessage<CommendationsMsg>((object) this, __methodptr(OnCommendations)), (NetMessageAccept) 3);
  }

  private void OnCommendations(CommendationsMsg message)
  {
    this._commendationsReceived.Clear();
    this._commendationsReceived.AddRange((IEnumerable<Commendation>) message.CommendationsReceived.OrderByDescending<Commendation, int>((Func<Commendation, int>) (c => c.Round)));
    this._commendationsGiven.Clear();
    this._commendationsGiven.AddRange((IEnumerable<Commendation>) message.CommendationsGiven.OrderByDescending<Commendation, int>((Func<Commendation, int>) (c => c.Round)));
  }

  private void OpenWindow(
    ref CommendationsWindow? window,
    Action onClose,
    List<Commendation> commendations)
  {
    if (window != null)
    {
      ((BaseWindow) window).MoveToFront();
    }
    else
    {
      window = new CommendationsWindow();
      ((BaseWindow) window).OnClose += onClose;
      ((BaseWindow) window).OpenCentered();
      foreach (Commendation commendation in commendations)
      {
        CommendationContainer commendationContainer = new CommendationContainer();
        commendationContainer.Title.Text = $"[bold]Round {commendation.Round} - {commendation.Name}[/bold]";
        commendationContainer.Description.Text = $"Issued to [bold]{commendation.Receiver}[/bold] by [bold]{commendation.Giver}[/bold] for:\n{commendation.Text}";
        ((Control) window.Commendations).AddChild((Control) commendationContainer);
      }
    }
  }

  public void OpenReceivedWindow()
  {
    this.OpenWindow(ref this._receivedWindow, (Action) (() => this._receivedWindow = (CommendationsWindow) null), this._commendationsReceived);
  }

  public void OpenGivenWindow()
  {
    this.OpenWindow(ref this._givenWindow, (Action) (() => this._givenWindow = (CommendationsWindow) null), this._commendationsGiven);
  }
}
