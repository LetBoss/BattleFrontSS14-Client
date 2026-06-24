// Decompiled with JetBrains decompiler
// Type: Content.Client.Eye.Blinding.BlindingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Eye.Blinding.Components;
using Content.Shared.GameTicking;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Eye.Blinding;

public sealed class BlindingSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IOverlayManager _overlayMan;
  [Dependency]
  private ILightManager _lightManager;
  private BlindOverlay _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlindableComponent, ComponentInit>(new ComponentEventHandler<BlindableComponent, ComponentInit>((object) this, __methodptr(OnBlindInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlindableComponent, ComponentShutdown>(new ComponentEventHandler<BlindableComponent, ComponentShutdown>((object) this, __methodptr(OnBlindShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlindableComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<BlindableComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlindableComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<BlindableComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.RoundRestartCleanup), (Type[]) null, (Type[]) null);
    this._overlay = new BlindOverlay();
  }

  private void OnPlayerAttached(
    EntityUid uid,
    BlindableComponent component,
    LocalPlayerAttachedEvent args)
  {
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnPlayerDetached(
    EntityUid uid,
    BlindableComponent component,
    LocalPlayerDetachedEvent args)
  {
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
    this._lightManager.Enabled = true;
  }

  private void OnBlindInit(EntityUid uid, BlindableComponent component, ComponentInit args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnBlindShutdown(EntityUid uid, BlindableComponent component, ComponentShutdown args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }

  private void RoundRestartCleanup(RoundRestartCleanupEvent ev)
  {
    this._lightManager.Enabled = true;
  }
}
