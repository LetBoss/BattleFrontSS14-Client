using System;
using Content.Shared.Explosion;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.OrbitalCannon;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class OrbitalCannonExplosion : ISerializationGenerated<OrbitalCannonExplosion>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ExplosionPrototype>? Type;

	[DataField(null, false, 1, false, false, null)]
	public float Total;

	[DataField(null, false, 1, false, false, null)]
	public float Slope;

	[DataField(null, false, 1, false, false, null)]
	public float Max;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Delay;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? Fire;

	[DataField(null, false, 1, false, false, null)]
	public int FireRange = 18;

	[DataField(null, false, 1, false, false, null)]
	public int Intensity = 80;

	[DataField(null, false, 1, false, false, null)]
	public int Duration = 70;

	[DataField(null, false, 1, false, false, null)]
	public int Times = 1;

	[DataField(null, false, 1, false, false, null)]
	public int TimesPer = 1;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan DelayPer;

	[DataField(null, false, 1, false, false, null)]
	public int Spread;

	[DataField(null, false, 1, false, false, null)]
	public bool CheckProtectionPer;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? ExplosionEffect;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OrbitalCannonExplosion target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<OrbitalCannonExplosion>(this, ref target, hookCtx, false, context))
		{
			ProtoId<ExplosionPrototype>? TypeTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<ExplosionPrototype>?>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = serialization.CreateCopy<ProtoId<ExplosionPrototype>?>(Type, hookCtx, context, false);
			}
			target.Type = TypeTemp;
			float TotalTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Total, ref TotalTemp, hookCtx, false, context))
			{
				TotalTemp = Total;
			}
			target.Total = TotalTemp;
			float SlopeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Slope, ref SlopeTemp, hookCtx, false, context))
			{
				SlopeTemp = Slope;
			}
			target.Slope = SlopeTemp;
			float MaxTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Max, ref MaxTemp, hookCtx, false, context))
			{
				MaxTemp = Max;
			}
			target.Max = MaxTemp;
			TimeSpan DelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = serialization.CreateCopy<TimeSpan>(Delay, hookCtx, context, false);
			}
			target.Delay = DelayTemp;
			EntProtoId? FireTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(Fire, ref FireTemp, hookCtx, false, context))
			{
				FireTemp = serialization.CreateCopy<EntProtoId?>(Fire, hookCtx, context, false);
			}
			target.Fire = FireTemp;
			int FireRangeTemp = 0;
			if (!serialization.TryCustomCopy<int>(FireRange, ref FireRangeTemp, hookCtx, false, context))
			{
				FireRangeTemp = FireRange;
			}
			target.FireRange = FireRangeTemp;
			int IntensityTemp = 0;
			if (!serialization.TryCustomCopy<int>(Intensity, ref IntensityTemp, hookCtx, false, context))
			{
				IntensityTemp = Intensity;
			}
			target.Intensity = IntensityTemp;
			int DurationTemp = 0;
			if (!serialization.TryCustomCopy<int>(Duration, ref DurationTemp, hookCtx, false, context))
			{
				DurationTemp = Duration;
			}
			target.Duration = DurationTemp;
			int TimesTemp = 0;
			if (!serialization.TryCustomCopy<int>(Times, ref TimesTemp, hookCtx, false, context))
			{
				TimesTemp = Times;
			}
			target.Times = TimesTemp;
			int TimesPerTemp = 0;
			if (!serialization.TryCustomCopy<int>(TimesPer, ref TimesPerTemp, hookCtx, false, context))
			{
				TimesPerTemp = TimesPer;
			}
			target.TimesPer = TimesPerTemp;
			TimeSpan DelayPerTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(DelayPer, ref DelayPerTemp, hookCtx, false, context))
			{
				DelayPerTemp = serialization.CreateCopy<TimeSpan>(DelayPer, hookCtx, context, false);
			}
			target.DelayPer = DelayPerTemp;
			int SpreadTemp = 0;
			if (!serialization.TryCustomCopy<int>(Spread, ref SpreadTemp, hookCtx, false, context))
			{
				SpreadTemp = Spread;
			}
			target.Spread = SpreadTemp;
			bool CheckProtectionPerTemp = false;
			if (!serialization.TryCustomCopy<bool>(CheckProtectionPer, ref CheckProtectionPerTemp, hookCtx, false, context))
			{
				CheckProtectionPerTemp = CheckProtectionPer;
			}
			target.CheckProtectionPer = CheckProtectionPerTemp;
			EntProtoId? ExplosionEffectTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(ExplosionEffect, ref ExplosionEffectTemp, hookCtx, false, context))
			{
				ExplosionEffectTemp = serialization.CreateCopy<EntProtoId?>(ExplosionEffect, hookCtx, context, false);
			}
			target.ExplosionEffect = ExplosionEffectTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OrbitalCannonExplosion target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OrbitalCannonExplosion cast = (OrbitalCannonExplosion)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public OrbitalCannonExplosion Instantiate()
	{
		return new OrbitalCannonExplosion();
	}
}
