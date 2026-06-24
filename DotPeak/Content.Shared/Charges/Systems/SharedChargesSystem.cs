// Decompiled with JetBrains decompiler
// Type: Content.Shared.Charges.Systems.SharedChargesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions.Events;
using Content.Shared.Charges.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Charges.Systems;

public abstract class SharedChargesSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming _timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LimitedChargesComponent, ExaminedEvent>(new ComponentEventHandler<LimitedChargesComponent, ExaminedEvent>((object) this, __methodptr(OnExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LimitedChargesComponent, ActionAttemptEvent>(new EntityEventRefHandler<LimitedChargesComponent, ActionAttemptEvent>((object) this, __methodptr(OnChargesAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LimitedChargesComponent, MapInitEvent>(new EntityEventRefHandler<LimitedChargesComponent, MapInitEvent>((object) this, __methodptr(OnChargesMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LimitedChargesComponent, ActionPerformedEvent>(new EntityEventRefHandler<LimitedChargesComponent, ActionPerformedEvent>((object) this, __methodptr(OnChargesPerformed)), (Type[]) null, (Type[]) null);
  }

  private void OnExamine(EntityUid uid, LimitedChargesComponent comp, ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    Entity<LimitedChargesComponent, AutoRechargeComponent> entity = new Entity<LimitedChargesComponent, AutoRechargeComponent>(uid, comp, (AutoRechargeComponent) null);
    int currentCharges = this.GetCurrentCharges(entity);
    using (args.PushGroup("LimitedChargesComponent"))
    {
      args.PushMarkup(this.Loc.GetString("limited-charges-charges-remaining", ("charges", (object) currentCharges)));
      if (currentCharges == comp.MaxCharges)
        args.PushMarkup(this.Loc.GetString("limited-charges-max-charges"));
      if (currentCharges == comp.MaxCharges || !this.Resolve<AutoRechargeComponent>(uid, ref entity.Comp2, false))
        return;
      TimeSpan nextRechargeTime = this.GetNextRechargeTime(entity);
      args.PushMarkup(this.Loc.GetString("limited-charges-recharging", ("seconds", (object) nextRechargeTime.TotalSeconds.ToString("F1"))));
    }
  }

  private void OnChargesAttempt(Entity<LimitedChargesComponent> ent, ref ActionAttemptEvent args)
  {
    if (args.Cancelled || this.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((ent.Owner, ent.Comp, (AutoRechargeComponent) null))) > 0)
      return;
    args.Cancelled = true;
  }

  private void OnChargesPerformed(
    Entity<LimitedChargesComponent> ent,
    ref ActionPerformedEvent args)
  {
    this.AddCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((ent.Owner, ent.Comp)), -1);
  }

  private void OnChargesMapInit(Entity<LimitedChargesComponent> ent, ref MapInitEvent args)
  {
    if (ent.Comp.LastCharges == 0)
      ent.Comp.LastCharges = ent.Comp.MaxCharges;
    else if (ent.Comp.LastCharges < 0)
      ent.Comp.LastCharges = 0;
    ent.Comp.LastUpdate = this._timing.CurTime;
    this.Dirty<LimitedChargesComponent>(ent, (MetaDataComponent) null);
  }

  public bool HasCharges(Entity<LimitedChargesComponent?> action, int charges)
  {
    return this.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(action)) >= charges;
  }

  public void AddCharges(
    Entity<LimitedChargesComponent?, AutoRechargeComponent?> action,
    int addCharges)
  {
    if (addCharges == 0)
      return;
    ref LimitedChargesComponent local = ref action.Comp1;
    if (local == null)
      local = this.EnsureComp<LimitedChargesComponent>(action.Owner);
    int currentCharges = this.GetCurrentCharges(action);
    int num1 = currentCharges + addCharges;
    if (currentCharges == num1)
      return;
    if (num1 == action.Comp1.MaxCharges || currentCharges == action.Comp1.MaxCharges)
    {
      action.Comp1.LastUpdate = this._timing.CurTime;
      action.Comp1.LastCharges = action.Comp1.MaxCharges;
    }
    else if (this.Resolve<AutoRechargeComponent>(action.Owner, ref action.Comp2, false))
    {
      TimeSpan rechargeDuration = action.Comp2.RechargeDuration;
      int num2 = (int) ((this._timing.CurTime - action.Comp1.LastUpdate) / rechargeDuration);
      action.Comp1.LastCharges += num2;
      action.Comp1.LastUpdate += (double) num2 * rechargeDuration;
    }
    action.Comp1.LastCharges = Math.Clamp(action.Comp1.LastCharges + addCharges, 0, action.Comp1.MaxCharges);
    this.Dirty(action.Owner, (IComponent) action.Comp1, (MetaDataComponent) null);
  }

  public bool TryUseCharge(Entity<LimitedChargesComponent?> entity)
  {
    return this.TryUseCharges(entity, 1);
  }

  public bool TryUseCharges(Entity<LimitedChargesComponent?> entity, int amount)
  {
    if (this.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(entity)) < amount)
      return false;
    this.AddCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(entity), -amount);
    return true;
  }

  public bool IsEmpty(Entity<LimitedChargesComponent?> entity)
  {
    return this.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(entity)) == 0;
  }

  public void ResetCharges(Entity<LimitedChargesComponent?> action)
  {
    if (!this.Resolve<LimitedChargesComponent>(action.Owner, ref action.Comp, false) || this.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((action.Owner, action.Comp, (AutoRechargeComponent) null))) == action.Comp.MaxCharges)
      return;
    action.Comp.LastCharges = action.Comp.MaxCharges;
    action.Comp.LastUpdate = this._timing.CurTime;
    this.Dirty<LimitedChargesComponent>(action, (MetaDataComponent) null);
  }

  public void SetCharges(Entity<LimitedChargesComponent?> action, int value)
  {
    ref LimitedChargesComponent local = ref action.Comp;
    if (local == null)
      local = this.EnsureComp<LimitedChargesComponent>(action.Owner);
    int num = Math.Clamp(value, 0, action.Comp.MaxCharges);
    if (action.Comp.LastCharges == num)
      return;
    action.Comp.LastCharges = num;
    action.Comp.LastUpdate = this._timing.CurTime;
    this.Dirty<LimitedChargesComponent>(action, (MetaDataComponent) null);
  }

  public TimeSpan GetNextRechargeTime(
    Entity<LimitedChargesComponent?, AutoRechargeComponent?> entity)
  {
    if (!this.Resolve<LimitedChargesComponent, AutoRechargeComponent>(entity.Owner, ref entity.Comp1, ref entity.Comp2, false))
      return TimeSpan.Zero;
    TimeSpan timeSpan = (double) (entity.Comp1.MaxCharges - entity.Comp1.LastCharges) * entity.Comp2.RechargeDuration + entity.Comp1.LastUpdate - this._timing.CurTime;
    return timeSpan < TimeSpan.Zero ? TimeSpan.Zero : TimeSpan.FromSeconds(timeSpan.TotalSeconds % entity.Comp2.RechargeDuration.TotalSeconds);
  }

  public int GetCurrentCharges(
    Entity<LimitedChargesComponent?, AutoRechargeComponent?> entity)
  {
    if (!this.Resolve<LimitedChargesComponent>(entity.Owner, ref entity.Comp1, false))
      return -1;
    int num = 0;
    if (this.Resolve<AutoRechargeComponent>(entity.Owner, ref entity.Comp2, false) && entity.Comp2.RechargeDuration.TotalSeconds != 0.0)
      num = (int) ((this._timing.CurTime - entity.Comp1.LastUpdate).TotalSeconds / entity.Comp2.RechargeDuration.TotalSeconds);
    return Math.Clamp(entity.Comp1.LastCharges + num, 0, entity.Comp1.MaxCharges);
  }
}
