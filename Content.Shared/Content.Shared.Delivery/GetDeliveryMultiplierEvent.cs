using Robust.Shared.GameObjects;

namespace Content.Shared.Delivery;

[ByRefEvent]
public record struct GetDeliveryMultiplierEvent(float AdditiveMultiplier, float MultiplicativeMultiplier)
{
	public GetDeliveryMultiplierEvent()
		: this(1f, 1f)
	{
	}
}
