using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IDirectedEventBus
{
	void RaiseLocalEvent<TEvent>(EntityUid uid, TEvent args, bool broadcast = false) where TEvent : notnull;

	void RaiseLocalEvent(EntityUid uid, object args, bool broadcast = false);

	void SubscribeLocalEvent<TComp, TEvent>(ComponentEventHandler<TComp, TEvent> handler) where TComp : IComponent where TEvent : notnull;

	void SubscribeLocalEvent<TComp, TEvent>(ComponentEventHandler<TComp, TEvent> handler, Type orderType, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull;

	void RaiseLocalEvent<TEvent>(EntityUid uid, ref TEvent args, bool broadcast = false) where TEvent : notnull;

	void RaiseLocalEvent(EntityUid uid, ref object args, bool broadcast = false);

	void SubscribeLocalEvent<TComp, TEvent>(ComponentEventRefHandler<TComp, TEvent> handler) where TComp : IComponent where TEvent : notnull;

	void SubscribeLocalEvent<TComp, TEvent>(ComponentEventRefHandler<TComp, TEvent> handler, Type orderType, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull;

	void SubscribeLocalEvent<TComp, TEvent>(EntityEventRefHandler<TComp, TEvent> handler, Type orderType, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull;

	void UnsubscribeLocalEvent<TComp, TEvent>() where TComp : IComponent where TEvent : notnull;

	void RaiseComponentEvent<TEvent, TComponent>(EntityUid uid, TComponent component, TEvent args) where TEvent : notnull where TComponent : IComponent;

	void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, TEvent args) where TEvent : notnull;

	void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, CompIdx idx, TEvent args) where TEvent : notnull;

	void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, ref TEvent args) where TEvent : notnull;

	void RaiseComponentEvent<TEvent, TComponent>(EntityUid uid, TComponent component, ref TEvent args) where TEvent : notnull where TComponent : IComponent;

	void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, CompIdx idx, ref TEvent args) where TEvent : notnull;

	void OnlyCallOnRobustUnitTestISwearToGodPleaseSomebodyKillThisNightmare();
}
