// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Item.ItemToggle.RMCItemToggleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Item.ItemToggle;

public sealed class RMCItemToggleSystem : EntitySystem
{
  [Dependency]
  private ClothingSystem _clothing;
  [Dependency]
  private SharedItemSystem _item;
  private Robust.Shared.GameObjects.EntityQuery<ItemToggleComponent> _query;

  public override void Initialize()
  {
    base.Initialize();
    this._query = this.GetEntityQuery<ItemToggleComponent>();
    this.SubscribeLocalEvent<RMCItemToggleClothingVisualsComponent, ItemToggledEvent>(new EntityEventRefHandler<RMCItemToggleClothingVisualsComponent, ItemToggledEvent>(this.OnToggled));
  }

  private void OnToggled(
    Entity<RMCItemToggleClothingVisualsComponent> ent,
    ref ItemToggledEvent args)
  {
    string prefix = args.Activated ? ent.Comp.Prefix : (string) null;
    this._item.SetHeldPrefix((EntityUid) ent, prefix);
    this._clothing.SetEquippedPrefix((EntityUid) ent, prefix);
  }
}
