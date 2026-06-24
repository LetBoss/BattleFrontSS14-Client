using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Humanoid.Markings;

public sealed class MarkingManager
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly List<MarkingPrototype> _index = new List<MarkingPrototype>();

	public FrozenDictionary<MarkingCategories, FrozenDictionary<string, MarkingPrototype>> CategorizedMarkings;

	public FrozenDictionary<string, MarkingPrototype> Markings;

	public void Initialize()
	{
		_prototypeManager.PrototypesReloaded += OnPrototypeReload;
		CachePrototypes();
	}

	private void CachePrototypes()
	{
		_index.Clear();
		Dictionary<MarkingCategories, Dictionary<string, MarkingPrototype>> markingDict = new Dictionary<MarkingCategories, Dictionary<string, MarkingPrototype>>();
		MarkingCategories[] values = Enum.GetValues<MarkingCategories>();
		foreach (MarkingCategories category in values)
		{
			markingDict.Add(category, new Dictionary<string, MarkingPrototype>());
		}
		foreach (MarkingPrototype prototype in _prototypeManager.EnumeratePrototypes<MarkingPrototype>())
		{
			_index.Add(prototype);
			markingDict[prototype.MarkingCategory].Add(prototype.ID, prototype);
		}
		Markings = _prototypeManager.EnumeratePrototypes<MarkingPrototype>().ToFrozenDictionary((MarkingPrototype x) => x.ID);
		CategorizedMarkings = markingDict.ToFrozenDictionary((KeyValuePair<MarkingCategories, Dictionary<string, MarkingPrototype>> x) => x.Key, (KeyValuePair<MarkingCategories, Dictionary<string, MarkingPrototype>> x) => x.Value.ToFrozenDictionary());
	}

	public FrozenDictionary<string, MarkingPrototype> MarkingsByCategory(MarkingCategories category)
	{
		return CategorizedMarkings[category];
	}

	public IReadOnlyDictionary<string, MarkingPrototype> MarkingsByCategoryAndSpecies(MarkingCategories category, string species)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		SpeciesPrototype speciesProto = _prototypeManager.Index<SpeciesPrototype>(species);
		MarkingPointsPrototype markingPoints = _prototypeManager.Index<MarkingPointsPrototype>(speciesProto.MarkingPoints);
		Dictionary<string, MarkingPrototype> res = new Dictionary<string, MarkingPrototype>();
		foreach (var (key, marking) in MarkingsByCategory(category))
		{
			if (!markingPoints.OnlyWhitelisted)
			{
				MarkingPoints? valueOrDefault = markingPoints.Points.GetValueOrDefault(category);
				if (valueOrDefault == null || !valueOrDefault.OnlyWhitelisted)
				{
					goto IL_007b;
				}
			}
			if (marking.SpeciesRestrictions == null)
			{
				continue;
			}
			goto IL_007b;
			IL_007b:
			if (marking.SpeciesRestrictions == null || marking.SpeciesRestrictions.Contains(species))
			{
				res.Add(key, marking);
			}
		}
		return res;
	}

	public IReadOnlyDictionary<string, MarkingPrototype> MarkingsByCategoryAndSex(MarkingCategories category, Sex sex)
	{
		Dictionary<string, MarkingPrototype> res = new Dictionary<string, MarkingPrototype>();
		foreach (var (key, marking) in MarkingsByCategory(category))
		{
			if (!marking.SexRestriction.HasValue || marking.SexRestriction == sex)
			{
				res.Add(key, marking);
			}
		}
		return res;
	}

	public IReadOnlyDictionary<string, MarkingPrototype> MarkingsByCategoryAndSpeciesAndSex(MarkingCategories category, string species, Sex sex)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		SpeciesPrototype speciesProto = _prototypeManager.Index<SpeciesPrototype>(species);
		bool onlyWhitelisted = _prototypeManager.Index<MarkingPointsPrototype>(speciesProto.MarkingPoints).OnlyWhitelisted;
		Dictionary<string, MarkingPrototype> res = new Dictionary<string, MarkingPrototype>();
		foreach (var (key, marking) in MarkingsByCategory(category))
		{
			if ((!onlyWhitelisted || marking.SpeciesRestrictions != null) && (marking.SpeciesRestrictions == null || marking.SpeciesRestrictions.Contains(species)) && (!marking.SexRestriction.HasValue || marking.SexRestriction == sex))
			{
				res.Add(key, marking);
			}
		}
		return res;
	}

	public bool TryGetMarking(Marking marking, [NotNullWhen(true)] out MarkingPrototype? markingResult)
	{
		return Markings.TryGetValue(marking.MarkingId, out markingResult);
	}

	public bool IsValidMarking(Marking marking, MarkingCategories category, string species, Sex sex)
	{
		if (!TryGetMarking(marking, out MarkingPrototype proto))
		{
			return false;
		}
		if (proto.MarkingCategory != category || (proto.SpeciesRestrictions != null && !proto.SpeciesRestrictions.Contains(species)) || (proto.SexRestriction.HasValue && proto.SexRestriction != sex))
		{
			return false;
		}
		if (marking.MarkingColors.Count != proto.Sprites.Count)
		{
			return false;
		}
		return true;
	}

	private void OnPrototypeReload(PrototypesReloadedEventArgs args)
	{
		if (args.WasModified<MarkingPrototype>())
		{
			CachePrototypes();
		}
	}

	public bool CanBeApplied(string species, Sex sex, Marking marking, IPrototypeManager? prototypeManager = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
		SpeciesPrototype speciesProto = prototypeManager.Index<SpeciesPrototype>(species);
		bool onlyWhitelisted = prototypeManager.Index<MarkingPointsPrototype>(speciesProto.MarkingPoints).OnlyWhitelisted;
		if (!TryGetMarking(marking, out MarkingPrototype prototype))
		{
			return false;
		}
		if (onlyWhitelisted && prototype.SpeciesRestrictions == null)
		{
			return false;
		}
		if (prototype.SpeciesRestrictions != null && !prototype.SpeciesRestrictions.Contains(species))
		{
			return false;
		}
		if (prototype.SexRestriction.HasValue && prototype.SexRestriction != sex)
		{
			return false;
		}
		return true;
	}

	public bool CanBeApplied(string species, Sex sex, MarkingPrototype prototype, IPrototypeManager? prototypeManager = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
		SpeciesPrototype speciesProto = prototypeManager.Index<SpeciesPrototype>(species);
		if (prototypeManager.Index<MarkingPointsPrototype>(speciesProto.MarkingPoints).OnlyWhitelisted && prototype.SpeciesRestrictions == null)
		{
			return false;
		}
		if (prototype.SpeciesRestrictions != null && !prototype.SpeciesRestrictions.Contains(species))
		{
			return false;
		}
		if (prototype.SexRestriction.HasValue && prototype.SexRestriction != sex)
		{
			return false;
		}
		return true;
	}

	public bool MustMatchSkin(string species, HumanoidVisualLayers layer, out float alpha, IPrototypeManager? prototypeManager = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
		SpeciesPrototype speciesProto = prototypeManager.Index<SpeciesPrototype>(species);
		HumanoidSpeciesBaseSpritesPrototype baseSprites = default(HumanoidSpeciesBaseSpritesPrototype);
		HumanoidSpeciesSpriteLayer sprite = default(HumanoidSpeciesSpriteLayer);
		if (!prototypeManager.TryIndex<HumanoidSpeciesBaseSpritesPrototype>(speciesProto.SpriteSet, ref baseSprites) || !baseSprites.Sprites.TryGetValue(layer, out string spriteName) || !prototypeManager.TryIndex<HumanoidSpeciesSpriteLayer>(spriteName, ref sprite) || sprite == null || !sprite.MarkingsMatchSkin)
		{
			alpha = 1f;
			return false;
		}
		alpha = sprite.LayerAlpha;
		return true;
	}
}
