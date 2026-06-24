using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceLinking.Events;

public sealed class PortDisconnectedEvent : EntityEventArgs
{
	public readonly string Port;

	public PortDisconnectedEvent(string port)
	{
		Port = port;
	}
}
