// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Webbing.WebbingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Clothing;
using Content.Client.Items.Systems;
using Content.Shared._RMC14.Webbing;
using Content.Shared.Clothing;
using Content.Shared.Inventory.Events;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Webbing;

public sealed class WebbingSystem : SharedWebbingSystem
{
  [Dependency]
  private ItemSystem _item;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SpriteSystem _sprite;

  public event Action? PlayerWebbingUpdated;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WebbingClothingComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<WebbingClothingComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnClothingState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WebbingClothingComponent, GetEquipmentVisualsEvent>(new EntityEventRefHandler<WebbingClothingComponent, GetEquipmentVisualsEvent>((object) this, __methodptr(OnWebbingClothingEquipmentVisuals)), (Type[]) null, new Type[1]
    {
      typeof (ClientClothingSystem)
    });
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WebbingClothingComponent, GotEquippedEvent>(new EntityEventRefHandler<WebbingClothingComponent, GotEquippedEvent>((object) this, __methodptr(OnClothingEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WebbingClothingComponent, GotUnequippedEvent>(new EntityEventRefHandler<WebbingClothingComponent, GotUnequippedEvent>((object) this, __methodptr(OnClothingUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WebbingTransferComponent, ComponentRemove>(new EntityEventRefHandler<WebbingTransferComponent, ComponentRemove>((object) this, __methodptr(OnWebbingTransferRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnWebbingClothingEquipmentVisuals(
    Entity<WebbingClothingComponent> ent,
    ref GetEquipmentVisualsEvent args)
  {
    WebbingComponent webbingComponent1;
    if (!this.TryComp<WebbingComponent>(ent.Comp.Webbing, ref webbingComponent1))
      return;
    SpriteComponent spriteComponent1;
    if (webbingComponent1.PlayerSprite == null && this.TryComp<SpriteComponent>(ent.Comp.Webbing, ref spriteComponent1))
    {
      WebbingComponent webbingComponent2 = webbingComponent1;
      RSI baseRsi = spriteComponent1.BaseRSI;
      SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(baseRsi != null ? baseRsi.Path : new ResPath("_RMC14/Objects/Clothing/Webbing/webbing.rsi"), "equipped");
      webbingComponent2.PlayerSprite = rsi;
    }
    SpriteSpecifier.Rsi playerSprite = webbingComponent1.PlayerSprite;
    if (playerSprite == null)
      return;
    SpriteComponent spriteComponent2;
    int num;
    if (this.TryComp<SpriteComponent>(Entity<WebbingClothingComponent>.op_Implicit(ent), ref spriteComponent2) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)), (Enum) WebbingVisualLayers.Base, ref num, false))
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)), num, true);
      this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)), num, playerSprite.RsiPath, new RSI.StateId?());
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)), num, RSI.StateId.op_Implicit(playerSprite.RsiState));
    }
    args.Layers.Add(("enum.WebbingVisualLayers.Base", new PrototypeLayerData()
    {
      RsiPath = playerSprite.RsiPath.CanonPath,
      State = playerSprite.RsiState
    }));
  }

  private void OnClothingState(
    Entity<WebbingClothingComponent> clothing,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (this.TryComp<SpriteComponent>(Entity<WebbingClothingComponent>.op_Implicit(clothing), ref spriteComponent) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((clothing.Owner, spriteComponent)), (Enum) WebbingVisualLayers.Base, ref num, false))
    {
      WebbingComponent webbingComponent;
      if (this.TryComp<WebbingComponent>(clothing.Comp.Webbing, ref webbingComponent))
      {
        SpriteSpecifier.Rsi playerSprite = webbingComponent.PlayerSprite;
        if (playerSprite != null)
        {
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((clothing.Owner, spriteComponent)), num, true);
          this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((clothing.Owner, spriteComponent)), num, playerSprite.RsiPath, new RSI.StateId?());
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((clothing.Owner, spriteComponent)), num, RSI.StateId.op_Implicit(playerSprite.RsiState));
          goto label_5;
        }
      }
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((clothing.Owner, spriteComponent)), num, false);
    }
label_5:
    this._item.VisualsChanged(Entity<WebbingClothingComponent>.op_Implicit(clothing));
    Action playerWebbingUpdated = this.PlayerWebbingUpdated;
    if (playerWebbingUpdated == null)
      return;
    playerWebbingUpdated();
  }

  private void OnClothingEquipped(
    Entity<WebbingClothingComponent> clothing,
    ref GotEquippedEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid equipee = args.Equipee;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), equipee) ? 1 : 0) : 0) == 0)
      return;
    Action playerWebbingUpdated = this.PlayerWebbingUpdated;
    if (playerWebbingUpdated == null)
      return;
    playerWebbingUpdated();
  }

  private void OnClothingUnequipped(
    Entity<WebbingClothingComponent> clothing,
    ref GotUnequippedEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid equipee = args.Equipee;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), equipee) ? 1 : 0) : 0) == 0)
      return;
    Action playerWebbingUpdated = this.PlayerWebbingUpdated;
    if (playerWebbingUpdated == null)
      return;
    playerWebbingUpdated();
  }

  protected override void OnClothingInserted(
    Entity<WebbingClothingComponent> clothing,
    ref EntInsertedIntoContainerMessage args)
  {
    base.OnClothingInserted(clothing, ref args);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid owner = ((ContainerModifiedMessage) args).Container.Owner;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), owner) ? 1 : 0) : 0) == 0)
      return;
    Action playerWebbingUpdated = this.PlayerWebbingUpdated;
    if (playerWebbingUpdated == null)
      return;
    playerWebbingUpdated();
  }

  protected override void OnClothingRemoved(
    Entity<WebbingClothingComponent> clothing,
    ref EntRemovedFromContainerMessage args)
  {
    base.OnClothingRemoved(clothing, ref args);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid owner = ((ContainerModifiedMessage) args).Container.Owner;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), owner) ? 1 : 0) : 0) == 0)
      return;
    Action playerWebbingUpdated = this.PlayerWebbingUpdated;
    if (playerWebbingUpdated == null)
      return;
    playerWebbingUpdated();
  }

  private void OnWebbingTransferRemove(
    Entity<WebbingTransferComponent> ent,
    ref ComponentRemove args)
  {
    Action playerWebbingUpdated = this.PlayerWebbingUpdated;
    if (playerWebbingUpdated == null)
      return;
    playerWebbingUpdated();
  }
}
