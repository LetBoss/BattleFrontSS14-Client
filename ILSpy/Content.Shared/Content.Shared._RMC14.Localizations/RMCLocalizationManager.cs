using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Content.Shared._RMC14.IdentityManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Localizations;

public sealed class RMCLocalizationManager
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private ILocalizationManager _loc;

	public void Initialize(CultureInfo culture)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		_loc.AddFunction(culture, "GENDER", new LocFunction(FuncGender));
		_loc.AddFunction(culture, "REFLEXIVE", new LocFunction(FuncReflexive));
		_loc.AddFunction(culture, "PROPER", new LocFunction(FuncProper));
	}

	private ILocValue FuncGender(LocArgs args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		if (((LocArgs)(ref args)).Args.Count >= 1)
		{
			object entity0 = ((LocArgs)(ref args)).Args[0].Value;
			if (entity0 is IdentityEntity identity)
			{
				entity0 = identity;
			}
			if (entity0 is EntityUid entity1)
			{
				GrammarComponent grammar = default(GrammarComponent);
				if (_entity.TryGetComponent<GrammarComponent>(entity1, ref grammar) && grammar.Gender.HasValue)
				{
					return (ILocValue)new LocValueString(((object)grammar.Gender.Value/*cast due to constrained. prefix*/).ToString().ToLowerInvariant());
				}
				if (TryGetEntityLocAttrib(entity1, "gender", out string gender))
				{
					return (ILocValue)new LocValueString(gender);
				}
			}
			return (ILocValue)new LocValueString("Neuter");
		}
		return (ILocValue)new LocValueString("Neuter");
	}

	private ILocValue FuncReflexive(LocArgs args)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		ILocValue arg = ((LocArgs)(ref args)).Args[0];
		if (arg.Value is IdentityEntity identity)
		{
			arg = (ILocValue)new LocValueEntity(identity.Entity);
		}
		return (ILocValue)new LocValueString(_loc.GetString("zzzz-reflexive-pronoun", (ValueTuple<string, object>)("ent", arg)));
	}

	private ILocValue FuncProper(LocArgs args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		if (((LocArgs)(ref args)).Args.Count >= 1)
		{
			object entity0 = ((LocArgs)(ref args)).Args[0].Value;
			if (entity0 is IdentityEntity identity)
			{
				entity0 = identity;
			}
			if (entity0 is EntityUid entity1)
			{
				GrammarComponent grammar = default(GrammarComponent);
				if (_entity.TryGetComponent<GrammarComponent>(entity1, ref grammar) && grammar.ProperNoun.HasValue)
				{
					return (ILocValue)new LocValueString(grammar.ProperNoun.Value.ToString().ToLowerInvariant());
				}
				if (TryGetEntityLocAttrib(entity1, "proper", out string proper))
				{
					return (ILocValue)new LocValueString(proper);
				}
			}
			return (ILocValue)new LocValueString("false");
		}
		return (ILocValue)new LocValueString("false");
	}

	private bool TryGetEntityLocAttrib(EntityUid entity, string attribute, [NotNullWhen(true)] out string? value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		GrammarComponent grammar = default(GrammarComponent);
		if (_entity.TryGetComponent<GrammarComponent>(entity, ref grammar) && grammar.Attributes.TryGetValue(attribute, out value))
		{
			return true;
		}
		EntityPrototype prototype = _entity.GetComponent<MetaDataComponent>(entity).EntityPrototype;
		if (prototype == null)
		{
			value = null;
			return false;
		}
		return _loc.GetEntityData(prototype.ID).Attributes.TryGetValue(attribute, out value);
	}
}
