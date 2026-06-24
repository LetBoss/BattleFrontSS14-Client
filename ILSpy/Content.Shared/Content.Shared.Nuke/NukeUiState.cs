using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Nuke;

[Serializable]
[NetSerializable]
public sealed class NukeUiState : BoundUserInterfaceState
{
	public bool DiskInserted;

	public NukeStatus Status;

	public int RemainingTime;

	public int CooldownTime;

	public bool IsAnchored;

	public int EnteredCodeLength;

	public int MaxCodeLength;

	public bool AllowArm;
}
