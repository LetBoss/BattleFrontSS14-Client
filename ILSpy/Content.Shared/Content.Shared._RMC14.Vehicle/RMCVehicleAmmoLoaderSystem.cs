using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Vehicle.Weapons;
using Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Vehicle;

public sealed class RMCVehicleAmmoLoaderSystem : EntitySystem
{
	[Dependency]
	private BulletBoxSystem _bulletBox;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private ItemSlotsSystem _itemSlots;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private VehicleSystem _vehicleSystem;

	private readonly Dictionary<EntityUid, Dictionary<EntityUid, EntityUid>> _activeAmmoBoxes = new Dictionary<EntityUid, Dictionary<EntityUid, EntityUid>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, InteractUsingEvent>((EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, VehicleAmmoLoaderDoAfterEvent>((EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, VehicleAmmoLoaderDoAfterEvent>)OnLoadDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, BoundUIOpenedEvent>((EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, BoundUIOpenedEvent>)OnUiOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, BoundUIClosedEvent>((EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, BoundUIClosedEvent>)OnUiClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, RMCVehicleAmmoLoaderSelectMessage>((EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, RMCVehicleAmmoLoaderSelectMessage>)OnUiSelect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, RMCVehicleAmmoLoaderRefreshMessage>((EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, RMCVehicleAmmoLoaderRefreshMessage>)OnUiRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, ShellTypeSelectMessage>((EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, ShellTypeSelectMessage>)OnShellTypeSelect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, ShellSelectionCancelMessage>((EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, ShellSelectionCancelMessage>)OnShellSelectionCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, ComponentShutdown>((EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, ComponentShutdown>)OnLoaderShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ShellSelectionComponent, PlayerDetachedEvent>((EntityEventRefHandler<ShellSelectionComponent, PlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(Entity<RMCVehicleAmmoLoaderComponent> ent, ref InteractUsingEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		BulletBoxComponent box = default(BulletBoxComponent);
		UserInterfaceComponent uiComp = default(UserInterfaceComponent);
		if (((HandledEntityEventArgs)args).Handled || _net.IsClient || !((EntitySystem)this).TryComp<BulletBoxComponent>(args.Used, ref box) || !((EntitySystem)this).TryComp<UserInterfaceComponent>(ent.Owner, ref uiComp) || !_ui.HasUi(ent.Owner, (Enum)RMCVehicleAmmoLoaderUiKey.Key, uiComp))
		{
			return;
		}
		if (!_vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicleUid) || !vehicleUid.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-no-vehicle"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), args.User);
			return;
		}
		if (box.Amount <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-empty", (ValueTuple<string, object>)("box", args.Used)), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), args.User);
			return;
		}
		if (ent.Comp.BulletType.HasValue)
		{
			EntProtoId? bulletType = ent.Comp.BulletType;
			EntProtoId bulletType2 = box.BulletType;
			if (!bulletType.HasValue || bulletType.GetValueOrDefault() != bulletType2)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-wrong-ammo"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), args.User);
				return;
			}
		}
		HardpointSlotsComponent hardpoints = default(HardpointSlotsComponent);
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(vehicleUid.Value, ref hardpoints) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(vehicleUid.Value, ref itemSlots))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-no-hardpoint"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), args.User);
			return;
		}
		TryFindAmmoProvider(vehicleUid.Value, hardpoints, itemSlots, ent.Comp, box, null, out EntityUid _, out BallisticAmmoProviderComponent _, out RMCVehicleHardpointAmmoComponent _);
		if (!CanOpenUi(ent.Owner, args.User))
		{
			return;
		}
		TrySetActiveAmmoBox(ent.Owner, args.User, args.Used);
		if (ent.Comp.EnableShellSelection)
		{
			if (!CanOpenShellSelectionUi(ent.Owner, args.User))
			{
				return;
			}
			if (!_ui.HasUi(ent.Owner, (Enum)ShellSelectionUiKey.Key, uiComp))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-shell-selection-unavailable"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), args.User);
				return;
			}
			ShellSelectionComponent shellSelection = default(ShellSelectionComponent);
			if (!((EntitySystem)this).TryComp<ShellSelectionComponent>(ent.Owner, ref shellSelection))
			{
				shellSelection = ((EntitySystem)this).AddComp<ShellSelectionComponent>(ent.Owner);
			}
			if (shellSelection.AvailableShells.Count == 0)
			{
				PopulateAvailableShells(Entity<ShellSelectionComponent>.op_Implicit((ent.Owner, shellSelection)));
			}
			if (ent.Comp.SelectedShellType.HasValue)
			{
				shellSelection.SelectedShellType = ent.Comp.SelectedShellType.Value;
				((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)shellSelection, (MetaDataComponent)null);
			}
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ShellSelectionUiKey.Key, (EntityUid?)args.User, false);
			UpdateShellSelectionUi(ent.Owner, args.User, shellSelection);
		}
		else
		{
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCVehicleAmmoLoaderUiKey.Key, (EntityUid?)args.User, false);
			UpdateUi(ent.Owner, box);
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnLoadDoAfter(Entity<RMCVehicleAmmoLoaderComponent> ent, ref VehicleAmmoLoaderDoAfterEvent args)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? used = args.Used;
		if (!used.HasValue)
		{
			return;
		}
		EntityUid used2 = used.GetValueOrDefault();
		BulletBoxComponent box = default(BulletBoxComponent);
		if (!((EntitySystem)this).TryComp<BulletBoxComponent>(used2, ref box) || !TryGetActiveAmmoBox(ent.Owner, args.User, out var activeBox) || activeBox != used2 || string.IsNullOrWhiteSpace(args.SlotId) || !CanLoad(ent, args.User, Entity<BulletBoxComponent>.op_Implicit((used2, box)), args.SlotId, out EntityUid _, out EntityUid ammoUid, out BallisticAmmoProviderComponent ammo, out RMCVehicleHardpointAmmoComponent hardpointAmmo))
		{
			return;
		}
		int magazineSize = Math.Max(1, hardpointAmmo.MagazineSize);
		if (box.Amount < magazineSize)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-not-enough"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), args.User);
		}
		else
		{
			if (!_bulletBox.TryConsume(Entity<BulletBoxComponent>.op_Implicit((used2, box)), magazineSize))
			{
				return;
			}
			int chambered = ammo.Count;
			EntProtoId? projectileProto = null;
			RefillableByBulletBoxComponent refill = default(RefillableByBulletBoxComponent);
			if (((EntitySystem)this).TryComp<RefillableByBulletBoxComponent>(ammoUid, ref refill) && !refill.BulletType.HasValue)
			{
				projectileProto = GetProjectileProtoFromShell(box.BulletType);
				if (!projectileProto.HasValue)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-invalid-shell"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), args.User);
					return;
				}
			}
			else
			{
				projectileProto = ammo.Proto;
			}
			if (chambered == 0)
			{
				if (projectileProto.HasValue)
				{
					ammo.Proto = projectileProto;
					((EntitySystem)this).Dirty(ammoUid, (IComponent)(object)ammo, (MetaDataComponent)null);
				}
				int chamberSize = Math.Min(magazineSize, ammo.Capacity);
				_gun.SetBallisticUnspawned(Entity<BallisticAmmoProviderComponent>.op_Implicit((ammoUid, ammo)), chamberSize);
			}
			else
			{
				hardpointAmmo.StoredMagazines++;
				if (projectileProto.HasValue)
				{
					hardpointAmmo.MagazineProjectileQueue.Enqueue(projectileProto.Value);
				}
				((EntitySystem)this).Dirty(ammoUid, (IComponent)(object)hardpointAmmo, (MetaDataComponent)null);
			}
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-loaded", (ValueTuple<string, object>)("amount", magazineSize), (ValueTuple<string, object>)("target", ammoUid)), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), args.User);
			UpdateUi(ent.Owner, box);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private EntProtoId? GetProjectileProtoFromShell(EntProtoId? shellType)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!shellType.HasValue)
		{
			return null;
		}
		EntProtoId value = shellType.Value;
		return EntProtoId.op_Implicit(((EntProtoId)(ref value)).Id switch
		{
			"PUBGVehicleAmmoBoxTankAPFSDS" => "PUBGVehicleCartridgeTankAPFSDS", 
			"PUBGVehicleAmmoBoxTankHE" => "PUBGVehicleCartridgeTankHE", 
			"PUBGVehicleAmmoBoxTankHEAT" => "PUBGVehicleCartridgeTankHEAT", 
			_ => null, 
		});
	}

	private string? GetLoadedAmmoTypeName(BallisticAmmoProviderComponent ammo, RefillableByBulletBoxComponent? refill)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId value;
		if ((refill == null || !refill.BulletType.HasValue) && ammo.Proto.HasValue)
		{
			value = ammo.Proto.Value;
			return ((EntProtoId)(ref value)).Id switch
			{
				"PUBGVehicleCartridgeTankAPFSDS" => "APFSDS", 
				"PUBGVehicleCartridgeTankHE" => "HE", 
				"PUBGVehicleCartridgeTankHEAT" => "HEAT", 
				_ => null, 
			};
		}
		if (refill != null && refill.BulletType.HasValue)
		{
			value = refill.BulletType.Value;
			string id = ((EntProtoId)(ref value)).Id;
			if (!(id == "PubgMagazineM2"))
			{
				if (id == "PubgMagazineKord")
				{
					return "KORD Magazine";
				}
				value = refill.BulletType.Value;
				return ((EntProtoId)(ref value)).Id;
			}
			return "M2 Magazine";
		}
		return null;
	}

	private void OnUiOpened(Entity<RMCVehicleAmmoLoaderComponent> ent, ref BoundUIOpenedEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		BulletBoxComponent box = default(BulletBoxComponent);
		if (object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, RMCVehicleAmmoLoaderUiKey.Key) && !_net.IsClient && TryGetActiveAmmoBox(ent.Owner, ((BaseBoundUserInterfaceEvent)args).Actor, out var boxUid) && ((EntitySystem)this).TryComp<BulletBoxComponent>(boxUid, ref box))
		{
			UpdateUi(ent.Owner, box);
		}
	}

	private void OnUiClosed(Entity<RMCVehicleAmmoLoaderComponent> ent, ref BoundUIClosedEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, RMCVehicleAmmoLoaderUiKey.Key))
		{
			ClearActiveAmmoBox(ent.Owner, ((BaseBoundUserInterfaceEvent)args).Actor);
		}
	}

	private void OnUiSelect(Entity<RMCVehicleAmmoLoaderComponent> ent, ref RMCVehicleAmmoLoaderSelectMessage args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		if (!object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, RMCVehicleAmmoLoaderUiKey.Key) || ((BaseBoundUserInterfaceEvent)args).Actor == default(EntityUid) || !((EntitySystem)this).Exists(((BaseBoundUserInterfaceEvent)args).Actor) || !_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor), out var activeItem))
		{
			return;
		}
		if (TryGetActiveAmmoBox(ent.Owner, ((BaseBoundUserInterfaceEvent)args).Actor, out var boxUid))
		{
			EntityUid? val = activeItem;
			EntityUid vehicle = boxUid;
			if (val.HasValue && !(val.GetValueOrDefault() != vehicle))
			{
				BulletBoxComponent box = default(BulletBoxComponent);
				if (((EntitySystem)this).TryComp<BulletBoxComponent>(boxUid, ref box) && CanLoad(ent, ((BaseBoundUserInterfaceEvent)args).Actor, Entity<BulletBoxComponent>.op_Implicit((boxUid, box)), args.SlotId, out vehicle, out EntityUid ammoUid, out BallisticAmmoProviderComponent ammo, out RMCVehicleHardpointAmmoComponent hardpointAmmo))
				{
					int magazineSize = Math.Max(1, hardpointAmmo.MagazineSize);
					int chambered = ammo.Count;
					bool canStore = hardpointAmmo.StoredMagazines < hardpointAmmo.MaxStoredMagazines;
					if (box.Amount < magazineSize)
					{
						_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-not-enough"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
						return;
					}
					if (chambered > 0 && !canStore)
					{
						_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-full", (ValueTuple<string, object>)("target", ammoUid)), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
						return;
					}
					DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, ((BaseBoundUserInterfaceEvent)args).Actor, ent.Comp.LoadDelay, new VehicleAmmoLoaderDoAfterEvent(args.SlotId), ent.Owner, ent.Owner, boxUid)
					{
						BreakOnMove = true,
						BreakOnDropItem = true,
						NeedHand = true
					};
					_doAfter.TryStartDoAfter(doAfter);
				}
				return;
			}
		}
		_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-hold-ammo"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
	}

	private void OnUiRefresh(Entity<RMCVehicleAmmoLoaderComponent> ent, ref RMCVehicleAmmoLoaderRefreshMessage args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, RMCVehicleAmmoLoaderUiKey.Key) && !_net.IsClient && !(((BaseBoundUserInterfaceEvent)args).Actor == default(EntityUid)) && ((EntitySystem)this).Exists(((BaseBoundUserInterfaceEvent)args).Actor))
		{
			BulletBoxComponent box = default(BulletBoxComponent);
			EntityUid? activeItem;
			BulletBoxComponent activeBox = default(BulletBoxComponent);
			if (TryGetActiveAmmoBox(ent.Owner, ((BaseBoundUserInterfaceEvent)args).Actor, out var boxUid) && ((EntitySystem)this).TryComp<BulletBoxComponent>(boxUid, ref box))
			{
				UpdateUi(ent.Owner, box);
			}
			else if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor), out activeItem) && ((EntitySystem)this).TryComp<BulletBoxComponent>(activeItem, ref activeBox))
			{
				TrySetActiveAmmoBox(ent.Owner, ((BaseBoundUserInterfaceEvent)args).Actor, activeItem.Value);
				UpdateUi(ent.Owner, activeBox);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-hold-ammo"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
			}
		}
	}

	private void OnShellTypeSelect(Entity<RMCVehicleAmmoLoaderComponent> ent, ref ShellTypeSelectMessage args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		if (!object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, ShellSelectionUiKey.Key) || _net.IsClient || ((BaseBoundUserInterfaceEvent)args).Actor == default(EntityUid) || !((EntitySystem)this).Exists(((BaseBoundUserInterfaceEvent)args).Actor))
		{
			return;
		}
		if (!_prototypes.HasIndex<EntityPrototype>(EntProtoId.op_Implicit(args.SelectedShellType)))
		{
			((EntitySystem)this).Log.Error($"Invalid shell type selected: {args.SelectedShellType} by user {((BaseBoundUserInterfaceEvent)args).Actor}. Defaulting to APFSDS.");
			_popup.PopupClient(base.Loc.GetString("rmc-shell-selection-invalid"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
			string defaultShell = "PUBGVehicleAmmoBoxTankAPFSDS";
			if (!_prototypes.HasIndex<EntityPrototype>(defaultShell))
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ShellSelectionUiKey.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
				return;
			}
			args = new ShellTypeSelectMessage(EntProtoId.op_Implicit(defaultShell));
		}
		if (!IsShellAvailable(((BaseBoundUserInterfaceEvent)args).Actor, args.SelectedShellType))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-shell-selection-unavailable"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ShellSelectionUiKey.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
			return;
		}
		ShellSelectionComponent shellSelection = default(ShellSelectionComponent);
		if (((EntitySystem)this).TryComp<ShellSelectionComponent>(ent.Owner, ref shellSelection))
		{
			shellSelection.SelectedShellType = args.SelectedShellType;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)shellSelection, (MetaDataComponent)null);
		}
		ent.Comp.SelectedShellType = args.SelectedShellType;
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		EntityUid? spawnedShell = SpawnShellInHands(((BaseBoundUserInterfaceEvent)args).Actor, args.SelectedShellType);
		if (!spawnedShell.HasValue)
		{
			((EntitySystem)this).Log.Error($"Failed to spawn shell {args.SelectedShellType} for user {((BaseBoundUserInterfaceEvent)args).Actor}");
			_popup.PopupClient(base.Loc.GetString("rmc-shell-selection-invalid"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ShellSelectionUiKey.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
		}
		else
		{
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ShellSelectionUiKey.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
			BulletBoxComponent box = default(BulletBoxComponent);
			if (((EntitySystem)this).TryComp<BulletBoxComponent>(spawnedShell.Value, ref box))
			{
				_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCVehicleAmmoLoaderUiKey.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
				UpdateUi(ent.Owner, box);
			}
		}
	}

	private void OnShellSelectionCancel(Entity<RMCVehicleAmmoLoaderComponent> ent, ref ShellSelectionCancelMessage args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, ShellSelectionUiKey.Key) && !_net.IsClient && !(((BaseBoundUserInterfaceEvent)args).Actor == default(EntityUid)) && ((EntitySystem)this).Exists(((BaseBoundUserInterfaceEvent)args).Actor))
		{
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ShellSelectionUiKey.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
		}
	}

	private bool CanOpenUi(EntityUid loader, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		using (IEnumerator<EntityUid> enumerator = _ui.GetActors(Entity<UserInterfaceComponent>.op_Implicit(loader), (Enum)RMCVehicleAmmoLoaderUiKey.Key).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				if (enumerator.Current == user)
				{
					return true;
				}
				_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-in-use"), loader, user);
				return false;
			}
		}
		return true;
	}

	private bool TrySetActiveAmmoBox(EntityUid loader, EntityUid user, EntityUid boxUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!_activeAmmoBoxes.TryGetValue(loader, out Dictionary<EntityUid, EntityUid> userBoxes))
		{
			userBoxes = new Dictionary<EntityUid, EntityUid>();
			_activeAmmoBoxes[loader] = userBoxes;
		}
		userBoxes[user] = boxUid;
		return true;
	}

	private bool TryGetActiveAmmoBox(EntityUid loader, EntityUid user, out EntityUid boxUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		boxUid = default(EntityUid);
		if (_activeAmmoBoxes.TryGetValue(loader, out Dictionary<EntityUid, EntityUid> userBoxes))
		{
			return userBoxes.TryGetValue(user, out boxUid);
		}
		return false;
	}

	private void ClearActiveAmmoBox(EntityUid loader, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (_activeAmmoBoxes.TryGetValue(loader, out Dictionary<EntityUid, EntityUid> userBoxes))
		{
			userBoxes.Remove(user);
			if (userBoxes.Count == 0)
			{
				_activeAmmoBoxes.Remove(loader);
			}
		}
	}

	private bool CanLoad(Entity<RMCVehicleAmmoLoaderComponent> loader, EntityUid user, Entity<BulletBoxComponent> box, string? slotId, out EntityUid vehicle, out EntityUid ammoUid, out BallisticAmmoProviderComponent ammo, out RMCVehicleHardpointAmmoComponent hardpointAmmo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		ammoUid = default(EntityUid);
		ammo = null;
		hardpointAmmo = null;
		vehicle = default(EntityUid);
		if (!_vehicleSystem.TryGetVehicleFromInterior(loader.Owner, out var vehicleUid) || !vehicleUid.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-no-vehicle"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(loader), user);
			return false;
		}
		vehicle = vehicleUid.Value;
		if (box.Comp.Amount <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-empty", (ValueTuple<string, object>)("box", box.Owner)), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(loader), user);
			return false;
		}
		if (loader.Comp.BulletType.HasValue)
		{
			EntProtoId? bulletType = loader.Comp.BulletType;
			EntProtoId bulletType2 = box.Comp.BulletType;
			if (!bulletType.HasValue || bulletType.GetValueOrDefault() != bulletType2)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-wrong-ammo"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(loader), user);
				return false;
			}
		}
		HardpointSlotsComponent hardpoints = default(HardpointSlotsComponent);
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(vehicle, ref hardpoints) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(vehicle, ref itemSlots))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-no-hardpoint"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(loader), user);
			return false;
		}
		if (!TryFindAmmoProvider(vehicle, hardpoints, itemSlots, loader.Comp, box.Comp, slotId, out ammoUid, out ammo, out hardpointAmmo))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-no-hardpoint"), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(loader), user);
			return false;
		}
		int count = ammo.Count;
		bool canStore = hardpointAmmo.StoredMagazines < hardpointAmmo.MaxStoredMagazines;
		if (count > 0 && !canStore)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-ammo-loader-full", (ValueTuple<string, object>)("target", ammoUid)), Entity<RMCVehicleAmmoLoaderComponent>.op_Implicit(loader), user);
			return false;
		}
		return true;
	}

	private bool TryFindAmmoProvider(EntityUid vehicle, HardpointSlotsComponent hardpoints, ItemSlotsComponent itemSlots, RMCVehicleAmmoLoaderComponent loader, BulletBoxComponent box, string? slotId, out EntityUid ammoUid, out BallisticAmmoProviderComponent ammo, out RMCVehicleHardpointAmmoComponent hardpointAmmo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		ammoUid = default(EntityUid);
		ammo = null;
		hardpointAmmo = null;
		if (!string.IsNullOrWhiteSpace(slotId) && VehicleTurretSlotIds.TryParse(slotId, out string parentSlotId, out string childSlotId))
		{
			if (!TryGetTurretSlot(vehicle, parentSlotId, childSlotId, itemSlots, out HardpointSlot turretSlot, out EntityUid item))
			{
				return false;
			}
			if (!string.IsNullOrWhiteSpace(loader.HardpointType) && !string.Equals(turretSlot.HardpointType, loader.HardpointType, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			return TryGetAmmoProviderFromItem(item, box, out ammoUid, out ammo, out hardpointAmmo);
		}
		HardpointSlotsComponent turretSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent turretItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id) || (!string.IsNullOrWhiteSpace(slotId) && !string.Equals(slot.Id, slotId, StringComparison.OrdinalIgnoreCase)) || !_itemSlots.TryGetSlot(vehicle, slot.Id, out ItemSlot itemSlot, itemSlots) || !itemSlot.HasItem)
			{
				continue;
			}
			EntityUid item2 = itemSlot.Item.Value;
			if ((string.IsNullOrWhiteSpace(loader.HardpointType) || string.Equals(slot.HardpointType, loader.HardpointType, StringComparison.OrdinalIgnoreCase)) && TryGetAmmoProviderFromItem(item2, box, out ammoUid, out ammo, out hardpointAmmo))
			{
				return true;
			}
			if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(item2, ref turretSlots) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(item2, ref turretItemSlots))
			{
				continue;
			}
			foreach (HardpointSlot turretSlot2 in turretSlots.Slots)
			{
				if (!string.IsNullOrWhiteSpace(turretSlot2.Id) && (string.IsNullOrWhiteSpace(loader.HardpointType) || string.Equals(turretSlot2.HardpointType, loader.HardpointType, StringComparison.OrdinalIgnoreCase)) && _itemSlots.TryGetSlot(item2, turretSlot2.Id, out ItemSlot turretItemSlot, turretItemSlots) && turretItemSlot.HasItem)
				{
					EntityUid turretItem = turretItemSlot.Item.Value;
					if (TryGetAmmoProviderFromItem(turretItem, box, out ammoUid, out ammo, out hardpointAmmo))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void UpdateUi(EntityUid loader, BulletBoxComponent box)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		RMCVehicleAmmoLoaderComponent loaderComp = default(RMCVehicleAmmoLoaderComponent);
		HardpointSlotsComponent hardpoints = default(HardpointSlotsComponent);
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp<RMCVehicleAmmoLoaderComponent>(loader, ref loaderComp) || !_vehicleSystem.TryGetVehicleFromInterior(loader, out var vehicleUid) || !vehicleUid.HasValue || !((EntitySystem)this).TryComp<HardpointSlotsComponent>(vehicleUid.Value, ref hardpoints) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(vehicleUid.Value, ref itemSlots))
		{
			return;
		}
		List<RMCVehicleAmmoLoaderUiEntry> entries = new List<RMCVehicleAmmoLoaderUiEntry>(hardpoints.Slots.Count);
		RefillableByBulletBoxComponent refill = default(RefillableByBulletBoxComponent);
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (!string.IsNullOrWhiteSpace(slot.Id) && _itemSlots.TryGetSlot(vehicleUid.Value, slot.Id, out ItemSlot itemSlot, itemSlots) && itemSlot.HasItem)
			{
				EntityUid item = itemSlot.Item.Value;
				if ((string.IsNullOrWhiteSpace(loaderComp.HardpointType) || string.Equals(slot.HardpointType, loaderComp.HardpointType, StringComparison.OrdinalIgnoreCase)) && TryGetAmmoProviderFromItem(item, box, out EntityUid _, out BallisticAmmoProviderComponent ammoProvider, out RMCVehicleHardpointAmmoComponent hardpointAmmo))
				{
					int chambered = ammoProvider.Count;
					int magazineSize = Math.Max(1, hardpointAmmo.MagazineSize);
					bool canLoad = box.Amount >= magazineSize && (chambered == 0 || hardpointAmmo.StoredMagazines < hardpointAmmo.MaxStoredMagazines);
					((EntitySystem)this).TryComp<RefillableByBulletBoxComponent>(item, ref refill);
					string loadedAmmoType = GetLoadedAmmoTypeName(ammoProvider, refill);
					string name = ((EntitySystem)this).Name(item, (MetaDataComponent)null);
					entries.Add(new RMCVehicleAmmoLoaderUiEntry(slot.Id, slot.HardpointType, name, ((EntitySystem)this).GetNetEntity(item, (MetaDataComponent)null), chambered, magazineSize, hardpointAmmo.StoredMagazines, hardpointAmmo.MaxStoredMagazines, canLoad, loadedAmmoType));
				}
				AppendTurretAmmoEntries(entries, item, slot.Id, loaderComp, box);
			}
		}
		_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(loader), (Enum)RMCVehicleAmmoLoaderUiKey.Key, (BoundUserInterfaceState)(object)new RMCVehicleAmmoLoaderUiState(entries, box.Amount, box.Max));
	}

	private void AppendTurretAmmoEntries(List<RMCVehicleAmmoLoaderUiEntry> entries, EntityUid turretUid, string parentSlotId, RMCVehicleAmmoLoaderComponent loaderComp, BulletBoxComponent box)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		HardpointSlotsComponent turretSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent turretItemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(turretUid, ref turretSlots) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(turretUid, ref turretItemSlots))
		{
			return;
		}
		RefillableByBulletBoxComponent refill = default(RefillableByBulletBoxComponent);
		foreach (HardpointSlot turretSlot in turretSlots.Slots)
		{
			if (!string.IsNullOrWhiteSpace(turretSlot.Id) && (string.IsNullOrWhiteSpace(loaderComp.HardpointType) || string.Equals(turretSlot.HardpointType, loaderComp.HardpointType, StringComparison.OrdinalIgnoreCase)) && _itemSlots.TryGetSlot(turretUid, turretSlot.Id, out ItemSlot turretItemSlot, turretItemSlots) && turretItemSlot.HasItem)
			{
				EntityUid item = turretItemSlot.Item.Value;
				if (TryGetAmmoProviderFromItem(item, box, out EntityUid _, out BallisticAmmoProviderComponent ammoProvider, out RMCVehicleHardpointAmmoComponent hardpointAmmo))
				{
					int chambered = ammoProvider.Count;
					int magazineSize = Math.Max(1, hardpointAmmo.MagazineSize);
					bool canLoad = box.Amount >= magazineSize && (chambered == 0 || hardpointAmmo.StoredMagazines < hardpointAmmo.MaxStoredMagazines);
					((EntitySystem)this).TryComp<RefillableByBulletBoxComponent>(item, ref refill);
					string loadedAmmoType = GetLoadedAmmoTypeName(ammoProvider, refill);
					entries.Add(new RMCVehicleAmmoLoaderUiEntry(VehicleTurretSlotIds.Compose(parentSlotId, turretSlot.Id), turretSlot.HardpointType, ((EntitySystem)this).Name(item, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(item, (MetaDataComponent)null), chambered, magazineSize, hardpointAmmo.StoredMagazines, hardpointAmmo.MaxStoredMagazines, canLoad, loadedAmmoType));
				}
			}
		}
	}

	private bool TryGetTurretSlot(EntityUid vehicle, string parentSlotId, string childSlotId, ItemSlotsComponent itemSlots, out HardpointSlot turretSlot, out EntityUid item)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		turretSlot = null;
		item = default(EntityUid);
		if (!_itemSlots.TryGetSlot(vehicle, parentSlotId, out ItemSlot parentSlot, itemSlots) || !parentSlot.HasItem)
		{
			return false;
		}
		EntityUid turretUid = parentSlot.Item.Value;
		HardpointSlotsComponent turretSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent turretItemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(turretUid, ref turretSlots) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(turretUid, ref turretItemSlots))
		{
			return false;
		}
		foreach (HardpointSlot slot in turretSlots.Slots)
		{
			if (string.Equals(slot.Id, childSlotId, StringComparison.OrdinalIgnoreCase))
			{
				turretSlot = slot;
				if (!_itemSlots.TryGetSlot(turretUid, slot.Id, out ItemSlot turretItemSlot, turretItemSlots) || !turretItemSlot.HasItem)
				{
					return false;
				}
				item = turretItemSlot.Item.Value;
				return true;
			}
		}
		return false;
	}

	private bool TryGetAmmoProviderFromItem(EntityUid item, BulletBoxComponent box, out EntityUid ammoUid, out BallisticAmmoProviderComponent ammo, out RMCVehicleHardpointAmmoComponent hardpointAmmo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		ammoUid = default(EntityUid);
		ammo = null;
		hardpointAmmo = null;
		BallisticAmmoProviderComponent ammoProvider = default(BallisticAmmoProviderComponent);
		if (!((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(item, ref ammoProvider))
		{
			return false;
		}
		RMCVehicleHardpointAmmoComponent hardpointAmmoComp = default(RMCVehicleHardpointAmmoComponent);
		if (!((EntitySystem)this).TryComp<RMCVehicleHardpointAmmoComponent>(item, ref hardpointAmmoComp))
		{
			return false;
		}
		RefillableByBulletBoxComponent refill = default(RefillableByBulletBoxComponent);
		if (!((EntitySystem)this).TryComp<RefillableByBulletBoxComponent>(item, ref refill))
		{
			return false;
		}
		if (refill.BulletType.HasValue)
		{
			EntProtoId? bulletType = refill.BulletType;
			EntProtoId bulletType2 = box.BulletType;
			if (!bulletType.HasValue || bulletType.GetValueOrDefault() != bulletType2)
			{
				return false;
			}
		}
		ammoUid = item;
		ammo = ammoProvider;
		hardpointAmmo = hardpointAmmoComp;
		return true;
	}

	public void PopulateAvailableShells(Entity<ShellSelectionComponent> shellSelection)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		shellSelection.Comp.AvailableShells.Clear();
		(string, string, string, string)[] array = new(string, string, string, string)[3]
		{
			("PUBGVehicleAmmoBoxTankAPFSDS", "APFSDS Shell", "Armor-Piercing Fin-Stabilized Discarding Sabot shell with high penetration", "APFSDS"),
			("PUBGVehicleAmmoBoxTankHE", "HE Shell", "High Explosive shell with area damage and shrapnel", "HE"),
			("PUBGVehicleAmmoBoxTankHEAT", "HEAT Shell", "High Explosive Anti-Tank shaped charge shell", "HEAT")
		};
		for (int i = 0; i < array.Length; i++)
		{
			var (protoId, name, description, spriteState) = array[i];
			if (_prototypes.HasIndex<EntityPrototype>(protoId))
			{
				shellSelection.Comp.AvailableShells.Add(new ShellTypeInfo
				{
					ProtoId = EntProtoId.op_Implicit(protoId),
					Name = name,
					Description = description,
					SpriteState = spriteState
				});
			}
		}
		((EntitySystem)this).Dirty<ShellSelectionComponent>(shellSelection, (MetaDataComponent)null);
	}

	private void UpdateShellSelectionUi(EntityUid loader, EntityUid user, ShellSelectionComponent shellSelection)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		List<ShellTypeEntry> entries = new List<ShellTypeEntry>();
		foreach (ShellTypeInfo shell in shellSelection.AvailableShells)
		{
			bool isAvailable = IsShellAvailable(user, shell.ProtoId);
			entries.Add(new ShellTypeEntry(shell.ProtoId, shell.Name, shell.Description, isAvailable));
		}
		ShellSelectionUiState state = new ShellSelectionUiState(entries, shellSelection.SelectedShellType);
		_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(loader), (Enum)ShellSelectionUiKey.Key, (BoundUserInterfaceState)(object)state);
	}

	public bool IsShellAvailable(EntityUid user, EntProtoId shellProtoId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var activeItem))
		{
			return false;
		}
		BulletBoxComponent box = default(BulletBoxComponent);
		if (!((EntitySystem)this).TryComp<BulletBoxComponent>(activeItem, ref box))
		{
			return false;
		}
		if (box.BulletType != shellProtoId)
		{
			return false;
		}
		return box.Amount > 0;
	}

	public EntityUid? SpawnShellInHands(EntityUid user, EntProtoId shellProtoId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!_prototypes.HasIndex<EntityPrototype>(EntProtoId.op_Implicit(shellProtoId)))
		{
			return null;
		}
		EntityCoordinates coordinates = ((EntitySystem)this).Transform(user).Coordinates;
		EntityUid shell = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(shellProtoId), coordinates);
		if (!_hands.TryPickupAnyHand(user, shell))
		{
			((EntitySystem)this).QueueDel((EntityUid?)shell);
			return null;
		}
		return shell;
	}

	private bool CanOpenShellSelectionUi(EntityUid loader, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		using (IEnumerator<EntityUid> enumerator = _ui.GetActors(Entity<UserInterfaceComponent>.op_Implicit(loader), (Enum)ShellSelectionUiKey.Key).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				if (enumerator.Current == user)
				{
					return true;
				}
				_popup.PopupClient(base.Loc.GetString("rmc-shell-selection-in-use"), loader, user);
				return false;
			}
		}
		return true;
	}

	private void OnPlayerDetached(Entity<ShellSelectionComponent> ent, ref PlayerDetachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		RMCVehicleAmmoLoaderComponent loaderComp = default(RMCVehicleAmmoLoaderComponent);
		UserInterfaceComponent uiComp = default(UserInterfaceComponent);
		if (((EntitySystem)this).TryComp<RMCVehicleAmmoLoaderComponent>(ent.Owner, ref loaderComp) && ((EntitySystem)this).TryComp<UserInterfaceComponent>(ent.Owner, ref uiComp))
		{
			if (_ui.HasUi(ent.Owner, (Enum)ShellSelectionUiKey.Key, uiComp))
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ShellSelectionUiKey.Key, args.Player.AttachedEntity, false);
			}
			if (_ui.HasUi(ent.Owner, (Enum)RMCVehicleAmmoLoaderUiKey.Key, uiComp))
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCVehicleAmmoLoaderUiKey.Key, args.Player.AttachedEntity, false);
			}
			if (args.Player.AttachedEntity.HasValue)
			{
				ClearActiveAmmoBox(ent.Owner, args.Player.AttachedEntity.Value);
			}
		}
	}

	private void OnLoaderShutdown(Entity<RMCVehicleAmmoLoaderComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		UserInterfaceComponent uiComp = default(UserInterfaceComponent);
		if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(ent.Owner, ref uiComp))
		{
			return;
		}
		if (_ui.HasUi(ent.Owner, (Enum)ShellSelectionUiKey.Key, uiComp))
		{
			EntityUid[] array = _ui.GetActors(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ShellSelectionUiKey.Key).ToArray();
			foreach (EntityUid actor in array)
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ShellSelectionUiKey.Key, (EntityUid?)actor, false);
			}
		}
		if (_ui.HasUi(ent.Owner, (Enum)RMCVehicleAmmoLoaderUiKey.Key, uiComp))
		{
			EntityUid[] array = _ui.GetActors(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCVehicleAmmoLoaderUiKey.Key).ToArray();
			foreach (EntityUid actor2 in array)
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCVehicleAmmoLoaderUiKey.Key, (EntityUid?)actor2, false);
			}
		}
		if (_activeAmmoBoxes.ContainsKey(ent.Owner))
		{
			_activeAmmoBoxes.Remove(ent.Owner);
		}
	}
}
