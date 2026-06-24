using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;

namespace Robust.Shared.Player;

internal sealed class DummySession : ICommonSessionInternal, ICommonSession
{
	public DummyChannel DummyChannel;

	public EntityUid? AttachedEntity { get; set; }

	public SessionStatus Status { get; set; } = SessionStatus.Connecting;

	public NetUserId UserId => UserData.UserId;

	public string Name => UserData.UserName;

	public short Ping { get; set; }

	public INetChannel Channel
	{
		get
		{
			return DummyChannel;
		}
		[Obsolete]
		set
		{
			throw new NotSupportedException();
		}
	}

	public LoginType AuthType { get; set; } = LoginType.GuestAssigned;

	public HashSet<EntityUid> ViewSubscriptions { get; } = new HashSet<EntityUid>();

	public DateTime ConnectedTime { get; set; }

	public SessionState State { get; set; } = new SessionState();

	public SessionData Data { get; set; }

	public bool ClientSide { get; set; }

	public NetUserData UserData { get; set; }

	public DummySession(NetUserId userId, string userName, SessionData data)
	{
		Data = data;
		UserData = new NetUserData(userId, userName)
		{
			HWId = ImmutableArray<byte>.Empty
		};
		DummyChannel = new DummyChannel(this);
	}

	public void SetStatus(SessionStatus status)
	{
		Status = status;
	}

	public void SetAttachedEntity(EntityUid? uid)
	{
		AttachedEntity = uid;
	}

	public void SetPing(short ping)
	{
		Ping = ping;
	}

	public void SetName(string name)
	{
		UserData = new NetUserData(UserData.UserId, name)
		{
			HWId = UserData.HWId
		};
	}

	public void SetChannel(INetChannel channel)
	{
		throw new NotSupportedException();
	}
}
