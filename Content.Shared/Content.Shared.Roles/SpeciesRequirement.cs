using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Roles;

[Serializable]
[NetSerializable]
public sealed class SpeciesRequirement : JobRequirement, ISerializationGenerated<SpeciesRequirement>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public HashSet<ProtoId<SpeciesPrototype>> Species = new HashSet<ProtoId<SpeciesPrototype>>();

	public override bool Check(IEntityManager entManager, IPrototypeManager protoManager, HumanoidCharacterProfile? profile, IReadOnlyDictionary<string, TimeSpan> playTimes, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		reason = new FormattedMessage();
		if (profile == null)
		{
			return true;
		}
		StringBuilder sb = new StringBuilder();
		sb.Append("[color=yellow]");
		foreach (ProtoId<SpeciesPrototype> s in Species)
		{
			sb.Append(Loc.GetString(protoManager.Index<SpeciesPrototype>(s).Name) + " ");
		}
		sb.Append("[/color]");
		if (!Inverted)
		{
			reason = FormattedMessage.FromMarkupPermissive($"{Loc.GetString("role-timer-whitelisted-species")}\n{sb}");
			if (!Species.Contains(profile.Species))
			{
				return false;
			}
		}
		else
		{
			reason = FormattedMessage.FromMarkupPermissive($"{Loc.GetString("role-timer-blacklisted-species")}\n{sb}");
			if (Species.Contains(profile.Species))
			{
				return false;
			}
		}
		return true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpeciesRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		JobRequirement definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SpeciesRequirement)definitionCast;
		if (!serialization.TryCustomCopy<SpeciesRequirement>(this, ref target, hookCtx, false, context))
		{
			HashSet<ProtoId<SpeciesPrototype>> SpeciesTemp = null;
			if (Species == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<SpeciesPrototype>>>(Species, ref SpeciesTemp, hookCtx, true, context))
			{
				SpeciesTemp = serialization.CreateCopy<HashSet<ProtoId<SpeciesPrototype>>>(Species, hookCtx, context, false);
			}
			target.Species = SpeciesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpeciesRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref JobRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpeciesRequirement cast = (SpeciesRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpeciesRequirement cast = (SpeciesRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpeciesRequirement Instantiate()
	{
		return new SpeciesRequirement();
	}
}
