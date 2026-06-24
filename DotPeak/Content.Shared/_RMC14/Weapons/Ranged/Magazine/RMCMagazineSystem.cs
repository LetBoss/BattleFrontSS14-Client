// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Magazine.RMCMagazineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Magazine;

public sealed class RMCMagazineSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCMagazineVisualsComponent, MapInitEvent>(new EntityEventRefHandler<RMCMagazineVisualsComponent, MapInitEvent>(this.OnMagazineInit), after: new Type[1]
    {
      typeof (SharedGunSystem)
    });
    this.SubscribeLocalEvent<RMCMagazineVisualsComponent, GunShotEvent>(new EntityEventRefHandler<RMCMagazineVisualsComponent, GunShotEvent>(this.OnMagazineGunShot));
    this.SubscribeLocalEvent<RMCMagazineVisualsComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<RMCMagazineVisualsComponent, EntInsertedIntoContainerMessage>(this.OnMagazineSlotInserted));
    this.SubscribeLocalEvent<RMCMagazineVisualsComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<RMCMagazineVisualsComponent, EntRemovedFromContainerMessage>(this.OnMagazineSlotRemoved));
  }

  private void OnMagazineInit(Entity<RMCMagazineVisualsComponent> ent, ref MapInitEvent args)
  {
    this.UpdateMagazine((EntityUid) ent);
  }

  private void OnMagazineGunShot(Entity<RMCMagazineVisualsComponent> ent, ref GunShotEvent args)
  {
    this.UpdateMagazine((EntityUid) ent);
  }

  private void OnMagazineSlotInserted(
    Entity<RMCMagazineVisualsComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    this.UpdateMagazine((EntityUid) ent);
  }

  private void OnMagazineSlotRemoved(
    Entity<RMCMagazineVisualsComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    this.UpdateMagazine((EntityUid) ent);
  }

  public void UpdateMagazine(EntityUid uid)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    GetAmmoCountEvent args = new GetAmmoCountEvent();
    this.RaiseLocalEvent<GetAmmoCountEvent>(uid, ref args);
    this._appearance.SetData(uid, (Enum) RMCMagazineVisuals.SlideOpen, (object) (args.Count <= 0), comp);
  }
}
