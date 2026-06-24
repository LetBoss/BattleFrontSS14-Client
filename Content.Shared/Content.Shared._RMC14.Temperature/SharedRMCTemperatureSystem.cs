using Content.Shared.Temperature;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Temperature;

public abstract class SharedRMCTemperatureSystem : EntitySystem
{
	public virtual float GetTemperature(EntityUid entity)
	{
		return 0f;
	}

	public virtual void ForceChangeTemperature(EntityUid entity, float temperature)
	{
	}

	public virtual bool TryGetCurrentTemperature(EntityUid uid, out float temperature)
	{
		temperature = TemperatureHelpers.CelsiusToKelvin(37f);
		return true;
	}
}
