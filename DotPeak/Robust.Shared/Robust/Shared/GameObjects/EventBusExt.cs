// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EventBusExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.GameObjects;

public static class EventBusExt
{
  public static void SubscribeSessionEvent<T>(
    this IEventBus eventBus,
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntitySessionEventHandler<T> handler)
  {
    EventBusExt.HandlerWrapper<T> handlerWrapper = new EventBusExt.HandlerWrapper<T>(handler);
    eventBus.SubscribeEvent<EntitySessionMessage<T>>(source, subscriber, new EntityEventHandler<EntitySessionMessage<T>>(handlerWrapper.Invoke));
  }

  public static void SubscribeSessionEvent<T>(
    this IEventBus eventBus,
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntitySessionEventHandler<T> handler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
  {
    EventBusExt.HandlerWrapper<T> handlerWrapper = new EventBusExt.HandlerWrapper<T>(handler);
    eventBus.SubscribeEvent<EntitySessionMessage<T>>(source, subscriber, new EntityEventHandler<EntitySessionMessage<T>>(handlerWrapper.Invoke), orderType, before, after);
  }

  private sealed class HandlerWrapper<T>
  {
    public HandlerWrapper(EntitySessionEventHandler<T> handler) => this.Handler = handler;

    public EntitySessionEventHandler<T> Handler { get; }

    public void Invoke(EntitySessionMessage<T> msg) => this.Handler(msg.Message, msg.EventArgs);

    private bool Equals(EventBusExt.HandlerWrapper<T> other)
    {
      return this.Handler.Equals((object) other.Handler);
    }

    public override bool Equals(object? obj)
    {
      if (this == obj)
        return true;
      return obj is EventBusExt.HandlerWrapper<T> other && this.Equals(other);
    }

    public override int GetHashCode() => this.Handler.GetHashCode();
  }
}
