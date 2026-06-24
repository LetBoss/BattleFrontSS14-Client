// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Inventory.CMInventoryExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable disable
namespace Content.Shared._RMC14.Inventory;

public static class CMInventoryExtensions
{
  public static bool TryGetFirst(
    EntityUid storageId,
    EntityUid itemId,
    out ItemStorageLocation location)
  {
    location = new ItemStorageLocation();
    IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
    SharedStorageSystem sharedStorageSystem = entityManager.System<SharedStorageSystem>();
    StorageComponent component;
    if (!entityManager.TryGetComponent<StorageComponent>(storageId, out component) || !entityManager.TryGetComponent<ItemComponent>(itemId, out ItemComponent _))
      return false;
    Box2i boundingBox = component.Grid.GetBoundingBox();
    for (int bottom = boundingBox.Bottom; bottom <= boundingBox.Top; ++bottom)
    {
      for (int left = boundingBox.Left; left <= boundingBox.Right; ++left)
      {
        location = new ItemStorageLocation(Angle.op_Implicit(0.0f), Vector2i.op_Implicit((left, bottom)));
        if (sharedStorageSystem.ItemFitsInGridLocation((Entity<ItemComponent>) itemId, (Entity<StorageComponent>) storageId, location))
          return true;
      }
    }
    location = new ItemStorageLocation();
    return false;
  }
}
