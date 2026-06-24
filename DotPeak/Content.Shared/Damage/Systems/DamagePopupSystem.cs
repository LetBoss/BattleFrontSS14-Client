// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.DamagePopupSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Damage.Systems;

public sealed class DamagePopupSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popupSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamagePopupComponent, DamageChangedEvent>(new EntityEventRefHandler<DamagePopupComponent, DamageChangedEvent>((object) this, __methodptr(OnDamageChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamagePopupComponent, InteractHandEvent>(new EntityEventRefHandler<DamagePopupComponent, InteractHandEvent>((object) this, __methodptr(OnInteractHand)), (Type[]) null, (Type[]) null);
  }

  private void OnDamageChange(Entity<DamagePopupComponent> ent, ref DamageChangedEvent args)
  {
    if (args.DamageDelta == null)
      return;
    FixedPoint2 totalDamage = args.Damageable.TotalDamage;
    FixedPoint2 total = args.DamageDelta.GetTotal();
    string message;
    switch (ent.Comp.Type)
    {
      case DamagePopupType.Combined:
        message = $"{total.ToString()} | {totalDamage.ToString()}";
        break;
      case DamagePopupType.Total:
        message = totalDamage.ToString();
        break;
      case DamagePopupType.Delta:
        message = total.ToString();
        break;
      case DamagePopupType.Hit:
        message = "!";
        break;
      default:
        message = "Invalid type";
        break;
    }
    this._popupSystem.PopupPredicted(message, ent.Owner, args.Origin);
  }

  private void OnInteractHand(Entity<DamagePopupComponent> ent, ref InteractHandEvent args)
  {
    if (!ent.Comp.AllowTypeChange)
      return;
    DamagePopupType damagePopupType = (DamagePopupType) ((uint) (ent.Comp.Type + (byte) 1) % (uint) Enum.GetValues<DamagePopupType>().Length);
    ent.Comp.Type = damagePopupType;
    this.Dirty<DamagePopupComponent>(ent, (MetaDataComponent) null);
    this._popupSystem.PopupPredicted(this.Loc.GetString("damage-popup-component-switched", ("setting", (object) ent.Comp.Type)), ent.Owner, new EntityUid?(args.User));
  }
}
