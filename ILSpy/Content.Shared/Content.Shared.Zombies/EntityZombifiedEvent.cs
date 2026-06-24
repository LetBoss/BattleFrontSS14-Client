using Robust.Shared.GameObjects;

namespace Content.Shared.Zombies;

[ByRefEvent]
public readonly struct EntityZombifiedEvent(EntityUid target)
{
	public readonly EntityUid Target = target;
}
