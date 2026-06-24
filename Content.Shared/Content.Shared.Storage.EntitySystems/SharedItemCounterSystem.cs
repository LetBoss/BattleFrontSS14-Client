using System;
using Content.Shared.Stacks;
using Content.Shared.Storage.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Storage.EntitySystems;

public abstract class SharedItemCounterSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemCounterComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<ItemCounterComponent, EntInsertedIntoContainerMessage>)CounterEntityInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemCounterComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<ItemCounterComponent, EntRemovedFromContainerMessage>)CounterEntityRemoved, (Type[])null, (Type[])null);
	}

	private void CounterEntityInserted(EntityUid uid, ItemCounterComponent itemCounter, EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearanceComponent = default(AppearanceComponent);
		if (!((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearanceComponent))
		{
			return;
		}
		int? count = GetCount((ContainerModifiedMessage)(object)args, itemCounter);
		if (count.HasValue)
		{
			_appearance.SetData(uid, (Enum)StackVisuals.Actual, (object)count, appearanceComponent);
			if (itemCounter.MaxAmount.HasValue)
			{
				_appearance.SetData(uid, (Enum)StackVisuals.MaxCount, (object)itemCounter.MaxAmount, appearanceComponent);
			}
		}
	}

	private void CounterEntityRemoved(EntityUid uid, ItemCounterComponent itemCounter, EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearanceComponent = default(AppearanceComponent);
		if (!((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearanceComponent))
		{
			return;
		}
		int? count = GetCount((ContainerModifiedMessage)(object)args, itemCounter);
		if (count.HasValue)
		{
			_appearance.SetData(uid, (Enum)StackVisuals.Actual, (object)count, appearanceComponent);
			if (itemCounter.MaxAmount.HasValue)
			{
				_appearance.SetData(uid, (Enum)StackVisuals.MaxCount, (object)itemCounter.MaxAmount, appearanceComponent);
			}
		}
	}

	protected abstract int? GetCount(ContainerModifiedMessage msg, ItemCounterComponent itemCounter);
}
