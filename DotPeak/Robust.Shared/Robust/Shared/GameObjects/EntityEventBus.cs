// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityEventBus
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Reflection;
using Robust.Shared.Utility;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.GameObjects;

internal sealed class EntityEventBus : 
  IBroadcastEventBusInternal,
  IBroadcastEventBus,
  IEventBus,
  IDirectedEventBus,
  IDisposable
{
  private EntityManager _entMan;
  private IComponentFactory _comFac;
  private IReflectionManager _reflection;
  private FrozenDictionary<Type, EntityEventBus.EventData> _eventData = FrozenDictionary<Type, EntityEventBus.EventData>.Empty;
  private readonly Dictionary<Type, EntityEventBus.EventData> _eventDataUnfrozen = new Dictionary<Type, EntityEventBus.EventData>();
  private readonly Dictionary<IEntityEventSubscriber, Dictionary<Type, EntityEventBus.BroadcastRegistration>> _inverseEventSubscriptions = new Dictionary<IEntityEventSubscriber, Dictionary<Type, EntityEventBus.BroadcastRegistration>>();
  private readonly Queue<(EventSource source, object args)> _eventQueue = new Queue<(EventSource, object)>();
  internal Dictionary<EntityUid, EntityEventBus.EventTable> _entEventTables = new Dictionary<EntityUid, EntityEventBus.EventTable>();
  private FrozenDictionary<Type, EntityEventBus.DirectedRegistration>[] _eventSubs;
  private FrozenDictionary<Type, EntityEventBus.DirectedEventHandler>[] _compEventSubs;
  private Dictionary<Type, EntityEventBus.DirectedRegistration>?[] _eventSubsUnfrozen = Array.Empty<Dictionary<Type, EntityEventBus.DirectedRegistration>>();
  private Dictionary<Type, EntityEventBus.DirectedEventHandler>?[] _compEventSubsUnfrozen = Array.Empty<Dictionary<Type, EntityEventBus.DirectedEventHandler>>();
  private Dictionary<Type, HashSet<CompIdx>> _eventSubsInv = new Dictionary<Type, HashSet<CompIdx>>();
  private bool _subscriptionLock;
  public bool IgnoreUnregisteredComponents;
  private readonly List<Type> _childrenTypesTemp = new List<Type>();
  private const int MaxEventLinkedListSize = 256 /*0x0100*/;

  public void UnsubscribeEvents(IEntityEventSubscriber subscriber)
  {
    if (subscriber == null)
      throw new ArgumentNullException(nameof (subscriber));
    Dictionary<Type, EntityEventBus.BroadcastRegistration> source;
    if (!this._inverseEventSubscriptions.TryGetValue(subscriber, out source))
      return;
    foreach ((Type type, EntityEventBus.BroadcastRegistration tuple) in source.ToList<KeyValuePair<Type, EntityEventBus.BroadcastRegistration>>())
      this.UnsubscribeEvent(type, tuple, subscriber);
  }

  public void ProcessEventQueue()
  {
    while (this._eventQueue.Count != 0)
    {
      (EventSource source, object args) = this._eventQueue.Dequeue();
      Type type = args.GetType();
      ref EntityEventBus.Unit local = ref EntityEventBus.ExtractUnitRef(ref args, type);
      this.ProcessSingleEvent(source, ref local, type);
    }
  }

  public void SubscribeEvent<T>(
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntityEventHandler<T> eventHandler)
    where T : notnull
  {
    if (eventHandler == null)
      throw new ArgumentNullException(nameof (eventHandler));
    this.SubscribeEventCommon<T>(source, subscriber, (EntityEventBus.RefEventHandler) ((ref EntityEventBus.Unit ev) => eventHandler(Unsafe.As<EntityEventBus.Unit, T>(ref ev))), (object) eventHandler, (EntityEventBus.OrderingData) null, false);
  }

  public void SubscribeEvent<T>(
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntityEventHandler<T> eventHandler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where T : notnull
  {
    if (eventHandler == null)
      throw new ArgumentNullException(nameof (eventHandler));
    EntityEventBus.OrderingData orderingData = this.CreateOrderingData(orderType, before, after);
    this.SubscribeEventCommon<T>(source, subscriber, (EntityEventBus.RefEventHandler) ((ref EntityEventBus.Unit ev) => eventHandler(Unsafe.As<EntityEventBus.Unit, T>(ref ev))), (object) eventHandler, orderingData, false);
  }

  public void SubscribeEvent<T>(
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntityEventRefHandler<T> eventHandler)
    where T : notnull
  {
    this.SubscribeEventCommon<T>(source, subscriber, (EntityEventBus.RefEventHandler) ((ref EntityEventBus.Unit ev) => eventHandler(ref Unsafe.As<EntityEventBus.Unit, T>(ref ev))), (object) eventHandler, (EntityEventBus.OrderingData) null, true);
  }

  public void SubscribeEvent<T>(
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntityEventRefHandler<T> eventHandler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where T : notnull
  {
    EntityEventBus.OrderingData orderingData = this.CreateOrderingData(orderType, before, after);
    this.SubscribeEventCommon<T>(source, subscriber, (EntityEventBus.RefEventHandler) ((ref EntityEventBus.Unit ev) => eventHandler(ref Unsafe.As<EntityEventBus.Unit, T>(ref ev))), (object) eventHandler, orderingData, true);
  }

  private void SubscribeEventCommon<T>(
    EventSource source,
    IEntityEventSubscriber subscriber,
    EntityEventBus.RefEventHandler handler,
    object equalityToken,
    EntityEventBus.OrderingData? order,
    bool byRef)
    where T : notnull
  {
    if (source == EventSource.None)
      throw new ArgumentOutOfRangeException(nameof (source));
    if (subscriber == null)
      throw new ArgumentNullException(nameof (subscriber));
    Type type = typeof (T);
    bool flag = type.HasCustomAttribute<ByRefEventAttribute>();
    if (flag != byRef)
      throw new InvalidOperationException($"Attempted to subscribe by-ref and by-value to the same broadcast event! event={type} eventIsByRef={flag} subscriptionIsByRef={byRef}");
    EntityEventBus.BroadcastRegistration broadcastRegistration = new EntityEventBus.BroadcastRegistration(source, handler, equalityToken, order, byRef);
    EntityEventBus.EventData subs;
    this.RegisterCommon(type, order, out subs);
    if (!subs.BroadcastRegistrations.Contains(broadcastRegistration))
      subs.BroadcastRegistrations.Add(broadcastRegistration);
    if (!this._inverseEventSubscriptions.GetOrNew<IEntityEventSubscriber, Dictionary<Type, EntityEventBus.BroadcastRegistration>>(subscriber).TryAdd(type, broadcastRegistration))
      throw new InvalidOperationException($"{subscriber.GetType().Name} attempted to subscribe twice to the same event: {type.Name}");
  }

  public void UnsubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber) where T : notnull
  {
    if (source == EventSource.None)
      throw new ArgumentOutOfRangeException(nameof (source));
    if (subscriber == null)
      throw new ArgumentNullException(nameof (subscriber));
    Type type = typeof (T);
    Dictionary<Type, EntityEventBus.BroadcastRegistration> dictionary;
    EntityEventBus.BroadcastRegistration tuple;
    if (!this._inverseEventSubscriptions.TryGetValue(subscriber, out dictionary) || !dictionary.TryGetValue(type, out tuple))
      return;
    this.UnsubscribeEvent(type, tuple, subscriber);
  }

  public void RaiseEvent(EventSource source, object toRaise)
  {
    if (source == EventSource.None)
      throw new ArgumentOutOfRangeException(nameof (source));
    Type type = toRaise.GetType();
    ref EntityEventBus.Unit local = ref EntityEventBus.ExtractUnitRef(ref toRaise, type);
    this.ProcessSingleEvent(source, ref local, type);
  }

  public void RaiseEvent<T>(EventSource source, T toRaise) where T : notnull
  {
    if (source == EventSource.None)
      throw new ArgumentOutOfRangeException(nameof (source));
    this.ProcessSingleEvent(source, ref Unsafe.As<T, EntityEventBus.Unit>(ref toRaise), typeof (T));
  }

  public void RaiseEvent<T>(EventSource source, ref T toRaise) where T : notnull
  {
    if (source == EventSource.None)
      throw new ArgumentOutOfRangeException(nameof (source));
    this.ProcessSingleEvent(source, ref Unsafe.As<T, EntityEventBus.Unit>(ref toRaise), typeof (T));
  }

  public void QueueEvent(EventSource source, EntityEventArgs toRaise)
  {
    if (source == EventSource.None)
      throw new ArgumentOutOfRangeException(nameof (source));
    if (toRaise == null)
      throw new ArgumentNullException(nameof (toRaise));
    this._eventQueue.Enqueue((source, (object) toRaise));
  }

  private void UnsubscribeEvent(
    Type eventType,
    EntityEventBus.BroadcastRegistration tuple,
    IEntityEventSubscriber subscriber)
  {
    if (this._subscriptionLock)
      throw new InvalidOperationException("Subscription locked.");
    EntityEventBus.EventData eventData;
    if (this._eventDataUnfrozen.TryGetValue(eventType, out eventData) && eventData.BroadcastRegistrations.Contains(tuple))
      eventData.BroadcastRegistrations.Remove(tuple);
    Dictionary<Type, EntityEventBus.BroadcastRegistration> dictionary;
    if (!this._inverseEventSubscriptions.TryGetValue(subscriber, out dictionary) || !dictionary.ContainsKey(eventType))
      return;
    dictionary.Remove(eventType);
  }

  private void ProcessSingleEvent(
    EventSource source,
    ref EntityEventBus.Unit unitRef,
    Type eventType)
  {
    EntityEventBus.EventData eventData;
    if (!this._eventData.TryGetValue(eventType, out eventData))
      return;
    if (eventData.IsOrdered && !eventData.OrderingUpToDate)
      this.UpdateOrderSeq(eventType, eventData);
    EntityEventBus.ProcessSingleEventCore(source, ref unitRef, eventData);
  }

  private static void ProcessSingleEventCore(
    EventSource source,
    ref EntityEventBus.Unit unitRef,
    EntityEventBus.EventData subs)
  {
    Span<EntityEventBus.BroadcastRegistration> span = subs.BroadcastRegistrations.Span;
    for (int index = 0; index < span.Length; ++index)
    {
      EntityEventBus.BroadcastRegistration broadcastRegistration = span[index];
      if ((broadcastRegistration.Mask & source) != EventSource.None)
        broadcastRegistration.Handler(ref unitRef);
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ref EntityEventBus.Unit ExtractUnitRef(ref object obj, Type objType)
  {
    return ref (!objType.IsValueType ? ref Unsafe.As<object, EntityEventBus.Unit>(ref obj) : ref Unsafe.As<object, EntityEventBus.UnitBox>(ref obj).Value);
  }

  private void RegisterCommon(
    Type eventType,
    EntityEventBus.OrderingData? data,
    out EntityEventBus.EventData subs)
  {
    if (this._subscriptionLock)
      throw new InvalidOperationException("Subscription locked.");
    subs = this._eventDataUnfrozen.GetOrNew<Type, EntityEventBus.EventData>(eventType);
    if (data == (EntityEventBus.OrderingData) null || data.Before.Length == 0 && data.After.Length == 0)
      return;
    subs.IsOrdered = true;
    subs.OrderingUpToDate = false;
  }

  public EntityEventBus(EntityManager entMan, IReflectionManager reflection)
  {
    this._entMan = entMan;
    this._comFac = entMan.ComponentFactory;
    this._reflection = reflection;
    this._comFac.ComponentsAdded += new Action<ComponentRegistration[]>(this.ComFacOnComponentsAdded);
    this.ComFacOnComponentsAdded(this._comFac.GetAllRegistrations().ToArray<ComponentRegistration>());
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, TEvent args) where TEvent : notnull
  {
    this.RaiseComponentEvent<TEvent>(uid, component, this._comFac.GetIndex(component.GetType()), ref args);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void RaiseComponentEvent<TEvent, TComponent>(
    EntityUid uid,
    TComponent component,
    TEvent args)
    where TEvent : notnull
    where TComponent : IComponent
  {
    this.RaiseComponentEvent<TEvent>(uid, (IComponent) component, CompIdx.Index<TComponent>(), ref args);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void RaiseComponentEvent<TEvent>(
    EntityUid uid,
    IComponent component,
    CompIdx type,
    TEvent args)
    where TEvent : notnull
  {
    this.RaiseComponentEvent<TEvent>(uid, component, type, ref args);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, ref TEvent args) where TEvent : notnull
  {
    this.RaiseComponentEvent<TEvent>(uid, component, this._comFac.GetIndex(component.GetType()), ref args);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void RaiseComponentEvent<TEvent, TComponent>(
    EntityUid uid,
    TComponent component,
    ref TEvent args)
    where TEvent : notnull
    where TComponent : IComponent
  {
    this.RaiseComponentEvent<TEvent>(uid, (IComponent) component, CompIdx.Index<TComponent>(), ref args);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void RaiseComponentEvent<TEvent>(
    EntityUid uid,
    IComponent component,
    CompIdx type,
    ref TEvent args)
    where TEvent : notnull
  {
    EntityEventBus.DirectedEventHandler directedEventHandler;
    if (!this._compEventSubs[type.Value].TryGetValue(typeof (TEvent), out directedEventHandler))
      return;
    directedEventHandler(uid, component, ref Unsafe.As<TEvent, EntityEventBus.Unit>(ref args));
  }

  public void OnlyCallOnRobustUnitTestISwearToGodPleaseSomebodyKillThisNightmare()
  {
    this.IgnoreUnregisteredComponents = true;
  }

  public void RaiseLocalEvent<TEvent>(EntityUid uid, TEvent args, bool broadcast = false) where TEvent : notnull
  {
    Type type = typeof (TEvent);
    ref EntityEventBus.Unit local = ref Unsafe.As<TEvent, EntityEventBus.Unit>(ref args);
    this.RaiseLocalEventCore(uid, ref local, type, broadcast);
  }

  public void RaiseLocalEvent(EntityUid uid, object args, bool broadcast = false)
  {
    Type type = args.GetType();
    ref EntityEventBus.Unit local = ref Unsafe.As<object, EntityEventBus.Unit>(ref args);
    this.RaiseLocalEventCore(uid, ref local, type, broadcast);
  }

  public void RaiseLocalEvent<TEvent>(EntityUid uid, ref TEvent args, bool broadcast = false) where TEvent : notnull
  {
    Type type = typeof (TEvent);
    ref EntityEventBus.Unit local = ref Unsafe.As<TEvent, EntityEventBus.Unit>(ref args);
    this.RaiseLocalEventCore(uid, ref local, type, broadcast);
  }

  public void RaiseLocalEvent(EntityUid uid, ref object args, bool broadcast = false)
  {
    Type type = args.GetType();
    ref EntityEventBus.Unit local = ref Unsafe.As<object, EntityEventBus.Unit>(ref args);
    this.RaiseLocalEventCore(uid, ref local, type, broadcast);
  }

  private void RaiseLocalEventCore(
    EntityUid uid,
    ref EntityEventBus.Unit unitRef,
    Type type,
    bool broadcast)
  {
    EntityEventBus.EventData subs;
    if (!this._eventData.TryGetValue(type, out subs))
      return;
    if (subs.IsOrdered)
    {
      this.RaiseLocalOrdered(uid, type, subs, ref unitRef, broadcast);
    }
    else
    {
      this.EntDispatch(uid, type, ref unitRef);
      if (!broadcast)
        return;
      EntityEventBus.ProcessSingleEventCore(EventSource.Local, ref unitRef, subs);
    }
  }

  public void SubscribeLocalEvent<TComp, TEvent>(ComponentEventHandler<TComp, TEvent> handler)
    where TComp : IComponent
    where TEvent : notnull
  {
    this.EntAddSubscription(CompIdx.Index<TComp>(), typeof (TComp), typeof (TEvent), new EntityEventBus.DirectedEventHandler(EventHandler));

    void EventHandler(EntityUid uid, IComponent comp, ref EntityEventBus.Unit ev)
    {
      ref TEvent local = ref Unsafe.As<EntityEventBus.Unit, TEvent>(ref ev);
      handler(uid, (TComp) comp, local);
    }
  }

  public void SubscribeLocalEvent<TComp, TEvent>(
    ComponentEventHandler<TComp, TEvent> handler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where TComp : IComponent
    where TEvent : notnull
  {
    this.EntAddSubscription(CompIdx.Index<TComp>(), typeof (TComp), typeof (TEvent), new EntityEventBus.DirectedEventHandler(EventHandler), orderType, before, after);

    void EventHandler(EntityUid uid, IComponent comp, ref EntityEventBus.Unit ev)
    {
      ref TEvent local = ref Unsafe.As<EntityEventBus.Unit, TEvent>(ref ev);
      handler(uid, (TComp) comp, local);
    }
  }

  public void SubscribeLocalEvent<TComp, TEvent>(ComponentEventRefHandler<TComp, TEvent> handler)
    where TComp : IComponent
    where TEvent : notnull
  {
    this.EntAddSubscription(CompIdx.Index<TComp>(), typeof (TComp), typeof (TEvent), new EntityEventBus.DirectedEventHandler(EventHandler));

    void EventHandler(EntityUid uid, IComponent comp, ref EntityEventBus.Unit ev)
    {
      ref TEvent local = ref Unsafe.As<EntityEventBus.Unit, TEvent>(ref ev);
      handler(uid, (TComp) comp, ref local);
    }
  }

  public void SubscribeLocalEvent<TComp, TEvent>(
    ComponentEventRefHandler<TComp, TEvent> handler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where TComp : IComponent
    where TEvent : notnull
  {
    this.EntAddSubscription(CompIdx.Index<TComp>(), typeof (TComp), typeof (TEvent), new EntityEventBus.DirectedEventHandler(EventHandler), orderType, before, after);

    void EventHandler(EntityUid uid, IComponent comp, ref EntityEventBus.Unit ev)
    {
      ref TEvent local = ref Unsafe.As<EntityEventBus.Unit, TEvent>(ref ev);
      handler(uid, (TComp) comp, ref local);
    }
  }

  public void SubscribeLocalEvent<TComp, TEvent>(
    EntityEventRefHandler<TComp, TEvent> handler,
    Type orderType,
    Type[]? before = null,
    Type[]? after = null)
    where TComp : IComponent
    where TEvent : notnull
  {
    this.EntAddSubscription(CompIdx.Index<TComp>(), typeof (TComp), typeof (TEvent), new EntityEventBus.DirectedEventHandler(EventHandler), orderType, before, after);

    void EventHandler(EntityUid uid, IComponent comp, ref EntityEventBus.Unit ev)
    {
      ref TEvent local = ref Unsafe.As<EntityEventBus.Unit, TEvent>(ref ev);
      handler(new Entity<TComp>(uid, (TComp) comp), ref local);
    }
  }

  public void UnsubscribeLocalEvent<TComp, TEvent>()
    where TComp : IComponent
    where TEvent : notnull
  {
    if (!this._comFac.TryGetRegistration(typeof (TComp), out ComponentRegistration _))
    {
      if (!this.IgnoreUnregisteredComponents)
        throw new InvalidOperationException("Component is not a valid reference type: " + typeof (TComp).Name);
    }
    else
    {
      if (this._subscriptionLock)
        throw new InvalidOperationException("Subscription locked.");
      int index = CompIdx.ArrayIndex<TComp>();
      this._eventSubsUnfrozen[index].Remove(typeof (TEvent));
      this._compEventSubsUnfrozen[index].Remove(typeof (TEvent));
      HashSet<CompIdx> compIdxSet;
      if (!this._eventSubsInv.TryGetValue(typeof (TEvent), out compIdxSet))
        return;
      compIdxSet.Remove(CompIdx.Index<TComp>());
    }
  }

  private void ComFacOnComponentsAdded(ComponentRegistration[] regs)
  {
    if (this._subscriptionLock)
      throw new InvalidOperationException("Subscription locked.");
    foreach (ComponentRegistration reg in regs)
    {
      ref Dictionary<Type, EntityEventBus.DirectedRegistration> local1 = ref CompIdx.RefArray<Dictionary<Type, EntityEventBus.DirectedRegistration>>(ref this._eventSubsUnfrozen, reg.Idx);
      if (local1 == null)
        local1 = new Dictionary<Type, EntityEventBus.DirectedRegistration>();
      ref Dictionary<Type, EntityEventBus.DirectedEventHandler> local2 = ref CompIdx.RefArray<Dictionary<Type, EntityEventBus.DirectedEventHandler>>(ref this._compEventSubsUnfrozen, reg.Idx);
      if (local2 == null)
        local2 = new Dictionary<Type, EntityEventBus.DirectedEventHandler>();
    }
  }

  public void OnEntityAdded(EntityUid e) => this.EntAddEntity(e);

  public void OnEntityDeleted(EntityUid e) => this.EntRemoveEntity(e);

  public void OnComponentAdded(in AddedComponentEventArgs e)
  {
    this.EntAddComponent(e.BaseArgs.Owner, e.ComponentType.Idx);
  }

  internal void LockSubscriptions()
  {
    this._subscriptionLock = true;
    this._eventData = this._eventDataUnfrozen.ToFrozenDictionary<Type, EntityEventBus.EventData>();
    this._eventSubs = ((IEnumerable<Dictionary<Type, EntityEventBus.DirectedRegistration>>) EntityEventBus.TrimNull<Dictionary<Type, EntityEventBus.DirectedRegistration>>(this._eventSubsUnfrozen)).Select<Dictionary<Type, EntityEventBus.DirectedRegistration>, FrozenDictionary<Type, EntityEventBus.DirectedRegistration>>((Func<Dictionary<Type, EntityEventBus.DirectedRegistration>, FrozenDictionary<Type, EntityEventBus.DirectedRegistration>>) (dict => dict == null ? (FrozenDictionary<Type, EntityEventBus.DirectedRegistration>) null : dict.ToFrozenDictionary<Type, EntityEventBus.DirectedRegistration>())).ToArray<FrozenDictionary<Type, EntityEventBus.DirectedRegistration>>();
    this._compEventSubs = ((IEnumerable<Dictionary<Type, EntityEventBus.DirectedEventHandler>>) EntityEventBus.TrimNull<Dictionary<Type, EntityEventBus.DirectedEventHandler>>(this._compEventSubsUnfrozen)).Select<Dictionary<Type, EntityEventBus.DirectedEventHandler>, FrozenDictionary<Type, EntityEventBus.DirectedEventHandler>>((Func<Dictionary<Type, EntityEventBus.DirectedEventHandler>, FrozenDictionary<Type, EntityEventBus.DirectedEventHandler>>) (dict => dict == null ? (FrozenDictionary<Type, EntityEventBus.DirectedEventHandler>) null : dict.ToFrozenDictionary<Type, EntityEventBus.DirectedEventHandler>())).ToArray<FrozenDictionary<Type, EntityEventBus.DirectedEventHandler>>();
    this.CalcOrdering();
  }

  public void OnComponentRemoved(in RemovedComponentEventArgs e)
  {
    this.EntRemoveComponent(e.BaseArgs.Owner, e.Idx);
  }

  private void EntAddSubscription(
    CompIdx compType,
    Type compTypeObj,
    Type eventType,
    EntityEventBus.DirectedEventHandler handler,
    Type? orderType = null,
    Type[]? before = null,
    Type[]? after = null)
  {
    if (this._subscriptionLock)
      throw new InvalidOperationException("Subscription locked.");
    if (!this._comFac.TryGetRegistration(compTypeObj, out ComponentRegistration _))
    {
      if (!this.IgnoreUnregisteredComponents)
        throw new InvalidOperationException("Component is not a valid reference type: " + compTypeObj.Name);
    }
    else
    {
      ComponentEventAttribute customAttribute = eventType.GetCustomAttribute<ComponentEventAttribute>();
      if (customAttribute != null)
      {
        if (!this._compEventSubsUnfrozen[compType.Value].TryAdd(eventType, handler))
          throw new InvalidOperationException($"Duplicate Subscriptions for comp={compTypeObj}, event={eventType.Name}");
        if (customAttribute.Exclusive)
          return;
      }
      EntityEventBus.DirectedRegistration directedRegistration = new EntityEventBus.DirectedRegistration(orderType == (Type) null ? (EntityEventBus.OrderingData) null : this.CreateOrderingData(orderType, before, after), handler);
      if (!this._eventSubsUnfrozen[compType.Value].TryAdd(eventType, directedRegistration))
        throw new InvalidOperationException($"Duplicate Subscriptions for comp={compTypeObj}, event={eventType.Name}");
      this.RegisterCommon(eventType, directedRegistration.Ordering, out EntityEventBus.EventData _);
      this._eventSubsInv.GetOrNew<Type, HashSet<CompIdx>>(eventType).Add(compType);
    }
  }

  private void EntAddEntity(EntityUid euid)
  {
    this._entEventTables.Add(euid, new EntityEventBus.EventTable());
  }

  private void EntRemoveEntity(EntityUid euid) => this._entEventTables.Remove(euid);

  private void EntAddComponent(EntityUid euid, CompIdx compType)
  {
    EntityEventBus.EventTable entEventTable = this._entEventTables[euid];
    foreach (Type key in this._eventSubs[compType.Value].Keys)
    {
      if (entEventTable.Free < 0)
        EntityEventBus.GrowEventTable(entEventTable);
      bool exists;
      ref (int, int) local1 = ref CollectionsMarshal.GetValueRefOrAddDefault<Type, (int, int)>(entEventTable.EventIndices, key, out exists);
      int free = entEventTable.Free;
      ref EntityEventBus.EventTableListEntry local2 = ref entEventTable.ComponentLists[free];
      entEventTable.Free = local2.Next;
      local2.Component = compType;
      local2.Next = exists ? local1.Item1 : -1;
      local1.Item1 = free;
      ++local1.Item2;
      if (local1.Item2 > 256 /*0x0100*/)
        throw new NotSupportedException("Exceeded maximum event linked list size. Need to implement stackalloc fallback.");
    }
  }

  private static void GrowEventTable(EntityEventBus.EventTable table)
  {
    int length = table.ComponentLists.Length * 2;
    EntityEventBus.EventTableListEntry[] componentLists = table.ComponentLists;
    EntityEventBus.EventTableListEntry[] eventTableListEntryArray = GC.AllocateUninitializedArray<EntityEventBus.EventTableListEntry>(length);
    Array.Copy((Array) componentLists, (Array) eventTableListEntryArray, componentLists.Length);
    EntityEventBus.InitEventTableFreeList((Span<EntityEventBus.EventTableListEntry>) eventTableListEntryArray, eventTableListEntryArray.Length, componentLists.Length);
    table.Free = componentLists.Length;
    table.ComponentLists = eventTableListEntryArray;
  }

  private static void InitEventTableFreeList(
    Span<EntityEventBus.EventTableListEntry> entries,
    int end,
    int start)
  {
    int num = -1;
    for (int index = end - 1; index >= start; --index)
    {
      ref EntityEventBus.EventTableListEntry local = ref entries[index];
      local.Component = new CompIdx();
      local.Next = num;
      num = index;
    }
  }

  private void EntRemoveComponent(EntityUid euid, CompIdx compType)
  {
    EntityEventBus.EventTable entEventTable = this._entEventTables[euid];
    foreach (Type key in this._eventSubs[compType.Value].Keys)
    {
      ref (int, int) local1 = ref CollectionsMarshal.GetValueRefOrNullRef<Type, (int, int)>(entEventTable.EventIndices, key);
      if (!Unsafe.IsNullRef<(int, int)>(ref local1))
      {
        int index = local1.Item1;
        ref EntityEventBus.EventTableListEntry local2 = ref entEventTable.ComponentLists[index];
        if (local1.Item2 == 1)
        {
          entEventTable.EventIndices.Remove(key);
        }
        else
        {
          // ISSUE: explicit reference operation
          ref int local3 = @local1.Item1;
          for (; local2.Component != compType; local2 = ref entEventTable.ComponentLists[index])
          {
            local3 = ref local2.Next;
            index = local2.Next;
          }
          local3 = local2.Next;
          --local1.Item2;
        }
        local2.Next = entEventTable.Free;
        entEventTable.Free = index;
      }
    }
  }

  private void EntDispatch(EntityUid euid, Type eventType, ref EntityEventBus.Unit args)
  {
    EntityEventBus.EventTable eventTable;
    (int Start, int Count) tuple;
    if (!this._entEventTables.TryGetValue(euid, out eventTable) || !eventTable.EventIndices.TryGetValue(eventType, out tuple))
      return;
    Span<CompIdx> span1 = stackalloc CompIdx[tuple.Count];
    int index1 = tuple.Start;
    for (int index2 = 0; index2 < span1.Length; ++index2)
    {
      ref EntityEventBus.EventTableListEntry local = ref eventTable.ComponentLists[index1];
      index1 = local.Next;
      span1[index2] = local.Component;
    }
    Span<CompIdx> span2 = span1;
    for (int index3 = 0; index3 < span2.Length; ++index3)
    {
      CompIdx type = span2[index3];
      IComponent component;
      if (this._entMan.TryGetComponent(euid, type, out component))
        this._eventSubs[type.Value][eventType].Handler(euid, component, ref args);
    }
  }

  private void EntCollectOrdered(
    EntityUid euid,
    Type eventType,
    ref ValueList<EntityEventBus.OrderedEventDispatch> found)
  {
    EntityEventBus.EventTable eventTable;
    (int Start, int Count) tuple;
    if (!this._entEventTables.TryGetValue(euid, out eventTable) || !eventTable.EventIndices.TryGetValue(eventType, out tuple))
      return;
    int index = tuple.Start;
    while (index != -1)
    {
      ref EntityEventBus.EventTableListEntry local = ref eventTable.ComponentLists[index];
      index = local.Next;
      IComponent comp = this._entMan.GetComponentInternal(euid, local.Component);
      EntityEventBus.DirectedRegistration reg = this._eventSubs[local.Component.Value][eventType];
      found.Add(new EntityEventBus.OrderedEventDispatch((EntityEventBus.RefEventHandler) ((ref EntityEventBus.Unit ev) =>
      {
        if (comp.Deleted)
          return;
        reg.Handler(euid, comp, ref ev);
      }), reg.Order));
    }
  }

  public void ClearSubscriptions()
  {
    this._subscriptionLock = false;
    this._eventDataUnfrozen.Clear();
    this._entEventTables.Clear();
    this._inverseEventSubscriptions.Clear();
    this._compEventSubs = (FrozenDictionary<Type, EntityEventBus.DirectedEventHandler>[]) null;
    this._eventSubs = (FrozenDictionary<Type, EntityEventBus.DirectedRegistration>[]) null;
    this._eventData = FrozenDictionary<Type, EntityEventBus.EventData>.Empty;
    foreach (Dictionary<Type, EntityEventBus.DirectedRegistration> dictionary in this._eventSubsUnfrozen)
      dictionary?.Clear();
    foreach (Dictionary<Type, EntityEventBus.DirectedEventHandler> dictionary in this._compEventSubsUnfrozen)
      dictionary?.Clear();
  }

  public void Dispose()
  {
    this._comFac.ComponentsAdded -= new Action<ComponentRegistration[]>(this.ComFacOnComponentsAdded);
    this._entMan = (EntityManager) null;
    this._comFac = (IComponentFactory) null;
    this._reflection = (IReflectionManager) null;
    this._entEventTables = (Dictionary<EntityUid, EntityEventBus.EventTable>) null;
    this._compEventSubs = (FrozenDictionary<Type, EntityEventBus.DirectedEventHandler>[]) null;
    this._eventSubs = (FrozenDictionary<Type, EntityEventBus.DirectedRegistration>[]) null;
    this._eventSubsUnfrozen = (Dictionary<Type, EntityEventBus.DirectedRegistration>[]) null;
    this._compEventSubsUnfrozen = (Dictionary<Type, EntityEventBus.DirectedEventHandler>[]) null;
    this._eventSubsInv = (Dictionary<Type, HashSet<CompIdx>>) null;
  }

  public static T[] TrimNull<T>(T[] input)
  {
    int num = 0;
    for (int index = 0; index < input.Length; ++index)
    {
      if ((object) input[index] != null)
        num = index;
    }
    return RuntimeHelpers.GetSubArray<T>(input, Range.EndAt((Index) (num + 1)));
  }

  internal EntityEventBus.DirectedEventHandler?[] GetNetCompEventHandlers<TEvent>()
  {
    IReadOnlyList<ComponentRegistration> networkedComponents = this._comFac.NetworkedComponents;
    EntityEventBus.DirectedEventHandler[] compEventHandlers = new EntityEventBus.DirectedEventHandler[networkedComponents.Count];
    for (int index = 0; index < networkedComponents.Count; ++index)
    {
      ComponentRegistration componentRegistration = networkedComponents[index];
      compEventHandlers[index] = this._compEventSubs[componentRegistration.Idx.Value].GetValueOrDefault<Type, EntityEventBus.DirectedEventHandler>(typeof (TEvent));
    }
    return compEventHandlers;
  }

  private static void CollectBroadcastOrdered(
    EventSource source,
    EntityEventBus.EventData sub,
    ref ValueList<EntityEventBus.OrderedEventDispatch> found)
  {
    Span<EntityEventBus.BroadcastRegistration> span = sub.BroadcastRegistrations.Span;
    for (int index = 0; index < span.Length; ++index)
    {
      EntityEventBus.BroadcastRegistration broadcastRegistration = span[index];
      if ((broadcastRegistration.Mask & source) != EventSource.None)
        found.Add(new EntityEventBus.OrderedEventDispatch(broadcastRegistration.Handler, broadcastRegistration.Order));
    }
  }

  private void RaiseLocalOrdered(
    EntityUid uid,
    Type eventType,
    EntityEventBus.EventData subs,
    ref EntityEventBus.Unit unitRef,
    bool broadcast)
  {
    if (!subs.OrderingUpToDate)
      this.UpdateOrderSeq(eventType, subs);
    ValueList<EntityEventBus.OrderedEventDispatch> found = new ValueList<EntityEventBus.OrderedEventDispatch>();
    if (broadcast)
      EntityEventBus.CollectBroadcastOrdered(EventSource.Local, subs, ref found);
    this.EntCollectOrdered(uid, eventType, ref found);
    EntityEventBus.DispatchOrderedEvents(ref unitRef, ref found);
  }

  private static void DispatchOrderedEvents(
    ref EntityEventBus.Unit eventArgs,
    ref ValueList<EntityEventBus.OrderedEventDispatch> found)
  {
    found.Sort((IComparer<EntityEventBus.OrderedEventDispatch>) EntityEventBus.OrderedEventDispatchComparer.Instance);
    Span<EntityEventBus.OrderedEventDispatch> span = found.Span;
    for (int index = 0; index < span.Length; ++index)
    {
      (EntityEventBus.RefEventHandler Handler, int _) = span[index];
      Handler(ref eventArgs);
    }
  }

  private void UpdateOrderSeq(Type eventType, EntityEventBus.EventData sub)
  {
    IEnumerable<EntityEventBus.OrderedRegistration> orderedRegistrations = (IEnumerable<EntityEventBus.OrderedRegistration>) sub.BroadcastRegistrations;
    HashSet<CompIdx> source1;
    if (this._eventSubsInv.TryGetValue(eventType, out source1))
      orderedRegistrations = orderedRegistrations.Concat<EntityEventBus.OrderedRegistration>((IEnumerable<EntityEventBus.OrderedRegistration>) source1.Select<CompIdx, FrozenDictionary<Type, EntityEventBus.DirectedRegistration>>((Func<CompIdx, FrozenDictionary<Type, EntityEventBus.DirectedRegistration>>) (c => this._eventSubs[c.Value])).Where<FrozenDictionary<Type, EntityEventBus.DirectedRegistration>>((Func<FrozenDictionary<Type, EntityEventBus.DirectedRegistration>, bool>) (c => c != null)).Select<FrozenDictionary<Type, EntityEventBus.DirectedRegistration>, EntityEventBus.DirectedRegistration>((Func<FrozenDictionary<Type, EntityEventBus.DirectedRegistration>, EntityEventBus.DirectedRegistration>) (c => c[eventType])));
    IGrouping<Type, EntityEventBus.OrderedRegistration>[] array = orderedRegistrations.Where<EntityEventBus.OrderedRegistration>((Func<EntityEventBus.OrderedRegistration, bool>) (r => r.Ordering != (EntityEventBus.OrderingData) null)).GroupBy<EntityEventBus.OrderedRegistration, Type>((Func<EntityEventBus.OrderedRegistration, Type>) (b => b.Ordering.OrderType)).ToArray<IGrouping<Type, EntityEventBus.OrderedRegistration>>();
    foreach (IGrouping<Type, EntityEventBus.OrderedRegistration> source2 in array)
    {
      EntityEventBus.OrderingData firstOrder = source2.First<EntityEventBus.OrderedRegistration>().Ordering;
      if (!source2.All<EntityEventBus.OrderedRegistration>((Func<EntityEventBus.OrderedRegistration, bool>) (e => e.Ordering.Equals(firstOrder))))
        throw new InvalidOperationException($"{source2.Key} uses different ordering constraints for different subscriptions to the same event {eventType}. " + "All subscriptions to the same event from the same registrar must use the same ordering.");
    }
    IEnumerable<TopologicalSort.GraphNode<EntityEventBus.OrderedRegistration[]>> nodes = TopologicalSort.FromBeforeAfter<EntityEventBus.OrderedRegistration[], Type, EntityEventBus.OrderedRegistration[]>(((IEnumerable<IGrouping<Type, EntityEventBus.OrderedRegistration>>) array).Select<IGrouping<Type, EntityEventBus.OrderedRegistration>, EntityEventBus.OrderedRegistration[]>((Func<IGrouping<Type, EntityEventBus.OrderedRegistration>, EntityEventBus.OrderedRegistration[]>) (g => g.ToArray<EntityEventBus.OrderedRegistration>())), (Func<EntityEventBus.OrderedRegistration[], Type>) (n => n[0].Ordering.OrderType), (Func<EntityEventBus.OrderedRegistration[], EntityEventBus.OrderedRegistration[]>) (n => n), (Func<EntityEventBus.OrderedRegistration[], IEnumerable<Type>>) (n => (IEnumerable<Type>) n[0].Ordering.Before), (Func<EntityEventBus.OrderedRegistration[], IEnumerable<Type>>) (n => (IEnumerable<Type>) n[0].Ordering.After), true);
    int num = 1;
    foreach (EntityEventBus.OrderedRegistration[] orderedRegistrationArray in TopologicalSort.Sort<EntityEventBus.OrderedRegistration[]>(nodes))
    {
      foreach (EntityEventBus.OrderedRegistration orderedRegistration in orderedRegistrationArray)
        orderedRegistration.Order = num++;
    }
    sub.OrderingUpToDate = true;
    sub.BroadcastRegistrations.Sort((IComparer<EntityEventBus.BroadcastRegistration>) EntityEventBus.RegistrationOrderComparer.Instance);
  }

  public void CalcOrdering()
  {
    foreach ((Type type, EntityEventBus.EventData sub) in this._eventData)
    {
      if (sub.IsOrdered && !sub.OrderingUpToDate)
        this.UpdateOrderSeq(type, sub);
    }
  }

  private EntityEventBus.OrderingData CreateOrderingData(
    Type orderType,
    Type[]? before,
    Type[]? after)
  {
    this.AddChildrenTypes(ref before);
    this.AddChildrenTypes(ref after);
    return new EntityEventBus.OrderingData(orderType, before ?? Array.Empty<Type>(), after ?? Array.Empty<Type>());
  }

  private void AddChildrenTypes(ref Type[]? original)
  {
    if (original == null || original.Length == 0)
      return;
    this._childrenTypesTemp.Clear();
    foreach (Type baseType in original)
    {
      foreach (Type allChild in this._reflection.GetAllChildren(baseType))
        this._childrenTypesTemp.Add(allChild);
    }
    if (this._childrenTypesTemp.Count <= 0)
      return;
    Array.Resize<Type>(ref original, original.Length + this._childrenTypesTemp.Count);
    this._childrenTypesTemp.CopyTo(original, original.Length - this._childrenTypesTemp.Count);
  }

  private delegate void RefEventHandler(ref EntityEventBus.Unit ev);

  private sealed class BroadcastRegistration : 
    EntityEventBus.OrderedRegistration,
    IEquatable<EntityEventBus.BroadcastRegistration>
  {
    public readonly object EqualityToken;
    public readonly EntityEventBus.RefEventHandler Handler;
    public readonly EventSource Mask;
    public readonly bool ReferenceEvent;

    public BroadcastRegistration(
      EventSource mask,
      EntityEventBus.RefEventHandler handler,
      object equalityToken,
      EntityEventBus.OrderingData? ordering,
      bool referenceEvent)
      : base(ordering)
    {
      this.Mask = mask;
      this.Handler = handler;
      this.EqualityToken = equalityToken;
      this.ReferenceEvent = referenceEvent;
    }

    public bool Equals(EntityEventBus.BroadcastRegistration? other)
    {
      return other != null && this.Mask == other.Mask && object.Equals(this.EqualityToken, other.EqualityToken);
    }

    public override bool Equals(object? obj)
    {
      return obj is EntityEventBus.BroadcastRegistration other && this.Equals(other);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine<EventSource, object>(this.Mask, this.EqualityToken);
    }
  }

  private sealed class EventData
  {
    public bool IsOrdered;
    public bool OrderingUpToDate;
    public ValueList<EntityEventBus.BroadcastRegistration> BroadcastRegistrations;
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal readonly struct Unit
  {
  }

  [StructLayout(LayoutKind.Sequential)]
  private sealed class UnitBox
  {
    public EntityEventBus.Unit Value;
  }

  internal delegate void DirectedEventHandler(
    EntityUid uid,
    IComponent comp,
    ref EntityEventBus.Unit args);

  internal sealed class DirectedRegistration(
    EntityEventBus.OrderingData? ordering,
    EntityEventBus.DirectedEventHandler handler) : EntityEventBus.OrderedRegistration(ordering)
  {
    public readonly EntityEventBus.DirectedEventHandler Handler = handler;

    public void SetOrder(int order) => this.Order = order;
  }

  internal sealed class EventTable
  {
    private const int InitialListSize = 8;
    public readonly Dictionary<Type, (int Start, int Count)> EventIndices = new Dictionary<Type, (int, int)>();
    public int Free;
    public EntityEventBus.EventTableListEntry[] ComponentLists = new EntityEventBus.EventTableListEntry[8];

    public EventTable()
    {
      EntityEventBus.InitEventTableFreeList((Span<EntityEventBus.EventTableListEntry>) this.ComponentLists, this.ComponentLists.Length, 0);
      this.Free = 0;
    }
  }

  internal struct EventTableListEntry
  {
    public int Next;
    public CompIdx Component;
  }

  internal sealed record OrderingData(Type OrderType, Type[] Before, Type[] After)
  {
    public bool Equals(EntityEventBus.OrderingData? other)
    {
      return !(other == (EntityEventBus.OrderingData) null) && other.OrderType == this.OrderType && ((ReadOnlySpan<Type>) this.Before.AsSpan<Type>()).SequenceEqual<Type>((ReadOnlySpan<Type>) other.Before, (IEqualityComparer<Type>) null) && ((ReadOnlySpan<Type>) this.After.AsSpan<Type>()).SequenceEqual<Type>((ReadOnlySpan<Type>) other.After, (IEqualityComparer<Type>) null);
    }

    public override int GetHashCode()
    {
      HashCode hc = new HashCode();
      hc.Add<Type>(this.OrderType);
      hc.AddArray<Type>(this.Before);
      hc.AddArray<Type>(this.After);
      return hc.ToHashCode();
    }
  }

  private sealed class RegistrationOrderComparer : IComparer<EntityEventBus.OrderedRegistration>
  {
    public static readonly EntityEventBus.RegistrationOrderComparer Instance = new EntityEventBus.RegistrationOrderComparer();

    public int Compare(EntityEventBus.OrderedRegistration? x, EntityEventBus.OrderedRegistration? y)
    {
      return x.Order.CompareTo(y.Order);
    }
  }

  private record struct OrderedEventDispatch(EntityEventBus.RefEventHandler Handler, int Order);

  private sealed class OrderedEventDispatchComparer : IComparer<EntityEventBus.OrderedEventDispatch>
  {
    public static readonly EntityEventBus.OrderedEventDispatchComparer Instance = new EntityEventBus.OrderedEventDispatchComparer();

    public int Compare(EntityEventBus.OrderedEventDispatch x, EntityEventBus.OrderedEventDispatch y)
    {
      return x.Order.CompareTo(y.Order);
    }
  }

  internal abstract class OrderedRegistration
  {
    public int Order;
    public readonly EntityEventBus.OrderingData? Ordering;

    protected OrderedRegistration(EntityEventBus.OrderingData? ordering)
    {
      this.Ordering = ordering;
    }
  }
}
