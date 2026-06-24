using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Gateway;

[Serializable]
[NetSerializable]
public sealed class GatewayBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly List<GatewayDestinationData> Destinations;

	public readonly NetEntity? Current;

	public readonly TimeSpan NextReady;

	public readonly TimeSpan Cooldown;

	public readonly TimeSpan NextUnlock;

	public readonly TimeSpan UnlockTime;

	public GatewayBoundUserInterfaceState(List<GatewayDestinationData> destinations, NetEntity? current, TimeSpan nextReady, TimeSpan cooldown, TimeSpan nextUnlock, TimeSpan unlockTime)
	{
		Destinations = destinations;
		Current = current;
		NextReady = nextReady;
		Cooldown = cooldown;
		NextUnlock = nextUnlock;
		UnlockTime = unlockTime;
	}
}
