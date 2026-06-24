using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.Connection;

public sealed class QueueOverlay : Control
{
	private readonly PanelContainer _panel;

	private readonly Label _titleLabel;

	private readonly Label _positionLabel;

	private readonly Label _infoLabel;

	private bool _accepted;

	private TimeSpan _acceptedTime;

	public QueueOverlay()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0081: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		((Control)this).VerticalAlignment = (VAlignment)2;
		((Control)this).HorizontalAlignment = (HAlignment)2;
		_panel = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = new Color(0f, 0f, 0f, 0.85f),
				ContentMarginLeftOverride = 40f,
				ContentMarginRightOverride = 40f,
				ContentMarginTopOverride = 30f,
				ContentMarginBottomOverride = 30f
			}
		};
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2
		};
		_titleLabel = new Label
		{
			Text = Loc.GetString("pubg-queue-title"),
			HorizontalAlignment = (HAlignment)2,
			StyleClasses = { "LabelHeadingBigger" },
			Margin = new Thickness(0f, 0f, 0f, 20f)
		};
		_positionLabel = new Label
		{
			Text = "0 / 0",
			HorizontalAlignment = (HAlignment)2,
			StyleClasses = { "LabelHeadingBigger" },
			Margin = new Thickness(0f, 0f, 0f, 10f)
		};
		_infoLabel = new Label
		{
			Text = Loc.GetString("pubg-queue-waiting"),
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 10f, 0f, 0f)
		};
		((Control)val).AddChild((Control)(object)_titleLabel);
		((Control)val).AddChild((Control)(object)_positionLabel);
		((Control)val).AddChild((Control)(object)_infoLabel);
		((Control)_panel).AddChild((Control)(object)val);
		((Control)this).AddChild((Control)(object)_panel);
	}

	public void UpdatePosition(int position, int total)
	{
		if (!_accepted)
		{
			_positionLabel.Text = $"{position} / {total}";
		}
	}

	public void ShowAccepted()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		_accepted = true;
		_acceptedTime = IoCManager.Resolve<IGameTiming>().CurTime;
		_titleLabel.Text = Loc.GetString("pubg-queue-accepted-title");
		((Control)_positionLabel).Visible = false;
		_infoLabel.Text = Loc.GetString("pubg-queue-accepted");
		((Control)_infoLabel).Modulate = Color.LimeGreen;
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_accepted && IoCManager.Resolve<IGameTiming>().CurTime - _acceptedTime > TimeSpan.FromSeconds(3L))
		{
			((Control)this).Orphan();
		}
	}
}
