using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Atmos.Monitor;

[Serializable]
[DataDefinition]
public readonly struct AlarmThresholdSetting : IEquatable<AlarmThresholdSetting>, ISerializationGenerated<AlarmThresholdSetting>, ISerializationGenerated
{
	public static AlarmThresholdSetting Disabled = new AlarmThresholdSetting
	{
		Enabled = false,
		Value = 0f
	};

	[DataField("enabled", false, 1, false, false, null)]
	public bool Enabled { get; init; } = true;

	[DataField("threshold", false, 1, false, false, null)]
	public float Value { get; init; } = 1f;

	public AlarmThresholdSetting()
	{
	}

	public static bool operator <=(float a, AlarmThresholdSetting b)
	{
		if (b.Enabled)
		{
			return a <= b.Value;
		}
		return false;
	}

	public static bool operator >=(float a, AlarmThresholdSetting b)
	{
		if (b.Enabled)
		{
			return a >= b.Value;
		}
		return false;
	}

	public AlarmThresholdSetting WithThreshold(float threshold)
	{
		AlarmThresholdSetting result = this;
		result.Value = threshold;
		return result;
	}

	public AlarmThresholdSetting WithEnabled(bool enabled)
	{
		AlarmThresholdSetting result = this;
		result.Enabled = enabled;
		return result;
	}

	public bool Equals(AlarmThresholdSetting other)
	{
		if (Enabled != other.Enabled)
		{
			return false;
		}
		if (Value != other.Value)
		{
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is AlarmThresholdSetting ats)
		{
			return Equals(ats);
		}
		return false;
	}

	public static bool operator ==(AlarmThresholdSetting lhs, AlarmThresholdSetting rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(AlarmThresholdSetting lhs, AlarmThresholdSetting rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Enabled, Value);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AlarmThresholdSetting target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<AlarmThresholdSetting>(this, ref target, hookCtx, false, context))
		{
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			float ValueTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Value, ref ValueTemp, hookCtx, false, context))
			{
				ValueTemp = Value;
			}
			AlarmThresholdSetting alarmThresholdSetting = target;
			alarmThresholdSetting.Enabled = EnabledTemp;
			alarmThresholdSetting.Value = ValueTemp;
			target = alarmThresholdSetting;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AlarmThresholdSetting target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AlarmThresholdSetting cast = (AlarmThresholdSetting)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public AlarmThresholdSetting Instantiate()
	{
		return new AlarmThresholdSetting();
	}
}
