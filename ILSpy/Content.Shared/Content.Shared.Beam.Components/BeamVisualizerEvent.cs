using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Beam.Components;

[Serializable]
[NetSerializable]
public sealed class BeamVisualizerEvent : EntityEventArgs
{
	public readonly NetEntity Beam;

	public readonly float DistanceLength;

	public readonly Angle UserAngle;

	public readonly string? BodyState;

	public readonly string Shader = "unshaded";

	public BeamVisualizerEvent(NetEntity beam, float distanceLength, Angle userAngle, string? bodyState = null, string shader = "unshaded")
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Beam = beam;
		DistanceLength = distanceLength;
		UserAngle = userAngle;
		BodyState = bodyState;
		Shader = shader;
	}
}
