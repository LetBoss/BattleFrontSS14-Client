using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public sealed class SentryLaptopGlobalSetFactionsBuiMsg(List<string> factions) : BoundUserInterfaceMessage
{
	public List<string> Factions = factions;
}
