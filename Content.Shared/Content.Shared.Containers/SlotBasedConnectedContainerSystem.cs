using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Chemistry.Components;
using Content.Shared.Inventory;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Containers;

public sealed class SlotBasedConnectedContainerSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private InventorySystem _inventory;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SlotBasedConnectedContainerComponent, GetConnectedContainerEvent>((EntityEventRefHandler<SlotBasedConnectedContainerComponent, GetConnectedContainerEvent>)OnGettingConnectedContainer, (Type[])null, (Type[])null);
	}

	public bool TryGetConnectedContainer(EntityUid uid, [NotNullWhen(true)] out EntityUid? slotEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		SlotBasedConnectedContainerComponent component = default(SlotBasedConnectedContainerComponent);
		if (!((EntitySystem)this).TryComp<SlotBasedConnectedContainerComponent>(uid, ref component))
		{
			slotEntity = null;
			return false;
		}
		return TryGetConnectedContainer(uid, component.TargetSlot, component.ContainerWhitelist, out slotEntity);
	}

	private void OnGettingConnectedContainer(Entity<SlotBasedConnectedContainerComponent> ent, ref GetConnectedContainerEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetConnectedContainer(Entity<SlotBasedConnectedContainerComponent>.op_Implicit(ent), ent.Comp.TargetSlot, ent.Comp.ContainerWhitelist, out var val))
		{
			args.ContainerEntity = val;
		}
	}

	private bool TryGetConnectedContainer(EntityUid uid, SlotFlags slotFlag, EntityWhitelist? providerWhitelist, [NotNullWhen(true)] out EntityUid? slotEntity)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		slotEntity = null;
		BaseContainer container = default(BaseContainer);
		if (!_containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, null)), ref container))
		{
			return false;
		}
		EntityUid user = container.Owner;
		if (!_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(user), out var enumerator, slotFlag))
		{
			return false;
		}
		EntityUid item;
		while (enumerator.NextItem(out item))
		{
			if (!_whitelistSystem.IsWhitelistFailOrNull(providerWhitelist, item))
			{
				slotEntity = item;
				return true;
			}
		}
		return false;
	}
}
