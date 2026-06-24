// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Lifesteal.XenoLifestealSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Marines;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Lifesteal;

public sealed class XenoLifestealSystem : EntitySystem
{
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCEmoteSystem _rmcEmote;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedAuraSystem _aura;
  [Dependency]
  private INetManager _net;
  private readonly HashSet<Entity<MobStateComponent>> _targets = new HashSet<Entity<MobStateComponent>>();
  private Robust.Shared.GameObjects.EntityQuery<DamageableComponent> _damageableQuery;
  private Robust.Shared.GameObjects.EntityQuery<MarineComponent> _marineQuery;

  public override void Initialize()
  {
    this._damageableQuery = this.GetEntityQuery<DamageableComponent>();
    this._marineQuery = this.GetEntityQuery<MarineComponent>();
    this.SubscribeLocalEvent<XenoLifestealComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoLifestealComponent, MeleeHitEvent>(this.OnMeleeHit));
  }

  private void OnMeleeHit(Entity<XenoLifestealComponent> xeno, ref MeleeHitEvent args)
  {
    if (!args.IsHit || !this._xeno.CanHeal(xeno.Owner))
      return;
    bool flag = false;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    ProtoId<EmotePrototype>? emote = xeno.Comp.Emote;
    if (emote.HasValue)
    {
      ProtoId<EmotePrototype> valueOrDefault = emote.GetValueOrDefault();
      this._rmcEmote.TryEmoteWithChat((EntityUid) xeno, valueOrDefault, cooldown: xeno.Comp.EmoteCooldown);
    }
    DamageableComponent component;
    if (!this._damageableQuery.TryComp((EntityUid) xeno, out component))
      return;
    FixedPoint2 totalDamage = component.TotalDamage;
    if (totalDamage == FixedPoint2.Zero)
      return;
    this._targets.Clear();
    this._entityLookup.GetEntitiesInRange<MobStateComponent>(xeno.Owner.ToCoordinates(), xeno.Comp.TargetRange, this._targets);
    FixedPoint2 fixedPoint2 = xeno.Comp.BasePercentage;
    foreach (Entity<MobStateComponent> target in this._targets)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) target))
      {
        fixedPoint2 += xeno.Comp.TargetIncreasePercentage;
        if (fixedPoint2 >= xeno.Comp.MaxPercentage)
        {
          fixedPoint2 = xeno.Comp.MaxPercentage;
          break;
        }
      }
    }
    FixedPoint2 amount = -FixedPoint2.Clamp(totalDamage * fixedPoint2, xeno.Comp.MinHeal, xeno.Comp.MaxHeal);
    DamageSpecifier damage = this._rmcDamageable.DistributeTypes((Entity<DamageableComponent>) xeno.Owner, amount);
    this._damageable.TryChangeDamage(new EntityUid?((EntityUid) xeno), damage, true, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno));
    if (!(fixedPoint2 >= xeno.Comp.MaxPercentage))
      return;
    Filter filter = Filter.PvsExcept((EntityUid) xeno).RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => !this._marineQuery.HasComp(e)));
    this._popup.PopupEntity(this.Loc.GetString("rmc-lifesteal-more-marine", (nameof (xeno), (object) xeno.Owner)), (EntityUid) xeno, filter, true, PopupType.SmallCaution);
    this._popup.PopupClient(this.Loc.GetString("rmc-lifesteal-more-self"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    this._aura.GiveAura((EntityUid) xeno, xeno.Comp.AuraColor, new TimeSpan?(TimeSpan.FromSeconds(1L)));
    if (!this._net.IsServer)
      return;
    EntProtoId? maxEffect = xeno.Comp.MaxEffect;
    if (!maxEffect.HasValue)
      return;
    this.SpawnAttachedTo((string) maxEffect.GetValueOrDefault(), xeno.Owner.ToCoordinates(), rotation: new Angle());
  }
}
