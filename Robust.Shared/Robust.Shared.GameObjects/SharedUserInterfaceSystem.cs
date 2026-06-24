using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Robust.Shared.Collections;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Threading;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Robust.Shared.GameObjects;

public abstract class SharedUserInterfaceSystem : EntitySystem
{
	private record struct ActorRangeCheckJob() : IParallelRobustJob, IParallelRangeRobustJob
	{
		public required EntityQuery<TransformComponent> XformQuery = default(EntityQuery<TransformComponent>);

		public required SharedUserInterfaceSystem System = null;

		public readonly List<(EntityUid Ui, Enum Key, InterfaceData Data, EntityUid Actor, bool Result)> ActorRanges = new List<(EntityUid, Enum, InterfaceData, EntityUid, bool)>();

		public void Execute(int index)
		{
			(EntityUid, Enum, InterfaceData, EntityUid, bool) value = ActorRanges[index];
			if (!XformQuery.TryComp(value.Item1, out TransformComponent component) || !XformQuery.TryComp(value.Item4, out TransformComponent component2))
			{
				value.Item5 = false;
			}
			else
			{
				value.Item5 = System.CheckRange((Owner: value.Item1, Comp: component), value.Item2, value.Item3, (Owner: value.Item4, Comp: component2));
			}
			ActorRanges[index] = value;
		}
	}

	[Dependency]
	private readonly IDynamicTypeFactory _factory;

	[Dependency]
	private readonly IGameTiming _timing;

	[Dependency]
	private readonly INetManager _netManager;

	[Dependency]
	private readonly IParallelManager _parallel;

	[Dependency]
	protected readonly IPrototypeManager ProtoManager;

	[Dependency]
	private readonly IReflectionManager _reflection;

	[Dependency]
	protected readonly ISharedPlayerManager Player;

	[Dependency]
	private readonly SharedTransformSystem _transforms;

	private EntityQuery<IgnoreUIRangeComponent> _ignoreUIRangeQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	protected EntityQuery<UserInterfaceComponent> UIQuery;

	protected EntityQuery<UserInterfaceUserComponent> UserQuery;

	private ActorRangeCheckJob _rangeJob;

	private readonly List<(BoundUserInterface Bui, bool value)> _queuedBuis = new List<(BoundUserInterface, bool)>();

	public override void Initialize()
	{
		base.Initialize();
		EntityManager.ComponentFactory.RegisterNetworkedFields<UserInterfaceComponent>(new string[3] { "Actors", "Interfaces", "States" });
		_ignoreUIRangeQuery = GetEntityQuery<IgnoreUIRangeComponent>();
		_xformQuery = GetEntityQuery<TransformComponent>();
		UIQuery = GetEntityQuery<UserInterfaceComponent>();
		UserQuery = GetEntityQuery<UserInterfaceUserComponent>();
		_rangeJob = new ActorRangeCheckJob
		{
			System = this,
			XformQuery = _xformQuery
		};
		SubscribeAllEvent(delegate(BoundUIWrapMessage msg, EntitySessionEventArgs args)
		{
			EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
				OnMessageReceived(msg, valueOrDefault);
			}
		});
		SubscribeLocalEvent<UserInterfaceComponent, OpenBoundInterfaceMessage>(OnUserInterfaceOpen);
		SubscribeLocalEvent<UserInterfaceComponent, CloseBoundInterfaceMessage>(OnUserInterfaceClosed);
		SubscribeLocalEvent<UserInterfaceComponent, ComponentStartup>(OnUserInterfaceStartup);
		SubscribeLocalEvent<UserInterfaceComponent, ComponentShutdown>(OnUserInterfaceShutdown);
		SubscribeLocalEvent<UserInterfaceComponent, ComponentGetState>(OnUserInterfaceGetState);
		SubscribeLocalEvent<UserInterfaceComponent, ComponentHandleState>(OnUserInterfaceHandleState);
		SubscribeLocalEvent<PlayerAttachedEvent>(OnPlayerAttached);
		SubscribeLocalEvent<PlayerDetachedEvent>(OnPlayerDetached);
		SubscribeLocalEvent<UserInterfaceUserComponent, ComponentShutdown>(OnActorShutdown);
	}

	private void AddQueued(BoundUserInterface bui, bool value)
	{
		_queuedBuis.Add((bui, value));
	}

	private void OnMessageReceived(BoundUIWrapMessage msg, EntityUid sender)
	{
		EntityUid entity = GetEntity(msg.Entity);
		if (!UIQuery.TryComp(entity, out UserInterfaceComponent component))
		{
			return;
		}
		if (!component.Interfaces.TryGetValue(msg.UiKey, out InterfaceData value))
		{
			base.Log.Debug($"Got BoundInterfaceMessageWrapMessage for unknown UI key: {msg.UiKey}");
			return;
		}
		if (!(msg.Message is OpenBoundInterfaceMessage) && (!component.Actors.TryGetValue(msg.UiKey, out HashSet<EntityUid> value2) || !value2.Contains(sender)))
		{
			base.Log.Debug($"UI {msg.UiKey} got BoundInterfaceMessageWrapMessage from a client who was not subscribed: {ToPrettyString(sender)}");
			return;
		}
		if (!(msg.Message is CloseBoundInterfaceMessage) && value.RequireInputValidation)
		{
			BoundUserInterfaceMessageAttempt boundUserInterfaceMessageAttempt = new BoundUserInterfaceMessageAttempt(sender, entity, msg.UiKey, msg.Message);
			RaiseLocalEvent(boundUserInterfaceMessageAttempt);
			if (boundUserInterfaceMessageAttempt.Cancelled)
			{
				return;
			}
			RaiseLocalEvent(entity, boundUserInterfaceMessageAttempt);
			if (boundUserInterfaceMessageAttempt.Cancelled)
			{
				return;
			}
		}
		BoundUserInterfaceMessage message = msg.Message;
		message.Actor = sender;
		message.Entity = msg.Entity;
		message.UiKey = msg.UiKey;
		if (component.ClientOpenInterfaces.TryGetValue(msg.UiKey, out BoundUserInterface value3))
		{
			value3.ReceiveMessage(message);
		}
		RaiseLocalEvent(entity, (object)message, true);
	}

	private void OnActorShutdown(Entity<UserInterfaceUserComponent> ent, ref ComponentShutdown args)
	{
		CloseUserUis((Owner: ent.Owner, Comp: ent.Comp));
	}

	private void OnPlayerAttached(PlayerAttachedEvent ev)
	{
		if (!UserQuery.TryGetComponent(ev.Entity, out UserInterfaceUserComponent component))
		{
			return;
		}
		foreach (var (entityUid2, list2) in component.OpenInterfaces)
		{
			if (!UIQuery.TryGetComponent(entityUid2, out UserInterfaceComponent component2))
			{
				continue;
			}
			Dirty(entityUid2, component2);
			foreach (Enum item in list2)
			{
				if (component2.Interfaces.TryGetValue(item, out InterfaceData value))
				{
					EnsureClientBui((Owner: entityUid2, Comp: component2), item, value);
				}
			}
		}
	}

	private void OnPlayerDetached(PlayerDetachedEvent ev)
	{
		if (!UserQuery.TryGetComponent(ev.Entity, out UserInterfaceUserComponent component))
		{
			return;
		}
		foreach (var (uid, list2) in component.OpenInterfaces)
		{
			if (!UIQuery.TryGetComponent(uid, out UserInterfaceComponent component2))
			{
				continue;
			}
			Dirty(uid, component2);
			foreach (Enum item in list2)
			{
				if (component2.ClientOpenInterfaces.Remove(item, out BoundUserInterface value))
				{
					value.Dispose();
				}
			}
		}
	}

	private void OnUserInterfaceClosed(Entity<UserInterfaceComponent> ent, ref CloseBoundInterfaceMessage args)
	{
		CloseUiInternal(ent, args.UiKey, args.Actor);
	}

	private void CloseUiInternal(Entity<UserInterfaceComponent?> ent, Enum key, EntityUid actor)
	{
		if (!UIQuery.Resolve(ent.Owner, ref ent.Comp, logMissing: false) || !ent.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value))
		{
			return;
		}
		value.Remove(actor);
		if (value.Count == 0)
		{
			ent.Comp.Actors.Remove(key);
		}
		DirtyField(ent, "Actors");
		if (!TerminatingOrDeleted(actor) && UserQuery.TryComp(actor, out UserInterfaceUserComponent component) && component.OpenInterfaces.TryGetValue(ent.Owner, out List<Enum> value2))
		{
			value2.Remove(key);
			if (value2.Count == 0)
			{
				component.OpenInterfaces.Remove(ent.Owner);
				if (component.OpenInterfaces.Count == 0)
				{
					RemCompDeferred<UserInterfaceUserComponent>(actor);
				}
			}
		}
		if (ent.Comp.ClientOpenInterfaces.TryGetValue(key, out BoundUserInterface value3))
		{
			AddQueued(value3, value: false);
		}
		if (ent.Comp.Actors.Count == 0)
		{
			RemCompDeferred<ActiveUserInterfaceComponent>(ent.Owner);
		}
		BoundUIClosedEvent args = new BoundUIClosedEvent(key, ent.Owner, actor);
		RaiseLocalEvent(ent.Owner, args);
	}

	private void OnUserInterfaceOpen(Entity<UserInterfaceComponent> ent, ref OpenBoundInterfaceMessage args)
	{
		OpenUiInternal(ent, args.UiKey, args.Actor);
	}

	private void OpenUiInternal(Entity<UserInterfaceComponent?> ent, Enum key, EntityUid actor)
	{
		if (UIQuery.Resolve(ent.Owner, ref ent.Comp, logMissing: false))
		{
			EnsureComp<ActiveUserInterfaceComponent>(ent.Owner);
			EnsureComp<UserInterfaceUserComponent>(actor).OpenInterfaces.GetOrNew(ent.Owner).Add(key);
			ent.Comp.Actors.GetOrNew(key).Add(actor);
			DirtyField(ent, "Actors");
			BoundUIOpenedEvent args = new BoundUIOpenedEvent(key, ent.Owner, actor);
			RaiseLocalEvent(ent.Owner, args);
			EnsureClientBui(ent, key, ent.Comp.Interfaces[key]);
		}
	}

	private void OnUserInterfaceStartup(Entity<UserInterfaceComponent> ent, ref ComponentStartup args)
	{
		foreach (var (_, bui) in ent.Comp.ClientOpenInterfaces)
		{
			AddQueued(bui, value: true);
		}
	}

	protected void OnUserInterfaceShutdown(Entity<UserInterfaceComponent> ent, ref ComponentShutdown args)
	{
		ValueList<EntityUid> valueList = default(ValueList<EntityUid>);
		foreach (var (key, hashSet2) in ent.Comp.Actors)
		{
			valueList.Clear();
			valueList.AddRange(hashSet2);
			foreach (EntityUid item in valueList)
			{
				CloseUiInternal(ent, key, item);
			}
		}
	}

	private void OnUserInterfaceGetState(Entity<UserInterfaceComponent> ent, ref ComponentGetState args)
	{
		if (args.FromTick > ent.Comp.CreationTick && ent.Comp.LastFieldUpdate >= args.FromTick)
		{
			switch (EntityManager.GetModifiedFields(ent.Comp, args.FromTick))
			{
			case 1u:
			{
				UserInterfaceActorsDeltaState userInterfaceActorsDeltaState = new UserInterfaceActorsDeltaState();
				AddActors(ent, userInterfaceActorsDeltaState.Actors, ref args);
				args.State = userInterfaceActorsDeltaState;
				return;
			}
			case 4u:
			{
				Dictionary<Enum, BoundUserInterfaceState> dictionary = ent.Comp.States;
				if (_netManager.IsClient)
				{
					dictionary = new Dictionary<Enum, BoundUserInterfaceState>(dictionary);
				}
				args.State = new UserInterfaceStatesDeltaState
				{
					States = dictionary
				};
				return;
			}
			}
		}
		Dictionary<Enum, List<NetEntity>> actors = new Dictionary<Enum, List<NetEntity>>();
		Dictionary<Enum, InterfaceData> dictionary2 = new Dictionary<Enum, InterfaceData>(ent.Comp.Interfaces.Count);
		foreach (KeyValuePair<Enum, InterfaceData> @interface in ent.Comp.Interfaces)
		{
			@interface.Deconstruct(out var key, out var value);
			Enum key2 = key;
			InterfaceData data = value;
			dictionary2[key2] = new InterfaceData(data);
		}
		args.State = new UserInterfaceComponentState(actors, new Dictionary<Enum, BoundUserInterfaceState>(ent.Comp.States), dictionary2);
		AddActors(ent, actors, ref args);
	}

	private void AddActors(Entity<UserInterfaceComponent> ent, Dictionary<Enum, List<NetEntity>> actors, ref ComponentGetState args)
	{
		Enum key;
		HashSet<EntityUid> value;
		if (args.ReplayState)
		{
			foreach (KeyValuePair<Enum, HashSet<EntityUid>> actor in ent.Comp.Actors)
			{
				actor.Deconstruct(out key, out value);
				Enum key2 = key;
				HashSet<EntityUid> uids = value;
				actors[key2] = GetNetEntityList(uids);
			}
			return;
		}
		EntityUid? attachedEntity = args.Player.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
		List<NetEntity> value2 = new List<NetEntity> { GetNetEntity(valueOrDefault) };
		foreach (KeyValuePair<Enum, HashSet<EntityUid>> actor2 in ent.Comp.Actors)
		{
			actor2.Deconstruct(out key, out value);
			Enum key3 = key;
			if (value.Contains(valueOrDefault))
			{
				actors[key3] = value2;
			}
		}
	}

	private void OnUserInterfaceHandleState(Entity<UserInterfaceComponent> ent, ref ComponentHandleState args)
	{
		Dictionary<Enum, List<NetEntity>> dictionary = null;
		Dictionary<Enum, InterfaceData> dictionary2 = null;
		Dictionary<Enum, BoundUserInterfaceState> dictionary3 = null;
		if (args.Current is UserInterfaceComponentState userInterfaceComponentState)
		{
			dictionary = userInterfaceComponentState.Actors;
			dictionary2 = userInterfaceComponentState.Data;
			dictionary3 = userInterfaceComponentState.States;
		}
		else if (args.Current is UserInterfaceActorsDeltaState userInterfaceActorsDeltaState)
		{
			dictionary = userInterfaceActorsDeltaState.Actors;
		}
		else
		{
			if (!(args.Current is UserInterfaceStatesDeltaState userInterfaceStatesDeltaState))
			{
				return;
			}
			dictionary3 = userInterfaceStatesDeltaState.States;
		}
		if (dictionary2 != null)
		{
			ent.Comp.Interfaces.Clear();
			foreach (KeyValuePair<Enum, InterfaceData> item2 in dictionary2)
			{
				ent.Comp.Interfaces[item2.Key] = new InterfaceData(item2.Value);
			}
		}
		EntityUid? localEntity = Player.LocalEntity;
		Enum key;
		if (dictionary != null)
		{
			foreach (Enum key5 in ent.Comp.Actors.Keys)
			{
				if (!dictionary.ContainsKey(key5))
				{
					CloseUi(ent, key5);
				}
			}
			ValueList<EntityUid> valueList = default(ValueList<EntityUid>);
			HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
			foreach (KeyValuePair<Enum, List<NetEntity>> item3 in dictionary)
			{
				item3.Deconstruct(out key, out var value);
				Enum key2 = key;
				List<NetEntity> list = value;
				HashSet<EntityUid> orNew = ent.Comp.Actors.GetOrNew(key2);
				hashSet.Clear();
				foreach (NetEntity item4 in list)
				{
					EntityUid item = EnsureEntity<UserInterfaceComponent>(item4, ent.Owner);
					if (item.IsValid())
					{
						hashSet.Add(item);
					}
				}
				foreach (EntityUid item5 in hashSet)
				{
					if (!orNew.Contains(item5))
					{
						OpenUiInternal(ent, key2, item5);
					}
				}
				foreach (EntityUid item6 in orNew)
				{
					if (!hashSet.Contains(item6))
					{
						valueList.Add(item6);
					}
				}
				foreach (EntityUid item7 in valueList)
				{
					CloseUiInternal(ent, key2, item7);
				}
			}
			foreach (Enum item8 in new ValueList<Enum>(ent.Comp.ClientOpenInterfaces.Keys))
			{
				if (!ent.Comp.Actors.TryGetValue(item8, out HashSet<EntityUid> value2) || (localEntity.HasValue && !value2.Contains(localEntity.Value)))
				{
					BoundUserInterface bui = ent.Comp.ClientOpenInterfaces[item8];
					AddQueued(bui, value: false);
				}
			}
		}
		if (dictionary3 != null)
		{
			foreach (Enum key6 in ent.Comp.States.Keys)
			{
				if (!dictionary3.ContainsKey(key6))
				{
					ent.Comp.States.Remove(key6);
				}
			}
			foreach (KeyValuePair<Enum, BoundUserInterfaceState> item9 in dictionary3)
			{
				item9.Deconstruct(out key, out var value3);
				Enum key3 = key;
				BoundUserInterfaceState boundUserInterfaceState = value3;
				if (!ent.Comp.States.TryGetValue(key3, out BoundUserInterfaceState value4) || !value4.Equals(boundUserInterfaceState))
				{
					ent.Comp.States[key3] = boundUserInterfaceState;
					if (ent.Comp.ClientOpenInterfaces.TryGetValue(key3, out BoundUserInterface value5) && value5.IsOpened)
					{
						value5.State = boundUserInterfaceState;
						value5.UpdateState(boundUserInterfaceState);
						value5.Update();
					}
				}
			}
		}
		bool open = (int)ent.Comp.LifeStage > 2;
		if (!localEntity.HasValue || dictionary == null)
		{
			return;
		}
		foreach (KeyValuePair<Enum, InterfaceData> @interface in ent.Comp.Interfaces)
		{
			@interface.Deconstruct(out key, out var value6);
			Enum key4 = key;
			InterfaceData data = value6;
			EnsureClientBui(ent, key4, data, open);
		}
	}

	private void EnsureClientBui(Entity<UserInterfaceComponent> entity, Enum key, InterfaceData data, bool open = true)
	{
		EntityUid? localEntity = Player.LocalEntity;
		HashSet<EntityUid> value2;
		if (entity.Comp.ClientOpenInterfaces.TryGetValue(key, out BoundUserInterface value))
		{
			_queuedBuis.Remove((value, false));
		}
		else if (localEntity.HasValue && entity.Comp.Actors.TryGetValue(key, out value2) && value2.Contains(localEntity.Value))
		{
			Type type = _reflection.LooseGetType(data.ClientType);
			BoundUserInterface boundUserInterface = (BoundUserInterface)_factory.CreateInstance(type, new object[2] { entity.Owner, key });
			entity.Comp.ClientOpenInterfaces[key] = boundUserInterface;
			if (open)
			{
				AddQueued(boundUserInterface, value: true);
			}
		}
	}

	public IEnumerable<(EntityUid Entity, Enum Key)> GetActorUis(Entity<UserInterfaceUserComponent?> entity)
	{
		if (!UserQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			yield break;
		}
		foreach (KeyValuePair<EntityUid, List<Enum>> berry in entity.Comp.OpenInterfaces)
		{
			foreach (Enum item in berry.Value)
			{
				yield return (Entity: berry.Key, Key: item);
			}
		}
	}

	public IEnumerable<EntityUid> GetActors(Entity<UserInterfaceComponent?> entity, Enum key)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) || !entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value))
		{
			yield break;
		}
		foreach (EntityUid item in value)
		{
			yield return item;
		}
	}

	public void CloseUi(Entity<UserInterfaceComponent?> entity, Enum key)
	{
		if (UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) && entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value))
		{
			EntityUid[] array = value.ToArray();
			foreach (EntityUid actor in array)
			{
				CloseUiInternal(entity, key, actor);
			}
		}
	}

	public void CloseUi(Entity<UserInterfaceComponent?> entity, Enum key, ICommonSession? actor, bool predicted = false)
	{
		EntityUid? entityUid = actor?.AttachedEntity;
		if (entityUid.HasValue)
		{
			CloseUi(entity, key, entityUid.Value, predicted);
		}
	}

	public void CloseUi(Entity<UserInterfaceComponent?> entity, Enum key, EntityUid? actor, bool predicted = false)
	{
		if (actor.HasValue && UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) && entity.Comp.Interfaces.ContainsKey(key) && entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value) && value.Contains(actor.Value))
		{
			if (!predicted)
			{
				CloseUiInternal(entity, key, actor.Value);
			}
			else if (_timing.IsFirstTimePredicted)
			{
				RaisePredictiveEvent(new BoundUIWrapMessage(GetNetEntity(entity.Owner), new CloseBoundInterfaceMessage(), key));
			}
		}
	}

	public bool TryOpenUi(Entity<UserInterfaceComponent?> entity, Enum key, EntityUid actor, bool predicted = false)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return false;
		}
		OpenUi(entity, key, actor, predicted);
		if (!entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value) || !value.Contains(actor))
		{
			return false;
		}
		return true;
	}

	public virtual void OpenUi(Entity<UserInterfaceComponent?> entity, Enum key, bool predicted = false)
	{
	}

	public void OpenUi(Entity<UserInterfaceComponent?> entity, Enum key, EntityUid? actor, bool predicted = false)
	{
		if (!actor.HasValue || !UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) || !entity.Comp.Interfaces.ContainsKey(key) || (entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value) && value.Contains(actor.Value)))
		{
			return;
		}
		if (predicted)
		{
			if (_timing.IsFirstTimePredicted)
			{
				RaisePredictiveEvent(new BoundUIWrapMessage(GetNetEntity(entity.Owner), new OpenBoundInterfaceMessage(), key));
			}
		}
		else
		{
			OnMessageReceived(new BoundUIWrapMessage(GetNetEntity(entity.Owner), new OpenBoundInterfaceMessage(), key), actor.Value);
		}
	}

	public void OpenUi(Entity<UserInterfaceComponent?> entity, Enum key, ICommonSession actor, bool predicted = false)
	{
		EntityUid? attachedEntity = actor.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			OpenUi(entity, key, attachedEntity.Value, predicted);
		}
	}

	public virtual bool TryGetPosition(Entity<UserInterfaceComponent?> entity, Enum key, out Vector2 position)
	{
		position = Vector2.Zero;
		return false;
	}

	protected virtual void SavePosition(BoundUserInterface bui)
	{
	}

	public void SetUiState(Entity<UserInterfaceComponent?> entity, Enum key, BoundUserInterfaceState? state)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) || !entity.Comp.Interfaces.ContainsKey(key))
		{
			return;
		}
		if (state == null)
		{
			if (!entity.Comp.States.Remove(key))
			{
				return;
			}
			DirtyField(entity, "States");
		}
		else
		{
			bool exists;
			ref BoundUserInterfaceState valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(entity.Comp.States, key, out exists);
			if (exists)
			{
				BoundUserInterfaceState obj = valueRefOrAddDefault;
				if (obj != null && obj.Equals(state))
				{
					return;
				}
			}
			valueRefOrAddDefault = state;
		}
		if (state != null && _netManager.IsClient && entity.Comp.ClientOpenInterfaces.TryGetValue(key, out BoundUserInterface value))
		{
			BoundUserInterfaceState? state2 = value.State;
			if (state2 == null || !state2.Equals(state))
			{
				value.UpdateState(state);
				value.Update();
			}
		}
		DirtyField(entity, "States");
	}

	public bool HasUi(EntityUid uid, Enum uiKey, UserInterfaceComponent? ui = null)
	{
		if (!Resolve(uid, ref ui, logMissing: false))
		{
			return false;
		}
		return ui.Interfaces.ContainsKey(uiKey);
	}

	public bool IsUiOpen(Entity<UserInterfaceComponent?> entity, Enum uiKey)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return false;
		}
		if (!entity.Comp.Actors.TryGetValue(uiKey, out HashSet<EntityUid> value))
		{
			return false;
		}
		return value.Count > 0;
	}

	public bool IsUiOpen(Entity<UserInterfaceComponent?> entity, Enum uiKey, EntityUid actor)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return false;
		}
		if (!entity.Comp.Actors.TryGetValue(uiKey, out HashSet<EntityUid> value))
		{
			return false;
		}
		return value.Contains(actor);
	}

	public bool IsUiOpen(Entity<UserInterfaceComponent?> entity, IEnumerable<Enum> uiKeys)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return false;
		}
		foreach (Enum uiKey in uiKeys)
		{
			if (entity.Comp.Actors.ContainsKey(uiKey))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAnyUiOpen(Entity<UserInterfaceComponent?> entity)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return false;
		}
		return entity.Comp.Actors.Count > 0;
	}

	public void RaiseUiMessage(Entity<UserInterfaceComponent?> entity, Enum key, BoundUserInterfaceMessage message)
	{
		if (UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) && entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> _))
		{
			OnMessageReceived(new BoundUIWrapMessage(GetNetEntity(entity.Owner), message, key), message.Actor);
		}
	}

	public void ServerSendUiMessage(Entity<UserInterfaceComponent?> entity, Enum key, BoundUserInterfaceMessage message)
	{
		if (UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) && entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value))
		{
			Filter filter = Filter.Entities(value.ToArray());
			RaiseNetworkEvent(new BoundUIWrapMessage(GetNetEntity(entity.Owner), message, key), filter);
		}
	}

	public void ServerSendUiMessage(Entity<UserInterfaceComponent?> entity, Enum key, BoundUserInterfaceMessage message, EntityUid actor)
	{
		if (UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) && entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value) && value.Contains(actor))
		{
			RaiseNetworkEvent(new BoundUIWrapMessage(GetNetEntity(entity.Owner), message, key), actor);
		}
	}

	public void ServerSendUiMessage(Entity<UserInterfaceComponent?> entity, Enum key, BoundUserInterfaceMessage message, ICommonSession actor)
	{
		if (!_netManager.IsClient || !UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return;
		}
		EntityUid? attachedEntity = actor.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
			if (entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value) && value.Contains(valueOrDefault))
			{
				RaiseNetworkEvent(new BoundUIWrapMessage(GetNetEntity(entity.Owner), message, key), actor);
			}
		}
	}

	public void ClientSendUiMessage(Entity<UserInterfaceComponent?> entity, Enum key, BoundUserInterfaceMessage message)
	{
		EntityUid? localEntity = Player.LocalEntity;
		if (localEntity.HasValue && UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) && entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> value) && value.Contains(localEntity.Value))
		{
			RaiseNetworkEvent(new BoundUIWrapMessage(GetNetEntity(entity.Owner), message, key));
		}
	}

	public void CloseUserUis<T>(Entity<UserInterfaceUserComponent?> actor) where T : Enum
	{
		if (!UserQuery.Resolve(actor.Owner, ref actor.Comp, logMissing: false) || actor.Comp.OpenInterfaces.Count == 0)
		{
			return;
		}
		ValueList<Enum> valueList = default(ValueList<Enum>);
		foreach (var (entityUid2, list2) in actor.Comp.OpenInterfaces)
		{
			valueList.Clear();
			valueList.AddRange(list2);
			foreach (Enum item in valueList)
			{
				if (item is T)
				{
					CloseUiInternal(entityUid2, item, actor.Owner);
				}
			}
		}
	}

	public void CloseUserUis(Entity<UserInterfaceUserComponent?> actor)
	{
		if (!UserQuery.Resolve(actor.Owner, ref actor.Comp, logMissing: false) || actor.Comp.OpenInterfaces.Count == 0)
		{
			return;
		}
		ValueList<Enum> valueList = default(ValueList<Enum>);
		foreach (var (entityUid2, list2) in actor.Comp.OpenInterfaces)
		{
			valueList.Clear();
			valueList.AddRange(list2);
			foreach (Enum item in valueList)
			{
				CloseUiInternal(entityUid2, item, actor.Owner);
			}
		}
	}

	public void CloseUis(Entity<UserInterfaceComponent?> entity)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return;
		}
		ValueList<EntityUid> valueList = default(ValueList<EntityUid>);
		foreach (var (key, hashSet2) in entity.Comp.Actors)
		{
			valueList.Clear();
			valueList.AddRange(hashSet2);
			foreach (EntityUid item in valueList)
			{
				CloseUiInternal(entity, key, item);
			}
		}
	}

	public void CloseUis(Entity<UserInterfaceComponent?> entity, EntityUid actor)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return;
		}
		foreach (Enum key in entity.Comp.Interfaces.Keys)
		{
			CloseUiInternal(entity, key, actor);
		}
	}

	public void CloseUis(Entity<UserInterfaceComponent?> entity, ICommonSession actor)
	{
		EntityUid? attachedEntity = actor.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
			if (UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
			{
				CloseUis(entity, valueOrDefault);
			}
		}
	}

	public bool TryGetOpenUi(Entity<UserInterfaceComponent?> entity, Enum uiKey, [NotNullWhen(true)] out BoundUserInterface? bui)
	{
		bui = null;
		if (UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return entity.Comp.ClientOpenInterfaces.TryGetValue(uiKey, out bui);
		}
		return false;
	}

	public bool TryGetOpenUi<T>(Entity<UserInterfaceComponent?> entity, Enum uiKey, [NotNullWhen(true)] out T? bui) where T : BoundUserInterface
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) || !entity.Comp.ClientOpenInterfaces.TryGetValue(uiKey, out BoundUserInterface value))
		{
			bui = null;
			return false;
		}
		bui = (T)value;
		return true;
	}

	public bool TryToggleUi(Entity<UserInterfaceComponent?> entity, Enum uiKey, ICommonSession actor)
	{
		EntityUid? attachedEntity = actor.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
			return TryToggleUi(entity, uiKey, valueOrDefault);
		}
		return false;
	}

	public bool TryToggleUi(Entity<UserInterfaceComponent?> entity, Enum uiKey, EntityUid actor)
	{
		if (!UIQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false) || !entity.Comp.Interfaces.ContainsKey(uiKey))
		{
			return false;
		}
		if (entity.Comp.Actors.TryGetValue(uiKey, out HashSet<EntityUid> value) && value.Contains(actor))
		{
			CloseUi(entity, uiKey, actor);
		}
		else
		{
			OpenUi(entity, uiKey, actor);
		}
		return true;
	}

	public void SendPredictedUiMessage(BoundUserInterface bui, BoundUserInterfaceMessage msg)
	{
		RaisePredictiveEvent(new BoundUIWrapMessage(GetNetEntity(bui.Owner), msg, bui.UiKey));
	}

	public bool TryGetInterfaceData(Entity<UserInterfaceComponent?> entity, Enum key, [NotNullWhen(true)] out InterfaceData? data)
	{
		data = null;
		if (Resolve(entity, ref entity.Comp, logMissing: false))
		{
			return entity.Comp.Interfaces.TryGetValue(key, out data);
		}
		return false;
	}

	public float GetUiRange(Entity<UserInterfaceComponent?> entity, Enum key)
	{
		TryGetInterfaceData(entity, key, out InterfaceData data);
		return data?.InteractionRange ?? 0f;
	}

	public override void Update(float frameTime)
	{
		if (_timing.IsFirstTimePredicted)
		{
			foreach (var queuedBui in _queuedBuis)
			{
				var (boundUserInterface, _) = queuedBui;
				if (queuedBui.value)
				{
					try
					{
						boundUserInterface.Open();
						if (UIQuery.TryComp(boundUserInterface.Owner, out UserInterfaceComponent component) && component.States.TryGetValue(boundUserInterface.UiKey, out BoundUserInterfaceState value))
						{
							boundUserInterface.State = value;
							boundUserInterface.UpdateState(value);
							boundUserInterface.Update();
						}
					}
					catch (Exception value2)
					{
						base.Log.Error($"Caught exception while attempting to create a BUI {boundUserInterface.UiKey} with type {boundUserInterface.GetType()} on entity {ToPrettyString(boundUserInterface.Owner)}. Exception: {value2}");
					}
					continue;
				}
				if (UIQuery.TryComp(boundUserInterface.Owner, out UserInterfaceComponent component2))
				{
					component2.ClientOpenInterfaces.Remove(boundUserInterface.UiKey);
				}
				try
				{
					if (!TerminatingOrDeleted(boundUserInterface.Owner))
					{
						SavePosition(boundUserInterface);
					}
					boundUserInterface.Dispose();
				}
				catch (Exception value3)
				{
					base.Log.Error($"Caught exception while attempting to dispose of a BUI {boundUserInterface.UiKey} with type {boundUserInterface.GetType()} on entity {ToPrettyString(boundUserInterface.Owner)}. Exception: {value3}");
				}
			}
			_queuedBuis.Clear();
		}
		AllEntityQueryEnumerator<ActiveUserInterfaceComponent, UserInterfaceComponent> allEntityQueryEnumerator = AllEntityQuery<ActiveUserInterfaceComponent, UserInterfaceComponent>();
		_rangeJob.ActorRanges.Clear();
		EntityUid uid;
		ActiveUserInterfaceComponent comp;
		UserInterfaceComponent comp2;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp, out comp2))
		{
			foreach (KeyValuePair<Enum, HashSet<EntityUid>> actor in comp2.Actors)
			{
				actor.Deconstruct(out var key, out var value4);
				Enum obj = key;
				HashSet<EntityUid> hashSet = value4;
				InterfaceData interfaceData = comp2.Interfaces[obj];
				if (interfaceData.InteractionRange <= 0f)
				{
					continue;
				}
				foreach (EntityUid item4 in hashSet)
				{
					if (!_netManager.IsClient || item4.IsValid())
					{
						_rangeJob.ActorRanges.Add((uid, obj, interfaceData, item4, false));
					}
				}
			}
		}
		_parallel.ProcessNow(_rangeJob, _rangeJob.ActorRanges.Count);
		foreach (var actorRange in _rangeJob.ActorRanges)
		{
			EntityUid item = actorRange.Ui;
			EntityUid item2 = actorRange.Actor;
			Enum item3 = actorRange.Key;
			if (!actorRange.Result && !Deleted(item) && !Deleted(item2) && UIQuery.TryComp(item, out UserInterfaceComponent component3))
			{
				CloseUi((Owner: item, Comp: component3), item3, item2);
			}
		}
	}

	public void SetUi(Entity<UserInterfaceComponent?> ent, Enum key, InterfaceData data)
	{
		if (!Resolve(ent, ref ent.Comp, logMissing: false))
		{
			ent.Comp = AddComp<UserInterfaceComponent>(ent);
		}
		ent.Comp.Interfaces[key] = data;
		DirtyField(ent, "Interfaces");
	}

	public bool TryGetUiState<T>(Entity<UserInterfaceComponent?> ent, Enum key, [NotNullWhen(true)] out T? state) where T : BoundUserInterfaceState
	{
		if (!Resolve(ent, ref ent.Comp, logMissing: false) || !ent.Comp.States.TryGetValue(key, out BoundUserInterfaceState value))
		{
			state = null;
			return false;
		}
		state = (T)value;
		return true;
	}

	private bool CheckRange(Entity<TransformComponent> UiEnt, Enum key, InterfaceData data, Entity<TransformComponent> actor)
	{
		if (actor.Comp.MapID != UiEnt.Comp.MapID)
		{
			return false;
		}
		BoundUserInterfaceCheckRangeEvent args = new BoundUserInterfaceCheckRangeEvent(UiEnt, key, data, actor);
		RaiseLocalEvent(UiEnt.Owner, ref args, broadcast: true);
		if (args.Result == BoundUserInterfaceRangeResult.Pass)
		{
			return true;
		}
		if (_ignoreUIRangeQuery.HasComponent(actor))
		{
			return true;
		}
		if (args.Result == BoundUserInterfaceRangeResult.Fail)
		{
			return false;
		}
		return _transforms.InRange(UiEnt, (Owner: actor.Owner, Comp: actor.Comp), data.InteractionRange);
	}
}
