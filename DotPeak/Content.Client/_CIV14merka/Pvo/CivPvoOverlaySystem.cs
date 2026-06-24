// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Pvo.CivPvoOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Client._CIV14merka.Pvo;

public sealed class CivPvoOverlaySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IPrototypeManager _prototype;
  private CivPvoOverlay? _overlay;

  public bool Enabled { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay = new CivPvoOverlay((IEntityManager) this.EntityManager, this.EntityManager.System<SharedTransformSystem>(), this._prototype);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this.SetEnabled(false);
  }

  public bool Toggle()
  {
    this.SetEnabled(!this.Enabled);
    return this.Enabled;
  }

  public void SetEnabled(bool enabled)
  {
    if (this._overlay == null || this.Enabled == enabled)
      return;
    this.Enabled = enabled;
    if (enabled)
    {
      if (this._overlayManager.HasOverlay<CivPvoOverlay>())
        return;
      this._overlayManager.AddOverlay((Overlay) this._overlay);
    }
    else
    {
      if (!this._overlayManager.HasOverlay<CivPvoOverlay>())
        return;
      this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    }
  }
}
