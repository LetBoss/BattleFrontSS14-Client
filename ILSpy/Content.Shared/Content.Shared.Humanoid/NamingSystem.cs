using System;
using Content.Shared.Dataset;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Random.Helpers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Humanoid;

public sealed class NamingSystem : EntitySystem
{
	private static readonly ProtoId<SpeciesPrototype> FallbackSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Human");

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	public string GetName(string species, Gender? gender = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		SpeciesPrototype speciesProto = default(SpeciesPrototype);
		if (!_prototypeManager.TryIndex<SpeciesPrototype>(species, ref speciesProto))
		{
			speciesProto = _prototypeManager.Index<SpeciesPrototype>(FallbackSpecies);
			((EntitySystem)this).Log.Warning($"Unable to find species {species} for name, falling back to {FallbackSpecies}");
		}
		return speciesProto.Naming switch
		{
			SpeciesNaming.First => base.Loc.GetString("namepreset-first", (ValueTuple<string, object>)("first", GetFirstName(speciesProto, gender))), 
			SpeciesNaming.TheFirstofLast => base.Loc.GetString("namepreset-thefirstoflast", (ValueTuple<string, object>)("first", GetFirstName(speciesProto, gender)), (ValueTuple<string, object>)("last", GetLastName(speciesProto))), 
			SpeciesNaming.FirstDashFirst => base.Loc.GetString("namepreset-firstdashfirst", (ValueTuple<string, object>)("first1", GetFirstName(speciesProto, gender)), (ValueTuple<string, object>)("first2", GetFirstName(speciesProto, gender))), 
			SpeciesNaming.LastFirst => base.Loc.GetString("namepreset-lastfirst", (ValueTuple<string, object>)("last", GetLastName(speciesProto)), (ValueTuple<string, object>)("first", GetFirstName(speciesProto, gender))), 
			SpeciesNaming.FirstLastCombined => base.Loc.GetString("rmc-namepreset-firstlastcombined", (ValueTuple<string, object>)("first", GetFirstName(speciesProto, gender)), (ValueTuple<string, object>)("last", GetLastName(speciesProto))), 
			_ => base.Loc.GetString("namepreset-firstlast", (ValueTuple<string, object>)("first", GetFirstName(speciesProto, gender)), (ValueTuple<string, object>)("last", GetLastName(speciesProto))), 
		};
	}

	public string GetFirstName(SpeciesPrototype speciesProto, Gender? gender = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (gender.HasValue)
		{
			Gender valueOrDefault = gender.GetValueOrDefault();
			if ((int)valueOrDefault == 2)
			{
				return _random.Pick(_prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.FemaleFirstNames));
			}
			if ((int)valueOrDefault == 3)
			{
				return _random.Pick(_prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.MaleFirstNames));
			}
		}
		if (RandomExtensions.Prob(_random, 0.5f))
		{
			return _random.Pick(_prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.MaleFirstNames));
		}
		return _random.Pick(_prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.FemaleFirstNames));
	}

	public string GetLastName(SpeciesPrototype speciesProto)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return _random.Pick(_prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.LastNames));
	}
}
