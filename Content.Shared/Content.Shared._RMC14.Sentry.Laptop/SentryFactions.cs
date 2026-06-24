using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public static class SentryFactions
{
	public static List<string> AllFactions = new List<string>();

	public static Dictionary<string, string> FactionNames = new Dictionary<string, string>();
}
