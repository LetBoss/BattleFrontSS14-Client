// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Heal.SharedXenoHealSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.Actions;
using Content.Shared.Atmos.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Jittering;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Heal;

public abstract class SharedXenoHealSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedRMCFlammableSystem _flammable;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private SharedInteractionSystem _interact;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MobThresholdSystem _mobThreshold;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private QueenEyeSystem _queenEye;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedBodySystem _body;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private XenoEnergySystem _xenoEnergy;
  [Dependency]
  private SharedXenoAnnounceSystem _xenoAnnounce;
  [Dependency]
  private XenoStrainSystem _xenoStrain;
  [Dependency]
  private StatusEffectsSystem _status;
  private static readonly ProtoId<DamageGroupPrototype> BruteGroup = (ProtoId<DamageGroupPrototype>) "Brute";
  private static readonly ProtoId<DamageGroupPrototype> BurnGroup = (ProtoId<DamageGroupPrototype>) "Burn";
  private static readonly ProtoId<DamageTypePrototype> BluntGroup = (ProtoId<DamageTypePrototype>) "Blunt";
  private readonly HashSet<Entity<XenoComponent>> _xenos = new HashSet<Entity<XenoComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoHealComponent, XenoHealActionEvent>(new EntityEventRefHandler<XenoHealComponent, XenoHealActionEvent>(this.OnXenoHealAction));
    this.SubscribeLocalEvent<XenoComponent, XenoApplySalveActionEvent>(new EntityEventRefHandler<XenoComponent, XenoApplySalveActionEvent>(this.OnXenoApplySalveAction));
    this.SubscribeLocalEvent<XenoComponent, XenoSacrificeHealActionEvent>(new EntityEventRefHandler<XenoComponent, XenoSacrificeHealActionEvent>(this.OnXenoSacrificeHealAction));
  }

  private void OnXenoHealAction(Entity<XenoHealComponent> ent, ref XenoHealActionEvent args)
  {
    if (args.Handled || this._queenEye.IsInQueenEye((Entity<QueenEyeActionComponent>) ent.Owner) && !this._queenEye.CanSeeTarget((Entity<QueenEyeActionComponent>) ent.Owner, args.Target) || !this._rmcActions.TryUseAction((WorldTargetActionEvent) args))
      return;
    args.Handled = true;
    this._xenos.Clear();
    this._entityLookup.GetEntitiesInRange<XenoComponent>(args.Target, (float) ent.Comp.Radius, this._xenos);
    if (this._xenos.Count == 0)
      return;
    this._popup.PopupClient("We channel our plasma to heal our sisters' wounds around this area.", args.Target, new EntityUid?((EntityUid) ent), PopupType.Large);
    foreach (Entity<XenoComponent> xeno in this._xenos)
    {
      FixedPoint2? threshold;
      if (!this._mobState.IsDead((EntityUid) xeno) && !this._flammable.IsOnFire((Entity<FlammableComponent>) xeno.Owner) && this._mobThreshold.TryGetIncapThreshold((EntityUid) xeno, out threshold))
      {
        FixedPoint2? nullable = threshold;
        FixedPoint2 zero = FixedPoint2.Zero;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() <= zero ? 1 : 0) : 0) == 0 && this._hive.FromSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) xeno.Owner))
        {
          this.EnsureComp<XenoBeingHealedComponent>((EntityUid) xeno).HealStacks.Add(new XenoHealStack()
          {
            HealAmount = threshold.Value * ent.Comp.Percentage / (FixedPoint2) (ent.Comp.Duration.TotalSeconds * 10.0) * (ent.Comp.TimeBetweenHeals.TotalSeconds * 10.0),
            Charges = (int) (ent.Comp.Duration.TotalSeconds / ent.Comp.TimeBetweenHeals.TotalSeconds),
            TimeBetweenHeals = ent.Comp.TimeBetweenHeals
          });
          if (this._net.IsServer)
          {
            EntProtoId? healEffect = ent.Comp.HealEffect;
            this.SpawnAttachedTo(healEffect.HasValue ? (string) healEffect.GetValueOrDefault() : (string) null, xeno.Owner.ToCoordinates(), rotation: new Angle());
          }
        }
      }
    }
  }

  private void OnXenoApplySalveAction(Entity<XenoComponent> ent, ref XenoApplySalveActionEvent args)
  {
    EntityUid target = args.Target;
    LocId? nullable1 = new LocId?();
    if (!this.HasComp<XenoComponent>(target))
      nullable1 = (LocId?) "rmc-xeno-apply-salve-target-not-xeno-failure";
    if (ent.Owner == target)
      nullable1 = (LocId?) "rmc-xeno-apply-salve-target-self-failure";
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) target))
      nullable1 = (LocId?) "rmc-xeno-apply-salve-target-hostile-failure";
    if (!this._interact.InRangeUnobstructed((Entity<TransformComponent>) ent.Owner, (Entity<TransformComponent>) target, args.Range))
      nullable1 = (LocId?) "rmc-xeno-apply-salve-target-too-far-away-failure";
    if (this._mobState.IsDead(target))
      nullable1 = (LocId?) "rmc-xeno-apply-salve-target-dead-failure";
    if (this._flammable.IsOnFire((Entity<FlammableComponent>) target))
      nullable1 = (LocId?) "rmc-xeno-apply-salve-target-on-fire-failure";
    DamageableComponent comp1;
    if (this.TryComp<DamageableComponent>(target, out comp1) && comp1.TotalDamage == 0)
      nullable1 = (LocId?) "rmc-xeno-apply-salve-target-full-health-failure";
    if (nullable1.HasValue)
    {
      SharedPopupSystem popup = this._popup;
      ILocalizationManager loc = this.Loc;
      LocId? nullable2 = nullable1;
      string valueOrDefault = nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null;
      (string, object) valueTuple = ("target_xeno", (object) target);
      string message = loc.GetString(valueOrDefault, valueTuple);
      EntityUid? recipient = new EntityUid?((EntityUid) ent);
      popup.PopupClient(message, recipient);
    }
    else
    {
      FixedPoint2 fixedPoint2_1 = args.StandardHealAmount;
      FixedPoint2 fixedPoint2_2 = args.DamageTakenModifier;
      bool flag = false;
      RMCSizeComponent comp2;
      if (this.TryComp<RMCSizeComponent>(target, out comp2) && (comp2.Size == RMCSizes.Small || comp2.Size == RMCSizes.VerySmallXeno))
      {
        fixedPoint2_1 = args.SmallHealAmount;
        fixedPoint2_2 = (FixedPoint2) 1;
        flag = true;
      }
      if (this._xenoStrain.AreSameStrain((Entity<XenoStrainComponent>) ent.Owner, (Entity<XenoStrainComponent>) target))
      {
        fixedPoint2_2 = (FixedPoint2) 1;
        flag = true;
      }
      if (!this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) ent.Owner, fixedPoint2_1 * args.PlasmaCostModifier))
        return;
      FixedPoint2 energy = fixedPoint2_1 * fixedPoint2_2;
      DamageSpecifier damage = new DamageSpecifier()
      {
        DamageDict = {
          [(string) SharedXenoHealSystem.BluntGroup] = energy
        }
      };
      DamageableComponent comp3;
      if (this.TryComp<DamageableComponent>((EntityUid) ent, out comp3))
        this._damageable.AddDamage(ent.Owner, comp3, damage);
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-apply-salve-self", ("target_xeno", (object) target)), new EntityUid?((EntityUid) ent), PopupType.Medium);
      args.Handled = true;
      XenoBeingHealedComponent beingHealedComponent = this.EnsureComp<XenoBeingHealedComponent>(target);
      XenoHealStack xenoHealStack = new XenoHealStack()
      {
        Charges = (int) (args.TotalHealDuration.TotalSeconds / args.TimeBetweenHeals.TotalSeconds),
        TimeBetweenHeals = args.TimeBetweenHeals
      };
      xenoHealStack.HealAmount = fixedPoint2_1 / (float) xenoHealStack.Charges;
      xenoHealStack.NextHealAt = this._timing.CurTime + xenoHealStack.TimeBetweenHeals;
      beingHealedComponent.HealStacks.Add(xenoHealStack);
      beingHealedComponent.ParallizeHealing = true;
      this.EnsureComp<RecentlySalvedComponent>((EntityUid) ent).ExpiresAt = this._timing.CurTime + args.TotalHealDuration;
      if (this._net.IsServer)
        this.SpawnAttachedTo((string) args.HealEffect, target.ToCoordinates(), rotation: new Angle());
      this._jitter.DoJitter(target, TimeSpan.FromSeconds(1L), true, 80f, 8f, true);
      this._audio.PlayPredicted(args.HealSound, target.ToCoordinates(), new EntityUid?((EntityUid) ent));
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-apply-salve-target", ("healer_xeno", (object) ent)), new EntityUid?(target), PopupType.SmallCaution);
      XenoEnergyComponent comp4;
      if (!flag && this.TryComp<XenoEnergyComponent>((EntityUid) ent, out comp4) && !this._xenoEnergy.HasEnergy((Entity<XenoEnergyComponent>) ((EntityUid) ent, comp4), comp4.Max))
      {
        this._xenoEnergy.AddEnergy((Entity<XenoEnergyComponent>) ((EntityUid) ent, comp4), (int) energy, false);
        if (this._xenoEnergy.HasEnergy((Entity<XenoEnergyComponent>) ((EntityUid) ent, comp4), comp4.Max))
          this._popup.PopupClient(this.Loc.GetString("rmc-xeno-sacrifice-heal-will-respawn"), new EntityUid?((EntityUid) ent), PopupType.Large);
      }
      this.Dirty(target, (IComponent) beingHealedComponent);
    }
  }

  private void OnXenoSacrificeHealAction(
    Entity<XenoComponent> ent,
    ref XenoSacrificeHealActionEvent args)
  {
    EntityUid target = args.Target;
    LocId? nullable1 = new LocId?();
    if (!this.HasComp<XenoComponent>(target))
      nullable1 = (LocId?) "rmc-xeno-sacrifice-heal-target-not-xeno-failure";
    if (ent.Owner == target)
      nullable1 = (LocId?) "rmc-xeno-sacrifice-heal-target-self-failure";
    RMCSizeComponent comp1;
    if (this.HasComp<XenoParasiteComponent>(target) || this.TryComp<RMCSizeComponent>(target, out comp1) && comp1.Size == RMCSizes.VerySmallXeno)
      nullable1 = (LocId?) "rmc-xeno-sacrifice-heal-target-low-level-failure";
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) target))
      nullable1 = (LocId?) "rmc-xeno-sacrifice-heal-target-hostile-failure";
    if (!this._interact.InRangeUnobstructed((Entity<TransformComponent>) ent.Owner, (Entity<TransformComponent>) target, args.Range))
      nullable1 = (LocId?) "rmc-xeno-sacrifice-heal-target-too-far-away-failure";
    if (this._mobState.IsDead(target))
      nullable1 = (LocId?) "rmc-xeno-sacrifice-heal-target-dead-failure";
    DamageableComponent comp2;
    if (this.TryComp<DamageableComponent>(target, out comp2) && comp2.TotalDamage == 0)
      nullable1 = (LocId?) "rmc-xeno-sacrifice-heal-target-full-health-failure";
    if (nullable1.HasValue)
    {
      SharedPopupSystem popup = this._popup;
      ILocalizationManager loc = this.Loc;
      LocId? nullable2 = nullable1;
      string valueOrDefault = nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null;
      (string, object) valueTuple = ("target_xeno", (object) target);
      string message = loc.GetString(valueOrDefault, valueTuple);
      EntityUid? recipient = new EntityUid?((EntityUid) ent);
      popup.PopupClient(message, recipient);
    }
    else
    {
      DamageableComponent comp3;
      MobThresholdsComponent comp4;
      MobThresholdsComponent comp5;
      if (!this.TryComp<DamageableComponent>(target, out comp2) || !this.TryComp<DamageableComponent>((EntityUid) ent, out comp3) || !this.TryComp<MobThresholdsComponent>(target, out comp4) || !this.TryComp<MobThresholdsComponent>((EntityUid) ent, out comp5))
        return;
      this._flammable.Extinguish((Entity<FlammableComponent>) target);
      FixedPoint2? nullable3 = new FixedPoint2?();
      foreach (KeyValuePair<FixedPoint2, MobState> threshold in comp4.Thresholds)
      {
        if (threshold.Value == MobState.Critical)
          nullable3 = new FixedPoint2?(threshold.Key);
      }
      FixedPoint2? nullable4 = new FixedPoint2?();
      foreach (KeyValuePair<FixedPoint2, MobState> threshold in comp5.Thresholds)
      {
        if (threshold.Value == MobState.Dead)
          nullable4 = new FixedPoint2?(threshold.Key);
      }
      if (!nullable4.HasValue || !nullable3.HasValue)
        return;
      this.SacrificialHealShout((EntityUid) ent);
      this._xenoAnnounce.AnnounceSameHive((Entity<HiveMemberComponent>) ent.Owner, this.Loc.GetString("rmc-xeno-sacrifice-heal-target-announcement", ("healer_xeno", (object) ent), ("target_xeno", (object) target)), popup: new PopupType?(PopupType.Large));
      this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-sacrifice-heal-target-enviorment", ("healer_xeno", (object) ent), ("target_xeno", (object) target)), target, new EntityUid?((EntityUid) ent), PopupType.Medium);
      FixedPoint2 amount1 = comp2.TotalDamage - nullable3.Value;
      if (amount1 > 0)
        this.Heal(target, amount1);
      FixedPoint2 totalDamage = comp3.TotalDamage;
      FixedPoint2 amount2 = (nullable4.Value - totalDamage) * args.TransferProportion;
      this.Heal(target, amount2);
      foreach (ProtoId<StatusEffectPrototype> key in args.AilmentsRemove)
        this._status.TryRemoveStatusEffect(target, (string) key);
      this.EntityManager.RemoveComponents(target, args.ComponentsRemove);
      this._jitter.DoJitter(target, TimeSpan.FromSeconds(1L), true, 80f, 8f, true);
      XenoEnergyComponent comp6;
      if (this.TryComp<XenoEnergyComponent>((EntityUid) ent, out comp6) && this._xenoEnergy.HasEnergy((Entity<XenoEnergyComponent>) ((EntityUid) ent, comp6), comp6.Max))
      {
        EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) ent);
        if (this.GetHiveCore((EntityUid) ent))
          this.SacrificialHealRespawn((EntityUid) ent, args.RespawnDelay);
        else
          this.SacrificialHealRespawn((EntityUid) ent, args.RespawnDelay, true, new EntityCoordinates?(moverCoordinates));
      }
      else
        this.SacrificeNoRespawn((EntityUid) ent);
      if (!this._net.IsServer)
        return;
      this.SpawnAttachedTo((string) args.HealEffect, target.ToCoordinates(), rotation: new Angle());
      this._body.GibBody((EntityUid) ent, splatCone: new Angle());
    }
  }

  public void Heal(EntityUid target, FixedPoint2 amount)
  {
    DamageSpecifier equal = this._rmcDamageable.DistributeDamageCached((Entity<DamageableComponent>) target, SharedXenoHealSystem.BruteGroup, amount);
    FixedPoint2 total = equal.GetTotal();
    FixedPoint2 amount1 = amount - total;
    if (amount1 > FixedPoint2.Zero)
      equal = this._rmcDamageable.DistributeDamageCached((Entity<DamageableComponent>) target, SharedXenoHealSystem.BurnGroup, amount1, equal);
    this._damageable.TryChangeDamage(new EntityUid?(target), -equal, true);
  }

  public void CreateHealStacks(
    EntityUid target,
    FixedPoint2 healAmount,
    TimeSpan timeBetweenHeals,
    int charges,
    TimeSpan nextHealAt,
    bool ignoreFire = false)
  {
    if (!ignoreFire && this._flammable.IsOnFire((Entity<FlammableComponent>) target))
      return;
    XenoBeingHealedComponent beingHealedComponent = this.EnsureComp<XenoBeingHealedComponent>(target);
    XenoHealStack xenoHealStack = new XenoHealStack()
    {
      Charges = charges,
      TimeBetweenHeals = timeBetweenHeals
    };
    xenoHealStack.HealAmount = healAmount;
    xenoHealStack.NextHealAt = this._timing.CurTime + nextHealAt;
    beingHealedComponent.HealStacks.Add(xenoHealStack);
    beingHealedComponent.ParallizeHealing = true;
  }

  private bool GetHiveCore(EntityUid xeno)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveCoreComponent, HiveMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveCoreComponent, HiveMemberComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out HiveCoreComponent _, out HiveMemberComponent _))
    {
      if (this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno, (Entity<HiveMemberComponent>) uid) && !this._mobState.IsDead(uid))
        return true;
    }
    return false;
  }

  protected virtual void SacrificialHealShout(EntityUid xeno)
  {
  }

  protected virtual void SacrificialHealRespawn(
    EntityUid xeno,
    TimeSpan time,
    bool atCorpse = false,
    EntityCoordinates? corpse = null)
  {
  }

  protected virtual void SacrificeNoRespawn(EntityUid xeno)
  {
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoBeingHealedComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoBeingHealedComponent>();
    EntityUid uid;
    XenoBeingHealedComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.HealStacks.Count == 0 || this._mobState.IsDead(uid))
      {
        this.RemCompDeferred<XenoBeingHealedComponent>(uid);
      }
      else
      {
        List<XenoHealStack> xenoHealStackList = new List<XenoHealStack>();
        foreach (XenoHealStack healStack in comp1.HealStacks)
        {
          if (healStack.Charges <= 0)
            xenoHealStackList.Add(healStack);
          else if (!(healStack.NextHealAt > curTime))
          {
            this.Dirty(uid, (IComponent) comp1);
            this.Heal(uid, healStack.HealAmount);
            healStack.NextHealAt = curTime + healStack.TimeBetweenHeals;
            --healStack.Charges;
            if (!comp1.ParallizeHealing)
              break;
          }
        }
        foreach (XenoHealStack xenoHealStack in xenoHealStackList)
          comp1.HealStacks.Remove(xenoHealStack);
      }
    }
  }
}
