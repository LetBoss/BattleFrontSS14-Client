// Decompiled with JetBrains decompiler
// Type: Content.Client.Drunk.DrunkSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Drunk;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Drunk;

public sealed class DrunkSystem : SharedDrunkSystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IOverlayManager _overlayMan;
  private DrunkOverlay _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DrunkComponent, ComponentInit>(new ComponentEventHandler<DrunkComponent, ComponentInit>((object) this, __methodptr(OnDrunkInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DrunkComponent, ComponentShutdown>(new ComponentEventHandler<DrunkComponent, ComponentShutdown>((object) this, __methodptr(OnDrunkShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DrunkComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<DrunkComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DrunkComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<DrunkComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    this._overlay = new DrunkOverlay();
  }

  private void OnPlayerAttached(
    EntityUid uid,
    DrunkComponent component,
    LocalPlayerAttachedEvent args)
  {
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnPlayerDetached(
    EntityUid uid,
    DrunkComponent component,
    LocalPlayerDetachedEvent args)
  {
    this._overlay.CurrentBoozePower = 0.0f;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }

  private void OnDrunkInit(EntityUid uid, DrunkComponent component, ComponentInit args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnDrunkShutdown(EntityUid uid, DrunkComponent component, ComponentShutdown args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlay.CurrentBoozePower = 0.0f;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }
}
