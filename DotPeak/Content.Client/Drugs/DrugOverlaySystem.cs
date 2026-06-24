// Decompiled with JetBrains decompiler
// Type: Content.Client.Drugs.DrugOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Drugs;
using Content.Shared.StatusEffectNew;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;
using System;

#nullable enable
namespace Content.Client.Drugs;

public sealed class DrugOverlaySystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IOverlayManager _overlayMan;
  [Dependency]
  private IRobustRandom _random;
  private RainbowOverlay _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SeeingRainbowsStatusEffectComponent, StatusEffectAppliedEvent>(new EntityEventRefHandler<SeeingRainbowsStatusEffectComponent, StatusEffectAppliedEvent>((object) this, __methodptr(OnApplied)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SeeingRainbowsStatusEffectComponent, StatusEffectRemovedEvent>(new EntityEventRefHandler<SeeingRainbowsStatusEffectComponent, StatusEffectRemovedEvent>((object) this, __methodptr(OnRemoved)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SeeingRainbowsStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>(new EntityEventRefHandler<SeeingRainbowsStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SeeingRainbowsStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>(new EntityEventRefHandler<SeeingRainbowsStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    this._overlay = new RainbowOverlay();
  }

  private void OnRemoved(
    Entity<SeeingRainbowsStatusEffectComponent> ent,
    ref StatusEffectRemovedEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid target = args.Target;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), target) ? 1 : 0) : 1) != 0)
      return;
    this._overlay.Intoxication = 0.0f;
    this._overlay.TimeTicker = 0.0f;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }

  private void OnApplied(
    Entity<SeeingRainbowsStatusEffectComponent> ent,
    ref StatusEffectAppliedEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid target = args.Target;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), target) ? 1 : 0) : 1) != 0)
      return;
    this._overlay.Phase = this._random.NextFloat(6.28318548f);
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnPlayerAttached(
    Entity<SeeingRainbowsStatusEffectComponent> ent,
    ref StatusEffectRelayedEvent<LocalPlayerAttachedEvent> args)
  {
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  private void OnPlayerDetached(
    Entity<SeeingRainbowsStatusEffectComponent> ent,
    ref StatusEffectRelayedEvent<LocalPlayerDetachedEvent> args)
  {
    this._overlay.Intoxication = 0.0f;
    this._overlay.TimeTicker = 0.0f;
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }
}
