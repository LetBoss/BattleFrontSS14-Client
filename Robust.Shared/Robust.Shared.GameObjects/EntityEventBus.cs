using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Robust.Shared.Collections;
using Robust.Shared.Reflection;
using Robust.Shared.Utility;

namespace Robust.Shared.GameObjects;

internal sealed class EntityEventBus : IBroadcastEventBusInternal, IBroadcastEventBus, IEventBus, IDirectedEventBus, IDisposable
{
	private delegate void RefEventHandler(ref Unit ev);

	private sealed class BroadcastRegistration : OrderedRegistration, IEquatable<BroadcastRegistration>
	{
		public readonly object EqualityToken;

		public readonly RefEventHandler Handler;

		public readonly EventSource Mask;

		public readonly bool ReferenceEvent;

		public BroadcastRegistration(EventSource mask, RefEventHandler handler, object equalityToken, OrderingData? ordering, bool referenceEvent)
			: base(ordering)
		{
			Mask = mask;
			Handler = handler;
			EqualityToken = equalityToken;
			ReferenceEvent = referenceEvent;
		}

		public bool Equals(BroadcastRegistration? other)
		{
			if (other != null && Mask == other.Mask)
			{
				return object.Equals(EqualityToken, other.EqualityToken);
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			if (obj is BroadcastRegistration other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Mask, EqualityToken);
		}
	}

	private sealed class EventData
	{
		public bool IsOrdered;

		public bool OrderingUpToDate;

		public ValueList<BroadcastRegistration> BroadcastRegistrations;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal readonly struct Unit
	{
	}

	[StructLayout(LayoutKind.Sequential)]
	private sealed class UnitBox
	{
		public Unit Value;
	}

	internal delegate void DirectedEventHandler(EntityUid uid, IComponent comp, ref Unit args);

	internal sealed class DirectedRegistration(OrderingData? ordering, DirectedEventHandler handler) : OrderedRegistration(ordering)
	{
		public readonly DirectedEventHandler Handler = handler;

		public void SetOrder(int order)
		{
			Order = order;
		}
	}

	internal sealed class EventTable
	{
		private const int InitialListSize = 8;

		public readonly Dictionary<Type, (int Start, int Count)> EventIndices = new Dictionary<Type, (int, int)>();

		public int Free;

		public EventTableListEntry[] ComponentLists = new EventTableListEntry[8];

		public EventTable()
		{
			InitEventTableFreeList(ComponentLists, ComponentLists.Length, 0);
			Free = 0;
		}
	}

	internal struct EventTableListEntry
	{
		public int Next;

		public CompIdx Component;
	}

	internal sealed record OrderingData(Type OrderType, Type[] Before, Type[] After)
	{
		public bool Equals(OrderingData? other)
		{
			if (other == null)
			{
				return false;
			}
			if (other.OrderType == OrderType && ((ReadOnlySpan<Type>)Before.AsSpan()).SequenceEqual((ReadOnlySpan<Type>)other.Before, (IEqualityComparer<Type>?)null))
			{
				return ((ReadOnlySpan<Type>)After.AsSpan()).SequenceEqual((ReadOnlySpan<Type>)other.After, (IEqualityComparer<Type>?)null);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode hc = default(HashCode);
			hc.Add(OrderType);
			hc.AddArray(Before);
			hc.AddArray(After);
			return hc.ToHashCode();
		}
	}

	private sealed class RegistrationOrderComparer : IComparer<OrderedRegistration>
	{
		public static readonly RegistrationOrderComparer Instance = new RegistrationOrderComparer();

		public int Compare(OrderedRegistration? x, OrderedRegistration? y)
		{
			return x.Order.CompareTo(y.Order);
		}
	}

	private record struct OrderedEventDispatch(RefEventHandler Handler, int Order);

	private sealed class OrderedEventDispatchComparer : IComparer<OrderedEventDispatch>
	{
		public static readonly OrderedEventDispatchComparer Instance = new OrderedEventDispatchComparer();

		public int Compare(OrderedEventDispatch x, OrderedEventDispatch y)
		{
			return x.Order.CompareTo(y.Order);
		}
	}

	internal abstract class OrderedRegistration
	{
		public int Order;

		public readonly OrderingData? Ordering;

		protected OrderedRegistration(OrderingData? ordering)
		{
			Ordering = ordering;
		}
	}

	private EntityManager _entMan;

	private IComponentFactory _comFac;

	private IReflectionManager _reflection;

	private FrozenDictionary<Type, EventData> _eventData = FrozenDictionary<Type, EventData>.Empty;

	private readonly Dictionary<Type, EventData> _eventDataUnfrozen = new Dictionary<Type, EventData>();

	private readonly Dictionary<IEntityEventSubscriber, Dictionary<Type, BroadcastRegistration>> _inverseEventSubscriptions = new Dictionary<IEntityEventSubscriber, Dictionary<Type, BroadcastRegistration>>();

	private readonly Queue<(EventSource source, object args)> _eventQueue = new Queue<(EventSource, object)>();

	internal Dictionary<EntityUid, EventTable> _entEventTables = new Dictionary<EntityUid, EventTable>();

	private FrozenDictionary<Type, DirectedRegistration>[] _eventSubs;

	private FrozenDictionary<Type, DirectedEventHandler>[] _compEventSubs;

	private Dictionary<Type, DirectedRegistration>?[] _eventSubsUnfrozen = Array.Empty<Dictionary<Type, DirectedRegistration>>();

	private Dictionary<Type, DirectedEventHandler>?[] _compEventSubsUnfrozen = Array.Empty<Dictionary<Type, DirectedEventHandler>>();

	private Dictionary<Type, HashSet<CompIdx>> _eventSubsInv = new Dictionary<Type, HashSet<CompIdx>>();

	private bool _subscriptionLock;

	public bool IgnoreUnregisteredComponents;

	private readonly List<Type> _childrenTypesTemp = new List<Type>();

	private const int MaxEventLinkedListSize = 256;

	public void UnsubscribeEvents(IEntityEventSubscriber subscriber)
	{
		if (subscriber == null)
		{
			throw new ArgumentNullException("subscriber");
		}
		if (!_inverseEventSubscriptions.TryGetValue(subscriber, out Dictionary<Type, BroadcastRegistration> value))
		{
			return;
		}
		foreach (var (eventType, tuple) in value.ToList())
		{
			UnsubscribeEvent(eventType, tuple, subscriber);
		}
	}

	public void ProcessEventQueue()
	{
		while (_eventQueue.Count != 0)
		{
			(EventSource source, object args) tuple = _eventQueue.Dequeue();
			EventSource item = tuple.source;
			object obj = tuple.args;
			Type type = obj.GetType();
			ProcessSingleEvent(item, ref ExtractUnitRef(ref obj, type), type);
		}
	}

	public void SubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber, EntityEventHandler<T> eventHandler) where T : notnull
	{
		if (eventHandler == null)
		{
			throw new ArgumentNullException("eventHandler");
		}
		SubscribeEventCommon<T>(source, subscriber, delegate(ref Unit ev)
		{
			eventHandler(Unsafe.As<Unit, T>(ref ev));
		}, eventHandler, null, byRef: false);
	}

	public void SubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber, EntityEventHandler<T> eventHandler, Type orderType, Type[]? before = null, Type[]? after = null) where T : notnull
	{
		if (eventHandler == null)
		{
			throw new ArgumentNullException("eventHandler");
		}
		OrderingData order = CreateOrderingData(orderType, before, after);
		SubscribeEventCommon<T>(source, subscriber, delegate(ref Unit ev)
		{
			eventHandler(Unsafe.As<Unit, T>(ref ev));
		}, eventHandler, order, byRef: false);
	}

	public void SubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber, EntityEventRefHandler<T> eventHandler) where T : notnull
	{
		SubscribeEventCommon<T>(source, subscriber, delegate(ref Unit ev)
		{
			ref T ev2 = ref Unsafe.As<Unit, T>(ref ev);
			eventHandler(ref ev2);
		}, eventHandler, null, byRef: true);
	}

	public void SubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber, EntityEventRefHandler<T> eventHandler, Type orderType, Type[]? before = null, Type[]? after = null) where T : notnull
	{
		OrderingData order = CreateOrderingData(orderType, before, after);
		SubscribeEventCommon<T>(source, subscriber, delegate(ref Unit ev)
		{
			ref T ev2 = ref Unsafe.As<Unit, T>(ref ev);
			eventHandler(ref ev2);
		}, eventHandler, order, byRef: true);
	}

	private void SubscribeEventCommon<T>(EventSource source, IEntityEventSubscriber subscriber, RefEventHandler handler, object equalityToken, OrderingData? order, bool byRef) where T : notnull
	{
		if (source == EventSource.None)
		{
			throw new ArgumentOutOfRangeException("source");
		}
		if (subscriber == null)
		{
			throw new ArgumentNullException("subscriber");
		}
		Type typeFromHandle = typeof(T);
		bool flag = typeFromHandle.HasCustomAttribute<ByRefEventAttribute>();
		if (flag != byRef)
		{
			throw new InvalidOperationException($"Attempted to subscribe by-ref and by-value to the same broadcast event! event={typeFromHandle} eventIsByRef={flag} subscriptionIsByRef={byRef}");
		}
		BroadcastRegistration broadcastRegistration = new BroadcastRegistration(source, handler, equalityToken, order, byRef);
		RegisterCommon(typeFromHandle, order, out EventData subs);
		if (!subs.BroadcastRegistrations.Contains(broadcastRegistration))
		{
			subs.BroadcastRegistrations.Add(broadcastRegistration);
		}
		if (!_inverseEventSubscriptions.GetOrNew(subscriber).TryAdd(typeFromHandle, broadcastRegistration))
		{
			throw new InvalidOperationException(subscriber.GetType().Name + " attempted to subscribe twice to the same event: " + typeFromHandle.Name);
		}
	}

	public void UnsubscribeEvent<T>(EventSource source, IEntityEventSubscriber subscriber) where T : notnull
	{
		if (source == EventSource.None)
		{
			throw new ArgumentOutOfRangeException("source");
		}
		if (subscriber == null)
		{
			throw new ArgumentNullException("subscriber");
		}
		Type typeFromHandle = typeof(T);
		if (_inverseEventSubscriptions.TryGetValue(subscriber, out Dictionary<Type, BroadcastRegistration> value) && value.TryGetValue(typeFromHandle, out var value2))
		{
			UnsubscribeEvent(typeFromHandle, value2, subscriber);
		}
	}

	public void RaiseEvent(EventSource source, object toRaise)
	{
		if (source == EventSource.None)
		{
			throw new ArgumentOutOfRangeException("source");
		}
		Type type = toRaise.GetType();
		ProcessSingleEvent(source, ref ExtractUnitRef(ref toRaise, type), type);
	}

	public void RaiseEvent<T>(EventSource source, T toRaise) where T : notnull
	{
		if (source == EventSource.None)
		{
			throw new ArgumentOutOfRangeException("source");
		}
		ProcessSingleEvent(source, ref Unsafe.As<T, Unit>(ref toRaise), typeof(T));
	}

	public void RaiseEvent<T>(EventSource source, ref T toRaise) where T : notnull
	{
		if (source == EventSource.None)
		{
			throw new ArgumentOutOfRangeException("source");
		}
		ProcessSingleEvent(source, ref Unsafe.As<T, Unit>(ref toRaise), typeof(T));
	}

	public void QueueEvent(EventSource source, EntityEventArgs toRaise)
	{
		if (source == EventSource.None)
		{
			throw new ArgumentOutOfRangeException("source");
		}
		if (toRaise == null)
		{
			throw new ArgumentNullException("toRaise");
		}
		_eventQueue.Enqueue((source, toRaise));
	}

	private void UnsubscribeEvent(Type eventType, BroadcastRegistration tuple, IEntityEventSubscriber subscriber)
	{
		if (_subscriptionLock)
		{
			throw new InvalidOperationException("Subscription locked.");
		}
		if (_eventDataUnfrozen.TryGetValue(eventType, out EventData value) && value.BroadcastRegistrations.Contains(tuple))
		{
			value.BroadcastRegistrations.Remove(tuple);
		}
		if (_inverseEventSubscriptions.TryGetValue(subscriber, out Dictionary<Type, BroadcastRegistration> value2) && value2.ContainsKey(eventType))
		{
			value2.Remove(eventType);
		}
	}

	private void ProcessSingleEvent(EventSource source, ref Unit unitRef, Type eventType)
	{
		if (_eventData.TryGetValue(eventType, out EventData value))
		{
			if (value.IsOrdered && !value.OrderingUpToDate)
			{
				UpdateOrderSeq(eventType, value);
			}
			ProcessSingleEventCore(source, ref unitRef, value);
		}
	}

	private static void ProcessSingleEventCore(EventSource source, ref Unit unitRef, EventData subs)
	{
		Span<BroadcastRegistration> span = subs.BroadcastRegistrations.Span;
		for (int i = 0; i < span.Length; i++)
		{
			BroadcastRegistration broadcastRegistration = span[i];
			if ((broadcastRegistration.Mask & source) != EventSource.None)
			{
				broadcastRegistration.Handler(ref unitRef);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ref Unit ExtractUnitRef(ref object obj, Type objType)
	{
		if (!objType.IsValueType)
		{
			return ref Unsafe.As<object, Unit>(ref obj);
		}
		return ref Unsafe.As<object, UnitBox>(ref obj).Value;
	}

	private void RegisterCommon(Type eventType, OrderingData? data, out EventData subs)
	{
		if (_subscriptionLock)
		{
			throw new InvalidOperationException("Subscription locked.");
		}
		subs = _eventDataUnfrozen.GetOrNew(eventType);
		if (!(data == null) && (data.Before.Length != 0 || data.After.Length != 0))
		{
			subs.IsOrdered = true;
			subs.OrderingUpToDate = false;
		}
	}

	public EntityEventBus(EntityManager entMan, IReflectionManager reflection)
	{
		_entMan = entMan;
		_comFac = entMan.ComponentFactory;
		_reflection = reflection;
		_comFac.ComponentsAdded += ComFacOnComponentsAdded;
		ComFacOnComponentsAdded(_comFac.GetAllRegistrations().ToArray());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, TEvent args) where TEvent : notnull
	{
		RaiseComponentEvent(uid, component, _comFac.GetIndex(component.GetType()), ref args);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RaiseComponentEvent<TEvent, TComponent>(EntityUid uid, TComponent component, TEvent args) where TEvent : notnull where TComponent : IComponent
	{
		RaiseComponentEvent(uid, component, CompIdx.Index<TComponent>(), ref args);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, CompIdx type, TEvent args) where TEvent : notnull
	{
		RaiseComponentEvent(uid, component, type, ref args);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, ref TEvent args) where TEvent : notnull
	{
		RaiseComponentEvent(uid, component, _comFac.GetIndex(component.GetType()), ref args);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RaiseComponentEvent<TEvent, TComponent>(EntityUid uid, TComponent component, ref TEvent args) where TEvent : notnull where TComponent : IComponent
	{
		RaiseComponentEvent(uid, component, CompIdx.Index<TComponent>(), ref args);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, CompIdx type, ref TEvent args) where TEvent : notnull
	{
		if (_compEventSubs[type.Value].TryGetValue(typeof(TEvent), out DirectedEventHandler value))
		{
			value(uid, component, ref Unsafe.As<TEvent, Unit>(ref args));
		}
	}

	public void OnlyCallOnRobustUnitTestISwearToGodPleaseSomebodyKillThisNightmare()
	{
		IgnoreUnregisteredComponents = true;
	}

	public void RaiseLocalEvent<TEvent>(EntityUid uid, TEvent args, bool broadcast = false) where TEvent : notnull
	{
		Type typeFromHandle = typeof(TEvent);
		RaiseLocalEventCore(uid, ref Unsafe.As<TEvent, Unit>(ref args), typeFromHandle, broadcast);
	}

	public void RaiseLocalEvent(EntityUid uid, object args, bool broadcast = false)
	{
		Type type = args.GetType();
		RaiseLocalEventCore(uid, ref Unsafe.As<object, Unit>(ref args), type, broadcast);
	}

	public void RaiseLocalEvent<TEvent>(EntityUid uid, ref TEvent args, bool broadcast = false) where TEvent : notnull
	{
		Type typeFromHandle = typeof(TEvent);
		RaiseLocalEventCore(uid, ref Unsafe.As<TEvent, Unit>(ref args), typeFromHandle, broadcast);
	}

	public void RaiseLocalEvent(EntityUid uid, ref object args, bool broadcast = false)
	{
		Type type = args.GetType();
		RaiseLocalEventCore(uid, ref Unsafe.As<object, Unit>(ref args), type, broadcast);
	}

	private void RaiseLocalEventCore(EntityUid uid, ref Unit unitRef, Type type, bool broadcast)
	{
		if (!_eventData.TryGetValue(type, out EventData value))
		{
			return;
		}
		if (value.IsOrdered)
		{
			RaiseLocalOrdered(uid, type, value, ref unitRef, broadcast);
			return;
		}
		EntDispatch(uid, type, ref unitRef);
		if (broadcast)
		{
			ProcessSingleEventCore(EventSource.Local, ref unitRef, value);
		}
	}

	public void SubscribeLocalEvent<TComp, TEvent>(ComponentEventHandler<TComp, TEvent> handler) where TComp : IComponent where TEvent : notnull
	{
		EntAddSubscription(CompIdx.Index<TComp>(), typeof(TComp), typeof(TEvent), EventHandler);
		void EventHandler(EntityUid uid, IComponent comp, ref Unit ev)
		{
			ref TEvent reference = ref Unsafe.As<Unit, TEvent>(ref ev);
			handler(uid, (TComp)comp, reference);
		}
	}

	public void SubscribeLocalEvent<TComp, TEvent>(ComponentEventHandler<TComp, TEvent> handler, Type orderType, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull
	{
		EntAddSubscription(CompIdx.Index<TComp>(), typeof(TComp), typeof(TEvent), EventHandler, orderType, before, after);
		void EventHandler(EntityUid uid, IComponent comp, ref Unit ev)
		{
			ref TEvent reference = ref Unsafe.As<Unit, TEvent>(ref ev);
			handler(uid, (TComp)comp, reference);
		}
	}

	public void SubscribeLocalEvent<TComp, TEvent>(ComponentEventRefHandler<TComp, TEvent> handler) where TComp : IComponent where TEvent : notnull
	{
		EntAddSubscription(CompIdx.Index<TComp>(), typeof(TComp), typeof(TEvent), EventHandler);
		void EventHandler(EntityUid uid, IComponent comp, ref Unit ev)
		{
			ref TEvent args = ref Unsafe.As<Unit, TEvent>(ref ev);
			handler(uid, (TComp)comp, ref args);
		}
	}

	public void SubscribeLocalEvent<TComp, TEvent>(ComponentEventRefHandler<TComp, TEvent> handler, Type orderType, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull
	{
		EntAddSubscription(CompIdx.Index<TComp>(), typeof(TComp), typeof(TEvent), EventHandler, orderType, before, after);
		void EventHandler(EntityUid uid, IComponent comp, ref Unit ev)
		{
			ref TEvent args = ref Unsafe.As<Unit, TEvent>(ref ev);
			handler(uid, (TComp)comp, ref args);
		}
	}

	public void SubscribeLocalEvent<TComp, TEvent>(EntityEventRefHandler<TComp, TEvent> handler, Type orderType, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull
	{
		EntAddSubscription(CompIdx.Index<TComp>(), typeof(TComp), typeof(TEvent), EventHandler, orderType, before, after);
		void EventHandler(EntityUid uid, IComponent comp, ref Unit ev)
		{
			ref TEvent args = ref Unsafe.As<Unit, TEvent>(ref ev);
			handler(new Entity<TComp>(uid, (TComp)comp), ref args);
		}
	}

	public void UnsubscribeLocalEvent<TComp, TEvent>() where TComp : IComponent where TEvent : notnull
	{
		if (!_comFac.TryGetRegistration(typeof(TComp), out ComponentRegistration _))
		{
			if (!IgnoreUnregisteredComponents)
			{
				throw new InvalidOperationException("Component is not a valid reference type: " + typeof(TComp).Name);
			}
			return;
		}
		if (_subscriptionLock)
		{
			throw new InvalidOperationException("Subscription locked.");
		}
		int num = CompIdx.ArrayIndex<TComp>();
		_eventSubsUnfrozen[num].Remove(typeof(TEvent));
		_compEventSubsUnfrozen[num].Remove(typeof(TEvent));
		if (_eventSubsInv.TryGetValue(typeof(TEvent), out HashSet<CompIdx> value))
		{
			value.Remove(CompIdx.Index<TComp>());
		}
	}

	private void ComFacOnComponentsAdded(ComponentRegistration[] regs)
	{
		if (_subscriptionLock)
		{
			throw new InvalidOperationException("Subscription locked.");
		}
		foreach (ComponentRegistration componentRegistration in regs)
		{
			ref Dictionary<Type, DirectedRegistration> reference = ref CompIdx.RefArray(ref _eventSubsUnfrozen, componentRegistration.Idx);
			if (reference == null)
			{
				reference = new Dictionary<Type, DirectedRegistration>();
			}
			ref Dictionary<Type, DirectedEventHandler> reference2 = ref CompIdx.RefArray(ref _compEventSubsUnfrozen, componentRegistration.Idx);
			if (reference2 == null)
			{
				reference2 = new Dictionary<Type, DirectedEventHandler>();
			}
		}
	}

	public void OnEntityAdded(EntityUid e)
	{
		EntAddEntity(e);
	}

	public void OnEntityDeleted(EntityUid e)
	{
		EntRemoveEntity(e);
	}

	public void OnComponentAdded(in AddedComponentEventArgs e)
	{
		EntAddComponent(e.BaseArgs.Owner, e.ComponentType.Idx);
	}

	internal void LockSubscriptions()
	{
		_subscriptionLock = true;
		_eventData = _eventDataUnfrozen.ToFrozenDictionary();
		_eventSubs = (from dict in TrimNull(_eventSubsUnfrozen)
			select dict?.ToFrozenDictionary()).ToArray();
		_compEventSubs = (from dict in TrimNull(_compEventSubsUnfrozen)
			select dict?.ToFrozenDictionary()).ToArray();
		CalcOrdering();
	}

	public void OnComponentRemoved(in RemovedComponentEventArgs e)
	{
		EntRemoveComponent(e.BaseArgs.Owner, e.Idx);
	}

	private void EntAddSubscription(CompIdx compType, Type compTypeObj, Type eventType, DirectedEventHandler handler, Type? orderType = null, Type[]? before = null, Type[]? after = null)
	{
		if (_subscriptionLock)
		{
			throw new InvalidOperationException("Subscription locked.");
		}
		if (!_comFac.TryGetRegistration(compTypeObj, out ComponentRegistration _))
		{
			if (!IgnoreUnregisteredComponents)
			{
				throw new InvalidOperationException("Component is not a valid reference type: " + compTypeObj.Name);
			}
			return;
		}
		ComponentEventAttribute customAttribute = eventType.GetCustomAttribute<ComponentEventAttribute>();
		if (customAttribute != null)
		{
			if (!_compEventSubsUnfrozen[compType.Value].TryAdd(eventType, handler))
			{
				throw new InvalidOperationException($"Duplicate Subscriptions for comp={compTypeObj}, event={eventType.Name}");
			}
			if (customAttribute.Exclusive)
			{
				return;
			}
		}
		DirectedRegistration directedRegistration = new DirectedRegistration((orderType == null) ? null : CreateOrderingData(orderType, before, after), handler);
		if (!_eventSubsUnfrozen[compType.Value].TryAdd(eventType, directedRegistration))
		{
			throw new InvalidOperationException($"Duplicate Subscriptions for comp={compTypeObj}, event={eventType.Name}");
		}
		RegisterCommon(eventType, directedRegistration.Ordering, out EventData _);
		_eventSubsInv.GetOrNew(eventType).Add(compType);
	}

	private void EntAddEntity(EntityUid euid)
	{
		_entEventTables.Add(euid, new EventTable());
	}

	private void EntRemoveEntity(EntityUid euid)
	{
		_entEventTables.Remove(euid);
	}

	private void EntAddComponent(EntityUid euid, CompIdx compType)
	{
		EventTable eventTable = _entEventTables[euid];
		ImmutableArray<Type>.Enumerator enumerator = _eventSubs[compType.Value].Keys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Type current = enumerator.Current;
			if (eventTable.Free < 0)
			{
				GrowEventTable(eventTable);
			}
			bool exists;
			ref(int, int) valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(eventTable.EventIndices, current, out exists);
			int free = eventTable.Free;
			ref EventTableListEntry reference = ref eventTable.ComponentLists[free];
			eventTable.Free = reference.Next;
			reference.Component = compType;
			reference.Next = (exists ? valueRefOrAddDefault.Item1 : (-1));
			valueRefOrAddDefault.Item1 = free;
			valueRefOrAddDefault.Item2++;
			if (valueRefOrAddDefault.Item2 > 256)
			{
				throw new NotSupportedException("Exceeded maximum event linked list size. Need to implement stackalloc fallback.");
			}
		}
	}

	private static void GrowEventTable(EventTable table)
	{
		int length = table.ComponentLists.Length * 2;
		EventTableListEntry[] componentLists = table.ComponentLists;
		EventTableListEntry[] array = GC.AllocateUninitializedArray<EventTableListEntry>(length);
		Array.Copy(componentLists, array, componentLists.Length);
		InitEventTableFreeList(array, array.Length, componentLists.Length);
		table.Free = componentLists.Length;
		table.ComponentLists = array;
	}

	private static void InitEventTableFreeList(Span<EventTableListEntry> entries, int end, int start)
	{
		int next = -1;
		for (int num = end - 1; num >= start; num--)
		{
			ref EventTableListEntry reference = ref entries[num];
			reference.Component = default(CompIdx);
			reference.Next = next;
			next = num;
		}
	}

	private void EntRemoveComponent(EntityUid euid, CompIdx compType)
	{
		EventTable eventTable = _entEventTables[euid];
		ImmutableArray<Type>.Enumerator enumerator = _eventSubs[compType.Value].Keys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Type current = enumerator.Current;
			ref(int, int) valueRefOrNullRef = ref CollectionsMarshal.GetValueRefOrNullRef(eventTable.EventIndices, current);
			if (Unsafe.IsNullRef(in valueRefOrNullRef))
			{
				continue;
			}
			int num = valueRefOrNullRef.Item1;
			ref EventTableListEntry reference = ref eventTable.ComponentLists[num];
			if (valueRefOrNullRef.Item2 == 1)
			{
				eventTable.EventIndices.Remove(current);
			}
			else
			{
				ref int reference2 = ref valueRefOrNullRef.Item1;
				while (reference.Component != compType)
				{
					reference2 = ref reference.Next;
					num = reference.Next;
					reference = ref eventTable.ComponentLists[num];
				}
				reference2 = reference.Next;
				valueRefOrNullRef.Item2--;
			}
			reference.Next = eventTable.Free;
			eventTable.Free = num;
		}
	}

	private void EntDispatch(EntityUid euid, Type eventType, ref Unit args)
	{
		if (!_entEventTables.TryGetValue(euid, out EventTable value) || !value.EventIndices.TryGetValue(eventType, out (int, int) value2))
		{
			return;
		}
		Span<CompIdx> span = stackalloc CompIdx[value2.Item2];
		int num = value2.Item1;
		for (int i = 0; i < span.Length; i++)
		{
			ref EventTableListEntry reference = ref value.ComponentLists[num];
			num = reference.Next;
			span[i] = reference.Component;
		}
		Span<CompIdx> span2 = span;
		for (int j = 0; j < span2.Length; j++)
		{
			CompIdx type = span2[j];
			if (_entMan.TryGetComponent(euid, type, out IComponent component))
			{
				_eventSubs[type.Value][eventType].Handler(euid, component, ref args);
			}
		}
	}

	private void EntCollectOrdered(EntityUid euid, Type eventType, ref ValueList<OrderedEventDispatch> found)
	{
		if (!_entEventTables.TryGetValue(euid, out EventTable value) || !value.EventIndices.TryGetValue(eventType, out (int, int) value2))
		{
			return;
		}
		var (num, _) = value2;
		while (num != -1)
		{
			ref EventTableListEntry reference = ref value.ComponentLists[num];
			num = reference.Next;
			IComponent comp = _entMan.GetComponentInternal(euid, reference.Component);
			FrozenDictionary<Type, DirectedRegistration> frozenDictionary = _eventSubs[reference.Component.Value];
			DirectedRegistration reg = frozenDictionary[eventType];
			found.Add(new OrderedEventDispatch(delegate(ref Unit ev)
			{
				if (!comp.Deleted)
				{
					reg.Handler(euid, comp, ref ev);
				}
			}, reg.Order));
		}
	}

	public void ClearSubscriptions()
	{
		_subscriptionLock = false;
		_eventDataUnfrozen.Clear();
		_entEventTables.Clear();
		_inverseEventSubscriptions.Clear();
		_compEventSubs = null;
		_eventSubs = null;
		_eventData = FrozenDictionary<Type, EventData>.Empty;
		Dictionary<Type, DirectedRegistration>[] eventSubsUnfrozen = _eventSubsUnfrozen;
		for (int i = 0; i < eventSubsUnfrozen.Length; i++)
		{
			eventSubsUnfrozen[i]?.Clear();
		}
		Dictionary<Type, DirectedEventHandler>[] compEventSubsUnfrozen = _compEventSubsUnfrozen;
		for (int i = 0; i < compEventSubsUnfrozen.Length; i++)
		{
			compEventSubsUnfrozen[i]?.Clear();
		}
	}

	public void Dispose()
	{
		_comFac.ComponentsAdded -= ComFacOnComponentsAdded;
		_entMan = null;
		_comFac = null;
		_reflection = null;
		_entEventTables = null;
		_compEventSubs = null;
		_eventSubs = null;
		_eventSubsUnfrozen = null;
		_compEventSubsUnfrozen = null;
		_eventSubsInv = null;
	}

	public static T[] TrimNull<T>(T[] input)
	{
		int num = 0;
		for (int i = 0; i < input.Length; i++)
		{
			if (input[i] != null)
			{
				num = i;
			}
		}
		return input[..(num + 1)];
	}

	internal DirectedEventHandler?[] GetNetCompEventHandlers<TEvent>()
	{
		IReadOnlyList<ComponentRegistration> networkedComponents = _comFac.NetworkedComponents;
		DirectedEventHandler[] array = new DirectedEventHandler[networkedComponents.Count];
		for (int i = 0; i < networkedComponents.Count; i++)
		{
			ComponentRegistration componentRegistration = networkedComponents[i];
			array[i] = _compEventSubs[componentRegistration.Idx.Value].GetValueOrDefault(typeof(TEvent));
		}
		return array;
	}

	private static void CollectBroadcastOrdered(EventSource source, EventData sub, ref ValueList<OrderedEventDispatch> found)
	{
		Span<BroadcastRegistration> span = sub.BroadcastRegistrations.Span;
		for (int i = 0; i < span.Length; i++)
		{
			BroadcastRegistration broadcastRegistration = span[i];
			if ((broadcastRegistration.Mask & source) != EventSource.None)
			{
				found.Add(new OrderedEventDispatch(broadcastRegistration.Handler, broadcastRegistration.Order));
			}
		}
	}

	private void RaiseLocalOrdered(EntityUid uid, Type eventType, EventData subs, ref Unit unitRef, bool broadcast)
	{
		if (!subs.OrderingUpToDate)
		{
			UpdateOrderSeq(eventType, subs);
		}
		ValueList<OrderedEventDispatch> found = default(ValueList<OrderedEventDispatch>);
		if (broadcast)
		{
			CollectBroadcastOrdered(EventSource.Local, subs, ref found);
		}
		EntCollectOrdered(uid, eventType, ref found);
		DispatchOrderedEvents(ref unitRef, ref found);
	}

	private static void DispatchOrderedEvents(ref Unit eventArgs, ref ValueList<OrderedEventDispatch> found)
	{
		found.Sort(OrderedEventDispatchComparer.Instance);
		Span<OrderedEventDispatch> span = found.Span;
		for (int i = 0; i < span.Length; i++)
		{
			OrderedEventDispatch orderedEventDispatch = span[i];
			orderedEventDispatch.Deconstruct(out RefEventHandler Handler, out int _);
			Handler(ref eventArgs);
		}
	}

	private void UpdateOrderSeq(Type eventType, EventData sub)
	{
		IEnumerable<OrderedRegistration> enumerable = sub.BroadcastRegistrations;
		if (_eventSubsInv.TryGetValue(eventType, out HashSet<CompIdx> value))
		{
			enumerable = enumerable.Concat(from c in value
				select _eventSubs[c.Value] into c
				where c != null
				select c[eventType]);
		}
		IGrouping<Type, OrderedRegistration>[] array = (from b in enumerable
			where b.Ordering != null
			group b by b.Ordering.OrderType).ToArray();
		IGrouping<Type, OrderedRegistration>[] array2 = array;
		foreach (IGrouping<Type, OrderedRegistration> grouping in array2)
		{
			OrderingData firstOrder = grouping.First().Ordering;
			if (!grouping.All((OrderedRegistration e) => e.Ordering.Equals(firstOrder)))
			{
				throw new InvalidOperationException($"{grouping.Key} uses different ordering constraints for different subscriptions to the same event {eventType}. " + "All subscriptions to the same event from the same registrar must use the same ordering.");
			}
		}
		IEnumerable<TopologicalSort.GraphNode<OrderedRegistration[]>> nodes = TopologicalSort.FromBeforeAfter(array.Select((IGrouping<Type, OrderedRegistration> g) => g.ToArray()), (OrderedRegistration[] n) => n[0].Ordering.OrderType, (OrderedRegistration[] n) => n, (OrderedRegistration[] n) => n[0].Ordering.Before, (OrderedRegistration[] n) => n[0].Ordering.After, allowMissing: true);
		int num2 = 1;
		foreach (OrderedRegistration[] item in TopologicalSort.Sort(nodes))
		{
			for (int num = 0; num < item.Length; num++)
			{
				item[num].Order = num2++;
			}
		}
		sub.OrderingUpToDate = true;
		sub.BroadcastRegistrations.Sort(RegistrationOrderComparer.Instance);
	}

	public void CalcOrdering()
	{
		foreach (var (eventType, eventData2) in _eventData)
		{
			if (eventData2.IsOrdered && !eventData2.OrderingUpToDate)
			{
				UpdateOrderSeq(eventType, eventData2);
			}
		}
	}

	private OrderingData CreateOrderingData(Type orderType, Type[]? before, Type[]? after)
	{
		AddChildrenTypes(ref before);
		AddChildrenTypes(ref after);
		return new OrderingData(orderType, before ?? Array.Empty<Type>(), after ?? Array.Empty<Type>());
	}

	private void AddChildrenTypes(ref Type[]? original)
	{
		if (original == null || original.Length == 0)
		{
			return;
		}
		_childrenTypesTemp.Clear();
		Type[] array = original;
		foreach (Type baseType in array)
		{
			foreach (Type allChild in _reflection.GetAllChildren(baseType))
			{
				_childrenTypesTemp.Add(allChild);
			}
		}
		if (_childrenTypesTemp.Count > 0)
		{
			Array.Resize(ref original, original.Length + _childrenTypesTemp.Count);
			_childrenTypesTemp.CopyTo(original, original.Length - _childrenTypesTemp.Count);
		}
	}
}
