using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliRoutePanel : PanelContainer
{
	private readonly Label _pointsLabel;

	private readonly Label _etaLabel;

	private readonly Button _launch;

	public event Action? LaunchPressed;

	public event Action? BackPressed;

	public CivHeliRoutePanel()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		((PanelContainer)this).PanelOverride = (StyleBox)new StyleBoxFlat
		{
			BackgroundColor = new Color((byte)17, (byte)24, (byte)39, (byte)230)
		};
		((Control)this).Visible = false;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(10f, 8f)
		};
		((Control)val).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-heli-route-hint"),
			FontColorOverride = new Color((byte)229, (byte)231, (byte)235, byte.MaxValue)
		});
		_pointsLabel = new Label
		{
			FontColorOverride = new Color((byte)156, (byte)163, (byte)175, byte.MaxValue)
		};
		((Control)val).AddChild((Control)(object)_pointsLabel);
		_etaLabel = new Label
		{
			FontColorOverride = new Color((byte)74, (byte)222, (byte)128, byte.MaxValue)
		};
		((Control)val).AddChild((Control)(object)_etaLabel);
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Margin = new Thickness(0f, 6f, 0f, 0f)
		};
		_launch = new Button
		{
			Text = Loc.GetString("civ-heli-route-launch"),
			MinSize = new Vector2(120f, 32f)
		};
		((BaseButton)_launch).OnPressed += delegate
		{
			this.LaunchPressed?.Invoke();
		};
		((Control)val2).AddChild((Control)(object)_launch);
		Button val3 = new Button
		{
			Text = Loc.GetString("civ-heli-route-back"),
			MinSize = new Vector2(120f, 32f),
			Margin = new Thickness(6f, 0f, 0f, 0f)
		};
		((BaseButton)val3).OnPressed += delegate
		{
			this.BackPressed?.Invoke();
		};
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		((Control)this).AddChild((Control)(object)val);
	}

	public void SetPointCount(int count)
	{
		_pointsLabel.Text = Loc.GetString("civ-heli-route-points", new(string, object)[1] { ("count", count) });
	}

	public void SetEta(float? seconds)
	{
		((Control)_etaLabel).Visible = seconds.HasValue;
		if (seconds.HasValue)
		{
			_etaLabel.Text = Loc.GetString("civ-heli-route-eta", new(string, object)[1] { ("eta", (int)MathF.Round(seconds.Value)) });
		}
	}

	public void SetCost(int cost)
	{
		_launch.Text = ((cost > 0) ? Loc.GetString("civ-heli-route-launch-cost", new(string, object)[1] { ("cost", cost) }) : Loc.GetString("civ-heli-route-launch"));
	}
}
