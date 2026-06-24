using System;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Humanoid;

public sealed class EyeColorPicker : Control
{
	private readonly ColorSelectorSliders _colorSelectors;

	private Color _lastColor;

	public event Action<Color>? OnEyeColorPicked;

	public void SetData(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_lastColor = color;
		_colorSelectors.Color = color;
	}

	public EyeColorPicker()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0028: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		((Control)this).AddChild((Control)(object)val);
		ColorSelectorSliders val2 = new ColorSelectorSliders();
		ColorSelectorSliders val3 = val2;
		_colorSelectors = val2;
		((Control)val).AddChild((Control)(object)val3);
		_colorSelectors.SelectorType = (ColorSelectorType)1;
		ColorSelectorSliders colorSelectors = _colorSelectors;
		colorSelectors.OnColorChanged = (Action<Color>)Delegate.Combine(colorSelectors.OnColorChanged, new Action<Color>(ColorValueChanged));
	}

	private void ColorValueChanged(Color newColor)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		this.OnEyeColorPicked?.Invoke(newColor);
		_lastColor = newColor;
	}
}
