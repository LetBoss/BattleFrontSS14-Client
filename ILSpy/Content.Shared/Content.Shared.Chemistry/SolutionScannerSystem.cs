using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry;

public sealed class SolutionScannerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SolutionScannerComponent, SolutionScanEvent>((ComponentEventHandler<SolutionScannerComponent, SolutionScanEvent>)OnSolutionScanAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionScannerComponent, InventoryRelayedEvent<SolutionScanEvent>>((ComponentEventHandler<SolutionScannerComponent, InventoryRelayedEvent<SolutionScanEvent>>)delegate(EntityUid e, SolutionScannerComponent c, InventoryRelayedEvent<SolutionScanEvent> ev)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			OnSolutionScanAttempt(e, c, ev.Args);
		}, (Type[])null, (Type[])null);
	}

	private void OnSolutionScanAttempt(EntityUid eid, SolutionScannerComponent component, SolutionScanEvent args)
	{
		args.CanScan = true;
	}
}
