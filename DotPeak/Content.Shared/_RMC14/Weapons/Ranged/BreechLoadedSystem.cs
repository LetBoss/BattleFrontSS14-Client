// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.BreechLoadedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Common;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class BreechLoadedSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearanceSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private TagSystem _tagSystem;
  [Dependency]
  private UseDelaySystem _useDelay;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<BreechLoadedComponent, AttemptShootEvent>(new EntityEventRefHandler<BreechLoadedComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<BreechLoadedComponent, GunShotEvent>(new EntityEventRefHandler<BreechLoadedComponent, GunShotEvent>(this.OnGunShot));
    this.SubscribeLocalEvent<BreechLoadedComponent, RMCTryAmmoEjectEvent>(new EntityEventRefHandler<BreechLoadedComponent, RMCTryAmmoEjectEvent>(this.OnTryAmmoEject));
    this.SubscribeLocalEvent<BreechLoadedComponent, UniqueActionEvent>(new EntityEventRefHandler<BreechLoadedComponent, UniqueActionEvent>(this.OnUniqueAction));
    this.SubscribeLocalEvent<BreechLoadedComponent, InteractUsingEvent>(new EntityEventRefHandler<BreechLoadedComponent, InteractUsingEvent>(this.OnInteractUsing), new Type[1]
    {
      typeof (SharedGunSystem)
    });
  }

  private void OnAttemptShoot(Entity<BreechLoadedComponent> gun, ref AttemptShootEvent args)
  {
    if (args.Cancelled || !gun.Comp.Open && (!gun.Comp.NeedOpenClose || gun.Comp.Ready))
      return;
    args.Cancelled = true;
    if (gun.Comp.Open)
      this._popupSystem.PopupClient(this.Loc.GetString("rmc-breech-loaded-open-shoot-attempt"), args.User, new EntityUid?(args.User));
    else
      this._popupSystem.PopupClient(this.Loc.GetString("rmc-breech-loaded-not-ready-to-shoot"), args.User, new EntityUid?(args.User));
  }

  private void OnGunShot(Entity<BreechLoadedComponent> gun, ref GunShotEvent args)
  {
    if (!gun.Comp.NeedOpenClose)
      return;
    gun.Comp.Ready = false;
    this.Dirty<BreechLoadedComponent>(gun);
  }

  private void OnTryAmmoEject(Entity<BreechLoadedComponent> gun, ref RMCTryAmmoEjectEvent args)
  {
    if (gun.Comp.Open)
      return;
    this._popupSystem.PopupClient(this.Loc.GetString("rmc-breech-loaded-closed-extract-attempt"), args.User, new EntityUid?(args.User));
    args.Cancelled = true;
  }

  private void OnUniqueAction(Entity<BreechLoadedComponent> gun, ref UniqueActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    UseDelayComponent comp1;
    if (this.TryComp<UseDelayComponent>((EntityUid) gun, out comp1) && this._useDelay.IsDelayed((Entity<UseDelayComponent>) ((EntityUid) gun, comp1), gun.Comp.DelayId))
    {
      this._popupSystem.PopupClient(this.Loc.GetString("rmc-breech-loaded-toggle-attempt-cooldown", ("action", gun.Comp.Open ? (object) this.Loc.GetString("rmc-breech-loaded-close") : (object) this.Loc.GetString("rmc-breech-loaded-open"))), args.UserUid, new EntityUid?(args.UserUid));
    }
    else
    {
      gun.Comp.Open = !gun.Comp.Open;
      if (!gun.Comp.Open)
        gun.Comp.Ready = true;
      AppearanceComponent comp2;
      if (gun.Comp.ShowBreechOpen && this.TryComp<AppearanceComponent>(gun.Owner, out comp2))
        this._appearanceSystem.SetData((EntityUid) gun, (Enum) BreechVisuals.Open, (object) gun.Comp.Open, comp2);
      if (comp1 != null)
      {
        this._useDelay.SetLength((Entity<UseDelayComponent>) ((EntityUid) gun, comp1), gun.Comp.ToggleDelay, gun.Comp.DelayId);
        this._useDelay.TryResetDelay((Entity<UseDelayComponent>) ((EntityUid) gun, comp1), id: gun.Comp.DelayId);
      }
      this.Dirty<BreechLoadedComponent>(gun);
      this._audioSystem.PlayPredicted(gun.Comp.Open ? gun.Comp.OpenSound : gun.Comp.CloseSound, (EntityUid) gun, new EntityUid?(args.UserUid));
    }
  }

  private void OnInteractUsing(Entity<BreechLoadedComponent> gun, ref InteractUsingEvent args)
  {
    BallisticAmmoProviderComponent comp;
    if (gun.Comp.Open || !this.TryComp<BallisticAmmoProviderComponent>(gun.Owner, out comp) || comp.Whitelist == null || comp.Whitelist.Tags == null || !this._tagSystem.HasAnyTag(args.Used, comp.Whitelist.Tags))
      return;
    this._popupSystem.PopupClient(this.Loc.GetString("rmc-breech-loaded-closed-load-attempt"), args.User, new EntityUid?(args.User));
    args.Handled = true;
  }
}
