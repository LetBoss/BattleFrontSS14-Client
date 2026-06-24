// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.SharedPumpActionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Common;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public abstract class SharedPumpActionSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PumpActionComponent, ExaminedEvent>(new EntityEventRefHandler<PumpActionComponent, ExaminedEvent>(this.OnExamined), new Type[1]
    {
      typeof (SharedGunSystem)
    });
    this.SubscribeLocalEvent<PumpActionComponent, AttemptShootEvent>(new EntityEventRefHandler<PumpActionComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<PumpActionComponent, GunShotEvent>(new EntityEventRefHandler<PumpActionComponent, GunShotEvent>(this.OnGunShot));
    this.SubscribeLocalEvent<PumpActionComponent, UniqueActionEvent>(new EntityEventRefHandler<PumpActionComponent, UniqueActionEvent>(this.OnUniqueAction));
    this.SubscribeLocalEvent<PumpActionComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<PumpActionComponent, EntRemovedFromContainerMessage>(this.OnEntRemovedFromContainer));
  }

  protected virtual void OnExamined(Entity<PumpActionComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString((string) ent.Comp.Examine), 1);
  }

  protected virtual void OnAttemptShoot(Entity<PumpActionComponent> ent, ref AttemptShootEvent args)
  {
    GunComponent comp;
    if (args.Cancelled || this.TryComp<GunComponent>(ent.Owner, out comp) && comp.BurstActivated || ent.Comp.Pumped)
      return;
    args.Cancelled = true;
  }

  private void OnGunShot(Entity<PumpActionComponent> ent, ref GunShotEvent args)
  {
    if (ent.Comp.Once)
      return;
    ent.Comp.Pumped = false;
    this.Dirty<PumpActionComponent>(ent);
  }

  private void OnUniqueAction(Entity<PumpActionComponent> ent, ref UniqueActionEvent args)
  {
    if (args.Handled || !this.Pump(ent, args.UserUid))
      return;
    args.Handled = true;
  }

  private void OnEntRemovedFromContainer(
    Entity<PumpActionComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (args.Container.ID != ent.Comp.ContainerId || !ent.Comp.Once)
      return;
    ent.Comp.Pumped = false;
  }

  public bool Pump(Entity<PumpActionComponent> ent, EntityUid user)
  {
    GunComponent comp;
    if (this.TryComp<GunComponent>(ent.Owner, out comp) && comp.BurstActivated)
      return true;
    GetAmmoCountEvent args = new GetAmmoCountEvent();
    this.RaiseLocalEvent<GetAmmoCountEvent>(ent.Owner, ref args);
    if (args.Count <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-gun-no-ammo-message"), user, new EntityUid?(user));
      return true;
    }
    if (!ent.Comp.Running || ent.Comp.Pumped)
      return false;
    ent.Comp.Pumped = true;
    this.Dirty<PumpActionComponent>(ent);
    this._audio.PlayPredicted(ent.Comp.Sound, (EntityUid) ent, new EntityUid?(user));
    return true;
  }
}
