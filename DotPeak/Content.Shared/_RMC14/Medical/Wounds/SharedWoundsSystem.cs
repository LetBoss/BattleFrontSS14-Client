// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Wounds.SharedWoundsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.DoAfter;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Stacks;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Shared._RMC14.Medical.Wounds;

public abstract class SharedWoundsSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private RMCDoAfterSystem _rmcDoAfter;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedStackSystem _stacks;
  [Dependency]
  private IGameTiming _timing;
  private float _bloodlossMultiplier = 1f;
  private float _bleedTimeMultiplier = 1f;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MobStateComponent, CMBleedAttemptEvent>(new EntityEventRefHandler<MobStateComponent, CMBleedAttemptEvent>(this.OnMobStateBleedAttempt));
    this.SubscribeLocalEvent<WoundableComponent, DamageChangedEvent>(new EntityEventRefHandler<WoundableComponent, DamageChangedEvent>(this.OnWoundableDamaged));
    this.SubscribeLocalEvent<WoundableComponent, CMBleedEvent>(new EntityEventRefHandler<WoundableComponent, CMBleedEvent>(this.OnWoundableBleed));
    this.SubscribeLocalEvent<WoundedComponent, RejuvenateEvent>(new EntityEventRefHandler<WoundedComponent, RejuvenateEvent>(this.OnWoundedRejuvenate));
    this.SubscribeLocalEvent<WoundedComponent, EntityUnpausedEvent>(new EntityEventRefHandler<WoundedComponent, EntityUnpausedEvent>(this.OnWoundedUnpaused));
    this.SubscribeLocalEvent<WoundTreaterComponent, UseInHandEvent>(new EntityEventRefHandler<WoundTreaterComponent, UseInHandEvent>(this.OnWoundTreaterUseInHand));
    this.SubscribeLocalEvent<WoundTreaterComponent, AfterInteractEvent>(new EntityEventRefHandler<WoundTreaterComponent, AfterInteractEvent>(this.OnWoundTreaterAfterInteract));
    this.SubscribeLocalEvent<WoundTreaterComponent, TreatWoundDoAfterEvent>(new EntityEventRefHandler<WoundTreaterComponent, TreatWoundDoAfterEvent>(this.OnWoundTreaterDoAfter));
    this.Subs.CVar<float>(this._config, RMCCVars.CMBloodlossMultiplier, (Action<float>) (v => this._bloodlossMultiplier = v), true);
    this.Subs.CVar<float>(this._config, RMCCVars.CMBleedTimeMultiplier, (Action<float>) (v => this._bleedTimeMultiplier = v), true);
  }

  private void OnMobStateBleedAttempt(Entity<MobStateComponent> ent, ref CMBleedAttemptEvent args)
  {
    if (ent.Comp.CurrentState != MobState.Dead)
      return;
    args.Cancelled = true;
  }

  private void OnWoundableDamaged(Entity<WoundableComponent> ent, ref DamageChangedEvent args)
  {
    if (this._timing.ApplyingState || args.DamageDelta == null)
      return;
    FixedPoint2? limit = this.HasComp<WoundTreaterComponent>(args.Tool) ? new FixedPoint2?(FixedPoint2.New(0.5f)) : new FixedPoint2?();
    this.TryHealWounds((Entity<DamageableComponent, WoundedComponent>) (ent.Owner, args.Damageable), args.DamageDelta, limit);
    if (!args.DamageIncreased)
      return;
    this.TryAddWound(ent, ent.Comp.BruteWoundGroup, args.DamageDelta, WoundType.Brute);
    this.TryAddWound(ent, ent.Comp.BurnWoundGroup, args.DamageDelta, WoundType.Burn);
  }

  private void OnWoundableBleed(Entity<WoundableComponent> ent, ref CMBleedEvent args)
  {
    args.Handled = true;
  }

  private void OnWoundedRejuvenate(Entity<WoundedComponent> ent, ref RejuvenateEvent args)
  {
    this.RemCompDeferred<WoundedComponent>((EntityUid) ent);
  }

  private void OnWoundedUnpaused(Entity<WoundedComponent> ent, ref EntityUnpausedEvent args)
  {
    Span<Wound> span = CollectionsMarshal.AsSpan<Wound>(ent.Comp.Wounds);
    for (int index = 0; index < span.Length; ++index)
    {
      ref Wound local1 = ref span[index];
      TimeSpan? stopBleedAt = local1.StopBleedAt;
      if (stopBleedAt.HasValue)
      {
        ref Wound local2 = ref local1;
        stopBleedAt = local1.StopBleedAt;
        TimeSpan pausedTime = args.PausedTime;
        TimeSpan? nullable = stopBleedAt.HasValue ? new TimeSpan?(stopBleedAt.GetValueOrDefault() + pausedTime) : new TimeSpan?();
        local2.StopBleedAt = nullable;
      }
    }
    ent.Comp.UpdateAt += args.PausedTime;
    this.Dirty<WoundedComponent>(ent);
  }

  private void OnWoundTreaterUseInHand(Entity<WoundTreaterComponent> ent, ref UseInHandEvent args)
  {
    bool handled;
    this.StartTreatment(args.User, args.User, ent, out handled);
    args.Handled = handled;
  }

  private void OnWoundTreaterAfterInteract(
    Entity<WoundTreaterComponent> ent,
    ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target1 = args.Target;
    if (!target1.HasValue)
      return;
    EntityUid user = args.User;
    target1 = args.Target;
    EntityUid target2 = target1.Value;
    Entity<WoundTreaterComponent> treater = ent;
    bool flag;
    ref bool local = ref flag;
    this.StartTreatment(user, target2, treater, out local);
    args.Handled = flag;
  }

  private void OnWoundTreaterDoAfter(
    Entity<WoundTreaterComponent> treater,
    ref TreatWoundDoAfterEvent args)
  {
    EntityUid user = args.User;
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target1 = args.Target;
    if (!target1.HasValue)
      return;
    EntityUid target = target1.GetValueOrDefault();
    DamageableComponent comp1;
    if (!this.TryComp<DamageableComponent>(target, out comp1))
      return;
    WoundedComponent wounded;
    FixedPoint2 damage1;
    bool handle;
    if (!this.CanTreatPopup(user, target, treater, out wounded, out damage1, out handle))
    {
      args.Handled = handle;
    }
    else
    {
      args.Handled = true;
      if (damage1 != FixedPoint2.Zero)
      {
        DamageSpecifier damage2 = this._rmcDamageable.DistributeDamageCached((Entity<DamageableComponent>) (target, comp1), treater.Comp.Group, damage1);
        this._damageable.TryChangeDamage(new EntityUid?(target), damage2, true, damageable: comp1, origin: new EntityUid?(user), tool: args.Used);
      }
      bool flag = false;
      Span<Wound> span = CollectionsMarshal.AsSpan<Wound>(wounded.Wounds);
      for (int index = 0; index < span.Length; ++index)
      {
        ref Wound local = ref span[index];
        if (local.Type == treater.Comp.Wound && !local.Treated && (treater.Comp.Treats || !(FixedPoint2.Abs(local.Healed) < local.Damage / 2f)))
        {
          local.Treated = true;
          flag = true;
        }
      }
      if (flag)
        this.Dirty(target, (IComponent) wounded);
      else if (damage1 == FixedPoint2.Zero)
      {
        if (user == target)
        {
          LocId? woundsOnUserPopup = treater.Comp.NoWoundsOnUserPopup;
          if (!woundsOnUserPopup.HasValue)
            return;
          this._popup.PopupClient(this.Loc.GetString((string) woundsOnUserPopup.GetValueOrDefault()), user, new EntityUid?(user));
          return;
        }
        LocId? woundsOnTargetPopup = treater.Comp.NoWoundsOnTargetPopup;
        if (!woundsOnTargetPopup.HasValue)
          return;
        this._popup.PopupClient(this.Loc.GetString((string) woundsOnTargetPopup.GetValueOrDefault()), user, new EntityUid?(user));
        return;
      }
      this._audio.PlayPredicted(treater.Comp.TreatEndSound, user, new EntityUid?(user));
      if (treater.Comp.Consumable)
      {
        StackComponent comp2;
        if (this.TryComp<StackComponent>((EntityUid) treater, out comp2))
          this._stacks.Use((EntityUid) treater, 2, comp2);
        else if (this._net.IsServer)
          this.QueueDel(new EntityUid?((EntityUid) treater));
      }
      else if (this.CanTreatPopup(user, target, treater, out WoundedComponent _, out FixedPoint2 _, out bool _, false))
        args.Repeat = true;
      LocId? nullable1 = treater.Comp.UserPopup;
      LocId? nullable2 = treater.Comp.TargetPopup;
      LocId? nullable3 = treater.Comp.OthersPopup;
      if (!args.Repeat)
      {
        if (treater.Comp.UserFinishPopup.HasValue)
          nullable1 = treater.Comp.UserFinishPopup;
        if (treater.Comp.TargetFinishPopup.HasValue)
          nullable2 = treater.Comp.TargetFinishPopup;
        if (treater.Comp.OthersFinishPopup.HasValue)
          nullable3 = treater.Comp.OthersFinishPopup;
      }
      LocId? nullable4;
      if (nullable1.HasValue)
      {
        SharedPopupSystem popup = this._popup;
        ILocalizationManager loc = this.Loc;
        nullable4 = nullable1;
        string valueOrDefault = nullable4.HasValue ? (string) nullable4.GetValueOrDefault() : (string) null;
        (string, object) valueTuple = ("target", (object) target);
        string message = loc.GetString(valueOrDefault, valueTuple);
        EntityUid uid = target;
        EntityUid? recipient = new EntityUid?(user);
        popup.PopupClient(message, uid, recipient);
      }
      if (user != target && nullable2.HasValue)
      {
        SharedPopupSystem popup = this._popup;
        ILocalizationManager loc = this.Loc;
        nullable4 = nullable2;
        string valueOrDefault = nullable4.HasValue ? (string) nullable4.GetValueOrDefault() : (string) null;
        (string, object) valueTuple = ("user", (object) user);
        string message = loc.GetString(valueOrDefault, valueTuple);
        EntityUid uid = target;
        EntityUid recipient = target;
        popup.PopupEntity(message, uid, recipient, PopupType.Large);
      }
      if (!(user != target) || !nullable3.HasValue)
        return;
      Filter filter1 = Filter.PvsExcept(target).RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => e == user || e == target));
      SharedPopupSystem popup1 = this._popup;
      ILocalizationManager loc1 = this.Loc;
      nullable4 = nullable3;
      string valueOrDefault1 = nullable4.HasValue ? (string) nullable4.GetValueOrDefault() : (string) null;
      (string, object) valueTuple1 = ("user", (object) user);
      (string, object) valueTuple2 = ("target", (object) target);
      string message1 = loc1.GetString(valueOrDefault1, valueTuple1, valueTuple2);
      EntityUid uid1 = user;
      Filter filter2 = filter1;
      popup1.PopupEntity(message1, uid1, filter2, true);
    }
  }

  private bool CanTreatPopup(
    EntityUid user,
    EntityUid target,
    Entity<WoundTreaterComponent> treater,
    [NotNullWhen(true)] out WoundedComponent? wounded,
    out FixedPoint2 damage,
    out bool handle,
    bool doPopups = true)
  {
    handle = false;
    wounded = (WoundedComponent) null;
    damage = FixedPoint2.Zero;
    if (!this.HasComp<WoundableComponent>(target) && !this.TryComp<WoundedComponent>(target, out wounded) || this.HasComp<WoundableUntreatableComponent>(target))
      return false;
    IdentityEntity identityEntity = Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user));
    bool flag1 = this._skills.HasAllSkills((Entity<SkillsComponent>) user, treater.Comp.Skills);
    if (!treater.Comp.CanUseUnskilled && !flag1)
    {
      if (doPopups)
        this._popup.PopupClient(this.Loc.GetString("cm-wounds-failed-unskilled", (nameof (treater), (object) treater.Owner)), target, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (!this.TryComp<WoundedComponent>(target, out wounded) || wounded.Wounds.Count == 0)
    {
      if (user == target)
      {
        if (doPopups)
        {
          LocId? noneSelfPopup = treater.Comp.NoneSelfPopup;
          if (noneSelfPopup.HasValue)
            this._popup.PopupClient(this.Loc.GetString((string) noneSelfPopup.GetValueOrDefault()), target, new EntityUid?(user));
        }
        return false;
      }
      if (doPopups)
      {
        LocId? noneOtherPopup = treater.Comp.NoneOtherPopup;
        if (noneOtherPopup.HasValue)
          this._popup.PopupClient(this.Loc.GetString((string) noneOtherPopup.GetValueOrDefault(), (nameof (target), (object) target)), target, new EntityUid?(user));
      }
      return false;
    }
    FixedPoint2 fixedPoint2_1 = (flag1 ? treater.Comp.Damage : treater.Comp.UnskilledDamage) ?? FixedPoint2.Zero;
    bool flag2 = false;
    bool flag3 = false;
    bool flag4 = false;
    FixedPoint2 fixedPoint2_2 = FixedPoint2.New(2);
    Span<Wound> span = CollectionsMarshal.AsSpan<Wound>(wounded.Wounds);
    for (int index = 0; index < span.Length; ++index)
    {
      ref Wound local = ref span[index];
      if (local.Type == WoundType.Surgery && treater.Comp.Wound != WoundType.Surgery && !local.Treated)
        flag3 = true;
      if (local.Type != treater.Comp.Wound && !local.Treated)
        flag4 = true;
      if (local.Type == treater.Comp.Wound && (!treater.Comp.Treats || !local.Treated))
      {
        if (fixedPoint2_1 == FixedPoint2.Zero)
        {
          if (!local.Treated)
            flag2 = true;
        }
        else
        {
          FixedPoint2 fixedPoint2_3 = local.Damage / fixedPoint2_2;
          if (FixedPoint2.Abs(local.Healed) < fixedPoint2_3)
            damage += -FixedPoint2.Min(fixedPoint2_3 - local.Healed, FixedPoint2.Abs(fixedPoint2_1 - damage));
          if (!(damage <= fixedPoint2_1))
          {
            if (!local.Treated)
              flag2 = true;
          }
          else
            break;
        }
      }
    }
    if (flag2 || damage != FixedPoint2.Zero)
    {
      StackComponent comp;
      if (!treater.Comp.Consumable || !this.TryComp<StackComponent>((EntityUid) treater, out comp) || this._stacks.GetCount((EntityUid) treater, comp) >= 2)
        return true;
      this._popup.PopupClient(this.Loc.GetString("cm-wounds-failed-not-enough", (nameof (treater), (object) treater.Owner)), target, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (doPopups)
    {
      if (flag3)
        this._popup.PopupClient(this.Loc.GetString("cm-wounds-open-cut", (nameof (target), (object) identityEntity), (nameof (treater), (object) treater.Owner)), target, new EntityUid?(user), PopupType.SmallCaution);
      else if (flag4)
        this._popup.PopupClient(this.Loc.GetString("cm-wounds-cannot-treat", (nameof (treater), (object) treater.Owner)), target, new EntityUid?(user), PopupType.SmallCaution);
      else
        this._popup.PopupClient(this.Loc.GetString("cm-wounds-already-treated", (nameof (target), (object) target)), target, new EntityUid?(user));
    }
    wounded = (WoundedComponent) null;
    return false;
  }

  private void StartTreatment(
    EntityUid user,
    EntityUid target,
    Entity<WoundTreaterComponent> treater,
    out bool handled)
  {
    handled = false;
    FixedPoint2 damage;
    if (!this.CanTreatPopup(user, target, treater, out WoundedComponent _, out damage, out handled))
      return;
    handled = true;
    TimeSpan delay = this._skills.GetDelay(user, (EntityUid) treater);
    if (delay > TimeSpan.Zero)
      this._popup.PopupClient(this.Loc.GetString("cm-wounds-start-fumbling", ("name", (object) treater.Owner)), target, new EntityUid?(user));
    TimeSpan timeSpan1 = treater.Comp.ScalingDoAfter * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) user, treater.Comp.DoAfterSkill, treater.Comp.DoAfterSkillMultipliers);
    if (user == target)
      timeSpan1 *= (double) treater.Comp.SelfTargetDoAfterMultiplier;
    if (timeSpan1 > TimeSpan.Zero)
    {
      TimeSpan timeSpan2 = timeSpan1 * Math.Abs(damage.Double());
      if (timeSpan2 > TimeSpan.Zero)
        delay += timeSpan2;
    }
    if (user != target && treater.Comp.TargetStartPopup.HasValue)
    {
      SharedPopupSystem popup = this._popup;
      ILocalizationManager loc = this.Loc;
      LocId? targetStartPopup = treater.Comp.TargetStartPopup;
      string valueOrDefault = targetStartPopup.HasValue ? (string) targetStartPopup.GetValueOrDefault() : (string) null;
      (string, object) valueTuple = (nameof (user), (object) user);
      string message = loc.GetString(valueOrDefault, valueTuple);
      EntityUid uid = target;
      EntityUid recipient = target;
      popup.PopupEntity(message, uid, recipient, PopupType.Medium);
    }
    TreatWoundDoAfterEvent @event = new TreatWoundDoAfterEvent();
    DoAfterArgs args = new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) treater), new EntityUid?(target), new EntityUid?((EntityUid) treater))
    {
      BreakOnMove = true,
      BreakOnHandChange = true,
      NeedHand = true,
      CancelDuplicate = true,
      DuplicateCondition = DuplicateConditions.SameEvent,
      TargetEffect = (EntProtoId?) "RMCEffectHealBusy",
      MovementThreshold = 0.5f
    };
    this._rmcDoAfter.TryCancel((Entity<DoAfterComponent>) user, treater.Comp.CurrentDoAfter);
    DoAfterId? id;
    if (!this._doAfter.TryStartDoAfter(args, out id))
      return;
    treater.Comp.CurrentDoAfter = new ushort?(id.Value.Index);
    this.DirtyField<WoundTreaterComponent>((EntityUid) treater, treater.Comp, "CurrentDoAfter");
    this._audio.PlayPredicted(treater.Comp.TreatBeginSound, user, new EntityUid?(user));
  }

  private void TryAddWound(
    Entity<WoundableComponent> woundable,
    ProtoId<DamageGroupPrototype> groupId,
    DamageSpecifier damage,
    WoundType type)
  {
    DamageGroupPrototype prototype;
    FixedPoint2 total;
    if (!this._prototypes.TryIndex<DamageGroupPrototype>(groupId, out prototype) || !damage.TryGetDamageInGroup(prototype, out total) || total <= FixedPoint2.Zero)
      return;
    this.AddWound((Entity<WoundableComponent>) ((EntityUid) woundable, (WoundableComponent) woundable), total, type);
  }

  public void TryHealWounds(
    Entity<DamageableComponent, WoundedComponent?> wounded,
    DamageSpecifier damage,
    FixedPoint2? limit = null)
  {
    (EntityUid owner, DamageableComponent comp1, WoundedComponent component) = wounded;
    if (!this.Resolve<WoundedComponent>((EntityUid) wounded, ref component, false) || component.Wounds.Count == 0)
      return;
    this.HealOrRemove((Entity<DamageableComponent, WoundedComponent>) (owner, comp1, component), component.BruteWoundGroup, WoundType.Brute, damage, limit);
    this.HealOrRemove((Entity<DamageableComponent, WoundedComponent>) (owner, comp1, component), component.BurnWoundGroup, WoundType.Burn, damage, limit);
  }

  private void HealOrRemove(
    Entity<DamageableComponent, WoundedComponent> wounded,
    ProtoId<DamageGroupPrototype> group,
    WoundType type,
    DamageSpecifier damage,
    FixedPoint2? limit = null)
  {
    if (wounded.Comp1.DamagePerGroup.GetValueOrDefault<string, FixedPoint2>((string) group) <= FixedPoint2.Zero)
    {
      this.RemoveWounds((Entity<WoundedComponent>) ((EntityUid) wounded, (WoundedComponent) wounded), type);
    }
    else
    {
      DamageGroupPrototype prototype;
      FixedPoint2 total;
      if (!this._prototypes.TryIndex<DamageGroupPrototype>(group, out prototype) || !damage.TryGetDamageInGroup(prototype, out total))
        return;
      this.TryHealWounds((Entity<WoundedComponent>) ((EntityUid) wounded, (WoundedComponent) wounded), type, total, limit);
    }
  }

  public void TryHealWounds(
    Entity<WoundedComponent?> wounded,
    WoundType type,
    FixedPoint2 amount,
    FixedPoint2? limit = null)
  {
    if (amount >= FixedPoint2.Zero || !this.Resolve<WoundedComponent>((EntityUid) wounded, ref wounded.Comp, false) || wounded.Comp.Wounds.Count == 0)
      return;
    limit.GetValueOrDefault();
    if (!limit.HasValue)
      limit = new FixedPoint2?(FixedPoint2.New(1f));
    Span<Wound> span = CollectionsMarshal.AsSpan<Wound>(wounded.Comp.Wounds);
    for (int index = 0; index < span.Length; ++index)
    {
      ref Wound local = ref span[index];
      if (local.Type == type)
      {
        FixedPoint2 fixedPoint2 = -FixedPoint2.Max(-(local.Damage * limit.Value - local.Healed), amount);
        if (!(fixedPoint2 == FixedPoint2.Zero))
        {
          local.Healed += fixedPoint2;
          amount += fixedPoint2;
          if (amount == FixedPoint2.Zero)
            break;
        }
      }
    }
  }

  public void AddWound(
    Entity<WoundableComponent?> woundable,
    FixedPoint2 total,
    WoundType type,
    TimeSpan? fixedDuration = null)
  {
    if (!this.Resolve<WoundableComponent>((EntityUid) woundable, ref woundable.Comp, false))
      return;
    float num = 0.0f;
    if (type != WoundType.Burn && total >= woundable.Comp.BleedMinDamage)
      num = total.Float() * woundable.Comp.BloodLossMultiplier;
    float Bloodloss = num * this._bloodlossMultiplier;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan? nullable1 = fixedDuration;
    TimeSpan timeSpan = nullable1 ?? (double) total.Float() * woundable.Comp.DurationMultiplier * (double) this._bleedTimeMultiplier;
    WoundedComponent comp;
    if (this.EnsureComp<WoundedComponent>((EntityUid) woundable, out comp))
    {
      Span<Wound> span = CollectionsMarshal.AsSpan<Wound>(comp.Wounds);
      for (int index = span.Length - 1; index >= 0; --index)
      {
        ref Wound local = ref span[index];
        if (local.Type == type)
        {
          nullable1 = local.StopBleedAt;
          if (nullable1.HasValue)
          {
            TimeSpan valueOrDefault = nullable1.GetValueOrDefault();
            if (!(curTime >= valueOrDefault) && (double) local.Bloodloss > 0.0)
            {
              local.Bloodloss += Bloodloss / 1.5f;
              local.StopBleedAt = new TimeSpan?(valueOrDefault + timeSpan / 1.5);
              Bloodloss = 0.0f;
              timeSpan = TimeSpan.Zero;
              break;
            }
          }
        }
      }
    }
    comp.BruteWoundGroup = woundable.Comp.BruteWoundGroup;
    comp.BurnWoundGroup = woundable.Comp.BurnWoundGroup;
    TimeSpan? nullable2;
    if (!(timeSpan == TimeSpan.MaxValue))
    {
      nullable2 = new TimeSpan?(curTime + timeSpan);
    }
    else
    {
      nullable1 = new TimeSpan?();
      nullable2 = nullable1;
    }
    TimeSpan? StopBleedAt = nullable2;
    comp.Wounds.Add(new Wound(total, FixedPoint2.Zero, Bloodloss, StopBleedAt, type, false));
    this.Dirty((EntityUid) woundable, (IComponent) comp);
  }

  public void RemoveWounds(Entity<WoundedComponent?> wounded, WoundType type)
  {
    if (!this.Resolve<WoundedComponent>((EntityUid) wounded, ref wounded.Comp, false))
      return;
    List<Wound> wounds = wounded.Comp.Wounds;
    for (int index = wounds.Count - 1; index >= 0; --index)
    {
      if (wounds[index].Type == type)
        wounds.RemoveSwap<Wound>(index);
    }
  }

  public bool HasUntreated(Entity<WoundedComponent?> wounded, ProtoId<DamageGroupPrototype> group)
  {
    if (!this.Resolve<WoundedComponent>((EntityUid) wounded, ref wounded.Comp, false) || wounded.Comp.Wounds.Count == 0)
      return false;
    WoundType woundType;
    if (group == wounded.Comp.BruteWoundGroup)
    {
      woundType = WoundType.Brute;
    }
    else
    {
      if (!(group == wounded.Comp.BurnWoundGroup))
        return false;
      woundType = WoundType.Burn;
    }
    Span<Wound> span = CollectionsMarshal.AsSpan<Wound>(wounded.Comp.Wounds);
    for (int index = 0; index < span.Length; ++index)
    {
      ref Wound local = ref span[index];
      if (local.Type == woundType && !local.Treated)
        return true;
    }
    return false;
  }
}
