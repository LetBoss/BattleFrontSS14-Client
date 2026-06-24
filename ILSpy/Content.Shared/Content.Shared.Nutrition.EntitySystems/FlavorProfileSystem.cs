using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.Components;
using Content.Shared.Nutrition.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class FlavorProfileSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IConfigurationManager _configManager;

	private const string BackupFlavorMessage = "flavor-profile-unknown";

	private int FlavorLimit => _configManager.GetCVar<int>(CCVars.FlavorLimit);

	public string GetLocalizedFlavorsMessage(EntityUid uid, EntityUid user, Solution solution, FlavorProfileComponent? flavorProfile = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FlavorProfileComponent>(uid, ref flavorProfile, false))
		{
			return base.Loc.GetString("flavor-profile-unknown");
		}
		HashSet<string> flavors = new HashSet<string>(flavorProfile.Flavors);
		flavors.UnionWith(GetFlavorsFromReagents(solution, FlavorLimit - flavors.Count, flavorProfile.IgnoreReagents));
		FlavorProfileModificationEvent ev = new FlavorProfileModificationEvent(user, flavors);
		((EntitySystem)this).RaiseLocalEvent<FlavorProfileModificationEvent>(ev);
		((EntitySystem)this).RaiseLocalEvent<FlavorProfileModificationEvent>(uid, ev, false);
		((EntitySystem)this).RaiseLocalEvent<FlavorProfileModificationEvent>(user, ev, false);
		return FlavorsToFlavorMessage(flavors);
	}

	public string GetLocalizedFlavorsMessage(EntityUid user, Solution solution)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		HashSet<string> flavors = GetFlavorsFromReagents(solution, FlavorLimit);
		FlavorProfileModificationEvent ev = new FlavorProfileModificationEvent(user, flavors);
		((EntitySystem)this).RaiseLocalEvent<FlavorProfileModificationEvent>(user, ev, true);
		return FlavorsToFlavorMessage(flavors);
	}

	private string FlavorsToFlavorMessage(HashSet<string> flavorSet)
	{
		List<FlavorPrototype> flavors = new List<FlavorPrototype>();
		FlavorPrototype flavorPrototype = default(FlavorPrototype);
		foreach (string flavor in flavorSet)
		{
			if (!string.IsNullOrEmpty(flavor) && _prototypeManager.TryIndex<FlavorPrototype>(flavor, ref flavorPrototype))
			{
				flavors.Add(flavorPrototype);
			}
		}
		flavors.Sort((FlavorPrototype a, FlavorPrototype b) => a.FlavorType.CompareTo(b.FlavorType));
		if (flavors.Count == 1 && !string.IsNullOrEmpty(flavors[0].FlavorDescription))
		{
			return base.Loc.GetString("flavor-profile", (ValueTuple<string, object>)("flavor", base.Loc.GetString(flavors[0].FlavorDescription)));
		}
		if (flavors.Count > 1)
		{
			string lastFlavor = base.Loc.GetString(flavors[flavors.Count - 1].FlavorDescription);
			string allFlavors = string.Join(", ", from i in flavors.GetRange(0, flavors.Count - 1)
				select base.Loc.GetString(i.FlavorDescription));
			return base.Loc.GetString("flavor-profile-multiple", (ValueTuple<string, object>)("flavors", allFlavors), (ValueTuple<string, object>)("lastFlavor", lastFlavor));
		}
		return base.Loc.GetString("flavor-profile-unknown");
	}

	private HashSet<string> GetFlavorsFromReagents(Solution solution, int desiredAmount, HashSet<string>? toIgnore = null)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		HashSet<string> flavors = new HashSet<string>();
		foreach (var (reagent, quantity) in solution.GetReagentPrototypes(_prototypeManager))
		{
			if (toIgnore == null || !toIgnore.Contains(reagent.ID))
			{
				if (flavors.Count == desiredAmount)
				{
					break;
				}
				if (!(quantity < reagent.FlavorMinimum) && reagent.Flavor.HasValue)
				{
					ProtoId<FlavorPrototype>? flavor = reagent.Flavor;
					flavors.Add(flavor.HasValue ? ProtoId<FlavorPrototype>.op_Implicit(flavor.GetValueOrDefault()) : null);
				}
			}
		}
		return flavors;
	}
}
