using System;
using Content.Shared.Lock;
using Content.Shared.Storage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared.Storage.EntitySystems;

internal sealed class StoreOnCollideSystem : EntitySystem
{
	[Dependency]
	private SharedEntityStorageSystem _storage;

	[Dependency]
	private LockSystem _lock;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private IGameTiming _gameTiming;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StoreOnCollideComponent, StartCollideEvent>((EntityEventRefHandler<StoreOnCollideComponent, StartCollideEvent>)OnCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StoreOnCollideComponent, StorageAfterOpenEvent>((EntityEventRefHandler<StoreOnCollideComponent, StorageAfterOpenEvent>)AfterOpen, (Type[])null, (Type[])null);
	}

	private void OnCollide(Entity<StoreOnCollideComponent> ent, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		TryStoreTarget(ent, args.OtherEntity);
		TryLockStorage(ent);
	}

	private void AfterOpen(Entity<StoreOnCollideComponent> ent, ref StorageAfterOpenEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		StoreOnCollideComponent comp = ent.Comp;
		if (comp != null && comp.DisableWhenFirstOpened && !comp.Disabled)
		{
			comp.Disabled = true;
		}
	}

	private void TryStoreTarget(Entity<StoreOnCollideComponent> ent, EntityUid target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		EntityUid storageEnt = ent.Owner;
		StoreOnCollideComponent comp = ent.Comp;
		if (!_netMan.IsClient && !_gameTiming.ApplyingState && !ent.Comp.Disabled && !(storageEnt == target) && !((EntitySystem)this).Transform(target).Anchored && !_storage.IsOpen(storageEnt) && !_whitelist.IsWhitelistFail(comp.Whitelist, target))
		{
			_storage.Insert(target, storageEnt);
		}
	}

	private void TryLockStorage(Entity<StoreOnCollideComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		EntityUid storageEnt = ent.Owner;
		StoreOnCollideComponent comp = ent.Comp;
		if (!_netMan.IsClient && !_gameTiming.ApplyingState && !ent.Comp.Disabled && comp.LockOnCollide && !_lock.IsLocked(Entity<LockComponent>.op_Implicit(storageEnt)))
		{
			_lock.Lock(storageEnt, storageEnt);
		}
	}
}
