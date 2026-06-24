using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public sealed class SentryLaptopUnlinkBuiMsg(NetEntity sentry) : BoundUserInterfaceMessage
{
	public readonly NetEntity Sentry = sentry;
}
