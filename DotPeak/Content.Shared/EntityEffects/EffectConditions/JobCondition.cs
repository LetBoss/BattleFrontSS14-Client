// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EffectConditions.JobCondition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Localizations;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class JobCondition : 
  EntityEffectCondition,
  ISerializationGenerated<JobCondition>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<JobPrototype>> Job;

  public override bool Condition(EntityEffectBaseArgs args)
  {
    MindContainerComponent component1;
    args.EntityManager.TryGetComponent<MindContainerComponent>(args.TargetEntity, out component1);
    MindComponent component2;
    if (component1 == null || !args.EntityManager.TryGetComponent<MindComponent>(component1.Mind, out component2))
      return false;
    foreach (EntityUid mindRole in component2.MindRoles)
    {
      if (args.EntityManager.HasComponent<JobRoleComponent>(mindRole))
      {
        MindRoleComponent component3;
        if (!args.EntityManager.TryGetComponent<MindRoleComponent>(mindRole, out component3))
          IoCManager.Resolve<ILogManager>().GetSawmill("entity_effect").Error($"Encountered job mind role entity {mindRole} without a {"MindRoleComponent"}");
        else if (!component3.JobPrototype.HasValue)
          IoCManager.Resolve<ILogManager>().GetSawmill("entity_effect").Error($"Encountered job mind role entity {mindRole} without a {"JobPrototype"}");
        else if (this.Job.Contains(component3.JobPrototype.Value))
          return true;
      }
    }
    return false;
  }

  public override string GuidebookExplanation(IPrototypeManager prototype)
  {
    return Loc.GetString("reagent-effect-condition-guidebook-job-condition", ("job", (object) ContentLocalizationManager.FormatListToOr(this.Job.Select<ProtoId<JobPrototype>, string>((Func<ProtoId<JobPrototype>, string>) (jobId => prototype.Index<JobPrototype>(jobId).LocalizedName)).ToList<string>())));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref JobCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffectCondition target1 = (EntityEffectCondition) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (JobCondition) target1;
    if (serialization.TryCustomCopy<JobCondition>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<JobPrototype>> target2 = (List<ProtoId<JobPrototype>>) null;
    if (this.Job == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<JobPrototype>>>(this.Job, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<JobPrototype>>>(this.Job, hookCtx, context);
    target.Job = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref JobCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffectCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobCondition target1 = (JobCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffectCondition) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobCondition target1 = (JobCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual JobCondition EntityEffectCondition.Instantiate() => new JobCondition();
}
