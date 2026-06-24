using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public sealed class SentryLaptopViewCameraBuiMsg(NetEntity sentry) : BoundUserInterfaceMessage
{
	public NetEntity Sentry = sentry;
}
