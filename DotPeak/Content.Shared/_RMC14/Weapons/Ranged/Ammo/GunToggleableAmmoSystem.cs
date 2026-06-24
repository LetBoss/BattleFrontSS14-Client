// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Ammo.GunToggleableAmmoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Ammo;

public sealed class GunToggleableAmmoSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private CMArmorSystem _cmArmor;
  [Dependency]
  private SharedPopupSystem _popup;
  private Robust.Shared.GameObjects.EntityQuery<ProjectileComponent> _projectileQuery;
  private Robust.Shared.GameObjects.EntityQuery<CMArmorPiercingComponent> _armorPiercingQuery;

  public override void Initialize()
  {
    this._projectileQuery = this.GetEntityQuery<ProjectileComponent>();
    this._armorPiercingQuery = this.GetEntityQuery<CMArmorPiercingComponent>();
    this.SubscribeLocalEvent<GunToggleableAmmoComponent, GetItemActionsEvent>(new EntityEventRefHandler<GunToggleableAmmoComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<GunToggleableAmmoComponent, GunToggleAmmoActionEvent>(new EntityEventRefHandler<GunToggleableAmmoComponent, GunToggleAmmoActionEvent>(this.OnToggleAmmoAction));
    this.SubscribeLocalEvent<GunToggleableAmmoComponent, AmmoShotEvent>(new EntityEventRefHandler<GunToggleableAmmoComponent, AmmoShotEvent>(this.OnAmmoShot));
    this.SubscribeLocalEvent<GunToggleableAmmoComponent, UniqueActionEvent>(new EntityEventRefHandler<GunToggleableAmmoComponent, UniqueActionEvent>(this.OnUniqueAction));
  }

  private void OnGetItemActions(
    Entity<GunToggleableAmmoComponent> ent,
    ref GetItemActionsEvent args)
  {
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
    this.Dirty<GunToggleableAmmoComponent>(ent);
  }

  private void OnToggleAmmoAction(
    Entity<GunToggleableAmmoComponent> ent,
    ref GunToggleAmmoActionEvent args)
  {
    if (!this.ToggleAmmo(ent, args.Performer))
      return;
    args.Handled = true;
  }

  private void OnAmmoShot(Entity<GunToggleableAmmoComponent> ent, ref AmmoShotEvent args)
  {
    int setting = ent.Comp.Setting;
    if (setting < 0 || setting >= ent.Comp.Settings.Count)
      return;
    ref GunToggleableAmmoSetting local = ref CollectionsMarshal.AsSpan<GunToggleableAmmoSetting>(ent.Comp.Settings)[setting];
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      ProjectileComponent component1;
      if (this._projectileQuery.TryComp(firedProjectile, out component1))
      {
        component1.Damage = new DamageSpecifier(local.Damage);
        this.Dirty(firedProjectile, (IComponent) component1);
      }
      CMArmorPiercingComponent component2;
      if (this._armorPiercingQuery.TryComp(firedProjectile, out component2))
        this._cmArmor.SetArmorPiercing((Entity<CMArmorPiercingComponent>) (firedProjectile, component2), local.ArmorPiercing);
    }
  }

  private void OnUniqueAction(Entity<GunToggleableAmmoComponent> ent, ref UniqueActionEvent args)
  {
    if (args.Handled || !this.ToggleAmmo(ent, args.UserUid))
      return;
    args.Handled = true;
  }

  private bool ToggleAmmo(Entity<GunToggleableAmmoComponent> ent, EntityUid user)
  {
    if (ent.Comp.Settings.Count == 0)
      return false;
    ref int local = ref ent.Comp.Setting;
    ++local;
    if (local >= ent.Comp.Settings.Count)
      local = 0;
    GunToggleableAmmoSetting setting = ent.Comp.Settings[local];
    this._popup.PopupClient(this.Loc.GetString("rmc-toggleable-ammo-firing", ("ammo", (object) this.Loc.GetString((string) setting.Name))), user, new EntityUid?(user), PopupType.Large);
    this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(user));
    EntityUid? action = ent.Comp.Action;
    if (action.HasValue)
      this._actions.SetIcon((Entity<ActionComponent>) action.GetValueOrDefault(), (SpriteSpecifier) setting.Icon);
    this.Dirty<GunToggleableAmmoComponent>(ent);
    return true;
  }
}
