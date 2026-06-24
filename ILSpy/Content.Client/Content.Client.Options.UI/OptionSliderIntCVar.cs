using System;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;

namespace Content.Client.Options.UI;

public sealed class OptionSliderIntCVar : BaseOptionCVar<int>
{
	private readonly OptionSlider _slider;

	private readonly Func<OptionSliderIntCVar, int, string> _format;

	protected override int Value
	{
		get
		{
			return (int)((Range)_slider.Slider).Value;
		}
		set
		{
			((Range)_slider.Slider).Value = value;
			UpdateLabelValue();
		}
	}

	public OptionSliderIntCVar(OptionsTabControlRow controller, IConfigurationManager cfg, CVarDef<int> cVar, OptionSlider slider, int minValue, int maxValue, Func<OptionSliderIntCVar, int, string> format)
		: base(controller, cfg, cVar)
	{
		_slider = slider;
		_format = format;
		((Range)slider.Slider).MinValue = minValue;
		((Range)slider.Slider).MaxValue = maxValue;
		((Range)slider.Slider).Rounded = true;
		((Range)slider.Slider).OnValueChanged += delegate
		{
			ValueChanged();
			UpdateLabelValue();
		};
	}

	private void UpdateLabelValue()
	{
		_slider.ValueLabel.Text = _format(this, (int)((Range)_slider.Slider).Value);
	}
}
