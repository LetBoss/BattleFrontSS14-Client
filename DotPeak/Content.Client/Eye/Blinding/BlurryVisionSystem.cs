// Decompiled with JetBrains decompiler
// Type: Content.Client.Eye.Blinding.BlurryVisionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Eye.Blinding.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Eye.Blinding;

public sealed class BlurryVisionSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IOverlayManager _overlayMan;
  private BlurryVisionOverlay _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlurryVisionComponent, ComponentInit>(new ComponentEventHandler<BlurryVisionComponent, ComponentInit>((object) this, __methodptr(OnBlurryInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlurryVisionComponent, ComponentShutdown>(new ComponentEventHandler<BlurryVisionComponent, ComponentShutdown>((object) this, __methodptr(OnBlurryShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlurryVisionComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<BlurryVisionComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlurryVisionComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<BlurryVisionComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    this._overlay = new BlurryVisionOverlay();
  }

  private void OnPlayerAttached(
    EntityUid uid,
    BlurryVisionComponent component,
    LocalPlayerAttachedEvent args)
  {
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnPlayerDetached(
    EntityUid uid,
    BlurryVisionComponent component,
    LocalPlayerDetachedEvent args)
  {
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }

  private void OnBlurryInit(EntityUid uid, BlurryVisionComponent component, ComponentInit args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnBlurryShutdown(
    EntityUid uid,
    BlurryVisionComponent component,
    ComponentShutdown args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }
}
