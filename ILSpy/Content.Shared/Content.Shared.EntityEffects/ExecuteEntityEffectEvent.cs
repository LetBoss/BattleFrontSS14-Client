using Robust.Shared.GameObjects;

namespace Content.Shared.EntityEffects;

[ByRefEvent]
public struct ExecuteEntityEffectEvent<T>(T effect, EntityEffectBaseArgs args) where T : EntityEffect
{
	public T Effect = effect;

	public EntityEffectBaseArgs Args = args;
}
