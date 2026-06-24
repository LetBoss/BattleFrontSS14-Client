// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Atgm.CivAtgmWireOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._CIV14merka.Atgm;

public sealed class CivAtgmWireOverlaySystem : EntitySystem
{
  [Dependency]
  private readonly IOverlayManager _overlays;

  public virtual void Initialize()
  {
    base.Initialize();
    if (this._overlays.HasOverlay<CivAtgmWireOverlay>())
      return;
    this._overlays.AddOverlay((Overlay) new CivAtgmWireOverlay());
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlays.RemoveOverlay<CivAtgmWireOverlay>();
  }
}
