using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences.Loadouts.Effects;

public sealed class SpeciesLoadoutEffect : LoadoutEffect, ISerializationGenerated<SpeciesLoadoutEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<SpeciesPrototype>> Species = new List<ProtoId<SpeciesPrototype>>();

	public override bool Validate(HumanoidCharacterProfile profile, RoleLoadout loadout, ICommonSession? session, IDependencyCollection collection, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (Species.Contains(profile.Species))
		{
			reason = null;
			return true;
		}
		reason = FormattedMessage.FromUnformatted(Loc.GetString("loadout-group-species-restriction"));
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpeciesLoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		LoadoutEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SpeciesLoadoutEffect)definitionCast;
		if (!serialization.TryCustomCopy<SpeciesLoadoutEffect>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<SpeciesPrototype>> SpeciesTemp = null;
			if (Species == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<SpeciesPrototype>>>(Species, ref SpeciesTemp, hookCtx, true, context))
			{
				SpeciesTemp = serialization.CreateCopy<List<ProtoId<SpeciesPrototype>>>(Species, hookCtx, context, false);
			}
			target.Species = SpeciesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpeciesLoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpeciesLoadoutEffect cast = (SpeciesLoadoutEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpeciesLoadoutEffect cast = (SpeciesLoadoutEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpeciesLoadoutEffect Instantiate()
	{
		return new SpeciesLoadoutEffect();
	}
}
