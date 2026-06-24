using Robust.Shared.GameObjects;

namespace Content.Shared.IdentityManagement;

public sealed class TryGetIdentityShortInfoEvent(EntityUid? whileInteractingWith, EntityUid forActor, bool forLogging = false) : HandledEntityEventArgs
{
	public string? Title;

	public readonly EntityUid? WhileInteractingWith = whileInteractingWith;

	public readonly EntityUid ForActor = forActor;

	public readonly bool RequestForAccessLogging = forLogging;
}
