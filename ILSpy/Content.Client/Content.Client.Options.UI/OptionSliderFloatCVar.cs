using System;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;

namespace Content.Client.Options.UI;

public sealed class OptionSliderFloatCVar : BaseOptionCVar<float>
{
	private readonly OptionSlider _slider;

	private readonly Func<OptionSliderFloatCVar, float, string> _format;

	public float Scale { get; }

	protected override float Value
	{
		get
		{
			return ((Range)_slider.Slider).Value * Scale;
		}
		set
		{
			((Range)_slider.Slider).Value = value / Scale;
			UpdateLabelValue();
		}
	}

	public OptionSliderFloatCVar(OptionsTabControlRow controller, IConfigurationManager cfg, CVarDef<float> cVar, OptionSlider slider, float minValue, float maxValue, float scale, Func<OptionSliderFloatCVar, float, string> format)
		: base(controller, cfg, cVar)
	{
		Scale = scale;
		_slider = slider;
		_format = format;
		((Range)slider.Slider).MinValue = minValue;
		((Range)slider.Slider).MaxValue = maxValue;
		((Range)slider.Slider).OnValueChanged += delegate
		{
			ValueChanged();
			UpdateLabelValue();
		};
	}

	private void UpdateLabelValue()
	{
		_slider.ValueLabel.Text = _format(this, ((Range)_slider.Slider).Value);
	}
}
