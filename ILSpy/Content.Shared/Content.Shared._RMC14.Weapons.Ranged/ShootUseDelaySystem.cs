using System;
using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class ShootUseDelaySystem : EntitySystem
{
	[Dependency]
	private UseDelaySystem _useDelay;

	private const string ShootUseDelayId = "CMShootUseDelay";

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ShootUseDelayComponent, GunShotEvent>((EntityEventRefHandler<ShootUseDelayComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
	}

	private void OnGunShot(Entity<ShootUseDelayComponent> ent, ref GunShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent delayComponent = default(UseDelayComponent);
		GunComponent gunComponent = default(GunComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<ShootUseDelayComponent>.op_Implicit(ent), ref delayComponent) && ((EntitySystem)this).TryComp<GunComponent>(Entity<ShootUseDelayComponent>.op_Implicit(ent), ref gunComponent))
		{
			_useDelay.SetLength(Entity<UseDelayComponent>.op_Implicit((ent.Owner, delayComponent)), TimeSpan.FromSeconds(1f / gunComponent.FireRateModified), "CMShootUseDelay");
			_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((ent.Owner, delayComponent)), checkDelayed: true, "CMShootUseDelay");
		}
	}
}
