// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.NamingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Dataset;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Random.Helpers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

#nullable enable
namespace Content.Shared.Humanoid;

public sealed class NamingSystem : EntitySystem
{
  private static readonly ProtoId<SpeciesPrototype> FallbackSpecies = (ProtoId<SpeciesPrototype>) "Human";
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IPrototypeManager _prototypeManager;

  public string GetName(string species, Gender? gender = null)
  {
    SpeciesPrototype prototype;
    if (!this._prototypeManager.TryIndex<SpeciesPrototype>(species, out prototype))
    {
      prototype = this._prototypeManager.Index<SpeciesPrototype>(NamingSystem.FallbackSpecies);
      this.Log.Warning($"Unable to find species {species} for name, falling back to {NamingSystem.FallbackSpecies}");
    }
    switch (prototype.Naming)
    {
      case SpeciesNaming.First:
        return this.Loc.GetString("namepreset-first", ("first", (object) this.GetFirstName(prototype, gender)));
      case SpeciesNaming.FirstDashFirst:
        return this.Loc.GetString("namepreset-firstdashfirst", ("first1", (object) this.GetFirstName(prototype, gender)), ("first2", (object) this.GetFirstName(prototype, gender)));
      case SpeciesNaming.TheFirstofLast:
        return this.Loc.GetString("namepreset-thefirstoflast", ("first", (object) this.GetFirstName(prototype, gender)), ("last", (object) this.GetLastName(prototype)));
      case SpeciesNaming.LastFirst:
        return this.Loc.GetString("namepreset-lastfirst", ("last", (object) this.GetLastName(prototype)), ("first", (object) this.GetFirstName(prototype, gender)));
      case SpeciesNaming.FirstLastCombined:
        return this.Loc.GetString("rmc-namepreset-firstlastcombined", ("first", (object) this.GetFirstName(prototype, gender)), ("last", (object) this.GetLastName(prototype)));
      default:
        return this.Loc.GetString("namepreset-firstlast", ("first", (object) this.GetFirstName(prototype, gender)), ("last", (object) this.GetLastName(prototype)));
    }
  }

  public string GetFirstName(SpeciesPrototype speciesProto, Gender? gender = null)
  {
    if (gender.HasValue)
    {
      switch (gender.GetValueOrDefault())
      {
        case Gender.Female:
          return this._random.Pick(this._prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.FemaleFirstNames));
        case Gender.Male:
          return this._random.Pick(this._prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.MaleFirstNames));
      }
    }
    return this._random.Prob(0.5f) ? this._random.Pick(this._prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.MaleFirstNames)) : this._random.Pick(this._prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.FemaleFirstNames));
  }

  public string GetLastName(SpeciesPrototype speciesProto)
  {
    return this._random.Pick(this._prototypeManager.Index<LocalizedDatasetPrototype>(speciesProto.LastNames));
  }
}
