using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Content.Shared.Preferences;
using Content.Shared.Traits;
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
public sealed class TraitsRequirement : JobRequirement, ISerializationGenerated<TraitsRequirement>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public HashSet<ProtoId<TraitPrototype>> Traits = new HashSet<ProtoId<TraitPrototype>>();

	public override bool Check(IEntityManager entManager, IPrototypeManager protoManager, HumanoidCharacterProfile? profile, IReadOnlyDictionary<string, TimeSpan> playTimes, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		reason = new FormattedMessage();
		if (profile == null)
		{
			return true;
		}
		StringBuilder sb = new StringBuilder();
		sb.Append("[color=yellow]");
		foreach (ProtoId<TraitPrototype> t in Traits)
		{
			sb.Append(Loc.GetString(LocId.op_Implicit(protoManager.Index<TraitPrototype>(t).Name)) + " ");
		}
		sb.Append("[/color]");
		if (!Inverted)
		{
			reason = FormattedMessage.FromMarkupPermissive($"{Loc.GetString("role-timer-whitelisted-traits")}\n{sb}");
			foreach (ProtoId<TraitPrototype> trait in Traits)
			{
				if (profile.TraitPreferences.Contains(trait))
				{
					return true;
				}
			}
			return false;
		}
		reason = FormattedMessage.FromMarkupPermissive($"{Loc.GetString("role-timer-blacklisted-traits")}\n{sb}");
		foreach (ProtoId<TraitPrototype> trait2 in Traits)
		{
			if (profile.TraitPreferences.Contains(trait2))
			{
				return false;
			}
		}
		return true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TraitsRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		JobRequirement definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TraitsRequirement)definitionCast;
		if (!serialization.TryCustomCopy<TraitsRequirement>(this, ref target, hookCtx, false, context))
		{
			HashSet<ProtoId<TraitPrototype>> TraitsTemp = null;
			if (Traits == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<TraitPrototype>>>(Traits, ref TraitsTemp, hookCtx, true, context))
			{
				TraitsTemp = serialization.CreateCopy<HashSet<ProtoId<TraitPrototype>>>(Traits, hookCtx, context, false);
			}
			target.Traits = TraitsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TraitsRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref JobRequirement target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TraitsRequirement cast = (TraitsRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TraitsRequirement cast = (TraitsRequirement)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TraitsRequirement Instantiate()
	{
		return new TraitsRequirement();
	}
}
