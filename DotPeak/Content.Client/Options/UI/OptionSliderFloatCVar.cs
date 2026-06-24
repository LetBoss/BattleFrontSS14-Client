// Decompiled with JetBrains decompiler
// Type: Content.Client.Options.UI.OptionSliderFloatCVar
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using System;

#nullable enable
namespace Content.Client.Options.UI;

public sealed class OptionSliderFloatCVar : BaseOptionCVar<float>
{
  private readonly OptionSlider _slider;
  private readonly Func<OptionSliderFloatCVar, float, string> _format;

  public float Scale { get; }

  protected override float Value
  {
    get => ((Range) this._slider.Slider).Value * this.Scale;
    set
    {
      ((Range) this._slider.Slider).Value = value / this.Scale;
      this.UpdateLabelValue();
    }
  }

  public OptionSliderFloatCVar(
    OptionsTabControlRow controller,
    IConfigurationManager cfg,
    CVarDef<float> cVar,
    OptionSlider slider,
    float minValue,
    float maxValue,
    float scale,
    Func<OptionSliderFloatCVar, float, string> format)
    : base(controller, cfg, cVar)
  {
    this.Scale = scale;
    this._slider = slider;
    this._format = format;
    ((Range) slider.Slider).MinValue = minValue;
    ((Range) slider.Slider).MaxValue = maxValue;
    ((Range) slider.Slider).OnValueChanged += (Action<Range>) (_ =>
    {
      this.ValueChanged();
      this.UpdateLabelValue();
    });
  }

  private void UpdateLabelValue()
  {
    this._slider.ValueLabel.Text = this._format(this, ((Range) this._slider.Slider).Value);
  }
}
