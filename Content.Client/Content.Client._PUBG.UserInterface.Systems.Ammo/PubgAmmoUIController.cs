using System;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.Systems.Ammo;

public sealed class PubgAmmoUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private const int AmmoAnchorMargin = 15;

	private const float AmmoMarginBottom = -200f;

	private const float AmmoMarginLeft = 70f;

	private bool _ammoVisible;

	private bool _systemSubscribed;

	private PanelContainer? _ammoPanel;

	private Label? _currentAmmoLabel;

	private Label? _reserveAmmoLabel;

	private Label? _ammoTypeLabel;

	private bool _hasSnapshot;

	private int _snapshotCurrent;

	private int _snapshotReserve;

	private string _snapshotType = "";

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, (Action)delegate
		{
			RequestRefresh();
			if (_ammoVisible)
			{
				EnsureUI();
			}
		});
	}

	public void OnStateEntered(GameplayState state)
	{
		EnsureSystemSubscribed();
		RequestRefresh();
		if (_ammoVisible)
		{
			EnsureUI();
		}
	}

	public void OnStateExited(GameplayState state)
	{
		UnsubscribeFromSystem();
		PanelContainer? ammoPanel = _ammoPanel;
		if (ammoPanel != null)
		{
			((Control)ammoPanel).Orphan();
		}
		_ammoPanel = null;
		_currentAmmoLabel = null;
		_reserveAmmoLabel = null;
		_ammoTypeLabel = null;
	}

	private void EnsureSystemSubscribed()
	{
		if (!_systemSubscribed)
		{
			base.EntityManager.System<PubgAmmoUiSystem>().AmmoUpdated += OnAmmoUpdate;
			_systemSubscribed = true;
		}
	}

	private void UnsubscribeFromSystem()
	{
		if (_systemSubscribed)
		{
			PubgAmmoUiSystem pubgAmmoUiSystem = base.EntityManager.SystemOrNull<PubgAmmoUiSystem>();
			if (pubgAmmoUiSystem != null)
			{
				pubgAmmoUiSystem.AmmoUpdated -= OnAmmoUpdate;
			}
			_systemSubscribed = false;
		}
	}

	private void RequestRefresh()
	{
		EnsureSystemSubscribed();
		base.EntityManager.System<PubgAmmoUiSystem>().RequestRefresh();
	}

	private void EnsureUI()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
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
		if (_ammoPanel == null)
		{
			BoxContainer val2 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 5
			};
			_currentAmmoLabel = new Label
			{
				Text = "30",
				FontColorOverride = Color.White,
				MinWidth = 60f,
				HorizontalAlignment = (HAlignment)3
			};
			((Control)_currentAmmoLabel).SetOnlyStyleClass("LabelHeading");
			Label val3 = new Label
			{
				Text = "/",
				FontColorOverride = Color.Gray
			};
			_reserveAmmoLabel = new Label
			{
				Text = "90",
				FontColorOverride = Color.Gray,
				MinWidth = 60f
			};
			_ammoTypeLabel = new Label
			{
				Text = "",
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#ffa500", (Color?)null),
				Margin = new Thickness(10f, 0f, 0f, 0f)
			};
			((Control)val2).AddChild((Control)(object)_currentAmmoLabel);
			((Control)val2).AddChild((Control)(object)val3);
			((Control)val2).AddChild((Control)(object)_reserveAmmoLabel);
			((Control)val2).AddChild((Control)(object)_ammoTypeLabel);
			_ammoPanel = new PanelContainer();
			StyleBoxFlat val4 = new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1a1a1aDD", (Color?)null),
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#ffa500", (Color?)null),
				BorderThickness = new Thickness(0f, 0f, 3f, 0f)
			};
			((StyleBox)val4).SetContentMarginOverride((Margin)15, 10f);
			_ammoPanel.PanelOverride = (StyleBox)(object)val4;
			((Control)_ammoPanel).AddChild((Control)(object)val2);
			((Control)val).AddChild((Control)(object)_ammoPanel);
			ApplyAmmoLayout(_ammoPanel);
		}
		PanelContainer? ammoPanel = _ammoPanel;
		if ((object)((ammoPanel != null) ? ((Control)ammoPanel).Parent : null) != val)
		{
			PanelContainer? ammoPanel2 = _ammoPanel;
			if (ammoPanel2 != null)
			{
				((Control)ammoPanel2).Orphan();
			}
			if (_ammoPanel != null)
			{
				((Control)val).AddChild((Control)(object)_ammoPanel);
				ApplyAmmoLayout(_ammoPanel);
			}
		}
		PanelContainer? ammoPanel3 = _ammoPanel;
		if (ammoPanel3 != null)
		{
			((Control)ammoPanel3).SetPositionLast();
		}
		ApplySnapshotToLabels();
	}

	private void ApplySnapshotToLabels()
	{
		if (_hasSnapshot)
		{
			if (_currentAmmoLabel != null)
			{
				_currentAmmoLabel.Text = _snapshotCurrent.ToString();
			}
			if (_reserveAmmoLabel != null)
			{
				_reserveAmmoLabel.Text = _snapshotReserve.ToString();
			}
			if (_ammoTypeLabel != null)
			{
				_ammoTypeLabel.Text = _snapshotType;
			}
		}
	}

	private void ApplyAmmoLayout(PanelContainer panel)
	{
		LayoutContainer.SetAnchorAndMarginPreset((Control)(object)panel, (LayoutPreset)2, (LayoutPresetMode)0, 15);
		LayoutContainer.SetMarginBottom((Control)(object)panel, -200f);
		LayoutContainer.SetMarginLeft((Control)(object)panel, 70f);
	}

	private void OnAmmoUpdate(PubgAmmoUpdateEvent msg)
	{
		if (msg.CurrentAmmo == 0 && msg.MaxAmmo == 0 && msg.ReserveAmmo == 0)
		{
			HideAmmoUI();
			return;
		}
		_hasSnapshot = true;
		_snapshotCurrent = msg.CurrentAmmo;
		_snapshotReserve = msg.ReserveAmmo;
		_snapshotType = msg.AmmoType;
		_ammoVisible = true;
		EnsureUI();
		if (_currentAmmoLabel != null)
		{
			_currentAmmoLabel.Text = msg.CurrentAmmo.ToString();
		}
		if (_reserveAmmoLabel != null)
		{
			_reserveAmmoLabel.Text = msg.ReserveAmmo.ToString();
		}
		if (_ammoTypeLabel != null)
		{
			_ammoTypeLabel.Text = msg.AmmoType;
		}
	}

	public void HideAmmoUI()
	{
		_ammoVisible = false;
		_hasSnapshot = false;
		PanelContainer? ammoPanel = _ammoPanel;
		if (ammoPanel != null)
		{
			((Control)ammoPanel).Orphan();
		}
		_ammoPanel = null;
		_currentAmmoLabel = null;
		_reserveAmmoLabel = null;
		_ammoTypeLabel = null;
	}
}
