// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.CustomControls.HSeparator
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Administration.UI.CustomControls;

public sealed class HSeparator : Control
{
  private static readonly Color SeparatorColor = Color.FromHex((ReadOnlySpan<char>) "#3D4059", new Color?());
  private readonly StyleBoxFlat _styleBox;

  public Color Color
  {
    get => this._styleBox.BackgroundColor;
    set => this._styleBox.BackgroundColor = value;
  }

  public HSeparator(Color color)
  {
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = color;
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(2f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(2f);
    this._styleBox = styleBoxFlat;
    this.AddChild((Control) new PanelContainer()
    {
      PanelOverride = (StyleBox) this._styleBox
    });
  }

  public HSeparator()
    : this(HSeparator.SeparatorColor)
  {
  }
}
