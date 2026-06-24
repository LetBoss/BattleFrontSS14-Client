using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Construction;

[Serializable]
[NetSerializable]
public sealed class TryStartStructureConstructionMessage : EntityEventArgs
{
	public readonly NetCoordinates Location;

	public readonly string PrototypeName;

	public readonly Angle Angle;

	public readonly int Ack;

	public TryStartStructureConstructionMessage(NetCoordinates loc, string prototypeName, Angle angle, int ack)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Location = loc;
		PrototypeName = prototypeName;
		Angle = angle;
		Ack = ack;
	}
}
