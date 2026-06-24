// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IDirectedEventBus
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IDirectedEventBus
{
  void RaiseLocalEvent<TEvent>(EntityUid uid, TEvent args, bool broadcast = false) where TEvent : notnull;

  void RaiseLocalEvent(EntityUid uid, object args, bool broadcast = false);

  void SubscribeLocalEvent<TComp, TEvent>(ComponentEventHandler<TComp, TEvent> handler)
    where TComp : IComponent
    where TEvent : notnull;

  void SubscribeLocalEvent<TComp, TEvent>(
    ComponentEventHandler<TComp, TEvent> handler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where TComp : IComponent
    where TEvent : notnull;

  void RaiseLocalEvent<TEvent>(EntityUid uid, ref TEvent args, bool broadcast = false) where TEvent : notnull;

  void RaiseLocalEvent(EntityUid uid, ref object args, bool broadcast = false);

  void SubscribeLocalEvent<TComp, TEvent>(ComponentEventRefHandler<TComp, TEvent> handler)
    where TComp : IComponent
    where TEvent : notnull;

  void SubscribeLocalEvent<TComp, TEvent>(
    ComponentEventRefHandler<TComp, TEvent> handler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where TComp : IComponent
    where TEvent : notnull;

  void SubscribeLocalEvent<TComp, TEvent>(
    EntityEventRefHandler<TComp, TEvent> handler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where TComp : IComponent
    where TEvent : notnull;

  void UnsubscribeLocalEvent<TComp, TEvent>()
    where TComp : IComponent
    where TEvent : notnull;

  void RaiseComponentEvent<TEvent, TComponent>(EntityUid uid, TComponent component, TEvent args)
    where TEvent : notnull
    where TComponent : IComponent;

  void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, TEvent args) where TEvent : notnull;

  void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, CompIdx idx, TEvent args) where TEvent : notnull;

  void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, ref TEvent args) where TEvent : notnull;

  void RaiseComponentEvent<TEvent, TComponent>(
    EntityUid uid,
    TComponent component,
    ref TEvent args)
    where TEvent : notnull
    where TComponent : IComponent;

  void RaiseComponentEvent<TEvent>(
    EntityUid uid,
    IComponent component,
    CompIdx idx,
    ref TEvent args)
    where TEvent : notnull;

  void OnlyCallOnRobustUnitTestISwearToGodPleaseSomebodyKillThisNightmare();
}
