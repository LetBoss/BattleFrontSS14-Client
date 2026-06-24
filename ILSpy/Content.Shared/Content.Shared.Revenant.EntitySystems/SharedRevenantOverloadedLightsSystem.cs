using Content.Shared.Revenant.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Revenant.EntitySystems;

public abstract class SharedRevenantOverloadedLightsSystem : EntitySystem
{
	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<RevenantOverloadedLightsComponent> enumerator = ((EntitySystem)this).EntityQueryEnumerator<RevenantOverloadedLightsComponent>();
		EntityUid uid = default(EntityUid);
		RevenantOverloadedLightsComponent comp = default(RevenantOverloadedLightsComponent);
		while (enumerator.MoveNext(ref uid, ref comp))
		{
			comp.Accumulator += frameTime;
			if (!(comp.Accumulator < comp.ZapDelay))
			{
				OnZap(Entity<RevenantOverloadedLightsComponent>.op_Implicit((uid, comp)));
				((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)comp);
			}
		}
	}

	protected abstract void OnZap(Entity<RevenantOverloadedLightsComponent> component);
}
