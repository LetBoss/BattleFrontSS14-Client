using System;
using System.Numerics;
using Content.Shared.Arcade;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;

namespace Content.Client.Arcade;

public sealed class SpaceVillainArcadeMenu : DefaultWindow
{
	private readonly Label _enemyNameLabel;

	private readonly Label _playerInfoLabel;

	private readonly Label _enemyInfoLabel;

	private readonly Label _playerActionLabel;

	private readonly Label _enemyActionLabel;

	private readonly Button[] _gameButtons = (Button[])(object)new Button[3];

	public event Action<SharedSpaceVillainArcadeComponent.PlayerAction>? OnPlayerAction;

	public SpaceVillainArcadeMenu()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Expected O, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Expected O, but got Unknown
		Vector2 minSize = (((Control)this).SetSize = new Vector2(300f, 225f));
		((Control)this).MinSize = minSize;
		((DefaultWindow)this).Title = Loc.GetString("spacevillain-menu-title");
		GridContainer val = new GridContainer
		{
			Columns = 1
		};
		GridContainer val2 = new GridContainer
		{
			Columns = 3
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("spacevillain-menu-label-player"),
			Align = (AlignMode)1
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = "|",
			Align = (AlignMode)1
		});
		_enemyNameLabel = new Label
		{
			Align = (AlignMode)1
		};
		((Control)val2).AddChild((Control)(object)_enemyNameLabel);
		_playerInfoLabel = new Label
		{
			Align = (AlignMode)1
		};
		((Control)val2).AddChild((Control)(object)_playerInfoLabel);
		((Control)val2).AddChild((Control)new Label
		{
			Text = "|",
			Align = (AlignMode)1
		});
		_enemyInfoLabel = new Label
		{
			Align = (AlignMode)1
		};
		((Control)val2).AddChild((Control)(object)_enemyInfoLabel);
		CenterContainer val3 = new CenterContainer();
		((Control)val3).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val3);
		_playerActionLabel = new Label
		{
			Align = (AlignMode)1
		};
		((Control)val).AddChild((Control)(object)_playerActionLabel);
		_enemyActionLabel = new Label
		{
			Align = (AlignMode)1
		};
		((Control)val).AddChild((Control)(object)_enemyActionLabel);
		GridContainer val4 = new GridContainer
		{
			Columns = 3
		};
		_gameButtons[0] = new Button
		{
			Text = Loc.GetString("spacevillain-menu-button-attack")
		};
		((BaseButton)_gameButtons[0]).OnPressed += delegate
		{
			this.OnPlayerAction?.Invoke(SharedSpaceVillainArcadeComponent.PlayerAction.Attack);
		};
		((Control)val4).AddChild((Control)(object)_gameButtons[0]);
		_gameButtons[1] = new Button
		{
			Text = Loc.GetString("spacevillain-menu-button-heal")
		};
		((BaseButton)_gameButtons[1]).OnPressed += delegate
		{
			this.OnPlayerAction?.Invoke(SharedSpaceVillainArcadeComponent.PlayerAction.Heal);
		};
		((Control)val4).AddChild((Control)(object)_gameButtons[1]);
		_gameButtons[2] = new Button
		{
			Text = Loc.GetString("spacevillain-menu-button-recharge")
		};
		((BaseButton)_gameButtons[2]).OnPressed += delegate
		{
			this.OnPlayerAction?.Invoke(SharedSpaceVillainArcadeComponent.PlayerAction.Recharge);
		};
		((Control)val4).AddChild((Control)(object)_gameButtons[2]);
		val3 = new CenterContainer();
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val).AddChild((Control)(object)val3);
		Button val5 = new Button
		{
			Text = Loc.GetString("spacevillain-menu-button-new-game")
		};
		((BaseButton)val5).OnPressed += delegate
		{
			this.OnPlayerAction?.Invoke(SharedSpaceVillainArcadeComponent.PlayerAction.NewGame);
		};
		((Control)val).AddChild((Control)(object)val5);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}

	private void UpdateMetadata(SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage message)
	{
		((DefaultWindow)this).Title = message.GameTitle;
		_enemyNameLabel.Text = message.EnemyName;
		Button[] gameButtons = _gameButtons;
		for (int i = 0; i < gameButtons.Length; i++)
		{
			((BaseButton)gameButtons[i]).Disabled = message.ButtonsDisabled;
		}
	}

	public void UpdateInfo(SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage message)
	{
		if (message is SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage message2)
		{
			UpdateMetadata(message2);
		}
		_playerInfoLabel.Text = $"HP: {message.PlayerHP} MP: {message.PlayerMP}";
		_enemyInfoLabel.Text = $"HP: {message.EnemyHP} MP: {message.EnemyMP}";
		_playerActionLabel.Text = message.PlayerActionMessage;
		_enemyActionLabel.Text = message.EnemyActionMessage;
	}
}
