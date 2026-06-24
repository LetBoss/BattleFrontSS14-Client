namespace Robust.Shared.Network;

public enum ClientConnectionState : byte
{
	NotConnecting,
	ResolvingHost,
	EstablishingConnection,
	Handshake,
	Connected
}
