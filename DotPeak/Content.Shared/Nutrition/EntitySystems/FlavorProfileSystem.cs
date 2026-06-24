// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.FlavorProfileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CCVar;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class FlavorProfileSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IConfigurationManager _configManager;
  private const string BackupFlavorMessage = "flavor-profile-unknown";

  private int FlavorLimit => this._configManager.GetCVar<int>(CCVars.FlavorLimit);

  public string GetLocalizedFlavorsMessage(
    EntityUid uid,
    EntityUid user,
    Solution solution,
    FlavorProfileComponent? flavorProfile = null)
  {
    if (!this.Resolve<FlavorProfileComponent>(uid, ref flavorProfile, false))
      return this.Loc.GetString("flavor-profile-unknown");
    HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) flavorProfile.Flavors);
    stringSet.UnionWith((IEnumerable<string>) this.GetFlavorsFromReagents(solution, this.FlavorLimit - stringSet.Count, flavorProfile.IgnoreReagents));
    FlavorProfileModificationEvent modificationEvent = new FlavorProfileModificationEvent(user, stringSet);
    this.RaiseLocalEvent<FlavorProfileModificationEvent>(modificationEvent);
    this.RaiseLocalEvent<FlavorProfileModificationEvent>(uid, modificationEvent);
    this.RaiseLocalEvent<FlavorProfileModificationEvent>(user, modificationEvent);
    return this.FlavorsToFlavorMessage(stringSet);
  }

  public string GetLocalizedFlavorsMessage(EntityUid user, Solution solution)
  {
    HashSet<string> flavorsFromReagents = this.GetFlavorsFromReagents(solution, this.FlavorLimit);
    FlavorProfileModificationEvent args = new FlavorProfileModificationEvent(user, flavorsFromReagents);
    this.RaiseLocalEvent<FlavorProfileModificationEvent>(user, args, true);
    return this.FlavorsToFlavorMessage(flavorsFromReagents);
  }

  private string FlavorsToFlavorMessage(HashSet<string> flavorSet)
  {
    List<FlavorPrototype> flavorPrototypeList1 = new List<FlavorPrototype>();
    foreach (string flavor in flavorSet)
    {
      FlavorPrototype prototype;
      if (!string.IsNullOrEmpty(flavor) && this._prototypeManager.TryIndex<FlavorPrototype>(flavor, out prototype))
        flavorPrototypeList1.Add(prototype);
    }
    flavorPrototypeList1.Sort((Comparison<FlavorPrototype>) ((a, b) => a.FlavorType.CompareTo((object) b.FlavorType)));
    if (flavorPrototypeList1.Count == 1 && !string.IsNullOrEmpty(flavorPrototypeList1[0].FlavorDescription))
      return this.Loc.GetString("flavor-profile", ("flavor", (object) this.Loc.GetString(flavorPrototypeList1[0].FlavorDescription)));
    if (flavorPrototypeList1.Count <= 1)
      return this.Loc.GetString("flavor-profile-unknown");
    ILocalizationManager loc = this.Loc;
    List<FlavorPrototype> flavorPrototypeList2 = flavorPrototypeList1;
    string flavorDescription = flavorPrototypeList2[flavorPrototypeList2.Count - 1].FlavorDescription;
    string str = loc.GetString(flavorDescription);
    return this.Loc.GetString("flavor-profile-multiple", ("flavors", (object) string.Join(", ", flavorPrototypeList1.GetRange(0, flavorPrototypeList1.Count - 1).Select<FlavorPrototype, string>((Func<FlavorPrototype, string>) (i => this.Loc.GetString(i.FlavorDescription))))), ("lastFlavor", (object) str));
  }

  private HashSet<string> GetFlavorsFromReagents(
    Solution solution,
    int desiredAmount,
    HashSet<string>? toIgnore = null)
  {
    HashSet<string> flavorsFromReagents = new HashSet<string>();
    foreach ((ReagentPrototype key, FixedPoint2 fixedPoint2) in solution.GetReagentPrototypes(this._prototypeManager))
    {
      if (toIgnore == null || !toIgnore.Contains(key.ID))
      {
        if (flavorsFromReagents.Count != desiredAmount)
        {
          if (!(fixedPoint2 < key.FlavorMinimum) && key.Flavor.HasValue)
          {
            HashSet<string> stringSet = flavorsFromReagents;
            ProtoId<FlavorPrototype>? flavor = key.Flavor;
            string valueOrDefault = flavor.HasValue ? (string) flavor.GetValueOrDefault() : (string) null;
            stringSet.Add(valueOrDefault);
          }
        }
        else
          break;
      }
    }
    return flavorsFromReagents;
  }
}
