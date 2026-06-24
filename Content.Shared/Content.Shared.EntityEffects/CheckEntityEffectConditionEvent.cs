using Robust.Shared.GameObjects;

namespace Content.Shared.EntityEffects;

[ByRefEvent]
public struct CheckEntityEffectConditionEvent<T> where T : EntityEffectCondition
{
	public T Condition;

	public EntityEffectBaseArgs Args;

	public bool Result;
}
