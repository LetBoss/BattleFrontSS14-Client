// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.Systems.StorageSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Animations;
using Content.Shared.Hands;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Storage.Systems;

public sealed class StorageSystem : SharedStorageSystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private EntityPickupAnimationSystem _entityPickupAnimation;
  private Dictionary<EntityUid, ItemStorageLocation> _oldStoredItems = new Dictionary<EntityUid, ItemStorageLocation>();
  private List<(StorageBoundUserInterface Bui, bool Value)> _queuedBuis = new List<(StorageBoundUserInterface, bool)>();

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StorageComponent, ComponentHandleState>(new ComponentEventRefHandler<StorageComponent, ComponentHandleState>((object) this, __methodptr(OnStorageHandleState)), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PickupAnimationEvent>(new EntityEventHandler<PickupAnimationEvent>(this.HandlePickupAnimation), (Type[]) null, (Type[]) null);
    this.SubscribeAllEvent<AnimateInsertingEntitiesEvent>(new EntityEventHandler<AnimateInsertingEntitiesEvent>(this.HandleAnimatingInsertingEntities), (Type[]) null, (Type[]) null);
  }

  private void OnStorageHandleState(
    EntityUid uid,
    StorageComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is SharedStorageSystem.StorageComponentState current))
      return;
    component.Grid.Clear();
    component.Grid.AddRange((IEnumerable<Box2i>) current.Grid);
    component.MaxItemSize = current.MaxItemSize;
    component.Whitelist = current.Whitelist;
    component.Blacklist = current.Blacklist;
    this._oldStoredItems.Clear();
    foreach (KeyValuePair<EntityUid, ItemStorageLocation> storedItem in component.StoredItems)
      this._oldStoredItems.Add(storedItem.Key, storedItem.Value);
    component.StoredItems.Clear();
    foreach ((NetEntity key1, ItemStorageLocation itemStorageLocation) in current.StoredItems)
    {
      EntityUid key2 = this.EnsureEntity<StorageComponent>(key1, uid);
      component.StoredItems[key2] = itemStorageLocation;
    }
    component.SavedLocations.Clear();
    foreach (KeyValuePair<string, List<ItemStorageLocation>> savedLocation in current.SavedLocations)
      component.SavedLocations[savedLocation.Key] = new List<ItemStorageLocation>((IEnumerable<ItemStorageLocation>) savedLocation.Value);
    this.UpdateOccupied(Entity<StorageComponent>.op_Implicit((uid, component)));
    StorageBoundUserInterface boundUserInterface1;
    if (component.StoredItems.SequenceEqual<KeyValuePair<EntityUid, ItemStorageLocation>>((IEnumerable<KeyValuePair<EntityUid, ItemStorageLocation>>) this._oldStoredItems) || !this.UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) StorageComponent.StorageUiKey.Key, ref boundUserInterface1))
      return;
    boundUserInterface1.Refresh();
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    BaseContainer baseContainer;
    StorageBoundUserInterface boundUserInterface2;
    if (!this.NestedStorage || !localEntity.HasValue || !this.ContainerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer) || !this.UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(baseContainer.Owner), (Enum) StorageComponent.StorageUiKey.Key, ref boundUserInterface2))
      return;
    this._queuedBuis.Add((boundUserInterface2, false));
  }

  public override void UpdateUI(Entity<StorageComponent?> entity)
  {
    StorageBoundUserInterface boundUserInterface;
    if (!this.UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum) StorageComponent.StorageUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Refresh();
  }

  protected override void HideStorageWindow(EntityUid uid, EntityUid actor)
  {
    StorageBoundUserInterface boundUserInterface;
    if (!this.UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) StorageComponent.StorageUiKey.Key, ref boundUserInterface))
      return;
    this._queuedBuis.Add((boundUserInterface, false));
  }

  protected override void ShowStorageWindow(EntityUid uid, EntityUid actor)
  {
    StorageBoundUserInterface boundUserInterface;
    if (!this.UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) StorageComponent.StorageUiKey.Key, ref boundUserInterface))
      return;
    this._queuedBuis.Add((boundUserInterface, true));
  }

  public override void PlayPickupAnimation(
    EntityUid uid,
    EntityCoordinates initialCoordinates,
    EntityCoordinates finalCoordinates,
    Angle initialRotation,
    EntityUid? user = null)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    this.PickupAnimation(uid, initialCoordinates, finalCoordinates, initialRotation);
  }

  private void HandlePickupAnimation(PickupAnimationEvent msg)
  {
    this.PickupAnimation(this.GetEntity(msg.ItemUid), this.GetCoordinates(msg.InitialPosition), this.GetCoordinates(msg.FinalPosition), msg.InitialAngle);
  }

  public void PickupAnimation(
    EntityUid item,
    EntityCoordinates initialCoords,
    EntityCoordinates finalCoords,
    Angle initialAngle)
  {
    if (!this._timing.IsFirstTimePredicted || this.TransformSystem.InRange(finalCoords, initialCoords, 0.1f) || !this.Exists(initialCoords.EntityId) || !this.Exists(finalCoords.EntityId))
      return;
    Vector2 final = Vector2.Transform(this.TransformSystem.ToMapCoordinates(finalCoords, true).Position, this.TransformSystem.GetInvWorldMatrix(initialCoords.EntityId));
    this._entityPickupAnimation.AnimateEntityPickup(item, initialCoords, final, initialAngle);
  }

  public void HandleAnimatingInsertingEntities(AnimateInsertingEntitiesEvent msg)
  {
    TransformComponent transformComponent;
    this.TryComp(this.GetEntity(msg.Storage), ref transformComponent);
    for (int index = 0; msg.StoredEntities.Count > index; ++index)
    {
      EntityUid entity = this.GetEntity(msg.StoredEntities[index]);
      NetCoordinates entityPosition = msg.EntityPositions[index];
      if (this.Exists(entity) && transformComponent != null)
        this._entityPickupAnimation.AnimateEntityPickup(entity, this.GetCoordinates(entityPosition), transformComponent.LocalPosition, msg.EntityAngles[index]);
    }
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    foreach ((StorageBoundUserInterface Bui, bool Value) queuedBui in this._queuedBuis)
    {
      StorageBoundUserInterface bui = queuedBui.Bui;
      if (queuedBui.Value)
        bui.Show();
      else
        bui.Hide();
    }
    this._queuedBuis.Clear();
  }
}
