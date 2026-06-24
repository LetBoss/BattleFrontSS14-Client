// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox.BulletBoxSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;

public sealed class BulletBoxSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedHandsSystem _hands;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<BulletBoxComponent, MapInitEvent>(new EntityEventRefHandler<BulletBoxComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<BulletBoxComponent, ExaminedEvent>(new EntityEventRefHandler<BulletBoxComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<BulletBoxComponent, InteractUsingEvent>(new EntityEventRefHandler<BulletBoxComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<BulletBoxComponent, BulletBoxTransferDoAfterEvent>(new EntityEventRefHandler<BulletBoxComponent, BulletBoxTransferDoAfterEvent>(this.OnTransferDoAfter));
    this.SubscribeLocalEvent<BulletBoxComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<BulletBoxComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetAlternativeVerbs));
  }

  private void OnMapInit(Entity<BulletBoxComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAppearance(ent);
  }

  private void OnExamined(Entity<BulletBoxComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("BulletBoxComponent"))
      args.PushText(this.Loc.GetString("rmc-bullet-box-amount", ("amount", (object) ent.Comp.Amount)));
  }

  private void OnGetAlternativeVerbs(
    Entity<BulletBoxComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    EntityUid user = args.User;
    EntityUid? usedId;
    if (!this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out usedId))
      return;
    Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent> used = new Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>(usedId.Value, (RefillableByBulletBoxComponent) null, (BallisticAmmoProviderComponent) null);
    if (!this.Resolve<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>((EntityUid) used, ref used.Comp1, ref used.Comp2, false))
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Act = (Action) (() =>
    {
      if (!this.CanTransferPopup(ent, user, ref used, true))
        return;
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.Delay, (DoAfterEvent) new BulletBoxTransferDoAfterEvent(true), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), usedId)
      {
        BreakOnMove = true,
        BreakOnDropItem = true,
        NeedHand = true
      });
    });
    alternativeVerb.Text = this.Loc.GetString("rmc-bullet-box-transferto");
    alternativeVerb.Impact = LogImpact.Low;
    verbs.Add(alternativeVerb);
  }

  private void OnInteractUsing(Entity<BulletBoxComponent> ent, ref InteractUsingEvent args)
  {
    Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent> used = new Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>(args.Used, (RefillableByBulletBoxComponent) null, (BallisticAmmoProviderComponent) null);
    if (!this.Resolve<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>((EntityUid) used, ref used.Comp1, ref used.Comp2, false))
      return;
    args.Handled = true;
    EntityUid user = args.User;
    if (!this.CanTransferPopup(ent, user, ref used, false))
      return;
    BulletBoxTransferDoAfterEvent @event = new BulletBoxTransferDoAfterEvent(false);
    TimeSpan delay = ent.Comp.Delay;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(args.Used))
    {
      BreakOnMove = true,
      BreakOnDropItem = true,
      NeedHand = true
    });
  }

  private void OnTransferDoAfter(
    Entity<BulletBoxComponent> ent,
    ref BulletBoxTransferDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? used1 = args.Used;
    if (!used1.HasValue)
      return;
    EntityUid valueOrDefault = used1.GetValueOrDefault();
    args.Handled = true;
    EntityUid user = args.User;
    Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent> used2 = new Entity<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>(valueOrDefault, (RefillableByBulletBoxComponent) null, (BallisticAmmoProviderComponent) null);
    bool toBox = args.ToBox;
    if (!this.CanTransferPopup(ent, user, ref used2, toBox) || used2.Comp2 == null)
      return;
    int num;
    if (!toBox)
    {
      int val1 = used2.Comp2.Capacity - used2.Comp2.Count;
      if (val1 <= 0)
        return;
      num = Math.Min(val1, ent.Comp.Amount);
      this._gun.SetBallisticUnspawned((Entity<BallisticAmmoProviderComponent>) ((EntityUid) used2, used2.Comp2), used2.Comp2.UnspawnedCount + num);
      ent.Comp.Amount -= num;
    }
    else
    {
      int val1 = ent.Comp.Max - ent.Comp.Amount;
      if (val1 <= 0)
        return;
      num = Math.Min(val1, used2.Comp2.Count);
      this._gun.SetBallisticUnspawned((Entity<BallisticAmmoProviderComponent>) ((EntityUid) used2, used2.Comp2), used2.Comp2.UnspawnedCount - num);
      ent.Comp.Amount += num;
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-bullet-box-transfer-done", ("amount", (object) num), ("used", (object) ent)), (EntityUid) ent, new EntityUid?(user));
    this.Dirty<BulletBoxComponent>(ent);
    this.UpdateAppearance(ent);
  }

  private bool CanTransferPopup(
    Entity<BulletBoxComponent> box,
    EntityUid user,
    ref Entity<RefillableByBulletBoxComponent?, BallisticAmmoProviderComponent?> used,
    bool transferToBox)
  {
    if (!this.Resolve<RefillableByBulletBoxComponent, BallisticAmmoProviderComponent>((EntityUid) used, ref used.Comp1, ref used.Comp2, false))
      return false;
    string message = (string) null;
    EntProtoId bulletType1 = box.Comp.BulletType;
    EntProtoId? bulletType2 = used.Comp1.BulletType;
    if ((bulletType2.HasValue ? (bulletType1 != bulletType2.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      message = this.Loc.GetString("rmc-bullet-box-wrong-rounds");
    if (!transferToBox)
    {
      if (used.Comp2.Count >= used.Comp2.Capacity)
        message = this.Loc.GetString("rmc-bullet-box-mag-full");
      if (box.Comp.Amount <= 0)
        message = this.Loc.GetString("rmc-bullet-box-box-empty");
    }
    else
    {
      if (used.Comp2.Count <= 0)
        message = this.Loc.GetString("rmc-bullet-box-mag-empty");
      if (box.Comp.Amount >= box.Comp.Max)
        message = this.Loc.GetString("rmc-bullet-box-box-full");
    }
    if (message == null)
      return true;
    this._popup.PopupClient(message, (EntityUid) box, new EntityUid?(user));
    return false;
  }

  public bool TryConsume(Entity<BulletBoxComponent?> box, int amount)
  {
    if (!this.Resolve<BulletBoxComponent>((EntityUid) box, ref box.Comp, false))
      return false;
    if (amount <= 0)
      return true;
    if (box.Comp.Amount < amount)
      return false;
    box.Comp.Amount -= amount;
    this.Dirty<BulletBoxComponent>(box);
    this.UpdateAppearance((Entity<BulletBoxComponent>) ((EntityUid) box, box.Comp));
    return true;
  }

  public bool TrySetAmount(Entity<BulletBoxComponent?> box, int amount)
  {
    if (!this.Resolve<BulletBoxComponent>((EntityUid) box, ref box.Comp, false))
      return false;
    box.Comp.Amount = Math.Clamp(amount, 0, box.Comp.Max);
    this.Dirty<BulletBoxComponent>(box);
    this.UpdateAppearance((Entity<BulletBoxComponent>) ((EntityUid) box, box.Comp));
    return true;
  }

  private void UpdateAppearance(Entity<BulletBoxComponent> ent)
  {
    double num = (double) ent.Comp.Amount / (double) ent.Comp.Max;
    BulletBoxVisuals bulletBoxVisuals = num >= 1.0 ? BulletBoxVisuals.Full : (num >= 0.66 ? BulletBoxVisuals.High : (num >= 0.33 ? BulletBoxVisuals.Medium : (num > 0.0 ? BulletBoxVisuals.Low : BulletBoxVisuals.Empty)));
    this._appearance.SetData((EntityUid) ent, (Enum) BulletBoxLayers.Fill, (object) bulletBoxVisuals);
  }
}
