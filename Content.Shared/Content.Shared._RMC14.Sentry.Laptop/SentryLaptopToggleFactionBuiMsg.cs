using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public sealed class SentryLaptopToggleFactionBuiMsg(NetEntity sentry, string faction, bool targeted) : BoundUserInterfaceMessage
{
	public NetEntity Sentry = sentry;

	public string Faction = faction;

	public bool Targeted = targeted;
}
