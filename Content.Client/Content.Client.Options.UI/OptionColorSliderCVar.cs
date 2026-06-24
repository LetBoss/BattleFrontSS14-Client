using System;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Maths;

namespace Content.Client.Options.UI;

public sealed class OptionColorSliderCVar : BaseOptionCVar<string>
{
	private readonly OptionColorSlider _slider;

	protected override string Value
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Color color = _slider.Slider.Color;
			return ((Color)(ref color)).ToHex();
		}
		set
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			_slider.Slider.Color = Color.FromHex((ReadOnlySpan<char>)value, (Color?)null);
			UpdateLabelColor();
		}
	}

	public OptionColorSliderCVar(OptionsTabControlRow controller, IConfigurationManager cfg, CVarDef<string> cVar, OptionColorSlider slider)
		: base(controller, cfg, cVar)
	{
		_slider = slider;
		ColorSelectorSliders slider2 = slider.Slider;
		slider2.OnColorChanged = (Action<Color>)Delegate.Combine(slider2.OnColorChanged, (Action<Color>)delegate
		{
			ValueChanged();
			UpdateLabelColor();
		});
	}

	private void UpdateLabelColor()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_slider.ExampleLabel.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)Value, (Color?)null);
	}
}
