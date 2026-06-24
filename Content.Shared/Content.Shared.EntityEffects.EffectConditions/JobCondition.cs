using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class JobCondition : EntityEffectCondition, ISerializationGenerated<JobCondition>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<JobPrototype>> Job;

	public override bool Condition(EntityEffectBaseArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		MindContainerComponent mindContainer = default(MindContainerComponent);
		args.EntityManager.TryGetComponent<MindContainerComponent>(args.TargetEntity, ref mindContainer);
		MindComponent mind = default(MindComponent);
		if (mindContainer == null || !args.EntityManager.TryGetComponent<MindComponent>(mindContainer.Mind, ref mind))
		{
			return false;
		}
		MindRoleComponent mindRole = default(MindRoleComponent);
		foreach (EntityUid roleId in mind.MindRoles)
		{
			if (args.EntityManager.HasComponent<JobRoleComponent>(roleId))
			{
				if (!args.EntityManager.TryGetComponent<MindRoleComponent>(roleId, ref mindRole))
				{
					IoCManager.Resolve<ILogManager>().GetSawmill("entity_effect").Error($"Encountered job mind role entity {roleId} without a {"MindRoleComponent"}");
				}
				else if (!mindRole.JobPrototype.HasValue)
				{
					IoCManager.Resolve<ILogManager>().GetSawmill("entity_effect").Error($"Encountered job mind role entity {roleId} without a {"JobPrototype"}");
				}
				else if (Job.Contains(mindRole.JobPrototype.Value))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		List<string> localizedNames = Job.Select<ProtoId<JobPrototype>, string>((ProtoId<JobPrototype> jobId) => prototype.Index<JobPrototype>(jobId).LocalizedName).ToList();
		return Loc.GetString("reagent-effect-condition-guidebook-job-condition", new(string, object)[1] { ("job", ContentLocalizationManager.FormatListToOr(localizedNames)) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref JobCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityEffectCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (JobCondition)definitionCast;
		if (!serialization.TryCustomCopy<JobCondition>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<JobPrototype>> JobTemp = null;
			if (Job == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<JobPrototype>>>(Job, ref JobTemp, hookCtx, true, context))
			{
				JobTemp = serialization.CreateCopy<List<ProtoId<JobPrototype>>>(Job, hookCtx, context, false);
			}
			target.Job = JobTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref JobCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobCondition cast = (JobCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobCondition cast = (JobCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override JobCondition Instantiate()
	{
		return new JobCondition();
	}
}
