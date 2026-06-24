using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Parasite;

[ByRefEvent]
public record struct GetInfectedIncubationMultiplierEvent(int stage)
{
	public List<float> Additions = new List<float>();

	public List<float> Multipliers = new List<float>();

	public void Add(float multiplier)
	{
		Additions.Add(multiplier);
	}

	public void Multiply(float multiplier)
	{
		Multipliers.Add(multiplier);
	}
}
