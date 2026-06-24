// Decompiled with JetBrains decompiler
// Type: Content.Client.Fluids.PuddleDebugOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Fluids;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Fluids;

public sealed class PuddleDebugOverlaySystem : SharedPuddleDebugOverlaySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  public readonly Dictionary<EntityUid, PuddleOverlayDebugMessage> TileData = new Dictionary<EntityUid, PuddleOverlayDebugMessage>();
  private PuddleOverlay? _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PuddleOverlayDisableMessage>(new EntityEventHandler<PuddleOverlayDisableMessage>(this.DisableOverlay), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PuddleOverlayDebugMessage>(new EntityEventHandler<PuddleOverlayDebugMessage>(this.RenderDebugData), (Type[]) null, (Type[]) null);
  }

  private void RenderDebugData(PuddleOverlayDebugMessage message)
  {
    this.TileData[this.GetEntity(message.GridUid)] = message;
    if (this._overlay != null)
      return;
    this._overlay = new PuddleOverlay();
    this._overlayManager.AddOverlay((Overlay) this._overlay);
  }

  private void DisableOverlay(PuddleOverlayDisableMessage message)
  {
    this.TileData.Clear();
    if (this._overlay == null)
      return;
    this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    this._overlay = (PuddleOverlay) null;
  }

  public PuddleDebugOverlayData[] GetData(EntityUid mapGridGridEntityId)
  {
    return this.TileData[mapGridGridEntityId].OverlayData;
  }
}
