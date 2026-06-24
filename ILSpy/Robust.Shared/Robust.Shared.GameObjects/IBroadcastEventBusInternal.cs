namespace Robust.Shared.GameObjects;

internal interface IBroadcastEventBusInternal : IBroadcastEventBus
{
	void ProcessEventQueue();
}
