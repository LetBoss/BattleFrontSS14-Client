using Content.Shared.Item;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Inventory;

public static class CMInventoryExtensions
{
	public static bool TryGetFirst(EntityUid storageId, EntityUid itemId, out ItemStorageLocation location)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		location = default(ItemStorageLocation);
		IEntityManager entities = IoCManager.Resolve<IEntityManager>();
		SharedStorageSystem storageSystem = entities.System<SharedStorageSystem>();
		StorageComponent storage = default(StorageComponent);
		ItemComponent item = default(ItemComponent);
		if (!entities.TryGetComponent<StorageComponent>(storageId, ref storage) || !entities.TryGetComponent<ItemComponent>(itemId, ref item))
		{
			return false;
		}
		Box2i storageBounding = storage.Grid.GetBoundingBox();
		for (int y = storageBounding.Bottom; y <= storageBounding.Top; y++)
		{
			for (int x = storageBounding.Left; x <= storageBounding.Right; x++)
			{
				location = new ItemStorageLocation(Angle.op_Implicit(0f), Vector2i.op_Implicit((x, y)));
				if (storageSystem.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit(itemId), Entity<StorageComponent>.op_Implicit(storageId), location))
				{
					return true;
				}
			}
		}
		location = default(ItemStorageLocation);
		return false;
	}
}
