// Decompiled with JetBrains decompiler
// Type: Content.Client.Clothing.ClientClothingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.DisplacementMap;
using Content.Client.Inventory;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.DisplacementMap;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Client.Clothing;

public sealed class ClientClothingSystem : ClothingSystem
{
  public const string Jumpsuit = "jumpsuit";
  private static readonly Dictionary<string, string> TemporarySlotMap = new Dictionary<string, string>()
  {
    {
      "head",
      "HELMET"
    },
    {
      "headArmor",
      "HELMET"
    },
    {
      "eyes",
      "EYES"
    },
    {
      "ears",
      "EARS"
    },
    {
      "mask",
      "MASK"
    },
    {
      "outerClothing",
      "OUTERCLOTHING"
    },
    {
      "outerArmor",
      "OUTERCLOTHING"
    },
    {
      "jumpsuit",
      "INNERCLOTHING"
    },
    {
      "neck",
      "NECK"
    },
    {
      "back",
      "BACKPACK"
    },
    {
      "belt",
      "BELT"
    },
    {
      "gloves",
      "HAND"
    },
    {
      "shoes",
      "FEET"
    },
    {
      "id",
      "IDCARD"
    },
    {
      "pocket1",
      "POCKET1"
    },
    {
      "pocket2",
      "POCKET2"
    },
    {
      "suitstorage",
      "SUITSTORAGE"
    }
  };
  [Dependency]
  private IResourceCache _cache;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private DisplacementMapSystem _displacement;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingComponent, GetEquipmentVisualsEvent>(new ComponentEventHandler<ClothingComponent, GetEquipmentVisualsEvent>((object) this, __methodptr(OnGetVisuals)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingComponent, InventoryTemplateUpdated>(new EntityEventRefHandler<ClothingComponent, InventoryTemplateUpdated>((object) this, __methodptr(OnInventoryTemplateUpdated)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventoryComponent, VisualsChangedEvent>(new ComponentEventHandler<InventoryComponent, VisualsChangedEvent>((object) this, __methodptr(OnVisualsChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SpriteComponent, DidUnequipEvent>(new EntityEventRefHandler<SpriteComponent, DidUnequipEvent>((object) this, __methodptr(OnDidUnequip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventoryComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<InventoryComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceUpdate)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceUpdate(
    EntityUid uid,
    InventoryComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    this.UpdateAllSlots(uid, component);
    int num;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) HumanoidVisualLayers.StencilMask, ref num, false))
      return;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
  }

  private void OnInventoryTemplateUpdated(
    Entity<ClothingComponent> ent,
    ref InventoryTemplateUpdated args)
  {
    this.UpdateAllSlots(ent.Owner, clothing: ent.Comp);
  }

  private void UpdateAllSlots(
    EntityUid uid,
    InventoryComponent? inventoryComponent = null,
    ClothingComponent? clothing = null)
  {
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventorySystem.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit((uid, inventoryComponent)));
    EntityUid equipment;
    SlotDefinition slot;
    while (slotEnumerator.NextItem(out equipment, out slot))
      this.RenderEquipment(uid, equipment, slot.Name, inventoryComponent, clothingComponent: clothing);
  }

  private void OnGetVisuals(EntityUid uid, ClothingComponent item, GetEquipmentVisualsEvent args)
  {
    InventoryComponent inventoryComponent;
    if (!this.TryComp<InventoryComponent>(args.Equipee, ref inventoryComponent))
      return;
    List<PrototypeLayerData> layers = (List<PrototypeLayerData>) null;
    if (inventoryComponent.SpeciesId != null)
      item.ClothingVisuals.TryGetValue($"{args.Slot}-{inventoryComponent.SpeciesId}", out layers);
    if (layers == null && !item.ClothingVisuals.TryGetValue(args.Slot, out layers) && !this.TryGetDefaultVisuals(uid, item, args.Slot, inventoryComponent.SpeciesId, out layers))
      return;
    int num = 0;
    foreach (PrototypeLayerData prototypeLayerData in layers)
    {
      HashSet<string> mapKeys = prototypeLayerData.MapKeys;
      string str = mapKeys != null ? mapKeys.FirstOrDefault<string>() : (string) null;
      if (str == null)
      {
        str = $"{args.Slot}-{num}";
        ++num;
      }
      item.MappedLayer = str;
      args.Layers.Add((str, prototypeLayerData));
    }
  }

  private bool TryGetDefaultVisuals(
    EntityUid uid,
    ClothingComponent clothing,
    string slot,
    string? speciesId,
    [NotNullWhen(true)] out List<PrototypeLayerData>? layers)
  {
    layers = (List<PrototypeLayerData>) null;
    RSI rsi = (RSI) null;
    if (clothing.RsiPath != null)
    {
      rsi = this._cache.GetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, clothing.RsiPath), true).RSI;
    }
    else
    {
      SpriteComponent spriteComponent;
      if (this.TryComp<SpriteComponent>(uid, ref spriteComponent))
        rsi = spriteComponent.BaseRSI;
    }
    if (rsi == null)
      return false;
    string key = slot;
    ClientClothingSystem.TemporarySlotMap.TryGetValue(key, out key);
    string str = "equipped-" + key;
    if (!string.IsNullOrEmpty(clothing.EquippedPrefix))
      str = $"{clothing.EquippedPrefix}-equipped-{key}";
    if (clothing.EquippedState != null)
      str = clothing.EquippedState ?? "";
    RSI.State state;
    if (speciesId != null && rsi.TryGetState(RSI.StateId.op_Implicit($"{str}-{speciesId}"), ref state))
      str = $"{str}-{speciesId}";
    else if (!rsi.TryGetState(RSI.StateId.op_Implicit(str), ref state))
      return false;
    layers = new List<PrototypeLayerData>()
    {
      new PrototypeLayerData()
      {
        RsiPath = rsi.Path.ToString(),
        State = str
      }
    };
    return true;
  }

  private void OnVisualsChanged(
    EntityUid uid,
    InventoryComponent component,
    VisualsChangedEvent args)
  {
    EntityUid entity = this.GetEntity(args.Item);
    ClothingComponent clothingComponent;
    if (!this.TryComp<ClothingComponent>(entity, ref clothingComponent) || clothingComponent.InSlot == null)
      return;
    this.RenderEquipment(uid, entity, clothingComponent.InSlot, component, clothingComponent: clothingComponent);
  }

  private void OnDidUnequip(Entity<SpriteComponent> entity, ref DidUnequipEvent args)
  {
    InventorySlotsComponent inventorySlotsComponent;
    HashSet<string> stringSet;
    if (!this.TryComp<InventorySlotsComponent>(Entity<SpriteComponent>.op_Implicit(entity), ref inventorySlotsComponent) || !inventorySlotsComponent.VisualLayerKeys.TryGetValue(args.Slot, out stringSet))
      return;
    foreach (string str in stringSet)
    {
      try
      {
        this._sprite.RemoveLayer(entity.AsNullable(), str, true);
      }
      catch (Exception ex)
      {
        this.Log.Error($"Error removing layer:\n{ex}");
      }
    }
    stringSet.Clear();
  }

  public void InitClothing(EntityUid uid, InventoryComponent component)
  {
    SpriteComponent sprite;
    if (!this.TryComp<SpriteComponent>(uid, ref sprite))
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventorySystem.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit((uid, component)));
    EntityUid equipment;
    SlotDefinition slot;
    while (slotEnumerator.NextItem(out equipment, out slot))
      this.RenderEquipment(uid, equipment, slot.Name, component, sprite);
  }

  protected override void OnGotEquipped(
    EntityUid uid,
    ClothingComponent component,
    GotEquippedEvent args)
  {
    base.OnGotEquipped(uid, component, args);
    this.RenderEquipment(args.Equipee, uid, args.Slot, clothingComponent: component);
  }

  private void RenderEquipment(
    EntityUid equipee,
    EntityUid equipment,
    string slot,
    InventoryComponent? inventory = null,
    SpriteComponent? sprite = null,
    ClothingComponent? clothingComponent = null,
    InventorySlotsComponent? inventorySlots = null)
  {
    SlotDefinition slotDefinition;
    if (!this.Resolve<InventoryComponent, SpriteComponent, InventorySlotsComponent>(equipee, ref inventory, ref sprite, ref inventorySlots, true) || !this.Resolve<ClothingComponent>(equipment, ref clothingComponent, false) || !this._inventorySystem.TryGetSlot(equipee, slot, out slotDefinition, inventory))
      return;
    HashSet<string> revealedLayers;
    if (inventorySlots.VisualLayerKeys.TryGetValue(slot, out revealedLayers))
    {
      foreach (string str in revealedLayers)
        this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), str, true);
      revealedLayers.Clear();
    }
    else
    {
      revealedLayers = new HashSet<string>();
      inventorySlots.VisualLayerKeys[slot] = revealedLayers;
    }
    GetEquipmentVisualsEvent equipmentVisualsEvent = new GetEquipmentVisualsEvent(equipee, slot);
    this.RaiseLocalEvent<GetEquipmentVisualsEvent>(equipment, equipmentVisualsEvent, false);
    if (equipmentVisualsEvent.Layers.Count == 0)
    {
      this.RaiseLocalEvent<EquipmentVisualsUpdatedEvent>(equipment, new EquipmentVisualsUpdatedEvent(equipee, slot, revealedLayers), true);
    }
    else
    {
      int index;
      bool flag = this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), slot, ref index, false);
      DisplacementData valueOrDefault = inventory.Displacements.GetValueOrDefault<string, DisplacementData>(slot);
      Sex? sex = this.CompOrNull<HumanoidAppearanceComponent>(equipee)?.Sex;
      if (sex.HasValue && sex.HasValue)
      {
        switch (sex.GetValueOrDefault())
        {
          case Sex.Male:
            if (inventory.MaleDisplacements.Count > 0)
            {
              valueOrDefault = inventory.MaleDisplacements.GetValueOrDefault<string, DisplacementData>(slot);
              break;
            }
            break;
          case Sex.Female:
            if (inventory.FemaleDisplacements.Count > 0)
            {
              valueOrDefault = inventory.FemaleDisplacements.GetValueOrDefault<string, DisplacementData>(slot);
              break;
            }
            break;
        }
      }
      foreach ((string key, PrototypeLayerData prototypeLayerData) in equipmentVisualsEvent.Layers)
      {
        if (!revealedLayers.Add(key))
        {
          this.Log.Warning($"Duplicate key for clothing visuals: {key}. Are multiple components attempting to modify the same layer? Equipment: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(equipment))}");
        }
        else
        {
          if (flag)
          {
            ++index;
            this._sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), new int?(index));
            this._sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), key);
            this._sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), key, index);
            if (prototypeLayerData.Color.HasValue)
              this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), key, prototypeLayerData.Color.Value);
            if (prototypeLayerData.Scale.HasValue)
              this._sprite.LayerSetScale(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), key, prototypeLayerData.Scale.Value);
          }
          else
            index = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), key);
          if (sprite[index] is SpriteComponent.Layer layer)
          {
            SpriteComponent spriteComponent;
            if (prototypeLayerData.RsiPath == null && prototypeLayerData.TexturePath == null && layer.RSI == null && this.TryComp<SpriteComponent>(equipment, ref spriteComponent))
              this._sprite.LayerSetRsi(layer, spriteComponent.BaseRSI, new RSI.StateId?());
            this._sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((equipee, sprite)), index, prototypeLayerData);
            this._sprite.LayerSetOffset(layer, layer.Offset + slotDefinition.Offset);
            string displacementKey;
            if (valueOrDefault != null && (prototypeLayerData.State == null || inventory.SpeciesId == null || !prototypeLayerData.State.EndsWith(inventory.SpeciesId)) && this._displacement.TryAddDisplacement(valueOrDefault, Entity<SpriteComponent>.op_Implicit((equipee, sprite)), index, (object) key, out displacementKey))
            {
              revealedLayers.Add(displacementKey);
              ++index;
            }
          }
        }
      }
      this.RaiseLocalEvent<EquipmentVisualsUpdatedEvent>(equipment, new EquipmentVisualsUpdatedEvent(equipee, slot, revealedLayers), true);
    }
  }
}
