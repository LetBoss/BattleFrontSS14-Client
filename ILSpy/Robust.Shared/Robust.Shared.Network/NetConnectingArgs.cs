using System;
using System.Net;

namespace Robust.Shared.Network;

public sealed class NetConnectingArgs : EventArgs
{
	public bool IsDenied => DenyReasonData != null;

	public string? DenyReason => DenyReasonData?.Text;

	public NetDenyReason? DenyReasonData { get; private set; }

	public NetUserData UserData { get; }

	public NetUserId UserId => UserData.UserId;

	public string UserName => UserData.UserName;

	public IPEndPoint IP { get; }

	public LoginType AuthType { get; }

	public void Deny(string reason)
	{
		Deny(new NetDenyReason(reason));
	}

	public void Deny(NetDenyReason reason)
	{
		DenyReasonData = reason;
	}

	public NetConnectingArgs(NetUserData data, IPEndPoint ip, LoginType authType)
	{
		UserData = data;
		IP = ip;
		AuthType = authType;
	}
}
