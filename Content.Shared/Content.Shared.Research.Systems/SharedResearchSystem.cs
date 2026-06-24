using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Lathe;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

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
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TechnologyDatabaseComponent, MapInitEvent>((ComponentEventHandler<TechnologyDatabaseComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, TechnologyDatabaseComponent component, MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateTechnologyCards(uid, component);
	}

	public void UpdateTechnologyCards(EntityUid uid, TechnologyDatabaseComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<TechnologyDatabaseComponent>(uid, ref component, true))
		{
			return;
		}
		List<TechnologyPrototype> availableTechnology = GetAvailableTechnologies(uid, component);
		_random.Shuffle<TechnologyPrototype>((IList<TechnologyPrototype>)availableTechnology);
		component.CurrentTechnologyCards.Clear();
		foreach (string discipline in component.SupportedDisciplines)
		{
			TechnologyPrototype selected = availableTechnology.FirstOrDefault((TechnologyPrototype p) => p.Discipline == ProtoId<TechDisciplinePrototype>.op_Implicit(discipline));
			if (selected != null)
			{
				component.CurrentTechnologyCards.Add(selected.ID);
			}
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	public List<TechnologyPrototype> GetAvailableTechnologies(EntityUid uid, TechnologyDatabaseComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<TechnologyDatabaseComponent>(uid, ref component, false))
		{
			return new List<TechnologyPrototype>();
		}
		List<TechnologyPrototype> availableTechnologies = new List<TechnologyPrototype>();
		Dictionary<string, int> disciplineTiers = GetDisciplineTiers(component);
		foreach (TechnologyPrototype tech in PrototypeManager.EnumeratePrototypes<TechnologyPrototype>())
		{
			if (IsTechnologyAvailable(component, tech, disciplineTiers))
			{
				availableTechnologies.Add(tech);
			}
		}
		return availableTechnologies;
	}

	public bool IsTechnologyAvailable(TechnologyDatabaseComponent component, TechnologyPrototype tech, Dictionary<string, int>? disciplineTiers = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (disciplineTiers == null)
		{
			disciplineTiers = GetDisciplineTiers(component);
		}
		if (tech.Hidden)
		{
			return false;
		}
		if (!component.SupportedDisciplines.Contains(ProtoId<TechDisciplinePrototype>.op_Implicit(tech.Discipline)))
		{
			return false;
		}
		if (tech.Tier > disciplineTiers[ProtoId<TechDisciplinePrototype>.op_Implicit(tech.Discipline)])
		{
			return false;
		}
		if (component.UnlockedTechnologies.Contains(tech.ID))
		{
			return false;
		}
		foreach (ProtoId<TechnologyPrototype> prereq in tech.TechnologyPrerequisites)
		{
			if (!component.UnlockedTechnologies.Contains(ProtoId<TechnologyPrototype>.op_Implicit(prereq)))
			{
				return false;
			}
		}
		return true;
	}

	public Dictionary<string, int> GetDisciplineTiers(TechnologyDatabaseComponent component)
	{
		Dictionary<string, int> tiers = new Dictionary<string, int>();
		foreach (string discipline in component.SupportedDisciplines)
		{
			tiers.Add(discipline, GetHighestDisciplineTier(component, discipline));
		}
		return tiers;
	}

	public int GetHighestDisciplineTier(TechnologyDatabaseComponent component, string disciplineId)
	{
		return GetHighestDisciplineTier(component, PrototypeManager.Index<TechDisciplinePrototype>(disciplineId));
	}

	public int GetHighestDisciplineTier(TechnologyDatabaseComponent component, TechDisciplinePrototype techDiscipline)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		List<TechnologyPrototype> allTech = (from p in PrototypeManager.EnumeratePrototypes<TechnologyPrototype>()
			where p.Discipline == ProtoId<TechDisciplinePrototype>.op_Implicit(techDiscipline.ID) && !p.Hidden
			select p).ToList();
		List<TechnologyPrototype> allUnlocked = new List<TechnologyPrototype>();
		foreach (string recipe in component.UnlockedTechnologies)
		{
			TechnologyPrototype proto = PrototypeManager.Index<TechnologyPrototype>(recipe);
			if (!(proto.Discipline != ProtoId<TechDisciplinePrototype>.op_Implicit(techDiscipline.ID)))
			{
				allUnlocked.Add(proto);
			}
		}
		int highestTier = techDiscipline.TierPrerequisites.Keys.Max();
		int tier;
		for (tier = 2; tier <= highestTier; tier++)
		{
			List<TechnologyPrototype> unlockedTierTech = allUnlocked.Where((TechnologyPrototype p) => p.Tier == tier - 1).ToList();
			List<TechnologyPrototype> allTierTech = allTech.Where((TechnologyPrototype p) => p.Discipline == ProtoId<TechDisciplinePrototype>.op_Implicit(techDiscipline.ID) && p.Tier == tier - 1).ToList();
			if (allTierTech.Count == 0 || (float)unlockedTierTech.Count / (float)allTierTech.Count < techDiscipline.TierPrerequisites[tier] || (tier >= techDiscipline.LockoutTier && component.MainDiscipline != null && techDiscipline.ID != component.MainDiscipline))
			{
				break;
			}
		}
		return tier - 1;
	}

	public FormattedMessage GetTechnologyDescription(TechnologyPrototype technology, bool includeCost = true, bool includeTier = true, bool includePrereqs = false, TechDisciplinePrototype? disciplinePrototype = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage description = new FormattedMessage();
		if (includeTier)
		{
			if (disciplinePrototype == null)
			{
				disciplinePrototype = PrototypeManager.Index<TechDisciplinePrototype>(technology.Discipline);
			}
			description.AddMarkupOrThrow(base.Loc.GetString("research-console-tier-discipline-info", new(string, object)[3]
			{
				("tier", technology.Tier),
				("color", disciplinePrototype.Color),
				("discipline", base.Loc.GetString(disciplinePrototype.Name))
			}));
			description.PushNewline();
		}
		if (includeCost)
		{
			description.AddMarkupOrThrow(base.Loc.GetString("research-console-cost", (ValueTuple<string, object>)("amount", technology.Cost)));
			description.PushNewline();
		}
		if (includePrereqs && technology.TechnologyPrerequisites.Any())
		{
			description.AddMarkupOrThrow(base.Loc.GetString("research-console-prereqs-list-start"));
			foreach (ProtoId<TechnologyPrototype> recipe in technology.TechnologyPrerequisites)
			{
				TechnologyPrototype techProto = PrototypeManager.Index<TechnologyPrototype>(recipe);
				description.PushNewline();
				description.AddMarkupOrThrow(base.Loc.GetString("research-console-prereqs-list-entry", (ValueTuple<string, object>)("text", base.Loc.GetString(LocId.op_Implicit(techProto.Name)))));
			}
			description.PushNewline();
		}
		description.AddMarkupOrThrow(base.Loc.GetString("research-console-unlocks-list-start"));
		foreach (ProtoId<LatheRecipePrototype> recipe2 in technology.RecipeUnlocks)
		{
			LatheRecipePrototype recipeProto = PrototypeManager.Index<LatheRecipePrototype>(recipe2);
			description.PushNewline();
			description.AddMarkupOrThrow(base.Loc.GetString("research-console-unlocks-list-entry", (ValueTuple<string, object>)("name", _lathe.GetRecipeName(recipeProto))));
		}
		foreach (GenericUnlock generic in technology.GenericUnlocks)
		{
			description.PushNewline();
			description.AddMarkupOrThrow(base.Loc.GetString("research-console-unlocks-list-entry-generic", (ValueTuple<string, object>)("text", base.Loc.GetString(generic.UnlockDescription))));
		}
		return description;
	}

	public bool IsTechnologyUnlocked(EntityUid uid, TechnologyPrototype technology, TechnologyDatabaseComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<TechnologyDatabaseComponent>(uid, ref component, true))
		{
			return IsTechnologyUnlocked(uid, technology.ID, component);
		}
		return false;
	}

	public bool IsTechnologyUnlocked(EntityUid uid, string technologyId, TechnologyDatabaseComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<TechnologyDatabaseComponent>(uid, ref component, false))
		{
			return component.UnlockedTechnologies.Contains(technologyId);
		}
		return false;
	}

	public void TrySetMainDiscipline(TechnologyPrototype prototype, EntityUid uid, TechnologyDatabaseComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<TechnologyDatabaseComponent>(uid, ref component, true))
		{
			TechDisciplinePrototype discipline = PrototypeManager.Index<TechDisciplinePrototype>(prototype.Discipline);
			if (prototype.Tier >= discipline.LockoutTier)
			{
				component.MainDiscipline = ProtoId<TechDisciplinePrototype>.op_Implicit(prototype.Discipline);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
				TechnologyDatabaseModifiedEvent ev = default(TechnologyDatabaseModifiedEvent);
				((EntitySystem)this).RaiseLocalEvent<TechnologyDatabaseModifiedEvent>(uid, ref ev, false);
			}
		}
	}

	public bool TryRemoveTechnology(Entity<TechnologyDatabaseComponent> entity, ProtoId<TechnologyPrototype> tech)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return TryRemoveTechnology(entity, PrototypeManager.Index<TechnologyPrototype>(tech));
	}

	public bool TryRemoveTechnology(Entity<TechnologyDatabaseComponent> entity, TechnologyPrototype tech)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (!entity.Comp.UnlockedTechnologies.Remove(tech.ID))
		{
			return false;
		}
		foreach (ProtoId<LatheRecipePrototype> recipe in tech.RecipeUnlocks)
		{
			bool hasTechElsewhere = false;
			foreach (string unlockedTech in entity.Comp.UnlockedTechnologies)
			{
				if (PrototypeManager.Index<TechnologyPrototype>(unlockedTech).RecipeUnlocks.Contains(recipe))
				{
					hasTechElsewhere = true;
					break;
				}
			}
			if (!hasTechElsewhere)
			{
				entity.Comp.UnlockedRecipes.Remove(ProtoId<LatheRecipePrototype>.op_Implicit(recipe));
			}
		}
		((EntitySystem)this).Dirty(Entity<TechnologyDatabaseComponent>.op_Implicit(entity), (IComponent)(object)entity.Comp, (MetaDataComponent)null);
		UpdateTechnologyCards(Entity<TechnologyDatabaseComponent>.op_Implicit(entity), Entity<TechnologyDatabaseComponent>.op_Implicit(entity));
		return true;
	}

	public void ClearTechs(EntityUid uid, TechnologyDatabaseComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<TechnologyDatabaseComponent>(uid, ref comp, true) && comp.UnlockedTechnologies.Count != 0)
		{
			comp.UnlockedTechnologies.Clear();
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	public void AddLatheRecipe(EntityUid uid, string recipe, TechnologyDatabaseComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<TechnologyDatabaseComponent>(uid, ref component, true) && !component.UnlockedRecipes.Contains(recipe))
		{
			component.UnlockedRecipes.Add(recipe);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			TechnologyDatabaseModifiedEvent ev = new TechnologyDatabaseModifiedEvent(new List<string> { recipe });
			((EntitySystem)this).RaiseLocalEvent<TechnologyDatabaseModifiedEvent>(uid, ref ev, false);
		}
	}
}
