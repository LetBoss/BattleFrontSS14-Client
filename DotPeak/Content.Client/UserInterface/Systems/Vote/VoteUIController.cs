// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Vote.VoteUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.Voting;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Vote;

public sealed class VoteUIController : UIController
{
  [Dependency]
  private IVoteManager _votes;

  public virtual void Initialize()
  {
    base.Initialize();
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += new Action(this.OnScreenLoad);
    uiController.OnScreenUnload += new Action(this.OnScreenUnload);
  }

  private void OnScreenLoad()
  {
    switch (this.UIManager.ActiveScreen)
    {
      case DefaultGameScreen defaultGameScreen:
        this._votes.SetPopupContainer((Control) defaultGameScreen.VoteMenu);
        break;
      case SeparatedChatGameScreen separatedChatGameScreen:
        this._votes.SetPopupContainer((Control) separatedChatGameScreen.VoteMenu);
        break;
      case BattlefrontGameScreen battlefrontGameScreen:
        this._votes.SetPopupContainer((Control) battlefrontGameScreen.VoteMenu);
        break;
    }
  }

  private void OnScreenUnload() => this._votes.ClearPopupContainer();
}
