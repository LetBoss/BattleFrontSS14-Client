using System;
using Robust.Shared.Maths;

namespace Content.Shared._PUBG.FogOfWar;

public static class PubgFogOfWarFov
{
	public static float GetFovCosine(float fovAngle)
	{
		return MathF.Cos(MathHelper.DegreesToRadians(fovAngle / 2f));
	}
}
