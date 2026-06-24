// Decompiled with JetBrains decompiler
// Type: Content.Client.Arcade.SpaceVillainArcadeMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Arcade;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Arcade;

public sealed class SpaceVillainArcadeMenu : DefaultWindow
{
  private readonly Label _enemyNameLabel;
  private readonly Label _playerInfoLabel;
  private readonly Label _enemyInfoLabel;
  private readonly Label _playerActionLabel;
  private readonly Label _enemyActionLabel;
  private readonly Button[] _gameButtons = new Button[3];

  public event Action<SharedSpaceVillainArcadeComponent.PlayerAction>? OnPlayerAction;

  public SpaceVillainArcadeMenu()
  {
    Vector2 vector2 = new Vector2(300f, 225f);
    ((Control) this).SetSize = vector2;
    ((Control) this).MinSize = vector2;
    this.Title = Loc.GetString("spacevillain-menu-title");
    GridContainer gridContainer1 = new GridContainer()
    {
      Columns = 1
    };
    GridContainer gridContainer2 = new GridContainer()
    {
      Columns = 3
    };
    ((Control) gridContainer2).AddChild((Control) new Label()
    {
      Text = Loc.GetString("spacevillain-menu-label-player"),
      Align = (Label.AlignMode) 1
    });
    ((Control) gridContainer2).AddChild((Control) new Label()
    {
      Text = "|",
      Align = (Label.AlignMode) 1
    });
    this._enemyNameLabel = new Label()
    {
      Align = (Label.AlignMode) 1
    };
    ((Control) gridContainer2).AddChild((Control) this._enemyNameLabel);
    this._playerInfoLabel = new Label()
    {
      Align = (Label.AlignMode) 1
    };
    ((Control) gridContainer2).AddChild((Control) this._playerInfoLabel);
    ((Control) gridContainer2).AddChild((Control) new Label()
    {
      Text = "|",
      Align = (Label.AlignMode) 1
    });
    this._enemyInfoLabel = new Label()
    {
      Align = (Label.AlignMode) 1
    };
    ((Control) gridContainer2).AddChild((Control) this._enemyInfoLabel);
    CenterContainer centerContainer1 = new CenterContainer();
    ((Control) centerContainer1).AddChild((Control) gridContainer2);
    ((Control) gridContainer1).AddChild((Control) centerContainer1);
    this._playerActionLabel = new Label()
    {
      Align = (Label.AlignMode) 1
    };
    ((Control) gridContainer1).AddChild((Control) this._playerActionLabel);
    this._enemyActionLabel = new Label()
    {
      Align = (Label.AlignMode) 1
    };
    ((Control) gridContainer1).AddChild((Control) this._enemyActionLabel);
    GridContainer gridContainer3 = new GridContainer()
    {
      Columns = 3
    };
    this._gameButtons[0] = new Button()
    {
      Text = Loc.GetString("spacevillain-menu-button-attack")
    };
    ((BaseButton) this._gameButtons[0]).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<SharedSpaceVillainArcadeComponent.PlayerAction> onPlayerAction = this.OnPlayerAction;
      if (onPlayerAction == null)
        return;
      onPlayerAction(SharedSpaceVillainArcadeComponent.PlayerAction.Attack);
    });
    ((Control) gridContainer3).AddChild((Control) this._gameButtons[0]);
    this._gameButtons[1] = new Button()
    {
      Text = Loc.GetString("spacevillain-menu-button-heal")
    };
    ((BaseButton) this._gameButtons[1]).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<SharedSpaceVillainArcadeComponent.PlayerAction> onPlayerAction = this.OnPlayerAction;
      if (onPlayerAction == null)
        return;
      onPlayerAction(SharedSpaceVillainArcadeComponent.PlayerAction.Heal);
    });
    ((Control) gridContainer3).AddChild((Control) this._gameButtons[1]);
    this._gameButtons[2] = new Button()
    {
      Text = Loc.GetString("spacevillain-menu-button-recharge")
    };
    ((BaseButton) this._gameButtons[2]).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<SharedSpaceVillainArcadeComponent.PlayerAction> onPlayerAction = this.OnPlayerAction;
      if (onPlayerAction == null)
        return;
      onPlayerAction(SharedSpaceVillainArcadeComponent.PlayerAction.Recharge);
    });
    ((Control) gridContainer3).AddChild((Control) this._gameButtons[2]);
    CenterContainer centerContainer2 = new CenterContainer();
    ((Control) centerContainer2).AddChild((Control) gridContainer3);
    ((Control) gridContainer1).AddChild((Control) centerContainer2);
    Button button = new Button()
    {
      Text = Loc.GetString("spacevillain-menu-button-new-game")
    };
    ((BaseButton) button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<SharedSpaceVillainArcadeComponent.PlayerAction> onPlayerAction = this.OnPlayerAction;
      if (onPlayerAction == null)
        return;
      onPlayerAction(SharedSpaceVillainArcadeComponent.PlayerAction.NewGame);
    });
    ((Control) gridContainer1).AddChild((Control) button);
    this.Contents.AddChild((Control) gridContainer1);
  }

  private void UpdateMetadata(
    SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage message)
  {
    this.Title = message.GameTitle;
    this._enemyNameLabel.Text = message.EnemyName;
    foreach (BaseButton gameButton in this._gameButtons)
      gameButton.Disabled = message.ButtonsDisabled;
  }

  public void UpdateInfo(
    SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage message)
  {
    if (message is SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage message1)
      this.UpdateMetadata(message1);
    this._playerInfoLabel.Text = $"HP: {message.PlayerHP} MP: {message.PlayerMP}";
    this._enemyInfoLabel.Text = $"HP: {message.EnemyHP} MP: {message.EnemyMP}";
    this._playerActionLabel.Text = message.PlayerActionMessage;
    this._enemyActionLabel.Text = message.EnemyActionMessage;
  }
}
