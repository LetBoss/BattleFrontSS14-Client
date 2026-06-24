// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.DepartmentTimeRequirement
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Localizations;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
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
public sealed class DepartmentTimeRequirement : 
  JobRequirement,
  ISerializationGenerated<DepartmentTimeRequirement>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<DepartmentPrototype> Department;
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
    TimeSpan zero = TimeSpan.Zero;
    DepartmentPrototype departmentPrototype = protoManager.Index<DepartmentPrototype>(this.Department);
    foreach (ProtoId<JobPrototype> role in departmentPrototype.Roles)
    {
      string playTimeTracker = protoManager.Index<JobPrototype>(role).PlayTimeTracker;
      TimeSpan timeSpan;
      playTimes.TryGetValue(playTimeTracker, out timeSpan);
      zero += timeSpan;
    }
    TimeSpan time = this.Time - zero;
    double totalMinutes = time.TotalMinutes;
    string str = ContentLocalizationManager.FormatPlaytime(time);
    string messageId = "role-timer-department-unknown";
    DepartmentPrototype prototype;
    if (protoManager.TryIndex<DepartmentPrototype>(this.Department, out prototype))
      messageId = (string) prototype.Name;
    if (!this.Inverted)
    {
      if (totalMinutes <= 0.0)
        return true;
      reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-department-insufficient", ("time", (object) str), ("department", (object) Loc.GetString(messageId)), ("departmentColor", (object) ((Color) ref departmentPrototype.Color).ToHex())));
      return false;
    }
    if (totalMinutes > 0.0)
      return true;
    reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-department-too-high", ("time", (object) str), ("department", (object) Loc.GetString(messageId)), ("departmentColor", (object) ((Color) ref departmentPrototype.Color).ToHex())));
    return false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DepartmentTimeRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobRequirement target1 = (JobRequirement) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DepartmentTimeRequirement) target1;
    if (serialization.TryCustomCopy<DepartmentTimeRequirement>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<DepartmentPrototype> target2 = new ProtoId<DepartmentPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DepartmentPrototype>>(this.Department, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<DepartmentPrototype>>(this.Department, hookCtx, context);
    target.Department = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Time, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Time, hookCtx, context);
    target.Time = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DepartmentTimeRequirement target,
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
    DepartmentTimeRequirement target1 = (DepartmentTimeRequirement) target;
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
    DepartmentTimeRequirement target1 = (DepartmentTimeRequirement) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DepartmentTimeRequirement JobRequirement.Instantiate() => new DepartmentTimeRequirement();
}
