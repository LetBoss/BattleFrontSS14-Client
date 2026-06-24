// Decompiled with JetBrains decompiler
// Type: Content.Client.Arcade.BlockGameMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Content.Shared.Arcade;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

#nullable enable
namespace Content.Client.Arcade;

public sealed class BlockGameMenu : DefaultWindow
{
  private static readonly Color OverlayBackgroundColor = new Color((byte) 74, (byte) 74, (byte) 81, (byte) 180);
  private static readonly Color OverlayShadowColor = new Color((byte) 0, (byte) 0, (byte) 0, (byte) 83);
  private static readonly Vector2 BlockSize = new Vector2(15f, 15f);
  private readonly PanelContainer _mainPanel;
  private readonly BoxContainer _gameRootContainer;
  private GridContainer _gameGrid;
  private GridContainer _nextBlockGrid;
  private GridContainer _holdBlockGrid;
  private readonly Label _pointsLabel;
  private readonly Label _levelLabel;
  private readonly Button _pauseButton;
  private readonly PanelContainer _menuRootContainer;
  private readonly Button _unpauseButton;
  private readonly Control _unpauseButtonMargin;
  private readonly Button _newGameButton;
  private readonly Button _scoreBoardButton;
  private readonly PanelContainer _gameOverRootContainer;
  private readonly Label _finalScoreLabel;
  private readonly Button _finalNewGameButton;
  private readonly PanelContainer _highscoresRootContainer;
  private readonly Label _localHighscoresLabel;
  private readonly Label _globalHighscoresLabel;
  private readonly Button _highscoreBackButton;
  private bool _isPlayer;
  private bool _gameOver;

  public event Action<BlockGamePlayerAction>? OnAction;

  public BlockGameMenu()
  {
    this.Title = Loc.GetString("blockgame-menu-title");
    Vector2 vector2 = new Vector2(410f, 490f);
    ((Control) this).SetSize = vector2;
    ((Control) this).MinSize = vector2;
    Texture texture = IoCManager.Resolve<IResourceCache>().GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
    this._mainPanel = new PanelContainer();
    this._gameRootContainer = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    Label label1 = new Label();
    label1.Align = (Label.AlignMode) 1;
    ((Control) label1).HorizontalExpand = true;
    this._levelLabel = label1;
    ((Control) this._gameRootContainer).AddChild((Control) this._levelLabel);
    ((Control) this._gameRootContainer).AddChild(new Control()
    {
      MinSize = new Vector2(1f, 5f)
    });
    Label label2 = new Label();
    label2.Align = (Label.AlignMode) 1;
    ((Control) label2).HorizontalExpand = true;
    this._pointsLabel = label2;
    ((Control) this._gameRootContainer).AddChild((Control) this._pointsLabel);
    ((Control) this._gameRootContainer).AddChild(new Control()
    {
      MinSize = new Vector2(1f, 10f)
    });
    BoxContainer boxContainer1 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    ((Control) boxContainer1).AddChild(this.SetupHoldBox(texture));
    ((Control) boxContainer1).AddChild(new Control()
    {
      MinSize = new Vector2(10f, 1f)
    });
    ((Control) boxContainer1).AddChild(this.SetupGameGrid(texture));
    ((Control) boxContainer1).AddChild(new Control()
    {
      MinSize = new Vector2(10f, 1f)
    });
    ((Control) boxContainer1).AddChild(this.SetupNextBox(texture));
    ((Control) this._gameRootContainer).AddChild((Control) boxContainer1);
    ((Control) this._gameRootContainer).AddChild(new Control()
    {
      MinSize = new Vector2(1f, 10f)
    });
    this._pauseButton = new Button()
    {
      Text = Loc.GetString("blockgame-menu-button-pause"),
      TextAlign = (Label.AlignMode) 1
    };
    ((BaseButton) this._pauseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (e => this.TryPause());
    ((Control) this._gameRootContainer).AddChild((Control) this._pauseButton);
    ((Control) this._mainPanel).AddChild((Control) this._gameRootContainer);
    StyleBoxTexture styleBoxTexture1 = new StyleBoxTexture()
    {
      Texture = texture,
      Modulate = BlockGameMenu.OverlayShadowColor
    };
    styleBoxTexture1.SetPatchMargin((StyleBox.Margin) 15, 10f);
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) styleBoxTexture1;
    ((Control) panelContainer1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) panelContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    this._menuRootContainer = panelContainer1;
    StyleBoxTexture styleBoxTexture2 = new StyleBoxTexture()
    {
      Texture = texture,
      Modulate = BlockGameMenu.OverlayBackgroundColor
    };
    styleBoxTexture2.SetPatchMargin((StyleBox.Margin) 15, 10f);
    PanelContainer panelContainer2 = new PanelContainer();
    panelContainer2.PanelOverride = (StyleBox) styleBoxTexture2;
    ((Control) panelContainer2).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) panelContainer2).HorizontalAlignment = (Control.HAlignment) 2;
    PanelContainer panelContainer3 = panelContainer2;
    ((Control) this._menuRootContainer).AddChild((Control) panelContainer3);
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer2).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer2).VerticalAlignment = (Control.VAlignment) 2;
    BoxContainer boxContainer3 = boxContainer2;
    this._newGameButton = new Button()
    {
      Text = Loc.GetString("blockgame-menu-button-new-game"),
      TextAlign = (Label.AlignMode) 1
    };
    ((BaseButton) this._newGameButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (e =>
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.NewGame);
    });
    ((Control) boxContainer3).AddChild((Control) this._newGameButton);
    ((Control) boxContainer3).AddChild(new Control()
    {
      MinSize = new Vector2(1f, 10f)
    });
    this._scoreBoardButton = new Button()
    {
      Text = Loc.GetString("blockgame-menu-button-scoreboard"),
      TextAlign = (Label.AlignMode) 1
    };
    ((BaseButton) this._scoreBoardButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (e =>
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.ShowHighscores);
    });
    ((Control) boxContainer3).AddChild((Control) this._scoreBoardButton);
    this._unpauseButtonMargin = new Control()
    {
      MinSize = new Vector2(1f, 10f),
      Visible = false
    };
    ((Control) boxContainer3).AddChild(this._unpauseButtonMargin);
    Button button = new Button();
    button.Text = Loc.GetString("blockgame-menu-button-unpause");
    button.TextAlign = (Label.AlignMode) 1;
    ((Control) button).Visible = false;
    this._unpauseButton = button;
    ((BaseButton) this._unpauseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (e =>
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.Unpause);
    });
    ((Control) boxContainer3).AddChild((Control) this._unpauseButton);
    ((Control) panelContainer3).AddChild((Control) boxContainer3);
    StyleBoxTexture styleBoxTexture3 = new StyleBoxTexture()
    {
      Texture = texture,
      Modulate = BlockGameMenu.OverlayShadowColor
    };
    styleBoxTexture3.SetPatchMargin((StyleBox.Margin) 15, 10f);
    PanelContainer panelContainer4 = new PanelContainer();
    panelContainer4.PanelOverride = (StyleBox) styleBoxTexture3;
    ((Control) panelContainer4).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) panelContainer4).HorizontalAlignment = (Control.HAlignment) 2;
    this._gameOverRootContainer = panelContainer4;
    StyleBoxTexture styleBoxTexture4 = new StyleBoxTexture()
    {
      Texture = texture,
      Modulate = BlockGameMenu.OverlayBackgroundColor
    };
    styleBoxTexture4.SetPatchMargin((StyleBox.Margin) 15, 10f);
    PanelContainer panelContainer5 = new PanelContainer();
    panelContainer5.PanelOverride = (StyleBox) styleBoxTexture4;
    ((Control) panelContainer5).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) panelContainer5).HorizontalAlignment = (Control.HAlignment) 2;
    PanelContainer panelContainer6 = panelContainer5;
    ((Control) this._gameOverRootContainer).AddChild((Control) panelContainer6);
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer4).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer4).VerticalAlignment = (Control.VAlignment) 2;
    BoxContainer boxContainer5 = boxContainer4;
    ((Control) boxContainer5).AddChild((Control) new Label()
    {
      Text = Loc.GetString("blockgame-menu-msg-game-over"),
      Align = (Label.AlignMode) 1
    });
    ((Control) boxContainer5).AddChild(new Control()
    {
      MinSize = new Vector2(1f, 10f)
    });
    this._finalScoreLabel = new Label()
    {
      Align = (Label.AlignMode) 1
    };
    ((Control) boxContainer5).AddChild((Control) this._finalScoreLabel);
    ((Control) boxContainer5).AddChild(new Control()
    {
      MinSize = new Vector2(1f, 10f)
    });
    this._finalNewGameButton = new Button()
    {
      Text = Loc.GetString("blockgame-menu-button-new-game"),
      TextAlign = (Label.AlignMode) 1
    };
    ((BaseButton) this._finalNewGameButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (e =>
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.NewGame);
    });
    ((Control) boxContainer5).AddChild((Control) this._finalNewGameButton);
    ((Control) panelContainer6).AddChild((Control) boxContainer5);
    StyleBoxTexture styleBoxTexture5 = new StyleBoxTexture()
    {
      Texture = texture,
      Modulate = BlockGameMenu.OverlayShadowColor
    };
    styleBoxTexture5.SetPatchMargin((StyleBox.Margin) 15, 10f);
    PanelContainer panelContainer7 = new PanelContainer();
    panelContainer7.PanelOverride = (StyleBox) styleBoxTexture5;
    ((Control) panelContainer7).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) panelContainer7).HorizontalAlignment = (Control.HAlignment) 2;
    this._highscoresRootContainer = panelContainer7;
    Color color;
    // ISSUE: explicit constructor call
    ((Color) ref color).\u002Ector(BlockGameMenu.OverlayBackgroundColor.R, BlockGameMenu.OverlayBackgroundColor.G, BlockGameMenu.OverlayBackgroundColor.B, 220f);
    StyleBoxTexture styleBoxTexture6 = new StyleBoxTexture()
    {
      Texture = texture,
      Modulate = color
    };
    styleBoxTexture6.SetPatchMargin((StyleBox.Margin) 15, 10f);
    PanelContainer panelContainer8 = new PanelContainer();
    panelContainer8.PanelOverride = (StyleBox) styleBoxTexture6;
    ((Control) panelContainer8).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) panelContainer8).HorizontalAlignment = (Control.HAlignment) 2;
    PanelContainer panelContainer9 = panelContainer8;
    ((Control) this._highscoresRootContainer).AddChild((Control) panelContainer9);
    BoxContainer boxContainer6 = new BoxContainer();
    boxContainer6.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer6).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer6).VerticalAlignment = (Control.VAlignment) 2;
    BoxContainer boxContainer7 = boxContainer6;
    ((Control) boxContainer7).AddChild((Control) new Label()
    {
      Text = Loc.GetString("blockgame-menu-label-highscores")
    });
    ((Control) boxContainer7).AddChild(new Control()
    {
      MinSize = new Vector2(1f, 10f)
    });
    BoxContainer boxContainer8 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    this._localHighscoresLabel = new Label()
    {
      Align = (Label.AlignMode) 1
    };
    ((Control) boxContainer8).AddChild((Control) this._localHighscoresLabel);
    ((Control) boxContainer8).AddChild(new Control()
    {
      MinSize = new Vector2(40f, 1f)
    });
    this._globalHighscoresLabel = new Label()
    {
      Align = (Label.AlignMode) 1
    };
    ((Control) boxContainer8).AddChild((Control) this._globalHighscoresLabel);
    ((Control) boxContainer7).AddChild((Control) boxContainer8);
    ((Control) boxContainer7).AddChild(new Control()
    {
      MinSize = new Vector2(1f, 10f)
    });
    this._highscoreBackButton = new Button()
    {
      Text = Loc.GetString("blockgame-menu-button-back"),
      TextAlign = (Label.AlignMode) 1
    };
    ((BaseButton) this._highscoreBackButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (e =>
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.Pause);
    });
    ((Control) boxContainer7).AddChild((Control) this._highscoreBackButton);
    ((Control) panelContainer9).AddChild((Control) boxContainer7);
    this.Contents.AddChild((Control) this._mainPanel);
    ((Control) this).CanKeyboardFocus = true;
  }

  public void SetUsability(bool isPlayer)
  {
    this._isPlayer = isPlayer;
    this.UpdateUsability();
  }

  private void UpdateUsability()
  {
    ((BaseButton) this._pauseButton).Disabled = !this._isPlayer;
    ((BaseButton) this._newGameButton).Disabled = !this._isPlayer;
    ((BaseButton) this._scoreBoardButton).Disabled = !this._isPlayer;
    ((BaseButton) this._unpauseButton).Disabled = !this._isPlayer;
    ((BaseButton) this._finalNewGameButton).Disabled = !this._isPlayer;
    ((BaseButton) this._highscoreBackButton).Disabled = !this._isPlayer;
  }

  private Control SetupGameGrid(Texture panelTex)
  {
    this._gameGrid = new GridContainer()
    {
      Columns = 10,
      HSeparationOverride = new int?(1),
      VSeparationOverride = new int?(1)
    };
    this.UpdateBlocks(Array.Empty<BlockGameBlock>());
    StyleBoxTexture styleBoxTexture = new StyleBoxTexture()
    {
      Texture = panelTex,
      Modulate = Color.FromHex((ReadOnlySpan<char>) "#4a4a51", new Color?())
    };
    styleBoxTexture.SetPatchMargin((StyleBox.Margin) 15, 10f);
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) styleBoxTexture;
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).SizeFlagsStretchRatio = 34.25f;
    PanelContainer panelContainer2 = new PanelContainer()
    {
      PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#86868d", new Color?())
      }
    };
    ((Control) panelContainer2).AddChild((Control) this._gameGrid);
    ((Control) panelContainer1).AddChild((Control) panelContainer2);
    return (Control) panelContainer1;
  }

  private Control SetupNextBox(Texture panelTex)
  {
    StyleBoxTexture styleBoxTexture = new StyleBoxTexture()
    {
      Texture = panelTex,
      Modulate = Color.FromHex((ReadOnlySpan<char>) "#4a4a51", new Color?())
    };
    styleBoxTexture.SetPatchMargin((StyleBox.Margin) 15, 10f);
    GridContainer gridContainer = new GridContainer();
    gridContainer.Columns = 1;
    ((Control) gridContainer).HorizontalExpand = true;
    ((Control) gridContainer).SizeFlagsStretchRatio = 20f;
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) styleBoxTexture;
    ((Control) panelContainer1).MinSize = BlockGameMenu.BlockSize * 6.5f;
    ((Control) panelContainer1).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) panelContainer1).VerticalAlignment = (Control.VAlignment) 1;
    PanelContainer panelContainer2 = panelContainer1;
    CenterContainer centerContainer = new CenterContainer();
    this._nextBlockGrid = new GridContainer()
    {
      HSeparationOverride = new int?(1),
      VSeparationOverride = new int?(1)
    };
    ((Control) centerContainer).AddChild((Control) this._nextBlockGrid);
    ((Control) panelContainer2).AddChild((Control) centerContainer);
    ((Control) gridContainer).AddChild((Control) panelContainer2);
    ((Control) gridContainer).AddChild((Control) new Label()
    {
      Text = Loc.GetString("blockgame-menu-label-next"),
      Align = (Label.AlignMode) 1
    });
    return (Control) gridContainer;
  }

  private Control SetupHoldBox(Texture panelTex)
  {
    StyleBoxTexture styleBoxTexture = new StyleBoxTexture()
    {
      Texture = panelTex,
      Modulate = Color.FromHex((ReadOnlySpan<char>) "#4a4a51", new Color?())
    };
    styleBoxTexture.SetPatchMargin((StyleBox.Margin) 15, 10f);
    GridContainer gridContainer = new GridContainer();
    gridContainer.Columns = 1;
    ((Control) gridContainer).HorizontalExpand = true;
    ((Control) gridContainer).SizeFlagsStretchRatio = 20f;
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) styleBoxTexture;
    ((Control) panelContainer1).MinSize = BlockGameMenu.BlockSize * 6.5f;
    ((Control) panelContainer1).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) panelContainer1).VerticalAlignment = (Control.VAlignment) 1;
    PanelContainer panelContainer2 = panelContainer1;
    CenterContainer centerContainer = new CenterContainer();
    this._holdBlockGrid = new GridContainer()
    {
      HSeparationOverride = new int?(1),
      VSeparationOverride = new int?(1)
    };
    ((Control) centerContainer).AddChild((Control) this._holdBlockGrid);
    ((Control) panelContainer2).AddChild((Control) centerContainer);
    ((Control) gridContainer).AddChild((Control) panelContainer2);
    ((Control) gridContainer).AddChild((Control) new Label()
    {
      Text = Loc.GetString("blockgame-menu-label-hold"),
      Align = (Label.AlignMode) 1
    });
    return (Control) gridContainer;
  }

  protected virtual void KeyboardFocusExited()
  {
    if (!((BaseWindow) this).IsOpen || this._gameOver)
      return;
    this.TryPause();
  }

  private void TryPause()
  {
    Action<BlockGamePlayerAction> onAction = this.OnAction;
    if (onAction == null)
      return;
    onAction(BlockGamePlayerAction.Pause);
  }

  public void SetStarted()
  {
    this._gameOver = false;
    ((Control) this._unpauseButton).Visible = true;
    this._unpauseButtonMargin.Visible = true;
  }

  public void SetScreen(BlockGameMessages.BlockGameScreen screen)
  {
    if (this._gameOver)
      return;
    switch (screen)
    {
      case BlockGameMessages.BlockGameScreen.Game:
        ((Control) this).GrabKeyboardFocus();
        this.CloseMenus();
        ((BaseButton) this._pauseButton).Disabled = !this._isPlayer;
        break;
      case BlockGameMessages.BlockGameScreen.Pause:
        this.CloseMenus();
        ((Control) this._mainPanel).AddChild((Control) this._menuRootContainer);
        ((BaseButton) this._pauseButton).Disabled = true;
        break;
      case BlockGameMessages.BlockGameScreen.Gameover:
        this._gameOver = true;
        ((BaseButton) this._pauseButton).Disabled = true;
        this.CloseMenus();
        ((Control) this._mainPanel).AddChild((Control) this._gameOverRootContainer);
        break;
      case BlockGameMessages.BlockGameScreen.Highscores:
        this.CloseMenus();
        ((Control) this._mainPanel).AddChild((Control) this._highscoresRootContainer);
        break;
    }
  }

  private void CloseMenus()
  {
    if (((Control) this._mainPanel).Children.Contains((Control) this._menuRootContainer))
      ((Control) this._mainPanel).RemoveChild((Control) this._menuRootContainer);
    if (((Control) this._mainPanel).Children.Contains((Control) this._gameOverRootContainer))
      ((Control) this._mainPanel).RemoveChild((Control) this._gameOverRootContainer);
    if (!((Control) this._mainPanel).Children.Contains((Control) this._highscoresRootContainer))
      return;
    ((Control) this._mainPanel).RemoveChild((Control) this._highscoresRootContainer);
  }

  public void SetGameoverInfo(int amount, int? localPlacement, int? globalPlacement)
  {
    string str1;
    if (globalPlacement.HasValue)
      str1 = $"#{globalPlacement}";
    else
      str1 = "-";
    string str2 = str1;
    string str3;
    if (localPlacement.HasValue)
      str3 = $"#{localPlacement}";
    else
      str3 = "-";
    string str4 = str3;
    this._finalScoreLabel.Text = Loc.GetString("blockgame-menu-gameover-info", new (string, object)[3]
    {
      ("global", (object) str2),
      ("local", (object) str4),
      ("points", (object) amount)
    });
  }

  public void UpdatePoints(int points)
  {
    this._pointsLabel.Text = Loc.GetString("blockgame-menu-label-points", new (string, object)[1]
    {
      (nameof (points), (object) points)
    });
  }

  public void UpdateLevel(int level)
  {
    this._levelLabel.Text = Loc.GetString("blockgame-menu-label-level", new (string, object)[1]
    {
      (nameof (level), (object) (level + 1))
    });
  }

  public void UpdateHighscores(
    List<BlockGameMessages.HighScoreEntry> localHighscores,
    List<BlockGameMessages.HighScoreEntry> globalHighscores)
  {
    StringBuilder stringBuilder1 = new StringBuilder(Loc.GetString("blockgame-menu-text-station") + "\n");
    StringBuilder stringBuilder2 = new StringBuilder(Loc.GetString("blockgame-menu-text-nanotrasen") + "\n");
    for (int index = 0; index < 5; ++index)
    {
      StringBuilder stringBuilder3 = stringBuilder1;
      string str1;
      if (localHighscores.Count <= index)
        str1 = $"#{index + 1}: ??? - 0";
      else
        str1 = $"#{index + 1}: {localHighscores[index].Name} - {localHighscores[index].Score}";
      stringBuilder3.AppendLine(str1);
      StringBuilder stringBuilder4 = stringBuilder2;
      string str2;
      if (globalHighscores.Count <= index)
        str2 = $"#{index + 1}: ??? - 0";
      else
        str2 = $"#{index + 1}: {globalHighscores[index].Name} - {globalHighscores[index].Score}";
      stringBuilder4.AppendLine(str2);
    }
    this._localHighscoresLabel.Text = stringBuilder1.ToString();
    this._globalHighscoresLabel.Text = stringBuilder2.ToString();
  }

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    ((BaseWindow) this).KeyBindDown(args);
    if (!this._isPlayer || ((BoundKeyEventArgs) args).Handled)
      return;
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ArcadeLeft))
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.StartLeft);
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ArcadeRight))
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.StartRight);
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ArcadeUp))
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.Rotate);
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.Arcade3))
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.CounterRotate);
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ArcadeDown))
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.SoftdropStart);
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.Arcade2))
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.Hold);
    }
    else
    {
      if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.Arcade1))
        return;
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.Harddrop);
    }
  }

  protected virtual void KeyBindUp(GUIBoundKeyEventArgs args)
  {
    ((BaseWindow) this).KeyBindUp(args);
    if (!this._isPlayer || ((BoundKeyEventArgs) args).Handled)
      return;
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ArcadeLeft))
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.EndLeft);
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ArcadeRight))
    {
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.EndRight);
    }
    else
    {
      if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ArcadeDown))
        return;
      Action<BlockGamePlayerAction> onAction = this.OnAction;
      if (onAction == null)
        return;
      onAction(BlockGamePlayerAction.SoftdropEnd);
    }
  }

  public void UpdateNextBlock(BlockGameBlock[] blocks)
  {
    ((Control) this._nextBlockGrid).RemoveAllChildren();
    if (blocks.Length == 0)
      return;
    int num1 = ((IEnumerable<BlockGameBlock>) blocks).Max<BlockGameBlock>((Func<BlockGameBlock, int>) (b => b.Position.X)) + 1;
    int num2 = ((IEnumerable<BlockGameBlock>) blocks).Max<BlockGameBlock>((Func<BlockGameBlock, int>) (b => b.Position.Y)) + 1;
    this._nextBlockGrid.Columns = num1;
    for (int y = 0; y < num2; ++y)
    {
      for (int x = 0; x < num1; ++x)
      {
        Color colorForPosition = BlockGameMenu.GetColorForPosition(blocks, x, y);
        GridContainer nextBlockGrid = this._nextBlockGrid;
        PanelContainer panelContainer = new PanelContainer();
        panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
        {
          BackgroundColor = colorForPosition
        };
        ((Control) panelContainer).MinSize = BlockGameMenu.BlockSize;
        ((Control) panelContainer).RectDrawClipMargin = 0;
        ((Control) nextBlockGrid).AddChild((Control) panelContainer);
      }
    }
  }

  public void UpdateHeldBlock(BlockGameBlock[] blocks)
  {
    ((Control) this._holdBlockGrid).RemoveAllChildren();
    if (blocks.Length == 0)
      return;
    int num1 = ((IEnumerable<BlockGameBlock>) blocks).Max<BlockGameBlock>((Func<BlockGameBlock, int>) (b => b.Position.X)) + 1;
    int num2 = ((IEnumerable<BlockGameBlock>) blocks).Max<BlockGameBlock>((Func<BlockGameBlock, int>) (b => b.Position.Y)) + 1;
    this._holdBlockGrid.Columns = num1;
    for (int y = 0; y < num2; ++y)
    {
      for (int x = 0; x < num1; ++x)
      {
        Color colorForPosition = BlockGameMenu.GetColorForPosition(blocks, x, y);
        GridContainer holdBlockGrid = this._holdBlockGrid;
        PanelContainer panelContainer = new PanelContainer();
        panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
        {
          BackgroundColor = colorForPosition
        };
        ((Control) panelContainer).MinSize = BlockGameMenu.BlockSize;
        ((Control) panelContainer).RectDrawClipMargin = 0;
        ((Control) holdBlockGrid).AddChild((Control) panelContainer);
      }
    }
  }

  public void UpdateBlocks(BlockGameBlock[] blocks)
  {
    ((Control) this._gameGrid).RemoveAllChildren();
    for (int y = 0; y < 20; ++y)
    {
      for (int x = 0; x < 10; ++x)
      {
        Color colorForPosition = BlockGameMenu.GetColorForPosition(blocks, x, y);
        GridContainer gameGrid = this._gameGrid;
        PanelContainer panelContainer = new PanelContainer();
        panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
        {
          BackgroundColor = colorForPosition
        };
        ((Control) panelContainer).MinSize = BlockGameMenu.BlockSize;
        ((Control) panelContainer).RectDrawClipMargin = 0;
        ((Control) gameGrid).AddChild((Control) panelContainer);
      }
    }
  }

  private static Color GetColorForPosition(BlockGameBlock[] blocks, int x, int y)
  {
    Color colorForPosition = Color.Transparent;
    BlockGameBlock? nullable = Extensions.FirstOrNull<BlockGameBlock>((IEnumerable<BlockGameBlock>) blocks, (Func<BlockGameBlock, bool>) (b => b.Position.X == x && b.Position.Y == y));
    if (nullable.HasValue)
      colorForPosition = BlockGameBlock.ToColor(nullable.Value.GameBlockColor);
    return colorForPosition;
  }
}
