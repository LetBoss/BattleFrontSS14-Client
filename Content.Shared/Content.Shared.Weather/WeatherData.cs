using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weather;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class WeatherData : ISerializationGenerated<WeatherData>, ISerializationGenerated
{
	[NonSerialized]
	public EntityUid? Stream;

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan StartTime = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan? EndTime;

	[DataField(null, false, 1, false, false, null)]
	public WeatherState State;

	[ViewVariables]
	public TimeSpan Duration
	{
		get
		{
			if (EndTime.HasValue)
			{
				return EndTime.Value - StartTime;
			}
			return TimeSpan.MaxValue;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WeatherData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<WeatherData>(this, ref target, hookCtx, false, context))
		{
			TimeSpan StartTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(StartTime, ref StartTimeTemp, hookCtx, false, context))
			{
				StartTimeTemp = serialization.CreateCopy<TimeSpan>(StartTime, hookCtx, context, false);
			}
			target.StartTime = StartTimeTemp;
			TimeSpan? EndTimeTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(EndTime, ref EndTimeTemp, hookCtx, false, context))
			{
				EndTimeTemp = serialization.CreateCopy<TimeSpan?>(EndTime, hookCtx, context, false);
			}
			target.EndTime = EndTimeTemp;
			WeatherState StateTemp = WeatherState.Invalid;
			if (!serialization.TryCustomCopy<WeatherState>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target.State = StateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WeatherData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WeatherData cast = (WeatherData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public WeatherData Instantiate()
	{
		return new WeatherData();
	}
}
