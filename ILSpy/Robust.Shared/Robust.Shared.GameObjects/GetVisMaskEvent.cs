namespace Robust.Shared.GameObjects;

[ByRefEvent]
public record struct GetVisMaskEvent()
{
	public EntityUid Entity = default(EntityUid);

	public int VisibilityMask = 1;
}
