// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Teleporter.RMCTeleporterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Teleporter;
using Robust.Client.Graphics;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._RMC14.Teleporter;

public sealed class RMCTeleporterSystem : SharedRMCTeleporterSystem
{
  [Dependency]
  private IOverlayManager _overlay;

  public override void Initialize()
  {
    base.Initialize();
    this._overlay.AddOverlay((Overlay) new RMCTeleporterViewerOverlay());
  }

  public virtual void Shutdown() => this._overlay.RemoveOverlay<RMCTeleporterViewerOverlay>();
}
