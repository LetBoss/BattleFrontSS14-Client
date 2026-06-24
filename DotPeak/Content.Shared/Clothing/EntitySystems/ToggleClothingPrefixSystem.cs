// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.ToggleClothingPrefixSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class ToggleClothingPrefixSystem : EntitySystem
{
  [Dependency]
  private ClothingSystem _clothing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ToggleClothingPrefixComponent, ItemToggledEvent>(new EntityEventRefHandler<ToggleClothingPrefixComponent, ItemToggledEvent>((object) this, __methodptr(OnToggled)), (Type[]) null, (Type[]) null);
  }

  private void OnToggled(Entity<ToggleClothingPrefixComponent> ent, ref ItemToggledEvent args)
  {
    this._clothing.SetEquippedPrefix(Entity<ToggleClothingPrefixComponent>.op_Implicit(ent), args.Activated ? ent.Comp.PrefixOn : ent.Comp.PrefixOff);
  }
}
