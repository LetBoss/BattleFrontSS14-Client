// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.CustomControls.VSeparator
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable disable
namespace Content.Client.Administration.UI.CustomControls;

public sealed class VSeparator : PanelContainer
{
  private static readonly Color SeparatorColor = Color.FromHex((ReadOnlySpan<char>) "#3D4059", new Color?());

  public VSeparator(Color color)
  {
    ((Control) this).MinSize = new Vector2(2f, 5f);
    ((Control) this).AddChild((Control) new PanelContainer()
    {
      PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = color
      }
    });
  }

  public VSeparator()
    : this(VSeparator.SeparatorColor)
  {
  }
}
