using System;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Storage.EntitySystems;

public sealed class StoreAfterFailedInteractSystem : EntitySystem
{
	[Dependency]
	private SharedStorageSystem _storage;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StoreAfterFailedInteractComponent, StorageInsertFailedEvent>((EntityEventRefHandler<StoreAfterFailedInteractComponent, StorageInsertFailedEvent>)OnStorageInsertFailed, (Type[])null, (Type[])null);
	}

	private void OnStorageInsertFailed(Entity<StoreAfterFailedInteractComponent> ent, ref StorageInsertFailedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_storage.PlayerInsertHeldEntity(args.Storage, args.Player);
	}
}
