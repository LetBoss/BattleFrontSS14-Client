// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Damage.RMCDamagePopupSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Damage;

public sealed class RMCDamagePopupSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popupSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DamagePopupComponent, DamageDealtEvent>(new EntityEventRefHandler<DamagePopupComponent, DamageDealtEvent>(this.OnDamagePopup));
  }

  private void OnDamagePopup(Entity<DamagePopupComponent> ent, ref DamageDealtEvent args)
  {
    DamageableComponent comp;
    if (!this.TryComp<DamageableComponent>((EntityUid) ent, out comp))
      return;
    this.ShowClientDamagePopup((EntityUid) ent, comp.TotalDamage, ent.Comp.Type, args.Origin, args.DamageDelta);
  }

  private void ShowClientDamagePopup(
    EntityUid target,
    FixedPoint2 damageTotal,
    DamagePopupType type,
    EntityUid? origin,
    DamageSpecifier? damageDelta)
  {
    if (damageDelta == null)
      return;
    FixedPoint2 total = damageDelta.GetTotal();
    string str;
    switch (type)
    {
      case DamagePopupType.Combined:
        str = $"{total.ToString()} | {damageTotal.ToString()}";
        break;
      case DamagePopupType.Total:
        str = damageTotal.ToString();
        break;
      case DamagePopupType.Delta:
        str = total.ToString();
        break;
      case DamagePopupType.Hit:
        str = "!";
        break;
      default:
        str = "Invalid type";
        break;
    }
    string message = str;
    if (!origin.HasValue || !this._net.IsServer)
      return;
    this._popupSystem.PopupEntity(message, target, origin.Value);
  }
}
