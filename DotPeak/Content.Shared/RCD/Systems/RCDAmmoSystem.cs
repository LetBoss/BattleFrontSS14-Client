// Decompiled with JetBrains decompiler
// Type: Content.Shared.RCD.Systems.RCDAmmoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.RCD.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.RCD.Systems;

public sealed class RCDAmmoSystem : EntitySystem
{
  [Dependency]
  private SharedChargesSystem _sharedCharges;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RCDAmmoComponent, ExaminedEvent>(new ComponentEventHandler<RCDAmmoComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<RCDAmmoComponent, AfterInteractEvent>(new ComponentEventHandler<RCDAmmoComponent, AfterInteractEvent>(this.OnAfterInteract));
  }

  private void OnExamine(EntityUid uid, RCDAmmoComponent comp, ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    string text = this.Loc.GetString("rcd-ammo-component-on-examine", ("charges", (object) comp.Charges));
    args.PushText(text);
  }

  private void OnAfterInteract(EntityUid uid, RCDAmmoComponent comp, AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach || !this._timing.IsFirstTimePredicted)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    LimitedChargesComponent comp1;
    if (!valueOrDefault.Valid || !this.HasComp<RCDComponent>(valueOrDefault) || !this.TryComp<LimitedChargesComponent>(valueOrDefault, out comp1))
      return;
    int currentCharges = this._sharedCharges.GetCurrentCharges((Entity<LimitedChargesComponent, AutoRechargeComponent>) (valueOrDefault, comp1));
    EntityUid user = args.User;
    args.Handled = true;
    int addCharges = Math.Min(comp1.MaxCharges - currentCharges, comp.Charges);
    if (addCharges <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rcd-ammo-component-after-interact-full"), valueOrDefault, new EntityUid?(user));
    }
    else
    {
      this._popup.PopupClient(this.Loc.GetString("rcd-ammo-component-after-interact-refilled"), valueOrDefault, new EntityUid?(user));
      this._sharedCharges.AddCharges((Entity<LimitedChargesComponent, AutoRechargeComponent>) valueOrDefault, addCharges);
      comp.Charges -= addCharges;
      this.Dirty(uid, (IComponent) comp);
      if (comp.Charges > 0)
        return;
      this.QueueDel(new EntityUid?(uid));
    }
  }
}
