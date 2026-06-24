// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Shields.CrusherShieldSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Shields;

public sealed class CrusherShieldSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private XenoShieldSystem _shield;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CrusherShieldComponent, DamageModifyAfterResistEvent>(new EntityEventRefHandler<CrusherShieldComponent, DamageModifyAfterResistEvent>(this.OnDamage), new Type[1]
    {
      typeof (XenoShieldSystem)
    });
    this.SubscribeLocalEvent<CrusherShieldComponent, GetExplosionResistanceEvent>(new EntityEventRefHandler<CrusherShieldComponent, GetExplosionResistanceEvent>(this.OnGetExplosionResistance));
    this.SubscribeLocalEvent<CrusherShieldComponent, RemovedShieldEvent>(new EntityEventRefHandler<CrusherShieldComponent, RemovedShieldEvent>(this.OnShieldRemove));
    this.SubscribeLocalEvent<CrusherShieldComponent, XenoDefensiveShieldActionEvent>(new EntityEventRefHandler<CrusherShieldComponent, XenoDefensiveShieldActionEvent>(this.OnXenoDefensiveShieldAction));
  }

  private void OnXenoDefensiveShieldAction(
    Entity<CrusherShieldComponent> xeno,
    ref XenoDefensiveShieldActionEvent args)
  {
    if (args.Handled || !this._xenoPlasma.TryRemovePlasma((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    args.Handled = true;
    this.EnsureComp<XenoShieldComponent>((EntityUid) xeno);
    this._shield.ApplyShield((EntityUid) xeno, XenoShieldSystem.ShieldType.Crusher, xeno.Comp.Amount);
    this.ApplyEffects(xeno);
    if (this._net.IsClient)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-defensive-shield-activate", ("user", (object) xeno)), (EntityUid) xeno, Filter.PvsExcept((EntityUid) xeno), true, PopupType.MediumCaution);
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-defensive-shield-activate-self", ("user", (object) xeno)), (EntityUid) xeno, (EntityUid) xeno, PopupType.Medium);
    this.SpawnAttachedTo((string) xeno.Comp.Effect, xeno.Owner.ToCoordinates(), rotation: new Angle());
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoDefensiveShieldActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)), true);
  }

  public void ApplyEffects(Entity<CrusherShieldComponent> ent)
  {
    if (!this.TryComp<CMArmorComponent>((EntityUid) ent, out CMArmorComponent _))
      return;
    ent.Comp.ExplosionOffAt = this._timing.CurTime + ent.Comp.ExplosionResistanceDuration;
    ent.Comp.ShieldOffAt = this._timing.CurTime + ent.Comp.ShieldDuration;
    ent.Comp.ExplosionResistApplying = true;
  }

  public void OnShieldRemove(Entity<CrusherShieldComponent> ent, ref RemovedShieldEvent args)
  {
    if (args.Type != XenoShieldSystem.ShieldType.Crusher)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-defensive-shield-end"), (EntityUid) ent, (EntityUid) ent, PopupType.MediumCaution);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoDefensiveShieldActionEvent>((EntityUid) ent))
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) entity.Owner), false);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<CrusherShieldComponent, XenoShieldComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CrusherShieldComponent, XenoShieldComponent>();
    EntityUid uid;
    CrusherShieldComponent comp1;
    XenoShieldComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.ExplosionResistApplying && comp1.ExplosionOffAt <= curTime)
      {
        comp1.ExplosionResistApplying = false;
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-defensive-shield-resist-end"), uid, uid, PopupType.SmallCaution);
      }
      if (comp2.Active && comp2.Shield == XenoShieldSystem.ShieldType.Crusher && comp1.ShieldOffAt <= curTime)
        this._shield.RemoveShield(uid, XenoShieldSystem.ShieldType.Crusher);
    }
  }

  public void OnDamage(Entity<CrusherShieldComponent> ent, ref DamageModifyAfterResistEvent args)
  {
    XenoShieldComponent comp;
    if (!this.TryComp<XenoShieldComponent>((EntityUid) ent, out comp) || !comp.Active || comp.Shield != XenoShieldSystem.ShieldType.Crusher)
      return;
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in args.Damage.DamageDict)
    {
      if (!(args.Damage.DamageDict[keyValuePair.Key] <= 0))
      {
        args.Damage.DamageDict[keyValuePair.Key] -= (FixedPoint2) ent.Comp.DamageReduction;
        if (args.Damage.DamageDict[keyValuePair.Key] < 0)
          args.Damage.DamageDict[keyValuePair.Key] = (FixedPoint2) 0;
      }
    }
  }

  public void OnGetExplosionResistance(
    Entity<CrusherShieldComponent> ent,
    ref GetExplosionResistanceEvent args)
  {
    if (!ent.Comp.ExplosionResistApplying)
      return;
    float num = (float) Math.Pow(1.1, (double) ent.Comp.ExplosionResistance / 5.0);
    args.DamageCoefficient /= num;
  }
}
