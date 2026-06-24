// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderVisionResetAlphaOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderVisionResetAlphaOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entities;
  private readonly CivCommanderVisionHideSystem _hide;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public CivCommanderVisionResetAlphaOverlay()
  {
    IoCManager.InjectDependencies<CivCommanderVisionResetAlphaOverlay>(this);
    this._hide = this._entities.System<CivCommanderVisionHideSystem>();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return this._hide.CachedBaseAlphas.Count > 0;
  }

  protected virtual void Draw(in OverlayDrawArgs args) => this._hide.RestoreCachedAlphas();
}
