using System;
using Content.Shared.Item;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Toggleable;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.ContainerHeld;

public sealed class ContainerHeldSystem : EntitySystem
{
	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedStorageSystem _storage;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ContainerHeldComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<ContainerHeldComponent, EntInsertedIntoContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ContainerHeldComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<ContainerHeldComponent, EntRemovedFromContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
	}

	private void OnContainerModified(EntityUid uid, ContainerHeldComponent comp, ContainerModifiedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		ItemComponent item = default(ItemComponent);
		if (((EntitySystem)this).HasComp<StorageComponent>(uid) && ((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance) && ((EntitySystem)this).TryComp<ItemComponent>(uid, ref item))
		{
			if (_storage.GetCumulativeItemAreas(Entity<StorageComponent>.op_Implicit(uid)) >= comp.Threshold)
			{
				_item.SetHeldPrefix(uid, "full", force: false, item);
				_appearance.SetData(uid, (Enum)ToggleableVisuals.Enabled, (object)true, appearance);
			}
			else
			{
				_item.SetHeldPrefix(uid, "empty", force: false, item);
				_appearance.SetData(uid, (Enum)ToggleableVisuals.Enabled, (object)false, appearance);
			}
		}
	}
}
