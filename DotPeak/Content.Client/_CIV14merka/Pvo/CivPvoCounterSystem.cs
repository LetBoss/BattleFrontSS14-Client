// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Pvo.CivPvoCounterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._CIV14merka.Pvo;

public sealed class CivPvoCounterSystem : EntitySystem
{
  [Dependency]
  private readonly IOverlayManager _overlays;
  private CivPvoCounterOverlay? _overlay;

  public virtual void Initialize()
  {
    this._overlay = new CivPvoCounterOverlay();
    this._overlays.AddOverlay((Overlay) this._overlay);
  }

  public virtual void Shutdown()
  {
    if (this._overlay == null)
      return;
    this._overlays.RemoveOverlay((Overlay) this._overlay);
    this._overlay = (CivPvoCounterOverlay) null;
  }
}
