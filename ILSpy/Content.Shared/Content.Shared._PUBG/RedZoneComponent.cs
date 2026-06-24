using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG;

[RegisterComponent]
[NetworkedComponent]
public sealed class RedZoneComponent : Component, ISerializationGenerated<RedZoneComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Active;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 OverallCenter;

	[DataField(null, false, 1, false, false, null)]
	public float OverallRadius;

	[DataField(null, false, 1, false, false, null)]
	public MapId MapId;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 CurrentBombCenter;

	[DataField(null, false, 1, false, false, null)]
	public float CurrentBombRadius;

	[DataField(null, false, 1, false, false, null)]
	public float BombInitialRadius = 1.2f;

	[DataField(null, false, 1, false, false, null)]
	public float BombFinalRadius = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	public float BombShrinkDuration = 1f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? ZoneEndTime;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? NextBombTime;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? CurrentBombStartTime;

	[DataField(null, false, 1, false, false, null)]
	public float DelayBetweenBombs = 1f;

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

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RedZoneComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RedZoneComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RedZoneComponent>(this, ref target, hookCtx, false, context))
		{
			bool ActiveTemp = false;
			if (!serialization.TryCustomCopy<bool>(Active, ref ActiveTemp, hookCtx, false, context))
			{
				ActiveTemp = Active;
			}
			target.Active = ActiveTemp;
			Vector2 OverallCenterTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(OverallCenter, ref OverallCenterTemp, hookCtx, false, context))
			{
				OverallCenterTemp = serialization.CreateCopy<Vector2>(OverallCenter, hookCtx, context, false);
			}
			target.OverallCenter = OverallCenterTemp;
			float OverallRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(OverallRadius, ref OverallRadiusTemp, hookCtx, false, context))
			{
				OverallRadiusTemp = OverallRadius;
			}
			target.OverallRadius = OverallRadiusTemp;
			MapId MapIdTemp = default(MapId);
			if (!serialization.TryCustomCopy<MapId>(MapId, ref MapIdTemp, hookCtx, false, context))
			{
				MapIdTemp = serialization.CreateCopy<MapId>(MapId, hookCtx, context, false);
			}
			target.MapId = MapIdTemp;
			Vector2 CurrentBombCenterTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(CurrentBombCenter, ref CurrentBombCenterTemp, hookCtx, false, context))
			{
				CurrentBombCenterTemp = serialization.CreateCopy<Vector2>(CurrentBombCenter, hookCtx, context, false);
			}
			target.CurrentBombCenter = CurrentBombCenterTemp;
			float CurrentBombRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CurrentBombRadius, ref CurrentBombRadiusTemp, hookCtx, false, context))
			{
				CurrentBombRadiusTemp = CurrentBombRadius;
			}
			target.CurrentBombRadius = CurrentBombRadiusTemp;
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
			TimeSpan? ZoneEndTimeTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(ZoneEndTime, ref ZoneEndTimeTemp, hookCtx, false, context))
			{
				ZoneEndTimeTemp = serialization.CreateCopy<TimeSpan?>(ZoneEndTime, hookCtx, context, false);
			}
			target.ZoneEndTime = ZoneEndTimeTemp;
			TimeSpan? NextBombTimeTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(NextBombTime, ref NextBombTimeTemp, hookCtx, false, context))
			{
				NextBombTimeTemp = serialization.CreateCopy<TimeSpan?>(NextBombTime, hookCtx, context, false);
			}
			target.NextBombTime = NextBombTimeTemp;
			TimeSpan? CurrentBombStartTimeTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(CurrentBombStartTime, ref CurrentBombStartTimeTemp, hookCtx, false, context))
			{
				CurrentBombStartTimeTemp = serialization.CreateCopy<TimeSpan?>(CurrentBombStartTime, hookCtx, context, false);
			}
			target.CurrentBombStartTime = CurrentBombStartTimeTemp;
			float DelayBetweenBombsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DelayBetweenBombs, ref DelayBetweenBombsTemp, hookCtx, false, context))
			{
				DelayBetweenBombsTemp = DelayBetweenBombs;
			}
			target.DelayBetweenBombs = DelayBetweenBombsTemp;
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
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RedZoneComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RedZoneComponent cast = (RedZoneComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RedZoneComponent cast = (RedZoneComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RedZoneComponent def = (RedZoneComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RedZoneComponent Instantiate()
	{
		return new RedZoneComponent();
	}
}
