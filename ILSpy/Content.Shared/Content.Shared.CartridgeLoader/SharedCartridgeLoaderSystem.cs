using System;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.CartridgeLoader;

public abstract class SharedCartridgeLoaderSystem : EntitySystem
{
	public const string InstalledContainerId = "program-container";

	[Dependency]
	private ItemSlotsSystem _itemSlotsSystem;

	[Dependency]
	private SharedAppearanceSystem _appearanceSystem;

	[Dependency]
	private SharedContainerSystem _container;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CartridgeLoaderComponent, ComponentInit>((ComponentEventHandler<CartridgeLoaderComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CartridgeLoaderComponent, ComponentRemove>((ComponentEventHandler<CartridgeLoaderComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CartridgeLoaderComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<CartridgeLoaderComponent, EntInsertedIntoContainerMessage>)OnItemInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CartridgeLoaderComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<CartridgeLoaderComponent, EntRemovedFromContainerMessage>)OnItemRemoved, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, CartridgeLoaderComponent loader, ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_itemSlotsSystem.AddItemSlot(uid, "Cartridge-Slot", loader.CartridgeSlot);
	}

	private void OnComponentRemove(EntityUid uid, CartridgeLoaderComponent loader, ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		_itemSlotsSystem.RemoveItemSlot(uid, loader.CartridgeSlot);
		BaseContainer cont = default(BaseContainer);
		if (_container.TryGetContainer(uid, "program-container", ref cont, (ContainerManagerComponent)null))
		{
			_container.ShutdownContainer(cont);
		}
	}

	protected virtual void OnItemInserted(EntityUid uid, CartridgeLoaderComponent loader, EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearanceData(uid, loader);
	}

	protected virtual void OnItemRemoved(EntityUid uid, CartridgeLoaderComponent loader, EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearanceData(uid, loader);
	}

	private void UpdateAppearanceData(EntityUid uid, CartridgeLoaderComponent loader)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_appearanceSystem.SetData(uid, (Enum)CartridgeLoaderVisuals.CartridgeInserted, (object)loader.CartridgeSlot.HasItem, (AppearanceComponent)null);
	}
}
