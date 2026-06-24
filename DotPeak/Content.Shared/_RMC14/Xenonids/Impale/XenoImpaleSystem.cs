// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Impale.XenoImpaleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Finesse;
using Content.Shared.Actions;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Impale;

public sealed class XenoImpaleSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedColorFlashEffectSystem _flash;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private XenoSystem _xeno;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoImpaleComponent, XenoImpaleActionEvent>(new EntityEventRefHandler<XenoImpaleComponent, XenoImpaleActionEvent>(this.OnXenoImpaleAction));
  }

  private void OnXenoImpaleAction(Entity<XenoImpaleComponent> xeno, ref XenoImpaleActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((EntityTargetActionEvent) args))
      return;
    args.Handled = true;
    if (this.HasComp<XenoMarkedComponent>(args.Target))
    {
      ProtoId<EmotePrototype>? emote = xeno.Comp.Emote;
      if (emote.HasValue)
      {
        ProtoId<EmotePrototype> valueOrDefault = emote.GetValueOrDefault();
        this._emote.TryEmoteWithChat((EntityUid) xeno, valueOrDefault, cooldown: xeno.Comp.EmoteCooldown);
      }
      XenoSecondImpaleComponent secondImpaleComponent = this.EnsureComp<XenoSecondImpaleComponent>(args.Target);
      secondImpaleComponent.Damage = xeno.Comp.Damage;
      secondImpaleComponent.ImpaleAt = this._timing.CurTime + xeno.Comp.SecondImpaleTime;
      secondImpaleComponent.Origin = (EntityUid) xeno;
      this.RemCompDeferred<XenoMarkedComponent>(args.Target);
    }
    this.Impale(xeno.Comp.Damage, xeno.Comp.AP, xeno.Comp.Animation, xeno.Comp.Sound, args.Target, (EntityUid) xeno);
  }

  private void Impale(
    DamageSpecifier damage,
    int aP,
    EntProtoId animation,
    SoundSpecifier sound,
    EntityUid target,
    EntityUid xeno)
  {
    DamageableSystem damage1 = this._damage;
    EntityUid? uid = new EntityUid?(target);
    DamageSpecifier damage2 = this._xeno.TryApplyXenoSlashDamageMultiplier(target, damage);
    int num = aP;
    EntityUid? origin = new EntityUid?(xeno);
    EntityUid? tool = new EntityUid?(xeno);
    int armorPiercing = num;
    FixedPoint2? total = damage1.TryChangeDamage(uid, damage2, origin: origin, tool: tool, armorPiercing: armorPiercing)?.GetTotal();
    FixedPoint2 zero = FixedPoint2.Zero;
    if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
    {
      Filter filter1 = Filter.Pvs(target, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno));
      SharedColorFlashEffectSystem flash = this._flash;
      Color red = Color.Red;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(target);
      Filter filter2 = filter1;
      flash.RaiseEffect(red, entities, filter2);
    }
    this._rmcMelee.DoLunge(xeno, target);
    if (this._net.IsClient)
      return;
    this._audio.PlayPvs(sound, xeno);
    this.SpawnAttachedTo((string) animation, target.ToCoordinates(), rotation: new Angle());
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoSecondImpaleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoSecondImpaleComponent>();
    EntityUid uid;
    XenoSecondImpaleComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(comp1.ImpaleAt > curTime))
      {
        this.Impale(comp1.Damage, comp1.AP, comp1.Animation, comp1.Sound, uid, comp1.Origin);
        this.RemCompDeferred<XenoSecondImpaleComponent>(uid);
      }
    }
  }
}
