// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.RelaySubscriptionHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Inventory;

public static class RelaySubscriptionHelpers
{
  public static void SubscribeWithRelay<TComp, TEvent>(
    this EntitySystem.Subscriptions subs,
    EntityEventRefHandler<TComp, TEvent> handler,
    bool baseEvent = true,
    bool inventory = true,
    bool held = true)
    where TComp : IComponent
    where TEvent : notnull
  {
    if (baseEvent)
      subs.SubscribeLocalEvent<TComp, TEvent>(handler);
    if (inventory)
      subs.SubscribeLocalEvent<TComp, InventoryRelayedEvent<TEvent>>((EntityEventRefHandler<TComp, InventoryRelayedEvent<TEvent>>) ((Entity<TComp> ent, ref InventoryRelayedEvent<TEvent> ev) => handler(ent, ref ev.Args)));
    if (!held)
      return;
    subs.SubscribeLocalEvent<TComp, HeldRelayedEvent<TEvent>>((EntityEventRefHandler<TComp, HeldRelayedEvent<TEvent>>) ((Entity<TComp> ent, ref HeldRelayedEvent<TEvent> ev) => handler(ent, ref ev.Args)));
  }

  public static void SubscribeWithRelay<TComp, TEvent>(
    this EntitySystem.Subscriptions subs,
    ComponentEventHandler<TComp, TEvent> handler,
    bool baseEvent = true,
    bool inventory = true,
    bool held = true)
    where TComp : IComponent
    where TEvent : notnull
  {
    if (baseEvent)
      subs.SubscribeLocalEvent<TComp, TEvent>(handler);
    if (inventory)
      subs.SubscribeLocalEvent<TComp, InventoryRelayedEvent<TEvent>>((ComponentEventHandler<TComp, InventoryRelayedEvent<TEvent>>) ((uid, component, args) => handler(uid, component, args.Args)));
    if (!held)
      return;
    subs.SubscribeLocalEvent<TComp, HeldRelayedEvent<TEvent>>((ComponentEventHandler<TComp, HeldRelayedEvent<TEvent>>) ((uid, component, args) => handler(uid, component, args.Args)));
  }

  public static void SubscribeWithRelay<TComp, TEvent>(
    this EntitySystem.Subscriptions subs,
    ComponentEventRefHandler<TComp, TEvent> handler,
    bool baseEvent = true,
    bool inventory = true,
    bool held = true)
    where TComp : IComponent
    where TEvent : notnull
  {
    if (baseEvent)
      subs.SubscribeLocalEvent<TComp, TEvent>(handler);
    if (inventory)
      subs.SubscribeLocalEvent<TComp, InventoryRelayedEvent<TEvent>>((ComponentEventRefHandler<TComp, InventoryRelayedEvent<TEvent>>) ((EntityUid uid, TComp component, ref InventoryRelayedEvent<TEvent> args) => handler(uid, component, ref args.Args)));
    if (!held)
      return;
    subs.SubscribeLocalEvent<TComp, HeldRelayedEvent<TEvent>>((ComponentEventRefHandler<TComp, HeldRelayedEvent<TEvent>>) ((EntityUid uid, TComp component, ref HeldRelayedEvent<TEvent> args) => handler(uid, component, ref args.Args)));
  }
}
