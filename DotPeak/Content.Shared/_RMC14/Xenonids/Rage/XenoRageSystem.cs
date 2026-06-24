// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Rage.XenoRageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Rage;

public sealed class XenoRageSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedXenoHealSystem _xenoHeal;
  [Dependency]
  private SharedAuraSystem _aura;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private CMArmorSystem _armor;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoRageComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoRageComponent, MeleeHitEvent>(this.OnRageMeleeHit));
    this.SubscribeLocalEvent<XenoRageComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoRageComponent, RefreshMovementSpeedModifiersEvent>(this.OnRageRefreshSpeed));
    this.SubscribeLocalEvent<XenoRageComponent, GetMeleeAttackRateEvent>(new EntityEventRefHandler<XenoRageComponent, GetMeleeAttackRateEvent>(this.OnRageGetMeleeAttackRate));
    this.SubscribeLocalEvent<XenoRageComponent, CMGetArmorEvent>(new EntityEventRefHandler<XenoRageComponent, CMGetArmorEvent>(this.OnRageGetArmor));
    this.SubscribeLocalEvent<XenoRageComponent, ExaminedEvent>(new EntityEventRefHandler<XenoRageComponent, ExaminedEvent>(this.OnRageExamine));
  }

  public void IncrementRage(Entity<XenoRageComponent?> xeno, int amount)
  {
    if (!this.Resolve<XenoRageComponent>((EntityUid) xeno, ref xeno.Comp, false) || xeno.Comp.RageCooldownExpireAt > this._timing.CurTime || xeno.Comp.RageLocked)
      return;
    xeno.Comp.Rage += amount;
    xeno.Comp.Rage = Math.Min(xeno.Comp.Rage, xeno.Comp.MaxRage);
    this.Dirty<XenoRageComponent>(xeno);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    this._armor.UpdateArmorValue((Entity<CMArmorComponent>) xeno.Owner);
    if (xeno.Comp.Rage < xeno.Comp.MaxRage)
      return;
    this.RageLock((Entity<XenoRageComponent>) (xeno.Owner, xeno.Comp));
  }

  public int GetRage(EntityUid uid)
  {
    XenoRageComponent comp;
    return !this.TryComp<XenoRageComponent>(uid, out comp) ? 0 : comp.Rage;
  }

  public void RageLock(Entity<XenoRageComponent> xeno)
  {
    xeno.Comp.RageLocked = true;
    xeno.Comp.RageLockExpireAt = this._timing.CurTime + xeno.Comp.RageLockDuration;
    this.Dirty<XenoRageComponent>(xeno);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    this._armor.UpdateArmorValue((Entity<CMArmorComponent>) xeno.Owner);
    this._aura.GiveAura((EntityUid) xeno, xeno.Comp.RageLockColor, new TimeSpan?(xeno.Comp.RageLockDuration), 3f);
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rage-lock"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.Medium);
  }

  public void RageUnlock(Entity<XenoRageComponent> xeno)
  {
    xeno.Comp.RageLocked = false;
    this.IncrementRage((Entity<XenoRageComponent>) xeno.Owner, -xeno.Comp.Rage);
    xeno.Comp.RageCooldownExpireAt = this._timing.CurTime + xeno.Comp.RageCooldownDuration;
    this.Dirty<XenoRageComponent>(xeno);
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-rage-expire", ("cooldown", (object) xeno.Comp.RageCooldownDuration.Seconds.ToString())), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
    this._aura.GiveAura((EntityUid) xeno, xeno.Comp.RageLockWeakenColor, new TimeSpan?(xeno.Comp.RageCooldownDuration));
  }

  private void OnRageMeleeHit(Entity<XenoRageComponent> xeno, ref MeleeHitEvent args)
  {
    if (!args.IsHit || args.HitEntities.Count == 0)
      return;
    bool flag = false;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget(xeno.Owner, hitEntity))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    this.IncrementRage((Entity<XenoRageComponent>) xeno.Owner, 1);
    FixedPoint2 healAmount = (FixedPoint2) (0.05 * (double) xeno.Comp.Rage + 0.3) * xeno.Comp.HealAmount;
    this._xenoHeal.CreateHealStacks((EntityUid) xeno, healAmount, xeno.Comp.RageHealTime, 1, xeno.Comp.RageHealTime);
    xeno.Comp.LastHit = this._timing.CurTime;
    this.Dirty<XenoRageComponent>(xeno);
  }

  private void OnRageRefreshSpeed(
    Entity<XenoRageComponent> xeno,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    float num = (float) (1.0 + (double) xeno.Comp.Rage * (double) xeno.Comp.SpeedBuffPerRage);
    args.ModifySpeed(num, num);
  }

  private void OnRageGetMeleeAttackRate(
    Entity<XenoRageComponent> xeno,
    ref GetMeleeAttackRateEvent args)
  {
    args.Rate += (float) xeno.Comp.Rage * xeno.Comp.AttackSpeedPerRage;
  }

  private void OnRageGetArmor(Entity<XenoRageComponent> xeno, ref CMGetArmorEvent args)
  {
    args.XenoArmor += xeno.Comp.Rage * xeno.Comp.ArmorPerRage;
  }

  private void OnRageExamine(Entity<XenoRageComponent> xeno, ref ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    using (args.PushGroup("XenoRageComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-xeno-rage-examine", (nameof (xeno), (object) xeno), ("amount", (object) xeno.Comp.Rage), ("max", (object) xeno.Comp.MaxRage)));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoRageComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoRageComponent>();
    EntityUid uid;
    XenoRageComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.LastHit + comp1.RageDecayTime <= curTime && comp1.Rage > 0 && !comp1.RageLocked)
      {
        this.IncrementRage((Entity<XenoRageComponent>) (uid, comp1), -1);
        comp1.LastHit = curTime;
        this.Dirty(uid, (IComponent) comp1);
      }
      if (comp1.RageLocked && comp1.RageLockExpireAt <= curTime)
        this.RageUnlock((Entity<XenoRageComponent>) (uid, comp1));
    }
  }
}
