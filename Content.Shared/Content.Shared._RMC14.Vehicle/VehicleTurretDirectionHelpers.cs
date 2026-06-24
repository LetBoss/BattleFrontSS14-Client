using System;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Vehicle;

public static class VehicleTurretDirectionHelpers
{
	private const double SpriteDirectionBiasRadians = -0.05;

	public static Direction GetRenderAlignedCardinalDir(Angle facing)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Angle val = ((Angle)(ref facing)).Reduced();
		Angle val2 = ((Angle)(ref val)).FlipPositive();
		double mod = Math.Floor(val2.Theta / 1.5707963705062866) % 2.0 - 0.5;
		return (Direction)(((int)Math.Round((val2.Theta + mod * -0.05) / 1.5707963705062866) % 4) switch
		{
			0 => 0, 
			1 => 2, 
			2 => 4, 
			_ => 6, 
		});
	}
}
