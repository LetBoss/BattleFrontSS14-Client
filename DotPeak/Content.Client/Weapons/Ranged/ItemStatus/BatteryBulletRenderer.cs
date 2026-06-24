// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Ranged.ItemStatus.BatteryBulletRenderer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Weapons.Ranged.ItemStatus;

public sealed class BatteryBulletRenderer : BaseBulletRenderer
{
  private static readonly Color ItemColor = Color.FromHex((ReadOnlySpan<char>) "#E00000", new Color?());
  private static readonly Color ItemColorGone = Color.Black;
  private const int SizeH = 10;
  private const int SizeV = 10;
  private const int Separation = 4;

  public BatteryBulletRenderer()
  {
    this.Parameters = new BaseBulletRenderer.LayoutParameters()
    {
      ItemWidth = 10,
      ItemHeight = 10,
      ItemSeparation = 14,
      MinCountPerRow = 3,
      VerticalSeparation = 4
    };
  }

  protected override void DrawItem(
    DrawingHandleScreen handle,
    Vector2 renderPos,
    bool spent,
    bool altColor)
  {
    Color color = spent ? BatteryBulletRenderer.ItemColorGone : BatteryBulletRenderer.ItemColor;
    handle.DrawRect(UIBox2.FromDimensions(renderPos, new Vector2(10f, 10f)), color, true);
  }
}
