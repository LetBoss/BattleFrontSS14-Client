// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Systems.SharedResearchSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Lathe;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Research.Systems;

public abstract class SharedResearchSystem : EntitySystem
{
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedLatheSystem _lathe;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<TechnologyDatabaseComponent, MapInitEvent>(new ComponentEventHandler<TechnologyDatabaseComponent, MapInitEvent>(this.OnMapInit));
  }

  private void OnMapInit(EntityUid uid, TechnologyDatabaseComponent component, MapInitEvent args)
  {
    this.UpdateTechnologyCards(uid, component);
  }

  public void UpdateTechnologyCards(EntityUid uid, TechnologyDatabaseComponent? component = null)
  {
    if (!this.Resolve<TechnologyDatabaseComponent>(uid, ref component))
      return;
    List<TechnologyPrototype> availableTechnologies = this.GetAvailableTechnologies(uid, component);
    this._random.Shuffle<TechnologyPrototype>((IList<TechnologyPrototype>) availableTechnologies);
    component.CurrentTechnologyCards.Clear();
    foreach (string supportedDiscipline in component.SupportedDisciplines)
    {
      string discipline = supportedDiscipline;
      TechnologyPrototype technologyPrototype = availableTechnologies.FirstOrDefault<TechnologyPrototype>((Func<TechnologyPrototype, bool>) (p => p.Discipline == (ProtoId<TechDisciplinePrototype>) discipline));
      if (technologyPrototype != null)
        component.CurrentTechnologyCards.Add(technologyPrototype.ID);
    }
    this.Dirty(uid, (IComponent) component);
  }

  public List<TechnologyPrototype> GetAvailableTechnologies(
    EntityUid uid,
    TechnologyDatabaseComponent? component = null)
  {
    if (!this.Resolve<TechnologyDatabaseComponent>(uid, ref component, false))
      return new List<TechnologyPrototype>();
    List<TechnologyPrototype> availableTechnologies = new List<TechnologyPrototype>();
    Dictionary<string, int> disciplineTiers = this.GetDisciplineTiers(component);
    foreach (TechnologyPrototype enumeratePrototype in this.PrototypeManager.EnumeratePrototypes<TechnologyPrototype>())
    {
      if (this.IsTechnologyAvailable(component, enumeratePrototype, disciplineTiers))
        availableTechnologies.Add(enumeratePrototype);
    }
    return availableTechnologies;
  }

  public bool IsTechnologyAvailable(
    TechnologyDatabaseComponent component,
    TechnologyPrototype tech,
    Dictionary<string, int>? disciplineTiers = null)
  {
    if (disciplineTiers == null)
      disciplineTiers = this.GetDisciplineTiers(component);
    if (tech.Hidden || !component.SupportedDisciplines.Contains((string) tech.Discipline) || tech.Tier > disciplineTiers[(string) tech.Discipline] || component.UnlockedTechnologies.Contains(tech.ID))
      return false;
    foreach (ProtoId<TechnologyPrototype> technologyPrerequisite in tech.TechnologyPrerequisites)
    {
      if (!component.UnlockedTechnologies.Contains((string) technologyPrerequisite))
        return false;
    }
    return true;
  }

  public Dictionary<string, int> GetDisciplineTiers(TechnologyDatabaseComponent component)
  {
    Dictionary<string, int> disciplineTiers = new Dictionary<string, int>();
    foreach (string supportedDiscipline in component.SupportedDisciplines)
      disciplineTiers.Add(supportedDiscipline, this.GetHighestDisciplineTier(component, supportedDiscipline));
    return disciplineTiers;
  }

  public int GetHighestDisciplineTier(TechnologyDatabaseComponent component, string disciplineId)
  {
    return this.GetHighestDisciplineTier(component, this.PrototypeManager.Index<TechDisciplinePrototype>(disciplineId));
  }

  public int GetHighestDisciplineTier(
    TechnologyDatabaseComponent component,
    TechDisciplinePrototype techDiscipline)
  {
    List<TechnologyPrototype> list1 = this.PrototypeManager.EnumeratePrototypes<TechnologyPrototype>().Where<TechnologyPrototype>((Func<TechnologyPrototype, bool>) (p => p.Discipline == (ProtoId<TechDisciplinePrototype>) techDiscipline.ID && !p.Hidden)).ToList<TechnologyPrototype>();
    List<TechnologyPrototype> source = new List<TechnologyPrototype>();
    foreach (string unlockedTechnology in component.UnlockedTechnologies)
    {
      TechnologyPrototype technologyPrototype = this.PrototypeManager.Index<TechnologyPrototype>(unlockedTechnology);
      if (!(technologyPrototype.Discipline != (ProtoId<TechDisciplinePrototype>) techDiscipline.ID))
        source.Add(technologyPrototype);
    }
    int num = techDiscipline.TierPrerequisites.Keys.Max();
    int tier;
    for (tier = 2; tier <= num; tier++)
    {
      List<TechnologyPrototype> list2 = source.Where<TechnologyPrototype>((Func<TechnologyPrototype, bool>) (p => p.Tier == tier - 1)).ToList<TechnologyPrototype>();
      List<TechnologyPrototype> list3 = list1.Where<TechnologyPrototype>((Func<TechnologyPrototype, bool>) (p => p.Discipline == (ProtoId<TechDisciplinePrototype>) techDiscipline.ID && p.Tier == tier - 1)).ToList<TechnologyPrototype>();
      if (list3.Count == 0 || (double) list2.Count / (double) list3.Count < (double) techDiscipline.TierPrerequisites[tier] || tier >= techDiscipline.LockoutTier && component.MainDiscipline != null && techDiscipline.ID != component.MainDiscipline)
        break;
    }
    return tier - 1;
  }

  public FormattedMessage GetTechnologyDescription(
    TechnologyPrototype technology,
    bool includeCost = true,
    bool includeTier = true,
    bool includePrereqs = false,
    TechDisciplinePrototype? disciplinePrototype = null)
  {
    FormattedMessage technologyDescription = new FormattedMessage();
    if (includeTier)
    {
      if (disciplinePrototype == null)
        disciplinePrototype = this.PrototypeManager.Index<TechDisciplinePrototype>(technology.Discipline);
      technologyDescription.AddMarkupOrThrow(this.Loc.GetString("research-console-tier-discipline-info", ("tier", (object) technology.Tier), ("color", (object) disciplinePrototype.Color), ("discipline", (object) this.Loc.GetString(disciplinePrototype.Name))));
      technologyDescription.PushNewline();
    }
    if (includeCost)
    {
      technologyDescription.AddMarkupOrThrow(this.Loc.GetString("research-console-cost", ("amount", (object) technology.Cost)));
      technologyDescription.PushNewline();
    }
    if (includePrereqs && technology.TechnologyPrerequisites.Any<ProtoId<TechnologyPrototype>>())
    {
      technologyDescription.AddMarkupOrThrow(this.Loc.GetString("research-console-prereqs-list-start"));
      foreach (ProtoId<TechnologyPrototype> technologyPrerequisite in technology.TechnologyPrerequisites)
      {
        TechnologyPrototype technologyPrototype = this.PrototypeManager.Index<TechnologyPrototype>(technologyPrerequisite);
        technologyDescription.PushNewline();
        technologyDescription.AddMarkupOrThrow(this.Loc.GetString("research-console-prereqs-list-entry", ("text", (object) this.Loc.GetString((string) technologyPrototype.Name))));
      }
      technologyDescription.PushNewline();
    }
    technologyDescription.AddMarkupOrThrow(this.Loc.GetString("research-console-unlocks-list-start"));
    foreach (ProtoId<LatheRecipePrototype> recipeUnlock in technology.RecipeUnlocks)
    {
      LatheRecipePrototype proto = this.PrototypeManager.Index<LatheRecipePrototype>(recipeUnlock);
      technologyDescription.PushNewline();
      technologyDescription.AddMarkupOrThrow(this.Loc.GetString("research-console-unlocks-list-entry", ("name", (object) this._lathe.GetRecipeName(proto))));
    }
    foreach (GenericUnlock genericUnlock in (IEnumerable<GenericUnlock>) technology.GenericUnlocks)
    {
      technologyDescription.PushNewline();
      technologyDescription.AddMarkupOrThrow(this.Loc.GetString("research-console-unlocks-list-entry-generic", ("text", (object) this.Loc.GetString(genericUnlock.UnlockDescription))));
    }
    return technologyDescription;
  }

  public bool IsTechnologyUnlocked(
    EntityUid uid,
    TechnologyPrototype technology,
    TechnologyDatabaseComponent? component = null)
  {
    return this.Resolve<TechnologyDatabaseComponent>(uid, ref component) && this.IsTechnologyUnlocked(uid, technology.ID, component);
  }

  public bool IsTechnologyUnlocked(
    EntityUid uid,
    string technologyId,
    TechnologyDatabaseComponent? component = null)
  {
    return this.Resolve<TechnologyDatabaseComponent>(uid, ref component, false) && component.UnlockedTechnologies.Contains(technologyId);
  }

  public void TrySetMainDiscipline(
    TechnologyPrototype prototype,
    EntityUid uid,
    TechnologyDatabaseComponent? component = null)
  {
    if (!this.Resolve<TechnologyDatabaseComponent>(uid, ref component))
      return;
    TechDisciplinePrototype disciplinePrototype = this.PrototypeManager.Index<TechDisciplinePrototype>(prototype.Discipline);
    if (prototype.Tier < disciplinePrototype.LockoutTier)
      return;
    component.MainDiscipline = (string) prototype.Discipline;
    this.Dirty(uid, (IComponent) component);
    TechnologyDatabaseModifiedEvent args = new TechnologyDatabaseModifiedEvent();
    this.RaiseLocalEvent<TechnologyDatabaseModifiedEvent>(uid, ref args);
  }

  public bool TryRemoveTechnology(
    Entity<TechnologyDatabaseComponent> entity,
    ProtoId<TechnologyPrototype> tech)
  {
    return this.TryRemoveTechnology(entity, this.PrototypeManager.Index<TechnologyPrototype>(tech));
  }

  public bool TryRemoveTechnology(
    Entity<TechnologyDatabaseComponent> entity,
    TechnologyPrototype tech)
  {
    if (!entity.Comp.UnlockedTechnologies.Remove(tech.ID))
      return false;
    foreach (ProtoId<LatheRecipePrototype> recipeUnlock in tech.RecipeUnlocks)
    {
      bool flag = false;
      foreach (string unlockedTechnology in entity.Comp.UnlockedTechnologies)
      {
        if (this.PrototypeManager.Index<TechnologyPrototype>(unlockedTechnology).RecipeUnlocks.Contains(recipeUnlock))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        entity.Comp.UnlockedRecipes.Remove((string) recipeUnlock);
    }
    this.Dirty((EntityUid) entity, (IComponent) entity.Comp);
    this.UpdateTechnologyCards((EntityUid) entity, (TechnologyDatabaseComponent) entity);
    return true;
  }

  public void ClearTechs(EntityUid uid, TechnologyDatabaseComponent? comp = null)
  {
    if (!this.Resolve<TechnologyDatabaseComponent>(uid, ref comp) || comp.UnlockedTechnologies.Count == 0)
      return;
    comp.UnlockedTechnologies.Clear();
    this.Dirty(uid, (IComponent) comp);
  }

  public void AddLatheRecipe(EntityUid uid, string recipe, TechnologyDatabaseComponent? component = null)
  {
    if (!this.Resolve<TechnologyDatabaseComponent>(uid, ref component) || component.UnlockedRecipes.Contains(recipe))
      return;
    component.UnlockedRecipes.Add(recipe);
    this.Dirty(uid, (IComponent) component);
    TechnologyDatabaseModifiedEvent args = new TechnologyDatabaseModifiedEvent(new List<string>()
    {
      recipe
    });
    this.RaiseLocalEvent<TechnologyDatabaseModifiedEvent>(uid, ref args);
  }
}
