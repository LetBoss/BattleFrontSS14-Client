using Robust.Shared.GameObjects;

namespace Content.Shared.StatusEffect;

public readonly struct StatusEffectAddedEvent(EntityUid uid, string key)
{
	public readonly EntityUid Uid = uid;

	public readonly string Key = key;
}
