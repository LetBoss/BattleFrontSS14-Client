// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.RoleTimeRequirement
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Localizations;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Preferences;
using Content.Shared.Roles.Jobs;
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
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Roles;

[NetSerializable]
[Serializable]
public sealed class RoleTimeRequirement : 
  JobRequirement,
  ISerializationGenerated<RoleTimeRequirement>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<PlayTimeTrackerPrototype> Role;
  [DataField(null, false, 1, true, false, null)]
  public TimeSpan Time;
  private static readonly Color DefaultDepartmentColor = Color.Yellow;

  public override bool Check(
    IEntityManager entManager,
    IPrototypeManager protoManager,
    HumanoidCharacterProfile? profile,
    IReadOnlyDictionary<string, TimeSpan> playTimes,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    reason = new FormattedMessage();
    PlayTimeTrackerPrototype trackerPrototype = protoManager.Index<PlayTimeTrackerPrototype>(this.Role);
    SharedJobSystem entitySystem = entManager.EntitySysManager.GetEntitySystem<SharedJobSystem>();
    TimeSpan timeSpan;
    playTimes.TryGetValue((string) this.Role, out timeSpan);
    TimeSpan time = this.Time - timeSpan;
    double totalMinutes = time.TotalMinutes;
    string str = ContentLocalizationManager.FormatPlaytime(time);
    List<ProtoId<JobPrototype>> jobPrototypes = entitySystem.GetJobPrototypes(this.Role);
    Color color = RoleTimeRequirement.DefaultDepartmentColor;
    DepartmentPrototype chosenDepartment;
    if (entitySystem.TryGetListHighestWeightDepartment(jobPrototypes, out chosenDepartment))
      color = chosenDepartment.Color;
    string or = ContentLocalizationManager.FormatListToOr(jobPrototypes.Select<ProtoId<JobPrototype>, string>((Func<ProtoId<JobPrototype>, string>) (jobId => protoManager.Index<JobPrototype>(jobId).LocalizedName)).ToList<string>());
    LocId? name = trackerPrototype.Name;
    if (name.HasValue)
      or = Loc.GetString((string) name.GetValueOrDefault());
    if (!this.Inverted)
    {
      if (totalMinutes <= 0.0)
        return true;
      reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-role-insufficient", ("time", (object) str), ("job", (object) or), ("departmentColor", (object) ((Color) ref color).ToHex())));
      return false;
    }
    if (totalMinutes > 0.0)
      return true;
    reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-role-too-high", ("time", (object) str), ("job", (object) or), ("departmentColor", (object) ((Color) ref color).ToHex())));
    return false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RoleTimeRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobRequirement target1 = (JobRequirement) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RoleTimeRequirement) target1;
    if (serialization.TryCustomCopy<RoleTimeRequirement>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<PlayTimeTrackerPrototype> target2 = new ProtoId<PlayTimeTrackerPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<PlayTimeTrackerPrototype>>(this.Role, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<PlayTimeTrackerPrototype>>(this.Role, hookCtx, context);
    target.Role = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Time, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Time, hookCtx, context);
    target.Time = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RoleTimeRequirement target,
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
    RoleTimeRequirement target1 = (RoleTimeRequirement) target;
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
    RoleTimeRequirement target1 = (RoleTimeRequirement) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RoleTimeRequirement JobRequirement.Instantiate() => new RoleTimeRequirement();
}
