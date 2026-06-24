// Decompiled with JetBrains decompiler
// Type: Content.Client.Rendering.Profiled08b.RMCWeaponBurstMap0629b2
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCWeaponBurstMap0629b2 : Overlay
{
  private readonly RMCProfileCacheNodea4fdbc _f25087a37569a;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public RMCWeaponBurstMap0629b2(RMCProfileCacheNodea4fdbc weapon) => this._f25087a37569a = weapon;

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return this._f25087a37569a._m49a1ad791f32(out RMCDrawSkewSlice91dac4 _);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    RMCDrawSkewSlice91dac4 challenge;
    if (!this._f25087a37569a._m49a1ad791f32(out challenge))
      return;
    int x = Math.Max(1, args.ViewportBounds.Right - args.ViewportBounds.Left);
    int y = Math.Max(1, args.ViewportBounds.Bottom - args.ViewportBounds.Top);
    int num1 = Math.Max(1, (int) challenge.Grid);
    float num2 = MathF.Max(10f, (float) ((double) MathF.Min((float) x, (float) y) * (double) challenge.SizePercent / 100.0));
    float num3 = (float) x / (float) num1;
    float num4 = (float) y / (float) num1;
    float num5 = (float) args.ViewportBounds.Left + num3 * ((float) challenge.CellX + 0.5f);
    float num6 = (float) args.ViewportBounds.Top + num4 * ((float) challenge.CellY + 0.5f);
    Vector2 vector2_1 = new Vector2(num5 - num2 * 0.55f, num6 - num2 * 0.55f);
    Vector2 vector2_2 = new Vector2(num5 - num2 * 0.4f, num6 - num2 * 0.4f);
    ((OverlayDrawArgs) ref args).ScreenHandle.DrawRect(UIBox2.FromDimensions(vector2_1, new Vector2(num2 * 1.1f, num2 * 1.1f)), Color.Black, true);
    ((OverlayDrawArgs) ref args).ScreenHandle.DrawRect(UIBox2.FromDimensions(vector2_2, new Vector2(num2 * 0.8f, num2 * 0.8f)), challenge.Color, true);
  }
}
