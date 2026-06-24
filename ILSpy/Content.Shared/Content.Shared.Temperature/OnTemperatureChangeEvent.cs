using Robust.Shared.GameObjects;

namespace Content.Shared.Temperature;

public sealed class OnTemperatureChangeEvent : EntityEventArgs
{
	public readonly float CurrentTemperature;

	public readonly float LastTemperature;

	public readonly float TemperatureDelta;

	public OnTemperatureChangeEvent(float current, float last, float delta)
	{
		CurrentTemperature = current;
		LastTemperature = last;
		TemperatureDelta = delta;
	}
}
