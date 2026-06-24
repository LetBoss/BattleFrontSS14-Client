using System;

namespace Robust.Shared.Network;

public struct NetApproval
{
	private readonly string? _denyReason;

	public bool IsApproved => _denyReason == null;

	public string DenyReason
	{
		get
		{
			if (_denyReason == null)
			{
				throw new InvalidOperationException("This was not a denial.");
			}
			return _denyReason;
		}
	}

	private NetApproval(string? denyReason)
	{
		_denyReason = denyReason;
	}

	public static NetApproval Deny(string reason)
	{
		return new NetApproval(reason);
	}

	public static NetApproval Allow()
	{
		return new NetApproval(null);
	}
}
