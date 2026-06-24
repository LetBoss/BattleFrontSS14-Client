using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Timing;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class UseDelayInfo : ISerializationGenerated<UseDelayInfo>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Length { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan StartTime { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan EndTime { get; set; }

	public UseDelayInfo(TimeSpan length, TimeSpan startTime = default(TimeSpan), TimeSpan endTime = default(TimeSpan))
	{
		Length = length;
		StartTime = startTime;
		EndTime = endTime;
	}

	public UseDelayInfo()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref UseDelayInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<UseDelayInfo>(this, ref target, hookCtx, false, context))
		{
			TimeSpan LengthTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Length, ref LengthTemp, hookCtx, false, context))
			{
				LengthTemp = serialization.CreateCopy<TimeSpan>(Length, hookCtx, context, false);
			}
			target.Length = LengthTemp;
			TimeSpan StartTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(StartTime, ref StartTimeTemp, hookCtx, false, context))
			{
				StartTimeTemp = serialization.CreateCopy<TimeSpan>(StartTime, hookCtx, context, false);
			}
			target.StartTime = StartTimeTemp;
			TimeSpan EndTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(EndTime, ref EndTimeTemp, hookCtx, false, context))
			{
				EndTimeTemp = serialization.CreateCopy<TimeSpan>(EndTime, hookCtx, context, false);
			}
			target.EndTime = EndTimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref UseDelayInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UseDelayInfo cast = (UseDelayInfo)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public UseDelayInfo Instantiate()
	{
		return new UseDelayInfo();
	}
}
