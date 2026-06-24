using System;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class BlindfoldSystem : EntitySystem
{
	[Dependency]
	private BlindableSystem _blindableSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BlindfoldComponent, GotEquippedEvent>((EntityEventRefHandler<BlindfoldComponent, GotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlindfoldComponent, GotUnequippedEvent>((EntityEventRefHandler<BlindfoldComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlindfoldComponent, InventoryRelayedEvent<CanSeeAttemptEvent>>((EntityEventRefHandler<BlindfoldComponent, InventoryRelayedEvent<CanSeeAttemptEvent>>)OnBlindfoldTrySee, (Type[])null, (Type[])null);
	}

	private void OnBlindfoldTrySee(Entity<BlindfoldComponent> blindfold, ref InventoryRelayedEvent<CanSeeAttemptEvent> args)
	{
		((CancellableEntityEventArgs)args.Args).Cancel();
	}

	private void OnEquipped(Entity<BlindfoldComponent> blindfold, ref GotEquippedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_blindableSystem.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(args.Equipee));
	}

	private void OnUnequipped(Entity<BlindfoldComponent> blindfold, ref GotUnequippedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_blindableSystem.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(args.Equipee));
	}
}
