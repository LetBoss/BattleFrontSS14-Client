using Content.Shared.Body.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Systems;

public abstract class SharedMetabolizerSystem : EntitySystem
{
	public void UpdateMetabolicMultiplier(EntityUid uid)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		GetMetabolicMultiplierEvent getEv = new GetMetabolicMultiplierEvent();
		((EntitySystem)this).RaiseLocalEvent<GetMetabolicMultiplierEvent>(uid, ref getEv, false);
		ApplyMetabolicMultiplierEvent applyEv = new ApplyMetabolicMultiplierEvent(getEv.Multiplier);
		((EntitySystem)this).RaiseLocalEvent<ApplyMetabolicMultiplierEvent>(uid, ref applyEv, false);
	}
}
