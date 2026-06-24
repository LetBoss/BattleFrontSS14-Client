// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Inventory.CMInventorySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Inventory;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Inventory;

public sealed class CMInventorySystem : SharedCMInventorySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CMItemSlotsComponent, AppearanceChangeEvent>(new EntityEventRefHandler<CMItemSlotsComponent, AppearanceChangeEvent>((object) this, __methodptr(OnItemSlotsAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnItemSlotsAppearanceChange(
    Entity<CMItemSlotsComponent> ent,
    ref AppearanceChangeEvent args)
  {
    this.ContentsUpdated(ent);
  }

  protected override void ContentsUpdated(Entity<CMItemSlotsComponent> ent)
  {
    base.ContentsUpdated(ent);
    SpriteComponent spriteComponent;
    int num;
    if (!this.TryComp<SpriteComponent>(Entity<CMItemSlotsComponent>.op_Implicit(ent), ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), (Enum) CMItemSlotsLayers.Fill, ref num, false))
      return;
    ItemSlotsComponent itemSlotsComponent;
    if (!this.TryComp<ItemSlotsComponent>(Entity<CMItemSlotsComponent>.op_Implicit(ent), ref itemSlotsComponent))
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), num, false);
    }
    else
    {
      foreach ((string _, ItemSlot itemSlot) in itemSlotsComponent.Slots)
      {
        EntityUid? containedEntity = (EntityUid?) itemSlot.ContainerSlot?.ContainedEntity;
        if (containedEntity.HasValue && !this.TerminatingOrDeleted(containedEntity.GetValueOrDefault(), (MetaDataComponent) null))
        {
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), num, true);
          return;
        }
      }
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), num, false);
    }
  }
}
