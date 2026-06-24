using System;
using Content.Shared.Storage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Storage.EntitySystems;

public sealed class AnchoredStorageFilterSystem : EntitySystem
{
	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedContainerSystem _container;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AnchoredStorageFilterComponent, AnchorStateChangedEvent>((EntityEventRefHandler<AnchoredStorageFilterComponent, AnchorStateChangedEvent>)OnAnchorStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnchoredStorageFilterComponent, ContainerIsInsertingAttemptEvent>((EntityEventRefHandler<AnchoredStorageFilterComponent, ContainerIsInsertingAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
	}

	private void OnAnchorStateChanged(Entity<AnchoredStorageFilterComponent> ent, ref AnchorStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent storage = default(StorageComponent);
		if (!((AnchorStateChangedEvent)(ref args)).Anchored || !((EntitySystem)this).TryComp<StorageComponent>(Entity<AnchoredStorageFilterComponent>.op_Implicit(ent), ref storage))
		{
			return;
		}
		foreach (EntityUid item in storage.StoredItems.Keys)
		{
			if (!_whitelist.CheckBoth(item, ent.Comp.Blacklist, ent.Comp.Whitelist))
			{
				_container.RemoveEntity(Entity<AnchoredStorageFilterComponent>.op_Implicit(ent), item, (ContainerManagerComponent)null, (TransformComponent)null, (MetaDataComponent)null, true, false, (EntityCoordinates?)null, (Angle?)null);
			}
		}
	}

	private void OnInsertAttempt(Entity<AnchoredStorageFilterComponent> ent, ref ContainerIsInsertingAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && ((EntitySystem)this).Transform(Entity<AnchoredStorageFilterComponent>.op_Implicit(ent)).Anchored && !_whitelist.CheckBoth(((ContainerAttemptEventBase)args).EntityUid, ent.Comp.Blacklist, ent.Comp.Whitelist))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
