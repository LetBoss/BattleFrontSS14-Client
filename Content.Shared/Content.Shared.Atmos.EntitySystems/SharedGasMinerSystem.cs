using System;
using Content.Shared.Atmos.Components;
using Content.Shared.Examine;
using Content.Shared.Temperature;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasMinerSystem : EntitySystem
{
	[Dependency]
	private SharedAtmosphereSystem _sharedAtmosphereSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasMinerComponent, ExaminedEvent>((EntityEventRefHandler<GasMinerComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
	}

	private void OnExamine(Entity<GasMinerComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GasMinerComponent component = ent.Comp;
		using (args.PushGroup("GasMinerComponent"))
		{
			args.PushMarkup(base.Loc.GetString("gas-miner-mines-text", (ValueTuple<string, object>)("gas", base.Loc.GetString(_sharedAtmosphereSystem.GetGas(component.SpawnGas).Name))));
			args.PushText(base.Loc.GetString("gas-miner-amount-text", (ValueTuple<string, object>)("moles", $"{component.SpawnAmount:0.#}")));
			args.PushText(base.Loc.GetString("gas-miner-temperature-text", (ValueTuple<string, object>)("tempK", $"{component.SpawnTemperature:0.#}"), (ValueTuple<string, object>)("tempC", $"{TemperatureHelpers.KelvinToCelsius(component.SpawnTemperature):0.#}")));
			if (component.MaxExternalAmount < float.PositiveInfinity)
			{
				args.PushText(base.Loc.GetString("gas-miner-moles-cutoff-text", (ValueTuple<string, object>)("moles", $"{component.MaxExternalAmount:0.#}")));
			}
			if (component.MaxExternalPressure < float.PositiveInfinity)
			{
				args.PushText(base.Loc.GetString("gas-miner-pressure-cutoff-text", (ValueTuple<string, object>)("pressure", $"{component.MaxExternalPressure:0.#}")));
			}
			ExaminedEvent examinedEvent = args;
			examinedEvent.AddMarkup(component.MinerState switch
			{
				GasMinerState.Disabled => base.Loc.GetString("gas-miner-state-disabled-text"), 
				GasMinerState.Idle => base.Loc.GetString("gas-miner-state-idle-text"), 
				GasMinerState.Working => base.Loc.GetString("gas-miner-state-working-text"), 
				_ => throw new IndexOutOfRangeException("MinerState"), 
			});
		}
	}
}
