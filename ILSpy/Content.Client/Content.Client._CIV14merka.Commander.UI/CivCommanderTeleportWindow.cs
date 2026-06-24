using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Commander.UI;

public sealed class CivCommanderTeleportWindow : DefaultWindow
{
	private readonly LineEdit _search;

	private readonly ScrollContainer _scroll;

	private readonly BoxContainer _buttons;

	private readonly Label _emptyLabel;

	private readonly List<(string Name, NetEntity Entity)> _targets = new List<(string, NetEntity)>();

	public event Action<NetEntity>? TargetSelected;

	public CivCommanderTeleportWindow()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("civ-cmd-teleport-title");
		((Control)this).MinSize = new Vector2(500f, 560f);
		((Control)this).SetSize = new Vector2(560f, 700f);
		_search = new LineEdit
		{
			PlaceHolder = Loc.GetString("civ-cmd-teleport-search")
		};
		_scroll = new ScrollContainer
		{
			VerticalExpand = true,
			HScrollEnabled = false
		};
		_buttons = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4
		};
		_emptyLabel = new Label
		{
			Text = Loc.GetString("civ-cmd-teleport-empty"),
			HorizontalAlignment = (HAlignment)2
		};
		_search.OnTextChanged += delegate
		{
			Rebuild();
		};
		((Control)_scroll).AddChild((Control)(object)_buttons);
		Control contents = ((DefaultWindow)this).Contents;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			Margin = new Thickness(8f)
		};
		((Control)val).Children.Add((Control)(object)_search);
		((Control)val).Children.Add((Control)(object)_scroll);
		contents.AddChild((Control)val);
	}

	public void UpdateTargets(IEnumerable<(string Name, NetEntity Entity)> targets)
	{
		_targets.Clear();
		_targets.AddRange(targets);
		Rebuild();
	}

	private void Rebuild()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		((Control)_buttons).DisposeAllChildren();
		string text = _search.Text.Trim();
		bool flag = false;
		foreach (var target in _targets)
		{
			if (text.Length <= 0 || target.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
				Button val = new Button
				{
					Text = target.Name,
					HorizontalExpand = true,
					MinSize = new Vector2(420f, 30f),
					ClipText = true
				};
				NetEntity entity = target.Entity;
				((BaseButton)val).OnPressed += delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					this.TargetSelected?.Invoke(entity);
				};
				((Control)_buttons).AddChild((Control)(object)val);
			}
		}
		if (!flag)
		{
			((Control)_buttons).AddChild((Control)(object)_emptyLabel);
		}
		_scroll.SetScrollValue(Vector2.Zero);
	}
}
