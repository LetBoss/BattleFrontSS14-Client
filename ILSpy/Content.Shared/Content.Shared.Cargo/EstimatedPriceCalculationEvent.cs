using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Cargo;

[ByRefEvent]
public record struct EstimatedPriceCalculationEvent(EntityPrototype Prototype)
{
	public double Price = 0.0;

	public bool Handled = false;
}
