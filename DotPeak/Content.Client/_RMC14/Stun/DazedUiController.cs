// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Stun.DazedUiController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Stun;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.Stun;

public sealed class DazedUiController : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IEntityManager _entityManager;
  private DazedOverlay _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay = new DazedOverlay();
    this.SubscribeLocalEvent<LocalPlayerAttachedEvent>(new EntityEventHandler<LocalPlayerAttachedEvent>(this.OnPlayerAttach), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<LocalPlayerDetachedEvent>(new EntityEventHandler<LocalPlayerDetachedEvent>(this.OnPlayerDetached), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCDazedComponent, ComponentStartup>(new EntityEventRefHandler<RMCDazedComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<DazedComponentShutdownEvent>(new EntityEventHandler<DazedComponentShutdownEvent>(this.OnLocalPlayerDazedShutdown), (Type[]) null, (Type[]) null);
  }

  private void OnPlayerAttach(LocalPlayerAttachedEvent args)
  {
    this._overlay.IsEnabled = this._entityManager.HasComponent<RMCDazedComponent>(args.Entity);
    if (this._overlayManager.HasOverlay<DazedOverlay>())
      return;
    this._overlayManager.AddOverlay((Overlay) this._overlay);
  }

  private void OnPlayerDetached(LocalPlayerDetachedEvent args)
  {
    if (this._overlayManager.HasOverlay<DazedOverlay>())
      this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    this._overlay.IsEnabled = false;
  }

  private void OnStartup(Entity<RMCDazedComponent> ent, ref ComponentStartup args)
  {
    EntityUid entityUid = Entity<RMCDazedComponent>.op_Implicit(ent);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    this._overlay.IsEnabled = true;
    if (this._overlayManager.HasOverlay<DazedOverlay>())
      return;
    this._overlayManager.AddOverlay((Overlay) this._overlay);
  }

  private void OnLocalPlayerDazedShutdown(DazedComponentShutdownEvent args)
  {
    this._overlay.IsEnabled = false;
  }
}
