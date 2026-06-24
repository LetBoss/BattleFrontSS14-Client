using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponDrawSkewEvent : EntityEventArgs
{
	public bool Enabled { get; }

	public float ShiftTiles { get; }

	public float SwapIntervalSeconds { get; }

	public bool SyncLagEnabled { get; }

	public float SyncLagDelaySeconds { get; }

	public float SyncLagJitterSeconds { get; }

	public float SyncLagMaxOffsetTiles { get; }

	public RMCWeaponDrawSkewEvent(bool enabled, float shiftTiles, float swapIntervalSeconds, bool syncLagEnabled, float syncLagDelaySeconds, float syncLagJitterSeconds, float syncLagMaxOffsetTiles)
	{
		Enabled = enabled;
		ShiftTiles = shiftTiles;
		SwapIntervalSeconds = swapIntervalSeconds;
		SyncLagEnabled = syncLagEnabled;
		SyncLagDelaySeconds = syncLagDelaySeconds;
		SyncLagJitterSeconds = syncLagJitterSeconds;
		SyncLagMaxOffsetTiles = syncLagMaxOffsetTiles;
	}
}
