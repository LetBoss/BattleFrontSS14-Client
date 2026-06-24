using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public sealed class SentryLaptopSetNameBuiMsg(NetEntity sentry, string name) : BoundUserInterfaceMessage
{
	public NetEntity Sentry = sentry;

	public string Name = name;
}
