// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.OverallPlaytimeRequirement
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Localizations;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Roles;

[NetSerializable]
[Serializable]
public sealed class OverallPlaytimeRequirement : 
  JobRequirement,
  ISerializationGenerated<OverallPlaytimeRequirement>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public TimeSpan Time;

  public override bool Check(
    IEntityManager entManager,
    IPrototypeManager protoManager,
    HumanoidCharacterProfile? profile,
    IReadOnlyDictionary<string, TimeSpan> playTimes,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    reason = new FormattedMessage();
    TimeSpan valueOrDefault = playTimes.GetValueOrDefault<string, TimeSpan>((string) PlayTimeTrackingShared.TrackerOverall);
    TimeSpan time = this.Time - valueOrDefault;
    double totalMinutes = time.TotalMinutes;
    string str = ContentLocalizationManager.FormatPlaytime(time);
    if (!this.Inverted)
    {
      if (totalMinutes <= 0.0 || valueOrDefault >= this.Time)
        return true;
      reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-overall-insufficient", ("time", (object) str)));
      return false;
    }
    if (totalMinutes > 0.0 && !(valueOrDefault >= this.Time))
      return true;
    reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-overall-too-high", ("time", (object) str)));
    return false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OverallPlaytimeRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobRequirement target1 = (JobRequirement) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OverallPlaytimeRequirement) target1;
    if (serialization.TryCustomCopy<OverallPlaytimeRequirement>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Time, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Time, hookCtx, context);
    target.Time = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OverallPlaytimeRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref JobRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OverallPlaytimeRequirement target1 = (OverallPlaytimeRequirement) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (JobRequirement) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OverallPlaytimeRequirement target1 = (OverallPlaytimeRequirement) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual OverallPlaytimeRequirement JobRequirement.Instantiate()
  {
    return new OverallPlaytimeRequirement();
  }
}
