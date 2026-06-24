// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Armor.RMCBulkyArmorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Marines.Armor;

public sealed class RMCBulkyArmorSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCBulkyArmorComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<RMCBulkyArmorComponent, BeingEquippedAttemptEvent>(this.OnBeingEquippedAttempt));
  }

  private void OnBeingEquippedAttempt(
    Entity<RMCBulkyArmorComponent> armor,
    ref BeingEquippedAttemptEvent args)
  {
    if (!armor.Comp.IsBulky || !this.HasComp<RMCUserBulkyArmorIncapableComponent>(args.EquipTarget))
      return;
    if (args.EquipTarget == args.Equipee)
      this._popup.PopupClient(this.Loc.GetString("rmc-bulky-armor-user-unable", (nameof (armor), (object) armor)), args.Equipee, new EntityUid?(args.Equipee), PopupType.MediumCaution);
    else
      this._popup.PopupEntity(this.Loc.GetString("rmc-bulky-armor-target-unable", ("target", (object) args.EquipTarget), (nameof (armor), (object) armor)), args.Equipee, args.Equipee, PopupType.MediumCaution);
    args.Cancel();
  }
}
