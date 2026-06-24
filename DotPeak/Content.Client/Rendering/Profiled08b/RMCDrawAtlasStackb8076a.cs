// Decompiled with JetBrains decompiler
// Type: Content.Client.Rendering.Profiled08b.RMCDrawAtlasStackb8076a
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;

#nullable enable
namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCDrawAtlasStackb8076a : Overlay
{
  private readonly RMCProfileCacheNodea4fdbc _feea0b3351e4f;

  public virtual OverlaySpace Space => (OverlaySpace) 64 /*0x40*/;

  public RMCDrawAtlasStackb8076a(RMCProfileCacheNodea4fdbc weapon) => this._feea0b3351e4f = weapon;

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return this._feea0b3351e4f._mdb8dd0aca972();
  }

  protected virtual void Draw(in OverlayDrawArgs args) => this._feea0b3351e4f._m5691d410b8b1();
}
