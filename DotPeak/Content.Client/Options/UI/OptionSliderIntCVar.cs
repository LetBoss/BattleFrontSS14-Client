// Decompiled with JetBrains decompiler
// Type: Content.Client.Options.UI.OptionSliderIntCVar
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using System;

#nullable enable
namespace Content.Client.Options.UI;

public sealed class OptionSliderIntCVar : BaseOptionCVar<int>
{
  private readonly OptionSlider _slider;
  private readonly Func<OptionSliderIntCVar, int, string> _format;

  protected override int Value
  {
    get => (int) ((Range) this._slider.Slider).Value;
    set
    {
      ((Range) this._slider.Slider).Value = (float) value;
      this.UpdateLabelValue();
    }
  }

  public OptionSliderIntCVar(
    OptionsTabControlRow controller,
    IConfigurationManager cfg,
    CVarDef<int> cVar,
    OptionSlider slider,
    int minValue,
    int maxValue,
    Func<OptionSliderIntCVar, int, string> format)
    : base(controller, cfg, cVar)
  {
    this._slider = slider;
    this._format = format;
    ((Range) slider.Slider).MinValue = (float) minValue;
    ((Range) slider.Slider).MaxValue = (float) maxValue;
    ((Range) slider.Slider).Rounded = true;
    ((Range) slider.Slider).OnValueChanged += (Action<Range>) (_ =>
    {
      this.ValueChanged();
      this.UpdateLabelValue();
    });
  }

  private void UpdateLabelValue()
  {
    this._slider.ValueLabel.Text = this._format(this, (int) ((Range) this._slider.Slider).Value);
  }
}
