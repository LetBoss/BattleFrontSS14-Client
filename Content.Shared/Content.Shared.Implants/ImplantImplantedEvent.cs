using Robust.Shared.GameObjects;

namespace Content.Shared.Implants;

[ByRefEvent]
public readonly struct ImplantImplantedEvent(EntityUid implant, EntityUid? implanted)
{
	public readonly EntityUid Implant = implant;

	public readonly EntityUid? Implanted = implanted;
}
