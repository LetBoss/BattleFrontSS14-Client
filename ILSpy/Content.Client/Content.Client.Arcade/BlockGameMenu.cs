using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
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

namespace Content.Client.Arcade;

public sealed class BlockGameMenu : DefaultWindow
{
	private static readonly Color OverlayBackgroundColor = new Color((byte)74, (byte)74, (byte)81, (byte)180);

	private static readonly Color OverlayShadowColor = new Color((byte)0, (byte)0, (byte)0, (byte)83);

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
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Expected O, but got Unknown
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Expected O, but got Unknown
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Expected O, but got Unknown
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Expected O, but got Unknown
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Expected O, but got Unknown
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Expected O, but got Unknown
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Expected O, but got Unknown
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Expected O, but got Unknown
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Expected O, but got Unknown
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Expected O, but got Unknown
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Expected O, but got Unknown
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Expected O, but got Unknown
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Expected O, but got Unknown
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Expected O, but got Unknown
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Expected O, but got Unknown
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Expected O, but got Unknown
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Expected O, but got Unknown
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Expected O, but got Unknown
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Expected O, but got Unknown
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Expected O, but got Unknown
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Expected O, but got Unknown
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Expected O, but got Unknown
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Expected O, but got Unknown
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Expected O, but got Unknown
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Expected O, but got Unknown
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Expected O, but got Unknown
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Expected O, but got Unknown
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Expected O, but got Unknown
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Expected O, but got Unknown
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Expected O, but got Unknown
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Expected O, but got Unknown
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("blockgame-menu-title");
		Vector2 minSize = (((Control)this).SetSize = new Vector2(410f, 490f));
		((Control)this).MinSize = minSize;
		Texture texture = IoCManager.Resolve<IResourceCache>().GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
		_mainPanel = new PanelContainer();
		_gameRootContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		_levelLabel = new Label
		{
			Align = (AlignMode)1,
			HorizontalExpand = true
		};
		((Control)_gameRootContainer).AddChild((Control)(object)_levelLabel);
		((Control)_gameRootContainer).AddChild(new Control
		{
			MinSize = new Vector2(1f, 5f)
		});
		_pointsLabel = new Label
		{
			Align = (AlignMode)1,
			HorizontalExpand = true
		};
		((Control)_gameRootContainer).AddChild((Control)(object)_pointsLabel);
		((Control)_gameRootContainer).AddChild(new Control
		{
			MinSize = new Vector2(1f, 10f)
		});
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		((Control)val).AddChild(SetupHoldBox(texture));
		((Control)val).AddChild(new Control
		{
			MinSize = new Vector2(10f, 1f)
		});
		((Control)val).AddChild(SetupGameGrid(texture));
		((Control)val).AddChild(new Control
		{
			MinSize = new Vector2(10f, 1f)
		});
		((Control)val).AddChild(SetupNextBox(texture));
		((Control)_gameRootContainer).AddChild((Control)(object)val);
		((Control)_gameRootContainer).AddChild(new Control
		{
			MinSize = new Vector2(1f, 10f)
		});
		_pauseButton = new Button
		{
			Text = Loc.GetString("blockgame-menu-button-pause"),
			TextAlign = (AlignMode)1
		};
		((BaseButton)_pauseButton).OnPressed += delegate
		{
			TryPause();
		};
		((Control)_gameRootContainer).AddChild((Control)(object)_pauseButton);
		((Control)_mainPanel).AddChild((Control)(object)_gameRootContainer);
		StyleBoxTexture val2 = new StyleBoxTexture
		{
			Texture = texture,
			Modulate = OverlayShadowColor
		};
		val2.SetPatchMargin((Margin)15, 10f);
		_menuRootContainer = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val2,
			VerticalAlignment = (VAlignment)2,
			HorizontalAlignment = (HAlignment)2
		};
		StyleBoxTexture val3 = new StyleBoxTexture
		{
			Texture = texture,
			Modulate = OverlayBackgroundColor
		};
		val3.SetPatchMargin((Margin)15, 10f);
		PanelContainer val4 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val3,
			VerticalAlignment = (VAlignment)2,
			HorizontalAlignment = (HAlignment)2
		};
		((Control)_menuRootContainer).AddChild((Control)(object)val4);
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		_newGameButton = new Button
		{
			Text = Loc.GetString("blockgame-menu-button-new-game"),
			TextAlign = (AlignMode)1
		};
		((BaseButton)_newGameButton).OnPressed += delegate
		{
			this.OnAction?.Invoke(BlockGamePlayerAction.NewGame);
		};
		((Control)val5).AddChild((Control)(object)_newGameButton);
		((Control)val5).AddChild(new Control
		{
			MinSize = new Vector2(1f, 10f)
		});
		_scoreBoardButton = new Button
		{
			Text = Loc.GetString("blockgame-menu-button-scoreboard"),
			TextAlign = (AlignMode)1
		};
		((BaseButton)_scoreBoardButton).OnPressed += delegate
		{
			this.OnAction?.Invoke(BlockGamePlayerAction.ShowHighscores);
		};
		((Control)val5).AddChild((Control)(object)_scoreBoardButton);
		_unpauseButtonMargin = new Control
		{
			MinSize = new Vector2(1f, 10f),
			Visible = false
		};
		((Control)val5).AddChild(_unpauseButtonMargin);
		_unpauseButton = new Button
		{
			Text = Loc.GetString("blockgame-menu-button-unpause"),
			TextAlign = (AlignMode)1,
			Visible = false
		};
		((BaseButton)_unpauseButton).OnPressed += delegate
		{
			this.OnAction?.Invoke(BlockGamePlayerAction.Unpause);
		};
		((Control)val5).AddChild((Control)(object)_unpauseButton);
		((Control)val4).AddChild((Control)(object)val5);
		StyleBoxTexture val6 = new StyleBoxTexture
		{
			Texture = texture,
			Modulate = OverlayShadowColor
		};
		val6.SetPatchMargin((Margin)15, 10f);
		_gameOverRootContainer = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val6,
			VerticalAlignment = (VAlignment)2,
			HorizontalAlignment = (HAlignment)2
		};
		StyleBoxTexture val7 = new StyleBoxTexture
		{
			Texture = texture,
			Modulate = OverlayBackgroundColor
		};
		val7.SetPatchMargin((Margin)15, 10f);
		PanelContainer val8 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val7,
			VerticalAlignment = (VAlignment)2,
			HorizontalAlignment = (HAlignment)2
		};
		((Control)_gameOverRootContainer).AddChild((Control)(object)val8);
		BoxContainer val9 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		((Control)val9).AddChild((Control)new Label
		{
			Text = Loc.GetString("blockgame-menu-msg-game-over"),
			Align = (AlignMode)1
		});
		((Control)val9).AddChild(new Control
		{
			MinSize = new Vector2(1f, 10f)
		});
		_finalScoreLabel = new Label
		{
			Align = (AlignMode)1
		};
		((Control)val9).AddChild((Control)(object)_finalScoreLabel);
		((Control)val9).AddChild(new Control
		{
			MinSize = new Vector2(1f, 10f)
		});
		_finalNewGameButton = new Button
		{
			Text = Loc.GetString("blockgame-menu-button-new-game"),
			TextAlign = (AlignMode)1
		};
		((BaseButton)_finalNewGameButton).OnPressed += delegate
		{
			this.OnAction?.Invoke(BlockGamePlayerAction.NewGame);
		};
		((Control)val9).AddChild((Control)(object)_finalNewGameButton);
		((Control)val8).AddChild((Control)(object)val9);
		StyleBoxTexture val10 = new StyleBoxTexture
		{
			Texture = texture,
			Modulate = OverlayShadowColor
		};
		val10.SetPatchMargin((Margin)15, 10f);
		_highscoresRootContainer = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val10,
			VerticalAlignment = (VAlignment)2,
			HorizontalAlignment = (HAlignment)2
		};
		Color modulate = default(Color);
		((Color)(ref modulate))._002Ector(OverlayBackgroundColor.R, OverlayBackgroundColor.G, OverlayBackgroundColor.B, 220f);
		StyleBoxTexture val11 = new StyleBoxTexture
		{
			Texture = texture,
			Modulate = modulate
		};
		val11.SetPatchMargin((Margin)15, 10f);
		PanelContainer val12 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val11,
			VerticalAlignment = (VAlignment)2,
			HorizontalAlignment = (HAlignment)2
		};
		((Control)_highscoresRootContainer).AddChild((Control)(object)val12);
		BoxContainer val13 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		((Control)val13).AddChild((Control)new Label
		{
			Text = Loc.GetString("blockgame-menu-label-highscores")
		});
		((Control)val13).AddChild(new Control
		{
			MinSize = new Vector2(1f, 10f)
		});
		BoxContainer val14 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		_localHighscoresLabel = new Label
		{
			Align = (AlignMode)1
		};
		((Control)val14).AddChild((Control)(object)_localHighscoresLabel);
		((Control)val14).AddChild(new Control
		{
			MinSize = new Vector2(40f, 1f)
		});
		_globalHighscoresLabel = new Label
		{
			Align = (AlignMode)1
		};
		((Control)val14).AddChild((Control)(object)_globalHighscoresLabel);
		((Control)val13).AddChild((Control)(object)val14);
		((Control)val13).AddChild(new Control
		{
			MinSize = new Vector2(1f, 10f)
		});
		_highscoreBackButton = new Button
		{
			Text = Loc.GetString("blockgame-menu-button-back"),
			TextAlign = (AlignMode)1
		};
		((BaseButton)_highscoreBackButton).OnPressed += delegate
		{
			this.OnAction?.Invoke(BlockGamePlayerAction.Pause);
		};
		((Control)val13).AddChild((Control)(object)_highscoreBackButton);
		((Control)val12).AddChild((Control)(object)val13);
		((DefaultWindow)this).Contents.AddChild((Control)(object)_mainPanel);
		((Control)this).CanKeyboardFocus = true;
	}

	public void SetUsability(bool isPlayer)
	{
		_isPlayer = isPlayer;
		UpdateUsability();
	}

	private void UpdateUsability()
	{
		((BaseButton)_pauseButton).Disabled = !_isPlayer;
		((BaseButton)_newGameButton).Disabled = !_isPlayer;
		((BaseButton)_scoreBoardButton).Disabled = !_isPlayer;
		((BaseButton)_unpauseButton).Disabled = !_isPlayer;
		((BaseButton)_finalNewGameButton).Disabled = !_isPlayer;
		((BaseButton)_highscoreBackButton).Disabled = !_isPlayer;
	}

	private Control SetupGameGrid(Texture panelTex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00bb: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		_gameGrid = new GridContainer
		{
			Columns = 10,
			HSeparationOverride = 1,
			VSeparationOverride = 1
		};
		UpdateBlocks(Array.Empty<BlockGameBlock>());
		StyleBoxTexture val = new StyleBoxTexture
		{
			Texture = panelTex,
			Modulate = Color.FromHex((ReadOnlySpan<char>)"#4a4a51", (Color?)null)
		};
		val.SetPatchMargin((Margin)15, 10f);
		PanelContainer val2 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val,
			HorizontalExpand = true,
			SizeFlagsStretchRatio = 34.25f
		};
		PanelContainer val3 = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#86868d", (Color?)null)
			}
		};
		((Control)val3).AddChild((Control)(object)_gameGrid);
		((Control)val2).AddChild((Control)(object)val3);
		return (Control)val2;
	}

	private Control SetupNextBox(Texture panelTex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00ec: Expected O, but got Unknown
		StyleBoxTexture val = new StyleBoxTexture
		{
			Texture = panelTex,
			Modulate = Color.FromHex((ReadOnlySpan<char>)"#4a4a51", (Color?)null)
		};
		val.SetPatchMargin((Margin)15, 10f);
		GridContainer val2 = new GridContainer
		{
			Columns = 1,
			HorizontalExpand = true,
			SizeFlagsStretchRatio = 20f
		};
		PanelContainer val3 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val,
			MinSize = BlockSize * 6.5f,
			HorizontalAlignment = (HAlignment)1,
			VerticalAlignment = (VAlignment)1
		};
		CenterContainer val4 = new CenterContainer();
		_nextBlockGrid = new GridContainer
		{
			HSeparationOverride = 1,
			VSeparationOverride = 1
		};
		((Control)val4).AddChild((Control)(object)_nextBlockGrid);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("blockgame-menu-label-next"),
			Align = (AlignMode)1
		});
		return (Control)val2;
	}

	private Control SetupHoldBox(Texture panelTex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00ec: Expected O, but got Unknown
		StyleBoxTexture val = new StyleBoxTexture
		{
			Texture = panelTex,
			Modulate = Color.FromHex((ReadOnlySpan<char>)"#4a4a51", (Color?)null)
		};
		val.SetPatchMargin((Margin)15, 10f);
		GridContainer val2 = new GridContainer
		{
			Columns = 1,
			HorizontalExpand = true,
			SizeFlagsStretchRatio = 20f
		};
		PanelContainer val3 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val,
			MinSize = BlockSize * 6.5f,
			HorizontalAlignment = (HAlignment)1,
			VerticalAlignment = (VAlignment)1
		};
		CenterContainer val4 = new CenterContainer();
		_holdBlockGrid = new GridContainer
		{
			HSeparationOverride = 1,
			VSeparationOverride = 1
		};
		((Control)val4).AddChild((Control)(object)_holdBlockGrid);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("blockgame-menu-label-hold"),
			Align = (AlignMode)1
		});
		return (Control)val2;
	}

	protected override void KeyboardFocusExited()
	{
		if (((BaseWindow)this).IsOpen && !_gameOver)
		{
			TryPause();
		}
	}

	private void TryPause()
	{
		this.OnAction?.Invoke(BlockGamePlayerAction.Pause);
	}

	public void SetStarted()
	{
		_gameOver = false;
		((Control)_unpauseButton).Visible = true;
		_unpauseButtonMargin.Visible = true;
	}

	public void SetScreen(BlockGameMessages.BlockGameScreen screen)
	{
		if (!_gameOver)
		{
			switch (screen)
			{
			case BlockGameMessages.BlockGameScreen.Game:
				((Control)this).GrabKeyboardFocus();
				CloseMenus();
				((BaseButton)_pauseButton).Disabled = !_isPlayer;
				break;
			case BlockGameMessages.BlockGameScreen.Pause:
				CloseMenus();
				((Control)_mainPanel).AddChild((Control)(object)_menuRootContainer);
				((BaseButton)_pauseButton).Disabled = true;
				break;
			case BlockGameMessages.BlockGameScreen.Gameover:
				_gameOver = true;
				((BaseButton)_pauseButton).Disabled = true;
				CloseMenus();
				((Control)_mainPanel).AddChild((Control)(object)_gameOverRootContainer);
				break;
			case BlockGameMessages.BlockGameScreen.Highscores:
				CloseMenus();
				((Control)_mainPanel).AddChild((Control)(object)_highscoresRootContainer);
				break;
			}
		}
	}

	private void CloseMenus()
	{
		if (((Control)_mainPanel).Children.Contains((Control)(object)_menuRootContainer))
		{
			((Control)_mainPanel).RemoveChild((Control)(object)_menuRootContainer);
		}
		if (((Control)_mainPanel).Children.Contains((Control)(object)_gameOverRootContainer))
		{
			((Control)_mainPanel).RemoveChild((Control)(object)_gameOverRootContainer);
		}
		if (((Control)_mainPanel).Children.Contains((Control)(object)_highscoresRootContainer))
		{
			((Control)_mainPanel).RemoveChild((Control)(object)_highscoresRootContainer);
		}
	}

	public void SetGameoverInfo(int amount, int? localPlacement, int? globalPlacement)
	{
		string item = ((!globalPlacement.HasValue) ? "-" : $"#{globalPlacement}");
		string item2 = ((!localPlacement.HasValue) ? "-" : $"#{localPlacement}");
		_finalScoreLabel.Text = Loc.GetString("blockgame-menu-gameover-info", new(string, object)[3]
		{
			("global", item),
			("local", item2),
			("points", amount)
		});
	}

	public void UpdatePoints(int points)
	{
		_pointsLabel.Text = Loc.GetString("blockgame-menu-label-points", new(string, object)[1] { ("points", points) });
	}

	public void UpdateLevel(int level)
	{
		_levelLabel.Text = Loc.GetString("blockgame-menu-label-level", new(string, object)[1] { ("level", level + 1) });
	}

	public void UpdateHighscores(List<BlockGameMessages.HighScoreEntry> localHighscores, List<BlockGameMessages.HighScoreEntry> globalHighscores)
	{
		StringBuilder stringBuilder = new StringBuilder(Loc.GetString("blockgame-menu-text-station") + "\n");
		StringBuilder stringBuilder2 = new StringBuilder(Loc.GetString("blockgame-menu-text-nanotrasen") + "\n");
		for (int i = 0; i < 5; i++)
		{
			stringBuilder.AppendLine((localHighscores.Count > i) ? $"#{i + 1}: {localHighscores[i].Name} - {localHighscores[i].Score}" : $"#{i + 1}: ??? - 0");
			stringBuilder2.AppendLine((globalHighscores.Count > i) ? $"#{i + 1}: {globalHighscores[i].Name} - {globalHighscores[i].Score}" : $"#{i + 1}: ??? - 0");
		}
		_localHighscoresLabel.Text = stringBuilder.ToString();
		_globalHighscoresLabel.Text = stringBuilder2.ToString();
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		((BaseWindow)this).KeyBindDown(args);
		if (_isPlayer && !((BoundKeyEventArgs)args).Handled)
		{
			if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ArcadeLeft)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.StartLeft);
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ArcadeRight)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.StartRight);
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ArcadeUp)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.Rotate);
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.Arcade3)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.CounterRotate);
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ArcadeDown)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.SoftdropStart);
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.Arcade2)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.Hold);
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.Arcade1)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.Harddrop);
			}
		}
	}

	protected override void KeyBindUp(GUIBoundKeyEventArgs args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		((BaseWindow)this).KeyBindUp(args);
		if (_isPlayer && !((BoundKeyEventArgs)args).Handled)
		{
			if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ArcadeLeft)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.EndLeft);
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ArcadeRight)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.EndRight);
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ArcadeDown)
			{
				this.OnAction?.Invoke(BlockGamePlayerAction.SoftdropEnd);
			}
		}
	}

	public void UpdateNextBlock(BlockGameBlock[] blocks)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		((Control)_nextBlockGrid).RemoveAllChildren();
		if (blocks.Length == 0)
		{
			return;
		}
		int num = blocks.Max((BlockGameBlock b) => b.Position.X) + 1;
		int num2 = blocks.Max((BlockGameBlock b) => b.Position.Y) + 1;
		_nextBlockGrid.Columns = num;
		for (int num3 = 0; num3 < num2; num3++)
		{
			for (int num4 = 0; num4 < num; num4++)
			{
				Color colorForPosition = GetColorForPosition(blocks, num4, num3);
				((Control)_nextBlockGrid).AddChild((Control)new PanelContainer
				{
					PanelOverride = (StyleBox)new StyleBoxFlat
					{
						BackgroundColor = colorForPosition
					},
					MinSize = BlockSize,
					RectDrawClipMargin = 0
				});
			}
		}
	}

	public void UpdateHeldBlock(BlockGameBlock[] blocks)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		((Control)_holdBlockGrid).RemoveAllChildren();
		if (blocks.Length == 0)
		{
			return;
		}
		int num = blocks.Max((BlockGameBlock b) => b.Position.X) + 1;
		int num2 = blocks.Max((BlockGameBlock b) => b.Position.Y) + 1;
		_holdBlockGrid.Columns = num;
		for (int num3 = 0; num3 < num2; num3++)
		{
			for (int num4 = 0; num4 < num; num4++)
			{
				Color colorForPosition = GetColorForPosition(blocks, num4, num3);
				((Control)_holdBlockGrid).AddChild((Control)new PanelContainer
				{
					PanelOverride = (StyleBox)new StyleBoxFlat
					{
						BackgroundColor = colorForPosition
					},
					MinSize = BlockSize,
					RectDrawClipMargin = 0
				});
			}
		}
	}

	public void UpdateBlocks(BlockGameBlock[] blocks)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		((Control)_gameGrid).RemoveAllChildren();
		for (int i = 0; i < 20; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				Color colorForPosition = GetColorForPosition(blocks, j, i);
				((Control)_gameGrid).AddChild((Control)new PanelContainer
				{
					PanelOverride = (StyleBox)new StyleBoxFlat
					{
						BackgroundColor = colorForPosition
					},
					MinSize = BlockSize,
					RectDrawClipMargin = 0
				});
			}
		}
	}

	private static Color GetColorForPosition(BlockGameBlock[] blocks, int x, int y)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Color result = Color.Transparent;
		BlockGameBlock? blockGameBlock = Extensions.FirstOrNull<BlockGameBlock>((IEnumerable<BlockGameBlock>)blocks, (Func<BlockGameBlock, bool>)((BlockGameBlock b) => b.Position.X == x && b.Position.Y == y));
		if (blockGameBlock.HasValue)
		{
			result = BlockGameBlock.ToColor(blockGameBlock.Value.GameBlockColor);
		}
		return result;
	}
}
