// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Emplacements.WeaponMountSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Emplacements;
using Content.Shared._RMC14.Weapons.Ranged.Overheat;
using Content.Shared.Foldable;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Emplacements;

public sealed class WeaponMountSystem : SharedWeaponMountSystem
{
  [Dependency]
  private SpriteSystem _sprite;
  private const string FoldedLayer = "foldedLayer";

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WeaponMountComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<WeaponMountComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WeaponMountComponent, AppearanceChangeEvent>(new EntityEventRefHandler<WeaponMountComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(Entity<WeaponMountComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    this.UpdateVisuals(ent);
  }

  private void OnAppearanceChange(Entity<WeaponMountComponent> ent, ref AppearanceChangeEvent args)
  {
    this.UpdateVisuals(ent);
  }

  private void UpdateVisuals(Entity<WeaponMountComponent> mount)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<WeaponMountComponent>.op_Implicit(mount), ref spriteComponent))
      return;
    FoldableComponent foldableComponent;
    this.TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(mount), ref foldableComponent);
    (Entity<WeaponMountComponent>, SpriteComponent) valueTuple1 = (mount, spriteComponent);
    SpriteSystem sprite1 = this._sprite;
    (Entity<WeaponMountComponent>, SpriteComponent) valueTuple2 = valueTuple1;
    Entity<SpriteComponent> entity1 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple2.Item1), valueTuple2.Item2));
    // ISSUE: variable of a boxed type
    __Boxed<WeaponMountComponentVisualLayers> local1 = (Enum) WeaponMountComponentVisualLayers.Mounted;
    int num1;
    ref int local2 = ref num1;
    if (sprite1.LayerMapTryGet(entity1, (Enum) local1, ref local2, false))
    {
      bool flag = mount.Comp.MountedEntity.HasValue;
      if (foldableComponent != null)
        flag = flag && !foldableComponent.IsFolded && !mount.Comp.Broken;
      SpriteSystem sprite2 = this._sprite;
      (Entity<WeaponMountComponent>, SpriteComponent) valueTuple3 = valueTuple1;
      Entity<SpriteComponent> entity2 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple3.Item1), valueTuple3.Item2));
      // ISSUE: variable of a boxed type
      __Boxed<WeaponMountComponentVisualLayers> local3 = (Enum) WeaponMountComponentVisualLayers.Overheated;
      int num2;
      ref int local4 = ref num2;
      OverheatComponent overheatComponent;
      if (sprite2.LayerMapTryGet(entity2, (Enum) local3, ref local4, false) && this.TryComp<OverheatComponent>(mount.Comp.MountedEntity, ref overheatComponent))
      {
        SpriteSystem sprite3 = this._sprite;
        (Entity<WeaponMountComponent>, SpriteComponent) valueTuple4 = valueTuple1;
        Entity<SpriteComponent> entity3 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple4.Item1), valueTuple4.Item2));
        int num3 = num2;
        int num4 = flag ? 1 : 0;
        sprite3.LayerSetVisible(entity3, num3, num4 != 0);
        float num5 = Math.Clamp(overheatComponent.Heat / (float) overheatComponent.MaxHeat, 0.0f, 1f);
        SpriteSystem sprite4 = this._sprite;
        (Entity<WeaponMountComponent>, SpriteComponent) valueTuple5 = valueTuple1;
        Entity<SpriteComponent> entity4 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple5.Item1), valueTuple5.Item2));
        int num6 = num2;
        Color color1 = spriteComponent.Color;
        Color color2 = ((Color) ref color1).WithAlpha(num5);
        sprite4.LayerSetColor(entity4, num6, color2);
      }
      if (foldableComponent != null && foldableComponent.IsFolded)
      {
        SpriteSystem sprite5 = this._sprite;
        (Entity<WeaponMountComponent>, SpriteComponent) valueTuple6 = valueTuple1;
        Entity<SpriteComponent> entity5 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple6.Item1), valueTuple6.Item2));
        // ISSUE: variable of a boxed type
        __Boxed<WeaponMountComponentVisualLayers> local5 = (Enum) WeaponMountComponentVisualLayers.MountedAmmo;
        sprite5.LayerSetVisible(entity5, (Enum) local5, false);
      }
      if (foldableComponent == null || !foldableComponent.IsFolded)
      {
        Entity<WeaponMountComponent> mount1 = mount;
        (Entity<WeaponMountComponent>, SpriteComponent) valueTuple7 = valueTuple1;
        Entity<SpriteComponent> sprite6 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple7.Item1), valueTuple7.Item2));
        // ISSUE: variable of a boxed type
        __Boxed<WeaponMountComponentVisualLayers> mapKey = (Enum) WeaponMountComponentVisualLayers.MountedAmmo;
        this.UpdateAmmoVisual(mount1, sprite6, (Enum) mapKey);
      }
      SpriteSystem sprite7 = this._sprite;
      (Entity<WeaponMountComponent>, SpriteComponent) valueTuple8 = valueTuple1;
      Entity<SpriteComponent> entity6 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple8.Item1), valueTuple8.Item2));
      int num7 = num1;
      int num8 = flag ? 1 : 0;
      sprite7.LayerSetVisible(entity6, num7, num8 != 0);
      SpriteSystem sprite8 = this._sprite;
      (Entity<WeaponMountComponent>, SpriteComponent) valueTuple9 = valueTuple1;
      Entity<SpriteComponent> entity7 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple9.Item1), valueTuple9.Item2));
      // ISSUE: variable of a boxed type
      __Boxed<WeaponMountComponentVisualLayers> local6 = (Enum) WeaponMountComponentVisualLayers.Broken;
      int num9;
      ref int local7 = ref num9;
      if (sprite8.LayerMapTryGet(entity7, (Enum) local6, ref local7, false))
      {
        SpriteSystem sprite9 = this._sprite;
        (Entity<WeaponMountComponent>, SpriteComponent) valueTuple10 = valueTuple1;
        Entity<SpriteComponent> entity8 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple10.Item1), valueTuple10.Item2));
        int num10 = num9;
        int num11 = mount.Comp.Broken ? 1 : 0;
        sprite9.LayerSetVisible(entity8, num10, num11 != 0);
      }
    }
    SpriteSystem sprite10 = this._sprite;
    (Entity<WeaponMountComponent>, SpriteComponent) valueTuple11 = valueTuple1;
    Entity<SpriteComponent> entity9 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple11.Item1), valueTuple11.Item2));
    // ISSUE: variable of a boxed type
    __Boxed<WeaponMountComponentVisualLayers> local8 = (Enum) WeaponMountComponentVisualLayers.Folded;
    int num12;
    ref int local9 = ref num12;
    if (sprite10.LayerMapTryGet(entity9, (Enum) local8, ref local9, false) && foldableComponent != null)
    {
      if (foldableComponent.IsFolded)
      {
        Entity<WeaponMountComponent> mount2 = mount;
        (Entity<WeaponMountComponent>, SpriteComponent) valueTuple12 = valueTuple1;
        Entity<SpriteComponent> sprite11 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple12.Item1), valueTuple12.Item2));
        // ISSUE: variable of a boxed type
        __Boxed<WeaponMountComponentVisualLayers> mapKey = (Enum) WeaponMountComponentVisualLayers.FoldedAmmo;
        this.UpdateAmmoVisual(mount2, sprite11, (Enum) mapKey);
      }
      else
      {
        SpriteSystem sprite12 = this._sprite;
        (Entity<WeaponMountComponent>, SpriteComponent) valueTuple13 = valueTuple1;
        Entity<SpriteComponent> entity10 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple13.Item1), valueTuple13.Item2));
        // ISSUE: variable of a boxed type
        __Boxed<WeaponMountComponentVisualLayers> local10 = (Enum) WeaponMountComponentVisualLayers.FoldedAmmo;
        sprite12.LayerSetVisible(entity10, (Enum) local10, false);
      }
      bool flag = foldableComponent.IsFolded && !mount.Comp.Broken;
      SpriteSystem sprite13 = this._sprite;
      (Entity<WeaponMountComponent>, SpriteComponent) valueTuple14 = valueTuple1;
      Entity<SpriteComponent> entity11 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple14.Item1), valueTuple14.Item2));
      int num13 = num12;
      int num14 = !flag ? 0 : (mount.Comp.MountedEntity.HasValue ? 1 : 0);
      sprite13.LayerSetVisible(entity11, num13, num14 != 0);
      SpriteSystem sprite14 = this._sprite;
      (Entity<WeaponMountComponent>, SpriteComponent) valueTuple15 = valueTuple1;
      Entity<SpriteComponent> entity12 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple15.Item1), valueTuple15.Item2));
      int num15;
      ref int local11 = ref num15;
      if (sprite14.LayerMapTryGet(entity12, "foldedLayer", ref local11, false))
      {
        SpriteSystem sprite15 = this._sprite;
        (Entity<WeaponMountComponent>, SpriteComponent) valueTuple16 = valueTuple1;
        Entity<SpriteComponent> entity13 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple16.Item1), valueTuple16.Item2));
        int num16 = num15;
        int num17 = !flag ? 0 : (!mount.Comp.MountedEntity.HasValue ? 1 : 0);
        sprite15.LayerSetVisible(entity13, num16, num17 != 0);
      }
      SpriteSystem sprite16 = this._sprite;
      (Entity<WeaponMountComponent>, SpriteComponent) valueTuple17 = valueTuple1;
      Entity<SpriteComponent> entity14 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple17.Item1), valueTuple17.Item2));
      // ISSUE: variable of a boxed type
      __Boxed<WeaponMountComponentVisualLayers> local12 = (Enum) WeaponMountComponentVisualLayers.Broken;
      int num18;
      ref int local13 = ref num18;
      if (sprite16.LayerMapTryGet(entity14, (Enum) local12, ref local13, false))
      {
        SpriteSystem sprite17 = this._sprite;
        (Entity<WeaponMountComponent>, SpriteComponent) valueTuple18 = valueTuple1;
        Entity<SpriteComponent> entity15 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple18.Item1), valueTuple18.Item2));
        int num19 = num18;
        int num20 = mount.Comp.Broken ? 1 : 0;
        sprite17.LayerSetVisible(entity15, num19, num20 != 0);
      }
    }
    if (!mount.Comp.MountedEntity.HasValue || foldableComponent != null && foldableComponent.IsFolded)
    {
      SpriteSystem sprite18 = this._sprite;
      (Entity<WeaponMountComponent>, SpriteComponent) valueTuple19 = valueTuple1;
      Entity<SpriteComponent> entity16 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple19.Item1), valueTuple19.Item2));
      sprite18.SetDrawDepth(entity16, 4);
    }
    else
    {
      SpriteSystem sprite19 = this._sprite;
      (Entity<WeaponMountComponent>, SpriteComponent) valueTuple20 = valueTuple1;
      Entity<SpriteComponent> entity17 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(valueTuple20.Item1), valueTuple20.Item2));
      sprite19.SetDrawDepth(entity17, 6);
    }
  }

  private void UpdateAmmoVisual(
    Entity<WeaponMountComponent> mount,
    Entity<SpriteComponent?> sprite,
    Enum mapKey)
  {
    bool flag = false;
    int num;
    if (!this._sprite.LayerMapTryGet(sprite, mapKey, ref num, false))
      return;
    if (mount.Comp.MountedEntity.HasValue)
    {
      GetAmmoCountEvent getAmmoCountEvent = new GetAmmoCountEvent();
      this.RaiseLocalEvent<GetAmmoCountEvent>(mount.Comp.MountedEntity.Value, ref getAmmoCountEvent, false);
      if (getAmmoCountEvent.Count > 0)
        flag = true;
    }
    this._sprite.LayerSetVisible(sprite, num, flag);
  }
}
