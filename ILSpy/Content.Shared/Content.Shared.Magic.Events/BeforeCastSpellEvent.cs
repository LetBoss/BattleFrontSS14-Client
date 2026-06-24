using Robust.Shared.GameObjects;

namespace Content.Shared.Magic.Events;

[ByRefEvent]
public struct BeforeCastSpellEvent(EntityUid performer)
{
	public EntityUid Performer = performer;

	public bool Cancelled = false;
}
