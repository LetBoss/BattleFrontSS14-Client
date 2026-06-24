// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Monitor.AlarmThresholdSetting
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Atmos.Monitor;

[DataDefinition]
[Serializable]
public readonly struct AlarmThresholdSetting : 
  IEquatable<AlarmThresholdSetting>,
  ISerializationGenerated<AlarmThresholdSetting>,
  ISerializationGenerated
{
  public static AlarmThresholdSetting Disabled = new AlarmThresholdSetting()
  {
    Enabled = false,
    Value = 0.0f
  };

  [DataField("enabled", false, 1, false, false, null)]
  public bool Enabled { get; init; }

  [DataField("threshold", false, 1, false, false, null)]
  public float Value { get; init; }

  public AlarmThresholdSetting()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CEnabled\u003Ek__BackingField = true;
    // ISSUE: reference to a compiler-generated field
    this.\u003CValue\u003Ek__BackingField = 1f;
  }

  public static bool operator <=(float a, AlarmThresholdSetting b)
  {
    return b.Enabled && (double) a <= (double) b.Value;
  }

  public static bool operator >=(float a, AlarmThresholdSetting b)
  {
    return b.Enabled && (double) a >= (double) b.Value;
  }

  public AlarmThresholdSetting WithThreshold(float threshold)
  {
    return this with { Value = threshold };
  }

  public AlarmThresholdSetting WithEnabled(bool enabled)
  {
    return this with { Enabled = enabled };
  }

  public bool Equals(AlarmThresholdSetting other)
  {
    return this.Enabled == other.Enabled && (double) this.Value == (double) other.Value;
  }

  public override bool Equals(object? obj)
  {
    return obj is AlarmThresholdSetting other && this.Equals(other);
  }

  public static bool operator ==(AlarmThresholdSetting lhs, AlarmThresholdSetting rhs)
  {
    return lhs.Equals(rhs);
  }

  public static bool operator !=(AlarmThresholdSetting lhs, AlarmThresholdSetting rhs)
  {
    return !lhs.Equals(rhs);
  }

  public override int GetHashCode() => HashCode.Combine<bool, float>(this.Enabled, this.Value);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AlarmThresholdSetting target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<AlarmThresholdSetting>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag, hookCtx, false, context))
      flag = this.Enabled;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Value, ref num, hookCtx, false, context))
      num = this.Value;
    target = target with { Enabled = flag, Value = num };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AlarmThresholdSetting target,
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
    AlarmThresholdSetting target1 = (AlarmThresholdSetting) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public AlarmThresholdSetting Instantiate() => new AlarmThresholdSetting();
}
