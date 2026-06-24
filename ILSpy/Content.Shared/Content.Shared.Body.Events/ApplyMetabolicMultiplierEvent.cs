using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events;

[ByRefEvent]
public readonly record struct ApplyMetabolicMultiplierEvent(float Multiplier)
{
	public readonly float Multiplier = Multiplier;

	[CompilerGenerated]
	public void Deconstruct(out float Multiplier)
	{
		Multiplier = this.Multiplier;
	}
}
