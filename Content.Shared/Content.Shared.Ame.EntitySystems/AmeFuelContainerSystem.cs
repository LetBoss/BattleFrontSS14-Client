using System;
using Content.Shared.Ame.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;

namespace Content.Shared.Ame.EntitySystems;

public sealed class AmeFuelContainerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AmeFuelContainerComponent, ExaminedEvent>((ComponentEventHandler<AmeFuelContainerComponent, ExaminedEvent>)OnFuelExamined, (Type[])null, (Type[])null);
	}

	private void OnFuelExamined(EntityUid uid, AmeFuelContainerComponent comp, ExaminedEvent args)
	{
		if (args.IsInDetailsRange)
		{
			bool low = comp.FuelAmount * 4 < comp.FuelCapacity;
			args.PushMarkup(base.Loc.GetString("ame-fuel-container-component-on-examine-detailed-message", new(string, object)[3]
			{
				("colorName", low ? "darkorange" : "orange"),
				("amount", comp.FuelAmount),
				("capacity", comp.FuelCapacity)
			}));
		}
	}
}
