// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Roles.TotalDepartmentsTimeRequirement
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Log;
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
namespace Content.Shared._RMC14.Roles;

[NetSerializable]
[Serializable]
public sealed class TotalDepartmentsTimeRequirement : 
  JobRequirement,
  ISerializationGenerated<TotalDepartmentsTimeRequirement>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Group;
  [DataField(null, false, 1, true, false, null)]
  public TimeSpan Time;

  public bool TryRequirementsMet(
    IReadOnlyDictionary<string, TimeSpan> playTimes,
    out FormattedMessage? reason,
    IEntityManager entManager,
    IPrototypeManager prototypes)
  {
    reason = (FormattedMessage) null;
    TimeSpan zero = TimeSpan.Zero;
    HashSet<string> stringSet = new HashSet<string>();
    DepartmentGroupComponent component;
    if (!prototypes.Index(this.Group).TryGetComponent<DepartmentGroupComponent>(out component, entManager.ComponentFactory))
    {
      Logger.GetSawmill("job.requirements").Error($"No {"DepartmentGroupComponent"} found on entity {this.Group}");
      return true;
    }
    foreach (ProtoId<DepartmentPrototype> department in component.Departments)
    {
      foreach (ProtoId<JobPrototype> role in prototypes.Index<DepartmentPrototype>(department).Roles)
      {
        string playTimeTracker = prototypes.Index<JobPrototype>(role).PlayTimeTracker;
        stringSet.Add(playTimeTracker);
      }
    }
    foreach (string key in stringSet)
    {
      TimeSpan timeSpan;
      playTimes.TryGetValue(key, out timeSpan);
      zero += timeSpan;
    }
    double a = this.Time.TotalMinutes - zero.TotalMinutes;
    if (!this.Inverted)
    {
      if (a <= 0.0)
        return true;
      reason = FormattedMessage.FromMarkupOrThrow(Loc.GetString("role-timer-total-department-insufficient", ("time", (object) Math.Ceiling(a)), ("roles", (object) Loc.GetString((string) component.Name)), ("rolesColor", (object) ((Color) ref component.Color).ToHex())));
      return false;
    }
    if (a > 0.0)
      return true;
    reason = FormattedMessage.FromMarkupOrThrow(Loc.GetString("role-timer-total-department-too-high", ("time", (object) -a), ("roles", (object) Loc.GetString((string) component.Name)), ("rolesColor", (object) ((Color) ref component.Color).ToHex())));
    return false;
  }

  public override bool Check(
    IEntityManager entManager,
    IPrototypeManager protoManager,
    HumanoidCharacterProfile? profile,
    IReadOnlyDictionary<string, TimeSpan> playTimes,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    return this.TryRequirementsMet(playTimes, out reason, entManager, protoManager);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TotalDepartmentsTimeRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobRequirement target1 = (JobRequirement) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TotalDepartmentsTimeRequirement) target1;
    if (serialization.TryCustomCopy<TotalDepartmentsTimeRequirement>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Group, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Group, hookCtx, context);
    target.Group = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Time, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Time, hookCtx, context);
    target.Time = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TotalDepartmentsTimeRequirement target,
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
    TotalDepartmentsTimeRequirement target1 = (TotalDepartmentsTimeRequirement) target;
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
    TotalDepartmentsTimeRequirement target1 = (TotalDepartmentsTimeRequirement) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TotalDepartmentsTimeRequirement JobRequirement.Instantiate()
  {
    return new TotalDepartmentsTimeRequirement();
  }
}
