using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class ModeMenuStatusEvent : EntityEventArgs
{
	public int ServerOnlineCount { get; }

	public int PubgOnlineCount { get; }

	public int Civ14OnlineCount { get; }

	public bool PubgEnabled { get; }

	public bool Civ14Enabled { get; }

	public ModeMenuStatusEvent(int serverOnlineCount, int pubgOnlineCount, int civ14OnlineCount, bool pubgEnabled, bool civ14Enabled)
	{
		ServerOnlineCount = serverOnlineCount;
		PubgOnlineCount = pubgOnlineCount;
		Civ14OnlineCount = civ14OnlineCount;
		PubgEnabled = pubgEnabled;
		Civ14Enabled = civ14Enabled;
	}
}
