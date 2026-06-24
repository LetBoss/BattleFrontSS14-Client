// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Blitz.XenoBlitzSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Blitz;

public sealed class XenoBlitzSystem : EntitySystem
{
  [Dependency]
  private XenoPlasmaSystem _plasma;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private VanguardShieldSystem _vanguard;
  [Dependency]
  private SharedInteractionSystem _interact;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoBlitzComponent, XenoLeapActionEvent>(new EntityEventRefHandler<XenoBlitzComponent, XenoLeapActionEvent>(this.OnLeapBlitz), new Type[1]
    {
      typeof (XenoLeapSystem)
    });
    this.SubscribeLocalEvent<XenoBlitzComponent, XenoBlitzEvent>(new EntityEventRefHandler<XenoBlitzComponent, XenoBlitzEvent>(this.OnAttackBlitz));
  }

  private void OnLeapBlitz(Entity<XenoBlitzComponent> xeno, ref XenoLeapActionEvent args)
  {
    if (args.Handled || this.HasComp<XenoLeapingComponent>((EntityUid) xeno))
      return;
    if (xeno.Comp.Dashed && xeno.Comp.SlashReady)
    {
      XenoBlitzEvent args1 = new XenoBlitzEvent();
      this.RaiseLocalEvent<XenoBlitzEvent>((EntityUid) xeno, ref args1);
      args.Handled = true;
    }
    else if (xeno.Comp.Dashed)
    {
      args.Handled = true;
    }
    else
    {
      XenoPlasmaComponent comp;
      if (!this.TryComp<XenoPlasmaComponent>((EntityUid) xeno, out comp) || !this._plasma.HasPlasma((Entity<XenoPlasmaComponent>) (xeno.Owner, comp), (FixedPoint2) xeno.Comp.PlasmaCost))
        return;
      xeno.Comp.Dashed = true;
      this._actions.SetUseDelay(new Entity<ActionComponent>?((Entity<ActionComponent>) args.Action.Owner), new TimeSpan?(xeno.Comp.BaseUseDelay));
      xeno.Comp.FirstPartActivatedAt = this._timing.CurTime;
      foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoLeapActionEvent>((EntityUid) xeno))
        this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)), true);
    }
    this.Dirty<XenoBlitzComponent>(xeno);
  }

  private void OnAttackBlitz(Entity<XenoBlitzComponent> xeno, ref XenoBlitzEvent args)
  {
    xeno.Comp.Dashed = false;
    xeno.Comp.SlashReady = false;
    this.SetBlitzDelays(xeno);
    if (!this._mob.IsAlive((EntityUid) xeno) || this.HasComp<StunnedComponent>((EntityUid) xeno))
      return;
    XenoLeapAttemptEvent args1 = new XenoLeapAttemptEvent();
    this.RaiseLocalEvent<XenoLeapAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled)
      return;
    this.EnsureComp<XenoSweepingComponent>((EntityUid) xeno);
    int num = 0;
    foreach (Entity<MobStateComponent> entity in this._lookup.GetEntitiesInRange<MobStateComponent>(this._transform.GetMapCoordinates((EntityUid) xeno), xeno.Comp.Range))
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) entity) && this._interact.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) entity.Owner, xeno.Comp.Range))
      {
        ++num;
        FixedPoint2? total = this._damage.TryChangeDamage(new EntityUid?((EntityUid) entity), this._xeno.TryApplyXenoSlashDamageMultiplier((EntityUid) entity, xeno.Comp.Damage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
        FixedPoint2 zero = FixedPoint2.Zero;
        if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
        {
          Filter filter1 = Filter.Pvs((EntityUid) entity, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
          SharedColorFlashEffectSystem colorFlash = this._colorFlash;
          Color red = Color.Red;
          List<EntityUid> entities = new List<EntityUid>();
          entities.Add((EntityUid) entity);
          Filter filter2 = filter1;
          colorFlash.RaiseEffect(red, entities, filter2);
        }
        if (this._net.IsServer)
          this.SpawnAttachedTo((string) xeno.Comp.Effect, entity.Owner.ToCoordinates(), rotation: new Angle());
      }
    }
    if (this._net.IsServer && num > 0)
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
    if (num >= xeno.Comp.HitsToRecharge)
      this._vanguard.RegenShield((EntityUid) xeno);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoLeapActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)), false);
    this.Dirty<XenoBlitzComponent>(xeno);
  }

  private void SetBlitzDelays(Entity<XenoBlitzComponent> xeno)
  {
    EntityUid? nullable1 = new EntityUid?();
    using (IEnumerator<Entity<ActionComponent>> enumerator = this._rmcActions.GetActionsWithEvent<XenoLeapActionEvent>((EntityUid) xeno).GetEnumerator())
    {
      if (enumerator.MoveNext())
        nullable1 = new EntityUid?((EntityUid) enumerator.Current);
    }
    if (!nullable1.HasValue)
      return;
    TimeSpan timeSpan = xeno.Comp.FinishedUseDelay - (this._timing.CurTime - xeno.Comp.FirstPartActivatedAt);
    if (timeSpan < TimeSpan.Zero)
      timeSpan = TimeSpan.Zero;
    SharedActionsSystem actions1 = this._actions;
    EntityUid? nullable2 = nullable1;
    Entity<ActionComponent>? action1 = nullable2.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable2.GetValueOrDefault()) : new Entity<ActionComponent>?();
    TimeSpan? delay = new TimeSpan?(timeSpan);
    actions1.SetUseDelay(action1, delay);
    SharedActionsSystem actions2 = this._actions;
    EntityUid? nullable3 = nullable1;
    Entity<ActionComponent>? action2 = nullable3.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable3.GetValueOrDefault()) : new Entity<ActionComponent>?();
    TimeSpan cooldown = timeSpan;
    actions2.SetCooldown(action2, cooldown);
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoBlitzComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoBlitzComponent>();
    EntityUid uid;
    XenoBlitzComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Dashed)
      {
        if (!this.HasComp<XenoLeapingComponent>(uid) && !comp1.SlashReady)
        {
          comp1.SlashAroundAt = curTime + comp1.SlashDashTime;
          comp1.SlashReady = true;
          this.Dirty(uid, (IComponent) comp1);
        }
        else if (comp1.SlashReady && !(curTime < comp1.SlashAroundAt))
        {
          XenoBlitzEvent args = new XenoBlitzEvent();
          this.RaiseLocalEvent<XenoBlitzEvent>(uid, ref args);
        }
      }
    }
  }
}
