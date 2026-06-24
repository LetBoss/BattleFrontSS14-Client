using Robust.Shared.GameObjects;

namespace Content.Shared.Cargo;

[ByRefEvent]
public record struct PriceCalculationEvent()
{
	public double Price = 0.0;

	public bool Handled = false;
}
