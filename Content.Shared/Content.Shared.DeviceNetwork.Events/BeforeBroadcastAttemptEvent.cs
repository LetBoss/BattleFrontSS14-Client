using System.Collections.Generic;
using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceNetwork.Events;

public sealed class BeforeBroadcastAttemptEvent : CancellableEntityEventArgs
{
	public readonly IReadOnlySet<DeviceNetworkComponent> Recipients;

	public HashSet<DeviceNetworkComponent>? ModifiedRecipients;

	public BeforeBroadcastAttemptEvent(IReadOnlySet<DeviceNetworkComponent> recipients)
	{
		Recipients = recipients;
	}
}
