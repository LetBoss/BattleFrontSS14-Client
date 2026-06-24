using Robust.Shared.GameObjects;

namespace Content.Shared.Alert.Components;

[ByRefEvent]
public record struct GetGenericAlertCounterAmountEvent(AlertPrototype Alert, int? Amount = null)
{
	public bool Handled => Amount.HasValue;
}
