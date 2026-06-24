// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Pheromones.SharedXenoPheromonesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids.CriticalGrace;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Stab;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Threading;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Pheromones;

public abstract class SharedXenoPheromonesSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MobThresholdSystem _mobThreshold;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IParallelManager _parallel;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private SharedRMCFlammableSystem _rmcFlammable;
  [Dependency]
  private SharedXenoWeedsSystem _weeds;
  [Dependency]
  private IPrototypeManager _protoManager;
  private readonly TimeSpan _pheromonePlasmaUseDelay = TimeSpan.FromSeconds(1L);
  private readonly HashSet<EntityUid>[] _oldReceivers = ((IEnumerable<XenoPheromones>) Enum.GetValues<XenoPheromones>()).Select<XenoPheromones, HashSet<EntityUid>>((Func<XenoPheromones, HashSet<EntityUid>>) (_ => new HashSet<EntityUid>())).ToArray<HashSet<EntityUid>>();
  private readonly HashSet<EntityUid> _refreshSpeeds = new HashSet<EntityUid>();
  private Robust.Shared.GameObjects.EntityQuery<DamageableComponent> _damageableQuery;
  private SharedXenoPheromonesSystem.PheromonesJob _pheromonesJob;

  public override void Initialize()
  {
    base.Initialize();
    this._pheromonesJob = new SharedXenoPheromonesSystem.PheromonesJob(this._entityLookup);
    this._damageableQuery = this.GetEntityQuery<DamageableComponent>();
    this.SubscribeLocalEvent<XenoPheromonesComponent, XenoPheromonesActionEvent>(new EntityEventRefHandler<XenoPheromonesComponent, XenoPheromonesActionEvent>(this.OnXenoPheromonesAction));
    this.SubscribeLocalEvent<XenoPheromonesComponent, PlayerDetachedEvent>(new EntityEventRefHandler<XenoPheromonesComponent, PlayerDetachedEvent>(this.OnXenoPheromonesDetached));
    this.SubscribeLocalEvent<XenoWardingPheromonesComponent, UpdateMobStateEvent>(new EntityEventRefHandler<XenoWardingPheromonesComponent, UpdateMobStateEvent>(this.OnWardingUpdateMobState), after: new Type[1]
    {
      typeof (MobThresholdSystem)
    });
    this.SubscribeLocalEvent<XenoWardingPheromonesComponent, ComponentRemove>(new EntityEventRefHandler<XenoWardingPheromonesComponent, ComponentRemove>(this.OnWardingRemove));
    this.SubscribeLocalEvent<XenoWardingPheromonesComponent, DamageStateCritBeforeDamageEvent>(new EntityEventRefHandler<XenoWardingPheromonesComponent, DamageStateCritBeforeDamageEvent>(this.OnWardingDamageCritModify));
    this.SubscribeLocalEvent<XenoWardingPheromonesComponent, GetCriticalGraceTimeEvent>(new EntityEventRefHandler<XenoWardingPheromonesComponent, GetCriticalGraceTimeEvent>(this.OnWardingGetGraceTime));
    this.SubscribeLocalEvent<XenoFrenzyPheromonesComponent, ComponentRemove>(new EntityEventRefHandler<XenoFrenzyPheromonesComponent, ComponentRemove>(this.OnFrenzyRemove));
    this.SubscribeLocalEvent<XenoFrenzyPheromonesComponent, GetMeleeDamageEvent>(new EntityEventRefHandler<XenoFrenzyPheromonesComponent, GetMeleeDamageEvent>(this.OnFrenzyGetMeleeDamage));
    this.SubscribeLocalEvent<XenoFrenzyPheromonesComponent, RMCGetTailStabBonusDamageEvent>(new EntityEventRefHandler<XenoFrenzyPheromonesComponent, RMCGetTailStabBonusDamageEvent>(this.OnFrenzyGetTailStabDamage));
    this.SubscribeLocalEvent<XenoFrenzyPheromonesComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoFrenzyPheromonesComponent, RefreshMovementSpeedModifiersEvent>(this.OnFrenzyMovementSpeedModifiers));
    this.SubscribeLocalEvent<XenoFrenzyPheromonesComponent, PullStartedMessage>(new EntityEventRefHandler<XenoFrenzyPheromonesComponent, PullStartedMessage>(this.OnFrenzyPullStarted), after: new Type[1]
    {
      typeof (RMCPullingSystem)
    });
    this.SubscribeLocalEvent<XenoFrenzyPheromonesComponent, PullStoppedMessage>(new EntityEventRefHandler<XenoFrenzyPheromonesComponent, PullStoppedMessage>(this.OnFrenzyPullStopped), after: new Type[1]
    {
      typeof (RMCPullingSystem)
    });
    this.SubscribeLocalEvent<XenoActivePheromonesComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoActivePheromonesComponent, MobStateChangedEvent>(this.OnActiveMobStateChanged));
    this.Subs.BuiEvents<XenoPheromonesComponent>((object) XenoPheromonesUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoPheromonesComponent>) (subs => subs.Event<XenoPheromonesChosenBuiMsg>(new EntityEventRefHandler<XenoPheromonesComponent, XenoPheromonesChosenBuiMsg>(this.OnXenoPheromonesChosenBui))));
  }

  private void OnActiveMobStateChanged(
    Entity<XenoActivePheromonesComponent> ent,
    ref MobStateChangedEvent args)
  {
    bool flag;
    switch (args.NewMobState)
    {
      case MobState.Critical:
      case MobState.Dead:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    if (!flag)
      return;
    this.DeactivatePheromones((Entity<XenoPheromonesComponent>) ent.Owner);
  }

  private void OnXenoPheromonesAction(
    Entity<XenoPheromonesComponent> xeno,
    ref XenoPheromonesActionEvent args)
  {
    args.Handled = true;
    this.DeactivatePheromones(xeno.AsNullable());
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoPheromonesUI.Key, (EntityUid) xeno);
  }

  private void OnXenoPheromonesDetached(
    Entity<XenoPheromonesComponent> xeno,
    ref PlayerDetachedEvent args)
  {
    this.DeactivatePheromones(xeno.AsNullable());
  }

  private void OnXenoPheromonesChosenBui(
    Entity<XenoPheromonesComponent> xeno,
    ref XenoPheromonesChosenBuiMsg args)
  {
    if (!Enum.IsDefined<XenoPheromones>(args.Pheromones) || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, (FixedPoint2) xeno.Comp.PheromonesPlasmaCost))
      return;
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoPheromonesActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), true);
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-pheromones-start", ("pheromones", (object) args.Pheromones.ToString())), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    this._ui.CloseUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoPheromonesUI.Key, new EntityUid?((EntityUid) xeno));
    if (this._net.IsClient)
      return;
    xeno.Comp.NextPheromonesPlasmaUse = this._timing.CurTime + this._pheromonePlasmaUseDelay;
    this.Dirty<XenoPheromonesComponent>(xeno);
    XenoActivePheromonesComponent pheromonesComponent = this.EnsureComp<XenoActivePheromonesComponent>((EntityUid) xeno);
    pheromonesComponent.Pheromones = args.Pheromones;
    this.Dirty((EntityUid) xeno, (IComponent) pheromonesComponent);
    XenoPheromonesActivatedEvent args1 = new XenoPheromonesActivatedEvent();
    this.RaiseLocalEvent<XenoPheromonesActivatedEvent>((EntityUid) xeno, ref args1);
    this._entityLookup.GetEntitiesInRange<XenoComponent>(xeno.Owner.ToCoordinates(), (float) xeno.Comp.PheromonesRange, pheromonesComponent.Receivers);
  }

  private void OnWardingUpdateMobState(
    Entity<XenoWardingPheromonesComponent> warding,
    ref UpdateMobStateEvent args)
  {
    DamageableComponent component;
    FixedPoint2? threshold;
    if (args.Component.CurrentState == MobState.Dead || args.State != MobState.Dead || !this._damageableQuery.TryGetComponent((EntityUid) warding, out component) || !this._mobThreshold.TryGetDeadThreshold((EntityUid) warding, out threshold) || !this._mobState.HasState((EntityUid) warding, MobState.Critical))
      return;
    FixedPoint2 fixedPoint2 = threshold.Value + ((FixedPoint2) 1 + (FixedPoint2) 20 * warding.Comp.Multiplier);
    if (component.TotalDamage >= fixedPoint2)
      return;
    args.State = MobState.Critical;
  }

  private void OnWardingGetGraceTime(
    Entity<XenoWardingPheromonesComponent> warding,
    ref GetCriticalGraceTimeEvent args)
  {
    args.Time += TimeSpan.FromSeconds(1L) * (double) Math.Max(warding.Comp.Multiplier.Int() - 1, 0);
  }

  private void OnWardingRemove(Entity<XenoWardingPheromonesComponent> ent, ref ComponentRemove args)
  {
    MobThresholdsComponent comp;
    if (!this.TryComp<MobThresholdsComponent>((EntityUid) ent, out comp))
      return;
    this._mobThreshold.VerifyThresholds((EntityUid) ent, comp);
  }

  private void OnWardingDamageCritModify(
    Entity<XenoWardingPheromonesComponent> warding,
    ref DamageStateCritBeforeDamageEvent args)
  {
    if (this._rmcFlammable.IsOnFire((Entity<FlammableComponent>) warding.Owner))
      return;
    XenoRegenComponent comp;
    if (!this.TryComp<XenoRegenComponent>((EntityUid) warding, out comp) || !comp.HealOffWeeds && !this._weeds.IsOnFriendlyWeeds((Entity<TransformComponent>) warding.Owner))
    {
      DamageSpecifier damageSpecifier = this._rmcDamageable.DistributeDamageCached((Entity<DamageableComponent>) warding.Owner, warding.Comp.CritDamageGroup, warding.Comp.Multiplier * 0.25);
      args.Damage -= damageSpecifier;
    }
    else
      args.Damage = -this._rmcDamageable.DistributeDamageCached((Entity<DamageableComponent>) warding.Owner, warding.Comp.CritDamageGroup, warding.Comp.Multiplier * 0.5f);
  }

  private void OnFrenzyRemove(Entity<XenoFrenzyPheromonesComponent> ent, ref ComponentRemove args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }

  private void OnFrenzyGetMeleeDamage(
    Entity<XenoFrenzyPheromonesComponent> frenzy,
    ref GetMeleeDamageEvent args)
  {
    args.Damage += new DamageSpecifier(this._protoManager.Index<DamageGroupPrototype>(frenzy.Comp.DamageGroup), (FixedPoint2) frenzy.Comp.AttackDamageAddPerMult * frenzy.Comp.Multiplier);
  }

  private void OnFrenzyGetTailStabDamage(
    Entity<XenoFrenzyPheromonesComponent> frenzy,
    ref RMCGetTailStabBonusDamageEvent args)
  {
    args.Damage += new DamageSpecifier(this._protoManager.Index<DamageGroupPrototype>(frenzy.Comp.DamageGroup), (FixedPoint2) frenzy.Comp.AttackDamageAddPerMult * frenzy.Comp.Multiplier * 1.2);
  }

  private void OnFrenzyMovementSpeedModifiers(
    Entity<XenoFrenzyPheromonesComponent> frenzy,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    float num = 1f + (frenzy.Comp.MovementSpeedModifier * frenzy.Comp.Multiplier).Float();
    if (this.HasComp<PullingSlowedComponent>(frenzy.Owner))
      num = 1f + (frenzy.Comp.PullMovementSpeedModifier * frenzy.Comp.Multiplier).Float();
    args.ModifySpeed(num, num);
  }

  private void OnFrenzyPullStarted(
    Entity<XenoFrenzyPheromonesComponent> frenzy,
    ref PullStartedMessage args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
  }

  private void OnFrenzyPullStopped(
    Entity<XenoFrenzyPheromonesComponent> frenzy,
    ref PullStoppedMessage args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
  }

  private void AssignMaxMultiplier(ref FixedPoint2 a, FixedPoint2 b) => a = FixedPoint2.Max(a, b);

  public void DeactivatePheromones(Entity<XenoPheromonesComponent?> xeno)
  {
    if (!this.Resolve<XenoPheromonesComponent>((EntityUid) xeno, ref xeno.Comp, false))
      return;
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoPheromonesActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), false);
    if (!this.HasComp<XenoActivePheromonesComponent>((EntityUid) xeno))
      return;
    if (this._net.IsServer)
      this.RemComp<XenoActivePheromonesComponent>((EntityUid) xeno);
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-pheromones-stop"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    XenoPheromonesDeactivatedEvent args = new XenoPheromonesDeactivatedEvent();
    this.RaiseLocalEvent<XenoPheromonesDeactivatedEvent>((EntityUid) xeno, ref args);
  }

  public void TryActivatePheromonesObject(Entity<XenoPheromonesObjectComponent?> ent)
  {
    XenoPheromonesComponent comp;
    if (!this.Resolve<XenoPheromonesObjectComponent>((EntityUid) ent, ref ent.Comp, false) || this._net.IsClient || !this.TryComp<XenoPheromonesComponent>((EntityUid) ent, out comp))
      return;
    XenoActivePheromonesComponent pheromonesComponent = this.EnsureComp<XenoActivePheromonesComponent>((EntityUid) ent);
    pheromonesComponent.Pheromones = ent.Comp.Pheromones;
    this.Dirty((EntityUid) ent, (IComponent) pheromonesComponent);
    this._entityLookup.GetEntitiesInRange<XenoComponent>(ent.Owner.ToCoordinates(), (float) comp.PheromonesRange, pheromonesComponent.Receivers);
    XenoPheromonesActivatedEvent args = new XenoPheromonesActivatedEvent();
    this.RaiseLocalEvent<XenoPheromonesActivatedEvent>((EntityUid) ent, ref args);
  }

  private bool KeepWarding(
    EntityUid ent,
    XenoWardingPheromonesComponent warding,
    FixedPoint2 newWardMult)
  {
    FixedPoint2? threshold;
    DamageableComponent component;
    if (!this._mobThreshold.TryGetIncapThreshold(ent, out threshold) || !this._damageableQuery.TryGetComponent(ent, out component))
      return false;
    FixedPoint2 totalDamage = component.TotalDamage;
    FixedPoint2? nullable = threshold;
    XenoRegenComponent comp;
    return (nullable.HasValue ? (totalDamage < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 && !(newWardMult > warding.Multiplier) && (!this.TryComp<XenoRegenComponent>(ent, out comp) || !comp.HealOffWeeds) && this._weeds.IsOnFriendlyWeeds((Entity<TransformComponent>) ent);
  }

  public string? GetPheroSuffix(Entity<XenoPheromonesComponent?> xeno)
  {
    return !this.Resolve<XenoPheromonesComponent>((EntityUid) xeno, ref xeno.Comp, false) ? (string) null : xeno.Comp.PheroSuffix;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._net.IsClient)
      return;
    HashSet<EntityUid> oldReceiver1 = this._oldReceivers[0];
    oldReceiver1.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoRecoveryPheromonesComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<XenoRecoveryPheromonesComponent>();
    EntityUid uid1;
    XenoRecoveryPheromonesComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      oldReceiver1.Add(uid1);
      comp1_1.Multiplier = (FixedPoint2) 0;
    }
    HashSet<EntityUid> oldReceiver2 = this._oldReceivers[1];
    oldReceiver2.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoWardingPheromonesComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<XenoWardingPheromonesComponent>();
    EntityUid uid2;
    XenoWardingPheromonesComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (this._mobState.IsDead(uid2) || !this.KeepWarding(uid2, comp1_2, (FixedPoint2) 0))
      {
        oldReceiver2.Add(uid2);
        comp1_2.Multiplier = (FixedPoint2) 0;
      }
    }
    HashSet<EntityUid> oldReceiver3 = this._oldReceivers[2];
    oldReceiver3.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoFrenzyPheromonesComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<XenoFrenzyPheromonesComponent>();
    EntityUid uid3;
    XenoFrenzyPheromonesComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_3))
    {
      oldReceiver3.Add(uid3);
      comp1_3.Multiplier = (FixedPoint2) 0;
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoActivePheromonesComponent, XenoPheromonesComponent, TransformComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<XenoActivePheromonesComponent, XenoPheromonesComponent, TransformComponent>();
    this._pheromonesJob.Receivers.Clear();
    this._pheromonesJob.Pheromones.Clear();
    this._refreshSpeeds.Clear();
    EntityUid uid4;
    XenoActivePheromonesComponent comp1_4;
    XenoPheromonesComponent comp2;
    TransformComponent comp3;
    while (entityQueryEnumerator4.MoveNext(out uid4, out comp1_4, out comp2, out comp3))
    {
      this._pheromonesJob.Pheromones.Add((uid4, comp1_4, comp2, comp3));
      if (this._timing.CurTime < comp2.NextPheromonesPlasmaUse)
      {
        this._pheromonesJob.Receivers.Add((false, comp1_4.Receivers));
      }
      else
      {
        comp2.NextPheromonesPlasmaUse = this._timing.CurTime + this._pheromonePlasmaUseDelay;
        this.Dirty(uid4, (IComponent) comp2);
        if (!this.HasComp<XenoPheromonesObjectComponent>(uid4) && comp2.PheromonesPlasmaUpkeep > 0 && !this._xenoPlasma.TryRemovePlasma((Entity<XenoPlasmaComponent>) uid4, comp2.PheromonesPlasmaUpkeep))
        {
          this._pheromonesJob.Pheromones.RemoveAt(this._pheromonesJob.Pheromones.Count - 1);
          this.RemCompDeferred<XenoActivePheromonesComponent>(uid4);
        }
        else
        {
          this._pheromonesJob.Receivers.Add((true, comp1_4.Receivers));
          this.Dirty(uid4, (IComponent) comp2);
        }
      }
    }
    this._parallel.ProcessNow((IParallelRobustJob) this._pheromonesJob, this._pheromonesJob.Pheromones.Count);
    for (int index = 0; index < this._pheromonesJob.Pheromones.Count; ++index)
    {
      (EntityUid _, XenoActivePheromonesComponent pheromonesComponent1, XenoPheromonesComponent pheromonesComponent2, TransformComponent _) = this._pheromonesJob.Pheromones[index];
      HashSet<Entity<XenoComponent>> entitySet = this._pheromonesJob.Receivers[index].Item2;
      switch (pheromonesComponent1.Pheromones)
      {
        case XenoPheromones.Recovery:
          using (HashSet<Entity<XenoComponent>>.Enumerator enumerator = entitySet.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Entity<XenoComponent> current = enumerator.Current;
              if (!this.Deleted((EntityUid) current) && !this._mobState.IsDead((EntityUid) current))
              {
                XenoPheromones? ignorePheromones = current.Comp.IgnorePheromones;
                XenoPheromones xenoPheromones = XenoPheromones.Recovery;
                if (!(ignorePheromones.GetValueOrDefault() == xenoPheromones & ignorePheromones.HasValue))
                {
                  oldReceiver1.Remove((EntityUid) current);
                  this.AssignMaxMultiplier(ref this.EnsureComp<XenoRecoveryPheromonesComponent>((EntityUid) current).Multiplier, pheromonesComponent2.PheromonesMultiplier);
                }
              }
            }
            break;
          }
        case XenoPheromones.Warding:
          using (HashSet<Entity<XenoComponent>>.Enumerator enumerator = pheromonesComponent1.Receivers.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Entity<XenoComponent> current = enumerator.Current;
              if (!this.Deleted((EntityUid) current) && !this._mobState.IsDead((EntityUid) current) && current.Comp.IgnorePheromones.GetValueOrDefault() != XenoPheromones.Warding)
              {
                oldReceiver2.Remove((EntityUid) current);
                this.AssignMaxMultiplier(ref this.EnsureComp<XenoWardingPheromonesComponent>((EntityUid) current).Multiplier, pheromonesComponent2.PheromonesMultiplier);
              }
            }
            break;
          }
        case XenoPheromones.Frenzy:
          using (HashSet<Entity<XenoComponent>>.Enumerator enumerator = pheromonesComponent1.Receivers.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Entity<XenoComponent> current = enumerator.Current;
              if (!this.Deleted((EntityUid) current) && !this._mobState.IsDead((EntityUid) current) && current.Comp.IgnorePheromones.GetValueOrDefault() != XenoPheromones.Frenzy)
              {
                oldReceiver3.Remove((EntityUid) current);
                XenoFrenzyPheromonesComponent pheromonesComponent3 = this.EnsureComp<XenoFrenzyPheromonesComponent>((EntityUid) current);
                FixedPoint2 multiplier = pheromonesComponent3.Multiplier;
                this.AssignMaxMultiplier(ref pheromonesComponent3.Multiplier, pheromonesComponent2.PheromonesMultiplier);
                if (pheromonesComponent3.Multiplier != multiplier)
                  this._refreshSpeeds.Add((EntityUid) current);
              }
            }
            break;
          }
      }
    }
    foreach (EntityUid refreshSpeed in this._refreshSpeeds)
      this._movementSpeed.RefreshMovementSpeedModifiers(refreshSpeed);
    foreach (EntityUid uid5 in oldReceiver1)
      this.RemComp<XenoRecoveryPheromonesComponent>(uid5);
    foreach (EntityUid uid6 in oldReceiver2)
      this.RemComp<XenoWardingPheromonesComponent>(uid6);
    foreach (EntityUid uid7 in oldReceiver3)
      this.RemComp<XenoFrenzyPheromonesComponent>(uid7);
  }

  private record struct PheromonesJob(EntityLookupSystem Lookup) : 
    IParallelRobustJob,
    IParallelRangeRobustJob
  {
    public ValueList<(EntityUid Uid, XenoActivePheromonesComponent Active, XenoPheromonesComponent Pheromones, TransformComponent Xform)> Pheromones = new ValueList<(EntityUid, XenoActivePheromonesComponent, XenoPheromonesComponent, TransformComponent)>();
    public ValueList<(bool Update, HashSet<Entity<XenoComponent>> Receivers)> Receivers = new ValueList<(bool, HashSet<Entity<XenoComponent>>)>();

    public EntityLookupSystem Lookup { get; set; } = Lookup;

    public int BatchSize => 8;

    public void Execute(int index)
    {
      ref (bool, HashSet<Entity<XenoComponent>>) local = ref this.Receivers[index];
      if (!local.Item1)
        return;
      (EntityUid _, XenoActivePheromonesComponent _, XenoPheromonesComponent pheromonesComponent, TransformComponent transformComponent) = this.Pheromones[index];
      local.Item2.Clear();
      this.Lookup.GetEntitiesInRange<XenoComponent>(transformComponent.Coordinates, (float) pheromonesComponent.PheromonesRange, local.Item2);
    }
  }
}
