// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.MagnetPickupSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.Storage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public sealed class MagnetPickupSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedStorageSystem _storage;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  private static readonly TimeSpan ScanDelay = TimeSpan.FromSeconds(1L);
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.SubscribeLocalEvent<MagnetPickupComponent, MapInitEvent>(new ComponentEventHandler<MagnetPickupComponent, MapInitEvent>(this.OnMagnetMapInit));
  }

  private void OnMagnetMapInit(EntityUid uid, MagnetPickupComponent component, MapInitEvent args)
  {
    component.NextScan = this._timing.CurTime;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<MagnetPickupComponent, StorageComponent, TransformComponent, MetaDataComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MagnetPickupComponent, StorageComponent, TransformComponent, MetaDataComponent>();
    TimeSpan curTime = this._timing.CurTime;
    EntityUid uid1;
    MagnetPickupComponent comp1;
    StorageComponent comp2;
    TransformComponent comp3;
    MetaDataComponent comp4;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1, out comp2, out comp3, out comp4))
    {
      if (!(comp1.NextScan > curTime))
      {
        comp1.NextScan += MagnetPickupSystem.ScanDelay;
        SlotDefinition slot;
        if (this._inventory.TryGetContainingSlot((Entity<TransformComponent, MetaDataComponent>) (uid1, comp3, comp4), out slot) && (slot.SlotFlags & comp1.SlotFlags) != SlotFlags.NONE && this._storage.HasSpace((Entity<StorageComponent>) (uid1, comp2)))
        {
          EntityUid parentUid = comp3.ParentUid;
          bool flag1 = false;
          EntityCoordinates coordinates1 = comp3.Coordinates;
          EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(uid1, comp3);
          foreach (EntityUid entityUid in this._lookup.GetEntitiesInRange(uid1, comp1.Range, LookupFlags.Dynamic | LookupFlags.Sundries))
          {
            PhysicsComponent component;
            if (!this._whitelistSystem.IsWhitelistFail(comp2.Whitelist, entityUid) && this._physicsQuery.TryGetComponent(entityUid, out component) && component.BodyStatus == BodyStatus.OnGround && !(entityUid == parentUid))
            {
              TransformComponent xform = this.Transform(entityUid);
              MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(entityUid, xform);
              EntityCoordinates coordinates2 = this._transform.ToCoordinates((Entity<TransformComponent>) moverCoordinates.EntityId, mapCoordinates);
              SharedStorageSystem storage = this._storage;
              EntityUid uid2 = uid1;
              EntityUid insertEnt = entityUid;
              EntityUid? nullable;
              ref EntityUid? local = ref nullable;
              StorageComponent storageComponent = comp2;
              bool flag2 = !flag1;
              EntityUid? user = new EntityUid?();
              StorageComponent storageComp = storageComponent;
              int num = flag2 ? 1 : 0;
              if (storage.Insert(uid2, insertEnt, out local, user, storageComp, num != 0))
              {
                if (nullable.HasValue)
                  this._storage.PlayPickupAnimation(nullable.Value, coordinates2, coordinates1, xform.LocalRotation);
                else
                  this._storage.PlayPickupAnimation(entityUid, coordinates2, coordinates1, xform.LocalRotation);
                flag1 = true;
              }
            }
          }
        }
      }
    }
  }
}
