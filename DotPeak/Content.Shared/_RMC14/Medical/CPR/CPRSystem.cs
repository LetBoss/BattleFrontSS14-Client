// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.CPR.CPRSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.ShakeStun;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Medical.CPR;

public sealed class CPRSystem : EntitySystem
{
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popups;
  [Dependency]
  private RMCUnrevivableSystem _unrevivable;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SkillsSystem _skills;
  public static readonly EntProtoId<SkillDefinitionComponent> SkillType = (EntProtoId<SkillDefinitionComponent>) "RMCSkillMedical";
  private static readonly ProtoId<DamageTypePrototype> HealType = (ProtoId<DamageTypePrototype>) "Asphyxiation";
  private static readonly FixedPoint2 HealAmount = FixedPoint2.New(10);

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MarineComponent, InteractHandEvent>(new EntityEventRefHandler<MarineComponent, InteractHandEvent>(this.OnMarineInteractHand), new Type[2]
    {
      typeof (InteractionPopupSystem),
      typeof (StunShakeableSystem)
    });
    this.SubscribeLocalEvent<MarineComponent, CPRDoAfterEvent>(new EntityEventRefHandler<MarineComponent, CPRDoAfterEvent>(this.OnMarineDoAfter));
    this.SubscribeLocalEvent<ReceivingCPRComponent, ReceiveCPRAttemptEvent>(new EntityEventRefHandler<ReceivingCPRComponent, ReceiveCPRAttemptEvent>(this.OnReceivingCPRAttempt));
    this.SubscribeLocalEvent<CPRReceivedComponent, ReceiveCPRAttemptEvent>(new EntityEventRefHandler<CPRReceivedComponent, ReceiveCPRAttemptEvent>(this.OnReceivedCPRAttempt));
    this.SubscribeLocalEvent<MobStateComponent, ReceiveCPRAttemptEvent>(new EntityEventRefHandler<MobStateComponent, ReceiveCPRAttemptEvent>(this.OnMobStateCPRAttempt));
  }

  private void OnMarineInteractHand(Entity<MarineComponent> ent, ref InteractHandEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.StartCPR(args.User, args.Target);
  }

  private void OnMarineDoAfter(Entity<MarineComponent> ent, ref CPRDoAfterEvent args)
  {
    EntityUid performer = args.User;
    if (args.Target.HasValue)
      this.RemComp<ReceivingCPRComponent>(args.Target.Value);
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    FixedPoint2 damage;
    if (!this.CanCPRPopup(performer, valueOrDefault, false, out damage))
      return;
    args.Handled = true;
    this._unrevivable.AddRevivableTime(valueOrDefault, TimeSpan.FromSeconds(7L));
    DamageableComponent comp;
    if (!this.TryComp<DamageableComponent>(valueOrDefault, out comp) || !comp.Damage.DamageDict.TryGetValue((string) CPRSystem.HealType, out damage))
      return;
    FixedPoint2 fixedPoint2 = -FixedPoint2.Min(damage, CPRSystem.HealAmount);
    this._damageable.TryChangeDamage(new EntityUid?(valueOrDefault), new DamageSpecifier()
    {
      DamageDict = {
        {
          (string) CPRSystem.HealType,
          fixedPoint2
        }
      }
    }, true);
    this.EnsureComp<CPRReceivedComponent>(valueOrDefault).Last = this._timing.CurTime;
    if (this._net.IsClient)
      return;
    this._popups.PopupEntity(this.Loc.GetString("cm-cpr-self-perform", ("target", (object) valueOrDefault), ("seconds", (object) 7)), valueOrDefault, performer, PopupType.Medium);
    string message = this.Loc.GetString("cm-cpr-other-perform", ("performer", (object) performer), ("target", (object) valueOrDefault));
    Filter filter = Filter.Pvs(performer).RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => e == performer));
    this._popups.PopupEntity(message, performer, filter, true, PopupType.Medium);
  }

  private void OnReceivingCPRAttempt(
    Entity<ReceivingCPRComponent> ent,
    ref ReceiveCPRAttemptEvent args)
  {
    if (this._timing.CurTime - ent.Comp.StartTime > TimeSpan.FromSeconds(7L))
    {
      this.RemCompDeferred<ReceivingCPRComponent>((EntityUid) ent);
    }
    else
    {
      args.Cancelled = true;
      if (this._net.IsClient)
        return;
      this._popups.PopupEntity(this.Loc.GetString("cm-cpr-already-being-performed", ("target", (object) ent.Owner)), (EntityUid) ent, args.Performer, PopupType.Medium);
    }
  }

  private void OnReceivedCPRAttempt(
    Entity<CPRReceivedComponent> ent,
    ref ReceiveCPRAttemptEvent args)
  {
    if (args.Start)
      return;
    EntityUid owner = ent.Owner;
    EntityUid performer = args.Performer;
    if (!this._mobState.IsDead((EntityUid) ent) || ent.Comp.Last <= this._timing.CurTime - TimeSpan.FromSeconds(7L))
      return;
    args.Cancelled = true;
    if (this._net.IsClient)
      return;
    this._popups.PopupEntity(this.Loc.GetString("cm-cpr-self-perform-fail-received-too-recently", ("target", (object) owner)), owner, performer, PopupType.MediumCaution);
    string message = this.Loc.GetString("cm-cpr-other-perform-fail", ("performer", (object) performer), ("target", (object) owner));
    Filter filter = Filter.Pvs(performer).RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => e == performer));
    this._popups.PopupEntity(message, performer, filter, true, PopupType.MediumCaution);
  }

  private void OnMobStateCPRAttempt(Entity<MobStateComponent> ent, ref ReceiveCPRAttemptEvent args)
  {
    if (args.Cancelled || !this._mobState.IsAlive((EntityUid) ent) && (!this._mobState.IsDead((EntityUid) ent) || !this._unrevivable.IsUnrevivable((EntityUid) ent)))
      return;
    args.Cancelled = true;
  }

  private bool CanCPRPopup(
    EntityUid performer,
    EntityUid target,
    bool start,
    out FixedPoint2 damage)
  {
    damage = new FixedPoint2();
    if (!this.HasComp<MarineComponent>(target) || !this.HasComp<MarineComponent>(performer))
      return false;
    PerformCPRAttemptEvent args1 = new PerformCPRAttemptEvent(target);
    this.RaiseLocalEvent<PerformCPRAttemptEvent>(performer, ref args1);
    if (args1.Cancelled)
      return false;
    ReceiveCPRAttemptEvent args2 = new ReceiveCPRAttemptEvent(performer, target, start);
    this.RaiseLocalEvent<ReceiveCPRAttemptEvent>(target, ref args2);
    return !args2.Cancelled && this._hands.TryGetEmptyHand((Entity<HandsComponent>) performer, out string _);
  }

  private bool StartCPR(EntityUid performer, EntityUid target)
  {
    if (!this.CanCPRPopup(performer, target, true, out FixedPoint2 _))
      return false;
    ReceivingCPRComponent receivingCprComponent = this.EnsureComp<ReceivingCPRComponent>(target);
    receivingCprComponent.StartTime = this._timing.CurTime;
    this.Dirty(target, (IComponent) receivingCprComponent);
    TimeSpan delay = TimeSpan.FromSeconds((double) receivingCprComponent.CPRPerformingTime * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) performer, CPRSystem.SkillType));
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, performer, delay, (DoAfterEvent) new CPRDoAfterEvent(), new EntityUid?(performer), new EntityUid?(target))
    {
      BreakOnMove = true,
      NeedHand = true,
      BlockDuplicate = true,
      DuplicateCondition = DuplicateConditions.SameEvent,
      TargetEffect = (EntProtoId?) "RMCEffectHealBusy"
    });
    if (this._net.IsClient)
      return true;
    this._popups.PopupEntity(this.Loc.GetString("cm-cpr-self-start-perform", (nameof (target), (object) target)), target, performer, PopupType.Medium);
    string message = this.Loc.GetString("cm-cpr-other-start-perform", (nameof (performer), (object) performer), (nameof (target), (object) target));
    Filter filter = Filter.Pvs(performer).RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => e == performer));
    this._popups.PopupEntity(message, performer, filter, true, PopupType.Medium);
    return true;
  }
}
