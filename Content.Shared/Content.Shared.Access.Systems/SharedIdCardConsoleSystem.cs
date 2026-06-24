using System;
using System.Collections.Generic;
using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems;

public abstract class SharedIdCardConsoleSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	private sealed class IdCardConsoleComponentState : ComponentState
	{
		public List<string> AccessLevels;

		public IdCardConsoleComponentState(List<string> accessLevels)
		{
			AccessLevels = accessLevels;
		}
	}

	[Dependency]
	private ItemSlotsSystem _itemSlotsSystem;

	[Dependency]
	private ILogManager _log;

	public const string Sawmill = "idconsole";

	protected ISawmill _sawmill;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_sawmill = _log.GetSawmill("idconsole");
		((EntitySystem)this).SubscribeLocalEvent<IdCardConsoleComponent, ComponentInit>((ComponentEventHandler<IdCardConsoleComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IdCardConsoleComponent, ComponentRemove>((ComponentEventHandler<IdCardConsoleComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, IdCardConsoleComponent component, ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_itemSlotsSystem.AddItemSlot(uid, IdCardConsoleComponent.PrivilegedIdCardSlotId, component.PrivilegedIdSlot);
		_itemSlotsSystem.AddItemSlot(uid, IdCardConsoleComponent.TargetIdCardSlotId, component.TargetIdSlot);
	}

	private void OnComponentRemove(EntityUid uid, IdCardConsoleComponent component, ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		_itemSlotsSystem.RemoveItemSlot(uid, component.PrivilegedIdSlot);
		_itemSlotsSystem.RemoveItemSlot(uid, component.TargetIdSlot);
	}
}
