// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.VentCrawl.VentCrawlingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vents;
using Robust.Client.Graphics;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._RMC14.VentCrawl;

public sealed class VentCrawlingSystem : SharedVentCrawlingSystem
{
  [Dependency]
  private IOverlayManager _overlay;

  public override void Initialize()
  {
    base.Initialize();
    if (this._overlay.HasOverlay<VentCrawlIconOverlay>())
      return;
    this._overlay.AddOverlay((Overlay) new VentCrawlIconOverlay());
  }

  public virtual void Shutdown() => this._overlay.RemoveOverlay<VentCrawlIconOverlay>();
}
