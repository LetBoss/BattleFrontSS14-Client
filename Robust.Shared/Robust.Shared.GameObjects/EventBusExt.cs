using System;

namespace Robust.Shared.GameObjects;

public static class EventBusExt
{
	private sealed class HandlerWrapper<T>
	{
		public EntitySessionEventHandler<T> Handler { get; }

		public HandlerWrapper(EntitySessionEventHandler<T> handler)
		{
			Handler = handler;
		}

		public void Invoke(EntitySessionMessage<T> msg)
		{
			Handler(msg.Message, msg.EventArgs);
		}

		private bool Equals(HandlerWrapper<T> other)
		{
			return Handler.Equals(other.Handler);
		}

		public override bool Equals(object? obj)
		{
			if (this != obj)
			{
				if (obj is HandlerWrapper<T> other)
				{
					return Equals(other);
				}
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return Handler.GetHashCode();
		}
	}

	public static void SubscribeSessionEvent<T>(this IEventBus eventBus, EventSource source, IEntityEventSubscriber subscriber, EntitySessionEventHandler<T> handler)
	{
		HandlerWrapper<T> handlerWrapper = new HandlerWrapper<T>(handler);
		eventBus.SubscribeEvent<EntitySessionMessage<T>>(source, subscriber, handlerWrapper.Invoke);
	}

	public static void SubscribeSessionEvent<T>(this IEventBus eventBus, EventSource source, IEntityEventSubscriber subscriber, EntitySessionEventHandler<T> handler, Type orderType, Type[]? before = null, Type[]? after = null)
	{
		HandlerWrapper<T> handlerWrapper = new HandlerWrapper<T>(handler);
		eventBus.SubscribeEvent<EntitySessionMessage<T>>(source, subscriber, handlerWrapper.Invoke, orderType, before, after);
	}
}
