using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Power.Generator;

public abstract class SharedGeneratorSystem : EntitySystem
{
	public static float CalcFuelEfficiency(float targetPower, float optimalPower, FuelGeneratorComponent component)
	{
		return MathF.Pow(optimalPower / targetPower, component.FuelEfficiencyConstant);
	}
}
