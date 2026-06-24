using System.Collections.Generic;
using Robust.Shared.Localization;

namespace Content.Shared.Atmos;

public static class Atmospherics
{
	public const float R = 8.314463f;

	public const float OneAtmosphere = 101.325f;

	public const float GasMinerDefaultMaxExternalPressure = 6500f;

	public const float TCMB = 2.7f;

	public const float T0C = 273.15f;

	public const float T20C = 293.15f;

	public const float FreezerTemp = 235f;

	public const float Tmax = 262144f;

	public const float CellVolume = 2500f;

	public const float BreathVolume = 0.5f;

	public const float BreathPercentage = 0.0002f;

	public const float MolesCellStandard = 103.92799f;

	public const float MolesCellFreezer = 129.64464f;

	public const float MolesCellGasMiner = 6666.982f;

	public const float MCellWithRatio = 0.51963997f;

	public const float OxygenStandard = 0.21f;

	public const float NitrogenStandard = 0.79f;

	public const float OxygenMolesStandard = 21.824879f;

	public const float NitrogenMolesStandard = 82.10312f;

	public const float OxygenMolesFreezer = 27.225372f;

	public const float NitrogenMolesFreezer = 102.419266f;

	public const float OxygenMolesGasMiner = 1400.0662f;

	public const float NitrogenMolesGasMiner = 5266.916f;

	public const float FactorGasVisibleMax = 20f;

	public const float GasMinMoles = 5E-08f;

	public const float OpenHeatTransferCoefficient = 0.4f;

	public const float HeatCapacityVacuum = 7000f;

	public const float MinimumAirRatioToSuspend = 0.1f;

	public const float MinimumAirRatioToMove = 0.001f;

	public const float MinimumAirToSuspend = 10.392799f;

	public const float MinimumTemperatureToMove = 393.15f;

	public const float MinimumMolesDeltaToMove = 0.103928f;

	public const float MinimumTemperatureDeltaToSuspend = 4f;

	public const float MinimumTemperatureDeltaToConsider = 0.01f;

	public const float MinimumTemperatureStartSuperConduction = 693.15f;

	public const float MinimumTemperatureForSuperconduction = 373.15f;

	public const float MinimumHeatCapacity = 0.0003f;

	public const float SpaceHeatCapacity = 7000f;

	public static Dictionary<Gas, string> GasAbbreviations = new Dictionary<Gas, string>
	{
		[Gas.Ammonia] = Loc.GetString("gas-ammonia-abbreviation"),
		[Gas.CarbonDioxide] = Loc.GetString("gas-carbon-dioxide-abbreviation"),
		[Gas.Frezon] = Loc.GetString("gas-frezon-abbreviation"),
		[Gas.Nitrogen] = Loc.GetString("gas-nitrogen-abbreviation"),
		[Gas.NitrousOxide] = Loc.GetString("gas-nitrous-oxide-abbreviation"),
		[Gas.Oxygen] = Loc.GetString("gas-oxygen-abbreviation"),
		[Gas.Plasma] = Loc.GetString("gas-plasma-abbreviation"),
		[Gas.Tritium] = Loc.GetString("gas-tritium-abbreviation"),
		[Gas.WaterVapor] = Loc.GetString("gas-water-vapor-abbreviation")
	};

	public const int ExcitedGroupBreakdownCycles = 4;

	public const int ExcitedGroupsDismantleCycles = 16;

	public const int MonstermosHardTileLimit = 2000;

	public const int MonstermosTileLimit = 200;

	public const int TotalNumberOfGases = 9;

	public const int AdjustedNumberOfGases = 12;

	public const float FireHydrogenEnergyReleased = 284000f;

	public const float FireMinimumTemperatureToExist = 373.15f;

	public const float FireMinimumTemperatureToSpread = 423.15f;

	public const float FireSpreadRadiosityScale = 0.85f;

	public const float FirePlasmaEnergyReleased = 160000f;

	public const float FireGrowthRate = 40000f;

	public const float SuperSaturationThreshold = 96f;

	public const float SuperSaturationEnds = 32f;

	public const float OxygenBurnRateBase = 1.4f;

	public const float PlasmaMinimumBurnTemperature = 373.15f;

	public const float PlasmaUpperTemperature = 1643.15f;

	public const float PlasmaOxygenFullburn = 10f;

	public const float PlasmaBurnRateDelta = 9f;

	public const float MinimumTritiumOxyburnEnergy = 143000f;

	public const float TritiumBurnOxyFactor = 100f;

	public const float TritiumBurnTritFactor = 10f;

	public const float FrezonCoolLowerTemperature = 23.15f;

	public const float FrezonCoolMidTemperature = 373.15f;

	public const float FrezonCoolMaximumEnergyModifier = 10f;

	public const float FrezonNitrogenCoolRatio = 5f;

	public const float FrezonCoolEnergyReleased = -600000f;

	public const float FrezonCoolRateModifier = 20f;

	public const float FrezonProductionMaxEfficiencyTemperature = 73.15f;

	public const float FrezonProductionNitrogenRatio = 10f;

	public const float FrezonProductionTritRatio = 8f;

	public const float FrezonProductionConversionRate = 50f;

	public const float N2ODecompositionRate = 2f;

	public const float AmmoniaOxygenReactionRate = 10f;

	public const float HazardHighPressure = 550f;

	public const float WarningHighPressure = 385f;

	public const float WarningLowPressure = 50f;

	public const float HazardLowPressure = 20f;

	public const float PressureDamageCoefficient = 4f;

	public const int MaxHighPressureDamage = 4;

	public const int LowPressureDamage = 4;

	public const float WindowHeatTransferCoefficient = 0.1f;

	public const int Directions = 4;

	public const float NormalBodyTemperature = 37f;

	public const float BreathMolesToReagentMultiplier = 1144f;

	public const float MaxOutputPressure = 4500f;

	public const float MaxTransferRate = 200f;
}
