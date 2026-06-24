// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Utility.Systems.RMCFultonSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Events;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Rules;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.GameTicking;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Utility.Systems;

public sealed class RMCFultonSystem : EntitySystem
{
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedDropshipWeaponSystem _dropshipWeapon;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRottingSystem _rotting;
  [Dependency]
  private RMCPlanetSystem _rmcPlanet;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedStackSystem _stack;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCPullingSystem _rmcpulling;
  [Dependency]
  private RMCUnrevivableSystem _unrevivable;
  private int _fultonCount;
  private MapId? _fultonMap;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<RMCCanBeFultonedComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCCanBeFultonedComponent, InteractUsingEvent>(this.OnCanBeFultonedInteractUsing));
    this.SubscribeLocalEvent<RMCCanBeFultonedComponent, RMCPrepareFultonDoAfterEvent>(new EntityEventRefHandler<RMCCanBeFultonedComponent, RMCPrepareFultonDoAfterEvent>(this.OnCanBeFultonedPrepareFulton));
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
  {
    this._fultonCount = 0;
    this._fultonMap = new MapId?();
  }

  private void OnCanBeFultonedInteractUsing(
    Entity<RMCCanBeFultonedComponent> ent,
    ref InteractUsingEvent args)
  {
    EntityUid user = args.User;
    EntityUid target = args.Target;
    EntityUid used = args.Used;
    if (!this.HasComp<RMCFultonComponent>(used))
      return;
    if (this._mobState.IsAlive(target) || this._mobState.IsCritical(target))
      this._popup.PopupClient(this.Loc.GetString("rmc-fulton-not-dead", ("fulton", (object) used), ("target", (object) target)), target, new EntityUid?(user));
    else if (this.HasComp<PerishableComponent>(target) && !this._rotting.IsRotten(target) || this.HasComp<RMCRevivableComponent>(target) && !this._unrevivable.IsUnrevivable(target))
      this._popup.PopupClient(this.Loc.GetString("rmc-fulton-not-unrevivable", ("fulton", (object) used), ("target", (object) target)), target, new EntityUid?(user));
    else if (!this._rmcPlanet.IsOnPlanet(target.ToCoordinates()))
      this._popup.PopupClient(this.Loc.GetString("rmc-fulton-not-planet", ("fulton", (object) used)), target, new EntityUid?(user));
    else if (!this._area.CanFulton(target.ToCoordinates()))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-fulton-underground", ("fulton", (object) used)), target, new EntityUid?(user));
    }
    else
    {
      TimeSpan delay = ent.Comp.Delay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) user, ent.Comp.Skill);
      RMCPrepareFultonDoAfterEvent @event = new RMCPrepareFultonDoAfterEvent();
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(used))
      {
        BreakOnMove = true
      }))
        return;
      this._popup.PopupPredicted(this.Loc.GetString("rmc-fulton-attach-start-self", ("fulton", (object) used), ("target", (object) target)), this.Loc.GetString("rmc-fulton-attach-start-others", ("user", (object) user), ("fulton", (object) used), ("target", (object) target)), user, new EntityUid?(user));
    }
  }

  private void OnCanBeFultonedPrepareFulton(
    Entity<RMCCanBeFultonedComponent> ent,
    ref RMCPrepareFultonDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    args.Handled = true;
    RMCActiveFultonComponent activeFultonComponent = this.EnsureComp<RMCActiveFultonComponent>(valueOrDefault);
    activeFultonComponent.ReturnAt = this._timing.CurTime + ent.Comp.ReturnDelay;
    activeFultonComponent.ReturnTo = this._transform.GetMoverCoordinates((EntityUid) ent);
    this.Dirty(valueOrDefault, (IComponent) activeFultonComponent);
    RMCCanBeFultonedComponent comp;
    if (this.TryComp<RMCCanBeFultonedComponent>(valueOrDefault, out comp))
      this._audio.PlayPredicted(comp.FultonSound, activeFultonComponent.ReturnTo, new EntityUid?(args.User));
    string abbreviation = this.Name(valueOrDefault);
    this._dropshipWeapon.MakeTarget(valueOrDefault, abbreviation, false);
    this._rmcpulling.TryStopAllPullsFromAndOn(valueOrDefault);
    MapId mapId = this.EnsureMap();
    this._transform.SetMapCoordinates(valueOrDefault, new MapCoordinates((float) (this._fultonCount++ * 50), 0.0f, mapId));
    nullable = args.Used;
    if (!nullable.HasValue)
      return;
    SharedStackSystem stack = this._stack;
    nullable = args.Used;
    EntityUid uid = nullable.Value;
    stack.Use(uid, 1);
  }

  private MapId EnsureMap()
  {
    if (!this._map.MapExists(this._fultonMap))
      this._fultonMap = new MapId?();
    if (!this._fultonMap.HasValue)
    {
      MapId mapId;
      this._map.CreateMap(out mapId);
      this._fultonMap = new MapId?(mapId);
    }
    return this._fultonMap.Value;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCActiveFultonComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCActiveFultonComponent>();
    EntityUid uid;
    RMCActiveFultonComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.ReturnAt))
      {
        this.RemComp<DropshipTargetComponent>(uid);
        this.RemCompDeferred<RMCActiveFultonComponent>(uid);
        this._transform.SetCoordinates(uid, comp1.ReturnTo);
        this._audio.PlayPvs(comp1.ReturnSound, comp1.ReturnTo);
      }
    }
  }
}
