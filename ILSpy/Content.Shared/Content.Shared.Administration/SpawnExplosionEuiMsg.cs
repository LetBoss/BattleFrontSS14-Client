using System;
using Content.Shared.Eui;
using Content.Shared.Explosion.Components;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

public static class SpawnExplosionEuiMsg
{
	[Serializable]
	[NetSerializable]
	public sealed class PreviewRequest : EuiMessageBase
	{
		public readonly MapCoordinates Epicenter;

		public readonly string TypeId;

		public readonly float TotalIntensity;

		public readonly float IntensitySlope;

		public readonly float MaxIntensity;

		public PreviewRequest(MapCoordinates epicenter, string typeId, float totalIntensity, float intensitySlope, float maxIntensity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			Epicenter = epicenter;
			TypeId = typeId;
			TotalIntensity = totalIntensity;
			IntensitySlope = intensitySlope;
			MaxIntensity = maxIntensity;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class PreviewData : EuiMessageBase
	{
		public readonly float Slope;

		public readonly float TotalIntensity;

		public readonly ExplosionVisualsState Explosion;

		public PreviewData(ExplosionVisualsState explosion, float slope, float totalIntensity)
		{
			Slope = slope;
			TotalIntensity = totalIntensity;
			Explosion = explosion;
		}
	}
}
