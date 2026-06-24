using System;
using Content.Shared.Hands;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory;

public static class RelaySubscriptionHelpers
{
	public static void SubscribeWithRelay<TComp, TEvent>(this Subscriptions subs, EntityEventRefHandler<TComp, TEvent> handler, bool baseEvent = true, bool inventory = true, bool held = true) where TComp : IComponent where TEvent : notnull
	{
		if (baseEvent)
		{
			subs.SubscribeLocalEvent<TComp, TEvent>(handler, (Type[])null, (Type[])null);
		}
		if (inventory)
		{
			subs.SubscribeLocalEvent<TComp, InventoryRelayedEvent<TEvent>>((EntityEventRefHandler<TComp, InventoryRelayedEvent<TEvent>>)delegate(Entity<TComp> ent, ref InventoryRelayedEvent<TEvent> ev)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				handler.Invoke(ent, ref ev.Args);
			}, (Type[])null, (Type[])null);
		}
		if (held)
		{
			subs.SubscribeLocalEvent<TComp, HeldRelayedEvent<TEvent>>((EntityEventRefHandler<TComp, HeldRelayedEvent<TEvent>>)delegate(Entity<TComp> ent, ref HeldRelayedEvent<TEvent> ev)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				handler.Invoke(ent, ref ev.Args);
			}, (Type[])null, (Type[])null);
		}
	}

	public static void SubscribeWithRelay<TComp, TEvent>(this Subscriptions subs, ComponentEventHandler<TComp, TEvent> handler, bool baseEvent = true, bool inventory = true, bool held = true) where TComp : IComponent where TEvent : notnull
	{
		if (baseEvent)
		{
			subs.SubscribeLocalEvent<TComp, TEvent>(handler, (Type[])null, (Type[])null);
		}
		if (inventory)
		{
			subs.SubscribeLocalEvent<TComp, InventoryRelayedEvent<TEvent>>((ComponentEventHandler<TComp, InventoryRelayedEvent<TEvent>>)delegate(EntityUid uid, TComp component, InventoryRelayedEvent<TEvent> args)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				handler.Invoke(uid, component, args.Args);
			}, (Type[])null, (Type[])null);
		}
		if (held)
		{
			subs.SubscribeLocalEvent<TComp, HeldRelayedEvent<TEvent>>((ComponentEventHandler<TComp, HeldRelayedEvent<TEvent>>)delegate(EntityUid uid, TComp component, HeldRelayedEvent<TEvent> args)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				handler.Invoke(uid, component, args.Args);
			}, (Type[])null, (Type[])null);
		}
	}

	public static void SubscribeWithRelay<TComp, TEvent>(this Subscriptions subs, ComponentEventRefHandler<TComp, TEvent> handler, bool baseEvent = true, bool inventory = true, bool held = true) where TComp : IComponent where TEvent : notnull
	{
		if (baseEvent)
		{
			subs.SubscribeLocalEvent<TComp, TEvent>(handler, (Type[])null, (Type[])null);
		}
		if (inventory)
		{
			subs.SubscribeLocalEvent<TComp, InventoryRelayedEvent<TEvent>>((ComponentEventRefHandler<TComp, InventoryRelayedEvent<TEvent>>)delegate(EntityUid uid, TComp component, ref InventoryRelayedEvent<TEvent> args)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				handler.Invoke(uid, component, ref args.Args);
			}, (Type[])null, (Type[])null);
		}
		if (held)
		{
			subs.SubscribeLocalEvent<TComp, HeldRelayedEvent<TEvent>>((ComponentEventRefHandler<TComp, HeldRelayedEvent<TEvent>>)delegate(EntityUid uid, TComp component, ref HeldRelayedEvent<TEvent> args)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				handler.Invoke(uid, component, ref args.Args);
			}, (Type[])null, (Type[])null);
		}
	}
}
