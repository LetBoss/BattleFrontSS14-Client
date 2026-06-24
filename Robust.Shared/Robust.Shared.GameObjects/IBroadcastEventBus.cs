using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IBroadcastEventBus
{
	void SubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber, EntityEventHandler<T> eventHandler) where T : notnull;

	void SubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber, EntityEventHandler<T> eventHandler, Type orderType, Type[]? before = null, Type[]? after = null) where T : notnull;

	void SubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber, EntityEventRefHandler<T> eventHandler) where T : notnull;

	void SubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber, EntityEventRefHandler<T> eventHandler, Type orderType, Type[]? before = null, Type[]? after = null) where T : notnull;

	void UnsubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber) where T : notnull;

	void RaiseEvent(EventSource source, object toRaise);

	void RaiseEvent<T>(EventSource source, T toRaise) where T : notnull;

	void RaiseEvent<T>(EventSource source, ref T toRaise) where T : notnull;

	void QueueEvent(EventSource source, EntityEventArgs toRaise);

	void UnsubscribeEvents(IEntityEventSubscriber subscriber);
}
