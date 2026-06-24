// Decompiled with JetBrains decompiler
// Type: Content.Client.Flash.FlashSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Flash;
using Content.Shared.Flash.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Flash;

public sealed class FlashSystem : SharedFlashSystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IOverlayManager _overlayMan;
  private FlashOverlay _overlay;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FlashedComponent, ComponentInit>(new ComponentEventHandler<FlashedComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FlashedComponent, ComponentShutdown>(new ComponentEventHandler<FlashedComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FlashedComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<FlashedComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FlashedComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<FlashedComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    this._overlay = new FlashOverlay();
  }

  private void OnPlayerAttached(
    EntityUid uid,
    FlashedComponent component,
    LocalPlayerAttachedEvent args)
  {
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnPlayerDetached(
    EntityUid uid,
    FlashedComponent component,
    LocalPlayerDetachedEvent args)
  {
    this._overlay.ScreenshotTexture = (Texture) null;
    this._overlay.RequestScreenTexture = false;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }

  private void OnInit(EntityUid uid, FlashedComponent component, ComponentInit args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlay.RequestScreenTexture = true;
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnShutdown(EntityUid uid, FlashedComponent component, ComponentShutdown args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlay.ScreenshotTexture = (Texture) null;
    this._overlay.RequestScreenTexture = false;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }
}
