// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.UniformAccessories.UniformAccessorySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Humanoid;
using Content.Shared._RMC14.UniformAccessories;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Item;
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
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.UniformAccessories;

public sealed class UniformAccessorySystem : SharedUniformAccessorySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private RMCHumanoidAppearanceSystem _rmcHumanoid;
  [Dependency]
  private SpriteSystem _sprite;

  public event Action? PlayerMedalUpdated;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UniformAccessoryHolderComponent, GetEquipmentVisualsEvent>(new EntityEventRefHandler<UniformAccessoryHolderComponent, GetEquipmentVisualsEvent>((object) this, __methodptr(OnHolderGetEquipmentVisuals)), (Type[]) null, new Type[1]
    {
      typeof (ClothingSystem)
    });
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UniformAccessoryHolderComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<UniformAccessoryHolderComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHolderAfterState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UniformAccessoryHolderComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<UniformAccessoryHolderComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnHolderInsertedContainer)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UniformAccessoryHolderComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<UniformAccessoryHolderComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnHolderRemovedContainer)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UniformAccessoryHolderComponent, EquipmentVisualsUpdatedEvent>(new EntityEventRefHandler<UniformAccessoryHolderComponent, EquipmentVisualsUpdatedEvent>((object) this, __methodptr(OnHolderVisualsUpdated)), (Type[]) null, new Type[1]
    {
      typeof (ClothingSystem)
    });
  }

  private void OnHolderGetEquipmentVisuals(
    Entity<UniformAccessoryHolderComponent> ent,
    ref GetEquipmentVisualsEvent args)
  {
    if (this._rmcHumanoid.HidePlayerIdentities && this.HasComp<XenoComponent>(((ISharedPlayerManager) this._player).LocalEntity))
      return;
    SpriteComponent spriteComponent1 = this.CompOrNull<SpriteComponent>(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
    BaseContainer baseContainer;
    if (!this._container.TryGetContainer(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref baseContainer, (ContainerManagerComponent) null))
      return;
    int index = 0;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
    {
      UniformAccessoryComponent component;
      if (this.TryComp<UniformAccessoryComponent>(containedEntity, ref component))
      {
        string layer = this.GetKey(containedEntity, component, index);
        SpriteComponent spriteComponent2;
        if (component.PlayerSprite == null && this.TryComp<SpriteComponent>(containedEntity, ref spriteComponent2))
        {
          UniformAccessoryComponent accessoryComponent = component;
          RSI baseRsi = spriteComponent2.BaseRSI;
          SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(baseRsi != null ? baseRsi.Path : new ResPath("_RMC14/Objects/Medals/bronze.rsi"), "equipped");
          accessoryComponent.PlayerSprite = rsi;
        }
        SpriteSpecifier.Rsi playerSprite = component.PlayerSprite;
        if (playerSprite != null && (!ent.Comp.HideAccessories || !component.HiddenByJacketRolling))
        {
          if (spriteComponent1 != null && component.HasIconSprite)
          {
            int num = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent1)), layer);
            this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent1)), num, !component.Hidden);
            this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent1)), num, playerSprite.RsiPath, new RSI.StateId?());
            this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent1)), num, RSI.StateId.op_Implicit(playerSprite.RsiState));
          }
          if (!args.Layers.Any<(string, PrototypeLayerData)>((Func<(string, PrototypeLayerData), bool>) (t => t.Item1 == layer)))
          {
            args.Layers.Add((layer, new PrototypeLayerData()
            {
              RsiPath = playerSprite.RsiPath.ToString(),
              State = playerSprite.RsiState,
              Visible = new bool?(!component.Hidden)
            }));
            ++index;
          }
        }
      }
    }
    Action playerMedalUpdated = this.PlayerMedalUpdated;
    if (playerMedalUpdated == null)
      return;
    playerMedalUpdated();
  }

  private void OnHolderAfterState(
    Entity<UniformAccessoryHolderComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this._item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
  }

  private void OnHolderInsertedContainer(
    Entity<UniformAccessoryHolderComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    this._item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
  }

  private void OnHolderRemovedContainer(
    Entity<UniformAccessoryHolderComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    EntityUid entity = ((ContainerModifiedMessage) args).Entity;
    UniformAccessoryComponent component;
    if (!this.TryComp<UniformAccessoryComponent>(entity, ref component))
      return;
    int index = 0;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) ((ContainerModifiedMessage) args).Container.ContainedEntities)
    {
      if (!EntityUid.op_Equality(containedEntity, entity))
        ++index;
      else
        break;
    }
    string key = this.GetKey(entity, component, index);
    SpriteComponent spriteComponent;
    int num;
    if (this.TryComp<SpriteComponent>(ent.Owner, ref spriteComponent) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), key, ref num, false))
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), num, false);
    this._item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
  }

  private void OnHolderVisualsUpdated(
    Entity<UniformAccessoryHolderComponent> ent,
    ref EquipmentVisualsUpdatedEvent args)
  {
    BaseContainer baseContainer;
    if (this._rmcHumanoid.HidePlayerIdentities && this.HasComp<XenoComponent>(((ISharedPlayerManager) this._player).LocalEntity) || !this._container.TryGetContainer(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref baseContainer, (ContainerManagerComponent) null))
      return;
    string str = string.Empty;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
    {
      UniformAccessoryComponent accessoryComponent1;
      if (!this.TryComp<UniformAccessoryComponent>(containedEntity, ref accessoryComponent1))
        return;
      SpriteComponent spriteComponent;
      if (accessoryComponent1.PlayerSprite == null && this.TryComp<SpriteComponent>(containedEntity, ref spriteComponent))
      {
        UniformAccessoryComponent accessoryComponent2 = accessoryComponent1;
        RSI baseRsi = spriteComponent.BaseRSI;
        SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(baseRsi != null ? baseRsi.Path : new ResPath("_RMC14/Objects/Medals/bronze.rsi"), "equipped");
        accessoryComponent2.PlayerSprite = rsi;
      }
      if (accessoryComponent1.LayerKey != null)
        str = accessoryComponent1.LayerKey;
    }
    SpriteComponent spriteComponent1;
    int num;
    SpriteComponent.Layer layer;
    if (str == string.Empty || !args.RevealedLayers.Contains(str) || !this.TryComp<SpriteComponent>(args.Equipee, ref spriteComponent1) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((args.Equipee, spriteComponent1)), str, ref num, false) || !this._sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((args.Equipee, spriteComponent1)), num, ref layer, false))
      return;
    PrototypeLayerData prototypeData = layer.ToPrototypeData();
    this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((args.Equipee, spriteComponent1)), num, true);
    num = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((args.Equipee, spriteComponent1)), str);
    this._sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((args.Equipee, spriteComponent1)), num, prototypeData);
  }

  private string GetKey(EntityUid uid, UniformAccessoryComponent component, int index)
  {
    string key = $"enum.{"UniformAccessoryLayer"}.{UniformAccessoryLayer.Base}{index}_{this.Name(uid, (MetaDataComponent) null)}_{uid.Id}";
    if (component.LayerKey != null)
      key = component.LayerKey;
    return key;
  }
}
