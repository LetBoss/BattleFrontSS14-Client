// Decompiled with JetBrains decompiler
// Type: Content.Client.Arcade.UI.BlockGameBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Arcade;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Arcade.UI;

public sealed class BlockGameBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private BlockGameMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<BlockGameMenu>((BoundUserInterface) this);
    this._menu.OnAction += new Action<BlockGamePlayerAction>(this.SendAction);
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    switch (message)
    {
      case BlockGameMessages.BlockGameVisualUpdateMessage visualUpdateMessage:
        switch (visualUpdateMessage.GameVisualType)
        {
          case BlockGameMessages.BlockGameVisualType.GameField:
            this._menu?.UpdateBlocks(visualUpdateMessage.Blocks);
            return;
          case BlockGameMessages.BlockGameVisualType.HoldBlock:
            this._menu?.UpdateHeldBlock(visualUpdateMessage.Blocks);
            return;
          case BlockGameMessages.BlockGameVisualType.NextBlock:
            this._menu?.UpdateNextBlock(visualUpdateMessage.Blocks);
            return;
          default:
            return;
        }
      case BlockGameMessages.BlockGameScoreUpdateMessage scoreUpdateMessage1:
        this._menu?.UpdatePoints(scoreUpdateMessage1.Points);
        break;
      case BlockGameMessages.BlockGameUserStatusMessage userStatusMessage:
        this._menu?.SetUsability(userStatusMessage.IsPlayer);
        break;
      case BlockGameMessages.BlockGameSetScreenMessage setScreenMessage:
        if (setScreenMessage.IsStarted)
          this._menu?.SetStarted();
        this._menu?.SetScreen(setScreenMessage.Screen);
        if (!(setScreenMessage is BlockGameMessages.BlockGameGameOverScreenMessage overScreenMessage))
          break;
        this._menu?.SetGameoverInfo(overScreenMessage.FinalScore, overScreenMessage.LocalPlacement, overScreenMessage.GlobalPlacement);
        break;
      case BlockGameMessages.BlockGameHighScoreUpdateMessage scoreUpdateMessage2:
        this._menu?.UpdateHighscores(scoreUpdateMessage2.LocalHighscores, scoreUpdateMessage2.GlobalHighscores);
        break;
      case BlockGameMessages.BlockGameLevelUpdateMessage levelUpdateMessage:
        this._menu?.UpdateLevel(levelUpdateMessage.Level);
        break;
    }
  }

  public void SendAction(BlockGamePlayerAction action)
  {
    this.SendMessage((BoundUserInterfaceMessage) new BlockGameMessages.BlockGamePlayerActionMessage(action));
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._menu)?.Orphan();
  }
}
