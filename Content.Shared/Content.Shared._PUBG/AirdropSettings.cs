using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG;

[DataDefinition]
public sealed class AirdropSettings : ISerializationGenerated<AirdropSettings>, ISerializationGenerated
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
	public bool UseNextZoneIfDropAfterStateChange = true;

	[DataField(null, false, 1, false, false, null)]
	public float SafeZoneMarginTiles = 6f;

	[DataField(null, false, 1, false, false, null)]
	public float SafeZoneMarginPercent = 0.08f;

	[DataField(null, false, 1, false, false, null)]
	public bool FallbackToMapWhenNoZone = true;

	[DataField(null, false, 1, false, false, null)]
	public bool FallbackToMapWhenZoneActive;

	[DataField(null, false, 1, false, false, null)]
	public float DropDelayMinSeconds = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float DropDelayMaxSeconds = 50f;

	[DataField(null, false, 1, false, false, null)]
	public float StartDelaySeconds = 30f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AirdropSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<AirdropSettings>(this, ref target, hookCtx, false, context))
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
			bool UseNextZoneIfDropAfterStateChangeTemp = false;
			if (!serialization.TryCustomCopy<bool>(UseNextZoneIfDropAfterStateChange, ref UseNextZoneIfDropAfterStateChangeTemp, hookCtx, false, context))
			{
				UseNextZoneIfDropAfterStateChangeTemp = UseNextZoneIfDropAfterStateChange;
			}
			target.UseNextZoneIfDropAfterStateChange = UseNextZoneIfDropAfterStateChangeTemp;
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
			float DropDelayMinSecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DropDelayMinSeconds, ref DropDelayMinSecondsTemp, hookCtx, false, context))
			{
				DropDelayMinSecondsTemp = DropDelayMinSeconds;
			}
			target.DropDelayMinSeconds = DropDelayMinSecondsTemp;
			float DropDelayMaxSecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DropDelayMaxSeconds, ref DropDelayMaxSecondsTemp, hookCtx, false, context))
			{
				DropDelayMaxSecondsTemp = DropDelayMaxSeconds;
			}
			target.DropDelayMaxSeconds = DropDelayMaxSecondsTemp;
			float StartDelaySecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(StartDelaySeconds, ref StartDelaySecondsTemp, hookCtx, false, context))
			{
				StartDelaySecondsTemp = StartDelaySeconds;
			}
			target.StartDelaySeconds = StartDelaySecondsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AirdropSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AirdropSettings cast = (AirdropSettings)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public AirdropSettings Instantiate()
	{
		return new AirdropSettings();
	}
}
