using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.Sponsor.UI;

public sealed class PubgSponsorDisplayWindow : DefaultWindow
{
	private SponsorTierInfo? _displayTier;

	private List<SponsorActiveTierInfo> _activeTiers = new List<SponsorActiveTierInfo>();

	private SponsorDisplayMode _displayMode;

	private string? _preferredTierKey;

	private bool _hasData;

	private bool _isUpdating;

	private readonly Label _currentLabel;

	private readonly Label _updatingLabel;

	private readonly Button _autoButton;

	private readonly Button _hiddenButton;

	private readonly BoxContainer _manualContainer;

	public event Action<SponsorDisplayMode, string?>? SelectionRequested;

	public PubgSponsorDisplayWindow()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Expected O, but got Unknown
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Expected O, but got Unknown
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("mainmenu-sponsor-display-title");
		((Control)this).MinSize = new Vector2(560f, 520f);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true,
			Margin = new Thickness(12f),
			SeparationOverride = 8
		};
		((Control)val).AddChild((Control)new Label
		{
			Text = Loc.GetString("mainmenu-sponsor-display-subtitle"),
			HorizontalAlignment = (HAlignment)2,
			FontColorOverride = Color.Gray
		});
		_currentLabel = new Label
		{
			Text = string.Empty,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 2f, 0f, 0f)
		};
		((Control)val).AddChild((Control)(object)_currentLabel);
		_updatingLabel = new Label
		{
			Text = Loc.GetString("mainmenu-sponsor-display-updating"),
			HorizontalAlignment = (HAlignment)2,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#88AAFF", (Color?)null),
			Visible = false
		};
		((Control)val).AddChild((Control)(object)_updatingLabel);
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)2,
			SeparationOverride = 8,
			Margin = new Thickness(0f, 2f, 0f, 4f)
		};
		_autoButton = new Button();
		((BaseButton)_autoButton).OnPressed += delegate
		{
			this.SelectionRequested?.Invoke(SponsorDisplayMode.Auto, null);
		};
		((Control)val2).AddChild((Control)(object)_autoButton);
		_hiddenButton = new Button();
		((BaseButton)_hiddenButton).OnPressed += delegate
		{
			this.SelectionRequested?.Invoke(SponsorDisplayMode.Hidden, null);
		};
		((Control)val2).AddChild((Control)(object)_hiddenButton);
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)new Label
		{
			Text = Loc.GetString("mainmenu-sponsor-display-list-title"),
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 4f, 0f, 0f)
		});
		ScrollContainer val3 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			HScrollEnabled = false,
			VScrollEnabled = true
		};
		_manualContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			SeparationOverride = 4
		};
		((Control)val3).AddChild((Control)(object)_manualContainer);
		((Control)val).AddChild((Control)(object)val3);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}

	public void UpdateState(SponsorTierInfo? displayTier, List<SponsorActiveTierInfo> activeTiers, SponsorDisplayMode displayMode, string? preferredTierKey, bool hasData, bool isUpdating)
	{
		_displayTier = displayTier;
		_activeTiers = activeTiers;
		_displayMode = displayMode;
		_preferredTierKey = preferredTierKey;
		_hasData = hasData;
		_isUpdating = isUpdating;
		RefreshUi();
	}

	private void RefreshUi()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Expected O, but got Unknown
		if (!_hasData)
		{
			_currentLabel.Text = Loc.GetString("mainmenu-sponsor-display-loading");
			((Control)_updatingLabel).Visible = false;
			_autoButton.Text = Loc.GetString("mainmenu-sponsor-display-mode-auto");
			_hiddenButton.Text = Loc.GetString("mainmenu-sponsor-display-mode-hidden");
			((BaseButton)_autoButton).Disabled = true;
			((BaseButton)_hiddenButton).Disabled = true;
			((Control)_manualContainer).RemoveAllChildren();
			((Control)_manualContainer).AddChild((Control)new Label
			{
				Text = Loc.GetString("mainmenu-sponsor-display-loading"),
				HorizontalAlignment = (HAlignment)2,
				FontColorOverride = Color.Gray
			});
			return;
		}
		string item = Loc.GetString("mainmenu-sponsor-display-current-none");
		if (_displayMode == SponsorDisplayMode.Hidden)
		{
			item = Loc.GetString("mainmenu-sponsor-display-current-hidden");
		}
		else if (_displayTier != null)
		{
			item = _displayTier.TierName;
		}
		_currentLabel.Text = Loc.GetString("mainmenu-sponsor-display-current", new(string, object)[1] { ("value", item) });
		((Control)_updatingLabel).Visible = _isUpdating;
		string text = Loc.GetString("mainmenu-selected-prefix");
		bool flag = _displayMode == SponsorDisplayMode.Auto;
		bool flag2 = _displayMode == SponsorDisplayMode.Hidden;
		string text2 = Loc.GetString("mainmenu-sponsor-display-mode-auto");
		_autoButton.Text = (flag ? (text + text2) : text2);
		((BaseButton)_autoButton).Disabled = _isUpdating || flag;
		string text3 = Loc.GetString("mainmenu-sponsor-display-mode-hidden");
		_hiddenButton.Text = (flag2 ? (text + text3) : text3);
		((BaseButton)_hiddenButton).Disabled = _isUpdating || flag2;
		((Control)_manualContainer).RemoveAllChildren();
		if (_activeTiers.Count == 0)
		{
			((Control)_manualContainer).AddChild((Control)new Label
			{
				Text = Loc.GetString("mainmenu-sponsor-display-no-tiers"),
				HorizontalAlignment = (HAlignment)2,
				FontColorOverride = Color.Gray
			});
			return;
		}
		foreach (SponsorActiveTierInfo activeTier in _activeTiers)
		{
			bool flag3 = _displayMode == SponsorDisplayMode.Manual && string.Equals(_preferredTierKey, activeTier.Key, StringComparison.OrdinalIgnoreCase);
			string text4 = Loc.GetString("mainmenu-sponsor-display-mode-manual", new(string, object)[1] { ("tier", activeTier.Name) });
			Button val = new Button
			{
				Text = (flag3 ? (text + text4) : text4),
				Disabled = (_isUpdating || flag3),
				HorizontalExpand = true
			};
			string tierKey = activeTier.Key;
			((BaseButton)val).OnPressed += delegate
			{
				this.SelectionRequested?.Invoke(SponsorDisplayMode.Manual, tierKey);
			};
			((Control)_manualContainer).AddChild((Control)(object)val);
		}
	}
}
