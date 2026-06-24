// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Medicine.PubgHealthOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._PUBG.Medicine;

public sealed class PubgHealthOverlaySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlay;
  private PubgHealthOverlay? _healthOverlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this._healthOverlay = new PubgHealthOverlay();
    this._overlay.AddOverlay((Overlay) this._healthOverlay);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._healthOverlay == null)
      return;
    this._overlay.RemoveOverlay((Overlay) this._healthOverlay);
  }
}
