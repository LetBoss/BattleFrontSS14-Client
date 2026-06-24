namespace Robust.Shared.Network;

public delegate void ProcessMessage(NetMessage message);
public delegate void ProcessMessage<in T>(T message) where T : NetMessage;
