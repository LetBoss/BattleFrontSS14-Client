using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG;

[DataDefinition]
public sealed class RedZoneSettings : ISerializationGenerated<RedZoneSettings>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int BaseChancePercent = 10;

	[DataField(null, false, 1, false, false, null)]
	public int ChanceIncreaseMin = 10;

	[DataField(null, false, 1, false, false, null)]
	public int ChanceIncreaseMax = 30;

	[DataField(null, false, 1, false, false, null)]
	public float MinIntervalSeconds = 30f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxIntervalSeconds = 300f;

	[DataField(null, false, 1, false, false, null)]
	public float ZoneRadiusMinPercent = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	public float ZoneRadiusMaxPercent = 0.8f;

	[DataField(null, false, 1, false, false, null)]
	public float ZoneDurationMinSeconds = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float ZoneDurationMaxSeconds = 30f;

	[DataField(null, false, 1, false, false, null)]
	public float BombInitialRadius = 1.2f;

	[DataField(null, false, 1, false, false, null)]
	public float BombFinalRadius = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	public float BombShrinkDuration = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float DelayBetweenBombs = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public bool UsePlayerDensity;

	[DataField(null, false, 1, false, false, null)]
	public int DensityMinPlayers = 6;

	[DataField(null, false, 1, false, false, null)]
	public float DensityWeight = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public int PatternRandomWeight = 100;

	[DataField(null, false, 1, false, false, null)]
	public int PatternClusterWeight;

	[DataField(null, false, 1, false, false, null)]
	public int PatternLineWeight;

	[DataField(null, false, 1, false, false, null)]
	public int PatternRingWeight;

	[DataField(null, false, 1, false, false, null)]
	public int ClusterCountMin = 3;

	[DataField(null, false, 1, false, false, null)]
	public int ClusterCountMax = 6;

	[DataField(null, false, 1, false, false, null)]
	public float ClusterRadius = 6f;

	[DataField(null, false, 1, false, false, null)]
	public int LineCountMin = 4;

	[DataField(null, false, 1, false, false, null)]
	public int LineCountMax = 8;

	[DataField(null, false, 1, false, false, null)]
	public float LineSpacing = 4f;

	[DataField(null, false, 1, false, false, null)]
	public int RingCount = 8;

	[DataField(null, false, 1, false, false, null)]
	public float RingRadiusMin = 8f;

	[DataField(null, false, 1, false, false, null)]
	public float RingRadiusMax = 14f;

	[DataField(null, false, 1, false, false, null)]
	public bool ShakeEnabled = true;

	[DataField(null, false, 1, false, false, null)]
	public int ShakeShakes = 2;

	[DataField(null, false, 1, false, false, null)]
	public int ShakeStrength = 1;

	[DataField(null, false, 1, false, false, null)]
	public float ShakeSpacingSeconds = 0.12f;

	[DataField(null, false, 1, false, false, null)]
	public float ShakeRadiusMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float ProtectedRadiusTiles = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public bool RestrictToSafeZone = true;

	[DataField(null, false, 1, false, false, null)]
	public bool RequireZoneVisible = true;

	[DataField(null, false, 1, false, false, null)]
	public int MinZonePhase;

	[DataField(null, false, 1, false, false, null)]
	public int MaxZonePhase = 6;

	[DataField(null, false, 1, false, false, null)]
	public bool AllowDuringZoneShrink = true;

	[DataField(null, false, 1, false, false, null)]
	public bool UseNextZoneOnShrink = true;

	[DataField(null, false, 1, false, false, null)]
	public float SafeZoneMarginTiles = 6f;

	[DataField(null, false, 1, false, false, null)]
	public float SafeZoneMarginPercent = 0.08f;

	[DataField(null, false, 1, false, false, null)]
	public bool FallbackToMapWhenNoZone = true;

	[DataField(null, false, 1, false, false, null)]
	public bool FallbackToMapWhenZoneActive;

	[DataField(null, false, 1, false, false, null)]
	public float StartDelaySeconds = 45f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RedZoneSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<RedZoneSettings>(this, ref target, hookCtx, false, context))
		{
			int BaseChancePercentTemp = 0;
			if (!serialization.TryCustomCopy<int>(BaseChancePercent, ref BaseChancePercentTemp, hookCtx, false, context))
			{
				BaseChancePercentTemp = BaseChancePercent;
			}
			target.BaseChancePercent = BaseChancePercentTemp;
			int ChanceIncreaseMinTemp = 0;
			if (!serialization.TryCustomCopy<int>(ChanceIncreaseMin, ref ChanceIncreaseMinTemp, hookCtx, false, context))
			{
				ChanceIncreaseMinTemp = ChanceIncreaseMin;
			}
			target.ChanceIncreaseMin = ChanceIncreaseMinTemp;
			int ChanceIncreaseMaxTemp = 0;
			if (!serialization.TryCustomCopy<int>(ChanceIncreaseMax, ref ChanceIncreaseMaxTemp, hookCtx, false, context))
			{
				ChanceIncreaseMaxTemp = ChanceIncreaseMax;
			}
			target.ChanceIncreaseMax = ChanceIncreaseMaxTemp;
			float MinIntervalSecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MinIntervalSeconds, ref MinIntervalSecondsTemp, hookCtx, false, context))
			{
				MinIntervalSecondsTemp = MinIntervalSeconds;
			}
			target.MinIntervalSeconds = MinIntervalSecondsTemp;
			float MaxIntervalSecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxIntervalSeconds, ref MaxIntervalSecondsTemp, hookCtx, false, context))
			{
				MaxIntervalSecondsTemp = MaxIntervalSeconds;
			}
			target.MaxIntervalSeconds = MaxIntervalSecondsTemp;
			float ZoneRadiusMinPercentTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ZoneRadiusMinPercent, ref ZoneRadiusMinPercentTemp, hookCtx, false, context))
			{
				ZoneRadiusMinPercentTemp = ZoneRadiusMinPercent;
			}
			target.ZoneRadiusMinPercent = ZoneRadiusMinPercentTemp;
			float ZoneRadiusMaxPercentTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ZoneRadiusMaxPercent, ref ZoneRadiusMaxPercentTemp, hookCtx, false, context))
			{
				ZoneRadiusMaxPercentTemp = ZoneRadiusMaxPercent;
			}
			target.ZoneRadiusMaxPercent = ZoneRadiusMaxPercentTemp;
			float ZoneDurationMinSecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ZoneDurationMinSeconds, ref ZoneDurationMinSecondsTemp, hookCtx, false, context))
			{
				ZoneDurationMinSecondsTemp = ZoneDurationMinSeconds;
			}
			target.ZoneDurationMinSeconds = ZoneDurationMinSecondsTemp;
			float ZoneDurationMaxSecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ZoneDurationMaxSeconds, ref ZoneDurationMaxSecondsTemp, hookCtx, false, context))
			{
				ZoneDurationMaxSecondsTemp = ZoneDurationMaxSeconds;
			}
			target.ZoneDurationMaxSeconds = ZoneDurationMaxSecondsTemp;
			float BombInitialRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BombInitialRadius, ref BombInitialRadiusTemp, hookCtx, false, context))
			{
				BombInitialRadiusTemp = BombInitialRadius;
			}
			target.BombInitialRadius = BombInitialRadiusTemp;
			float BombFinalRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BombFinalRadius, ref BombFinalRadiusTemp, hookCtx, false, context))
			{
				BombFinalRadiusTemp = BombFinalRadius;
			}
			target.BombFinalRadius = BombFinalRadiusTemp;
			float BombShrinkDurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BombShrinkDuration, ref BombShrinkDurationTemp, hookCtx, false, context))
			{
				BombShrinkDurationTemp = BombShrinkDuration;
			}
			target.BombShrinkDuration = BombShrinkDurationTemp;
			float DelayBetweenBombsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DelayBetweenBombs, ref DelayBetweenBombsTemp, hookCtx, false, context))
			{
				DelayBetweenBombsTemp = DelayBetweenBombs;
			}
			target.DelayBetweenBombs = DelayBetweenBombsTemp;
			bool UsePlayerDensityTemp = false;
			if (!serialization.TryCustomCopy<bool>(UsePlayerDensity, ref UsePlayerDensityTemp, hookCtx, false, context))
			{
				UsePlayerDensityTemp = UsePlayerDensity;
			}
			target.UsePlayerDensity = UsePlayerDensityTemp;
			int DensityMinPlayersTemp = 0;
			if (!serialization.TryCustomCopy<int>(DensityMinPlayers, ref DensityMinPlayersTemp, hookCtx, false, context))
			{
				DensityMinPlayersTemp = DensityMinPlayers;
			}
			target.DensityMinPlayers = DensityMinPlayersTemp;
			float DensityWeightTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DensityWeight, ref DensityWeightTemp, hookCtx, false, context))
			{
				DensityWeightTemp = DensityWeight;
			}
			target.DensityWeight = DensityWeightTemp;
			int PatternRandomWeightTemp = 0;
			if (!serialization.TryCustomCopy<int>(PatternRandomWeight, ref PatternRandomWeightTemp, hookCtx, false, context))
			{
				PatternRandomWeightTemp = PatternRandomWeight;
			}
			target.PatternRandomWeight = PatternRandomWeightTemp;
			int PatternClusterWeightTemp = 0;
			if (!serialization.TryCustomCopy<int>(PatternClusterWeight, ref PatternClusterWeightTemp, hookCtx, false, context))
			{
				PatternClusterWeightTemp = PatternClusterWeight;
			}
			target.PatternClusterWeight = PatternClusterWeightTemp;
			int PatternLineWeightTemp = 0;
			if (!serialization.TryCustomCopy<int>(PatternLineWeight, ref PatternLineWeightTemp, hookCtx, false, context))
			{
				PatternLineWeightTemp = PatternLineWeight;
			}
			target.PatternLineWeight = PatternLineWeightTemp;
			int PatternRingWeightTemp = 0;
			if (!serialization.TryCustomCopy<int>(PatternRingWeight, ref PatternRingWeightTemp, hookCtx, false, context))
			{
				PatternRingWeightTemp = PatternRingWeight;
			}
			target.PatternRingWeight = PatternRingWeightTemp;
			int ClusterCountMinTemp = 0;
			if (!serialization.TryCustomCopy<int>(ClusterCountMin, ref ClusterCountMinTemp, hookCtx, false, context))
			{
				ClusterCountMinTemp = ClusterCountMin;
			}
			target.ClusterCountMin = ClusterCountMinTemp;
			int ClusterCountMaxTemp = 0;
			if (!serialization.TryCustomCopy<int>(ClusterCountMax, ref ClusterCountMaxTemp, hookCtx, false, context))
			{
				ClusterCountMaxTemp = ClusterCountMax;
			}
			target.ClusterCountMax = ClusterCountMaxTemp;
			float ClusterRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ClusterRadius, ref ClusterRadiusTemp, hookCtx, false, context))
			{
				ClusterRadiusTemp = ClusterRadius;
			}
			target.ClusterRadius = ClusterRadiusTemp;
			int LineCountMinTemp = 0;
			if (!serialization.TryCustomCopy<int>(LineCountMin, ref LineCountMinTemp, hookCtx, false, context))
			{
				LineCountMinTemp = LineCountMin;
			}
			target.LineCountMin = LineCountMinTemp;
			int LineCountMaxTemp = 0;
			if (!serialization.TryCustomCopy<int>(LineCountMax, ref LineCountMaxTemp, hookCtx, false, context))
			{
				LineCountMaxTemp = LineCountMax;
			}
			target.LineCountMax = LineCountMaxTemp;
			float LineSpacingTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LineSpacing, ref LineSpacingTemp, hookCtx, false, context))
			{
				LineSpacingTemp = LineSpacing;
			}
			target.LineSpacing = LineSpacingTemp;
			int RingCountTemp = 0;
			if (!serialization.TryCustomCopy<int>(RingCount, ref RingCountTemp, hookCtx, false, context))
			{
				RingCountTemp = RingCount;
			}
			target.RingCount = RingCountTemp;
			float RingRadiusMinTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RingRadiusMin, ref RingRadiusMinTemp, hookCtx, false, context))
			{
				RingRadiusMinTemp = RingRadiusMin;
			}
			target.RingRadiusMin = RingRadiusMinTemp;
			float RingRadiusMaxTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RingRadiusMax, ref RingRadiusMaxTemp, hookCtx, false, context))
			{
				RingRadiusMaxTemp = RingRadiusMax;
			}
			target.RingRadiusMax = RingRadiusMaxTemp;
			bool ShakeEnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(ShakeEnabled, ref ShakeEnabledTemp, hookCtx, false, context))
			{
				ShakeEnabledTemp = ShakeEnabled;
			}
			target.ShakeEnabled = ShakeEnabledTemp;
			int ShakeShakesTemp = 0;
			if (!serialization.TryCustomCopy<int>(ShakeShakes, ref ShakeShakesTemp, hookCtx, false, context))
			{
				ShakeShakesTemp = ShakeShakes;
			}
			target.ShakeShakes = ShakeShakesTemp;
			int ShakeStrengthTemp = 0;
			if (!serialization.TryCustomCopy<int>(ShakeStrength, ref ShakeStrengthTemp, hookCtx, false, context))
			{
				ShakeStrengthTemp = ShakeStrength;
			}
			target.ShakeStrength = ShakeStrengthTemp;
			float ShakeSpacingSecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ShakeSpacingSeconds, ref ShakeSpacingSecondsTemp, hookCtx, false, context))
			{
				ShakeSpacingSecondsTemp = ShakeSpacingSeconds;
			}
			target.ShakeSpacingSeconds = ShakeSpacingSecondsTemp;
			float ShakeRadiusMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ShakeRadiusMultiplier, ref ShakeRadiusMultiplierTemp, hookCtx, false, context))
			{
				ShakeRadiusMultiplierTemp = ShakeRadiusMultiplier;
			}
			target.ShakeRadiusMultiplier = ShakeRadiusMultiplierTemp;
			float ProtectedRadiusTilesTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ProtectedRadiusTiles, ref ProtectedRadiusTilesTemp, hookCtx, false, context))
			{
				ProtectedRadiusTilesTemp = ProtectedRadiusTiles;
			}
			target.ProtectedRadiusTiles = ProtectedRadiusTilesTemp;
			bool RestrictToSafeZoneTemp = false;
			if (!serialization.TryCustomCopy<bool>(RestrictToSafeZone, ref RestrictToSafeZoneTemp, hookCtx, false, context))
			{
				RestrictToSafeZoneTemp = RestrictToSafeZone;
			}
			target.RestrictToSafeZone = RestrictToSafeZoneTemp;
			bool RequireZoneVisibleTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequireZoneVisible, ref RequireZoneVisibleTemp, hookCtx, false, context))
			{
				RequireZoneVisibleTemp = RequireZoneVisible;
			}
			target.RequireZoneVisible = RequireZoneVisibleTemp;
			int MinZonePhaseTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinZonePhase, ref MinZonePhaseTemp, hookCtx, false, context))
			{
				MinZonePhaseTemp = MinZonePhase;
			}
			target.MinZonePhase = MinZonePhaseTemp;
			int MaxZonePhaseTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxZonePhase, ref MaxZonePhaseTemp, hookCtx, false, context))
			{
				MaxZonePhaseTemp = MaxZonePhase;
			}
			target.MaxZonePhase = MaxZonePhaseTemp;
			bool AllowDuringZoneShrinkTemp = false;
			if (!serialization.TryCustomCopy<bool>(AllowDuringZoneShrink, ref AllowDuringZoneShrinkTemp, hookCtx, false, context))
			{
				AllowDuringZoneShrinkTemp = AllowDuringZoneShrink;
			}
			target.AllowDuringZoneShrink = AllowDuringZoneShrinkTemp;
			bool UseNextZoneOnShrinkTemp = false;
			if (!serialization.TryCustomCopy<bool>(UseNextZoneOnShrink, ref UseNextZoneOnShrinkTemp, hookCtx, false, context))
			{
				UseNextZoneOnShrinkTemp = UseNextZoneOnShrink;
			}
			target.UseNextZoneOnShrink = UseNextZoneOnShrinkTemp;
			float SafeZoneMarginTilesTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SafeZoneMarginTiles, ref SafeZoneMarginTilesTemp, hookCtx, false, context))
			{
				SafeZoneMarginTilesTemp = SafeZoneMarginTiles;
			}
			target.SafeZoneMarginTiles = SafeZoneMarginTilesTemp;
			float SafeZoneMarginPercentTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SafeZoneMarginPercent, ref SafeZoneMarginPercentTemp, hookCtx, false, context))
			{
				SafeZoneMarginPercentTemp = SafeZoneMarginPercent;
			}
			target.SafeZoneMarginPercent = SafeZoneMarginPercentTemp;
			bool FallbackToMapWhenNoZoneTemp = false;
			if (!serialization.TryCustomCopy<bool>(FallbackToMapWhenNoZone, ref FallbackToMapWhenNoZoneTemp, hookCtx, false, context))
			{
				FallbackToMapWhenNoZoneTemp = FallbackToMapWhenNoZone;
			}
			target.FallbackToMapWhenNoZone = FallbackToMapWhenNoZoneTemp;
			bool FallbackToMapWhenZoneActiveTemp = false;
			if (!serialization.TryCustomCopy<bool>(FallbackToMapWhenZoneActive, ref FallbackToMapWhenZoneActiveTemp, hookCtx, false, context))
			{
				FallbackToMapWhenZoneActiveTemp = FallbackToMapWhenZoneActive;
			}
			target.FallbackToMapWhenZoneActive = FallbackToMapWhenZoneActiveTemp;
			float StartDelaySecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(StartDelaySeconds, ref StartDelaySecondsTemp, hookCtx, false, context))
			{
				StartDelaySecondsTemp = StartDelaySeconds;
			}
			target.StartDelaySeconds = StartDelaySecondsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RedZoneSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RedZoneSettings cast = (RedZoneSettings)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RedZoneSettings Instantiate()
	{
		return new RedZoneSettings();
	}
}
