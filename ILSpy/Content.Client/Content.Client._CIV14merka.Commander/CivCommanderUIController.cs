using System;
using Content.Client._CIV14merka.Commander.UI;
using Content.Client._CIV14merka.GlobalMap;
using Content.Client._CIV14merka.Supply;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	[Dependency]
	private IPlayerManager _player;

	[UISystemDependency]
	private readonly CivCommanderPurchasePlacementSystem? _placement;

	[UISystemDependency]
	private readonly CivCommanderBotControlSystem? _botControl;

	[UISystemDependency]
	private readonly CivCommanderTeleportSystem? _teleport;

	[UISystemDependency]
	private readonly CivGlobalMapSystem? _globalMap;

	[UISystemDependency]
	private readonly CivAirstrikeSystem? _airstrike;

	[UISystemDependency]
	private readonly CivCommanderLinesSystem? _lines;

	[UISystemDependency]
	private readonly CivSupplyRefillSystem? _supplyRefill;

	private bool _loaded;

	private CivCommanderLinesGui? _linesPanel;

	private CivCommanderLabelInputWindow? _labelWindow;

	private CivCommanderGui? Gui => base.UIManager.GetActiveUIWidgetOrNull<CivCommanderGui>();

	private CivCommanderInfoGui? GuiInfo => base.UIManager.GetActiveUIWidgetOrNull<CivCommanderInfoGui>();

	public void OnStateEntered(GameplayState state)
	{
		LoadGui();
		EnsureLinesPanel();
	}

	public void OnStateExited(GameplayState state)
	{
		UnloadGui();
		DisposeLinesPanel();
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		if (_loaded && Gui != null)
		{
			bool flag = IsCommander();
			((Control)Gui).Visible = flag;
			CivCommanderInfoGui guiInfo = GuiInfo;
			if (guiInfo != null)
			{
				((Control)guiInfo).Visible = flag;
				guiInfo.SetCurrency(_globalMap?.GetCommanderCurrency() ?? 0);
				guiInfo.SetAirstrikeCooldown(_globalMap?.GetAirstrikeCooldown() ?? 0f);
				guiInfo.SetArtilleryCooldown(_globalMap?.GetArtilleryCooldown() ?? 0f);
			}
			EnsureLinesPanel();
			if (_linesPanel != null)
			{
				((Control)_linesPanel).Visible = flag;
			}
			if (!flag)
			{
				_placement?.CancelPlacement();
				_botControl?.ClearSelection();
			}
		}
	}

	private void EnsureLinesPanel()
	{
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen == null || activeScreen.GetWidget<MainViewport>() == null)
		{
			return;
		}
		LayoutContainer val;
		try
		{
			val = ((Control)activeScreen).FindControl<LayoutContainer>("ViewportContainer");
		}
		catch (ArgumentException)
		{
			return;
		}
		if (_linesPanel == null)
		{
			_linesPanel = new CivCommanderLinesGui();
			_linesPanel.LineColorSelected += OnLineColorSelected;
			_linesPanel.ClearLinesPressed += OnClearLinesPressed;
			_linesPanel.LabelPressed += OnLabelPressed;
			_linesPanel.PresetLabelPressed += OnPresetLabelPressed;
			if (_lines != null)
			{
				_linesPanel.UpdateSelectedColor(_lines.SelectedColor);
			}
		}
		if ((object)((Control)_linesPanel).Parent != val)
		{
			((Control)_linesPanel).Orphan();
			((Control)val).AddChild((Control)(object)_linesPanel);
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_linesPanel, (LayoutPreset)2, (LayoutPresetMode)0, 8);
			LayoutContainer.SetGrowHorizontal((Control)(object)_linesPanel, (GrowDirection)0);
			LayoutContainer.SetGrowVertical((Control)(object)_linesPanel, (GrowDirection)1);
		}
		((Control)_linesPanel).SetPositionLast();
	}

	private void DisposeLinesPanel()
	{
		if (_linesPanel != null)
		{
			_linesPanel.LineColorSelected -= OnLineColorSelected;
			_linesPanel.ClearLinesPressed -= OnClearLinesPressed;
			_linesPanel.LabelPressed -= OnLabelPressed;
			_linesPanel.PresetLabelPressed -= OnPresetLabelPressed;
			((Control)_linesPanel).Orphan();
			_linesPanel = null;
		}
	}

	private void LoadGui()
	{
		if (Gui != null && !_loaded)
		{
			Gui.HeadquartersPressed += OnHeadquartersPressed;
			Gui.ShopPressed += OnShopPressed;
			Gui.AlliesPressed += OnAlliesPressed;
			Gui.FireSupportPressed += OnFireSupportPressed;
			Gui.HeliPressed += OnHeliPressed;
			Gui.SupplyPressed += OnSupplyPressed;
			((Control)Gui).Visible = IsCommander();
			CivCommanderInfoGui guiInfo = GuiInfo;
			if (guiInfo != null)
			{
				guiInfo.BomPressed += OnBomPressed;
				((Control)guiInfo).Visible = IsCommander();
			}
			_loaded = true;
		}
	}

	private void UnloadGui()
	{
		if (Gui != null && _loaded)
		{
			Gui.HeadquartersPressed -= OnHeadquartersPressed;
			Gui.ShopPressed -= OnShopPressed;
			Gui.AlliesPressed -= OnAlliesPressed;
			Gui.FireSupportPressed -= OnFireSupportPressed;
			Gui.HeliPressed -= OnHeliPressed;
			Gui.SupplyPressed -= OnSupplyPressed;
			((Control)Gui).Visible = false;
			CivCommanderInfoGui guiInfo = GuiInfo;
			if (guiInfo != null)
			{
				guiInfo.BomPressed -= OnBomPressed;
				((Control)guiInfo).Visible = false;
			}
			_loaded = false;
		}
		_placement?.CancelPlacement();
		_botControl?.ClearSelection();
		_globalMap?.CloseCommanderShopWindow();
		_teleport?.CloseWindow();
		_supplyRefill?.CloseWindow();
	}

	private void OnHeadquartersPressed()
	{
		_globalMap?.OpenCommanderWindow();
	}

	private void OnAlliesPressed()
	{
		_teleport?.OpenWindow();
	}

	private void OnShopPressed()
	{
		_globalMap?.OpenCommanderShopWindow();
	}

	private void OnSupplyPressed()
	{
		_supplyRefill?.OpenWindow();
	}

	private void OnFireSupportPressed()
	{
		_airstrike?.OpenWindow(_globalMap?.GetAirstrikeCooldown() ?? 0f, _globalMap?.GetArtilleryCooldown() ?? 0f, _globalMap?.GetSmokeSupportCooldown() ?? 0f);
	}

	private void OnHeliPressed()
	{
		if (IsCommander())
		{
			_globalMap?.RequestCommanderCallHeli();
		}
	}

	private void OnBomPressed()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!IsCommander())
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			MapCoordinates mapCoordinates = base.EntityManager.System<SharedTransformSystem>().GetMapCoordinates(valueOrDefault, (TransformComponent)null);
			if (!(mapCoordinates.MapId == MapId.Nullspace))
			{
				_airstrike?.RequestArtillery(mapCoordinates.Position);
			}
		}
	}

	private void OnLineColorSelected(CivCommanderLineColor color)
	{
		if (_lines != null)
		{
			_lines.SelectedColor = color;
		}
	}

	private void OnClearLinesPressed()
	{
		_lines?.RequestClearAll();
	}

	private void OnLabelPressed()
	{
		if (_lines != null && IsCommander())
		{
			if (_labelWindow != null)
			{
				((BaseWindow)_labelWindow).Close();
				_labelWindow = null;
			}
			_labelWindow = new CivCommanderLabelInputWindow();
			_labelWindow.TextConfirmed += delegate(string text)
			{
				_lines.StartLabelPlacement(text);
			};
			((BaseWindow)_labelWindow).OnClose += delegate
			{
				_labelWindow = null;
			};
			((BaseWindow)_labelWindow).OpenCentered();
		}
	}

	private void OnPresetLabelPressed(string text)
	{
		if (_lines != null && IsCommander())
		{
			_lines.StartLabelPlacement(text);
		}
	}

	private bool IsCommander()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (base.EntityManager.TryGetComponent<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent))
			{
				return civTeamMemberComponent.IsCommander;
			}
		}
		return false;
	}
}
