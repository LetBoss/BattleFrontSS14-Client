using Content.Shared.Random;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Communications;

public abstract class SharedCommsHackerSystem : EntitySystem
{
	public void SetThreats(EntityUid uid, string threats, CommsHackerComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<CommsHackerComponent>(uid, ref comp, true))
		{
			comp.Threats = ProtoId<WeightedRandomPrototype>.op_Implicit(threats);
		}
	}
}
