// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.CMAmmoBoxSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged;

public sealed class CMAmmoBoxSystem : EntitySystem
{
  [Dependency]
  private AppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CMAmmoBoxComponent, ComponentStartup>(new EntityEventRefHandler<CMAmmoBoxComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CMAmmoBoxComponent, AppearanceChangeEvent>(new EntityEventRefHandler<CMAmmoBoxComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(Entity<CMAmmoBoxComponent> box, ref ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    AppearanceComponent appearanceComponent;
    if (!this.TryComp<SpriteComponent>(Entity<CMAmmoBoxComponent>.op_Implicit(box), ref spriteComponent) || !this.TryComp<AppearanceComponent>(Entity<CMAmmoBoxComponent>.op_Implicit(box), ref appearanceComponent))
      return;
    this.UpdateAppearance(Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent>.op_Implicit((Entity<CMAmmoBoxComponent>.op_Implicit(box), Entity<CMAmmoBoxComponent>.op_Implicit(box), spriteComponent, appearanceComponent)));
  }

  private void OnAppearanceChange(Entity<CMAmmoBoxComponent> box, ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    this.UpdateAppearance(Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent>.op_Implicit((Entity<CMAmmoBoxComponent>.op_Implicit(box), Entity<CMAmmoBoxComponent>.op_Implicit(box), args.Sprite, args.Component)));
  }

  private void UpdateAppearance(
    Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent> box)
  {
    int num;
    ((SharedAppearanceSystem) this._appearance).TryGetData<int>(Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent>.op_Implicit(box), (Enum) AmmoVisuals.AmmoCount, ref num, Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent>.op_Implicit(box));
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((box.Owner, box.Comp2)), box.Comp1.AmmoLayer, num > 0);
  }
}
