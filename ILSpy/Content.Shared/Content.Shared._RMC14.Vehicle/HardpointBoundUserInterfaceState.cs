using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class HardpointBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly List<HardpointUiEntry> Hardpoints;

	public readonly float FrameIntegrity;

	public readonly float FrameMaxIntegrity;

	public readonly bool HasFrameIntegrity;

	public readonly string? Error;

	public HardpointBoundUserInterfaceState(List<HardpointUiEntry> hardpoints, float frameIntegrity, float frameMaxIntegrity, bool hasFrameIntegrity, string? error)
	{
		Hardpoints = hardpoints;
		FrameIntegrity = frameIntegrity;
		FrameMaxIntegrity = frameMaxIntegrity;
		HasFrameIntegrity = hasFrameIntegrity;
		Error = error;
	}
}
