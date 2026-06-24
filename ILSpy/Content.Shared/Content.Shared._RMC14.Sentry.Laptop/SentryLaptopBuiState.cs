using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public sealed class SentryLaptopBuiState : BoundUserInterfaceState
{
	public readonly List<SentryInfo> Sentries;

	public readonly List<string> AllFactions;

	public readonly Dictionary<string, string> FactionNames;

	public SentryLaptopBuiState(List<SentryInfo> sentries, List<string> allFactions, Dictionary<string, string> factionNames)
	{
		Sentries = sentries;
		AllFactions = allFactions;
		FactionNames = factionNames;
	}
}
