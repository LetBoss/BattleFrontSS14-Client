using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
public sealed class AgeRequirement : JobRequirement, ISerializationGenerated<AgeRequirement>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public int RequiredAge;

	public override bool Check(IEntityManager entManager, IPrototypeManager protoManager, HumanoidCharacterProfile? profile, IReadOnlyDictionary<string, TimeSpan> playTimes, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		reason = new FormattedMessage();
		if (profile == null)
		{
			return true;
		}
		if (!Inverted)
		{
			reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-age-too-young", new(string, object)[1] { ("age", RequiredAge) }));
			if (profile.Age < RequiredAge)
			{
				return false;
			}
		}
		else
		{
			reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-age-too-old", new(string, object)[1] { ("age", RequiredAge) }));
			if (profile.Age > RequiredAge)
			{
				return false;
			}
		}
		return true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AgeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobRequirement definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AgeRequirement)definitionCast;
		if (!serialization.TryCustomCopy<AgeRequirement>(this, ref target, hookCtx, false, context))
		{
			int RequiredAgeTemp = 0;
			if (!serialization.TryCustomCopy<int>(RequiredAge, ref RequiredAgeTemp, hookCtx, false, context))
			{
				RequiredAgeTemp = RequiredAge;
			}
			target.RequiredAge = RequiredAgeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AgeRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref JobRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AgeRequirement cast = (AgeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AgeRequirement cast = (AgeRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AgeRequirement Instantiate()
	{
		return new AgeRequirement();
	}
}
