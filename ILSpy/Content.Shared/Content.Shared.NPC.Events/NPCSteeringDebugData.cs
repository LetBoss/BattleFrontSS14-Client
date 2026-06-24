using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC.Events;

[Serializable]
[NetSerializable]
public readonly record struct NPCSteeringDebugData(NetEntity EntityUid, Vector2 Direction, float[] Interest, float[] Danger, List<Vector2> DangerPoints)
{
	public readonly NetEntity EntityUid = EntityUid;

	public readonly Vector2 Direction = Direction;

	public readonly float[] Interest = Interest;

	public readonly float[] Danger = Danger;

	public readonly List<Vector2> DangerPoints = DangerPoints;

	[CompilerGenerated]
	public void Deconstruct(out NetEntity EntityUid, out Vector2 Direction, out float[] Interest, out float[] Danger, out List<Vector2> DangerPoints)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		EntityUid = this.EntityUid;
		Direction = this.Direction;
		Interest = this.Interest;
		Danger = this.Danger;
		DangerPoints = this.DangerPoints;
	}
}
