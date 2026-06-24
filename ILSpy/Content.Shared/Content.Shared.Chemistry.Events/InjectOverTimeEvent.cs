using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Events;

[ByRefEvent]
public readonly record struct InjectOverTimeEvent(EntityUid embeddedIntoUid)
{
	public readonly EntityUid EmbeddedIntoUid = embeddedIntoUid;
}
