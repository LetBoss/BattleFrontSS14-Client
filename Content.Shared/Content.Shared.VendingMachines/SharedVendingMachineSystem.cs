using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Advertise.Components;
using Content.Shared.Advertise.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Wires;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.VendingMachines;

public abstract class SharedVendingMachineSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	protected IPrototypeManager PrototypeManager;

	[Dependency]
	private AccessReaderSystem _accessReader;

	[Dependency]
	private SharedAppearanceSystem _appearanceSystem;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	protected SharedPointLightSystem Light;

	[Dependency]
	private SharedPowerReceiverSystem _receiver;

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	private SharedSpeakOnUIClosedSystem _speakOn;

	[Dependency]
	protected SharedUserInterfaceSystem UISystem;

	[Dependency]
	protected IRobustRandom Randomizer;

	[Dependency]
	private EmagSystem _emag;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<VendingMachineComponent, ComponentGetState>((EntityEventRefHandler<VendingMachineComponent, ComponentGetState>)OnVendingGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VendingMachineComponent, MapInitEvent>((ComponentEventHandler<VendingMachineComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VendingMachineComponent, GotEmaggedEvent>((ComponentEventRefHandler<VendingMachineComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VendingMachineRestockComponent, AfterInteractEvent>((ComponentEventHandler<VendingMachineRestockComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<VendingMachineComponent>(((EntitySystem)this).Subs, (object)VendingMachineUiKey.Key, (BuiEventSubscriber<VendingMachineComponent>)delegate(Subscriber<VendingMachineComponent> subs)
		{
			subs.Event<VendingMachineEjectMessage>((EntityEventRefHandler<VendingMachineComponent, VendingMachineEjectMessage>)OnInventoryEjectMessage);
		});
	}

	private void OnVendingGetState(Entity<VendingMachineComponent> entity, ref ComponentGetState args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		VendingMachineComponent component = entity.Comp;
		Dictionary<string, VendingMachineInventoryEntry> inventory = new Dictionary<string, VendingMachineInventoryEntry>();
		Dictionary<string, VendingMachineInventoryEntry> emaggedInventory = new Dictionary<string, VendingMachineInventoryEntry>();
		Dictionary<string, VendingMachineInventoryEntry> contrabandInventory = new Dictionary<string, VendingMachineInventoryEntry>();
		foreach (KeyValuePair<string, VendingMachineInventoryEntry> weh in component.Inventory)
		{
			inventory[weh.Key] = new VendingMachineInventoryEntry(weh.Value);
		}
		foreach (KeyValuePair<string, VendingMachineInventoryEntry> weh2 in component.EmaggedInventory)
		{
			emaggedInventory[weh2.Key] = new VendingMachineInventoryEntry(weh2.Value);
		}
		foreach (KeyValuePair<string, VendingMachineInventoryEntry> weh3 in component.ContrabandInventory)
		{
			contrabandInventory[weh3.Key] = new VendingMachineInventoryEntry(weh3.Value);
		}
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new VendingMachineComponentState
		{
			Inventory = inventory,
			EmaggedInventory = emaggedInventory,
			ContrabandInventory = contrabandInventory,
			Contraband = component.Contraband,
			EjectEnd = component.EjectEnd,
			DenyEnd = component.DenyEnd,
			DispenseOnHitEnd = component.DispenseOnHitEnd
		};
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<VendingMachineComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VendingMachineComponent>();
		TimeSpan curTime = Timing.CurTime;
		EntityUid uid = default(EntityUid);
		VendingMachineComponent comp = default(VendingMachineComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (comp.Ejecting)
			{
				TimeSpan value = curTime;
				TimeSpan? ejectEnd = comp.EjectEnd;
				if (value > ejectEnd)
				{
					comp.EjectEnd = null;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
					EjectItem(uid, comp);
					UpdateUI(Entity<VendingMachineComponent>.op_Implicit((uid, comp)));
				}
			}
			if (comp.Denying)
			{
				TimeSpan value = curTime;
				TimeSpan? ejectEnd = comp.DenyEnd;
				if (value > ejectEnd)
				{
					comp.DenyEnd = null;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
					TryUpdateVisualState(Entity<VendingMachineComponent>.op_Implicit((uid, comp)));
				}
			}
			if (comp.DispenseOnHitCoolingDown)
			{
				TimeSpan value = curTime;
				TimeSpan? ejectEnd = comp.DispenseOnHitEnd;
				if (value > ejectEnd)
				{
					comp.DispenseOnHitEnd = null;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
				}
			}
		}
	}

	private void OnInventoryEjectMessage(Entity<VendingMachineComponent> entity, ref VendingMachineEjectMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (_receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(entity.Owner)) && !((EntitySystem)this).Deleted(Entity<VendingMachineComponent>.op_Implicit(entity), (MetaDataComponent)null))
		{
			EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
			if (((EntityUid)(ref actor)).Valid)
			{
				AuthorizedVend(entity.Owner, actor, args.Type, args.ID, entity.Comp);
			}
		}
	}

	protected virtual void OnMapInit(EntityUid uid, VendingMachineComponent component, MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RestockInventoryFromPrototype(uid, component, component.InitialStockQuality);
	}

	protected virtual void EjectItem(EntityUid uid, VendingMachineComponent? vendComponent = null, bool forceEject = false)
	{
	}

	public bool IsAuthorized(EntityUid uid, EntityUid sender, VendingMachineComponent? vendComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VendingMachineComponent>(uid, ref vendComponent, true))
		{
			return false;
		}
		AccessReaderComponent accessReader = default(AccessReaderComponent);
		if (!((EntitySystem)this).TryComp<AccessReaderComponent>(uid, ref accessReader))
		{
			return true;
		}
		if (_accessReader.IsAllowed(sender, uid, accessReader) || ((EntitySystem)this).HasComp<EmaggedComponent>(uid))
		{
			return true;
		}
		Popup.PopupClient(base.Loc.GetString("vending-machine-component-try-eject-access-denied"), uid, sender);
		Deny(Entity<VendingMachineComponent>.op_Implicit((uid, vendComponent)), sender);
		return false;
	}

	protected VendingMachineInventoryEntry? GetEntry(EntityUid uid, string entryId, InventoryType type, VendingMachineComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VendingMachineComponent>(uid, ref component, true))
		{
			return null;
		}
		if (type == InventoryType.Emagged && ((EntitySystem)this).HasComp<EmaggedComponent>(uid))
		{
			return component.EmaggedInventory.GetValueOrDefault(entryId);
		}
		if (type == InventoryType.Contraband && component.Contraband)
		{
			return component.ContrabandInventory.GetValueOrDefault(entryId);
		}
		return component.Inventory.GetValueOrDefault(entryId);
	}

	public void TryEjectVendorItem(EntityUid uid, InventoryType type, string itemId, bool throwItem, EntityUid? user = null, VendingMachineComponent? vendComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VendingMachineComponent>(uid, ref vendComponent, true) || vendComponent.Ejecting || vendComponent.Broken || !_receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(uid)))
		{
			return;
		}
		VendingMachineInventoryEntry entry = GetEntry(uid, itemId, type, vendComponent);
		if (string.IsNullOrEmpty(entry?.ID))
		{
			Popup.PopupClient(base.Loc.GetString("vending-machine-component-try-eject-invalid-item"), uid);
			Deny(Entity<VendingMachineComponent>.op_Implicit((uid, vendComponent)));
			return;
		}
		if (entry.Amount == 0)
		{
			Popup.PopupClient(base.Loc.GetString("vending-machine-component-try-eject-out-of-stock"), uid);
			Deny(Entity<VendingMachineComponent>.op_Implicit((uid, vendComponent)));
			return;
		}
		vendComponent.EjectEnd = Timing.CurTime + vendComponent.EjectDelay;
		vendComponent.NextItemToEject = entry.ID;
		vendComponent.ThrowNextItem = throwItem;
		SpeakOnUIClosedComponent speakComponent = default(SpeakOnUIClosedComponent);
		if (((EntitySystem)this).TryComp<SpeakOnUIClosedComponent>(uid, ref speakComponent))
		{
			_speakOn.TrySetFlag(Entity<SpeakOnUIClosedComponent>.op_Implicit((uid, speakComponent)));
		}
		entry.Amount--;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)vendComponent, (MetaDataComponent)null);
		UpdateUI(Entity<VendingMachineComponent>.op_Implicit((uid, vendComponent)));
		TryUpdateVisualState(Entity<VendingMachineComponent>.op_Implicit((uid, vendComponent)));
		Audio.PlayPredicted(vendComponent.SoundVend, uid, user, (AudioParams?)null);
	}

	public void Deny(Entity<VendingMachineComponent?> entity, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<VendingMachineComponent>(entity.Owner, ref entity.Comp, true) && !entity.Comp.Denying)
		{
			entity.Comp.DenyEnd = Timing.CurTime + entity.Comp.DenyDelay;
			Audio.PlayPredicted(entity.Comp.SoundDeny, entity.Owner, user, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-2f));
			TryUpdateVisualState(entity);
			((EntitySystem)this).Dirty<VendingMachineComponent>(entity, (MetaDataComponent)null);
		}
	}

	protected virtual void UpdateUI(Entity<VendingMachineComponent?> entity)
	{
	}

	public void TryUpdateVisualState(Entity<VendingMachineComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<VendingMachineComponent>(entity.Owner, ref entity.Comp, true))
		{
			VendingMachineVisualState finalState = VendingMachineVisualState.Normal;
			if (entity.Comp.Broken)
			{
				finalState = VendingMachineVisualState.Broken;
			}
			else if (entity.Comp.Ejecting)
			{
				finalState = VendingMachineVisualState.Eject;
			}
			else if (entity.Comp.Denying)
			{
				finalState = VendingMachineVisualState.Deny;
			}
			else if (!_receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(entity.Owner)))
			{
				finalState = VendingMachineVisualState.Off;
			}
			SharedPointLightComponent pointlight = default(SharedPointLightComponent);
			if (Light.TryGetLight(entity.Owner, ref pointlight))
			{
				bool lightEnabled = finalState != VendingMachineVisualState.Broken && finalState != VendingMachineVisualState.Off;
				Light.SetEnabled(entity.Owner, lightEnabled, pointlight, (MetaDataComponent)null);
			}
			_appearanceSystem.SetData(entity.Owner, (Enum)VendingMachineVisuals.VisualState, (object)finalState, (AppearanceComponent)null);
		}
	}

	public void AuthorizedVend(EntityUid uid, EntityUid sender, InventoryType type, string itemId, VendingMachineComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (IsAuthorized(uid, sender, component))
		{
			TryEjectVendorItem(uid, type, itemId, component.CanShoot, sender, component);
		}
	}

	public void RestockInventoryFromPrototype(EntityUid uid, VendingMachineComponent? component = null, float restockQuality = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		VendingMachineInventoryPrototype packPrototype = default(VendingMachineInventoryPrototype);
		if (((EntitySystem)this).Resolve<VendingMachineComponent>(uid, ref component, true) && PrototypeManager.TryIndex<VendingMachineInventoryPrototype>(component.PackPrototypeId, ref packPrototype))
		{
			AddInventoryFromPrototype(uid, packPrototype.StartingInventory, InventoryType.Regular, component, restockQuality);
			AddInventoryFromPrototype(uid, packPrototype.EmaggedInventory, InventoryType.Emagged, component, restockQuality);
			AddInventoryFromPrototype(uid, packPrototype.ContrabandInventory, InventoryType.Contraband, component, restockQuality);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void OnEmagged(EntityUid uid, VendingMachineComponent component, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_emag.CompareFlag(args.Type, EmagType.Interaction) && !_emag.CheckFlag(uid, EmagType.Interaction))
		{
			args.Handled = component.EmaggedInventory.Count > 0;
		}
	}

	public List<VendingMachineInventoryEntry> GetAllInventory(EntityUid uid, VendingMachineComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VendingMachineComponent>(uid, ref component, true))
		{
			return new List<VendingMachineInventoryEntry>();
		}
		List<VendingMachineInventoryEntry> inventory = new List<VendingMachineInventoryEntry>(component.Inventory.Values);
		if (_emag.CheckFlag(uid, EmagType.Interaction))
		{
			inventory.AddRange(component.EmaggedInventory.Values);
		}
		if (component.Contraband)
		{
			inventory.AddRange(component.ContrabandInventory.Values);
		}
		return inventory;
	}

	public List<VendingMachineInventoryEntry> GetAvailableInventory(EntityUid uid, VendingMachineComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VendingMachineComponent>(uid, ref component, true))
		{
			return new List<VendingMachineInventoryEntry>();
		}
		return (from _ in GetAllInventory(uid, component)
			where _.Amount != 0
			select _).ToList();
	}

	private void AddInventoryFromPrototype(EntityUid uid, Dictionary<string, uint>? entries, InventoryType type, VendingMachineComponent? component = null, float restockQuality = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VendingMachineComponent>(uid, ref component, true) || entries == null)
		{
			return;
		}
		Dictionary<string, VendingMachineInventoryEntry> inventory;
		switch (type)
		{
		default:
			return;
		case InventoryType.Regular:
			inventory = component.Inventory;
			break;
		case InventoryType.Emagged:
			inventory = component.EmaggedInventory;
			break;
		case InventoryType.Contraband:
			inventory = component.ContrabandInventory;
			break;
		}
		foreach (var (id, amount) in entries)
		{
			if (PrototypeManager.HasIndex<EntityPrototype>(id))
			{
				uint restock = amount;
				float chanceOfMissingStock = 1f - restockQuality;
				float result = Randomizer.NextFloat(0f, 1f);
				if (result < chanceOfMissingStock)
				{
					restock = (uint)Math.Floor((float)amount * result / chanceOfMissingStock);
				}
				if (inventory.TryGetValue(id, out var entry))
				{
					entry.Amount = Math.Min(entry.Amount + amount, 3 * restock);
				}
				else
				{
					inventory.Add(id, new VendingMachineInventoryEntry(type, id, restock));
				}
			}
		}
	}

	public bool TryAccessMachine(EntityUid uid, VendingMachineRestockComponent restock, VendingMachineComponent machineComponent, EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		WiresPanelComponent panel = default(WiresPanelComponent);
		if (!((EntitySystem)this).TryComp<WiresPanelComponent>(target, ref panel) || !panel.Open)
		{
			Popup.PopupPredictedCursor(base.Loc.GetString("vending-machine-restock-needs-panel-open", new(string, object)[3]
			{
				("this", uid),
				("user", user),
				("target", target)
			}), user);
			return false;
		}
		return true;
	}

	public bool TryMatchPackageToMachine(EntityUid uid, VendingMachineRestockComponent component, VendingMachineComponent machineComponent, EntityUid user, EntityUid target)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (!component.CanRestock.Contains(machineComponent.PackPrototypeId))
		{
			Popup.PopupPredictedCursor(base.Loc.GetString("vending-machine-restock-invalid-inventory", new(string, object)[3]
			{
				("this", uid),
				("user", user),
				("target", target)
			}), user);
			return false;
		}
		return true;
	}

	private void OnAfterInteract(EntityUid uid, VendingMachineRestockComponent component, AfterInteractEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		VendingMachineComponent machineComponent = default(VendingMachineComponent);
		if (args.CanReach && !((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<VendingMachineComponent>(args.Target, ref machineComponent) && TryMatchPackageToMachine(uid, component, machineComponent, args.User, target2) && TryAccessMachine(uid, component, machineComponent, args.User, target2))
		{
			((HandledEntityEventArgs)args).Handled = true;
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, (float)component.RestockDelay.TotalSeconds, new RestockDoAfterEvent(), target2, target2, uid)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				NeedHand = true
			};
			if (_doAfter.TryStartDoAfter(doAfterArgs))
			{
				string selfMessage = base.Loc.GetString("vending-machine-restock-start-self", (ValueTuple<string, object>)("target", target2));
				string othersMessage = base.Loc.GetString("vending-machine-restock-start-others", (ValueTuple<string, object>)("user", Identity.Entity(args.User, (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("target", target2));
				Popup.PopupPredicted(selfMessage, othersMessage, uid, args.User, PopupType.Medium);
				Audio.PlayPredicted(component.SoundRestockStart, uid, (EntityUid?)args.User, (AudioParams?)null);
			}
		}
	}
}
