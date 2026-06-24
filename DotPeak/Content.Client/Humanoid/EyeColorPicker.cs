// Decompiled with JetBrains decompiler
// Type: Content.Client.Humanoid.EyeColorPicker
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Humanoid;

public sealed class EyeColorPicker : Control
{
  private readonly ColorSelectorSliders _colorSelectors;
  private Color _lastColor;

  public event Action<Color>? OnEyeColorPicked;

  public void SetData(Color color)
  {
    this._lastColor = color;
    this._colorSelectors.Color = color;
  }

  public EyeColorPicker()
  {
    BoxContainer boxContainer = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    this.AddChild((Control) boxContainer);
    ((Control) boxContainer).AddChild((Control) (this._colorSelectors = new ColorSelectorSliders()));
    this._colorSelectors.SelectorType = (ColorSelectorSliders.ColorSelectorType) 1;
    this._colorSelectors.OnColorChanged += new Action<Color>(this.ColorValueChanged);
  }

  private void ColorValueChanged(Color newColor)
  {
    Action<Color> onEyeColorPicked = this.OnEyeColorPicked;
    if (onEyeColorPicked != null)
      onEyeColorPicked(newColor);
    this._lastColor = newColor;
  }
}
