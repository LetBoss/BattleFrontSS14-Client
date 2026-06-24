using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Scoping;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Visor;

public sealed class VisorSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedPowerCellSystem _powerCell;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ScopedEvent>((EntityEventRefHandler<InventoryComponent, ScopedEvent>)OnInventoryScoped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CycleableVisorComponent, GetEquipmentVisualsEvent>((EntityEventRefHandler<CycleableVisorComponent, GetEquipmentVisualsEvent>)OnCycleableVisorGetEquipmentVisuals, (Type[])null, new Type[1] { typeof(ClothingSystem) });
		((EntitySystem)this).SubscribeLocalEvent<CycleableVisorComponent, GetItemActionsEvent>((EntityEventRefHandler<CycleableVisorComponent, GetItemActionsEvent>)OnCycleableVisorGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CycleableVisorComponent, CycleVisorActionEvent>((EntityEventRefHandler<CycleableVisorComponent, CycleVisorActionEvent>)OnCycleableVisorAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CycleableVisorComponent, InteractUsingEvent>((EntityEventRefHandler<CycleableVisorComponent, InteractUsingEvent>)OnCycleableVisorInteractUsing, new Type[1] { typeof(SharedStorageSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CycleableVisorComponent, InventoryRelayedEvent<ScopedEvent>>((EntityEventRefHandler<CycleableVisorComponent, InventoryRelayedEvent<ScopedEvent>>)OnCycleableVisorScoped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CycleableVisorComponent, ExaminedEvent>((EntityEventRefHandler<CycleableVisorComponent, ExaminedEvent>)OnCycleableVisorExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CycleableVisorComponent, GotEquippedEvent>((EntityEventRefHandler<CycleableVisorComponent, GotEquippedEvent>)OnCycleableVisorEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VisorComponent, ActivateVisorAttemptEvent>((EntityEventRefHandler<VisorComponent, ActivateVisorAttemptEvent>)OnVisorAttemptActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VisorComponent, ActivateVisorEvent>((EntityEventRefHandler<VisorComponent, ActivateVisorEvent>)OnVisorActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VisorComponent, DeactivateVisorEvent>((EntityEventRefHandler<VisorComponent, DeactivateVisorEvent>)OnVisorDeactivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VisorComponent, PowerCellChangedEvent>((EntityEventRefHandler<VisorComponent, PowerCellChangedEvent>)OnCycleableVisorPowerCellChanged, (Type[])null, new Type[1] { typeof(SharedPowerCellSystem) });
		((EntitySystem)this).SubscribeLocalEvent<ToggleVisorComponent, ActivateVisorAttemptEvent>((EntityEventRefHandler<ToggleVisorComponent, ActivateVisorAttemptEvent>)OnToggleVisorAttemptActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleVisorComponent, ActivateVisorEvent>((EntityEventRefHandler<ToggleVisorComponent, ActivateVisorEvent>)OnToggleVisorActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleVisorComponent, DeactivateVisorEvent>((EntityEventRefHandler<ToggleVisorComponent, DeactivateVisorEvent>)OnToggleVisorDeactivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntegratedVisorsComponent, MapInitEvent>((EntityEventRefHandler<IntegratedVisorsComponent, MapInitEvent>)OnIntegratedVisorsInit, (Type[])null, new Type[1] { typeof(SharedItemSystem) });
	}

	private void OnInventoryScoped(Entity<InventoryComponent> ent, ref ScopedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_inventory.RelayEvent(ent, ref args);
	}

	private void OnCycleableVisorGetItemActions(Entity<CycleableVisorComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
		string currentId = default(string);
		BaseContainer currentContainer = default(BaseContainer);
		VisorComponent visorComp = default(VisorComponent);
		if (ent.Comp.CurrentVisor.HasValue && Extensions.TryGetValue<string>((IList<string>)ent.Comp.Containers, ent.Comp.CurrentVisor.Value, ref currentId) && _container.TryGetContainer(Entity<CycleableVisorComponent>.op_Implicit(ent), currentId, ref currentContainer, (ContainerManagerComponent)null) && ((EntitySystem)this).TryComp<VisorComponent>(currentContainer.ContainedEntities.FirstOrDefault(), ref visorComp))
		{
			EntityUid? action = ent.Comp.Action;
			if (action.HasValue)
			{
				EntityUid action2 = action.GetValueOrDefault();
				_actions.SetIcon(Entity<ActionComponent>.op_Implicit(action2), (SpriteSpecifier?)(object)visorComp.OnIcon);
			}
		}
	}

	private void OnCycleableVisorGetEquipmentVisuals(Entity<CycleableVisorComponent> ent, ref GetEquipmentVisualsEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		string currentId = default(string);
		BaseContainer currentContainer = default(BaseContainer);
		VisorComponent visorComp = default(VisorComponent);
		if (!ent.Comp.CurrentVisor.HasValue || !Extensions.TryGetValue<string>((IList<string>)ent.Comp.Containers, ent.Comp.CurrentVisor.Value, ref currentId) || !_container.TryGetContainer(Entity<CycleableVisorComponent>.op_Implicit(ent), currentId, ref currentContainer, (ContainerManagerComponent)null) || !((EntitySystem)this).TryComp<VisorComponent>(currentContainer.ContainedEntities.FirstOrDefault(), ref visorComp) || visorComp.ToggledSprite == null || (_inventory.TryGetSlot(args.Equipee, args.Slot, out SlotDefinition slot) && (slot.SlotFlags & visorComp.Slot) == 0))
		{
			return;
		}
		string layer = $"enum.{"VisorVisualLayers"}.{VisorVisualLayers.Base}";
		if (!args.Layers.Any<(string, PrototypeLayerData)>(((string, PrototypeLayerData) l) => l.Item1 == layer))
		{
			args.Layers.Add((layer, new PrototypeLayerData
			{
				RsiPath = ((object)visorComp.ToggledSprite.RsiPath/*cast due to constrained. prefix*/).ToString(),
				State = visorComp.ToggledSprite.RsiState,
				Visible = true
			}));
			EntityUid? action = ent.Comp.Action;
			if (action.HasValue)
			{
				EntityUid action2 = action.GetValueOrDefault();
				_actions.SetIcon(Entity<ActionComponent>.op_Implicit(action2), (SpriteSpecifier?)(object)visorComp.OnIcon);
			}
		}
	}

	private void OnCycleableVisorAction(Entity<CycleableVisorComponent> ent, ref CycleVisorActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		List<ContainerSlot> containers = new List<ContainerSlot>();
		foreach (string id in ent.Comp.Containers)
		{
			containers.Add(_container.EnsureContainer<ContainerSlot>(Entity<CycleableVisorComponent>.op_Implicit(ent), id, (ContainerManagerComponent)null));
		}
		if (containers.Count == 0)
		{
			return;
		}
		if (containers.All((ContainerSlot c) => !c.ContainedEntity.HasValue))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-no-visors-to-swap"), Entity<CycleableVisorComponent>.op_Implicit(ent), args.Performer, PopupType.SmallCaution);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		ref int? current = ref ent.Comp.CurrentVisor;
		ContainerSlot currentContainer = default(ContainerSlot);
		EntityUid? containedEntity;
		if (current.HasValue && Extensions.TryGetValue<ContainerSlot>((IList<ContainerSlot>)containers, current.Value, ref currentContainer))
		{
			containedEntity = currentContainer.ContainedEntity;
			if (containedEntity.HasValue)
			{
				EntityUid currentContained = containedEntity.GetValueOrDefault();
				DeactivateVisorEvent ev = new DeactivateVisorEvent(ent, args.Performer);
				((EntitySystem)this).RaiseLocalEvent<DeactivateVisorEvent>(currentContained, ref ev, false);
			}
		}
		bool startedNull = !current.HasValue;
		do
		{
			current = ((!current.HasValue) ? new int?(0) : (current + 1));
			((EntitySystem)this).Dirty<CycleableVisorComponent>(ent, (MetaDataComponent)null);
			if (current >= containers.Count)
			{
				current = null;
				break;
			}
			if (!current.HasValue || !Extensions.TryGetValue<ContainerSlot>((IList<ContainerSlot>)containers, current.Value, ref currentContainer))
			{
				continue;
			}
			containedEntity = currentContainer.ContainedEntity;
			if (!containedEntity.HasValue)
			{
				continue;
			}
			EntityUid newContained = containedEntity.GetValueOrDefault();
			if (!_powerCell.HasDrawCharge(newContained, null, null, args.Performer))
			{
				continue;
			}
			ActivateVisorAttemptEvent rev = new ActivateVisorAttemptEvent(args.Performer);
			((EntitySystem)this).RaiseLocalEvent<ActivateVisorAttemptEvent>(newContained, ref rev, false);
			if (!((CancellableEntityEventArgs)rev).Cancelled)
			{
				ActivateVisorEvent ev2 = new ActivateVisorEvent(ent, args.Performer);
				((EntitySystem)this).RaiseLocalEvent<ActivateVisorEvent>(newContained, ref ev2, false);
				if (ev2.Handled)
				{
					break;
				}
				current = null;
			}
		}
		while (current.HasValue);
		if (startedNull && !current.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-no-visors-to-swap"), Entity<CycleableVisorComponent>.op_Implicit(ent), args.Performer, PopupType.SmallCaution);
		}
		containedEntity = ent.Comp.Action;
		if (containedEntity.HasValue)
		{
			EntityUid action = containedEntity.GetValueOrDefault();
			if (!current.HasValue)
			{
				_actions.SetIcon(Entity<ActionComponent>.op_Implicit(action), (SpriteSpecifier?)(object)ent.Comp.OffIcon);
			}
		}
		_item.VisualsChanged(Entity<CycleableVisorComponent>.op_Implicit(ent));
	}

	private void OnCycleableVisorEquipped(Entity<CycleableVisorComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		List<ContainerSlot> containers = new List<ContainerSlot>();
		foreach (string id in ent.Comp.Containers)
		{
			containers.Add(_container.EnsureContainer<ContainerSlot>(Entity<CycleableVisorComponent>.op_Implicit(ent), id, (ContainerManagerComponent)null));
		}
		if (containers.Count == 0)
		{
			return;
		}
		ref int? current = ref ent.Comp.CurrentVisor;
		ContainerSlot currentContainer = default(ContainerSlot);
		if (!current.HasValue || !Extensions.TryGetValue<ContainerSlot>((IList<ContainerSlot>)containers, current.Value, ref currentContainer))
		{
			return;
		}
		EntityUid? containedEntity = currentContainer.ContainedEntity;
		if (!containedEntity.HasValue)
		{
			return;
		}
		EntityUid newContained = containedEntity.GetValueOrDefault();
		ActivateVisorAttemptEvent rev = new ActivateVisorAttemptEvent(args.Equipee);
		((EntitySystem)this).RaiseLocalEvent<ActivateVisorAttemptEvent>(newContained, ref rev, false);
		if (((CancellableEntityEventArgs)rev).Cancelled)
		{
			DeactivateVisor(ent, Entity<VisorComponent>.op_Implicit(newContained), args.Equipee);
			current = null;
			_item.VisualsChanged(Entity<CycleableVisorComponent>.op_Implicit(ent));
			containedEntity = ent.Comp.Action;
			if (containedEntity.HasValue)
			{
				EntityUid action = containedEntity.GetValueOrDefault();
				_actions.SetIcon(Entity<ActionComponent>.op_Implicit(action), (SpriteSpecifier?)(object)ent.Comp.OffIcon);
			}
			_popup.PopupClient(base.Loc.GetString("rmc-skills-no-training", (ValueTuple<string, object>)("target", newContained)), args.Equipee, args.Equipee, PopupType.SmallCaution);
		}
	}

	private void OnCycleableVisorInteractUsing(Entity<CycleableVisorComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		VisorComponent visor = default(VisorComponent);
		if (((EntitySystem)this).TryComp<VisorComponent>(args.Used, ref visor))
		{
			if (AttachVisor(ent, Entity<VisorComponent>.op_Implicit((args.Used, visor)), args.User))
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
			return;
		}
		foreach (ProtoId<ToolQualityPrototype> tool in ent.Comp.RemoveQuality)
		{
			if (!_tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(tool)))
			{
				return;
			}
		}
		((HandledEntityEventArgs)args).Handled = true;
		string currentId = default(string);
		BaseContainer currentContainer = default(BaseContainer);
		if (ent.Comp.CurrentVisor.HasValue && Extensions.TryGetValue<string>((IList<string>)ent.Comp.Containers, ent.Comp.CurrentVisor.Value, ref currentId) && _container.TryGetContainer(Entity<CycleableVisorComponent>.op_Implicit(ent), currentId, ref currentContainer, (ContainerManagerComponent)null))
		{
			foreach (EntityUid contained in currentContainer.ContainedEntities)
			{
				DeactivateVisorEvent ev = new DeactivateVisorEvent(ent, args.User);
				((EntitySystem)this).RaiseLocalEvent<DeactivateVisorEvent>(contained, ref ev, false);
			}
		}
		bool anyRemoved = false;
		BaseContainer container = default(BaseContainer);
		foreach (string id in ent.Comp.Containers)
		{
			if (!_container.TryGetContainer(Entity<CycleableVisorComponent>.op_Implicit(ent), id, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			bool canRemove = true;
			foreach (EntityUid contained2 in container.ContainedEntities)
			{
				if (((EntitySystem)this).HasComp<UnremovableVisorComponent>(contained2))
				{
					canRemove = false;
				}
			}
			if (canRemove && _container.EmptyContainer(container, false, (EntityCoordinates?)null, true).Count > 0)
			{
				anyRemoved = true;
			}
		}
		if (anyRemoved)
		{
			_popup.PopupClient("You remove the inserted visors", args.Target, args.User);
		}
		else
		{
			_popup.PopupClient("There are no visors left to take out!", args.Target, args.User);
		}
		ent.Comp.CurrentVisor = null;
		((EntitySystem)this).Dirty<CycleableVisorComponent>(ent, (MetaDataComponent)null);
	}

	private void OnCycleableVisorScoped(Entity<CycleableVisorComponent> ent, ref InventoryRelayedEvent<ScopedEvent> args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		VisorRelayedEvent<ScopedEvent> ev = new VisorRelayedEvent<ScopedEvent>(ent, args.Args);
		BaseContainer container = default(BaseContainer);
		foreach (string containerId in ent.Comp.Containers)
		{
			if (!_container.TryGetContainer(Entity<CycleableVisorComponent>.op_Implicit(ent), containerId, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid contained in container.ContainedEntities)
			{
				((EntitySystem)this).RaiseLocalEvent<VisorRelayedEvent<ScopedEvent>>(contained, ref ev, false);
			}
		}
		args.Args = ev.Event;
	}

	private void OnCycleableVisorExamined(Entity<CycleableVisorComponent> ent, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("CycleableVisorComponent"))
		{
			if (ent.Comp.CurrentVisor.HasValue)
			{
				string currentId = default(string);
				BaseContainer currentContainer = default(BaseContainer);
				if (!Extensions.TryGetValue<string>((IList<string>)ent.Comp.Containers, ent.Comp.CurrentVisor.Value, ref currentId) || !_container.TryGetContainer(Entity<CycleableVisorComponent>.op_Implicit(ent), currentId, ref currentContainer, (ContainerManagerComponent)null))
				{
					return;
				}
				args.PushMarkup(base.Loc.GetString("rmc-visor-down", (ValueTuple<string, object>)("visor", currentContainer.ContainedEntities.FirstOrDefault())));
			}
			args.PushMarkup("Use a [color=cyan]screwdriver[/color] on this to take out any visors!");
		}
	}

	private void OnVisorAttemptActivate(Entity<VisorComponent> ent, ref ActivateVisorAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.SkillsRequired != null && !_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.SkillsRequired))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnVisorActivate(Entity<VisorComponent> ent, ref ActivateVisorEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? action = args.CycleableVisor.Comp.Action;
		if (action.HasValue)
		{
			EntityUid action2 = action.GetValueOrDefault();
			_actions.SetIcon(Entity<ActionComponent>.op_Implicit(action2), (SpriteSpecifier?)(object)ent.Comp.OnIcon);
		}
		if (((EntitySystem)this).HasComp<PowerCellSlotComponent>(Entity<VisorComponent>.op_Implicit(ent)))
		{
			_powerCell.SetDrawEnabled(Entity<PowerCellDrawComponent>.op_Implicit(ent.Owner), enabled: true);
		}
	}

	private void OnVisorDeactivate(Entity<VisorComponent> ent, ref DeactivateVisorEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_powerCell.SetDrawEnabled(Entity<PowerCellDrawComponent>.op_Implicit(ent.Owner), enabled: false);
	}

	private void OnCycleableVisorPowerCellChanged(Entity<VisorComponent> ent, ref PowerCellChangedEvent args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Ejected && _powerCell.HasDrawCharge(Entity<VisorComponent>.op_Implicit(ent)))
		{
			return;
		}
		PowerCellDrawComponent powerCellDraw = default(PowerCellDrawComponent);
		if (((EntitySystem)this).TryComp<PowerCellDrawComponent>(Entity<VisorComponent>.op_Implicit(ent), ref powerCellDraw))
		{
			bool canDraw = !args.Ejected && _powerCell.HasDrawCharge(Entity<VisorComponent>.op_Implicit(ent), powerCellDraw);
			bool canUse = !args.Ejected && _powerCell.HasDrawCharge(Entity<VisorComponent>.op_Implicit(ent), powerCellDraw);
			powerCellDraw.CanDraw = canDraw;
			powerCellDraw.CanUse = canUse;
			((EntitySystem)this).Dirty(Entity<VisorComponent>.op_Implicit(ent), (IComponent)(object)powerCellDraw, (MetaDataComponent)null);
		}
		BaseContainer visorContainer = default(BaseContainer);
		CycleableVisorComponent cycleable = default(CycleableVisorComponent);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<VisorComponent>.op_Implicit(ent), null)), ref visorContainer) || !((EntitySystem)this).TryComp<CycleableVisorComponent>(visorContainer.Owner, ref cycleable))
		{
			return;
		}
		DeactivateVisorEvent ev = new DeactivateVisorEvent(Entity<CycleableVisorComponent>.op_Implicit((visorContainer.Owner, cycleable)), null);
		((EntitySystem)this).RaiseLocalEvent<DeactivateVisorEvent>(Entity<VisorComponent>.op_Implicit(ent), ref ev, false);
		int? currentVisor = cycleable.CurrentVisor;
		if (currentVisor.HasValue)
		{
			int current = currentVisor.GetValueOrDefault();
			string container = default(string);
			if (current >= 0 && Extensions.TryGetValue<string>((IList<string>)cycleable.Containers, current, ref container) && visorContainer.ID == container)
			{
				cycleable.CurrentVisor = null;
				((EntitySystem)this).Dirty(visorContainer.Owner, (IComponent)(object)cycleable, (MetaDataComponent)null);
			}
		}
	}

	private bool AttachVisor(Entity<CycleableVisorComponent> cycleable, Entity<VisorComponent> visor, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<ItemComponent>(Entity<VisorComponent>.op_Implicit(visor)))
		{
			return false;
		}
		string msg;
		foreach (string id in cycleable.Comp.Containers)
		{
			ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<CycleableVisorComponent>.op_Implicit(cycleable), id, (ContainerManagerComponent)null);
			if (_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(visor.Owner), (BaseContainer)(object)container, (TransformComponent)null, false))
			{
				msg = $"You connect the {((EntitySystem)this).Name(Entity<VisorComponent>.op_Implicit(visor), (MetaDataComponent)null)} to {((EntitySystem)this).Name(Entity<CycleableVisorComponent>.op_Implicit(cycleable), (MetaDataComponent)null)}.";
				_popup.PopupClient(msg, Entity<CycleableVisorComponent>.op_Implicit(cycleable), user);
				return true;
			}
		}
		msg = ((EntitySystem)this).Name(Entity<CycleableVisorComponent>.op_Implicit(cycleable), (MetaDataComponent)null) + " has used all of its visor attachment sockets.";
		_popup.PopupClient(msg, Entity<CycleableVisorComponent>.op_Implicit(cycleable), user, PopupType.SmallCaution);
		return true;
	}

	public void DeactivateVisor(Entity<CycleableVisorComponent> cycleable, Entity<VisorComponent?> visor, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		ref int? current = ref cycleable.Comp.CurrentVisor;
		if (!current.HasValue || current < 0 || current >= cycleable.Comp.Containers.Count)
		{
			return;
		}
		string containerId = cycleable.Comp.Containers[current.Value];
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<CycleableVisorComponent>.op_Implicit(cycleable), containerId, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		foreach (EntityUid contained in container.ContainedEntities)
		{
			if (contained == visor.Owner)
			{
				DeactivateVisorEvent ev = new DeactivateVisorEvent(cycleable, user);
				((EntitySystem)this).RaiseLocalEvent<DeactivateVisorEvent>(contained, ref ev, false);
				current = null;
				((EntitySystem)this).Dirty<CycleableVisorComponent>(cycleable, (MetaDataComponent)null);
				break;
			}
		}
	}

	public void OnToggleVisorAttemptActivate(Entity<ToggleVisorComponent> visor, ref ActivateVisorAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		ComponentTogglerComponent toggle = default(ComponentTogglerComponent);
		VisorComponent visComp = default(VisorComponent);
		if (visor.Comp.IgnoreRedundancy || !((EntitySystem)this).TryComp<ComponentTogglerComponent>(Entity<ToggleVisorComponent>.op_Implicit(visor), ref toggle) || !((EntitySystem)this).TryComp<VisorComponent>(Entity<ToggleVisorComponent>.op_Implicit(visor), ref visComp))
		{
			return;
		}
		foreach (ComponentRegistryEntry value in ((Dictionary<string, ComponentRegistryEntry>)(object)toggle.Components).Values)
		{
			Type type = ((object)value.Component).GetType();
			if (((EntitySystem)this).HasComp(args.User, type))
			{
				continue;
			}
			if (!_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.User), out var containerSlotEnumerator))
			{
				return;
			}
			bool itemHasComp = false;
			EntityUid item;
			SlotDefinition slot;
			while (containerSlotEnumerator.NextItem(out item, out slot))
			{
				if ((slot.SlotFlags & visComp.Slot) == 0 && ((EntitySystem)this).HasComp(item, type))
				{
					itemHasComp = true;
					break;
				}
			}
			if (!itemHasComp)
			{
				return;
			}
		}
		((CancellableEntityEventArgs)args).Cancel();
	}

	public void OnToggleVisorActivate(Entity<ToggleVisorComponent> visor, ref ActivateVisorEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		ComponentTogglerComponent toggle = default(ComponentTogglerComponent);
		if (((EntitySystem)this).TryComp<ComponentTogglerComponent>(Entity<ToggleVisorComponent>.op_Implicit(visor), ref toggle))
		{
			args.Handled = true;
			base.EntityManager.AddComponents(Entity<CycleableVisorComponent>.op_Implicit(args.CycleableVisor), toggle.Components, true);
			if (args.User.HasValue)
			{
				_audio.PlayLocal(visor.Comp.SoundOn, Entity<ToggleVisorComponent>.op_Implicit(visor), args.User, (AudioParams?)null);
			}
		}
	}

	public void OnToggleVisorDeactivate(Entity<ToggleVisorComponent> visor, ref DeactivateVisorEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		ComponentTogglerComponent toggle = default(ComponentTogglerComponent);
		if (((EntitySystem)this).TryComp<ComponentTogglerComponent>(Entity<ToggleVisorComponent>.op_Implicit(visor), ref toggle))
		{
			base.EntityManager.RemoveComponents(Entity<CycleableVisorComponent>.op_Implicit(args.CycleableVisor), toggle.RemoveComponents ?? toggle.Components);
			if (args.User.HasValue)
			{
				_audio.PlayLocal(visor.Comp.SoundOff, Entity<ToggleVisorComponent>.op_Implicit(visor), args.User, (AudioParams?)null);
			}
		}
	}

	private void OnIntegratedVisorsInit(Entity<IntegratedVisorsComponent> integrated, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		CycleableVisorComponent cycleable = default(CycleableVisorComponent);
		if (!((EntitySystem)this).TryComp<CycleableVisorComponent>(Entity<IntegratedVisorsComponent>.op_Implicit(integrated), ref cycleable))
		{
			return;
		}
		List<ContainerSlot> containers = new List<ContainerSlot>();
		foreach (string id in cycleable.Containers)
		{
			containers.Add(_container.EnsureContainer<ContainerSlot>(Entity<IntegratedVisorsComponent>.op_Implicit(integrated), id, (ContainerManagerComponent)null));
		}
		VisorComponent visor = default(VisorComponent);
		foreach (EntProtoId proto in integrated.Comp.VisorsToAdd)
		{
			EntityUid vis = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(proto), integrated.Owner.ToCoordinates(), (ComponentRegistry)null);
			if (!((EntitySystem)this).TryComp<VisorComponent>(vis, ref visor))
			{
				((EntitySystem)this).QueueDel((EntityUid?)vis);
			}
			else if (!AttachVisor(Entity<CycleableVisorComponent>.op_Implicit((integrated.Owner, cycleable)), Entity<VisorComponent>.op_Implicit((vis, visor)), null))
			{
				((EntitySystem)this).QueueDel((EntityUid?)vis);
			}
		}
		ref int? current = ref cycleable.CurrentVisor;
		if (!integrated.Comp.StartToggled || containers.Count <= 0)
		{
			return;
		}
		current = ((!current.HasValue) ? new int?(0) : (current + 1));
		((EntitySystem)this).Dirty(integrated.Owner, (IComponent)(object)cycleable, (MetaDataComponent)null);
		if (current >= containers.Count)
		{
			current = null;
		}
		ContainerSlot currentContainer = default(ContainerSlot);
		if (current.HasValue && Extensions.TryGetValue<ContainerSlot>((IList<ContainerSlot>)containers, current.Value, ref currentContainer))
		{
			EntityUid? containedEntity = currentContainer.ContainedEntity;
			if (containedEntity.HasValue)
			{
				EntityUid newContained = containedEntity.GetValueOrDefault();
				if (!_powerCell.HasDrawCharge(newContained))
				{
					current = null;
					return;
				}
				ActivateVisorEvent ev = new ActivateVisorEvent(Entity<CycleableVisorComponent>.op_Implicit((integrated.Owner, cycleable)), null);
				((EntitySystem)this).RaiseLocalEvent<ActivateVisorEvent>(newContained, ref ev, false);
				if (!ev.Handled)
				{
					current = null;
				}
			}
		}
		_item.VisualsChanged(integrated.Owner);
	}
}
