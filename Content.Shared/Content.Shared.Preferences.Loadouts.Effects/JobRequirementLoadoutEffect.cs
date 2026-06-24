using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences.Loadouts.Effects;

public sealed class JobRequirementLoadoutEffect : LoadoutEffect, ISerializationGenerated<JobRequirementLoadoutEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public JobRequirement Requirement;

	public override bool Validate(HumanoidCharacterProfile profile, RoleLoadout loadout, ICommonSession? session, IDependencyCollection collection, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		if (session == null)
		{
			reason = FormattedMessage.Empty;
			return true;
		}
		IReadOnlyDictionary<string, TimeSpan> playtimes = collection.Resolve<ISharedPlaytimeManager>().GetPlayTimes(session);
		return Requirement.Check(collection.Resolve<IEntityManager>(), collection.Resolve<IPrototypeManager>(), profile, playtimes, out reason);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref JobRequirementLoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		LoadoutEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (JobRequirementLoadoutEffect)definitionCast;
		if (!serialization.TryCustomCopy<JobRequirementLoadoutEffect>(this, ref target, hookCtx, false, context))
		{
			JobRequirement RequirementTemp = null;
			if (Requirement == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<JobRequirement>(Requirement, ref RequirementTemp, hookCtx, true, context))
			{
				RequirementTemp = serialization.CreateCopy<JobRequirement>(Requirement, hookCtx, context, false);
			}
			target.Requirement = RequirementTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref JobRequirementLoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobRequirementLoadoutEffect cast = (JobRequirementLoadoutEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobRequirementLoadoutEffect cast = (JobRequirementLoadoutEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override JobRequirementLoadoutEffect Instantiate()
	{
		return new JobRequirementLoadoutEffect();
	}
}
