using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.Configurable.UI;

public sealed class ConfigurationMenu : DefaultWindow
{
	public readonly BoxContainer Column;

	public readonly BoxContainer Row;

	public readonly List<(string name, LineEdit input)> Inputs;

	[ViewVariables]
	public Regex? Validation { get; internal set; }

	public event Action<Dictionary<string, string>>? OnConfiguration;

	public ConfigurationMenu()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		Vector2 minSize = (((Control)this).SetSize = new Vector2(300f, 250f));
		((Control)this).MinSize = minSize;
		Inputs = new List<(string, LineEdit)>();
		((DefaultWindow)this).Title = Loc.GetString("configuration-menu-device-title");
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			VerticalExpand = true,
			HorizontalExpand = true
		};
		Column = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(8f),
			SeparationOverride = 16
		};
		Row = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 16,
			HorizontalExpand = true
		};
		Button val2 = new Button
		{
			Text = Loc.GetString("configuration-menu-confirm"),
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		((BaseButton)val2).OnButtonUp += OnConfirm;
		ScrollContainer val3 = new ScrollContainer
		{
			VerticalExpand = true,
			HorizontalExpand = true,
			ModulateSelfOverride = Color.FromHex((ReadOnlySpan<char>)"#202025", (Color?)null)
		};
		((Control)val3).AddChild((Control)(object)Column);
		((Control)val).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}

	private void OnConfirm(ButtonEventArgs args)
	{
		Dictionary<string, string> obj = GenerateDictionary(Inputs, "Text");
		this.OnConfiguration?.Invoke(obj);
		((BaseWindow)this).Close();
	}

	public bool Validate(string value)
	{
		return Validation?.IsMatch(value) ?? true;
	}

	private Dictionary<string, string> GenerateDictionary(IEnumerable<(string name, LineEdit input)> inputs, string propertyName)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (var input in inputs)
		{
			dictionary.Add(input.name, input.input.Text);
		}
		return dictionary;
	}

	public static void CopyProperties<T>(T from, T to) where T : Control
	{
		foreach (KeyValuePair<AttachedProperty, object> allAttachedProperty in ((Control)from).AllAttachedProperties)
		{
			((Control)to).SetValue(allAttachedProperty.Key, allAttachedProperty.Value);
		}
	}
}
