using System;
using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class UseDelayOnShootSystem : EntitySystem
{
	[Dependency]
	private UseDelaySystem _delay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<UseDelayOnShootComponent, GunShotEvent>((ComponentEventRefHandler<UseDelayOnShootComponent, GunShotEvent>)OnUseShoot, (Type[])null, (Type[])null);
	}

	private void OnUseShoot(EntityUid uid, UseDelayOnShootComponent component, ref GunShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(uid, ref useDelay))
		{
			_delay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((uid, useDelay)));
		}
	}
}
