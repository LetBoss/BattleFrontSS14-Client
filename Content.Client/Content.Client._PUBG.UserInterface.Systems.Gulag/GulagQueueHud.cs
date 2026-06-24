using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagQueueHud : PanelContainer
{
	private Label _titleLabel;

	private Label _positionLabel;

	private Label _infoLabel;

	private Button _hideButton;

	public event Action? HideRequested;

	public GulagQueueHud()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		StyleBoxFlat val = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#000000CC", (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)"#FFA500", (Color?)null),
			BorderThickness = new Thickness(3f)
		};
		((StyleBox)val).SetContentMarginOverride((Margin)15, 20f);
		((PanelContainer)this).PanelOverride = (StyleBox)(object)val;
		((Control)this).MinWidth = 400f;
		((Control)this).MinHeight = 200f;
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 15
		};
		_titleLabel = new Label
		{
			Text = Loc.GetString("gulag-ui-title"),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFA500", (Color?)null),
			HorizontalAlignment = (HAlignment)2
		};
		Label val3 = new Label();
		val3.Text = Loc.GetString("gulag-ui-queue-position", new(string, object)[2]
		{
			("position", "-"),
			("total", "-")
		});
		val3.FontColorOverride = Color.White;
		((Control)val3).HorizontalAlignment = (HAlignment)2;
		_positionLabel = val3;
		_infoLabel = new Label
		{
			Text = Loc.GetString("gulag-ui-queue-info").Replace("\\n", "\n"),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#AAAAAA", (Color?)null),
			HorizontalAlignment = (HAlignment)2
		};
		_hideButton = new Button
		{
			Text = Loc.GetString("gulag-ui-hide"),
			HorizontalAlignment = (HAlignment)2
		};
		((BaseButton)_hideButton).OnPressed += delegate
		{
			this.HideRequested?.Invoke();
		};
		((Control)val2).AddChild((Control)(object)_titleLabel);
		((Control)val2).AddChild((Control)(object)_positionLabel);
		((Control)val2).AddChild((Control)(object)_infoLabel);
		((Control)val2).AddChild((Control)(object)_hideButton);
		((Control)this).AddChild((Control)(object)val2);
	}

	public void UpdatePosition(int position, int total)
	{
		_positionLabel.Text = Loc.GetString("gulag-ui-queue-position", new(string, object)[2]
		{
			("position", position),
			("total", total)
		});
	}
}
