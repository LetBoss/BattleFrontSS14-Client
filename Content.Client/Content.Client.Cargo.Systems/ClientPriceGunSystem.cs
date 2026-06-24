using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Systems;
using Content.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Cargo.Systems;

public sealed class ClientPriceGunSystem : SharedPriceGunSystem
{
	[Dependency]
	private UseDelaySystem _useDelay;

	protected override bool GetPriceOrBounty(Entity<PriceGunComponent> entity, EntityUid target, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent item = default(UseDelayComponent);
		if (!((EntitySystem)this).TryComp<UseDelayComponent>(Entity<PriceGunComponent>.op_Implicit(entity), ref item) || _useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<PriceGunComponent>.op_Implicit(entity), item))))
		{
			return false;
		}
		return true;
	}
}
