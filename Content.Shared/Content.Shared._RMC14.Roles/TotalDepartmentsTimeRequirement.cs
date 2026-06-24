using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace Content.Shared._RMC14.Roles;

[Serializable]
[NetSerializable]
public sealed class TotalDepartmentsTimeRequirement : JobRequirement, ISerializationGenerated<TotalDepartmentsTimeRequirement>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Group;

	[DataField(null, false, 1, true, false, null)]
	public TimeSpan Time;

	public bool TryRequirementsMet(IReadOnlyDictionary<string, TimeSpan> playTimes, out FormattedMessage? reason, IEntityManager entManager, IPrototypeManager prototypes)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		reason = null;
		TimeSpan playtime = TimeSpan.Zero;
		HashSet<string> trackers = new HashSet<string>();
		DepartmentGroupComponent comp = default(DepartmentGroupComponent);
		if (!prototypes.Index(Group).TryGetComponent<DepartmentGroupComponent>(ref comp, entManager.ComponentFactory))
		{
			Logger.GetSawmill("job.requirements").Error($"No {"DepartmentGroupComponent"} found on entity {Group}");
			return true;
		}
		foreach (ProtoId<DepartmentPrototype> departmentId in comp.Departments)
		{
			foreach (ProtoId<JobPrototype> other in prototypes.Index<DepartmentPrototype>(departmentId).Roles)
			{
				string proto = prototypes.Index<JobPrototype>(other).PlayTimeTracker;
				trackers.Add(proto);
			}
		}
		foreach (string tracker in trackers)
		{
			playTimes.TryGetValue(tracker, out var otherTime);
			playtime += otherTime;
		}
		double deptDiff = Time.TotalMinutes - playtime.TotalMinutes;
		if (!Inverted)
		{
			if (deptDiff <= 0.0)
			{
				return true;
			}
			reason = FormattedMessage.FromMarkupOrThrow(Loc.GetString("role-timer-total-department-insufficient", new(string, object)[3]
			{
				("time", Math.Ceiling(deptDiff)),
				("roles", Loc.GetString(LocId.op_Implicit(comp.Name))),
				("rolesColor", ((Color)(ref comp.Color)).ToHex())
			}));
			return false;
		}
		if (deptDiff <= 0.0)
		{
			reason = FormattedMessage.FromMarkupOrThrow(Loc.GetString("role-timer-total-department-too-high", new(string, object)[3]
			{
				("time", 0.0 - deptDiff),
				("roles", Loc.GetString(LocId.op_Implicit(comp.Name))),
				("rolesColor", ((Color)(ref comp.Color)).ToHex())
			}));
			return false;
		}
		return true;
	}

	public override bool Check(IEntityManager entManager, IPrototypeManager protoManager, HumanoidCharacterProfile? profile, IReadOnlyDictionary<string, TimeSpan> playTimes, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		return TryRequirementsMet(playTimes, out reason, entManager, protoManager);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TotalDepartmentsTimeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (TotalDepartmentsTimeRequirement)definitionCast;
		if (!serialization.TryCustomCopy<TotalDepartmentsTimeRequirement>(this, ref target, hookCtx, false, context))
		{
			EntProtoId GroupTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Group, ref GroupTemp, hookCtx, false, context))
			{
				GroupTemp = serialization.CreateCopy<EntProtoId>(Group, hookCtx, context, false);
			}
			target.Group = GroupTemp;
			TimeSpan TimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Time, ref TimeTemp, hookCtx, false, context))
			{
				TimeTemp = serialization.CreateCopy<TimeSpan>(Time, hookCtx, context, false);
			}
			target.Time = TimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TotalDepartmentsTimeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref JobRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TotalDepartmentsTimeRequirement cast = (TotalDepartmentsTimeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TotalDepartmentsTimeRequirement cast = (TotalDepartmentsTimeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TotalDepartmentsTimeRequirement Instantiate()
	{
		return new TotalDepartmentsTimeRequirement();
	}
}
