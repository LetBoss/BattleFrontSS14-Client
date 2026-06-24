// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fax.Systems.FaxecuteSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Fax.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Fax.Systems;

public sealed class FaxecuteSystem : EntitySystem
{
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedPopupSystem _popupSystem;

  public override void Initialize() => base.Initialize();

  public void Faxecute(EntityUid uid, FaxMachineComponent component, DamageOnFaxecuteEvent? args = null)
  {
    EntityUid? uid1 = component.PaperSlot.Item;
    FaxecuteComponent comp;
    if (!uid1.HasValue || !this.TryComp<FaxecuteComponent>(uid, out comp))
      return;
    DamageSpecifier damage = comp.Damage;
    this._damageable.TryChangeDamage(uid1, damage);
    this._popupSystem.PopupEntity(this.Loc.GetString("fax-machine-popup-error", ("target", (object) uid)), uid, PopupType.LargeCaution);
  }
}
