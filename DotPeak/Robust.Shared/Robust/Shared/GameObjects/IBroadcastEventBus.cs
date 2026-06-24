// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IBroadcastEventBus
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IBroadcastEventBus
{
  void SubscribeEvent<T>(
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntityEventHandler<T> eventHandler)
    where T : notnull;

  void SubscribeEvent<T>(
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntityEventHandler<T> eventHandler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where T : notnull;

  void SubscribeEvent<T>(
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntityEventRefHandler<T> eventHandler)
    where T : notnull;

  void SubscribeEvent<T>(
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntityEventRefHandler<T> eventHandler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where T : notnull;

  void UnsubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber) where T : notnull;

  void RaiseEvent(EventSource source, object toRaise);

  void RaiseEvent<T>(EventSource source, T toRaise) where T : notnull;

  void RaiseEvent<T>(EventSource source, ref T toRaise) where T : notnull;

  void QueueEvent(EventSource source, EntityEventArgs toRaise);

  void UnsubscribeEvents(IEntityEventSubscriber subscriber);
}
