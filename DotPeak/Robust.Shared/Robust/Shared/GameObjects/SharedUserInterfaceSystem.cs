// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedUserInterfaceSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedUserInterfaceSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private readonly IDynamicTypeFactory _factory;
  [Robust.Shared.IoC.Dependency]
  private readonly IGameTiming _timing;
  [Robust.Shared.IoC.Dependency]
  private readonly INetManager _netManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IParallelManager _parallel;
  [Robust.Shared.IoC.Dependency]
  protected readonly IPrototypeManager ProtoManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IReflectionManager _reflection;
  [Robust.Shared.IoC.Dependency]
  protected readonly ISharedPlayerManager Player;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedTransformSystem _transforms;
  private Robust.Shared.GameObjects.EntityQuery<IgnoreUIRangeComponent> _ignoreUIRangeQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;
  protected Robust.Shared.GameObjects.EntityQuery<UserInterfaceComponent> UIQuery;
  protected Robust.Shared.GameObjects.EntityQuery<UserInterfaceUserComponent> UserQuery;
  private SharedUserInterfaceSystem.ActorRangeCheckJob _rangeJob;
  private readonly List<(BoundUserInterface Bui, bool value)> _queuedBuis = new List<(BoundUserInterface, bool)>();

  public override void Initialize()
  {
    base.Initialize();
    this.EntityManager.ComponentFactory.RegisterNetworkedFields<UserInterfaceComponent>("Actors", "Interfaces", "States");
    this._ignoreUIRangeQuery = this.GetEntityQuery<IgnoreUIRangeComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
    this.UIQuery = this.GetEntityQuery<UserInterfaceComponent>();
    this.UserQuery = this.GetEntityQuery<UserInterfaceUserComponent>();
    this._rangeJob = new SharedUserInterfaceSystem.ActorRangeCheckJob()
    {
      System = this,
      XformQuery = this._xformQuery
    };
    this.SubscribeAllEvent<BoundUIWrapMessage>((EntitySessionEventHandler<BoundUIWrapMessage>) ((msg, args) =>
    {
      EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
      this.OnMessageReceived(msg, valueOrDefault);
    }));
    this.SubscribeLocalEvent<UserInterfaceComponent, OpenBoundInterfaceMessage>(new EntityEventRefHandler<UserInterfaceComponent, OpenBoundInterfaceMessage>(this.OnUserInterfaceOpen));
    this.SubscribeLocalEvent<UserInterfaceComponent, CloseBoundInterfaceMessage>(new EntityEventRefHandler<UserInterfaceComponent, CloseBoundInterfaceMessage>(this.OnUserInterfaceClosed));
    this.SubscribeLocalEvent<UserInterfaceComponent, ComponentStartup>(new EntityEventRefHandler<UserInterfaceComponent, ComponentStartup>(this.OnUserInterfaceStartup));
    this.SubscribeLocalEvent<UserInterfaceComponent, ComponentShutdown>(new EntityEventRefHandler<UserInterfaceComponent, ComponentShutdown>(this.OnUserInterfaceShutdown));
    this.SubscribeLocalEvent<UserInterfaceComponent, ComponentGetState>(new EntityEventRefHandler<UserInterfaceComponent, ComponentGetState>(this.OnUserInterfaceGetState));
    this.SubscribeLocalEvent<UserInterfaceComponent, ComponentHandleState>(new EntityEventRefHandler<UserInterfaceComponent, ComponentHandleState>(this.OnUserInterfaceHandleState));
    this.SubscribeLocalEvent<PlayerAttachedEvent>(new EntityEventHandler<PlayerAttachedEvent>(this.OnPlayerAttached));
    this.SubscribeLocalEvent<PlayerDetachedEvent>(new EntityEventHandler<PlayerDetachedEvent>(this.OnPlayerDetached));
    this.SubscribeLocalEvent<UserInterfaceUserComponent, ComponentShutdown>(new EntityEventRefHandler<UserInterfaceUserComponent, ComponentShutdown>(this.OnActorShutdown));
  }

  private void AddQueued(BoundUserInterface bui, bool value) => this._queuedBuis.Add((bui, value));

  private void OnMessageReceived(BoundUIWrapMessage msg, EntityUid sender)
  {
    EntityUid entity = this.GetEntity(msg.Entity);
    UserInterfaceComponent component;
    if (!this.UIQuery.TryComp(entity, out component))
      return;
    InterfaceData interfaceData;
    if (!component.Interfaces.TryGetValue(msg.UiKey, out interfaceData))
    {
      this.Log.Debug($"Got BoundInterfaceMessageWrapMessage for unknown UI key: {msg.UiKey}");
    }
    else
    {
      HashSet<EntityUid> entityUidSet;
      if (!(msg.Message is OpenBoundInterfaceMessage) && (!component.Actors.TryGetValue(msg.UiKey, out entityUidSet) || !entityUidSet.Contains(sender)))
      {
        this.Log.Debug($"UI {msg.UiKey} got BoundInterfaceMessageWrapMessage from a client who was not subscribed: {this.ToPrettyString((Entity<MetaDataComponent>) sender)}");
      }
      else
      {
        if (!(msg.Message is CloseBoundInterfaceMessage) && interfaceData.RequireInputValidation)
        {
          BoundUserInterfaceMessageAttempt interfaceMessageAttempt = new BoundUserInterfaceMessageAttempt(sender, entity, msg.UiKey, msg.Message);
          this.RaiseLocalEvent<BoundUserInterfaceMessageAttempt>(interfaceMessageAttempt);
          if (interfaceMessageAttempt.Cancelled)
            return;
          this.RaiseLocalEvent<BoundUserInterfaceMessageAttempt>(entity, interfaceMessageAttempt);
          if (interfaceMessageAttempt.Cancelled)
            return;
        }
        BoundUserInterfaceMessage message = msg.Message;
        message.Actor = sender;
        message.Entity = msg.Entity;
        message.UiKey = msg.UiKey;
        BoundUserInterface boundUserInterface;
        if (component.ClientOpenInterfaces.TryGetValue(msg.UiKey, out boundUserInterface))
          boundUserInterface.ReceiveMessage(message);
        this.RaiseLocalEvent(entity, (object) message, true);
      }
    }
  }

  private void OnActorShutdown(Entity<UserInterfaceUserComponent> ent, ref ComponentShutdown args)
  {
    this.CloseUserUis((Entity<UserInterfaceUserComponent>) (ent.Owner, ent.Comp));
  }

  private void OnPlayerAttached(PlayerAttachedEvent ev)
  {
    UserInterfaceUserComponent component1;
    if (!this.UserQuery.TryGetComponent(ev.Entity, out component1))
      return;
    foreach ((EntityUid entityUid, List<Enum> enumList) in component1.OpenInterfaces)
    {
      UserInterfaceComponent component2;
      if (this.UIQuery.TryGetComponent(entityUid, out component2))
      {
        this.Dirty(entityUid, (IComponent) component2);
        foreach (Enum key in enumList)
        {
          InterfaceData data;
          if (component2.Interfaces.TryGetValue(key, out data))
            this.EnsureClientBui((Entity<UserInterfaceComponent>) (entityUid, component2), key, data);
        }
      }
    }
  }

  private void OnPlayerDetached(PlayerDetachedEvent ev)
  {
    UserInterfaceUserComponent component1;
    if (!this.UserQuery.TryGetComponent(ev.Entity, out component1))
      return;
    foreach ((EntityUid entityUid, List<Enum> enumList) in component1.OpenInterfaces)
    {
      UserInterfaceComponent component2;
      if (this.UIQuery.TryGetComponent(entityUid, out component2))
      {
        this.Dirty(entityUid, (IComponent) component2);
        foreach (Enum key in enumList)
        {
          BoundUserInterface boundUserInterface;
          if (component2.ClientOpenInterfaces.Remove(key, out boundUserInterface))
            boundUserInterface.Dispose();
        }
      }
    }
  }

  private void OnUserInterfaceClosed(
    Entity<UserInterfaceComponent> ent,
    ref CloseBoundInterfaceMessage args)
  {
    this.CloseUiInternal(ent, args.UiKey, args.Actor);
  }

  private void CloseUiInternal(Entity<UserInterfaceComponent?> ent, Enum key, EntityUid actor)
  {
    HashSet<EntityUid> entityUidSet;
    if (!this.UIQuery.Resolve(ent.Owner, ref ent.Comp, false) || !ent.Comp.Actors.TryGetValue(key, out entityUidSet))
      return;
    entityUidSet.Remove(actor);
    if (entityUidSet.Count == 0)
      ent.Comp.Actors.Remove(key);
    this.DirtyField<UserInterfaceComponent>(ent, "Actors");
    UserInterfaceUserComponent component;
    List<Enum> enumList;
    if (!this.TerminatingOrDeleted(actor) && this.UserQuery.TryComp(actor, out component) && component.OpenInterfaces.TryGetValue(ent.Owner, out enumList))
    {
      enumList.Remove(key);
      if (enumList.Count == 0)
      {
        component.OpenInterfaces.Remove(ent.Owner);
        if (component.OpenInterfaces.Count == 0)
          this.RemCompDeferred<UserInterfaceUserComponent>(actor);
      }
    }
    BoundUserInterface bui;
    if (ent.Comp.ClientOpenInterfaces.TryGetValue(key, out bui))
      this.AddQueued(bui, false);
    if (ent.Comp.Actors.Count == 0)
      this.RemCompDeferred<ActiveUserInterfaceComponent>(ent.Owner);
    BoundUIClosedEvent args = new BoundUIClosedEvent(key, ent.Owner, actor);
    this.RaiseLocalEvent<BoundUIClosedEvent>(ent.Owner, args);
  }

  private void OnUserInterfaceOpen(
    Entity<UserInterfaceComponent> ent,
    ref OpenBoundInterfaceMessage args)
  {
    this.OpenUiInternal(ent, args.UiKey, args.Actor);
  }

  private void OpenUiInternal(Entity<UserInterfaceComponent?> ent, Enum key, EntityUid actor)
  {
    if (!this.UIQuery.Resolve(ent.Owner, ref ent.Comp, false))
      return;
    this.EnsureComp<ActiveUserInterfaceComponent>(ent.Owner);
    this.EnsureComp<UserInterfaceUserComponent>(actor).OpenInterfaces.GetOrNew<EntityUid, List<Enum>>(ent.Owner).Add(key);
    ent.Comp.Actors.GetOrNew<Enum, HashSet<EntityUid>>(key).Add(actor);
    this.DirtyField<UserInterfaceComponent>(ent, "Actors");
    BoundUIOpenedEvent args = new BoundUIOpenedEvent(key, ent.Owner, actor);
    this.RaiseLocalEvent<BoundUIOpenedEvent>(ent.Owner, args);
    this.EnsureClientBui(ent, key, ent.Comp.Interfaces[key]);
  }

  private void OnUserInterfaceStartup(Entity<UserInterfaceComponent> ent, ref ComponentStartup args)
  {
    foreach ((Enum _, BoundUserInterface bui) in ent.Comp.ClientOpenInterfaces)
      this.AddQueued(bui, true);
  }

  protected void OnUserInterfaceShutdown(
    Entity<UserInterfaceComponent> ent,
    ref ComponentShutdown args)
  {
    ValueList<EntityUid> valueList = new ValueList<EntityUid>();
    foreach ((Enum key, HashSet<EntityUid> select) in ent.Comp.Actors)
    {
      valueList.Clear();
      valueList.AddRange((IEnumerable<EntityUid>) select);
      foreach (EntityUid actor in valueList)
        this.CloseUiInternal(ent, key, actor);
    }
  }

  private void OnUserInterfaceGetState(
    Entity<UserInterfaceComponent> ent,
    ref ComponentGetState args)
  {
    if (args.FromTick > ent.Comp.CreationTick && ent.Comp.LastFieldUpdate >= args.FromTick)
    {
      switch (this.EntityManager.GetModifiedFields((IComponentDelta) ent.Comp, args.FromTick))
      {
        case 1:
          UserInterfaceActorsDeltaState actorsDeltaState = new UserInterfaceActorsDeltaState();
          this.AddActors(ent, actorsDeltaState.Actors, ref args);
          args.State = (IComponentState) actorsDeltaState;
          return;
        case 4:
          Dictionary<Enum, BoundUserInterfaceState> dictionary = ent.Comp.States;
          if (this._netManager.IsClient)
            dictionary = new Dictionary<Enum, BoundUserInterfaceState>((IDictionary<Enum, BoundUserInterfaceState>) dictionary);
          args.State = (IComponentState) new UserInterfaceStatesDeltaState()
          {
            States = dictionary
          };
          return;
      }
    }
    Dictionary<Enum, List<NetEntity>> actors = new Dictionary<Enum, List<NetEntity>>();
    Dictionary<Enum, InterfaceData> data1 = new Dictionary<Enum, InterfaceData>(ent.Comp.Interfaces.Count);
    foreach ((Enum key, InterfaceData data2) in ent.Comp.Interfaces)
      data1[key] = new InterfaceData(data2);
    args.State = (IComponentState) new UserInterfaceComponentState(actors, new Dictionary<Enum, BoundUserInterfaceState>((IDictionary<Enum, BoundUserInterfaceState>) ent.Comp.States), data1);
    this.AddActors(ent, actors, ref args);
  }

  private void AddActors(
    Entity<UserInterfaceComponent> ent,
    Dictionary<Enum, List<NetEntity>> actors,
    ref ComponentGetState args)
  {
    if (args.ReplayState)
    {
      foreach ((Enum key, HashSet<EntityUid> uids) in ent.Comp.Actors)
        actors[key] = this.GetNetEntityList((ICollection<EntityUid>) uids);
    }
    else
    {
      EntityUid? attachedEntity = args.Player.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
      List<NetEntity> netEntityList = new List<NetEntity>()
      {
        this.GetNetEntity(valueOrDefault)
      };
      foreach ((Enum key, HashSet<EntityUid> entityUidSet) in ent.Comp.Actors)
      {
        if (entityUidSet.Contains(valueOrDefault))
          actors[key] = netEntityList;
      }
    }
  }

  private void OnUserInterfaceHandleState(
    Entity<UserInterfaceComponent> ent,
    ref ComponentHandleState args)
  {
    Dictionary<Enum, List<NetEntity>> dictionary1 = (Dictionary<Enum, List<NetEntity>>) null;
    Dictionary<Enum, InterfaceData> dictionary2 = (Dictionary<Enum, InterfaceData>) null;
    Dictionary<Enum, BoundUserInterfaceState> dictionary3 = (Dictionary<Enum, BoundUserInterfaceState>) null;
    if (args.Current is UserInterfaceComponentState current2)
    {
      dictionary1 = current2.Actors;
      dictionary2 = current2.Data;
      dictionary3 = current2.States;
    }
    else if (args.Current is UserInterfaceActorsDeltaState current1)
    {
      dictionary1 = current1.Actors;
    }
    else
    {
      if (!(args.Current is UserInterfaceStatesDeltaState current))
        return;
      dictionary3 = current.States;
    }
    if (dictionary2 != null)
    {
      ent.Comp.Interfaces.Clear();
      foreach (KeyValuePair<Enum, InterfaceData> keyValuePair in dictionary2)
        ent.Comp.Interfaces[keyValuePair.Key] = new InterfaceData(keyValuePair.Value);
    }
    EntityUid? localEntity = this.Player.LocalEntity;
    Enum key9;
    if (dictionary1 != null)
    {
      foreach (Enum key2 in ent.Comp.Actors.Keys)
      {
        if (!dictionary1.ContainsKey(key2))
          this.CloseUi(ent, key2);
      }
      ValueList<EntityUid> valueList = new ValueList<EntityUid>();
      HashSet<EntityUid> entityUidSet1 = new HashSet<EntityUid>();
      List<NetEntity> netEntityList2;
      foreach ((key9, netEntityList2) in dictionary1)
      {
        Enum key4 = key9;
        List<NetEntity> netEntityList3 = netEntityList2;
        HashSet<EntityUid> orNew = ent.Comp.Actors.GetOrNew<Enum, HashSet<EntityUid>>(key4);
        entityUidSet1.Clear();
        foreach (NetEntity netEntity in netEntityList3)
        {
          EntityUid entityUid = this.EnsureEntity<UserInterfaceComponent>(netEntity, ent.Owner);
          if (entityUid.IsValid())
            entityUidSet1.Add(entityUid);
        }
        foreach (EntityUid actor in entityUidSet1)
        {
          if (!orNew.Contains(actor))
            this.OpenUiInternal(ent, key4, actor);
        }
        foreach (EntityUid entityUid in orNew)
        {
          if (!entityUidSet1.Contains(entityUid))
            valueList.Add(entityUid);
        }
        foreach (EntityUid actor in valueList)
          this.CloseUiInternal(ent, key4, actor);
      }
      foreach (Enum key5 in new ValueList<Enum>((IReadOnlyCollection<Enum>) ent.Comp.ClientOpenInterfaces.Keys))
      {
        HashSet<EntityUid> entityUidSet2;
        if (!ent.Comp.Actors.TryGetValue(key5, out entityUidSet2) || localEntity.HasValue && !entityUidSet2.Contains(localEntity.Value))
          this.AddQueued(ent.Comp.ClientOpenInterfaces[key5], false);
      }
    }
    if (dictionary3 != null)
    {
      foreach (Enum key6 in ent.Comp.States.Keys)
      {
        if (!dictionary3.ContainsKey(key6))
          ent.Comp.States.Remove(key6);
      }
      BoundUserInterfaceState userInterfaceState2;
      foreach ((key9, userInterfaceState2) in dictionary3)
      {
        Enum key8 = key9;
        BoundUserInterfaceState state = userInterfaceState2;
        BoundUserInterfaceState userInterfaceState3;
        if (!ent.Comp.States.TryGetValue(key8, out userInterfaceState3) || !userInterfaceState3.Equals((object) state))
        {
          ent.Comp.States[key8] = state;
          BoundUserInterface boundUserInterface;
          if (ent.Comp.ClientOpenInterfaces.TryGetValue(key8, out boundUserInterface) && boundUserInterface.IsOpened)
          {
            boundUserInterface.State = state;
            boundUserInterface.UpdateState(state);
            boundUserInterface.Update();
          }
        }
      }
    }
    bool open = ent.Comp.LifeStage > ComponentLifeStage.Added;
    if (!localEntity.HasValue || dictionary1 == null)
      return;
    InterfaceData interfaceData2;
    foreach ((key9, interfaceData2) in ent.Comp.Interfaces)
    {
      Enum key10 = key9;
      InterfaceData data = interfaceData2;
      this.EnsureClientBui(ent, key10, data, open);
    }
  }

  private void EnsureClientBui(
    Entity<UserInterfaceComponent> entity,
    Enum key,
    InterfaceData data,
    bool open = true)
  {
    EntityUid? localEntity = this.Player.LocalEntity;
    BoundUserInterface boundUserInterface;
    if (entity.Comp.ClientOpenInterfaces.TryGetValue(key, out boundUserInterface))
    {
      this._queuedBuis.Remove((boundUserInterface, false));
    }
    else
    {
      HashSet<EntityUid> entityUidSet;
      if (!localEntity.HasValue || !entity.Comp.Actors.TryGetValue(key, out entityUidSet) || !entityUidSet.Contains(localEntity.Value))
        return;
      BoundUserInterface instance = (BoundUserInterface) this._factory.CreateInstance(this._reflection.LooseGetType(data.ClientType), new object[2]
      {
        (object) entity.Owner,
        (object) key
      });
      entity.Comp.ClientOpenInterfaces[key] = instance;
      if (!open)
        return;
      this.AddQueued(instance, true);
    }
  }

  public IEnumerable<(EntityUid Entity, Enum Key)> GetActorUis(
    Entity<UserInterfaceUserComponent?> entity)
  {
    if (this.UserQuery.Resolve(entity.Owner, ref entity.Comp, false))
    {
      foreach (KeyValuePair<EntityUid, List<Enum>> openInterface in entity.Comp.OpenInterfaces)
      {
        KeyValuePair<EntityUid, List<Enum>> berry = openInterface;
        List<Enum>.Enumerator enumerator = berry.Value.GetEnumerator();
        while (enumerator.MoveNext())
          yield return (berry.Key, enumerator.Current);
        enumerator = new List<Enum>.Enumerator();
        berry = new KeyValuePair<EntityUid, List<Enum>>();
      }
    }
  }

  public IEnumerable<EntityUid> GetActors(Entity<UserInterfaceComponent?> entity, Enum key)
  {
    HashSet<EntityUid> entityUidSet;
    if (this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) && entity.Comp.Actors.TryGetValue(key, out entityUidSet))
    {
      foreach (EntityUid actor in entityUidSet)
        yield return actor;
    }
  }

  public void CloseUi(Entity<UserInterfaceComponent?> entity, Enum key)
  {
    HashSet<EntityUid> source;
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.Actors.TryGetValue(key, out source))
      return;
    foreach (EntityUid actor in source.ToArray<EntityUid>())
      this.CloseUiInternal(entity, key, actor);
  }

  public void CloseUi(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    ICommonSession? actor,
    bool predicted = false)
  {
    EntityUid? attachedEntity = (EntityUid?) actor?.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    this.CloseUi(entity, key, new EntityUid?(attachedEntity.Value), predicted);
  }

  public void CloseUi(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    EntityUid? actor,
    bool predicted = false)
  {
    HashSet<EntityUid> entityUidSet;
    if (!actor.HasValue || !this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.Interfaces.ContainsKey(key) || !entity.Comp.Actors.TryGetValue(key, out entityUidSet) || !entityUidSet.Contains(actor.Value))
      return;
    if (!predicted)
    {
      this.CloseUiInternal(entity, key, actor.Value);
    }
    else
    {
      if (!this._timing.IsFirstTimePredicted)
        return;
      this.RaisePredictiveEvent<BoundUIWrapMessage>(new BoundUIWrapMessage(this.GetNetEntity(entity.Owner), (BoundUserInterfaceMessage) new CloseBoundInterfaceMessage(), key));
    }
  }

  public bool TryOpenUi(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    EntityUid actor,
    bool predicted = false)
  {
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false))
      return false;
    this.OpenUi(entity, key, new EntityUid?(actor), predicted);
    HashSet<EntityUid> entityUidSet;
    return entity.Comp.Actors.TryGetValue(key, out entityUidSet) && entityUidSet.Contains(actor);
  }

  public virtual void OpenUi(Entity<UserInterfaceComponent?> entity, Enum key, bool predicted = false)
  {
  }

  public void OpenUi(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    EntityUid? actor,
    bool predicted = false)
  {
    HashSet<EntityUid> entityUidSet;
    if (!actor.HasValue || !this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.Interfaces.ContainsKey(key) || entity.Comp.Actors.TryGetValue(key, out entityUidSet) && entityUidSet.Contains(actor.Value))
      return;
    if (predicted)
    {
      if (!this._timing.IsFirstTimePredicted)
        return;
      this.RaisePredictiveEvent<BoundUIWrapMessage>(new BoundUIWrapMessage(this.GetNetEntity(entity.Owner), (BoundUserInterfaceMessage) new OpenBoundInterfaceMessage(), key));
    }
    else
      this.OnMessageReceived(new BoundUIWrapMessage(this.GetNetEntity(entity.Owner), (BoundUserInterfaceMessage) new OpenBoundInterfaceMessage(), key), actor.Value);
  }

  public void OpenUi(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    ICommonSession actor,
    bool predicted = false)
  {
    EntityUid? attachedEntity = actor.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    this.OpenUi(entity, key, new EntityUid?(attachedEntity.Value), predicted);
  }

  public virtual bool TryGetPosition(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    out Vector2 position)
  {
    position = Vector2.Zero;
    return false;
  }

  protected virtual void SavePosition(BoundUserInterface bui)
  {
  }

  public void SetUiState(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    BoundUserInterfaceState? state)
  {
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.Interfaces.ContainsKey(key))
      return;
    if (state == null)
    {
      if (!entity.Comp.States.Remove(key))
        return;
      this.DirtyField<UserInterfaceComponent>(entity, "States");
    }
    else
    {
      bool exists;
      ref BoundUserInterfaceState local = ref CollectionsMarshal.GetValueRefOrAddDefault<Enum, BoundUserInterfaceState>(entity.Comp.States, key, out exists);
      if (exists)
      {
        BoundUserInterfaceState userInterfaceState = local;
        if ((userInterfaceState != null ? (userInterfaceState.Equals((object) state) ? 1 : 0) : 0) != 0)
          return;
      }
      local = state;
    }
    BoundUserInterface boundUserInterface;
    if (state != null && this._netManager.IsClient && entity.Comp.ClientOpenInterfaces.TryGetValue(key, out boundUserInterface))
    {
      BoundUserInterfaceState state1 = boundUserInterface.State;
      if ((state1 != null ? (!state1.Equals((object) state) ? 1 : 0) : 1) != 0)
      {
        boundUserInterface.UpdateState(state);
        boundUserInterface.Update();
      }
    }
    this.DirtyField<UserInterfaceComponent>(entity, "States");
  }

  public bool HasUi(EntityUid uid, Enum uiKey, UserInterfaceComponent? ui = null)
  {
    return this.Resolve<UserInterfaceComponent>(uid, ref ui, false) && ui.Interfaces.ContainsKey(uiKey);
  }

  public bool IsUiOpen(Entity<UserInterfaceComponent?> entity, Enum uiKey)
  {
    HashSet<EntityUid> entityUidSet;
    return this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) && entity.Comp.Actors.TryGetValue(uiKey, out entityUidSet) && entityUidSet.Count > 0;
  }

  public bool IsUiOpen(Entity<UserInterfaceComponent?> entity, Enum uiKey, EntityUid actor)
  {
    HashSet<EntityUid> entityUidSet;
    return this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) && entity.Comp.Actors.TryGetValue(uiKey, out entityUidSet) && entityUidSet.Contains(actor);
  }

  public bool IsUiOpen(Entity<UserInterfaceComponent?> entity, IEnumerable<Enum> uiKeys)
  {
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false))
      return false;
    foreach (Enum uiKey in uiKeys)
    {
      if (entity.Comp.Actors.ContainsKey(uiKey))
        return true;
    }
    return false;
  }

  public bool IsAnyUiOpen(Entity<UserInterfaceComponent?> entity)
  {
    return this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) && entity.Comp.Actors.Count > 0;
  }

  public void RaiseUiMessage(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    BoundUserInterfaceMessage message)
  {
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.Actors.TryGetValue(key, out HashSet<EntityUid> _))
      return;
    this.OnMessageReceived(new BoundUIWrapMessage(this.GetNetEntity(entity.Owner), message, key), message.Actor);
  }

  public void ServerSendUiMessage(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    BoundUserInterfaceMessage message)
  {
    HashSet<EntityUid> source;
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.Actors.TryGetValue(key, out source))
      return;
    Filter filter = Filter.Entities(source.ToArray<EntityUid>());
    this.RaiseNetworkEvent((EntityEventArgs) new BoundUIWrapMessage(this.GetNetEntity(entity.Owner), message, key), filter);
  }

  public void ServerSendUiMessage(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    BoundUserInterfaceMessage message,
    EntityUid actor)
  {
    HashSet<EntityUid> entityUidSet;
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.Actors.TryGetValue(key, out entityUidSet) || !entityUidSet.Contains(actor))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new BoundUIWrapMessage(this.GetNetEntity(entity.Owner), message, key), actor);
  }

  public void ServerSendUiMessage(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    BoundUserInterfaceMessage message,
    ICommonSession actor)
  {
    if (!this._netManager.IsClient || !this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false))
      return;
    EntityUid? attachedEntity = actor.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    HashSet<EntityUid> entityUidSet;
    if (!entity.Comp.Actors.TryGetValue(key, out entityUidSet) || !entityUidSet.Contains(valueOrDefault))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new BoundUIWrapMessage(this.GetNetEntity(entity.Owner), message, key), actor);
  }

  public void ClientSendUiMessage(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    BoundUserInterfaceMessage message)
  {
    EntityUid? localEntity = this.Player.LocalEntity;
    HashSet<EntityUid> entityUidSet;
    if (!localEntity.HasValue || !this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.Actors.TryGetValue(key, out entityUidSet) || !entityUidSet.Contains(localEntity.Value))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new BoundUIWrapMessage(this.GetNetEntity(entity.Owner), message, key));
  }

  public void CloseUserUis<T>(Entity<UserInterfaceUserComponent?> actor) where T : Enum
  {
    if (!this.UserQuery.Resolve(actor.Owner, ref actor.Comp, false) || actor.Comp.OpenInterfaces.Count == 0)
      return;
    ValueList<Enum> valueList = new ValueList<Enum>();
    foreach ((EntityUid entityUid, List<Enum> list) in actor.Comp.OpenInterfaces)
    {
      valueList.Clear();
      valueList.AddRange(list);
      foreach (Enum key in valueList)
      {
        if (key is T)
          this.CloseUiInternal((Entity<UserInterfaceComponent>) entityUid, key, actor.Owner);
      }
    }
  }

  public void CloseUserUis(Entity<UserInterfaceUserComponent?> actor)
  {
    if (!this.UserQuery.Resolve(actor.Owner, ref actor.Comp, false) || actor.Comp.OpenInterfaces.Count == 0)
      return;
    ValueList<Enum> valueList = new ValueList<Enum>();
    foreach ((EntityUid entityUid, List<Enum> list) in actor.Comp.OpenInterfaces)
    {
      valueList.Clear();
      valueList.AddRange(list);
      foreach (Enum key in valueList)
        this.CloseUiInternal((Entity<UserInterfaceComponent>) entityUid, key, actor.Owner);
    }
  }

  public void CloseUis(Entity<UserInterfaceComponent?> entity)
  {
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false))
      return;
    ValueList<EntityUid> valueList = new ValueList<EntityUid>();
    foreach ((Enum key, HashSet<EntityUid> select) in entity.Comp.Actors)
    {
      valueList.Clear();
      valueList.AddRange((IEnumerable<EntityUid>) select);
      foreach (EntityUid actor in valueList)
        this.CloseUiInternal(entity, key, actor);
    }
  }

  public void CloseUis(Entity<UserInterfaceComponent?> entity, EntityUid actor)
  {
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false))
      return;
    foreach (Enum key in entity.Comp.Interfaces.Keys)
      this.CloseUiInternal(entity, key, actor);
  }

  public void CloseUis(Entity<UserInterfaceComponent?> entity, ICommonSession actor)
  {
    EntityUid? attachedEntity = actor.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false))
      return;
    this.CloseUis(entity, valueOrDefault);
  }

  public bool TryGetOpenUi(
    Entity<UserInterfaceComponent?> entity,
    Enum uiKey,
    [NotNullWhen(true)] out BoundUserInterface? bui)
  {
    bui = (BoundUserInterface) null;
    return this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) && entity.Comp.ClientOpenInterfaces.TryGetValue(uiKey, out bui);
  }

  public bool TryGetOpenUi<T>(Entity<UserInterfaceComponent?> entity, Enum uiKey, [NotNullWhen(true)] out T? bui) where T : BoundUserInterface
  {
    BoundUserInterface boundUserInterface;
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.ClientOpenInterfaces.TryGetValue(uiKey, out boundUserInterface))
    {
      bui = default (T);
      return false;
    }
    bui = (T) boundUserInterface;
    return true;
  }

  public bool TryToggleUi(Entity<UserInterfaceComponent?> entity, Enum uiKey, ICommonSession actor)
  {
    EntityUid? attachedEntity = actor.AttachedEntity;
    if (!attachedEntity.HasValue)
      return false;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    return this.TryToggleUi(entity, uiKey, valueOrDefault);
  }

  public bool TryToggleUi(Entity<UserInterfaceComponent?> entity, Enum uiKey, EntityUid actor)
  {
    if (!this.UIQuery.Resolve(entity.Owner, ref entity.Comp, false) || !entity.Comp.Interfaces.ContainsKey(uiKey))
      return false;
    HashSet<EntityUid> entityUidSet;
    if (entity.Comp.Actors.TryGetValue(uiKey, out entityUidSet) && entityUidSet.Contains(actor))
      this.CloseUi(entity, uiKey, new EntityUid?(actor));
    else
      this.OpenUi(entity, uiKey, new EntityUid?(actor));
    return true;
  }

  public void SendPredictedUiMessage(BoundUserInterface bui, BoundUserInterfaceMessage msg)
  {
    this.RaisePredictiveEvent<BoundUIWrapMessage>(new BoundUIWrapMessage(this.GetNetEntity(bui.Owner), msg, bui.UiKey));
  }

  public bool TryGetInterfaceData(
    Entity<UserInterfaceComponent?> entity,
    Enum key,
    [NotNullWhen(true)] out InterfaceData? data)
  {
    data = (InterfaceData) null;
    return this.Resolve<UserInterfaceComponent>((EntityUid) entity, ref entity.Comp, false) && entity.Comp.Interfaces.TryGetValue(key, out data);
  }

  public float GetUiRange(Entity<UserInterfaceComponent?> entity, Enum key)
  {
    InterfaceData data;
    this.TryGetInterfaceData(entity, key, out data);
    return data == null ? 0.0f : data.InteractionRange;
  }

  public override void Update(float frameTime)
  {
    if (this._timing.IsFirstTimePredicted)
    {
      foreach ((BoundUserInterface Bui, bool value) queuedBui in this._queuedBuis)
      {
        BoundUserInterface bui = queuedBui.Bui;
        if (queuedBui.value)
        {
          try
          {
            bui.Open();
            UserInterfaceComponent component;
            if (this.UIQuery.TryComp(bui.Owner, out component))
            {
              BoundUserInterfaceState state;
              if (component.States.TryGetValue(bui.UiKey, out state))
              {
                bui.State = state;
                bui.UpdateState(state);
                bui.Update();
              }
            }
          }
          catch (Exception ex)
          {
            this.Log.Error($"Caught exception while attempting to create a BUI {bui.UiKey} with type {bui.GetType()} on entity {this.ToPrettyString((Entity<MetaDataComponent>) bui.Owner)}. Exception: {ex}");
          }
        }
        else
        {
          UserInterfaceComponent component;
          if (this.UIQuery.TryComp(bui.Owner, out component))
            component.ClientOpenInterfaces.Remove(bui.UiKey);
          try
          {
            if (!this.TerminatingOrDeleted(bui.Owner))
              this.SavePosition(bui);
            bui.Dispose();
          }
          catch (Exception ex)
          {
            this.Log.Error($"Caught exception while attempting to dispose of a BUI {bui.UiKey} with type {bui.GetType()} on entity {this.ToPrettyString((Entity<MetaDataComponent>) bui.Owner)}. Exception: {ex}");
          }
        }
      }
      this._queuedBuis.Clear();
    }
    AllEntityQueryEnumerator<ActiveUserInterfaceComponent, UserInterfaceComponent> entityQueryEnumerator = this.AllEntityQuery<ActiveUserInterfaceComponent, UserInterfaceComponent>();
    this._rangeJob.ActorRanges.Clear();
    EntityUid uid;
    UserInterfaceComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out ActiveUserInterfaceComponent _, out comp2))
    {
      foreach ((Enum key, HashSet<EntityUid> entityUidSet) in comp2.Actors)
      {
        InterfaceData interfaceData = comp2.Interfaces[key];
        if ((double) interfaceData.InteractionRange > 0.0)
        {
          foreach (EntityUid entityUid in entityUidSet)
          {
            if (!this._netManager.IsClient || entityUid.IsValid())
              this._rangeJob.ActorRanges.Add((uid, key, interfaceData, entityUid, false));
          }
        }
      }
    }
    this._parallel.ProcessNow((IParallelRobustJob) this._rangeJob, this._rangeJob.ActorRanges.Count);
    foreach ((EntityUid Ui, Enum Key, InterfaceData Data, EntityUid Actor, bool Result) actorRange in this._rangeJob.ActorRanges)
    {
      EntityUid ui = actorRange.Ui;
      EntityUid actor = actorRange.Actor;
      Enum key = actorRange.Key;
      UserInterfaceComponent component;
      if (!actorRange.Result && !this.Deleted(ui) && !this.Deleted(actor) && this.UIQuery.TryComp(ui, out component))
        this.CloseUi((Entity<UserInterfaceComponent>) (ui, component), key, new EntityUid?(actor));
    }
  }

  public void SetUi(Entity<UserInterfaceComponent?> ent, Enum key, InterfaceData data)
  {
    if (!this.Resolve<UserInterfaceComponent>((EntityUid) ent, ref ent.Comp, false))
      ent.Comp = this.AddComp<UserInterfaceComponent>((EntityUid) ent);
    ent.Comp.Interfaces[key] = data;
    this.DirtyField<UserInterfaceComponent>(ent, "Interfaces");
  }

  public bool TryGetUiState<T>(Entity<UserInterfaceComponent?> ent, Enum key, [NotNullWhen(true)] out T? state) where T : BoundUserInterfaceState
  {
    BoundUserInterfaceState userInterfaceState;
    if (!this.Resolve<UserInterfaceComponent>((EntityUid) ent, ref ent.Comp, false) || !ent.Comp.States.TryGetValue(key, out userInterfaceState))
    {
      state = default (T);
      return false;
    }
    state = (T) userInterfaceState;
    return true;
  }

  private bool CheckRange(
    Entity<TransformComponent> UiEnt,
    Enum key,
    InterfaceData data,
    Entity<TransformComponent> actor)
  {
    if (actor.Comp.MapID != UiEnt.Comp.MapID)
      return false;
    BoundUserInterfaceCheckRangeEvent args = new BoundUserInterfaceCheckRangeEvent(UiEnt, key, data, actor);
    this.RaiseLocalEvent<BoundUserInterfaceCheckRangeEvent>(UiEnt.Owner, ref args, true);
    if (args.Result == BoundUserInterfaceRangeResult.Pass || this._ignoreUIRangeQuery.HasComponent((EntityUid) actor))
      return true;
    return args.Result != BoundUserInterfaceRangeResult.Fail && this._transforms.InRange(UiEnt, (Entity<TransformComponent>) (actor.Owner, actor.Comp), data.InteractionRange);
  }

  private record struct ActorRangeCheckJob : IParallelRobustJob, IParallelRangeRobustJob
  {
    public required Robust.Shared.GameObjects.EntityQuery<TransformComponent> XformQuery;
    public required SharedUserInterfaceSystem System;
    public readonly List<(EntityUid Ui, Enum Key, InterfaceData Data, EntityUid Actor, bool Result)> ActorRanges;

    public ActorRangeCheckJob()
    {
      this.XformQuery = new Robust.Shared.GameObjects.EntityQuery<TransformComponent>();
      this.System = (SharedUserInterfaceSystem) null;
      this.ActorRanges = new List<(EntityUid, Enum, InterfaceData, EntityUid, bool)>();
    }

    public void Execute(int index)
    {
      (EntityUid Ui, Enum Key, InterfaceData Data, EntityUid Actor, bool Result) actorRange = this.ActorRanges[index];
      TransformComponent component1;
      TransformComponent component2;
      actorRange.Result = this.XformQuery.TryComp(actorRange.Ui, out component1) && this.XformQuery.TryComp(actorRange.Actor, out component2) && this.System.CheckRange((Entity<TransformComponent>) (actorRange.Ui, component1), actorRange.Key, actorRange.Data, (Entity<TransformComponent>) (actorRange.Actor, component2));
      this.ActorRanges[index] = actorRange;
    }

    [CompilerGenerated]
    public override readonly int GetHashCode()
    {
      return (EqualityComparer<Robust.Shared.GameObjects.EntityQuery<TransformComponent>>.Default.GetHashCode(this.XformQuery) * -1521134295 + EqualityComparer<SharedUserInterfaceSystem>.Default.GetHashCode(this.System)) * -1521134295 + EqualityComparer<List<(EntityUid, Enum, InterfaceData, EntityUid, bool)>>.Default.GetHashCode(this.ActorRanges);
    }

    [CompilerGenerated]
    public readonly bool Equals(SharedUserInterfaceSystem.ActorRangeCheckJob other)
    {
      return EqualityComparer<Robust.Shared.GameObjects.EntityQuery<TransformComponent>>.Default.Equals(this.XformQuery, other.XformQuery) && EqualityComparer<SharedUserInterfaceSystem>.Default.Equals(this.System, other.System) && EqualityComparer<List<(EntityUid, Enum, InterfaceData, EntityUid, bool)>>.Default.Equals(this.ActorRanges, other.ActorRanges);
    }
  }
}
