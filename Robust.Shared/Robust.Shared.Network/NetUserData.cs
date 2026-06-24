using System;
using System.Collections.Immutable;
using System.Text;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Network;

public sealed record NetUserData
{
	[ViewVariables]
	public NetUserId UserId { get; }

	[ViewVariables]
	public string UserName { get; }

	[ViewVariables]
	public string? PatronTier { get; init; }

	[ViewVariables]
	public DateTime? CreatedTime { get; init; }

	public ImmutableArray<byte> HWId { get; init; }

	public ImmutableArray<ImmutableArray<byte>> ModernHWIds { get; init; }

	public float Trust { get; init; }

	public NetUserData(NetUserId userId, string userName)
	{
		UserId = userId;
		UserName = userName;
	}

	public sealed override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("NetUserData");
		stringBuilder.Append(" { ");
		if (this with
		{
			HWId = default(ImmutableArray<byte>)
		}.PrintMembers(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}
}
