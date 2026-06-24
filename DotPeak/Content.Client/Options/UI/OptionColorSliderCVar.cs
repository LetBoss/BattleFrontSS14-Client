// Decompiled with JetBrains decompiler
// Type: Content.Client.Options.UI.OptionColorSliderCVar
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Configuration;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Options.UI;

public sealed class OptionColorSliderCVar : BaseOptionCVar<string>
{
  private readonly OptionColorSlider _slider;

  protected override string Value
  {
    get
    {
      Color color = this._slider.Slider.Color;
      return ((Color) ref color).ToHex();
    }
    set
    {
      this._slider.Slider.Color = Color.FromHex((ReadOnlySpan<char>) value, new Color?());
      this.UpdateLabelColor();
    }
  }

  public OptionColorSliderCVar(
    OptionsTabControlRow controller,
    IConfigurationManager cfg,
    CVarDef<string> cVar,
    OptionColorSlider slider)
    : base(controller, cfg, cVar)
  {
    this._slider = slider;
    slider.Slider.OnColorChanged += (Action<Color>) (_ =>
    {
      this.ValueChanged();
      this.UpdateLabelColor();
    });
  }

  private void UpdateLabelColor()
  {
    this._slider.ExampleLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) this.Value, new Color?()));
  }
}
