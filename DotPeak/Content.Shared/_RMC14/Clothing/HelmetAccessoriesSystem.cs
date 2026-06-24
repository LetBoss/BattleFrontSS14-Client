// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Clothing.HelmetAccessoriesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Storage;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Clothing;

public sealed class HelmetAccessoriesSystem : EntitySystem
{
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private ItemToggleSystem _itemToggle;
  private Robust.Shared.GameObjects.EntityQuery<StorageComponent> _storageQuery;
  private Robust.Shared.GameObjects.EntityQuery<HelmetAccessoryComponent> _accessoryQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._storageQuery = this.GetEntityQuery<StorageComponent>();
    this._accessoryQuery = this.GetEntityQuery<HelmetAccessoryComponent>();
    this.SubscribeLocalEvent<HelmetAccessoryHolderComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<HelmetAccessoryHolderComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted));
    this.SubscribeLocalEvent<HelmetAccessoryHolderComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<HelmetAccessoryHolderComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved));
    this.SubscribeLocalEvent<HelmetAccessoryHolderComponent, GetEquipmentVisualsEvent>(new EntityEventRefHandler<HelmetAccessoryHolderComponent, GetEquipmentVisualsEvent>(this.OnGetEquipmentVisuals), after: new Type[1]
    {
      typeof (ClothingSystem)
    });
    this.SubscribeLocalEvent<HelmetAccessoryComponent, ItemToggledEvent>(new EntityEventRefHandler<HelmetAccessoryComponent, ItemToggledEvent>(this.OnToggled));
  }

  private void OnEntInserted(
    Entity<HelmetAccessoryHolderComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    this._item.VisualsChanged((EntityUid) ent);
  }

  private void OnEntRemoved(
    Entity<HelmetAccessoryHolderComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    this._item.VisualsChanged((EntityUid) ent);
  }

  private void OnToggled(Entity<HelmetAccessoryComponent> ent, ref ItemToggledEvent args)
  {
    TransformComponent comp;
    if (!this.TryComp((EntityUid) ent, out comp) || this.TerminatingOrDeleted(comp.ParentUid))
      return;
    this._item.VisualsChanged(comp.ParentUid);
  }

  private void OnGetEquipmentVisuals(
    Entity<HelmetAccessoryHolderComponent> ent,
    ref GetEquipmentVisualsEvent args)
  {
    SlotDefinition slotDefinition;
    StorageComponent component1;
    if (this._inventory.TryGetSlot(args.Equipee, args.Slot, out slotDefinition) && (slotDefinition.SlotFlags & ent.Comp.Slot) == SlotFlags.NONE || !this._storageQuery.TryComp(ent.Owner, out component1) || component1.Container == null)
      return;
    int num = 0;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) component1.Container.ContainedEntities)
    {
      string str = $"enum.{"HelmetAccessoryLayers"}.{HelmetAccessoryLayers.Helmet}{num}_{this.Name(ent.Owner)}";
      HelmetAccessoryComponent component2;
      if (this._accessoryQuery.TryComp(containedEntity, out component2))
      {
        SpriteSpecifier.Rsi rsi = !this._itemToggle.IsActivated((Entity<ItemToggleComponent>) containedEntity) || component2.ToggledRsi == null ? (!ent.Comp.IsHat || component2.HatRsi == null ? component2.Rsi : component2.HatRsi) : (!ent.Comp.IsHat || component2.HatToggledRsi == null ? component2.ToggledRsi : component2.HatToggledRsi);
        args.Layers.Add((str, new PrototypeLayerData()
        {
          RsiPath = rsi.RsiPath.ToString(),
          State = rsi.RsiState,
          Visible = new bool?(true)
        }));
        ++num;
      }
    }
  }
}
