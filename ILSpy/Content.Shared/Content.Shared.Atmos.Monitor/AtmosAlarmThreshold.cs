using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Monitor;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class AtmosAlarmThreshold : ISerializationGenerated<AtmosAlarmThreshold>, ISerializationGenerated
{
	[DataField("ignore", false, 1, false, false, null)]
	public bool Ignore;

	[DataField("upperBound", false, 1, false, false, null)]
	private AlarmThresholdSetting _upperBound = AlarmThresholdSetting.Disabled;

	[DataField("lowerBound", false, 1, false, false, null)]
	private AlarmThresholdSetting _lowerBound = AlarmThresholdSetting.Disabled;

	[DataField("upperWarnAround", false, 1, false, false, null)]
	public AlarmThresholdSetting UpperWarningPercentage = AlarmThresholdSetting.Disabled;

	[DataField("lowerWarnAround", false, 1, false, false, null)]
	public AlarmThresholdSetting LowerWarningPercentage = AlarmThresholdSetting.Disabled;

	public AlarmThresholdSetting UpperBound
	{
		get
		{
			return _upperBound;
		}
		set
		{
			AlarmThresholdSetting oldWarning = UpperWarningBound;
			_upperBound = value;
			UpperWarningBound = oldWarning;
		}
	}

	public AlarmThresholdSetting LowerBound
	{
		get
		{
			return _lowerBound;
		}
		set
		{
			AlarmThresholdSetting oldWarning = LowerWarningBound;
			_lowerBound = value;
			LowerWarningBound = oldWarning;
		}
	}

	[ViewVariables]
	public AlarmThresholdSetting UpperWarningBound
	{
		get
		{
			return CalculateWarningBound(AtmosMonitorThresholdBound.Upper);
		}
		set
		{
			UpperWarningPercentage = CalculateWarningPercentage(AtmosMonitorThresholdBound.Upper, value);
		}
	}

	[ViewVariables]
	public AlarmThresholdSetting LowerWarningBound
	{
		get
		{
			return CalculateWarningBound(AtmosMonitorThresholdBound.Lower);
		}
		set
		{
			LowerWarningPercentage = CalculateWarningPercentage(AtmosMonitorThresholdBound.Lower, value);
		}
	}

	public AtmosAlarmThreshold()
	{
	}

	public AtmosAlarmThreshold(AtmosAlarmThreshold other)
	{
		Ignore = other.Ignore;
		UpperBound = other.UpperBound;
		LowerBound = other.LowerBound;
		UpperWarningPercentage = other.UpperWarningPercentage;
		LowerWarningPercentage = other.LowerWarningPercentage;
	}

	public AtmosAlarmThreshold(AtmosAlarmThresholdPrototype proto)
	{
		Ignore = proto.Ignore;
		UpperBound = proto.UpperBound;
		LowerBound = proto.LowerBound;
		UpperWarningPercentage = proto.UpperWarningPercentage;
		LowerWarningPercentage = proto.LowerWarningPercentage;
	}

	public bool CheckThreshold(float value, out AtmosAlarmType state)
	{
		AtmosMonitorThresholdBound whichFailed;
		return CheckThreshold(value, out state, out whichFailed);
	}

	public bool CheckThreshold(float value, out AtmosAlarmType state, out AtmosMonitorThresholdBound whichFailed)
	{
		state = AtmosAlarmType.Normal;
		whichFailed = AtmosMonitorThresholdBound.Upper;
		if (Ignore)
		{
			return false;
		}
		if (value >= UpperBound)
		{
			state = AtmosAlarmType.Danger;
			whichFailed = AtmosMonitorThresholdBound.Upper;
			return true;
		}
		if (value <= LowerBound)
		{
			state = AtmosAlarmType.Danger;
			whichFailed = AtmosMonitorThresholdBound.Lower;
			return true;
		}
		if (value >= UpperWarningBound)
		{
			state = AtmosAlarmType.Warning;
			whichFailed = AtmosMonitorThresholdBound.Upper;
			return true;
		}
		if (value <= LowerWarningBound)
		{
			state = AtmosAlarmType.Warning;
			whichFailed = AtmosMonitorThresholdBound.Lower;
			return true;
		}
		return false;
	}

	public AlarmThresholdSetting CalculateWarningBound(AtmosMonitorThresholdBound bound)
	{
		return bound switch
		{
			AtmosMonitorThresholdBound.Upper => new AlarmThresholdSetting
			{
				Enabled = UpperWarningPercentage.Enabled,
				Value = UpperBound.Value * UpperWarningPercentage.Value
			}, 
			AtmosMonitorThresholdBound.Lower => new AlarmThresholdSetting
			{
				Enabled = LowerWarningPercentage.Enabled,
				Value = LowerBound.Value * LowerWarningPercentage.Value
			}, 
			_ => new AlarmThresholdSetting(), 
		};
	}

	public AlarmThresholdSetting CalculateWarningPercentage(AtmosMonitorThresholdBound bound, AlarmThresholdSetting warningBound)
	{
		return bound switch
		{
			AtmosMonitorThresholdBound.Upper => new AlarmThresholdSetting
			{
				Enabled = UpperWarningPercentage.Enabled,
				Value = ((UpperBound.Value == 0f) ? 0f : (warningBound.Value / UpperBound.Value))
			}, 
			AtmosMonitorThresholdBound.Lower => new AlarmThresholdSetting
			{
				Enabled = LowerWarningPercentage.Enabled,
				Value = ((LowerBound.Value == 0f) ? 0f : (warningBound.Value / LowerBound.Value))
			}, 
			_ => new AlarmThresholdSetting(), 
		};
	}

	public void SetEnabled(AtmosMonitorLimitType whichLimit, bool isEnabled)
	{
		switch (whichLimit)
		{
		case AtmosMonitorLimitType.LowerDanger:
			LowerBound = LowerBound.WithEnabled(isEnabled);
			break;
		case AtmosMonitorLimitType.LowerWarning:
			LowerWarningPercentage = LowerWarningPercentage.WithEnabled(isEnabled);
			break;
		case AtmosMonitorLimitType.UpperWarning:
			UpperWarningPercentage = UpperWarningPercentage.WithEnabled(isEnabled);
			break;
		case AtmosMonitorLimitType.UpperDanger:
			UpperBound = UpperBound.WithEnabled(isEnabled);
			break;
		}
	}

	public void SetLimit(AtmosMonitorLimitType whichLimit, float limit)
	{
		if (!(limit <= 0f))
		{
			switch (whichLimit)
			{
			case AtmosMonitorLimitType.LowerDanger:
				LowerBound = LowerBound.WithThreshold(limit);
				LowerWarningBound = LowerWarningBound.WithThreshold(Math.Max(limit, LowerWarningBound.Value));
				UpperWarningBound = UpperWarningBound.WithThreshold(Math.Max(limit, UpperWarningBound.Value));
				UpperBound = UpperBound.WithThreshold(Math.Max(limit, UpperBound.Value));
				break;
			case AtmosMonitorLimitType.LowerWarning:
				LowerBound = LowerBound.WithThreshold(Math.Min(LowerBound.Value, limit));
				LowerWarningBound = LowerWarningBound.WithThreshold(limit);
				UpperWarningBound = UpperWarningBound.WithThreshold(Math.Max(limit, UpperWarningBound.Value));
				UpperBound = UpperBound.WithThreshold(Math.Max(limit, UpperBound.Value));
				break;
			case AtmosMonitorLimitType.UpperWarning:
				LowerBound = LowerBound.WithThreshold(Math.Min(LowerBound.Value, limit));
				LowerWarningBound = LowerWarningBound.WithThreshold(Math.Min(LowerWarningBound.Value, limit));
				UpperWarningBound = UpperWarningBound.WithThreshold(limit);
				UpperBound = UpperBound.WithThreshold(Math.Max(limit, UpperBound.Value));
				break;
			case AtmosMonitorLimitType.UpperDanger:
				LowerBound = LowerBound.WithThreshold(Math.Min(LowerBound.Value, limit));
				LowerWarningBound = LowerWarningBound.WithThreshold(Math.Min(LowerWarningBound.Value, limit));
				UpperWarningBound = UpperWarningBound.WithThreshold(Math.Min(UpperWarningBound.Value, limit));
				UpperBound = UpperBound.WithThreshold(limit);
				break;
			}
		}
	}

	public IEnumerable<AtmosAlarmThresholdChange> GetChanges(AtmosAlarmThreshold previous)
	{
		if (LowerBound != previous.LowerBound)
		{
			yield return new AtmosAlarmThresholdChange(AtmosMonitorLimitType.LowerDanger, previous.LowerBound, LowerBound);
		}
		if (LowerWarningBound != previous.LowerWarningBound)
		{
			yield return new AtmosAlarmThresholdChange(AtmosMonitorLimitType.LowerWarning, previous.LowerWarningBound, LowerWarningBound);
		}
		if (UpperBound != previous.UpperBound)
		{
			yield return new AtmosAlarmThresholdChange(AtmosMonitorLimitType.UpperDanger, previous.UpperBound, UpperBound);
		}
		if (UpperWarningBound != previous.UpperWarningBound)
		{
			yield return new AtmosAlarmThresholdChange(AtmosMonitorLimitType.UpperWarning, previous.UpperWarningBound, UpperWarningBound);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AtmosAlarmThreshold target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<AtmosAlarmThreshold>(this, ref target, hookCtx, false, context))
		{
			bool IgnoreTemp = false;
			if (!serialization.TryCustomCopy<bool>(Ignore, ref IgnoreTemp, hookCtx, false, context))
			{
				IgnoreTemp = Ignore;
			}
			target.Ignore = IgnoreTemp;
			AlarmThresholdSetting _upperBoundTemp = default(AlarmThresholdSetting);
			if (!serialization.TryCustomCopy<AlarmThresholdSetting>(_upperBound, ref _upperBoundTemp, hookCtx, false, context))
			{
				serialization.CopyTo<AlarmThresholdSetting>(_upperBound, ref _upperBoundTemp, hookCtx, context, false);
			}
			target._upperBound = _upperBoundTemp;
			AlarmThresholdSetting _lowerBoundTemp = default(AlarmThresholdSetting);
			if (!serialization.TryCustomCopy<AlarmThresholdSetting>(_lowerBound, ref _lowerBoundTemp, hookCtx, false, context))
			{
				serialization.CopyTo<AlarmThresholdSetting>(_lowerBound, ref _lowerBoundTemp, hookCtx, context, false);
			}
			target._lowerBound = _lowerBoundTemp;
			AlarmThresholdSetting UpperWarningPercentageTemp = default(AlarmThresholdSetting);
			if (!serialization.TryCustomCopy<AlarmThresholdSetting>(UpperWarningPercentage, ref UpperWarningPercentageTemp, hookCtx, false, context))
			{
				serialization.CopyTo<AlarmThresholdSetting>(UpperWarningPercentage, ref UpperWarningPercentageTemp, hookCtx, context, false);
			}
			target.UpperWarningPercentage = UpperWarningPercentageTemp;
			AlarmThresholdSetting LowerWarningPercentageTemp = default(AlarmThresholdSetting);
			if (!serialization.TryCustomCopy<AlarmThresholdSetting>(LowerWarningPercentage, ref LowerWarningPercentageTemp, hookCtx, false, context))
			{
				serialization.CopyTo<AlarmThresholdSetting>(LowerWarningPercentage, ref LowerWarningPercentageTemp, hookCtx, context, false);
			}
			target.LowerWarningPercentage = LowerWarningPercentageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AtmosAlarmThreshold target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosAlarmThreshold cast = (AtmosAlarmThreshold)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public AtmosAlarmThreshold Instantiate()
	{
		return new AtmosAlarmThreshold();
	}
}
