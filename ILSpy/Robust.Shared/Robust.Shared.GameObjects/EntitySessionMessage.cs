namespace Robust.Shared.GameObjects;

internal readonly struct EntitySessionMessage<T>(EntitySessionEventArgs eventArgs, T message)
{
	public EntitySessionEventArgs EventArgs { get; } = eventArgs;

	public T Message { get; } = message;

	public void Deconstruct(out EntitySessionEventArgs eventArgs, out T message)
	{
		eventArgs = EventArgs;
		message = Message;
	}
}
