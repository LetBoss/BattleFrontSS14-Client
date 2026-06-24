using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Localizations;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Roles;

[Serializable]
[NetSerializable]
public sealed class OverallPlaytimeRequirement : JobRequirement, ISerializationGenerated<OverallPlaytimeRequirement>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public TimeSpan Time;

	public override bool Check(IEntityManager entManager, IPrototypeManager protoManager, HumanoidCharacterProfile? profile, IReadOnlyDictionary<string, TimeSpan> playTimes, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		reason = new FormattedMessage();
		TimeSpan overallTime = playTimes.GetValueOrDefault(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(PlayTimeTrackingShared.TrackerOverall));
		TimeSpan overallDiffSpan = Time - overallTime;
		double overallDiff = overallDiffSpan.TotalMinutes;
		string formattedOverallDiff = ContentLocalizationManager.FormatPlaytime(overallDiffSpan);
		if (!Inverted)
		{
			if (overallDiff <= 0.0 || overallTime >= Time)
			{
				return true;
			}
			reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-overall-insufficient", new(string, object)[1] { ("time", formattedOverallDiff) }));
			return false;
		}
		if (overallDiff <= 0.0 || overallTime >= Time)
		{
			reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-overall-too-high", new(string, object)[1] { ("time", formattedOverallDiff) }));
			return false;
		}
		return true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OverallPlaytimeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobRequirement definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (OverallPlaytimeRequirement)definitionCast;
		if (!serialization.TryCustomCopy<OverallPlaytimeRequirement>(this, ref target, hookCtx, false, context))
		{
			TimeSpan TimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Time, ref TimeTemp, hookCtx, false, context))
			{
				TimeTemp = serialization.CreateCopy<TimeSpan>(Time, hookCtx, context, false);
			}
			target.Time = TimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OverallPlaytimeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref JobRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverallPlaytimeRequirement cast = (OverallPlaytimeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OverallPlaytimeRequirement cast = (OverallPlaytimeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OverallPlaytimeRequirement Instantiate()
	{
		return new OverallPlaytimeRequirement();
	}
}
