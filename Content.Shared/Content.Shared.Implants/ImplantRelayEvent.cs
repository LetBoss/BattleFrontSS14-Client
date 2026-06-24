namespace Content.Shared.Implants;

public sealed class ImplantRelayEvent<T> where T : notnull
{
	public readonly T Event;

	public ImplantRelayEvent(T ev)
	{
		Event = ev;
	}
}
