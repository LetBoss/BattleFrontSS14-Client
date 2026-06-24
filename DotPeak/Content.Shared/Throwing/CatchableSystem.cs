// Decompiled with JetBrains decompiler
// Type: Content.Shared.Throwing.CatchableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CombatMode;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Throwing;

public sealed class CatchableSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private ThrownItemSystem _thrown;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  private Robust.Shared.GameObjects.EntityQuery<HandsComponent> _handsQuery;
  private Robust.Shared.GameObjects.EntityQuery<CombatModeComponent> _combatModeQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CatchableComponent, ThrowDoHitEvent>(new EntityEventRefHandler<CatchableComponent, ThrowDoHitEvent>(this.OnDoHit));
    this._handsQuery = this.GetEntityQuery<HandsComponent>();
    this._combatModeQuery = this.GetEntityQuery<CombatModeComponent>();
  }

  private void OnDoHit(Entity<CatchableComponent> ent, ref ThrowDoHitEvent args)
  {
    HandsComponent component1;
    CombatModeComponent component2;
    if (!this._handsQuery.TryGetComponent(args.Target, out component1) || ent.Comp.RequireCombatMode && (!this._combatModeQuery.TryComp(args.Target, out component2) || !component2.IsInCombatMode) || !this._whitelist.IsWhitelistPassOrNull(ent.Comp.CatcherWhitelist, args.Target))
      return;
    CatchAttemptEvent args1 = new CatchAttemptEvent(ent.Owner, ent.Comp.CatchChance);
    this.RaiseLocalEvent<CatchAttemptEvent>(args.Target, ref args1);
    if (args1.Cancelled || new Random(HashCode.Combine<int, int>((int) this._timing.CurTick.Value, this.GetNetEntity((EntityUid) ent).Id)).NextDouble() >= (double) ent.Comp.CatchChance || !this._hands.TryPickupAnyHand(args.Target, ent.Owner, animate: false, handsComp: component1))
      return;
    this._thrown.StopThrow(ent.Owner, args.Component);
    if (this._net.IsClient)
      return;
    string message1 = this.Loc.GetString("catchable-component-success-self", ("item", (object) ent.Owner), ("catcher", (object) Identity.Entity(args.Target, (IEntityManager) this.EntityManager)));
    string message2 = this.Loc.GetString("catchable-component-success-others", ("item", (object) ent.Owner), ("catcher", (object) Identity.Entity(args.Target, (IEntityManager) this.EntityManager)));
    this._popup.PopupEntity(message1, args.Target, args.Target);
    this._popup.PopupEntity(message2, args.Target, Filter.PvsExcept(args.Target), true);
    this._audio.PlayPvs(ent.Comp.CatchSuccessSound, args.Target);
  }
}
