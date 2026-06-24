using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using Content.Shared.Configurable;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.Configurable.UI;

public sealed class ConfigurationBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private ConfigurationMenu? _menu;

	public ConfigurationBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<ConfigurationMenu>((BoundUserInterface)(object)this);
		_menu.OnConfiguration += SendConfiguration;
		ConfigurationComponent item = default(ConfigurationComponent);
		if (base.EntMan.TryGetComponent<ConfigurationComponent>(((BoundUserInterface)this).Owner, ref item))
		{
			Refresh(Entity<ConfigurationComponent>.op_Implicit((((BoundUserInterface)this).Owner, item)));
		}
	}

	public void Refresh(Entity<ConfigurationComponent> entity)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		if (_menu == null)
		{
			return;
		}
		((Control)_menu.Column).Children.Clear();
		_menu.Inputs.Clear();
		foreach (KeyValuePair<string, string> item in entity.Comp.Config)
		{
			Label val = new Label
			{
				Margin = new Thickness(0f, 0f, 8f, 0f),
				Name = item.Key,
				Text = item.Key + ":",
				VerticalAlignment = (VAlignment)2,
				HorizontalExpand = true,
				SizeFlagsStretchRatio = 0.2f,
				MinSize = new Vector2(60f, 0f)
			};
			LineEdit val2 = new LineEdit
			{
				Name = item.Key + "-input",
				Text = (item.Value ?? ""),
				IsValid = _menu.Validate,
				HorizontalExpand = true,
				SizeFlagsStretchRatio = 0.8f
			};
			_menu.Inputs.Add((item.Key, val2));
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			ConfigurationMenu.CopyProperties<BoxContainer>(_menu.Row, val3);
			((Control)val3).AddChild((Control)(object)val);
			((Control)val3).AddChild((Control)(object)val2);
			((Control)_menu.Column).AddChild((Control)(object)val3);
		}
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		((BoundUserInterface)this).ReceiveMessage(message);
		if (_menu != null && message is ConfigurationComponent.ValidationUpdateMessage validationUpdateMessage)
		{
			_menu.Validation = new Regex(validationUpdateMessage.ValidationString, RegexOptions.Compiled);
		}
	}

	public void SendConfiguration(Dictionary<string, string> config)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ConfigurationComponent.ConfigurationUpdatedMessage(config));
	}
}
