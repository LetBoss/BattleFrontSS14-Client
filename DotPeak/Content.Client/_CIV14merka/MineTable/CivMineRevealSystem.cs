// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.MineTable.CivMineRevealSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.MineTable;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._CIV14merka.MineTable;

public sealed class CivMineRevealSystem : EntitySystem
{
  [Dependency]
  private readonly IOverlayManager _overlays;
  private CivMineRevealOverlay? _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay = new CivMineRevealOverlay();
    this._overlays.AddOverlay((Overlay) this._overlay);
    this.SubscribeNetworkEvent<CivMineRevealSnapshotEvent>(new EntityEventHandler<CivMineRevealSnapshotEvent>(this.OnSnapshot), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlay != null)
      this._overlays.RemoveOverlay((Overlay) this._overlay);
    this._overlay = (CivMineRevealOverlay) null;
  }

  private void OnSnapshot(CivMineRevealSnapshotEvent ev)
  {
    if (this._overlay == null)
      return;
    this._overlay.Entries = ev.Entries;
  }
}
