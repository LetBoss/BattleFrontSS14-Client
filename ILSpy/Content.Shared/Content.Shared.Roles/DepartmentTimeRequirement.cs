using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace Content.Shared.Roles;

[Serializable]
[NetSerializable]
public sealed class DepartmentTimeRequirement : JobRequirement, ISerializationGenerated<DepartmentTimeRequirement>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<DepartmentPrototype> Department;

	[DataField(null, false, 1, true, false, null)]
	public TimeSpan Time;

	public override bool Check(IEntityManager entManager, IPrototypeManager protoManager, HumanoidCharacterProfile? profile, IReadOnlyDictionary<string, TimeSpan> playTimes, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		reason = new FormattedMessage();
		TimeSpan playtime = TimeSpan.Zero;
		DepartmentPrototype department = protoManager.Index<DepartmentPrototype>(Department);
		foreach (ProtoId<JobPrototype> other in department.Roles)
		{
			string proto = protoManager.Index<JobPrototype>(other).PlayTimeTracker;
			playTimes.TryGetValue(proto, out var otherTime);
			playtime += otherTime;
		}
		TimeSpan deptDiffSpan = Time - playtime;
		double deptDiff = deptDiffSpan.TotalMinutes;
		string formattedDeptDiff = ContentLocalizationManager.FormatPlaytime(deptDiffSpan);
		string nameDepartment = "role-timer-department-unknown";
		DepartmentPrototype departmentIndexed = default(DepartmentPrototype);
		if (protoManager.TryIndex<DepartmentPrototype>(Department, ref departmentIndexed))
		{
			nameDepartment = LocId.op_Implicit(departmentIndexed.Name);
		}
		if (!Inverted)
		{
			if (deptDiff <= 0.0)
			{
				return true;
			}
			reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-department-insufficient", new(string, object)[3]
			{
				("time", formattedDeptDiff),
				("department", Loc.GetString(nameDepartment)),
				("departmentColor", ((Color)(ref department.Color)).ToHex())
			}));
			return false;
		}
		if (deptDiff <= 0.0)
		{
			reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-department-too-high", new(string, object)[3]
			{
				("time", formattedDeptDiff),
				("department", Loc.GetString(nameDepartment)),
				("departmentColor", ((Color)(ref department.Color)).ToHex())
			}));
			return false;
		}
		return true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DepartmentTimeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (DepartmentTimeRequirement)definitionCast;
		if (!serialization.TryCustomCopy<DepartmentTimeRequirement>(this, ref target, hookCtx, false, context))
		{
			ProtoId<DepartmentPrototype> DepartmentTemp = default(ProtoId<DepartmentPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<DepartmentPrototype>>(Department, ref DepartmentTemp, hookCtx, false, context))
			{
				DepartmentTemp = serialization.CreateCopy<ProtoId<DepartmentPrototype>>(Department, hookCtx, context, false);
			}
			target.Department = DepartmentTemp;
			TimeSpan TimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Time, ref TimeTemp, hookCtx, false, context))
			{
				TimeTemp = serialization.CreateCopy<TimeSpan>(Time, hookCtx, context, false);
			}
			target.Time = TimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DepartmentTimeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref JobRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DepartmentTimeRequirement cast = (DepartmentTimeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DepartmentTimeRequirement cast = (DepartmentTimeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DepartmentTimeRequirement Instantiate()
	{
		return new DepartmentTimeRequirement();
	}
}
