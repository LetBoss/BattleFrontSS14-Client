// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.UniformAccessories.SharedUniformAccessorySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.UniformAccessories;

public abstract class SharedUniformAccessorySystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<UniformAccessoryHolderComponent, MapInitEvent>(new EntityEventRefHandler<UniformAccessoryHolderComponent, MapInitEvent>(this.OnHolderMapInit));
    this.SubscribeLocalEvent<UniformAccessoryHolderComponent, InteractUsingEvent>(new EntityEventRefHandler<UniformAccessoryHolderComponent, InteractUsingEvent>(this.OnHolderInteractUsing));
    this.SubscribeLocalEvent<UniformAccessoryHolderComponent, GotEquippedEvent>(new EntityEventRefHandler<UniformAccessoryHolderComponent, GotEquippedEvent>(this.OnHolderGotEquipped));
    this.SubscribeLocalEvent<UniformAccessoryHolderComponent, GetVerbsEvent<EquipmentVerb>>(new EntityEventRefHandler<UniformAccessoryHolderComponent, GetVerbsEvent<EquipmentVerb>>(this.OnHolderGetEquipmentVerbs));
    this.Subs.BuiEvents<UniformAccessoryHolderComponent>((object) UniformAccessoriesUi.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<UniformAccessoryHolderComponent>) (subs => subs.Event<UniformAccessoriesBuiMsg>(new EntityEventRefHandler<UniformAccessoryHolderComponent, UniformAccessoriesBuiMsg>(this.OnAccessoriesBuiMsg))));
  }

  private void OnHolderMapInit(Entity<UniformAccessoryHolderComponent> ent, ref MapInitEvent args)
  {
    this._container.EnsureContainer<Container>((EntityUid) ent, ent.Comp.ContainerId);
    List<EntProtoId> startingAccessories = ent.Comp.StartingAccessories;
    if (startingAccessories == null || this._net.IsClient)
      return;
    foreach (EntProtoId protoName in startingAccessories)
      this.SpawnInContainerOrDrop((string) protoName, ent.Owner, ent.Comp.ContainerId);
  }

  private void OnHolderInteractUsing(
    Entity<UniformAccessoryHolderComponent> ent,
    ref InteractUsingEvent args)
  {
    UniformAccessoryComponent comp1;
    if (!this.TryComp<UniformAccessoryComponent>(args.Used, out comp1))
      return;
    Container container = this._container.EnsureContainer<Container>((EntityUid) ent, ent.Comp.ContainerId);
    args.Handled = true;
    NetEntity? user = comp1.User;
    if (user.HasValue && !this.BelongsToUser(user.GetValueOrDefault(), args.User))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-uniform-accessory-fail"), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
      this._hands.TryDrop((Entity<HandsComponent>) args.User, (EntityUid) ent, checkActionBlocker: false);
    }
    else if (!ent.Comp.AllowedCategories.Contains(comp1.Category))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-uniform-accessory-fail-not-allowed"), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
      {
        UniformAccessoryComponent comp2;
        if (this.TryComp<UniformAccessoryComponent>(containedEntity, out comp2))
        {
          int num;
          dictionary[comp2.Category] = !dictionary.TryGetValue(comp2.Category, out num) ? 1 : num + 1;
        }
      }
      int num1;
      if (dictionary.TryGetValue(comp1.Category, out num1) && comp1.Limit <= num1)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-uniform-accessory-fail-limit"), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
      }
      else
      {
        this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, (BaseContainer) container);
        this._item.VisualsChanged((EntityUid) ent);
      }
    }
  }

  private void OnHolderGotEquipped(
    Entity<UniformAccessoryHolderComponent> ent,
    ref GotEquippedEvent args)
  {
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
    {
      UniformAccessoryComponent comp;
      if (this.TryComp<UniformAccessoryComponent>(containedEntity, out comp))
      {
        NetEntity? user = comp.User;
        if (user.HasValue && !this.BelongsToUser(user.GetValueOrDefault(), args.Equipee))
        {
          this._container.Remove((Entity<TransformComponent, MetaDataComponent>) containedEntity, container);
          return;
        }
      }
    }
    this._item.VisualsChanged((EntityUid) ent);
  }

  private void OnHolderGetEquipmentVerbs(
    Entity<UniformAccessoryHolderComponent> ent,
    ref GetVerbsEvent<EquipmentVerb> args)
  {
    BaseContainer container;
    EntityUid? firstAccessory;
    if (!args.CanAccess || !args.CanInteract || this.HasComp<XenoComponent>(args.User) || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container) || !container.ContainedEntities.TryFirstOrNull<EntityUid>(out firstAccessory))
      return;
    EntityUid user = args.User;
    SortedSet<EquipmentVerb> verbs = args.Verbs;
    EquipmentVerb equipmentVerb = new EquipmentVerb();
    equipmentVerb.Text = this.Loc.GetString("rmc-uniform-accessory-remove");
    equipmentVerb.Act = (Action) (() =>
    {
      if (container.ContainedEntities.Count == 1 && firstAccessory.HasValue)
      {
        this._container.Remove((Entity<TransformComponent, MetaDataComponent>) firstAccessory.Value, container);
        this._hands.TryPickupAnyHand(user, firstAccessory.Value);
        this._item.VisualsChanged((EntityUid) ent);
      }
      else
        this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) UniformAccessoriesUi.Key, new EntityUid?(user));
    });
    equipmentVerb.IconEntity = this.GetNetEntity(firstAccessory);
    verbs.Add(equipmentVerb);
  }

  private void OnAccessoriesBuiMsg(
    Entity<UniformAccessoryHolderComponent> ent,
    ref UniformAccessoriesBuiMsg args)
  {
    EntityUid actor = args.Actor;
    EntityUid entity = this.GetEntity(args.ToRemove);
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container))
      return;
    if (this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entity, container))
    {
      this._hands.TryPickupAnyHand(actor, entity);
      this._item.VisualsChanged((EntityUid) ent);
    }
    if (container.ContainedEntities.Count <= 1)
    {
      this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) UniformAccessoriesUi.Key);
    }
    else
    {
      UniformAccessoriesBuiState state = new UniformAccessoriesBuiState();
      this._ui.SetUiState((Entity<UserInterfaceComponent>) ent.Owner, (Enum) UniformAccessoriesUi.Key, (BoundUserInterfaceState) state);
    }
  }

  public bool BelongsToUser(NetEntity user, EntityUid target) => user == this.GetNetEntity(target);

  public void SetAccessoriesHidden(EntityUid accessoryHolder, bool hideAccessories)
  {
    UniformAccessoryHolderComponent comp;
    if (!this.TryComp<UniformAccessoryHolderComponent>(accessoryHolder, out comp))
      return;
    comp.HideAccessories = hideAccessories;
    this.Dirty(accessoryHolder, (IComponent) comp);
    this._item.VisualsChanged(accessoryHolder);
  }
}
