// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Headbite.XenoHeadbiteSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Headbite;

public sealed class XenoHeadbiteSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MobThresholdSystem _mobThresholds;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private SharedXenoHealSystem _xenoHeal;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private StatusEffectsSystem _status;
  private static readonly ProtoId<DamageTypePrototype> LethalDamageType = (ProtoId<DamageTypePrototype>) "Asphyxiation";
  private static readonly ProtoId<StatusEffectPrototype> Unconsciousness = (ProtoId<StatusEffectPrototype>) "Unconscious";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoHeadbiteComponent, XenoHeadbiteActionEvent>(new EntityEventRefHandler<XenoHeadbiteComponent, XenoHeadbiteActionEvent>(this.OnXenoHeadbiteAction));
    this.SubscribeLocalEvent<XenoHeadbiteComponent, XenoHeadbiteDoAfterEvent>(new EntityEventRefHandler<XenoHeadbiteComponent, XenoHeadbiteDoAfterEvent>(this.OnXenoHeadbiteDoAfter));
  }

  private void OnXenoHeadbiteAction(
    Entity<XenoHeadbiteComponent> xeno,
    ref XenoHeadbiteActionEvent args)
  {
    EntityUid target = args.Target;
    if (!this.CanHeadbite((EntityUid) xeno, target))
      return;
    DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.HeadbiteDelay, (DoAfterEvent) new XenoHeadbiteDoAfterEvent(), new EntityUid?((EntityUid) xeno), new EntityUid?(target))
    {
      BreakOnMove = true,
      BreakOnDamage = false,
      ForceVisible = true,
      CancelDuplicate = true,
      DuplicateCondition = DuplicateConditions.SameEvent
    };
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-headbite-self", (nameof (xeno), (object) xeno.Owner), ("target", (object) target)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.Medium);
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-headbite-others", (nameof (xeno), (object) xeno.Owner), ("target", (object) target)), (EntityUid) xeno, Filter.PvsExcept((EntityUid) xeno), true, PopupType.MediumCaution);
    if (!this._doAfter.TryStartDoAfter(args1))
      return;
    args.Handled = true;
  }

  private void OnXenoHeadbiteDoAfter(
    Entity<XenoHeadbiteComponent> xeno,
    ref XenoHeadbiteDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    if (!this.CanHeadbite((EntityUid) xeno, valueOrDefault))
      return;
    args.Handled = true;
    if (this._net.IsServer)
    {
      this.SpawnAttachedTo((string) xeno.Comp.HealEffect, xeno.Owner.ToCoordinates(), rotation: new Angle());
      this.SpawnAttachedTo((string) xeno.Comp.HeadbiteEffect, valueOrDefault.ToCoordinates(), rotation: new Angle());
      this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.Emote, cooldown: xeno.Comp.EmoteCooldown);
      this._audio.PlayPvs(xeno.Comp.HitSound, (EntityUid) xeno);
    }
    this._xenoHeal.CreateHealStacks((EntityUid) xeno, (FixedPoint2) xeno.Comp.HealAmount, xeno.Comp.HealDelay, 1, xeno.Comp.HealDelay);
    this._jitter.DoJitter((EntityUid) xeno, xeno.Comp.JitterTime, true, 80f, 8f, true);
    DamageableSystem damage1 = this._damage;
    EntityUid? uid1 = new EntityUid?(valueOrDefault);
    DamageSpecifier damage2 = xeno.Comp.Damage;
    nullable = new EntityUid?();
    EntityUid? origin1 = nullable;
    nullable = new EntityUid?();
    EntityUid? tool1 = nullable;
    FixedPoint2? total = damage1.TryChangeDamage(uid1, damage2, origin: origin1, tool: tool1)?.GetTotal();
    FixedPoint2 zero = FixedPoint2.Zero;
    if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
    {
      Filter filter1 = Filter.Pvs(valueOrDefault, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
      SharedColorFlashEffectSystem colorFlash = this._colorFlash;
      Color red = Color.Red;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(valueOrDefault);
      Filter filter2 = filter1;
      colorFlash.RaiseEffect(red, entities, filter2);
    }
    FixedPoint2? threshold;
    DamageableComponent comp;
    if (this._mobThresholds.TryGetDeadThreshold(valueOrDefault, out threshold) && this.TryComp<DamageableComponent>(valueOrDefault, out comp))
    {
      FixedPoint2 fixedPoint2 = threshold.Value - comp.TotalDamage;
      DamageSpecifier damageSpecifier = new DamageSpecifier(this._prototypeManager.Index<DamageTypePrototype>(XenoHeadbiteSystem.LethalDamageType), fixedPoint2);
      DamageableSystem damage3 = this._damage;
      EntityUid? uid2 = new EntityUid?(valueOrDefault);
      DamageSpecifier damage4 = damageSpecifier;
      EntityUid? origin2 = new EntityUid?((EntityUid) xeno);
      nullable = new EntityUid?();
      EntityUid? tool2 = nullable;
      damage3.TryChangeDamage(uid2, damage4, true, origin: origin2, tool: tool2);
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-headbite-hit-self", (nameof (xeno), (object) xeno.Owner), ("target", (object) valueOrDefault)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.Medium);
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-headbite-hit-others", (nameof (xeno), (object) xeno.Owner), ("target", (object) valueOrDefault)), (EntityUid) xeno, Filter.PvsExcept((EntityUid) xeno), true, PopupType.MediumCaution);
  }

  private bool CanHeadbite(EntityUid xeno, EntityUid target)
  {
    if (!this._mobState.IsCritical(target) && !this._status.HasStatusEffect(target, "Unconscious"))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-headbite-warning"), xeno, new EntityUid?(xeno), PopupType.SmallCaution);
      return false;
    }
    VictimInfectedComponent comp;
    if (!this.HasComp<XenoComponent>(xeno) || !this.TryComp<VictimInfectedComponent>(target, out comp) || !this._hive.IsMember((Entity<HiveMemberComponent>) xeno, comp.Hive))
      return true;
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-headbite-warning-larva"), xeno, new EntityUid?(xeno), PopupType.SmallCaution);
    return false;
  }
}
