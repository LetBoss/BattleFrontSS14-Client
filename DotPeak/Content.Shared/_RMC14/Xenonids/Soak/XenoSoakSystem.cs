// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Soak.XenoSoakSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Stab;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Soak;

public sealed class XenoSoakSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _action;
  [Dependency]
  private SharedAuraSystem _aura;
  [Dependency]
  private XenoPlasmaSystem _plasma;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoSoakComponent, XenoSoakActionEvent>(new EntityEventRefHandler<XenoSoakComponent, XenoSoakActionEvent>(this.OnXenoSoakAction));
    this.SubscribeLocalEvent<XenoSoakingDamageComponent, DamageChangedEvent>(new EntityEventRefHandler<XenoSoakingDamageComponent, DamageChangedEvent>(this.OnXenoSoakingDamageChanged));
  }

  private void OnXenoSoakAction(Entity<XenoSoakComponent> xeno, ref XenoSoakActionEvent args)
  {
    if (args.Handled || !this._plasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    args.Handled = true;
    XenoSoakingDamageComponent soakingDamageComponent = this.EnsureComp<XenoSoakingDamageComponent>((EntityUid) xeno);
    soakingDamageComponent.EffectExpiresAt = this._timing.CurTime + xeno.Comp.Duration;
    soakingDamageComponent.DamageAccumulated = 0.0f;
    this.Dirty(xeno.Owner, (IComponent) soakingDamageComponent);
    this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-soak-self"), this.Loc.GetString("rmc-xeno-soak-others", (nameof (xeno), (object) xeno)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.MediumCaution);
    this._aura.GiveAura((EntityUid) xeno, soakingDamageComponent.SoakColor, new TimeSpan?(xeno.Comp.Duration));
  }

  private void OnXenoSoakingDamageChanged(
    Entity<XenoSoakingDamageComponent> xeno,
    ref DamageChangedEvent args)
  {
    if (!args.DamageIncreased || args.DamageDelta == null || args.DamageDelta.GetTotal() < 0)
      return;
    xeno.Comp.DamageAccumulated += args.DamageDelta.GetTotal().Float();
    if ((double) xeno.Comp.DamageAccumulated < (double) xeno.Comp.DamageGoal)
      return;
    DamageSpecifier damage = -this._rmcDamageable.DistributeTypesTotal((Entity<DamageableComponent>) xeno.Owner, xeno.Comp.Heal);
    this._damage.TryChangeDamage(new EntityUid?((EntityUid) xeno), damage, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno));
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoTailStabEvent>((EntityUid) xeno))
      this._action.ClearCooldown(new Entity<ActionComponent>?(entity.AsNullable()));
    this.RemCompDeferred<XenoSoakingDamageComponent>((EntityUid) xeno);
    this._aura.GiveAura((EntityUid) xeno, xeno.Comp.RageColor, new TimeSpan?(xeno.Comp.RageDuration));
    if (!this._net.IsServer)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-soak-end-self"), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-soak-end-others", (nameof (xeno), (object) xeno)), (EntityUid) xeno, Filter.PvsExcept((EntityUid) xeno), true, PopupType.MediumCaution);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoSoakingDamageComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoSoakingDamageComponent>();
    EntityUid uid;
    XenoSoakingDamageComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(comp1.EffectExpiresAt > curTime))
      {
        this.RemCompDeferred<XenoSoakingDamageComponent>(uid);
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-soak-end-fail"), uid, uid, PopupType.SmallCaution);
      }
    }
  }
}
