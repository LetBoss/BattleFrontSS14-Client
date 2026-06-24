using System;
using Content.Shared.DragDrop;
using Content.Shared.Kitchen.Components;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Kitchen;

public abstract class SharedKitchenSpikeSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<KitchenSpikeComponent, CanDropTargetEvent>((ComponentEventRefHandler<KitchenSpikeComponent, CanDropTargetEvent>)OnCanDrop, (Type[])null, (Type[])null);
	}

	private void OnCanDrop(EntityUid uid, KitchenSpikeComponent component, ref CanDropTargetEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			args.Handled = true;
			if (!((EntitySystem)this).HasComp<ButcherableComponent>(args.Dragged))
			{
				args.CanDrop = false;
			}
			else
			{
				args.CanDrop = true;
			}
		}
	}
}
