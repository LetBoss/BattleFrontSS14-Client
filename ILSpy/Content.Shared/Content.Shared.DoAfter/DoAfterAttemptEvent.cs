using Robust.Shared.GameObjects;

namespace Content.Shared.DoAfter;

public sealed class DoAfterAttemptEvent<TEvent> : CancellableEntityEventArgs where TEvent : DoAfterEvent
{
	public readonly DoAfter DoAfter;

	public readonly TEvent Event;

	public DoAfterAttemptEvent(DoAfter doAfter, TEvent @event)
	{
		DoAfter = doAfter;
		Event = @event;
	}
}
