using System;
using Content.Client._CIV14merka.GlobalMap;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivBotControlPanel : Control
{
	private readonly CivCommanderBotControlSystem _control;

	private readonly CivGlobalMapSystem _globalMap;

	private readonly BoxContainer _root;

	private readonly Label _selectionLabel;

	private readonly Label _orderModeLabel;

	private readonly GridContainer _orderButtons;

	private readonly GridContainer _squadButtons;

	private readonly Button _clearSelectionButton;

	public CivBotControlPanel(CivCommanderBotControlSystem control, CivGlobalMapSystem globalMap)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Expected O, but got Unknown
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Expected O, but got Unknown
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Expected O, but got Unknown
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Expected O, but got Unknown
		IoCManager.InjectDependencies<CivBotControlPanel>(this);
		_control = control;
		_globalMap = globalMap;
		_root = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			Margin = new Thickness(8f),
			HorizontalExpand = true
		};
		((Control)this).AddChild((Control)(object)_root);
		((Control)_root).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-cmd-bot-panel-title"),
			FontColorOverride = Color.White,
			HorizontalExpand = true
		});
		Label val = new Label();
		val.Text = Loc.GetString("civ-cmd-bot-panel-selected", new(string, object)[1] { ("count", 0) });
		val.FontColorOverride = Color.LightGray;
		_selectionLabel = val;
		((Control)_root).AddChild((Control)(object)_selectionLabel);
		val = new Label();
		val.Text = Loc.GetString("civ-cmd-bot-panel-mode", new(string, object)[1] { ("mode", GetOrderName(CivBotOrderType.Move)) });
		val.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#4caf50", (Color?)null);
		_orderModeLabel = val;
		((Control)_root).AddChild((Control)(object)_orderModeLabel);
		((Control)_root).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-cmd-bot-panel-orders-header")
		});
		_orderButtons = new GridContainer
		{
			Columns = 3,
			HorizontalExpand = true
		};
		((Control)_root).AddChild((Control)(object)_orderButtons);
		AddOrderButton(Loc.GetString("civ-cmd-bot-btn-move"), CivBotOrderType.Move, "#4caf50");
		AddOrderButton(Loc.GetString("civ-cmd-bot-btn-attack-move"), CivBotOrderType.AttackMove, "#f44336");
		AddOrderButton(Loc.GetString("civ-cmd-bot-btn-hold"), CivBotOrderType.HoldPosition, "#2196f3");
		AddOrderButton(Loc.GetString("civ-cmd-bot-btn-follow"), CivBotOrderType.Follow, "#9c27b0");
		AddOrderButton(Loc.GetString("civ-cmd-bot-btn-defend"), CivBotOrderType.Defend, "#00bcd4");
		AddOrderButton(Loc.GetString("civ-cmd-bot-btn-patrol"), CivBotOrderType.Patrol, "#ff9800");
		((Control)_root).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-cmd-bot-panel-squads-header")
		});
		_squadButtons = new GridContainer
		{
			Columns = 5,
			HorizontalExpand = true
		};
		((Control)_root).AddChild((Control)(object)_squadButtons);
		for (int i = 1; i <= 9; i++)
		{
			int squadId = i;
			Button val2 = new Button
			{
				Text = i.ToString(),
				MinWidth = 30f
			};
			((BaseButton)val2).OnPressed += delegate
			{
				_control.SelectSquad(squadId);
			};
			((Control)_squadButtons).AddChild((Control)(object)val2);
		}
		_clearSelectionButton = new Button
		{
			Text = Loc.GetString("civ-cmd-bot-clear-selection"),
			HorizontalExpand = true
		};
		((BaseButton)_clearSelectionButton).OnPressed += delegate
		{
			_control.ClearSelection();
		};
		((Control)_root).AddChild((Control)(object)_clearSelectionButton);
		Button val3 = new Button
		{
			Text = Loc.GetString("civ-cmd-bot-cancel-patrol"),
			HorizontalExpand = true
		};
		((BaseButton)val3).OnPressed += delegate
		{
			_control.CancelPatrol();
		};
		((Control)_root).AddChild((Control)(object)val3);
	}

	private void AddOrderButton(string text, CivBotOrderType orderType, string colorHex)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		Color modulate = Color.FromHex((ReadOnlySpan<char>)colorHex, (Color?)null);
		Button val = new Button
		{
			Text = text,
			HorizontalExpand = true,
			Modulate = modulate
		};
		((BaseButton)val).OnPressed += delegate
		{
			_control.SetOrderMode(orderType);
		};
		((Control)_orderButtons).AddChild((Control)(object)val);
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		int selectedCount = _control.GetSelectedCount();
		_selectionLabel.Text = Loc.GetString("civ-cmd-bot-panel-selected", new(string, object)[1] { ("count", selectedCount) });
		CivBotOrderType currentOrderMode = _control.CurrentOrderMode;
		Color orderColor = GetOrderColor(currentOrderMode);
		_orderModeLabel.Text = Loc.GetString("civ-cmd-bot-panel-mode", new(string, object)[1] { ("mode", GetOrderName(currentOrderMode)) });
		_orderModeLabel.FontColorOverride = orderColor;
		if (_control.IsPatrolMode)
		{
			Label orderModeLabel = _orderModeLabel;
			orderModeLabel.Text += Loc.GetString("civ-cmd-bot-panel-patrol-points", new(string, object)[1] { ("count", _control.PatrolPoints.Count) });
		}
	}

	private static Color GetOrderColor(CivBotOrderType order)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(order switch
		{
			CivBotOrderType.Idle => Color.FromHex((ReadOnlySpan<char>)"#9e9e9e", (Color?)null), 
			CivBotOrderType.Move => Color.FromHex((ReadOnlySpan<char>)"#4caf50", (Color?)null), 
			CivBotOrderType.AttackMove => Color.FromHex((ReadOnlySpan<char>)"#f44336", (Color?)null), 
			CivBotOrderType.HoldPosition => Color.FromHex((ReadOnlySpan<char>)"#2196f3", (Color?)null), 
			CivBotOrderType.Follow => Color.FromHex((ReadOnlySpan<char>)"#9c27b0", (Color?)null), 
			CivBotOrderType.Defend => Color.FromHex((ReadOnlySpan<char>)"#00bcd4", (Color?)null), 
			CivBotOrderType.Patrol => Color.FromHex((ReadOnlySpan<char>)"#ff9800", (Color?)null), 
			_ => Color.White, 
		});
	}

	private static string GetOrderName(CivBotOrderType order)
	{
		return order switch
		{
			CivBotOrderType.Idle => Loc.GetString("civ-cmd-bot-mode-idle"), 
			CivBotOrderType.Move => Loc.GetString("civ-cmd-bot-mode-move"), 
			CivBotOrderType.AttackMove => Loc.GetString("civ-cmd-bot-mode-attack-move"), 
			CivBotOrderType.HoldPosition => Loc.GetString("civ-cmd-bot-mode-hold"), 
			CivBotOrderType.Follow => Loc.GetString("civ-cmd-bot-mode-follow"), 
			CivBotOrderType.Defend => Loc.GetString("civ-cmd-bot-mode-defend"), 
			CivBotOrderType.Patrol => Loc.GetString("civ-cmd-bot-mode-patrol"), 
			_ => Loc.GetString("civ-cmd-bot-mode-unknown"), 
		};
	}
}
