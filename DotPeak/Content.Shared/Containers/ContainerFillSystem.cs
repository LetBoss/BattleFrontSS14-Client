// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.ContainerFillSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Prototypes;
using Content.Shared._RMC14.Storage;
using Content.Shared.EntityTable;
using Content.Shared.EntityTable.EntitySelectors;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Containers;

public sealed class ContainerFillSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private EntityTableSystem _entityTable;
  [Dependency]
  private SharedTransformSystem _transform;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ContainerFillComponent, MapInitEvent>(new ComponentEventHandler<ContainerFillComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityTableContainerFillComponent, MapInitEvent>(new EntityEventRefHandler<EntityTableContainerFillComponent, MapInitEvent>((object) this, __methodptr(OnTableMapInit)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(EntityUid uid, ContainerFillComponent component, MapInitEvent args)
  {
    ContainerManagerComponent managerComponent;
    if (!this.TryComp<ContainerManagerComponent>(uid, ref managerComponent))
      return;
    TransformComponent transformComponent = this.Transform(uid);
    EntityCoordinates entityCoordinates;
    // ISSUE: explicit constructor call
    ((EntityCoordinates) ref entityCoordinates).\u002Ector(uid, Vector2.Zero);
    foreach ((string key, List<string> stringList) in component.Containers)
    {
      BaseContainer baseContainer;
      if (!this._containerSystem.TryGetContainer(uid, key, ref baseContainer, managerComponent))
      {
        this.Log.Error($"Entity {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))} with a {"ContainerFillComponent"} is missing a container ({key}).");
      }
      else
      {
        foreach (string str1 in stringList)
        {
          EntityUid entityUid = this.Spawn(str1, entityCoordinates);
          if (!this._containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(entityUid), baseContainer, transformComponent, false))
          {
            string str2 = baseContainer.ContainedEntities.Count > 0 ? string.Join("\n", baseContainer.ContainedEntities.Select<EntityUid, string>((Func<EntityUid, string>) (e => $"\t - {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(e))}"))) : "< empty >";
            if (CMPrototypeExtensions.FilterCM)
              this.Log.Error($"Entity {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))} with a {"ContainerFillComponent"} failed to insert an entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entityUid))}.\nCurrent contents:\n{str2}");
            this._transform.AttachToGridOrMap(entityUid, (TransformComponent) null);
            break;
          }
        }
      }
    }
  }

  private void OnTableMapInit(Entity<EntityTableContainerFillComponent> ent, ref MapInitEvent args)
  {
    ContainerManagerComponent managerComponent;
    if (!this.TryComp<ContainerManagerComponent>(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), ref managerComponent) || this.TerminatingOrDeleted(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), (MetaDataComponent) null) || !this.Exists(Entity<EntityTableContainerFillComponent>.op_Implicit(ent)))
      return;
    TransformComponent transformComponent = this.Transform(Entity<EntityTableContainerFillComponent>.op_Implicit(ent));
    EntityCoordinates entityCoordinates;
    // ISSUE: explicit constructor call
    ((EntityCoordinates) ref entityCoordinates).\u002Ector(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), Vector2.Zero);
    StorageComponent Storage = this.CompOrNull<StorageComponent>(Entity<EntityTableContainerFillComponent>.op_Implicit(ent));
    foreach ((string key, EntityTableSelector table) in ent.Comp.Containers)
    {
      BaseContainer baseContainer;
      if (!this._containerSystem.TryGetContainer(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), key, ref baseContainer, managerComponent))
      {
        this.Log.Error($"Entity {this.ToPrettyString(new EntityUid?(Entity<EntityTableContainerFillComponent>.op_Implicit(ent)), (MetaDataComponent) null)} with a {"EntityTableContainerFillComponent"} is missing a container ({key}).");
      }
      else
      {
        foreach (EntProtoId spawn in this._entityTable.GetSpawns(table))
        {
          EntityUid entityUid;
          try
          {
            entityUid = this.Spawn(EntProtoId.op_Implicit(spawn), entityCoordinates);
          }
          catch (Exception ex)
          {
            this.Log.Error($"Error spawning {spawn} at {entityCoordinates}:\n{ex}");
            continue;
          }
          ItemComponent itemComponent;
          if (Storage != null && this.TryComp<ItemComponent>(entityUid, ref itemComponent))
          {
            CMStorageItemFillEvent storageItemFillEvent = new CMStorageItemFillEvent(Entity<ItemComponent>.op_Implicit((entityUid, itemComponent)), Storage);
            this.RaiseLocalEvent<CMStorageItemFillEvent>(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), ref storageItemFillEvent, false);
          }
          if (!this._containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(entityUid), baseContainer, transformComponent, false))
          {
            string str = baseContainer.ContainedEntities.Count > 0 ? string.Join("\n", baseContainer.ContainedEntities.Select<EntityUid, string>((Func<EntityUid, string>) (e => $"\t - {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(e))}"))) : "< empty >";
            if (CMPrototypeExtensions.FilterCM)
              this.Log.Error($"Entity {this.ToPrettyString(new EntityUid?(Entity<EntityTableContainerFillComponent>.op_Implicit(ent)), (MetaDataComponent) null)} with a {"EntityTableContainerFillComponent"} failed to insert an entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entityUid))}.\nCurrent contents:\n{str}");
            this._transform.AttachToGridOrMap(entityUid, (TransformComponent) null);
            break;
          }
        }
      }
    }
  }
}
