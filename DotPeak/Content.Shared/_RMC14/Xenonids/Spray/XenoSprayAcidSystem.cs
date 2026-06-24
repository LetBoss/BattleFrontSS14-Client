// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Spray.XenoSprayAcidSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.OnCollide;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Spray;

public sealed class XenoSprayAcidSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private LineSystem _line;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedOnCollideSystem _onCollide;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  private static readonly ProtoId<ReagentPrototype> AcidRemovedBy = (ProtoId<ReagentPrototype>) "Water";
  private Robust.Shared.GameObjects.EntityQuery<BarricadeComponent> _barricadeQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoSprayAcidComponent> _xenoSprayAcidQuery;

  public override void Initialize()
  {
    this._barricadeQuery = this.GetEntityQuery<BarricadeComponent>();
    this._xenoSprayAcidQuery = this.GetEntityQuery<XenoSprayAcidComponent>();
    this.SubscribeLocalEvent<XenoSprayAcidComponent, XenoSprayAcidActionEvent>(new EntityEventRefHandler<XenoSprayAcidComponent, XenoSprayAcidActionEvent>(this.OnSprayAcidAction));
    this.SubscribeLocalEvent<XenoSprayAcidComponent, XenoSprayAcidDoAfter>(new EntityEventRefHandler<XenoSprayAcidComponent, XenoSprayAcidDoAfter>(this.OnSprayAcidDoAfter));
    this.SubscribeLocalEvent<SprayAcidedComponent, MapInitEvent>(new EntityEventRefHandler<SprayAcidedComponent, MapInitEvent>(this.OnSprayAcidedMapInit));
    this.SubscribeLocalEvent<SprayAcidedComponent, ComponentRemove>(new EntityEventRefHandler<SprayAcidedComponent, ComponentRemove>(this.OnSprayAcidedRemove));
    this.SubscribeLocalEvent<SprayAcidedComponent, VaporHitEvent>(new EntityEventRefHandler<SprayAcidedComponent, VaporHitEvent>(this.OnSprayAcidedVaporHit));
    this.SubscribeLocalEvent<XenoAcidSplatterComponent, ExtinguishFireAttemptEvent>(new EntityEventRefHandler<XenoAcidSplatterComponent, ExtinguishFireAttemptEvent>(this.OnAcidSplatterExtinguishFireAttempt));
  }

  private void OnSprayAcidAction(
    Entity<XenoSprayAcidComponent> xeno,
    ref XenoSprayAcidActionEvent args)
  {
    if (!this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    NetCoordinates coordinates = this.GetNetCoordinates(args.Target);
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) xeno);
    if ((double) (coordinates.Position - moverCoordinates.Position).Length() > (double) xeno.Comp.Range)
    {
      Vector2 vector2 = Vector2Helpers.Normalized(coordinates.Position - moverCoordinates.Position) * xeno.Comp.Range;
      coordinates = new NetCoordinates(this.GetNetEntity(args.Target.EntityId), moverCoordinates.Position + vector2);
    }
    XenoSprayAcidDoAfter @event = new XenoSprayAcidDoAfter(coordinates);
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.DoAfter, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
    {
      BreakOnMove = true
    });
  }

  private void OnSprayAcidDoAfter(
    Entity<XenoSprayAcidComponent> xeno,
    ref XenoSprayAcidDoAfter args)
  {
    if (args.Handled || args.Cancelled)
      return;
    args.Handled = true;
    if (!this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    if (this._net.IsClient)
      return;
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoSprayAcidActionEvent>((EntityUid) xeno))
      this._actions.StartUseDelay(new Entity<ActionComponent>?(entity.AsNullable()));
    EntityUid? blocker;
    List<LineTile> lineTileList = this._line.DrawLine(xeno.Owner.ToCoordinates(), this.GetCoordinates(args.Coordinates), xeno.Comp.Delay, new float?(xeno.Comp.Range), out blocker);
    ActiveAcidSprayingComponent sprayingComponent = this.EnsureComp<ActiveAcidSprayingComponent>((EntityUid) xeno);
    sprayingComponent.Blocker = blocker;
    sprayingComponent.Acid = xeno.Comp.Acid;
    sprayingComponent.Spawn = lineTileList;
    this.Dirty((EntityUid) xeno, (IComponent) sprayingComponent);
  }

  private void OnSprayAcidedMapInit(Entity<SprayAcidedComponent> ent, ref MapInitEvent args)
  {
    this._appearance.SetData((EntityUid) ent, (Enum) SprayAcidedVisuals.Acided, (object) true);
  }

  private void OnSprayAcidedRemove(Entity<SprayAcidedComponent> ent, ref ComponentRemove args)
  {
    if (this.TerminatingOrDeleted((EntityUid) ent))
      return;
    this._appearance.SetData((EntityUid) ent, (Enum) SprayAcidedVisuals.Acided, (object) false);
  }

  private void OnSprayAcidedVaporHit(Entity<SprayAcidedComponent> ent, ref VaporHitEvent args)
  {
    Entity<SolutionContainerManagerComponent> solution = args.Solution;
    foreach ((string Name, Entity<SolutionComponent> Solution) enumerateSolution in this._solutionContainer.EnumerateSolutions((Entity<SolutionContainerManagerComponent>) ((EntityUid) solution, (SolutionContainerManagerComponent) solution)))
    {
      if (enumerateSolution.Solution.Comp.Solution.ContainsReagent((string) XenoSprayAcidSystem.AcidRemovedBy, (List<ReagentData>) null))
      {
        this.RemCompDeferred<SprayAcidedComponent>((EntityUid) ent);
        break;
      }
    }
  }

  private void OnAcidSplatterExtinguishFireAttempt(
    Entity<XenoAcidSplatterComponent> ent,
    ref ExtinguishFireAttemptEvent args)
  {
    EntityUid? xeno = ent.Comp.Xeno;
    EntityUid target = args.Target;
    if ((xeno.HasValue ? (xeno.GetValueOrDefault() == target ? 1 : 0) : 0) == 0)
      return;
    args.Cancelled = true;
  }

  private void TryAcid(Entity<XenoSprayAcidComponent> acid, RMCAnchoredEntitiesEnumerator anchored)
  {
    EntityUid uid;
    while (anchored.MoveNext(out uid))
      this.TryAcid(acid, uid);
  }

  private void TryAcid(Entity<XenoSprayAcidComponent> acid, EntityUid target)
  {
    TimeSpan curTime = this._timing.CurTime;
    if (!this._barricadeQuery.HasComp(target))
      return;
    SprayAcidedComponent sprayAcidedComponent = this.EnsureComp<SprayAcidedComponent>(target);
    sprayAcidedComponent.Damage = acid.Comp.BarricadeDamage;
    sprayAcidedComponent.ExpireAt = curTime + acid.Comp.BarricadeDuration;
    this.Dirty(target, (IComponent) sprayAcidedComponent);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveAcidSprayingComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<ActiveAcidSprayingComponent>();
    EntityUid uid1;
    ActiveAcidSprayingComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      ActiveAcidSprayingComponent sprayingComponent = comp1_1;
      sprayingComponent.Chain.GetValueOrDefault();
      if (!sprayingComponent.Chain.HasValue)
      {
        EntityUid entityUid = (EntityUid) this._onCollide.SpawnChain();
        sprayingComponent.Chain = new EntityUid?(entityUid);
      }
      for (int index = comp1_1.Spawn.Count - 1; index >= 0; --index)
      {
        LineTile lineTile = comp1_1.Spawn[index];
        if (!(curTime < lineTile.At))
        {
          EntityUid entityUid = this.Spawn((string) comp1_1.Acid, lineTile.Coordinates, rotation: new Angle());
          XenoAcidSplatterComponent splatterComponent = this.EnsureComp<XenoAcidSplatterComponent>(entityUid);
          this._hive.SetSameHive((Entity<HiveMemberComponent>) uid1, (Entity<HiveMemberComponent>) entityUid);
          splatterComponent.Xeno = new EntityUid?(uid1);
          this.Dirty(entityUid, (IComponent) splatterComponent);
          XenoSprayAcidComponent component;
          if (this._xenoSprayAcidQuery.TryComp(uid1, out component))
          {
            Entity<XenoSprayAcidComponent> acid = new Entity<XenoSprayAcidComponent>(uid1, component);
            this.TryAcid(acid, this._rmcMap.GetAnchoredEntitiesEnumerator(entityUid, facing: (DirectionFlag) 0));
            if (comp1_1.Spawn.Count <= 1 && comp1_1.Blocker.HasValue)
            {
              this.TryAcid(acid, comp1_1.Blocker.Value);
              comp1_1.Blocker = new EntityUid?();
              this.Dirty(uid1, (IComponent) comp1_1);
            }
          }
          this._onCollide.SetChain((Entity<DamageOnCollideComponent>) entityUid, comp1_1.Chain.Value);
          comp1_1.Spawn.RemoveAt(index);
        }
      }
      if (comp1_1.Spawn.Count == 0)
        this.RemCompDeferred<ActiveAcidSprayingComponent>(uid1);
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<SprayAcidedComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<SprayAcidedComponent>();
    EntityUid uid2;
    SprayAcidedComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (curTime >= comp1_2.ExpireAt)
        this.RemCompDeferred<SprayAcidedComponent>(uid2);
      else if (!(curTime < comp1_2.NextDamageAt))
      {
        comp1_2.NextDamageAt = curTime + comp1_2.DamageEvery;
        this._damageable.TryChangeDamage(new EntityUid?(uid2), comp1_2.Damage, origin: new EntityUid?(uid2));
      }
    }
  }
}
