// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderVisionSetAlphaOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderVisionSetAlphaOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entities;
  private readonly CivCommanderVisionHideSystem _hide;
  private bool _ready;

  public virtual OverlaySpace Space => (OverlaySpace) 64 /*0x40*/;

  public CivCommanderVisionSetAlphaOverlay()
  {
    IoCManager.InjectDependencies<CivCommanderVisionSetAlphaOverlay>(this);
    this._hide = this._entities.System<CivCommanderVisionHideSystem>();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    this._ready = this._hide.Prepare(args.MapId);
    return this._ready;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this._ready)
      return;
    this._hide.Apply();
  }
}
