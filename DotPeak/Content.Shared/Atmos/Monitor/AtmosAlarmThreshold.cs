// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Monitor.AtmosAlarmThreshold
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.Monitor;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class AtmosAlarmThreshold : 
  ISerializationGenerated<AtmosAlarmThreshold>,
  ISerializationGenerated
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
    get => this._upperBound;
    set
    {
      AlarmThresholdSetting upperWarningBound = this.UpperWarningBound;
      this._upperBound = value;
      this.UpperWarningBound = upperWarningBound;
    }
  }

  public AlarmThresholdSetting LowerBound
  {
    get => this._lowerBound;
    set
    {
      AlarmThresholdSetting lowerWarningBound = this.LowerWarningBound;
      this._lowerBound = value;
      this.LowerWarningBound = lowerWarningBound;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public AlarmThresholdSetting UpperWarningBound
  {
    get => this.CalculateWarningBound(AtmosMonitorThresholdBound.Upper);
    set
    {
      this.UpperWarningPercentage = this.CalculateWarningPercentage(AtmosMonitorThresholdBound.Upper, value);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public AlarmThresholdSetting LowerWarningBound
  {
    get => this.CalculateWarningBound(AtmosMonitorThresholdBound.Lower);
    set
    {
      this.LowerWarningPercentage = this.CalculateWarningPercentage(AtmosMonitorThresholdBound.Lower, value);
    }
  }

  public AtmosAlarmThreshold()
  {
  }

  public AtmosAlarmThreshold(AtmosAlarmThreshold other)
  {
    this.Ignore = other.Ignore;
    this.UpperBound = other.UpperBound;
    this.LowerBound = other.LowerBound;
    this.UpperWarningPercentage = other.UpperWarningPercentage;
    this.LowerWarningPercentage = other.LowerWarningPercentage;
  }

  public AtmosAlarmThreshold(AtmosAlarmThresholdPrototype proto)
  {
    this.Ignore = proto.Ignore;
    this.UpperBound = proto.UpperBound;
    this.LowerBound = proto.LowerBound;
    this.UpperWarningPercentage = proto.UpperWarningPercentage;
    this.LowerWarningPercentage = proto.LowerWarningPercentage;
  }

  public bool CheckThreshold(float value, out AtmosAlarmType state)
  {
    return this.CheckThreshold(value, out state, out AtmosMonitorThresholdBound _);
  }

  public bool CheckThreshold(
    float value,
    out AtmosAlarmType state,
    out AtmosMonitorThresholdBound whichFailed)
  {
    state = AtmosAlarmType.Normal;
    whichFailed = AtmosMonitorThresholdBound.Upper;
    if (this.Ignore)
      return false;
    if (value >= this.UpperBound)
    {
      state = AtmosAlarmType.Danger;
      whichFailed = AtmosMonitorThresholdBound.Upper;
      return true;
    }
    if (value <= this.LowerBound)
    {
      state = AtmosAlarmType.Danger;
      whichFailed = AtmosMonitorThresholdBound.Lower;
      return true;
    }
    if (value >= this.UpperWarningBound)
    {
      state = AtmosAlarmType.Warning;
      whichFailed = AtmosMonitorThresholdBound.Upper;
      return true;
    }
    if (!(value <= this.LowerWarningBound))
      return false;
    state = AtmosAlarmType.Warning;
    whichFailed = AtmosMonitorThresholdBound.Lower;
    return true;
  }

  public AlarmThresholdSetting CalculateWarningBound(AtmosMonitorThresholdBound bound)
  {
    if (bound != AtmosMonitorThresholdBound.Upper)
    {
      if (bound != AtmosMonitorThresholdBound.Lower)
        return new AlarmThresholdSetting();
      return new AlarmThresholdSetting()
      {
        Enabled = this.LowerWarningPercentage.Enabled,
        Value = this.LowerBound.Value * this.LowerWarningPercentage.Value
      };
    }
    return new AlarmThresholdSetting()
    {
      Enabled = this.UpperWarningPercentage.Enabled,
      Value = this.UpperBound.Value * this.UpperWarningPercentage.Value
    };
  }

  public AlarmThresholdSetting CalculateWarningPercentage(
    AtmosMonitorThresholdBound bound,
    AlarmThresholdSetting warningBound)
  {
    switch (bound)
    {
      case AtmosMonitorThresholdBound.Upper:
        return new AlarmThresholdSetting()
        {
          Enabled = this.UpperWarningPercentage.Enabled,
          Value = (double) this.UpperBound.Value == 0.0 ? 0.0f : warningBound.Value / this.UpperBound.Value
        };
      case AtmosMonitorThresholdBound.Lower:
        return new AlarmThresholdSetting()
        {
          Enabled = this.LowerWarningPercentage.Enabled,
          Value = (double) this.LowerBound.Value == 0.0 ? 0.0f : warningBound.Value / this.LowerBound.Value
        };
      default:
        return new AlarmThresholdSetting();
    }
  }

  public void SetEnabled(AtmosMonitorLimitType whichLimit, bool isEnabled)
  {
    switch (whichLimit)
    {
      case AtmosMonitorLimitType.LowerDanger:
        this.LowerBound = this.LowerBound.WithEnabled(isEnabled);
        break;
      case AtmosMonitorLimitType.LowerWarning:
        this.LowerWarningPercentage = this.LowerWarningPercentage.WithEnabled(isEnabled);
        break;
      case AtmosMonitorLimitType.UpperWarning:
        this.UpperWarningPercentage = this.UpperWarningPercentage.WithEnabled(isEnabled);
        break;
      case AtmosMonitorLimitType.UpperDanger:
        this.UpperBound = this.UpperBound.WithEnabled(isEnabled);
        break;
    }
  }

  public void SetLimit(AtmosMonitorLimitType whichLimit, float limit)
  {
    if ((double) limit <= 0.0)
      return;
    switch (whichLimit)
    {
      case AtmosMonitorLimitType.LowerDanger:
        this.LowerBound = this.LowerBound.WithThreshold(limit);
        this.LowerWarningBound = this.LowerWarningBound.WithThreshold(Math.Max(limit, this.LowerWarningBound.Value));
        this.UpperWarningBound = this.UpperWarningBound.WithThreshold(Math.Max(limit, this.UpperWarningBound.Value));
        this.UpperBound = this.UpperBound.WithThreshold(Math.Max(limit, this.UpperBound.Value));
        break;
      case AtmosMonitorLimitType.LowerWarning:
        this.LowerBound = this.LowerBound.WithThreshold(Math.Min(this.LowerBound.Value, limit));
        AlarmThresholdSetting thresholdSetting1 = this.LowerWarningBound;
        this.LowerWarningBound = thresholdSetting1.WithThreshold(limit);
        thresholdSetting1 = this.UpperWarningBound;
        this.UpperWarningBound = thresholdSetting1.WithThreshold(Math.Max(limit, this.UpperWarningBound.Value));
        this.UpperBound = this.UpperBound.WithThreshold(Math.Max(limit, this.UpperBound.Value));
        break;
      case AtmosMonitorLimitType.UpperWarning:
        this.LowerBound = this.LowerBound.WithThreshold(Math.Min(this.LowerBound.Value, limit));
        this.LowerWarningBound = this.LowerWarningBound.WithThreshold(Math.Min(this.LowerWarningBound.Value, limit));
        AlarmThresholdSetting thresholdSetting2 = this.UpperWarningBound;
        this.UpperWarningBound = thresholdSetting2.WithThreshold(limit);
        thresholdSetting2 = this.UpperBound;
        this.UpperBound = thresholdSetting2.WithThreshold(Math.Max(limit, this.UpperBound.Value));
        break;
      case AtmosMonitorLimitType.UpperDanger:
        this.LowerBound = this.LowerBound.WithThreshold(Math.Min(this.LowerBound.Value, limit));
        this.LowerWarningBound = this.LowerWarningBound.WithThreshold(Math.Min(this.LowerWarningBound.Value, limit));
        this.UpperWarningBound = this.UpperWarningBound.WithThreshold(Math.Min(this.UpperWarningBound.Value, limit));
        this.UpperBound = this.UpperBound.WithThreshold(limit);
        break;
    }
  }

  public IEnumerable<AtmosAlarmThresholdChange> GetChanges(AtmosAlarmThreshold previous)
  {
    if (this.LowerBound != previous.LowerBound)
      yield return new AtmosAlarmThresholdChange(AtmosMonitorLimitType.LowerDanger, new AlarmThresholdSetting?(previous.LowerBound), this.LowerBound);
    if (this.LowerWarningBound != previous.LowerWarningBound)
      yield return new AtmosAlarmThresholdChange(AtmosMonitorLimitType.LowerWarning, new AlarmThresholdSetting?(previous.LowerWarningBound), this.LowerWarningBound);
    if (this.UpperBound != previous.UpperBound)
      yield return new AtmosAlarmThresholdChange(AtmosMonitorLimitType.UpperDanger, new AlarmThresholdSetting?(previous.UpperBound), this.UpperBound);
    if (this.UpperWarningBound != previous.UpperWarningBound)
      yield return new AtmosAlarmThresholdChange(AtmosMonitorLimitType.UpperWarning, new AlarmThresholdSetting?(previous.UpperWarningBound), this.UpperWarningBound);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AtmosAlarmThreshold target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<AtmosAlarmThreshold>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Ignore, ref flag, hookCtx, false, context))
      flag = this.Ignore;
    target.Ignore = flag;
    AlarmThresholdSetting thresholdSetting1 = new AlarmThresholdSetting();
    if (!serialization.TryCustomCopy<AlarmThresholdSetting>(this._upperBound, ref thresholdSetting1, hookCtx, false, context))
      serialization.CopyTo<AlarmThresholdSetting>(this._upperBound, ref thresholdSetting1, hookCtx, context, false);
    target._upperBound = thresholdSetting1;
    AlarmThresholdSetting thresholdSetting2 = new AlarmThresholdSetting();
    if (!serialization.TryCustomCopy<AlarmThresholdSetting>(this._lowerBound, ref thresholdSetting2, hookCtx, false, context))
      serialization.CopyTo<AlarmThresholdSetting>(this._lowerBound, ref thresholdSetting2, hookCtx, context, false);
    target._lowerBound = thresholdSetting2;
    AlarmThresholdSetting thresholdSetting3 = new AlarmThresholdSetting();
    if (!serialization.TryCustomCopy<AlarmThresholdSetting>(this.UpperWarningPercentage, ref thresholdSetting3, hookCtx, false, context))
      serialization.CopyTo<AlarmThresholdSetting>(this.UpperWarningPercentage, ref thresholdSetting3, hookCtx, context, false);
    target.UpperWarningPercentage = thresholdSetting3;
    AlarmThresholdSetting thresholdSetting4 = new AlarmThresholdSetting();
    if (!serialization.TryCustomCopy<AlarmThresholdSetting>(this.LowerWarningPercentage, ref thresholdSetting4, hookCtx, false, context))
      serialization.CopyTo<AlarmThresholdSetting>(this.LowerWarningPercentage, ref thresholdSetting4, hookCtx, context, false);
    target.LowerWarningPercentage = thresholdSetting4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AtmosAlarmThreshold target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AtmosAlarmThreshold target1 = (AtmosAlarmThreshold) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public AtmosAlarmThreshold Instantiate() => new AtmosAlarmThreshold();
}
