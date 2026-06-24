using Robust.Shared.GameObjects;

namespace Content.Shared.Random.Rules;

public sealed class RulesSystem : EntitySystem
{
	public bool IsTrue(EntityUid uid, RulesPrototype rules)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		foreach (RulesRule rule in rules.Rules)
		{
			if (!rule.Check(base.EntityManager, uid))
			{
				return false;
			}
		}
		return true;
	}
}
