// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.BlackAndWhiteOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Inventory.Events;
using Content.Shared.Overlays;
using Robust.Client.Graphics;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Overlays;

public sealed class BlackAndWhiteOverlaySystem : EquipmentHudSystem<BlackAndWhiteOverlayComponent>
{
  [Dependency]
  private IOverlayManager _overlayMan;
  private BlackAndWhiteOverlay _overlay;

  public override void Initialize()
  {
    base.Initialize();
    this._overlay = new BlackAndWhiteOverlay();
  }

  protected override void UpdateInternal(
    RefreshEquipmentHudEvent<BlackAndWhiteOverlayComponent> component)
  {
    base.UpdateInternal(component);
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  protected override void DeactivateInternal()
  {
    base.DeactivateInternal();
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }
}
