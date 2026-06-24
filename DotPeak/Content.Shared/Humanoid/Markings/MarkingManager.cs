// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.MarkingManager
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
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
    this._prototypeManager.PrototypesReloaded += new Action<PrototypesReloadedEventArgs>(this.OnPrototypeReload);
    this.CachePrototypes();
  }

  private void CachePrototypes()
  {
    this._index.Clear();
    Dictionary<MarkingCategories, Dictionary<string, MarkingPrototype>> source = new Dictionary<MarkingCategories, Dictionary<string, MarkingPrototype>>();
    foreach (MarkingCategories key in Enum.GetValues<MarkingCategories>())
      source.Add(key, new Dictionary<string, MarkingPrototype>());
    foreach (MarkingPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<MarkingPrototype>())
    {
      this._index.Add(enumeratePrototype);
      source[enumeratePrototype.MarkingCategory].Add(enumeratePrototype.ID, enumeratePrototype);
    }
    this.Markings = this._prototypeManager.EnumeratePrototypes<MarkingPrototype>().ToFrozenDictionary<MarkingPrototype, string>((Func<MarkingPrototype, string>) (x => x.ID));
    this.CategorizedMarkings = source.ToFrozenDictionary<KeyValuePair<MarkingCategories, Dictionary<string, MarkingPrototype>>, MarkingCategories, FrozenDictionary<string, MarkingPrototype>>((Func<KeyValuePair<MarkingCategories, Dictionary<string, MarkingPrototype>>, MarkingCategories>) (x => x.Key), (Func<KeyValuePair<MarkingCategories, Dictionary<string, MarkingPrototype>>, FrozenDictionary<string, MarkingPrototype>>) (x => x.Value.ToFrozenDictionary<string, MarkingPrototype>()));
  }

  public FrozenDictionary<string, MarkingPrototype> MarkingsByCategory(MarkingCategories category)
  {
    return this.CategorizedMarkings[category];
  }

  public IReadOnlyDictionary<string, MarkingPrototype> MarkingsByCategoryAndSpecies(
    MarkingCategories category,
    string species)
  {
    MarkingPointsPrototype markingPointsPrototype = this._prototypeManager.Index<MarkingPointsPrototype>(this._prototypeManager.Index<SpeciesPrototype>(species).MarkingPoints);
    Dictionary<string, MarkingPrototype> dictionary = new Dictionary<string, MarkingPrototype>();
    foreach ((string key, MarkingPrototype markingPrototype) in this.MarkingsByCategory(category))
    {
      if (!markingPointsPrototype.OnlyWhitelisted)
      {
        MarkingPoints valueOrDefault = markingPointsPrototype.Points.GetValueOrDefault<MarkingCategories, MarkingPoints>(category);
        if ((valueOrDefault != null ? (valueOrDefault.OnlyWhitelisted ? 1 : 0) : 0) == 0)
          goto label_5;
      }
      if (markingPrototype.SpeciesRestrictions == null)
        continue;
label_5:
      if (markingPrototype.SpeciesRestrictions == null || markingPrototype.SpeciesRestrictions.Contains(species))
        dictionary.Add(key, markingPrototype);
    }
    return (IReadOnlyDictionary<string, MarkingPrototype>) dictionary;
  }

  public IReadOnlyDictionary<string, MarkingPrototype> MarkingsByCategoryAndSex(
    MarkingCategories category,
    Sex sex)
  {
    Dictionary<string, MarkingPrototype> dictionary = new Dictionary<string, MarkingPrototype>();
    foreach ((string key, MarkingPrototype markingPrototype) in this.MarkingsByCategory(category))
    {
      Sex? sexRestriction = markingPrototype.SexRestriction;
      if (sexRestriction.HasValue)
      {
        sexRestriction = markingPrototype.SexRestriction;
        Sex sex1 = sex;
        if (!(sexRestriction.GetValueOrDefault() == sex1 & sexRestriction.HasValue))
          continue;
      }
      dictionary.Add(key, markingPrototype);
    }
    return (IReadOnlyDictionary<string, MarkingPrototype>) dictionary;
  }

  public IReadOnlyDictionary<string, MarkingPrototype> MarkingsByCategoryAndSpeciesAndSex(
    MarkingCategories category,
    string species,
    Sex sex)
  {
    bool onlyWhitelisted = this._prototypeManager.Index<MarkingPointsPrototype>(this._prototypeManager.Index<SpeciesPrototype>(species).MarkingPoints).OnlyWhitelisted;
    Dictionary<string, MarkingPrototype> dictionary = new Dictionary<string, MarkingPrototype>();
    foreach ((string key, MarkingPrototype markingPrototype) in this.MarkingsByCategory(category))
    {
      if ((!onlyWhitelisted || markingPrototype.SpeciesRestrictions != null) && (markingPrototype.SpeciesRestrictions == null || markingPrototype.SpeciesRestrictions.Contains(species)))
      {
        if (markingPrototype.SexRestriction.HasValue)
        {
          Sex? sexRestriction = markingPrototype.SexRestriction;
          Sex sex1 = sex;
          if (!(sexRestriction.GetValueOrDefault() == sex1 & sexRestriction.HasValue))
            continue;
        }
        dictionary.Add(key, markingPrototype);
      }
    }
    return (IReadOnlyDictionary<string, MarkingPrototype>) dictionary;
  }

  public bool TryGetMarking(Marking marking, [NotNullWhen(true)] out MarkingPrototype? markingResult)
  {
    return this.Markings.TryGetValue(marking.MarkingId, out markingResult);
  }

  public bool IsValidMarking(Marking marking, MarkingCategories category, string species, Sex sex)
  {
    MarkingPrototype markingResult;
    if (!this.TryGetMarking(marking, out markingResult) || markingResult.MarkingCategory != category || markingResult.SpeciesRestrictions != null && !markingResult.SpeciesRestrictions.Contains(species))
      return false;
    if (markingResult.SexRestriction.HasValue)
    {
      Sex? sexRestriction = markingResult.SexRestriction;
      Sex sex1 = sex;
      if (!(sexRestriction.GetValueOrDefault() == sex1 & sexRestriction.HasValue))
        return false;
    }
    return marking.MarkingColors.Count == markingResult.Sprites.Count;
  }

  private void OnPrototypeReload(PrototypesReloadedEventArgs args)
  {
    if (!args.WasModified<MarkingPrototype>())
      return;
    this.CachePrototypes();
  }

  public bool CanBeApplied(
    string species,
    Sex sex,
    Marking marking,
    IPrototypeManager? prototypeManager = null)
  {
    IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
    SpeciesPrototype speciesPrototype = prototypeManager.Index<SpeciesPrototype>(species);
    bool onlyWhitelisted = prototypeManager.Index<MarkingPointsPrototype>(speciesPrototype.MarkingPoints).OnlyWhitelisted;
    MarkingPrototype markingResult;
    if (!this.TryGetMarking(marking, out markingResult) || onlyWhitelisted && markingResult.SpeciesRestrictions == null || markingResult.SpeciesRestrictions != null && !markingResult.SpeciesRestrictions.Contains(species))
      return false;
    if (markingResult.SexRestriction.HasValue)
    {
      Sex? sexRestriction = markingResult.SexRestriction;
      Sex sex1 = sex;
      if (!(sexRestriction.GetValueOrDefault() == sex1 & sexRestriction.HasValue))
        return false;
    }
    return true;
  }

  public bool CanBeApplied(
    string species,
    Sex sex,
    MarkingPrototype prototype,
    IPrototypeManager? prototypeManager = null)
  {
    IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
    SpeciesPrototype speciesPrototype = prototypeManager.Index<SpeciesPrototype>(species);
    if (prototypeManager.Index<MarkingPointsPrototype>(speciesPrototype.MarkingPoints).OnlyWhitelisted && prototype.SpeciesRestrictions == null || prototype.SpeciesRestrictions != null && !prototype.SpeciesRestrictions.Contains(species))
      return false;
    if (prototype.SexRestriction.HasValue)
    {
      Sex? sexRestriction = prototype.SexRestriction;
      Sex sex1 = sex;
      if (!(sexRestriction.GetValueOrDefault() == sex1 & sexRestriction.HasValue))
        return false;
    }
    return true;
  }

  public bool MustMatchSkin(
    string species,
    HumanoidVisualLayers layer,
    out float alpha,
    IPrototypeManager? prototypeManager = null)
  {
    IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
    SpeciesPrototype speciesPrototype = prototypeManager.Index<SpeciesPrototype>(species);
    HumanoidSpeciesBaseSpritesPrototype prototype1;
    string id;
    HumanoidSpeciesSpriteLayer prototype2;
    if (!prototypeManager.TryIndex<HumanoidSpeciesBaseSpritesPrototype>(speciesPrototype.SpriteSet, out prototype1) || !prototype1.Sprites.TryGetValue(layer, out id) || !prototypeManager.TryIndex<HumanoidSpeciesSpriteLayer>(id, out prototype2) || prototype2 == null || !prototype2.MarkingsMatchSkin)
    {
      alpha = 1f;
      return false;
    }
    alpha = prototype2.LayerAlpha;
    return true;
  }
}
