// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.Richtext.ColorBox
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.Guidebook.Richtext;

public sealed class ColorBox : PanelContainer, IDocumentTag
{
  public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
  {
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    control = (Control) this;
    string s1;
    if (args.TryGetValue("Margin", out s1))
      ((Control) this).Margin = new Thickness(float.Parse(s1));
    string str1;
    if (args.TryGetValue("HorizontalAlignment", out str1))
      ((Control) this).HorizontalAlignment = Enum.Parse<Control.HAlignment>(str1);
    else
      ((Control) this).HorizontalAlignment = (Control.HAlignment) 0;
    string str2;
    if (args.TryGetValue("VerticalAlignment", out str2))
      ((Control) this).VerticalAlignment = Enum.Parse<Control.VAlignment>(str2);
    else
      ((Control) this).VerticalAlignment = (Control.VAlignment) 0;
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    string str3;
    if (args.TryGetValue("Color", out str3))
      styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) str3, new Color?());
    string s2;
    styleBoxFlat.BorderThickness = !args.TryGetValue("OutlineThickness", out s2) ? new Thickness(1f) : new Thickness(float.Parse(s2));
    string str4;
    styleBoxFlat.BorderColor = !args.TryGetValue("OutlineColor", out str4) ? Color.White : Color.FromHex((ReadOnlySpan<char>) str4, new Color?());
    this.PanelOverride = (StyleBox) styleBoxFlat;
    return true;
  }
}
