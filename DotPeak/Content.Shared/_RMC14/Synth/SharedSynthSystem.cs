// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Synth.SharedSynthSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Repairable;
using Content.Shared._RMC14.StatusEffect;
using Content.Shared.Bed.Sleep;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Tools.Systems;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Synth;

public abstract class SharedSynthSystem : EntitySystem
{
  [Dependency]
  private RMCRepairableSystem _repairable;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedToolSystem _tool;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedStackSystem _stack;
  [Dependency]
  private RMCStatusEffectSystem _rmcStatusEffects;
  [Dependency]
  private MobThresholdSystem _mobThreshold;
  [Dependency]
  private EntityWhitelistSystem _whitelist;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SynthComponent, MapInitEvent>(new EntityEventRefHandler<SynthComponent, MapInitEvent>(this.OnMapInit), after: new Type[1]
    {
      typeof (SharedBloodstreamSystem)
    });
    this.SubscribeLocalEvent<SynthComponent, AttackAttemptEvent>(new EntityEventRefHandler<SynthComponent, AttackAttemptEvent>(this.OnMeleeAttempted));
    this.SubscribeLocalEvent<SynthComponent, ShotAttemptedEvent>(new EntityEventRefHandler<SynthComponent, ShotAttemptedEvent>(this.OnShotAttempted));
    this.SubscribeLocalEvent<SynthComponent, TryingToSleepEvent>(new EntityEventRefHandler<SynthComponent, TryingToSleepEvent>(this.OnSleepAttempt));
    this.SubscribeLocalEvent<SynthComponent, InteractUsingEvent>(new EntityEventRefHandler<SynthComponent, InteractUsingEvent>(this.OnSynthInteractUsing));
    this.SubscribeLocalEvent<SynthComponent, RMCSynthRepairEvent>(new EntityEventRefHandler<SynthComponent, RMCSynthRepairEvent>(this.OnSynthRepairDoAfter));
    this.SubscribeLocalEvent<UseOnSynthBlockedComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<UseOnSynthBlockedComponent, BeforeRangedInteractEvent>(this.OnSynthBlockedBeforeRangedInteract));
  }

  private void OnMapInit(Entity<SynthComponent> ent, ref MapInitEvent args) => this.MakeSynth(ent);

  protected virtual void MakeSynth(Entity<SynthComponent> ent)
  {
    EntityPrototype prototype1;
    if (this._prototypes.TryIndex(ent.Comp.AddComponents, out prototype1))
      this.EntityManager.AddComponents(ent.Owner, prototype1.Components, true);
    EntityPrototype prototype2;
    if (this._prototypes.TryIndex(ent.Comp.RemoveComponents, out prototype2))
      this.EntityManager.RemoveComponents(ent.Owner, prototype2.Components);
    if (ent.Comp.StunResistance.HasValue)
      this._rmcStatusEffects.GiveStunResistance(ent.Owner, ent.Comp.StunResistance.Value);
    FixedIdentityComponent comp1;
    if (this.TryComp<FixedIdentityComponent>(ent.Owner, out comp1))
    {
      comp1.Name = new LocId?(ent.Comp.FixedIdentityReplacement);
      this.Dirty(ent.Owner, (IComponent) comp1);
    }
    MobThresholdsComponent comp2;
    if (this.TryComp<MobThresholdsComponent>(ent.Owner, out comp2))
      this._mobThreshold.SetMobStateThreshold(ent.Owner, ent.Comp.CritThreshold, MobState.Critical, comp2);
    RMCHealthIconsComponent comp3;
    if (this.TryComp<RMCHealthIconsComponent>(ent.Owner, out comp3))
    {
      comp3.Icons = ent.Comp.HealthIconOverrides;
      this.Dirty(ent.Owner, (IComponent) comp3);
    }
    this.RemCompDeferred<RMCRevivableComponent>(ent.Owner);
    this.RemCompDeferred<SlowOnDamageComponent>(ent.Owner);
  }

  private void OnMeleeAttempted(Entity<SynthComponent> ent, ref AttackAttemptEvent args)
  {
    if (ent.Owner != args.Uid || ent.Comp.CanUseMeleeWeapons || !args.Weapon.HasValue)
      return;
    args.Cancel();
    this.DoSynthUnableToUsePopup((EntityUid) ent, args.Weapon.Value.Owner);
  }

  private void OnShotAttempted(Entity<SynthComponent> ent, ref ShotAttemptedEvent args)
  {
    if (ent.Comp.CanUseGuns)
      return;
    args.Cancel();
    this.DoSynthUnableToUsePopup((EntityUid) ent, (EntityUid) args.Used);
  }

  private void OnSleepAttempt(Entity<SynthComponent> ent, ref TryingToSleepEvent args)
  {
    args.Cancelled = true;
  }

  private void OnSynthInteractUsing(Entity<SynthComponent> synth, ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    EntityUid used1 = args.Used;
    EntityUid user1 = args.User;
    bool flag = args.User == synth.Owner;
    RMCSynthRepairEvent synthRepairEvent = new RMCSynthRepairEvent();
    TimeSpan timeSpan = flag ? synth.Comp.SelfRepairTime : synth.Comp.RepairTime;
    EntityManager entityManager = this.EntityManager;
    EntityUid user2 = user1;
    TimeSpan delay = timeSpan;
    RMCSynthRepairEvent @event = synthRepairEvent;
    EntityUid? eventTarget = new EntityUid?((EntityUid) synth);
    EntityUid? nullable = new EntityUid?(args.Used);
    EntityUid? target = new EntityUid?();
    EntityUid? used2 = nullable;
    DoAfterArgs args1 = new DoAfterArgs((IEntityManager) entityManager, user2, delay, (DoAfterEvent) @event, eventTarget, target, used2)
    {
      BreakOnMove = true,
      BreakOnDropItem = true,
      BlockDuplicate = true,
      DuplicateCondition = DuplicateConditions.SameEvent
    };
    if (this.HasComp<BlowtorchComponent>(used1) && this._tool.HasQuality(used1, (string) synth.Comp.RepairQuality))
    {
      if (this.HasDamage((EntityUid) synth, synth.Comp.WelderDamageGroup) && this._repairable.UseFuel(args.Used, args.User, (FixedPoint2) 5, true))
      {
        args.Handled = true;
        if (!this._doAfter.TryStartDoAfter(args1))
          return;
        string recipientMessage = this.Loc.GetString("rmc-synth-repair-brute-start-self", ("user", (object) user1), ("target", (object) synth), ("tool", (object) used1), ("limb", (object) "chest"));
        string othersMessage = this.Loc.GetString("rmc-synth-repair-brute-start-others", ("user", (object) user1), ("target", (object) synth), ("tool", (object) used1), ("limb", (object) "chest"));
        if (!flag)
          return;
        this._popup.PopupPredicted(recipientMessage, othersMessage, user1, new EntityUid?(user1));
      }
      else
        this._popup.PopupClient(this.Loc.GetString("rmc-repairable-not-damaged", ("target", (object) synth)), user1, new EntityUid?(user1), PopupType.SmallCaution);
    }
    else
    {
      if (!this.HasComp<RMCCableCoilComponent>(used1))
        return;
      args.Handled = true;
      if (this.HasDamage((EntityUid) synth, synth.Comp.CableCoilDamageGroup))
      {
        if (!this._doAfter.TryStartDoAfter(args1))
          return;
        string recipientMessage = this.Loc.GetString("rmc-synth-repair-burn-start-self", ("user", (object) user1), ("target", (object) synth), ("tool", (object) used1), ("limb", (object) "chest"));
        string othersMessage = this.Loc.GetString("rmc-synth-repair-burn-start-others", ("user", (object) user1), ("target", (object) synth), ("tool", (object) used1), ("limb", (object) "chest"));
        if (!flag)
          return;
        this._popup.PopupPredicted(recipientMessage, othersMessage, user1, new EntityUid?(user1));
      }
      else
        this._popup.PopupClient(this.Loc.GetString("rmc-repairable-not-damaged", ("target", (object) synth)), user1, new EntityUid?(user1), PopupType.SmallCaution);
    }
  }

  private void OnSynthRepairDoAfter(Entity<SynthComponent> synth, ref RMCSynthRepairEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityUid? used = args.Used;
    EntityUid user = args.User;
    if (!used.HasValue)
      return;
    if (this.HasComp<BlowtorchComponent>(used) && this._repairable.UseFuel(used.Value, user, (FixedPoint2) 5))
    {
      if (synth.Comp.WelderDamageToRepair != null)
        this._damageable.TryChangeDamage(new EntityUid?((EntityUid) synth), synth.Comp.WelderDamageToRepair, true, false, origin: new EntityUid?(user));
      this._popup.PopupPredicted(this.Loc.GetString("rmc-synth-repair-brute-finish-self", ("user", (object) user), ("target", (object) synth), ("tool", (object) used), ("limb", (object) "chest")), this.Loc.GetString("rmc-synth-repair-brute-finish", ("user", (object) user), ("target", (object) synth), ("tool", (object) used), ("limb", (object) "chest")), user, new EntityUid?(user));
    }
    else
    {
      if (!this.HasComp<RMCCableCoilComponent>(args.Used) || !this._stack.Use(args.Used.Value, 1))
        return;
      if (synth.Comp.CableCoilDamageToRepair != null)
        this._damageable.TryChangeDamage(new EntityUid?((EntityUid) synth), synth.Comp.CableCoilDamageToRepair, true, false, origin: new EntityUid?(args.User));
      this._popup.PopupPredicted(this.Loc.GetString("rmc-synth-repair-burn-finish-self", ("user", (object) user), ("target", (object) synth), ("tool", (object) used), ("limb", (object) "chest")), this.Loc.GetString("rmc-synth-repair-burn-finish", ("user", (object) user), ("target", (object) synth), ("tool", (object) used), ("limb", (object) "chest")), user, new EntityUid?(user));
    }
  }

  private void OnSynthBlockedBeforeRangedInteract(
    Entity<UseOnSynthBlockedComponent> ent,
    ref BeforeRangedInteractEvent args)
  {
    if (args.Handled || !args.CanReach || !args.Target.HasValue || !this._whitelist.CheckBoth(args.Target, ent.Comp.Blacklist, ent.Comp.Whitelist))
      return;
    if (this.HasComp<SynthComponent>(args.Target) && !ent.Comp.Reversed)
      args.Handled = true;
    else if (!this.HasComp<SynthComponent>(args.Target) && ent.Comp.Reversed)
      args.Handled = true;
    if (!args.Handled)
      return;
    this._popup.PopupClient(this.Loc.GetString((string) ent.Comp.Popup, ("user", (object) args.User), ("used", (object) args.Used), ("target", (object) args.Target)), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  public bool HasDamage(EntityUid synth, ProtoId<DamageGroupPrototype> group)
  {
    DamageableComponent comp;
    return this.TryComp<DamageableComponent>(synth, out comp) && !comp.Damage.Empty && !(comp.Damage.GetDamagePerGroup(this._prototypes).GetValueOrDefault<string, FixedPoint2>((string) group) <= FixedPoint2.Zero);
  }

  public void DoSynthUnableToUsePopup(EntityUid synth, EntityUid tool)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-species-synth-programming-prevents-use", ("user", (object) synth), (nameof (tool), (object) tool)), synth, new EntityUid?(synth), PopupType.SmallCaution);
  }
}
