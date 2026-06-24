using System;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.PneumaticCannon;

public abstract class SharedPneumaticCannonSystem : EntitySystem
{
	[Dependency]
	protected SharedContainerSystem Container;

	[Dependency]
	protected SharedPopupSystem Popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PneumaticCannonComponent, AttemptShootEvent>((ComponentEventRefHandler<PneumaticCannonComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
	}

	private void OnAttemptShoot(EntityUid uid, PneumaticCannonComponent component, ref AttemptShootEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (component.GasUsage == 0f)
		{
			return;
		}
		args.ThrowItems = component.ThrowItems;
		BaseContainer container = default(BaseContainer);
		if (Container.TryGetContainer(uid, "gas_tank", ref container, (ContainerManagerComponent)null))
		{
			ContainerSlot slot = (ContainerSlot)(object)((container is ContainerSlot) ? container : null);
			if (slot != null && slot.ContainedEntity.HasValue)
			{
				return;
			}
		}
		args.Cancelled = true;
	}
}
