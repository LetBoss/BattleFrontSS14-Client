using System;
using Content.Shared.Interaction;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class RechargeCycleAmmoSystem : EntitySystem
{
	[Dependency]
	private SharedGunSystem _gun;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RechargeCycleAmmoComponent, ActivateInWorldEvent>((ComponentEventHandler<RechargeCycleAmmoComponent, ActivateInWorldEvent>)OnRechargeCycled, (Type[])null, (Type[])null);
	}

	private void OnRechargeCycled(EntityUid uid, RechargeCycleAmmoComponent component, ActivateInWorldEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		BasicEntityAmmoProviderComponent basic = default(BasicEntityAmmoProviderComponent);
		if (args.Complex && ((EntitySystem)this).TryComp<BasicEntityAmmoProviderComponent>(uid, ref basic) && !((HandledEntityEventArgs)args).Handled && !(basic.Count >= basic.Capacity) && basic.Count.HasValue)
		{
			_gun.UpdateBasicEntityAmmoCount(uid, basic.Count.Value + 1, basic);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)basic, (MetaDataComponent)null);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}
}
