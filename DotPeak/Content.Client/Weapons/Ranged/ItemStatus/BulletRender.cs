// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Ranged.ItemStatus.BulletRender
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Weapons.Ranged.ItemStatus;

public sealed class BulletRender : BaseBulletRenderer
{
  public const int MinCountPerRow = 7;
  public const int BulletHeight = 12;
  public const int VerticalSeparation = 2;
  private static readonly BaseBulletRenderer.LayoutParameters LayoutNormal = new BaseBulletRenderer.LayoutParameters()
  {
    ItemHeight = 12,
    ItemSeparation = 3,
    ItemWidth = 5,
    VerticalSeparation = 2,
    MinCountPerRow = 7
  };
  private static readonly BaseBulletRenderer.LayoutParameters LayoutTiny = new BaseBulletRenderer.LayoutParameters()
  {
    ItemHeight = 12,
    ItemSeparation = 2,
    ItemWidth = 2,
    VerticalSeparation = 2,
    MinCountPerRow = 7
  };
  private static readonly Color ColorA = Color.FromHex((ReadOnlySpan<char>) "#b68f0e", new Color?());
  private static readonly Color ColorB = Color.FromHex((ReadOnlySpan<char>) "#d7df60", new Color?());
  private static readonly Color ColorGoneA = Color.FromHex((ReadOnlySpan<char>) "#000000", new Color?());
  private static readonly Color ColorGoneB = Color.FromHex((ReadOnlySpan<char>) "#222222", new Color?());
  private readonly Texture _bulletTiny;
  private readonly Texture _bulletNormal;
  private BulletRender.BulletType _type;

  public BulletRender.BulletType Type
  {
    get => this._type;
    set
    {
      if (this._type == value)
        return;
      BaseBulletRenderer.LayoutParameters layoutParameters;
      switch (this._type)
      {
        case BulletRender.BulletType.Normal:
          layoutParameters = BulletRender.LayoutNormal;
          break;
        case BulletRender.BulletType.Tiny:
          layoutParameters = BulletRender.LayoutTiny;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      this.Parameters = layoutParameters;
      this._type = value;
    }
  }

  public BulletRender()
  {
    IResourceCache cache = IoCManager.Resolve<IResourceCache>();
    this._bulletTiny = cache.GetTexture("/Textures/Interface/ItemStatus/Bullets/tiny.png");
    this._bulletNormal = cache.GetTexture("/Textures/Interface/ItemStatus/Bullets/normal.png");
    this.Parameters = BulletRender.LayoutNormal;
  }

  protected override void DrawItem(
    DrawingHandleScreen handle,
    Vector2 renderPos,
    bool spent,
    bool altColor)
  {
    Color color = !spent ? (altColor ? BulletRender.ColorA : BulletRender.ColorB) : (altColor ? BulletRender.ColorGoneA : BulletRender.ColorGoneB);
    Texture texture = this._type == BulletRender.BulletType.Tiny ? this._bulletTiny : this._bulletNormal;
    ((DrawingHandleBase) handle).DrawTexture(texture, renderPos, new Color?(color));
  }

  public enum BulletType
  {
    Normal,
    Tiny,
  }
}
