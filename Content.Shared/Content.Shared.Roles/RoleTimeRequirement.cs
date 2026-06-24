using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

namespace Content.Shared.Roles;

[Serializable]
[NetSerializable]
public sealed class RoleTimeRequirement : JobRequirement, ISerializationGenerated<RoleTimeRequirement>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<PlayTimeTrackerPrototype> Role;

	[DataField(null, false, 1, true, false, null)]
	public TimeSpan Time;

	private static readonly Color DefaultDepartmentColor = Color.Yellow;

	public override bool Check(IEntityManager entManager, IPrototypeManager protoManager, HumanoidCharacterProfile? profile, IReadOnlyDictionary<string, TimeSpan> playTimes, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		reason = new FormattedMessage();
		PlayTimeTrackerPrototype playTimeTrackerPrototype = protoManager.Index<PlayTimeTrackerPrototype>(Role);
		SharedJobSystem entitySystem = entManager.EntitySysManager.GetEntitySystem<SharedJobSystem>();
		playTimes.TryGetValue(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(Role), out var roleTime);
		TimeSpan roleDiffSpan = Time - roleTime;
		double roleDiff = roleDiffSpan.TotalMinutes;
		string formattedRoleDiff = ContentLocalizationManager.FormatPlaytime(roleDiffSpan);
		List<ProtoId<JobPrototype>> jobList = entitySystem.GetJobPrototypes(Role);
		Color departmentColor = DefaultDepartmentColor;
		if (entitySystem.TryGetListHighestWeightDepartment(jobList, out DepartmentPrototype department))
		{
			departmentColor = department.Color;
		}
		string names = ContentLocalizationManager.FormatListToOr(jobList.Select((ProtoId<JobPrototype> jobId) => protoManager.Index<JobPrototype>(jobId).LocalizedName).ToList());
		LocId? name = playTimeTrackerPrototype.Name;
		if (name.HasValue)
		{
			LocId trackerName = name.GetValueOrDefault();
			names = Loc.GetString(LocId.op_Implicit(trackerName));
		}
		if (!Inverted)
		{
			if (roleDiff <= 0.0)
			{
				return true;
			}
			reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-role-insufficient", new(string, object)[3]
			{
				("time", formattedRoleDiff),
				("job", names),
				("departmentColor", ((Color)(ref departmentColor)).ToHex())
			}));
			return false;
		}
		if (roleDiff <= 0.0)
		{
			reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-role-too-high", new(string, object)[3]
			{
				("time", formattedRoleDiff),
				("job", names),
				("departmentColor", ((Color)(ref departmentColor)).ToHex())
			}));
			return false;
		}
		return true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RoleTimeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		JobRequirement definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RoleTimeRequirement)definitionCast;
		if (!serialization.TryCustomCopy<RoleTimeRequirement>(this, ref target, hookCtx, false, context))
		{
			ProtoId<PlayTimeTrackerPrototype> RoleTemp = default(ProtoId<PlayTimeTrackerPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<PlayTimeTrackerPrototype>>(Role, ref RoleTemp, hookCtx, false, context))
			{
				RoleTemp = serialization.CreateCopy<ProtoId<PlayTimeTrackerPrototype>>(Role, hookCtx, context, false);
			}
			target.Role = RoleTemp;
			TimeSpan TimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Time, ref TimeTemp, hookCtx, false, context))
			{
				TimeTemp = serialization.CreateCopy<TimeSpan>(Time, hookCtx, context, false);
			}
			target.Time = TimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RoleTimeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref JobRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RoleTimeRequirement cast = (RoleTimeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RoleTimeRequirement cast = (RoleTimeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RoleTimeRequirement Instantiate()
	{
		return new RoleTimeRequirement();
	}
}
