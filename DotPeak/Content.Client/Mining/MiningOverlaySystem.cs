// Decompiled with JetBrains decompiler
// Type: Content.Client.Mining.MiningOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mining.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Mining;

public sealed class MiningOverlaySystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IOverlayManager _overlayMan;
  private MiningOverlay _overlay;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MiningScannerViewerComponent, ComponentInit>(new EntityEventRefHandler<MiningScannerViewerComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MiningScannerViewerComponent, ComponentShutdown>(new EntityEventRefHandler<MiningScannerViewerComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MiningScannerViewerComponent, LocalPlayerAttachedEvent>(new EntityEventRefHandler<MiningScannerViewerComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MiningScannerViewerComponent, LocalPlayerDetachedEvent>(new EntityEventRefHandler<MiningScannerViewerComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    this._overlay = new MiningOverlay();
  }

  private void OnPlayerAttached(
    Entity<MiningScannerViewerComponent> ent,
    ref LocalPlayerAttachedEvent args)
  {
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnPlayerDetached(
    Entity<MiningScannerViewerComponent> ent,
    ref LocalPlayerDetachedEvent args)
  {
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }

  private void OnInit(Entity<MiningScannerViewerComponent> ent, ref ComponentInit args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = Entity<MiningScannerViewerComponent>.op_Implicit(ent);
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnShutdown(Entity<MiningScannerViewerComponent> ent, ref ComponentShutdown args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = Entity<MiningScannerViewerComponent>.op_Implicit(ent);
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }
}
