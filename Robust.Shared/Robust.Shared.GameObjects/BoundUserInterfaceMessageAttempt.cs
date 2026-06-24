using System;

namespace Robust.Shared.GameObjects;

public sealed class BoundUserInterfaceMessageAttempt(EntityUid actor, EntityUid target, Enum uiKey, BoundUserInterfaceMessage message) : CancellableEntityEventArgs
{
	public readonly EntityUid Actor = actor;

	public readonly EntityUid Target = target;

	public readonly Enum UiKey = uiKey;

	public readonly BoundUserInterfaceMessage Message = message;
}
