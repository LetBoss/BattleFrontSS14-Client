// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.SharedPlayerManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

#nullable enable
namespace Robust.Shared.Player;

internal abstract class SharedPlayerManager : ISharedPlayerManager
{
  [Dependency]
  protected readonly IEntityManager EntManager;
  [Dependency]
  protected readonly IComponentFactory Factory;
  [Dependency]
  protected readonly ILogManager LogMan;
  [Dependency]
  protected readonly IGameTiming Timing;
  [Dependency]
  private readonly INetManager _netMan;
  protected ISawmill Sawmill;
  public GameTick LastStateUpdate;
  [Robust.Shared.ViewVariables.ViewVariables]
  protected readonly Dictionary<string, NetUserId> UserIdMap = new Dictionary<string, NetUserId>();
  [Robust.Shared.ViewVariables.ViewVariables]
  protected readonly Dictionary<NetUserId, SessionData> PlayerData = new Dictionary<NetUserId, SessionData>();
  protected readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
  [Robust.Shared.ViewVariables.ViewVariables]
  protected readonly Dictionary<NetUserId, ICommonSession> InternalSessions = new Dictionary<NetUserId, ICommonSession>();

  public event EventHandler<SessionStatusEventArgs>? PlayerStatusChanged;

  [Robust.Shared.ViewVariables.ViewVariables]
  public virtual int MaxPlayers { get; protected set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int PlayerCount => this.InternalSessions.Count;

  [Robust.Shared.ViewVariables.ViewVariables]
  public ICommonSession? LocalSession { get; protected set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public NetUserId? LocalUser => this.LocalSession?.UserId;

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? LocalEntity => this.LocalSession?.AttachedEntity;

  public virtual void Initialize(int maxPlayers)
  {
    this.MaxPlayers = maxPlayers;
    this.Sawmill = this.LogMan.GetSawmill("player");
  }

  public virtual void Startup()
  {
  }

  public virtual void Shutdown()
  {
    this.InternalSessions.Clear();
    this.UserIdMap.Clear();
    this.PlayerData.Clear();
  }

  public bool TryGetUserId(string userName, out NetUserId userId)
  {
    return this.UserIdMap.TryGetValue(userName, out userId);
  }

  public SessionData GetPlayerData(NetUserId userId) => this.PlayerData[userId];

  public bool TryGetPlayerData(NetUserId userId, [NotNullWhen(true)] out SessionData? data)
  {
    return this.PlayerData.TryGetValue(userId, out data);
  }

  public bool TryGetPlayerDataByUsername(string userName, [NotNullWhen(true)] out SessionData? data)
  {
    data = (SessionData) null;
    NetUserId key;
    return this.UserIdMap.TryGetValue(userName, out key) && this.PlayerData.TryGetValue(key, out data);
  }

  public bool HasPlayerData(NetUserId userId) => this.PlayerData.ContainsKey(userId);

  public IEnumerable<SessionData> GetAllPlayerData()
  {
    return (IEnumerable<SessionData>) this.PlayerData.Values;
  }

  public IReadOnlyDictionary<NetUserId, ICommonSession> SessionsDict
  {
    get
    {
      this.Lock.EnterReadLock();
      try
      {
        return (IReadOnlyDictionary<NetUserId, ICommonSession>) this.InternalSessions.ShallowClone<NetUserId, ICommonSession>();
      }
      finally
      {
        this.Lock.ExitReadLock();
      }
    }
  }

  public ICommonSession[] Sessions
  {
    get
    {
      this.Lock.EnterReadLock();
      try
      {
        return this.InternalSessions.Values.ToArray<ICommonSession>();
      }
      finally
      {
        this.Lock.ExitReadLock();
      }
    }
  }

  public bool TryGetSessionById([NotNullWhen(true)] NetUserId? user, [NotNullWhen(true)] out ICommonSession? session)
  {
    if (!user.HasValue)
    {
      session = (ICommonSession) null;
      return false;
    }
    this.Lock.EnterReadLock();
    try
    {
      return this.InternalSessions.TryGetValue(user.Value, out session);
    }
    finally
    {
      this.Lock.ExitReadLock();
    }
  }

  public virtual ICommonSession[] NetworkedSessions => this.Sessions;

  public bool TryGetSessionByUsername(string username, [NotNullWhen(true)] out ICommonSession? session)
  {
    session = (ICommonSession) null;
    NetUserId netUserId;
    return this.UserIdMap.TryGetValue(username, out netUserId) && this.TryGetSessionById(new NetUserId?(netUserId), out session);
  }

  public ICommonSession GetSessionByChannel(INetChannel channel)
  {
    return this.GetSessionById(channel.UserId);
  }

  public bool TryGetSessionByChannel(INetChannel channel, [NotNullWhen(true)] out ICommonSession? session)
  {
    return this.TryGetSessionById(new NetUserId?(channel.UserId), out session);
  }

  public ICommonSession GetSessionById(NetUserId user)
  {
    ICommonSession session;
    if (!this.TryGetSessionById(new NetUserId?(user), out session))
      throw new KeyNotFoundException();
    return session;
  }

  public bool ValidSessionId(NetUserId user)
  {
    return this.TryGetSessionById(new NetUserId?(user), out ICommonSession _);
  }

  public abstract bool TryGetSessionByEntity(EntityUid uid, [NotNullWhen(true)] out ICommonSession? session);

  protected virtual CommonSession CreateSession(NetUserId user, string name, SessionData data)
  {
    return new CommonSession(user, name, data);
  }

  public ICommonSession CreateAndAddSession(INetChannel channel)
  {
    ICommonSessionInternal andAddSession = (ICommonSessionInternal) this.CreateAndAddSession(channel.UserId, channel.UserName);
    andAddSession.SetChannel(channel);
    return (ICommonSession) andAddSession;
  }

  public ICommonSession CreateAndAddSession(NetUserId user, string name)
  {
    this.Lock.EnterWriteLock();
    CommonSession session;
    try
    {
      this.UserIdMap[name] = user;
      SessionData data;
      if (!this.PlayerData.TryGetValue(user, out data))
        this.PlayerData[user] = data = new SessionData(user, name);
      session = this.CreateSession(user, name, data);
      this.InternalSessions.Add(user, (ICommonSession) session);
    }
    finally
    {
      this.Lock.ExitWriteLock();
    }
    this.UpdateState((ICommonSession) session);
    return (ICommonSession) session;
  }

  public void RemoveSession(ICommonSession session, bool removeData = false)
  {
    this.RemoveSession(session.UserId, removeData);
  }

  public void RemoveSession(NetUserId user, bool removeData = false)
  {
    this.Lock.EnterWriteLock();
    try
    {
      this.InternalSessions.Remove(user);
      if (!removeData)
        return;
      this.PlayerData.Remove(user);
    }
    finally
    {
      this.Lock.ExitWriteLock();
    }
  }

  public virtual bool SetAttachedEntity(
    [NotNullWhen(true)] ICommonSession? session,
    EntityUid? uid,
    out ICommonSession? kicked,
    bool force = false)
  {
    kicked = (ICommonSession) null;
    if (session == null)
      return false;
    EntityUid? attachedEntity = session.AttachedEntity;
    EntityUid? nullable = uid;
    if ((attachedEntity.HasValue == nullable.HasValue ? (attachedEntity.HasValue ? (attachedEntity.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return true;
    if (uid.HasValue)
      return this.Attach(session, uid.Value, out kicked, force);
    this.Detach(session);
    return true;
  }

  private void Detach(ICommonSession session)
  {
    EntityUid? attachedEntity = session.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    ((ICommonSessionInternal) session).SetAttachedEntity(new EntityUid?());
    this.UpdateState(session);
    ActorComponent component;
    if (this.EntManager.TryGetComponent<ActorComponent>(valueOrDefault, out component) && component.LifeStage <= ComponentLifeStage.Running)
    {
      component.PlayerSession = (ICommonSession) null;
      this.EntManager.RemoveComponent(valueOrDefault, (IComponent) component);
    }
    this.EntManager.EventBus.RaiseLocalEvent<PlayerDetachedEvent>(valueOrDefault, new PlayerDetachedEvent(valueOrDefault, session), true);
  }

  private bool Attach(
    ICommonSession session,
    EntityUid uid,
    out ICommonSession? kicked,
    bool force = false)
  {
    kicked = (ICommonSession) null;
    MetaDataComponent component1;
    if (!this.EntManager.TryGetComponent<MetaDataComponent>(uid, out component1) || component1.EntityLifeStage >= EntityLifeStage.Terminating)
      return false;
    ActorComponent component2;
    if (this.EntManager.EnsureComponent<ActorComponent>(uid, out component2))
    {
      if (!force)
        return false;
      kicked = component2.PlayerSession;
      if (kicked != null)
        this.Detach(kicked);
    }
    if (this._netMan.IsServer)
      this.EntManager.EnsureComponent<EyeComponent>(uid);
    if (session.AttachedEntity.HasValue)
      this.Detach(session);
    ((ICommonSessionInternal) session).SetAttachedEntity(new EntityUid?(uid));
    component2.PlayerSession = session;
    this.UpdateState(session);
    this.EntManager.EventBus.RaiseLocalEvent<PlayerAttachedEvent>(uid, new PlayerAttachedEvent(uid, session), true);
    return true;
  }

  public void SetStatus(ICommonSession session, SessionStatus status)
  {
    if (session.Status == status)
      return;
    SessionStatus status1 = session.Status;
    ((ICommonSessionInternal) session).SetStatus(status);
    this.UpdateState(session);
    EventHandler<SessionStatusEventArgs> playerStatusChanged = this.PlayerStatusChanged;
    if (playerStatusChanged == null)
      return;
    playerStatusChanged((object) this, new SessionStatusEventArgs(session, status1, status));
  }

  public void SetPing(ICommonSession session, short ping)
  {
    ((ICommonSessionInternal) session).SetPing(ping);
    this.UpdateState(session);
  }

  public void SetName(ICommonSession session, string name)
  {
    ((ICommonSessionInternal) session).SetName(name);
    this.UpdateState(session);
  }

  public void JoinGame(ICommonSession session) => this.SetStatus(session, SessionStatus.InGame);

  public void Dirty() => this.LastStateUpdate = this.Timing.CurTick;

  public void GetPlayerStates(GameTick fromTick, List<SessionState> states)
  {
    states.Clear();
    if (this.LastStateUpdate < fromTick)
      return;
    states.EnsureCapacity(this.InternalSessions.Count);
    foreach (ICommonSession commonSession in this.InternalSessions.Values)
      states.Add(commonSession.State);
  }

  public void UpdateState(ICommonSession session)
  {
    SessionState state = session.State;
    state.UserId = session.UserId;
    state.Status = session.Status;
    state.Name = session.Name;
    state.ControlledEntity = this.EntManager.GetNetEntity(session.AttachedEntity);
    this.Dirty();
  }
}
