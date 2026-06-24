using Robust.Shared.GameObjects;

namespace Content.Shared.Forensics;

[ByRefEvent]
public record struct GenerateDnaEvent()
{
	public EntityUid Owner = default(EntityUid);

	public required string DNA = null;
}
