// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Mimicry.MimicryVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Mimicry;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Maps;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Mimicry;

public sealed class MimicryVisualizerSystem : EntitySystem
{
  private const string SetSlot = "outerClothing";
  [Dependency]
  private readonly IPrototypeManager _proto;
  [Dependency]
  private readonly IResourceCache _resCache;
  [Dependency]
  private readonly SharedItemSystem _item;
  [Dependency]
  private readonly SpriteSystem _sprite;
  [Dependency]
  private readonly InventorySystem _inventory;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingComponent, EquipmentVisualsUpdatedEvent>(new EntityEventRefHandler<ClothingComponent, EquipmentVisualsUpdatedEvent>((object) this, __methodptr(OnVisualsUpdated)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MimicryComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<MimicryComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnState)), (Type[]) null, (Type[]) null);
  }

  private void OnState(Entity<MimicryComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    EntityUid parentUid = this.Transform(Entity<MimicryComponent>.op_Implicit(ent)).ParentUid;
    if (!this.Exists(parentUid))
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(parentUid));
    EntityUid owner;
    while (slotEnumerator.NextItem(out owner))
      this._item.VisualsChanged(owner);
  }

  private void OnVisualsUpdated(
    Entity<ClothingComponent> clothing,
    ref EquipmentVisualsUpdatedEvent args)
  {
    EntityUid? entityUid1;
    MimicryComponent mimicryComponent;
    SpriteComponent spriteComponent;
    if (!this._inventory.TryGetSlotEntity(args.Equipee, "outerClothing", out entityUid1) || !this.TryComp<MimicryComponent>(entityUid1, ref mimicryComponent) || !this.TryComp<SpriteComponent>(args.Equipee, ref spriteComponent))
      return;
    EntityUid? entityUid2;
    int num;
    if (mimicryComponent.HoodDown && this._inventory.TryGetSlotEntity(args.Equipee, mimicryComponent.HoodSlot, out entityUid2))
    {
      EntityUid? nullable = entityUid2;
      EntityUid owner = clothing.Owner;
      if ((nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), owner) ? 1 : 0) : 0) != 0)
      {
        using (HashSet<string>.Enumerator enumerator = args.RevealedLayers.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            string current = enumerator.Current;
            if (spriteComponent.LayerMapTryGet((object) current, ref num, false))
              spriteComponent.LayerSetVisible((object) current, false);
          }
          return;
        }
      }
    }
    ProtoId<ContentTileDefinition>? mimickedTile = mimicryComponent.MimickedTile;
    if (!mimickedTile.HasValue)
      return;
    ProtoId<ContentTileDefinition> valueOrDefault = mimickedTile.GetValueOrDefault();
    ContentTileDefinition contentTileDefinition;
    if (mimicryComponent.ExcludedSlots.Contains(args.Slot) || !this._proto.TryIndex<ContentTileDefinition>(valueOrDefault, ref contentTileDefinition))
      return;
    ResPath? sprite = contentTileDefinition.Sprite;
    TextureResource textureResource;
    if (!sprite.HasValue || !this._resCache.TryGetResource<TextureResource>(sprite.GetValueOrDefault(), ref textureResource))
      return;
    Texture texture = textureResource.Texture;
    Vector4 vector4 = new Vector4(0.0f, 0.0f, 1f / (texture.Height == 0 ? 1f : (float) Math.Max(1, texture.Width / texture.Height)), 1f);
    ShaderInstance shaderInstance = this._proto.Index<ShaderPrototype>("Mimicry").InstanceUnique();
    shaderInstance.SetParameter("tileTex", texture);
    shaderInstance.SetParameter("tileRegion", vector4);
    foreach (string revealedLayer in args.RevealedLayers)
    {
      if (spriteComponent.LayerMapTryGet((object) revealedLayer, ref num, false))
        spriteComponent.LayerSetShader((object) revealedLayer, shaderInstance, (string) null);
    }
  }
}
