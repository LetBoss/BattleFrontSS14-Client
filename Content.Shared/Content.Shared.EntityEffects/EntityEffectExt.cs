using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.EntityEffects;

public static class EntityEffectExt
{
	public static bool ShouldApply(this EntityEffect effect, EntityEffectBaseArgs args, IRobustRandom? random = null)
	{
		if (random == null)
		{
			random = IoCManager.Resolve<IRobustRandom>();
		}
		if (effect.Probability < 1f && !RandomExtensions.Prob(random, effect.Probability))
		{
			return false;
		}
		if (effect.Conditions != null)
		{
			EntityEffectCondition[] conditions = effect.Conditions;
			for (int i = 0; i < conditions.Length; i++)
			{
				if (!conditions[i].Condition(args))
				{
					return false;
				}
			}
		}
		return true;
	}
}
