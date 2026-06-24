using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._PUBG.Ghost;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.Ghost;

public sealed class PubgGhostFollowSelectWindow : DefaultWindow
{
	private readonly BoxContainer _optionsContainer;

	public event Action<NetEntity>? FollowRequested;

	public PubgGhostFollowSelectWindow()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("pubg-ghost-follow-window-title");
		Label val = new Label
		{
			Text = Loc.GetString("pubg-ghost-follow-window-subtitle")
		};
		_optionsContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		ScrollContainer val2 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			MinSize = new Vector2(320f, 240f),
			HScrollEnabled = false
		};
		((Control)val2).AddChild((Control)(object)_optionsContainer);
		Button val3 = new Button
		{
			Text = Loc.GetString("pubg-ghost-follow-window-close")
		};
		((BaseButton)val3).OnPressed += delegate
		{
			((BaseWindow)this).Close();
		};
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			Margin = new Thickness(12f)
		};
		((Control)val4).AddChild((Control)(object)val);
		((Control)val4).AddChild((Control)(object)val2);
		((Control)val4).AddChild((Control)(object)val3);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val4);
	}

	public void SetOptions(IReadOnlyList<PubgGhostFollowTeammateOptionState> options)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		((Control)_optionsContainer).RemoveAllChildren();
		if (options.Count == 0)
		{
			((Control)_optionsContainer).AddChild((Control)new Label
			{
				Text = Loc.GetString("pubg-ghost-follow-window-empty")
			});
			return;
		}
		foreach (PubgGhostFollowTeammateOptionState option in options)
		{
			NetEntity entity = option.Entity;
			Button val = new Button
			{
				Text = option.Name,
				HorizontalExpand = true
			};
			((BaseButton)val).OnPressed += delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				this.FollowRequested?.Invoke(entity);
			};
			((Control)_optionsContainer).AddChild((Control)(object)val);
		}
	}
}
