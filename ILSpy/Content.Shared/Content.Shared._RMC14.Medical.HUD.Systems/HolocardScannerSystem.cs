using System;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared._RMC14.Medical.HUD.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Medical.HUD.Systems;

public sealed class HolocardScannerSystem : EntitySystem
{
	[Dependency]
	private InventorySystem _inventory;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, HolocardScanEvent>((EntityEventRefHandler<InventoryComponent, HolocardScanEvent>)_inventory.RelayEvent<HolocardScanEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<HolocardScannerComponent>>((EntityEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<HolocardScannerComponent>>)_inventory.RelayEvent<RefreshEquipmentHudEvent<HolocardScannerComponent>>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HolocardScannerComponent, HolocardScanEvent>((EntityEventRefHandler<HolocardScannerComponent, HolocardScanEvent>)OnHolocardScanAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HolocardScannerComponent, InventoryRelayedEvent<HolocardScanEvent>>((EntityEventRefHandler<HolocardScannerComponent, InventoryRelayedEvent<HolocardScanEvent>>)OnRelayedHolocardScanAttempt, (Type[])null, (Type[])null);
	}

	private void OnHolocardScanAttempt(Entity<HolocardScannerComponent> ent, ref HolocardScanEvent args)
	{
		args.CanScan = true;
	}

	private void OnRelayedHolocardScanAttempt(Entity<HolocardScannerComponent> ent, ref InventoryRelayedEvent<HolocardScanEvent> args)
	{
		args.Args.CanScan = true;
	}
}
