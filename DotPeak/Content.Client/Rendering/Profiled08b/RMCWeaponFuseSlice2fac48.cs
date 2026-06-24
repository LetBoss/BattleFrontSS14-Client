// Decompiled with JetBrains decompiler
// Type: Content.Client.Rendering.Profiled08b.RMCWeaponFuseSlice2fac48
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;

#nullable enable
namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCWeaponFuseSlice2fac48 : Overlay
{
  private readonly RMCProfileCacheNodea4fdbc _fba75b59e508b;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public RMCWeaponFuseSlice2fac48(RMCProfileCacheNodea4fdbc weapon) => this._fba75b59e508b = weapon;

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return this._fba75b59e508b._m9988195534c0();
  }

  protected virtual void Draw(in OverlayDrawArgs args) => this._fba75b59e508b._m7902fc32fa5f();
}
