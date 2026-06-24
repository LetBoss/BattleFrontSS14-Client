using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

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

	[ViewVariables]
	protected readonly Dictionary<string, NetUserId> UserIdMap = new Dictionary<string, NetUserId>();

	[ViewVariables]
	protected readonly Dictionary<NetUserId, SessionData> PlayerData = new Dictionary<NetUserId, SessionData>();

	protected readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

	[ViewVariables]
	protected readonly Dictionary<NetUserId, ICommonSession> InternalSessions = new Dictionary<NetUserId, ICommonSession>();

	[ViewVariables]
	public virtual int MaxPlayers { get; protected set; }

	[ViewVariables]
	public int PlayerCount => InternalSessions.Count;

	[ViewVariables]
	public ICommonSession? LocalSession { get; protected set; }

	[ViewVariables]
	public NetUserId? LocalUser => LocalSession?.UserId;

	[ViewVariables]
	public EntityUid? LocalEntity => LocalSession?.AttachedEntity;

	public IReadOnlyDictionary<NetUserId, ICommonSession> SessionsDict
	{
		get
		{
			Lock.EnterReadLock();
			try
			{
				return InternalSessions.ShallowClone();
			}
			finally
			{
				Lock.ExitReadLock();
			}
		}
	}

	public ICommonSession[] Sessions
	{
		get
		{
			Lock.EnterReadLock();
			try
			{
				return InternalSessions.Values.ToArray();
			}
			finally
			{
				Lock.ExitReadLock();
			}
		}
	}

	public virtual ICommonSession[] NetworkedSessions => Sessions;

	public event EventHandler<SessionStatusEventArgs>? PlayerStatusChanged;

	public virtual void Initialize(int maxPlayers)
	{
		MaxPlayers = maxPlayers;
		Sawmill = LogMan.GetSawmill("player");
	}

	public virtual void Startup()
	{
	}

	public virtual void Shutdown()
	{
		InternalSessions.Clear();
		UserIdMap.Clear();
		PlayerData.Clear();
	}

	public bool TryGetUserId(string userName, out NetUserId userId)
	{
		return UserIdMap.TryGetValue(userName, out userId);
	}

	public SessionData GetPlayerData(NetUserId userId)
	{
		return PlayerData[userId];
	}

	public bool TryGetPlayerData(NetUserId userId, [NotNullWhen(true)] out SessionData? data)
	{
		return PlayerData.TryGetValue(userId, out data);
	}

	public bool TryGetPlayerDataByUsername(string userName, [NotNullWhen(true)] out SessionData? data)
	{
		data = null;
		if (UserIdMap.TryGetValue(userName, out var value))
		{
			return PlayerData.TryGetValue(value, out data);
		}
		return false;
	}

	public bool HasPlayerData(NetUserId userId)
	{
		return PlayerData.ContainsKey(userId);
	}

	public IEnumerable<SessionData> GetAllPlayerData()
	{
		return PlayerData.Values;
	}

	public bool TryGetSessionById([NotNullWhen(true)] NetUserId? user, [NotNullWhen(true)] out ICommonSession? session)
	{
		if (!user.HasValue)
		{
			session = null;
			return false;
		}
		Lock.EnterReadLock();
		try
		{
			return InternalSessions.TryGetValue(user.Value, out session);
		}
		finally
		{
			Lock.ExitReadLock();
		}
	}

	public bool TryGetSessionByUsername(string username, [NotNullWhen(true)] out ICommonSession? session)
	{
		session = null;
		if (UserIdMap.TryGetValue(username, out var value))
		{
			return TryGetSessionById(value, out session);
		}
		return false;
	}

	public ICommonSession GetSessionByChannel(INetChannel channel)
	{
		return GetSessionById(channel.UserId);
	}

	public bool TryGetSessionByChannel(INetChannel channel, [NotNullWhen(true)] out ICommonSession? session)
	{
		return TryGetSessionById(channel.UserId, out session);
	}

	public ICommonSession GetSessionById(NetUserId user)
	{
		if (!TryGetSessionById(user, out ICommonSession session))
		{
			throw new KeyNotFoundException();
		}
		return session;
	}

	public bool ValidSessionId(NetUserId user)
	{
		ICommonSession session;
		return TryGetSessionById(user, out session);
	}

	public abstract bool TryGetSessionByEntity(EntityUid uid, [NotNullWhen(true)] out ICommonSession? session);

	protected virtual CommonSession CreateSession(NetUserId user, string name, SessionData data)
	{
		return new CommonSession(user, name, data);
	}

	public ICommonSession CreateAndAddSession(INetChannel channel)
	{
		ICommonSessionInternal obj = (ICommonSessionInternal)CreateAndAddSession(channel.UserId, channel.UserName);
		obj.SetChannel(channel);
		return obj;
	}

	public ICommonSession CreateAndAddSession(NetUserId user, string name)
	{
		Lock.EnterWriteLock();
		CommonSession commonSession;
		try
		{
			UserIdMap[name] = user;
			if (!PlayerData.TryGetValue(user, out SessionData value))
			{
				value = (PlayerData[user] = new SessionData(user, name));
			}
			commonSession = CreateSession(user, name, value);
			InternalSessions.Add(user, commonSession);
		}
		finally
		{
			Lock.ExitWriteLock();
		}
		UpdateState(commonSession);
		return commonSession;
	}

	public void RemoveSession(ICommonSession session, bool removeData = false)
	{
		RemoveSession(session.UserId, removeData);
	}

	public void RemoveSession(NetUserId user, bool removeData = false)
	{
		Lock.EnterWriteLock();
		try
		{
			InternalSessions.Remove(user);
			if (removeData)
			{
				PlayerData.Remove(user);
			}
		}
		finally
		{
			Lock.ExitWriteLock();
		}
	}

	public virtual bool SetAttachedEntity([NotNullWhen(true)] ICommonSession? session, EntityUid? uid, out ICommonSession? kicked, bool force = false)
	{
		kicked = null;
		if (session == null)
		{
			return false;
		}
		if (session.AttachedEntity == uid)
		{
			return true;
		}
		if (uid.HasValue)
		{
			return Attach(session, uid.Value, out kicked, force);
		}
		Detach(session);
		return true;
	}

	private void Detach(ICommonSession session)
	{
		EntityUid? attachedEntity = session.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
			((ICommonSessionInternal)session).SetAttachedEntity(null);
			UpdateState(session);
			if (EntManager.TryGetComponent<ActorComponent>(valueOrDefault, out ActorComponent component) && (int)component.LifeStage <= 6)
			{
				component.PlayerSession = null;
				EntManager.RemoveComponent(valueOrDefault, component);
			}
			EntManager.EventBus.RaiseLocalEvent(valueOrDefault, new PlayerDetachedEvent(valueOrDefault, session), broadcast: true);
		}
	}

	private bool Attach(ICommonSession session, EntityUid uid, out ICommonSession? kicked, bool force = false)
	{
		kicked = null;
		if (!EntManager.TryGetComponent<MetaDataComponent>(uid, out MetaDataComponent component))
		{
			return false;
		}
		if ((int)component.EntityLifeStage >= 4)
		{
			return false;
		}
		if (EntManager.EnsureComponent<ActorComponent>(uid, out var component2))
		{
			if (!force)
			{
				return false;
			}
			kicked = component2.PlayerSession;
			if (kicked != null)
			{
				Detach(kicked);
			}
		}
		if (_netMan.IsServer)
		{
			EntManager.EnsureComponent<EyeComponent>(uid);
		}
		if (session.AttachedEntity.HasValue)
		{
			Detach(session);
		}
		((ICommonSessionInternal)session).SetAttachedEntity(uid);
		component2.PlayerSession = session;
		UpdateState(session);
		EntManager.EventBus.RaiseLocalEvent(uid, new PlayerAttachedEvent(uid, session), broadcast: true);
		return true;
	}

	public void SetStatus(ICommonSession session, SessionStatus status)
	{
		if (session.Status != status)
		{
			SessionStatus status2 = session.Status;
			((ICommonSessionInternal)session).SetStatus(status);
			UpdateState(session);
			this.PlayerStatusChanged?.Invoke(this, new SessionStatusEventArgs(session, status2, status));
		}
	}

	public void SetPing(ICommonSession session, short ping)
	{
		((ICommonSessionInternal)session).SetPing(ping);
		UpdateState(session);
	}

	public void SetName(ICommonSession session, string name)
	{
		((ICommonSessionInternal)session).SetName(name);
		UpdateState(session);
	}

	public void JoinGame(ICommonSession session)
	{
		SetStatus(session, SessionStatus.InGame);
	}

	public void Dirty()
	{
		LastStateUpdate = Timing.CurTick;
	}

	public void GetPlayerStates(GameTick fromTick, List<SessionState> states)
	{
		states.Clear();
		if (LastStateUpdate < fromTick)
		{
			return;
		}
		states.EnsureCapacity(InternalSessions.Count);
		foreach (ICommonSession value in InternalSessions.Values)
		{
			states.Add(value.State);
		}
	}

	public void UpdateState(ICommonSession session)
	{
		SessionState state = session.State;
		state.UserId = session.UserId;
		state.Status = session.Status;
		state.Name = session.Name;
		state.ControlledEntity = EntManager.GetNetEntity(session.AttachedEntity);
		Dirty();
	}
}
