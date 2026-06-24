// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.SharedGodmodeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Destructible;
using Content.Shared.Rejuvenate;
using Content.Shared.Slippery;
using Content.Shared.StatusEffectNew;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Damage.Systems;

public abstract class SharedGodmodeSystem : EntitySystem
{
  [Dependency]
  private DamageableSystem _damageable;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GodmodeComponent, BeforeDamageChangedEvent>(new ComponentEventRefHandler<GodmodeComponent, BeforeDamageChangedEvent>((object) this, __methodptr(OnBeforeDamageChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GodmodeComponent, BeforeStatusEffectAddedEvent>(new ComponentEventRefHandler<GodmodeComponent, BeforeStatusEffectAddedEvent>((object) this, __methodptr(OnBeforeStatusEffect)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GodmodeComponent, BeforeStaminaDamageEvent>(new ComponentEventRefHandler<GodmodeComponent, BeforeStaminaDamageEvent>((object) this, __methodptr(OnBeforeStaminaDamage)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GodmodeComponent, SlipAttemptEvent>(new ComponentEventHandler<GodmodeComponent, SlipAttemptEvent>((object) this, __methodptr(OnSlipAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GodmodeComponent, DestructionAttemptEvent>(new EntityEventRefHandler<GodmodeComponent, DestructionAttemptEvent>((object) this, __methodptr(OnDestruction)), (Type[]) null, (Type[]) null);
  }

  private void OnSlipAttempt(EntityUid uid, GodmodeComponent component, SlipAttemptEvent args)
  {
    args.NoSlip = true;
  }

  private void OnBeforeDamageChanged(
    EntityUid uid,
    GodmodeComponent component,
    ref BeforeDamageChangedEvent args)
  {
    args.Cancelled = true;
  }

  private void OnBeforeStatusEffect(
    EntityUid uid,
    GodmodeComponent component,
    ref BeforeStatusEffectAddedEvent args)
  {
    args.Cancelled = true;
  }

  private void OnBeforeStaminaDamage(
    EntityUid uid,
    GodmodeComponent component,
    ref BeforeStaminaDamageEvent args)
  {
    args.Cancelled = true;
  }

  private void OnDestruction(Entity<GodmodeComponent> ent, ref DestructionAttemptEvent args)
  {
    args.Cancel();
  }

  public virtual void EnableGodmode(EntityUid uid, GodmodeComponent? godmode = null)
  {
    if (godmode == null)
      godmode = this.EnsureComp<GodmodeComponent>(uid);
    DamageableComponent damageableComponent;
    if (this.TryComp<DamageableComponent>(uid, ref damageableComponent))
      godmode.OldDamage = new DamageSpecifier(damageableComponent.Damage);
    this.RaiseLocalEvent<RejuvenateEvent>(uid, new RejuvenateEvent(), false);
  }

  public virtual void DisableGodmode(EntityUid uid, GodmodeComponent? godmode = null)
  {
    if (!this.Resolve<GodmodeComponent>(uid, ref godmode, false))
      return;
    DamageableComponent damageable;
    if (this.TryComp<DamageableComponent>(uid, ref damageable) && godmode.OldDamage != null)
      this._damageable.SetDamage(uid, damageable, godmode.OldDamage);
    this.RemComp<GodmodeComponent>(uid);
  }

  public bool ToggleGodmode(EntityUid uid)
  {
    GodmodeComponent godmode;
    if (this.TryComp<GodmodeComponent>(uid, ref godmode))
    {
      this.DisableGodmode(uid, godmode);
      return false;
    }
    this.EnableGodmode(uid, godmode);
    return true;
  }
}
