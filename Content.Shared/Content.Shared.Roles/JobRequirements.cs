using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Roles;

public static class JobRequirements
{
	public static bool TryRequirementsMet(JobPrototype job, IReadOnlyDictionary<string, TimeSpan> playTimes, [NotNullWhen(false)] out FormattedMessage? reason, IEntityManager entManager, IPrototypeManager protoManager, HumanoidCharacterProfile? profile)
	{
		HashSet<JobRequirement> requirements = entManager.System<SharedRoleSystem>().GetJobRequirement(job);
		reason = null;
		if (requirements == null)
		{
			return true;
		}
		foreach (JobRequirement item in requirements)
		{
			if (!item.Check(entManager, protoManager, profile, playTimes, out reason))
			{
				return false;
			}
		}
		return true;
	}
}
