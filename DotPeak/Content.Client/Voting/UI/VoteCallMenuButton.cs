// Decompiled with JetBrains decompiler
// Type: Content.Client.Voting.UI.VoteCallMenuButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.Voting.UI;

public sealed class VoteCallMenuButton : Button
{
  [Dependency]
  private IVoteManager _voteManager;

  public VoteCallMenuButton()
  {
    IoCManager.InjectDependencies<VoteCallMenuButton>(this);
    this.Text = Loc.GetString("ui-vote-menu-button");
    ((BaseButton) this).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnOnPressed);
  }

  private void OnOnPressed(BaseButton.ButtonEventArgs obj) => new VoteCallMenu().OpenCentered();

  protected virtual void EnteredTree()
  {
    ((Control) this).EnteredTree();
    this.UpdateCanCall(this._voteManager.CanCallVote);
    this._voteManager.CanCallVoteChanged += new Action<bool>(this.UpdateCanCall);
  }

  protected virtual void ExitedTree()
  {
    ((Control) this).ExitedTree();
    this._voteManager.CanCallVoteChanged += new Action<bool>(this.UpdateCanCall);
  }

  private void UpdateCanCall(bool canCall) => ((BaseButton) this).Disabled = !canCall;
}
