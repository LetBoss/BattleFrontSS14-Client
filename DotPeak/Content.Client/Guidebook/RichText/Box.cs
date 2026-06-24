// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.Richtext.Box
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.Guidebook.Richtext;

public sealed class Box : BoxContainer, IDocumentTag
{
  public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
  {
    ((Control) this).HorizontalExpand = true;
    control = (Control) this;
    string s;
    if (args.TryGetValue("Margin", out s))
      ((Control) this).Margin = new Thickness(float.Parse(s));
    string str1;
    if (args.TryGetValue("Orientation", out str1))
      this.Orientation = Enum.Parse<BoxContainer.LayoutOrientation>(str1);
    else
      this.Orientation = (BoxContainer.LayoutOrientation) 0;
    string str2;
    if (args.TryGetValue("HorizontalAlignment", out str2))
      ((Control) this).HorizontalAlignment = Enum.Parse<Control.HAlignment>(str2);
    else
      ((Control) this).HorizontalAlignment = (Control.HAlignment) 2;
    string str3;
    if (args.TryGetValue("VerticalAlignment", out str3))
      ((Control) this).VerticalAlignment = Enum.Parse<Control.VAlignment>(str3);
    return true;
  }
}
