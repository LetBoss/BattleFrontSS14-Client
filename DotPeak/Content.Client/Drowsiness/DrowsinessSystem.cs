// Decompiled with JetBrains decompiler
// Type: Content.Client.Drowsiness.DrowsinessSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Drowsiness;
using Content.Shared.StatusEffectNew;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Drowsiness;

public sealed class DrowsinessSystem : SharedDrowsinessSystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IOverlayManager _overlayMan;
  [Dependency]
  private SharedStatusEffectsSystem _statusEffects;
  private DrowsinessOverlay _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DrowsinessStatusEffectComponent, StatusEffectAppliedEvent>(new EntityEventRefHandler<DrowsinessStatusEffectComponent, StatusEffectAppliedEvent>((object) this, __methodptr(OnDrowsinessApply)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DrowsinessStatusEffectComponent, StatusEffectRemovedEvent>(new EntityEventRefHandler<DrowsinessStatusEffectComponent, StatusEffectRemovedEvent>((object) this, __methodptr(OnDrowsinessShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DrowsinessStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>(new EntityEventRefHandler<DrowsinessStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>((object) this, __methodptr(OnStatusEffectPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DrowsinessStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>(new EntityEventRefHandler<DrowsinessStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>((object) this, __methodptr(OnStatusEffectPlayerDetached)), (Type[]) null, (Type[]) null);
    this._overlay = new DrowsinessOverlay();
  }

  private void OnDrowsinessApply(
    Entity<DrowsinessStatusEffectComponent> ent,
    ref StatusEffectAppliedEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid target = args.Target;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), target) ? 1 : 0) : 0) == 0)
      return;
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnDrowsinessShutdown(
    Entity<DrowsinessStatusEffectComponent> ent,
    ref StatusEffectRemovedEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid target = args.Target;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), target) ? 1 : 0) : 1) != 0 || this._statusEffects.HasEffectComp<DrowsinessStatusEffectComponent>(new EntityUid?(((ISharedPlayerManager) this._player).LocalEntity.Value)))
      return;
    this._overlay.CurrentPower = 0.0f;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }

  private void OnStatusEffectPlayerAttached(
    Entity<DrowsinessStatusEffectComponent> ent,
    ref StatusEffectRelayedEvent<LocalPlayerAttachedEvent> args)
  {
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnStatusEffectPlayerDetached(
    Entity<DrowsinessStatusEffectComponent> ent,
    ref StatusEffectRelayedEvent<LocalPlayerDetachedEvent> args)
  {
    if (!((ISharedPlayerManager) this._player).LocalEntity.HasValue || this._statusEffects.HasEffectComp<DrowsinessStatusEffectComponent>(new EntityUid?(((ISharedPlayerManager) this._player).LocalEntity.Value)))
      return;
    this._overlay.CurrentPower = 0.0f;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }
}
