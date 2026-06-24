using System;
using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Players;

public sealed class ContentPlayerData
{
	[ViewVariables]
	public NetUserId UserId { get; }

	[ViewVariables]
	public string Name { get; }

	[ViewVariables]
	[Access(new Type[]
	{
		typeof(SharedMindSystem),
		typeof(SharedGameTicker)
	})]
	public EntityUid? Mind { get; set; }

	public bool Stealthed { get; set; }

	public ContentPlayerData(NetUserId userId, string name)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserId = userId;
		Name = name;
	}
}
