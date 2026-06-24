using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.HeliSupply;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliFlybyInstance
{
	public required CivHeliPath Path;

	public required float DropDist;

	public required float Speed;

	public required float Alpha;

	public required float Scale;

	public required float TakeoffScale;

	public required float DropScale;

	public required float TakeoffZone;

	public required float DropZone;

	public required float ClimbZone;

	public required float AngleOffset;

	public required float DustRate;

	public required CivAirstrikeSide Side;

	public required MapId MapId;

	public required TimeSpan StartTime;

	public EntityUid? AudioEntity;

	public float DustAccumulator;

	public readonly List<CivHeliDustParticle> Dust = new List<CivHeliDustParticle>();
}
